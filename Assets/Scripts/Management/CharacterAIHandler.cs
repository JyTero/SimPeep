using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;

public class CharacterAIHandler : ManagementCore
{

    [SerializeField]
    private int characterAIIdleTimer; //This will change to be character dependant
    [SerializeField, Tooltip("Temp. replacement for interaction spesific base scores")]
    private int interactionBaseScore;

    private List<CharacterAI> idleCharacters = new();
    private List<CharacterAI> tasklessCharacters = new();
    private Dictionary<Character, CharacterAI> activeCharacters = new();
    private Dictionary<Character, CharacterAI> occupiedCharacters = new();

    private Dictionary<Character, CharacterAI> charactersAIsByCharacter = new();
    public Dictionary<Character, CharacterAI> CharactersAIsByCharacter { get { return charactersAIsByCharacter; } }

    private List<Character> charactersToRemove = new();

    private Debuglandia debuglandia;


    protected override void Start()
    {
        base.Start();
        debuglandia = FindAnyObjectByType<Debuglandia>();
    }

    public void AddNewCharacter(Character character)
    {
        CharacterAI cai = new CharacterAI(character);
        idleCharacters.Add(cai);
        charactersAIsByCharacter.Add(character, cai);

    }

    public void RemoveCharacter(Character character)
    {

    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);

        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;


        RunDecisionMaking(deltaTime);
    }


    //INTERACTION SELECTION
    private void RunDecisionMaking(float deltaTime)
    {
        //Idle
        for (int i = idleCharacters.Count - 1; i >= 0; i--)
        {
            CharacterAI character = idleCharacters[i];

            ActiveInteraction queueInteraction = GetInteractionFromQueue(character);


            if (queueInteraction == null)
                continue;
            else
            {
                StartInteraction(queueInteraction, character);
            }
        }

        //SearchInteraction
        for (int i = tasklessCharacters.Count - 1; i >= 0; i--)
        {
            //Gather
            CharacterAI characterAI = tasklessCharacters[i];
            List<StoredInteraction> storedInteractions = lotManager.GetAllStoredInteractionsOnLot(characterAI.chara.ThisLot);

            //Convert
            List<ActiveInteraction> interactions = new();
            foreach (StoredInteraction storedInteraction in storedInteractions)
            {
                if (storedInteraction.InteractionTuningSO.HiddenInteraction)
                    continue;
                if (storedInteraction.InteractionTuningSO.InvalidInteraction)
                    continue;

                interactions.Add(NewActiveInteraction(characterAI.chara, storedInteraction));
            }
            //Validity
            //Score
            foreach (ActiveInteraction interaction in interactions)
            {
                float score = 0;
                float needBonus = 0;
                Character thisCharacter = interaction.ThisCharacter;
                //NeedScoring
                foreach (NeedType needTypeToWeight in interaction.NeedsToWeight)
                {
                    float needPercentage = (float)thisCharacter.Needs[needTypeToWeight].NeedValue / 100f;
                    float needBasedMultiplier = thisCharacter.Needs[needTypeToWeight].NeedWeightOnInteractionScoring.Evaluate(needPercentage);
                    needBonus += interactionBaseScore * needBasedMultiplier;

                }
                score = needBonus;
                //Trait scoring
                float traitBonus = 0;
                foreach (InteractionScoringModifier modifier in interaction.ScoringModifiers)
                {
                    if (characterAI.chara.Traits.Contains(modifier.TraitSO))
                    {
                        traitBonus += modifier.TraitBonus;
                    }
                }
                score += traitBonus;

                interaction.interactionScore = score;
            }
            if (debugLog)
                PrintInteractionScoring(interactions);

            //Pick best (/Random)
            //interactions = SortScoredInteractions(interactions);
            interactions.Sort((a, b) => b.interactionScore.CompareTo(a.interactionScore));

            QueueInteraction(interactions[i], InteractionQueuePriority.NormalAISelect);
            tasklessCharacters.Remove(characterAI);
            idleCharacters.Add(characterAI);
            //StartInteraction(interactions[i], characterAI);
        }
    }

    private ActiveInteraction GetInteractionFromQueue(CharacterAI character)
    {

        if (character.InteractionQueuesByPriority.Count == 0)
            return null;

        string S = $"Iterating: {character.chara.ItemName}\n";


        foreach (InteractionQueuePriority iqp in (InteractionQueuePriority[])Enum.GetValues(typeof(InteractionQueuePriority)))
        {
            S += $"Queue type {iqp.ToString()}\n";
            if (!character.InteractionQueuesByPriority.ContainsKey(iqp))
                continue;
            else
            {
                QueuedInteraction queueInteraction = character.InteractionQueuesByPriority[iqp][0];

                idleCharacters.Remove(character);
                //StartInteraction(queueInteraction.interaction, character);
                character.InteractionQueuesByPriority[iqp].Remove(queueInteraction);
                if (character.InteractionQueuesByPriority[iqp].Count == 0)
                    character.ClearQueuePart(iqp);
                //TODO: QUEUE CLEANUP (REMOVE DONE ACTION,
                if (debugLog)
                    Debug.Log(S);

                UIController.RefreshInteractionQueueData(character);
                return queueInteraction.interaction;
            }
        }
        if (debugLog)
            Debug.Log(S);
        return null;
    }

    public void QueueInteraction(ActiveInteraction interaction, InteractionQueuePriority queuePriority)
    {
        //Handle queueing interaction prio / source 

        charactersAIsByCharacter[interaction.ThisCharacter].QueueNewInteraction(interaction, queuePriority);

        UIController.RefreshInteractionQueueData(charactersAIsByCharacter[interaction.ThisCharacter]);
        //DisplayInteractionQueue(charactersAIsByCharacter[interaction.ThisCharacter]);

    }

    public void AtDestination(Character character)
    {
        CharacterAI cai = activeCharacters[character];
        cai.CurrentInteraction.PopInteractionState();

        //OLD
        //if (cai != null)
        //{
        //    if (cai.CurrentSubInteraction != null)
        //        cai.CurrentSubInteraction.SetInteractionStateOLD(EInteractionState.AtDestination); //Refere to current interaction, even if sub (cant use CAI.CurrentInteraction for subs)
        //    else
        //        cai.CurrentInteraction.SetInteractionStateOLD(EInteractionState.AtDestination);
        //}
        //else
        //    Debug.LogError("Unhandeled AtDestination");
    }

    private void StartInteraction(ActiveInteraction interaction, CharacterAI charaAI)
    {
        if (interaction.InteractionGroupSO)
        {
            List<GroupedInteraction> validInteractions = new();
            //Choose among the grouped interactions
            foreach (GroupedInteraction groupedInteraction in interaction.InteractionGroupSO.GroupedInteractions)
            {
                if (groupedInteraction.RequiredItemInstructionSOs.Count == 0)
                {
                    validInteractions.Add(groupedInteraction);
                    continue;
                }
                foreach (Item_InstructionSO itemInstructionSO in groupedInteraction.RequiredItemInstructionSOs)
                {
                    if(instructionEngine.CanItemInstructionRun(itemInstructionSO, charaAI.chara))
                    {
                        validInteractions.Add(groupedInteraction);
                        continue;
                    }

                }
            }
            List<int> scores = new();
            foreach(GroupedInteraction groupedInteraction in validInteractions)
            {
                scores.Add(groupedInteraction.BasePreferenceScore);
            }
            int highestValue = scores.Max();
            int maxIndex = scores.IndexOf(highestValue);

            GroupedInteraction chosenG = validInteractions[maxIndex];

            ActiveInteraction chosen = NewActiveInteraction(charaAI.chara, new StoredInteraction(chosenG.InteractionSO, interaction.InteractionSource));
            charaAI.NewCurrentInteraction(chosen);
            interactionEngine.StartNewInteraction(chosen);
        }
        else
        {
        charaAI.NewCurrentInteraction(interaction);
        interactionEngine.StartNewInteraction(interaction);
        }



        activeCharacters.Add(charaAI.chara, charaAI);
        UIController.RefreshCurrentInteractionData(charaAI.CurrentInteraction.InteractionName);

    }

    private void PrintInteractionScoring(List<ActiveInteraction> interactions)
    {
        string s = $"{interactions[0].ThisCharacter.ItemName}'s Scoring Result:\n";
        foreach (ActiveInteraction interaction in interactions)
        {
            s += $"Interaction '{interaction.InteractionName}' (Item: {interaction.InteractionSource.ItemName} scored: {interaction.interactionScore})\n";
        }
        Debug.Log(s);
    }

    public void OnInteractionEnd(Character character, ActiveInteraction interaction)
    {
        CharacterAI charaAI = activeCharacters[character];
        activeCharacters.Remove(character);
        idleCharacters.Add(charaAI);

        //Suggested FollowUp
        if (interaction.FollowupInteractionSOs.Count != 0)
        {
            List<StoredInteraction> storedInteractions = lotManager.GetAllInteractionsOnLot(charaAI.chara.ThisLot);
            StoredInteraction si = null;

            foreach (InteractionSO intso in interaction.FollowupInteractionSOs)
            {
                //Carried item takes prio
                if (character.CarriedItem != null)
                {
                    foreach (StoredInteraction storedInteraction in character.CarriedItem.StoredInteractions)
                    {
                        if (storedInteraction.InteractionTuningSO == intso)
                        {
                            si = storedInteraction;
                            break;
                        }
                    }
                }
                //Find nearby interaction of that type
                else
                    si = lotManager.FindSuitableStoredInteractionOnLot(intso, character.ThisLot);

                QueueInteraction(NewActiveInteraction(character, si), InteractionQueuePriority.SuggestedFollowup);
                break;

            }
        }
        if (charactersAIsByCharacter[character].InteractionQueue.Count == 1)    //1, bc List<QueuedInteraction> interactionQueue doesn't 
            UIController.RefreshCurrentInteractionData("");                     //know when interaction has ended

    }


    //MISC
    public void FindAvailableChairAtTable(Character character)
    {
        List<ItemBase> tuckableChairsAttachedToTables = new();

        foreach (ItemBase item in character.ThisLot.ItemsOnLot)
        {
            if (item.Capabilites.Contains(ItemCapabilites.TuckableChairCapability))
            {
                TuckableChair_Capability tcc = item.CapabilitiesByEnum[ItemCapabilites.TuckableChairCapability] as TuckableChair_Capability;
                if (tcc.Table == null)
                    continue;
                else
                {
                    tuckableChairsAttachedToTables.Add(item);
                }
            }

        }
    }

    public SeatingWithTableData FindSeatWithTable(Character character)
    {
        Dictionary<ItemBase, List<Item_Slot>> diningTablesAndSlots = new();
        foreach (ItemBase item in character.ThisLot.ItemsOnLot) // <-- Name of DiningTable_ItemSO
        {
            if (item.ItemData.ItemName != "Eatin' table")
                continue;

            List<Item_Slot> slots = new();
            foreach (Item_Slot slot in item.ItemSlotsOnItem)
            {
                if (slot.SlotType.name == "DiningChairSlot") // DiningChairSlot is the asset name
                    slots.Add(slot);
            }
            diningTablesAndSlots.Add(item, slots);
        }

        //TODO: Get the nearest one

        ItemBase rChair = null;
        ItemBase rTable = null;
        Item_Slot rSlot = null;
        foreach (var pair in diningTablesAndSlots)
        {
            rTable = pair.Key;
            List<Item_Slot> chairSlots = pair.Value;
            DiningTable_Capability dtc = rTable.GetComponent<DiningTable_Capability>();
            if (!dtc)
                continue;
            foreach (Item_Slot chairSlot in chairSlots)
            {
                if (!chairSlot.ItemInSlot)
                    continue;
                if (chairSlot.ItemInSlot.ItemData.name != "DiningChair_ItemSO")
                    continue;

                Item_Slot onTableSlot = dtc.TableSlotsByChairSlot[chairSlot];
                if (onTableSlot.IsEmpty())
                {
                    rSlot = onTableSlot;
                    rChair = chairSlot.ItemInSlot;
                    break;
                }

            }
            if (rChair != null)
                break;
        }
        //AllFound!
        //table, chairSlot, onTableSlot
        return new SeatingWithTableData(rTable, rChair, rSlot);


    }
}
