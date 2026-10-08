using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

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
        character.SetCharacterAI(cai);

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
            {
                //Waited Characters
                if (waitedCharacters.Contains(character.chara))
                {
                    WaitedInteractableDeclaredAvailable(character.chara);
                    waitedCharacters.Remove(character.chara);
                }
                continue;
            }
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
                if (storedInteraction.InteractionData.HiddenInteraction)
                    continue;
                if (storedInteraction.InteractionData.InvalidInteraction)
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
                    if (instructionEngine.CanItemInstructionRun(itemInstructionSO, charaAI.chara))
                    {
                        validInteractions.Add(groupedInteraction);
                        continue;
                    }

                }
            }
            List<int> scores = new();
            foreach (GroupedInteraction groupedInteraction in validInteractions)
            {
                //Scoring
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


        charaAI.chara.IsIdle = false;
        activeCharacters.Add(charaAI.chara, charaAI);
        UIController.RefreshCurrentInteractionData(charaAI.CurrentInteraction.InteractionName, interaction);

    }

    public void CancelInteraction(Character character)
    {
        interactionEngine.CancelInteraction(activeCharacters[character]);
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
                        if (storedInteraction.InteractionSO == intso)
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
        if (charactersAIsByCharacter[character].InteractionQueue.Count == 1)
        {                                                                           //1, bc List<QueuedInteraction> interactionQueue doesn't 
            UIController.RefreshCurrentInteractionData("", interaction);            //know when interaction has ended
            charaAI.NewCurrentInteraction(null);
            character.IsIdle = true;
        }
    }

    public void SubscribeToKnowWhenItemAvailable(Character character)
    {
        waitedCharacters.Add(character);
    }
    private List<Character> waitedCharacters = new();


    public void ContinueSocialResponse(Character socialResponder)
    {
        //Pop two states to get to running state
        CharacterAI responderAI = charactersAIsByCharacter[socialResponder];
        ActiveInteraction responseInteraction = responderAI.CurrentInteraction;

        responseInteraction.PopInteractionState();
        responseInteraction.PopInteractionState();

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

    //WAITING SYSTEM

    private Dictionary<Interactable, WaitingCharacter> waitingCharactersByTargets = new();
    private Dictionary<Character, WaitingCharacter> waitingCharactersByCharacter = new();

    public void RegisterToWait(Character character, EWaitReason reason, Interactable target)
    {
        WaitingCharacter waitingCharacter = new(character, reason, target);

        if (waitingCharactersByTargets.ContainsKey(target))
        {
            WaitingCharacter targetWaitingCharacter = waitingCharactersByTargets[target];
            HandleWaitCircle(waitingCharacter, targetWaitingCharacter);
            waitingCharactersByTargets.Remove(target);
            return;
        }

        switch (reason)
        {
            case EWaitReason.Default:
                Debug.LogError($"Character waiting with unexpected cicumstances {character.ItemName} | {reason} | {target.ItemName}");
                break;
            case EWaitReason.WaitForSocialInteractionPartner:
                break;
            case EWaitReason.WaitForItemToBeAvailable:
                RegisterToWaitForItemToBeAvailable(target);
                break;
            default:
                Debug.LogError($"Character waiting with unexpected cicumstances {character.ItemName} | {reason} | {target.ItemName}");
                break;
        }


        waitingCharactersByTargets.Add(target, waitingCharacter);
        waitingCharactersByCharacter.Add(character, waitingCharacter);
    }

    private void RegisterToWaitForItemToBeAvailable(Interactable target)
    {
        if (target is ItemBase) //Currently no checks, always subscibes the same method, unsubscibing is ??
            itemManager.SubscribeToKnowWhenItemAvailable(target as ItemBase);
        else if (target is Character)
            SubscribeToKnowWhenItemAvailable(target as Character);
    }

    private void HandleWaitCircle(WaitingCharacter waitingCharacter, WaitingCharacter targetWaitingCharacter)
    {
        //Two characters are waiting for eachother.

        if (waitingCharacter.WaitReason == EWaitReason.WaitForSocialInteractionPartner
            && targetWaitingCharacter.WaitReason == EWaitReason.WaitForSocialInteractionPartner)
        {
            //Verify next to eachtoer
            if (!lotManager.NeighboringTileContainsInteractable(waitingCharacter.Character.CurrentTile, targetWaitingCharacter.Character))
            {
                //IF not, move route waitingCharacter
                //Stop waiting, all that stuff
                QuitWaiting(waitingCharacter);
                characterControl.RouteToTile(waitingCharacter.Character, lotManager.GetLotTile(targetWaitingCharacter.Character.InteractionSlot.SlotTransform.position));
                return;

            }
            else
            {
                //Begin social interaction
                QuitWaiting(waitingCharacter);
                QuitWaiting(targetWaitingCharacter);
            }
        }
        Debug.LogError("Two character wait on eachother with different reasons! (Not implemented)");
        //If one waits to get in pos, other waits for target to be free
        // InteractionPrio, player chosen interaction wins
        // Otherwise the one waiting target to get in pos wins (Furhter in the process).

        //ProcedeWithInteraction(WinningWaitingCharacter);
    }

    public void WaitedInteractableDeclaredAvailable(Interactable availableInteractable)
    {
        //When an earlier subscribed to item becomes available this is called

        WaitingCharacter waitingCharacter = waitingCharactersByTargets[availableInteractable];

        ProcedeWithInteraction(waitingCharacter);
    }

    private void ProcedeWithInteraction(WaitingCharacter waitingCharacter)
    {
        charactersAIsByCharacter[waitingCharacter.Character].CurrentInteraction.PopInteractionState();
        //throw new NotImplementedException();
    }


    public void QuitWaiting(WaitingCharacter wCharacter)
    {
        QuitWaiting(wCharacter.Character);
    }
    public void QuitWaiting(Character character)
    {
        //TBD
        // throw new NotImplementedException();

        WaitingCharacter wCharacter = waitingCharactersByTargets[character];
        waitingCharactersByTargets.Remove(wCharacter.WaitTarget);
        waitingCharactersByCharacter.Remove(character);
    }

}


