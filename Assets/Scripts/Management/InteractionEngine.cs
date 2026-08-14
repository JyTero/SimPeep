using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class InteractionEngine : ManagementCore
{
    [SerializeField]
    private GameObject plateRawFood;

    private List<ActiveInteraction> activeInteractions = new();

    private Dictionary<Character, ActiveInteraction> waitingInteractionsByWaitee = new();

    protected override void Start()
    {
        base.Start();
    }

    public void StartNewInteraction(ActiveInteraction interaction)
    {
        interaction.interactionState = InteractionState.Starting;

        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} started interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
        activeInteractions.Add(interaction);

        //TODO: Check that intraction is ready to start (other character available, item has free slot (BONUS: When to wait wait vs fail)
        //if(IsTargetAvailable())
        //  Start

        if (interaction.InteractionSource is Character)
        {
            //Cue "ResponceInteraction to given SocialInteraction" (Via AIHandler)
            //On the other one, if this is reaction, alert system to continue
            if (interaction.IsReaction)
            {
                RouteToInteraction(interaction);
            }
            else
            {
                ActiveInteraction responce = NewActiveInteraction(interaction.InteractionSource as Character, new StoredInteraction(interaction.InteractionTuningSO.SocialResponceInteractions[0], interaction.ThisCharacter));
                characterAIHandler.QueueInteraction(responce, InteractionQueuePriority.UserSelectNPCReaction);
                //  RouteToInteraction(interaction);
            }

        }
        else
            //Route
            RouteToInteraction(interaction);

    }

    private void RouteToInteraction(ActiveInteraction interaction)
    {
        interaction.interactionState = InteractionState.Moving;
        CharacterControl.StartRouting(interaction);
    }

    public void OnInteractionDestinationArrival(ActiveInteraction interaction)
    {
        //Later, make more complicated for multi step interactions (They shall be a "container of interactions"

        //Queue Social Interactions to wait, router initialises on arrival
        if (interaction.InteractionSource is Character)
        {
            if (interaction.IsReaction)
                interaction.interactionState = InteractionState.Waiting;
            else
            {
                //Run interaction on both parties from the same orderr
                interaction.interactionState = InteractionState.Running;
                SendOnInteractionBeginInstructions(interaction);

                //Social Responce interaction handling
                waitingInteractionsByWaitee[interaction.InteractionSource as Character].interactionState = InteractionState.Running;
                SendOnInteractionBeginInstructions(waitingInteractionsByWaitee[interaction.InteractionSource as Character]);
            }

        }
        else
        {
            interaction.interactionState = InteractionState.Running;
            SendOnInteractionBeginInstructions(interaction);
        }
    }

    private void SendOnInteractionBeginInstructions(ActiveInteraction interaction)
    {
        //TBD
    }

    private void RegisterToWait(ActiveInteraction interaction)
    {
        if (waitingInteractionsByWaitee.ContainsKey(interaction.ThisCharacter))
            return;

        waitingInteractionsByWaitee.Add(interaction.ThisCharacter, interaction);
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;

        InteractionUpdate(deltaTime);
    }

    private void InteractionUpdate(float deltaTime)
    {

        for (int i = activeInteractions.Count - 1; i >= 0; i--)
        {
            ActiveInteraction interaction = activeInteractions[i];
            switch (interaction.interactionState)
            {
                case InteractionState.Starting:
                    break;
                case InteractionState.Moving:
                    break;
                case InteractionState.AtDestination:
                    OnInteractionDestinationArrival(interaction);
                    break;
                case InteractionState.Waiting:
                    RegisterToWait(interaction);
                    break;
                case InteractionState.Running:
                    InteractionOnTick(interaction, deltaTime);
                    break;
                case InteractionState.Ending:
                    EndInteraction(interaction);
                    break;
                case InteractionState.Default:
                    break;
            }

            if (InteractionShouldEnd(interaction))
            {
                interaction.interactionState = InteractionState.Ending;

            }

        }
    }

    private void InteractionOnTick(ActiveInteraction interaction, float dt)
    {
        //OnTick Instructions
        if (interaction.TimeSinceLastInstructionsSent > OneUnitOfTime)
        {
            bool tooMuchTime = true;
            while (tooMuchTime)
            {
                interaction.TimeSinceLastInstructionsSent -= OneUnitOfTime;
                List<Need_Instruction> needInstructions = new();
                foreach (Need_InstructionSO niso in interaction.InteractionTuningSO.Need_InteractionInstructions)
                {
                    Need_Instruction ni = new(niso, interaction.ThisCharacter);
                    needInstructions.Add(ni);
                }
                needsEngine.NewInstructions(needInstructions);

                if (interaction.TimeSinceLastInstructionsSent < OneUnitOfTime)
                    tooMuchTime = false;
            }
        }
        else
            interaction.TimeSinceLastInstructionsSent += dt;
    }
    private bool InteractionShouldEnd(ActiveInteraction interaction)
    {
        switch (interaction.InteractionEndingType)

        {
            case InteractionEndingType.Default:
                return true;
            case InteractionEndingType.SetTime:
                interaction.interactionLenghtAccumulation += deltaTime + updateInterval;
                if (interaction.interactionLenghtAccumulation > interaction.InteractionLength)
                {
                    return true;
                }
                else
                    return false;
            case InteractionEndingType.UntillNeedAtValue:
                if (interaction.ThisCharacter.Needs[interaction.InteractionEndingTargetNeedType].NeedValue >= interaction.InteractionEndingTargetNeedValue)
                    return true;
                else
                    return false;
        }
        return true;
    }
    private List<Character> charactersWaitingItemSpawn = new();

    private void EndInteraction(ActiveInteraction interaction)
    {
        if (!interaction.allInstructionsDone)
        {
            SendOnInteractionEndInstructions(interaction);
            return;
        }
        //if(charactersWaitingItemSpawn.Contains(interaction.ThisCharacter)
        //Return


        //SubInteractions
        if (interaction.isSubinteraction)
        {
            CharacterAI charaAI = characterAIHandler.CharactersAIsByCharacter[interaction.ThisCharacter];
            //Begin next subinteraction
            if (charaAI.SubInteractionQueue.Count > 0)
            {
                StartSubInteraction(charaAI.SubInteractionQueue[0]);
                return;
            }
            else //OR finish parent
            {
                interaction.parentInteraction.subInteractionsHaveRan = true;
                EndInteraction(interaction.parentInteraction);
            }
        }
        else if (interaction.InteractionTuningSO.SubInteractionSOs.Count != 0 && !interaction.subInteractionsHaveRan)
        {
            HandleSubInteractions(interaction);
            return;
        }

        activeInteractions.Remove(interaction);
        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} finished interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");

        characterAIHandler.OnInteractionEnd(interaction.ThisCharacter, interaction);
    }


    private void SendOnInteractionEndInstructions(ActiveInteraction interaction)
    {
        //Relationships
        //TODO: Make RelationshipsEngine, move to there
        foreach (Relationship_InstructionSO relso in interaction.InteractionTuningSO.RelationshipChangeInstructions)
        {
            Character thisCharacter = interaction.ThisCharacter;
            Character targetCharacter = interaction.InteractionSource as Character;
            if (relationshipEngine.HasExistingRelationship(thisCharacter, targetCharacter))
            {
                relationshipEngine.AdjustRelationship(thisCharacter, targetCharacter, relso.RelationshipScoreChange);
            }
            else
            {
                relationshipEngine.NewRelationship(thisCharacter, targetCharacter);
                relationshipEngine.AdjustRelationship(thisCharacter, targetCharacter, relso.RelationshipScoreChange);
            }


            if (IsDebug)
                Debug.Log($"Relations!({interaction.ThisCharacter.ItemName} towards {interaction.InteractionSource.ItemName})");
        }

        //Item, Also should be handled elsewhere
        Iteminstructions(interaction);
        interaction.allInstructionsDone = true;
    }

    private void Iteminstructions(ActiveInteraction interaction)
    {
        foreach (Item_InstructionSO itemInstruction in interaction.InteractionTuningSO.ItemChangeInstructions)
        {
            Character thisCharacter = interaction.ThisCharacter;
            ItemBase thisItem = interaction.InteractionSource as ItemBase;
            if (itemInstruction.SpawnItem)
            {
                //HandleItemSPawning
                ItemBase newItem = itemManager.SpawnNewItem(itemInstruction.ItemToSpawn, thisCharacter.ThisLot);
                newItem.gameObject.transform.position = thisItem.transform.position;
                //Spawned items appear "between farmes" (fixed update or smth), this should cause a frame of waiting for the character to have item ready
                continue;
            }
            else if (itemInstruction.MoveThisItem)
            {
                switch (itemInstruction.WhereToMoveItem)
                {
                    case ItemLocation.Default:
                        break;
                    case ItemLocation.LotSpace:
                        break;
                    case ItemLocation.WorldSpace:
                        break;
                    case ItemLocation.InCharactacter:
                        break;
                    case ItemLocation.OnCharacter:
                        CharacterControl.PickupItem(thisCharacter, thisItem);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void HandleSubInteractions(ActiveInteraction interaction)
    {
        List<ActiveInteraction> subInteractions = new();
        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[interaction.ThisCharacter];

        int i = 0;
        foreach (StoredInteraction subInteraction in interaction.subInteractions)
        {
            //If null, interaction couldnt be found, try again now | GUMMY
            StoredInteraction retrySub;
            if (subInteraction == null)
            {
                retrySub = lotManager.FindSuitableStoredInteractionOnLot(interaction.InteractionTuningSO.SubInteractionSOs[i], interaction.ThisCharacter.ThisLot);
                if (retrySub != null)
                {
                    ActiveInteraction activeSubInteraction = NewActiveInteraction(interaction.ThisCharacter, retrySub);
                    activeSubInteraction.MakeIntoSubInteraction(interaction);
                    subInteractions.Add(activeSubInteraction);
                    thisCharaAI.AddSubInterations(subInteractions);
                }
            }
            else
            {
                ActiveInteraction activeSubInteraction = NewActiveInteraction(interaction.ThisCharacter, subInteraction);
                activeSubInteraction.MakeIntoSubInteraction(interaction);
                subInteractions.Add(activeSubInteraction);
                thisCharaAI.AddSubInterations(subInteractions);
            }

            i++;
        }
        StartSubInteraction(thisCharaAI.SubInteractionQueue[0]);
        thisCharaAI.RemoveSubInteraction(thisCharaAI.SubInteractionQueue[0]);
    }
    private void StartSubInteraction(ActiveInteraction interaction)
    {
        StartNewInteraction(interaction);

    }
}