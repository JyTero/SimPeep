using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class InteractionEngine : ManagementCore
{
    private List<ActiveInteraction> activeInteractions = new();

    private Dictionary<Character, ActiveInteraction> waitingInteractionsByWaitee = new();

    protected override void Start()
    {
        base.Start();
    }

    public void StartNewInteraction(ActiveInteraction interaction)
    {
        interaction.SetInteractionState(InteractionState.Starting);

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
        if (interaction.InteractionTuningSO.SkipMovement)
            interaction.SetInteractionState(InteractionState.AtDestination);
        else if (interaction.InteractionSource as ItemBase == interaction.ThisCharacter.CarriedItem)
            interaction.SetInteractionState(InteractionState.AtDestination);

        else
        {
            interaction.SetInteractionState(InteractionState.Moving);
            CharacterControl.StartRouting(interaction);
        }
    }

    public void OnInteractionDestinationArrival(ActiveInteraction interaction)
    {
        //Queue Social Interactions to wait, router initialises on arrival
        if (interaction.InteractionSource is Character)
        {
            if (interaction.IsReaction)
                interaction.SetInteractionState(InteractionState.Waiting);
            else
            {
                //Run interaction on both parties from the same orderr
                interaction.SetInteractionState(InteractionState.Running);
                SendOnInteractionBeginInstructions(interaction);

                //Social Responce interaction handling
                waitingInteractionsByWaitee[interaction.InteractionSource as Character].SetInteractionState(InteractionState.Running);
                SendOnInteractionBeginInstructions(waitingInteractionsByWaitee[interaction.InteractionSource as Character]);
            }

        }
        else
        {
            interaction.SetInteractionState(InteractionState.Running);
            SendOnInteractionBeginInstructions(interaction);
        }
    }

    private void SendOnInteractionBeginInstructions(ActiveInteraction interaction)
    {
        List<ItemCapability> itemCapabilities = new();
        if (interaction.InteractionTuningSO.RequiredItemCapabilities.Count > 0)
        {
            foreach (ItemCapabilites itemCapability in interaction.InteractionTuningSO.RequiredItemCapabilities)
            {
                ItemBase item = interaction.InteractionSource as ItemBase;
                if (item.Capabilites.Contains(itemCapability))
                    itemCapabilities.Add(item.CapabilitiesByEnum[itemCapability]);
            }
        }
        HandleItemCapabilities(itemCapabilities, interaction);
    }

    private void HandleItemCapabilities(List<ItemCapability> itemCapabilities, ActiveInteraction interaction)
    {
        foreach (ItemCapability itemCapability in itemCapabilities)
        {
            //Needs
            List<Need_Instruction> needInstructions = new();
            foreach (Need_InstructionSO needInstructionSO in itemCapability.OnBeginNeed_InteractionInstructions)
            {
                needInstructions.Add(new Need_Instruction(needInstructionSO, interaction.ThisCharacter));
            }
            needsEngine.NewInstructions(needInstructions);

            //Relationships
            List<RelationshipChange_Instruction> relChangeInstructions = new();
            foreach (Relationship_InstructionSO relso in itemCapability.OnBeginRelationshipChangeInstructions)
            {
                relChangeInstructions.Add(new RelationshipChange_Instruction(relso, interaction.ThisCharacter, interaction.InteractionSource as Character));
            }
            relationshipEngine.HandleRelationshipInstructions(relChangeInstructions);

            //ItemInstructions
            List<Item_Instruction> itemInstructions = new();
            foreach (Item_InstructionSO itemInstructionSO in itemCapability.OnBeginItemChangeInstructions)
            {
                itemInstructions.Add(new Item_Instruction(itemInstructionSO, interaction.ThisCharacter, interaction.InteractionSource as ItemBase));

            }
            itemManager.HandleItemInstructions(itemInstructions);

            //CapabilityLogic
            capabilityHandler.HandleCapability(itemCapability, interaction.ThisCharacter);

            //SubInteractions
            HandleSubInteractions(itemCapability.OnBeginSubInteractionSOs, interaction);
        }

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
            //if (IsDebug)
            //    Debug.Log($"Interaction {interaction.InteractionName} is in state {interaction.InteractionState}");
            switch (interaction.InteractionState)
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
                case InteractionState.SubInteractions:
                    RunningSubInteractions(interaction);
                    break;
                case InteractionState.Default:
                    break;
            }
            if (InteractionShouldEnd(interaction))
            {
                interaction.SetInteractionState(InteractionState.Ending);

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
        if (interaction.InteractionState == InteractionState.SubInteractions)
            return false;
        if (interaction.InteractionState == InteractionState.Moving)
            return false;
        if (interaction.InteractionState == InteractionState.Ending)
            return false;


        switch (interaction.InteractionEndingType)
        {
            case InteractionEndingType.Default:
                return false;
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
                activeInteractions.Remove(interaction);
                StartSubInteraction(charaAI.SubInteractionQueue[0]);
                return;
            }
            else //OR finish parent
            {
                interaction.parentInteraction.subInteractionsHaveRan = true;
                charaAI.NewCurrentSubInteraction(null);
                //interaction.parentInteraction.SetInteractionState(InteractionState.Ending);
                //EndInteraction(interaction.parentInteraction);
                activeInteractions.Remove(interaction);
                //return;
            }
        }
        else if (interaction.InteractionTuningSO.SubInteractions.Count != 0 && !interaction.subInteractionsHaveRan)
        {
            HandleSubInteractions(interaction.subInteractions, interaction);
            return;
        }

        activeInteractions.Remove(interaction);

        if (!interaction.isSubinteraction)
            characterAIHandler.OnInteractionEnd(interaction.ThisCharacter, interaction);

        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} finished interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");

    }

    private void RunningSubInteractions(ActiveInteraction interaction)
    {
        if (interaction.subInteractionsHaveRan)
            interaction.SetInteractionState(InteractionState.Ending);
    }

    private void SendOnInteractionEndInstructions(ActiveInteraction interaction)
    {
        //Relationships
        List<RelationshipChange_Instruction> relChangeInstructions = new();
        foreach (Relationship_InstructionSO relso in interaction.InteractionTuningSO.RelationshipChangeInstructions)
        {
            relChangeInstructions.Add(new RelationshipChange_Instruction(relso, interaction.ThisCharacter, interaction.InteractionSource as Character));
        }
        relationshipEngine.HandleRelationshipInstructions(relChangeInstructions);

        //Item, Also should be handled elsewhere
        Iteminstructions(interaction);
        interaction.allInstructionsDone = true;
    }

    private void Iteminstructions(ActiveInteraction interaction)
    {
        List<Item_Instruction> itemInstructions = new();
        foreach (Item_InstructionSO itemInstructionSO in interaction.InteractionTuningSO.ItemChangeInstructions)
        {
            itemInstructions.Add(new Item_Instruction(itemInstructionSO, interaction.ThisCharacter, interaction.InteractionSource as ItemBase));

        }
        itemManager.HandleItemInstructions(itemInstructions);
    }

    private void HandleSubInteractions(List<SubInteraction> subInteractions, ActiveInteraction mainInteraction)
    {
        if (subInteractions.Count == 0)
            return;

        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[mainInteraction.ThisCharacter];

        mainInteraction.SetInteractionState(InteractionState.SubInteractions);
        int i = 0;
        foreach (SubInteraction subInteraction in subInteractions)
        {
            StoredInteraction retrySub;

            //Item created by this interaction
            if (subInteraction.InteractionOnCreatedObject)
            {
                //No stored interaction found, item might not be ready? (Check order of initialise new item and this running)

                retrySub = itemManager.GetInteractionOnInteractable(subInteraction.InteractionSO, itemManager.GetItemCreatedByInteraction(mainInteraction.ThisCharacter));
                //MakeSubinteractionActive(mainInteraction, retrySub);
                ActiveInteraction activeSubInteraction = NewActiveInteraction(mainInteraction.ThisCharacter, retrySub);
                activeSubInteraction.MakeIntoSubInteraction(mainInteraction);
                thisCharaAI.AddSubInteration(activeSubInteraction);
            }
            //Item carried by user
            else if(subInteraction.InteractionOnItemHeldObject){

            }
            //Item on a main item slot
            else if (subInteraction.InteractionOnItemHeldObject)
            {
                foreach(Item_Slot slot in (mainInteraction.InteractionSource as ItemBase).ItemSlotsOnItem)
                {
                    if (slot.ItemInSlot == null)
                        continue;

                    retrySub = itemManager.GetInteractionOnInteractable(subInteraction.InteractionSO, slot.ItemInSlot);
                    if (retrySub == null)
                        continue;
                    //else
                        //the thingy
                }
            }
            //Any pre-existing world Item
            else
            {
                retrySub = lotManager.FindSuitableStoredInteractionOnLot(subInteraction.InteractionSO, mainInteraction.ThisCharacter.ThisLot);
                ActiveInteraction activeSubInteraction = NewActiveInteraction(mainInteraction.ThisCharacter, retrySub);
                activeSubInteraction.MakeIntoSubInteraction(mainInteraction);
                thisCharaAI.AddSubInteration(activeSubInteraction);
            }

            i++;
        }
        StartSubInteraction(thisCharaAI.SubInteractionQueue[0]);
    }
    private void MakeSubinteractionActive(ActiveInteraction mainInteraction, StoredInteraction subInteraction)
    {
        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[mainInteraction.ThisCharacter];
        ActiveInteraction activeSubInteraction = NewActiveInteraction(mainInteraction.ThisCharacter, subInteraction);
        activeSubInteraction.MakeIntoSubInteraction(mainInteraction);
        thisCharaAI.AddSubInteration(activeSubInteraction);
    }
    private void StartSubInteraction(ActiveInteraction subInteraction)
    {
        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[subInteraction.parentInteraction.ThisCharacter];
        thisCharaAI.RemoveSubInteraction(thisCharaAI.SubInteractionQueue[0]);
        thisCharaAI.NewCurrentSubInteraction(subInteraction);
        StartNewInteraction(subInteraction);

    }
}