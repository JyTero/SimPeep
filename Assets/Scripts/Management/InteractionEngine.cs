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

        if (debugLog)
            Debug.Log($"{interaction.ThisCharacter.ItemName} started interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
        interaction.PushInteractionState(EInteractionState.Default);
        activeInteractions.Add(interaction);
        PrepareInteractionInstructions(interaction);
        return;

    }

    private void PrepareInteractionInstructions(ActiveInteraction interaction)
    {
        interaction.PushInteractionState(EInteractionState.Ending);
        SetStateInstructions(interaction, interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionEnd,
     interaction.InteractionTuningSO.RelationshipChangeInstructionsOnInteractionEnd,
     interaction.InteractionTuningSO.ItemChangeInstructionSOsOnInteractionEnd,
     interaction.InteractionTuningSO.CharacterInstructionSOsOnInteractionEnd);
        

        interaction.PushInteractionState(EInteractionState.Running);
        interaction.State.stateNeedInstructionSOs = interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionTick;

        interaction.PushInteractionState(EInteractionState.Starting);
        SetStateInstructions(interaction, interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionBegin,
    interaction.InteractionTuningSO.RelationshipChangeInstructionsOnInteraction,
    interaction.InteractionTuningSO.ItemChangeInstructionSOsOnInteractionBegin,
    interaction.InteractionTuningSO.CharacterInstructionSOsOnInteractionBegin);
    }

    //private void InteractionRouting(ActiveInteraction interaction, LotGridTile destinationTile)
    //{
    //    interaction.PushInteractionState(EInteractionState.Moving);
    //    characterControl.RouteToTile(interaction.ThisCharacter, destinationTile);
    //}

    //private void RouteToInteraction(ActiveInteraction interaction)
    //{
    //    if (interaction.InteractionTuningSO.SkipMovement)
    //        interaction.SetInteractionStateOLD(EInteractionState.AtDestination);
    //    else if (interaction.InteractionSource as ItemBase == interaction.ThisCharacter.CarriedItem && !interaction.InteractionTuningSO.InteractionDestinationDifferentFromSource)
    //        interaction.SetInteractionStateOLD(EInteractionState.AtDestination);

    //    else
    //    {
    //        interaction.SetInteractionStateOLD(EInteractionState.Moving);
    //        characterControl.StartRouting(interaction);
    //    }
    //}

    public void OnInteractionDestinationArrival(ActiveInteraction interaction)
    {
        interaction.PopInteractionState();
    }

    private void SetStateInstructions(ActiveInteraction interaction, List<Need_InstructionSO> needInstructionSOs, List<Relationship_InstructionSO> relationshipInstructionSOs, List<Item_InstructionSO> itemInstructionSOs, List<Character_InstructionSO> characterInstructionSOs)
    {
        //NeedInstructions
        interaction.State.stateNeedInstructionSOs = needInstructionSOs;
        //RelationInstructions
        //ItemInstructions
        List<Item_InstructionData> item_InstructionDatas = new();
        foreach (Item_InstructionSO iiSO in itemInstructionSOs)
            item_InstructionDatas.Add(ItemInstructionSOToData(iiSO));
        interaction.State.stateItemInstructionDatas = item_InstructionDatas;
        //Character
        interaction.State.stateCharacterInstructionSOs = characterInstructionSOs;

        interaction.State.ReceivedInstructions();
    }

    public void ReceiveStateInstructions(ActiveInteraction interaction, List<Item_InstructionData> itemInstructionDatas)
    {
        interaction.State.stateItemInstructionDatas.AddRange(itemInstructionDatas);
        interaction.State.ReceivedInstructions();
    }

    private void HandleInteractionStateInstructions(ActiveInteraction interaction)
    {
        //SendNeeds
        if (interaction.State.needIndex > interaction.State.stateNeedInstructionSOs.Count - 1
            || interaction.State.stateNeedInstructionSOs.Count == 0)
            interaction.State.needInstructionsDone = true;
        else
        {
            Need_InstructionSO niSO = interaction.State.stateNeedInstructionSOs[interaction.State.needIndex];
            interaction.State.needIndex++;
            needsEngine.NewInstructionSO(niSO, interaction.ThisCharacter, interaction);

        }


        //SendRelations Commented Due Not Implemented
        //if (interaction.InteractionState.stateRelationshipInstructionSOs.Count - 1 == interaction.InteractionState.relationshipIndex)
        //    interaction.InteractionState.relationshipInstructionsDone = true;
        if (interaction.State.stateRelationshipInstructionSOs.Count != 0)
            Debug.LogError($"Interaction {interaction.InteractionName} ({interaction.ThisCharacter.ItemName}) has unhandeled RelationshipInstructionSO({interaction.State.stateRelationshipInstructionSOs[interaction.State.relationshipIndex]})");

        //SendItems
        if (interaction.State.itemIndex > interaction.State.stateItemInstructionDatas.Count - 1
            || interaction.State.stateItemInstructionDatas.Count == 0)
            interaction.State.itemInstructionsDone = true;
        else
        {
            Item_InstructionData iid = interaction.State.stateItemInstructionDatas[interaction.State.itemIndex];
            interaction.State.itemIndex++;
            itemManager.HandleItemInstructionData(iid, interaction);

        }

        //SendCharacterInstructions
        if (interaction.State.characterIndex > interaction.State.stateCharacterInstructionSOs.Count - 1
       || interaction.State.stateCharacterInstructionSOs.Count == 0)
            interaction.State.characterInstructionsDone = true;
        else
        {
            Character_InstructionSO charInstructionSO = interaction.State.stateCharacterInstructionSOs[interaction.State.characterIndex];
            interaction.State.characterIndex++;
            characterControl.HandleCharacterInstruction(charInstructionSO, interaction);

        }

    }

    public void PrepareInstructionInteraction(ActiveInteraction interaction, InteractionSO interSO, Interactable interactionSource)
    {
        interaction.PushInteractionState(EInteractionState.Ending);
        SetStateInstructions(interaction, interSO.Need_InteractionInstructionsOnInteractionEnd,
     interSO.RelationshipChangeInstructionsOnInteractionEnd,
     interSO.ItemChangeInstructionSOsOnInteractionEnd,
     interSO.CharacterInstructionSOsOnInteractionEnd);
        interaction.State.interactionSource = interactionSource;

        //interaction.PushInteractionState(EInteractionState.Running);
        //interaction.State.stateNeedInstructionSOs = interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionTick;

        interaction.PushInteractionState(EInteractionState.Starting);
        SetStateInstructions(interaction, interSO.Need_InteractionInstructionsOnInteractionBegin,
    interSO.RelationshipChangeInstructionsOnInteraction,
    interSO.ItemChangeInstructionSOsOnInteractionBegin,
    interSO.CharacterInstructionSOsOnInteractionBegin);
        interaction.State.interactionSource = interactionSource;
    }

    private void InteractionStarting(ActiveInteraction interaction)
    {


        OnInteractionBeginInstructions(interaction);

        //if (characterControl.IsCharacterNextToInteractionTarget(interaction.ThisCharacter, InteractionDestinationTile(interaction)))
        //    OnInteractionBeginInstructions(interaction);
        //else
        //    InteractionRouting(interaction, InteractionDestinationTile(interaction));
    }

    private LotGridTile InteractionDestinationTile(ActiveInteraction interaction)
    {
        if (interaction.InteractionTuningSO.InteractionDestinationDifferentFromSource)
            return lotManager.GetTileInteractableIsOn(lotManager.GetItemOnLotByType(interaction.ThisCharacter.ThisLot, interaction.InteractionTuningSO.DestinationItem));
        else
            return lotManager.GetTileInteractableIsOn(interaction.InteractionSource);
    }
    private void OnInteractionBeginInstructions(ActiveInteraction interaction)
    {
        if (!interaction.State.HasReceivedInstructions)
        {
            //SetStateInstructions(interaction, interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionBegin,
            //   interaction.InteractionTuningSO.RelationshipChangeInstructionsOnInteraction,
            //   interaction.InteractionTuningSO.ItemChangeInstructionSOsOnInteractionBegin,
            //   interaction.InteractionTuningSO.CharacterInstructionSOsOnInteractionBegin);
            return;
        }
        else if (interaction.State.StateInstructionsDone())
        {
            interaction.PopInteractionState();
            //interaction.PushInteractionState(EInteractionState.Running);
            return;
        }
        else
        {
            HandleInteractionStateInstructions(interaction);
            return;
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
            switch (interaction.State.thisState)
            {
                case EInteractionState.Starting:
                    InteractionStarting(interaction);
                    break;
                case EInteractionState.Moving:
                    break;
                case EInteractionState.AtDestination:
                    OnInteractionDestinationArrival(interaction);
                    break;
                case EInteractionState.Waiting:
                    RegisterToWait(interaction);
                    break;
                case EInteractionState.Running:
                    InteractionOnTick(interaction, deltaTime);
                    break;
                case EInteractionState.Ending:
                    EndInteraction(interaction);
                    break;
                case EInteractionState.SubInteractions:
                    RunningSubInteractions(interaction);
                    break;
                case EInteractionState.Routine:
                    RoutineInstructions(interaction);
                    break;
                case EInteractionState.Instruction:
                    break;
                case EInteractionState.Default:
                    break;
            }
            if (InteractionShouldEnd(interaction))
            {
                interaction.PopInteractionState();
                //interaction.PushInteractionState(EInteractionState.Ending);
                //interaction.SetInteractionStateOLD(EInteractionState.Ending);

            }

        }
    }

    private void InteractionOnTick(ActiveInteraction interaction, float dt)
    {
        if (!interaction.State.HasReceivedInstructions)
        {
            interaction.State.stateNeedInstructionSOs = interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionTick;
            //RelationInstructions
            //interaction.InteractionState.stateItemInstructionSOs = interaction.InteractionTuningSO. ONTICK LIST;
            interaction.State.ReceivedInstructions();
            return;
        }
        //else if (interaction.InteractionState.StateInstructionsDone())
        //{
        //    //interaction.ChangeInteractionState(EInteractionState.Running);
        //}
        else
        {
            if (interaction.TimeSinceLastInstructionsSent > OneUnitOfTime)
            {
                bool tooMuchTime = true;
                while (tooMuchTime)
                {
                    interaction.TimeSinceLastInstructionsSent -= OneUnitOfTime;
                    HandleInteractionStateInstructions(interaction);

                    //GUMMY (Refersh Instructions since on tick = send one set of instructions/tick for the duration of interaction)
                    if (interaction.State.StateInstructionsDone())
                        interaction.State.RefreshStateInstructions();

                    //List<Need_Instruction> needInstructions = new();
                    //foreach (Need_InstructionSO niso in interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionTick)
                    //{
                    //    Need_Instruction ni = new(niso, interaction.ThisCharacter);
                    //    needInstructions.Add(ni);
                    //}
                    //needsEngine.NewInstructions(needInstructions);

                    if (interaction.TimeSinceLastInstructionsSent < OneUnitOfTime)
                        tooMuchTime = false;
                }
            }
            else
                interaction.TimeSinceLastInstructionsSent += dt;
        }


    }
    private bool InteractionShouldEnd(ActiveInteraction interaction)
    {
        //if (interaction.OInteractionState == EInteractionState.SubInteractions)
        //    return false;
        //if (interaction.OInteractionState == EInteractionState.Moving)
        //    return false;
        //if (interaction.OInteractionState == EInteractionState.Ending)
        //    return false;


        switch (interaction.InteractionEndingType)
        {
            case InteractionEndingType.Default:
                return false;
            case InteractionEndingType.SetTime:
                if (interaction.State.thisState != EInteractionState.Running)
                    return false;
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
        if (!interaction.State.StateInstructionsDone())
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


        //TrueEnd
       // interaction.PopInteractionState();
        ActiveInteractionState ais = interaction.previousInteractionStates.Peek();
        if (ais.thisState != EInteractionState.Default)
        {
            interaction.PopInteractionState();
            return;
        }

        activeInteractions.Remove(interaction);


        if (!interaction.isSubinteraction)
            characterAIHandler.OnInteractionEnd(interaction.ThisCharacter, interaction);

        if (debugLog)
            Debug.Log($"{interaction.ThisCharacter.ItemName} finished interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
       
        interaction.previousInteractionStates.Clear();
        UIController.RefreshInteractionStateData(interaction);

    }

    private void RunningSubInteractions(ActiveInteraction interaction)
    {
        if (interaction.subInteractionsHaveRan)
            interaction.SetInteractionStateOLD(EInteractionState.Ending);
    }
    public void RoutineInstructions(ActiveInteraction interaction)
    {
        if (interaction.State.StateInstructionsDone())
        {
            interaction.PopInteractionState();
            return;
        }
        else
        {
            HandleInteractionStateInstructions(interaction);
            return;
        }

    }
    private void SendOnInteractionEndInstructions(ActiveInteraction interaction)
    {

        if (!interaction.State.HasReceivedInstructions)
        {
            //SetStateInstructions(interaction, interaction.InteractionTuningSO.Need_InteractionInstructionsOnInteractionEnd,
            //  interaction.InteractionTuningSO.RelationshipChangeInstructionsOnInteractionEnd,
            //  interaction.InteractionTuningSO.ItemChangeInstructionSOsOnInteractionEnd,
            //  interaction.InteractionTuningSO.CharacterInstructionSOsOnInteractionEnd);

            //interaction.State.stateRelationshipInstructionSOs = interaction.InteractionTuningSO.RelationshipChangeInstructionsOnInteractionEnd;
            //interaction.State.stateItemInstructionDatas = interaction.InteractionTuningSO.ItemChangeInstructionSOsOnInteractionEnd;
            //interaction.State.ReceivedInstructions();
            return;
        }
        else if (interaction.State.StateInstructionsDone())
        {
            //interaction.ChangeInteractionState(EInteractionState.Running); //EndInstruction
            return;
        }
        else
        {
            HandleInteractionStateInstructions(interaction);
            return;
        }
    }


    private void HandleSubInteractions(List<SubInteraction> subInteractions, ActiveInteraction mainInteraction)
    {
        if (subInteractions.Count == 0)
            return;

        if (mainInteraction.State.thisState == EInteractionState.Routine)
            HandleRoutine(mainInteraction, subInteractions);

        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[mainInteraction.ThisCharacter];
        mainInteraction.SetInteractionStateOLD(EInteractionState.SubInteractions);
        int i = 0;
        foreach (SubInteraction subInteraction in subInteractions)
        {
            switch (subInteraction.SubInteractionSource)
            {
                case EItemLocation.ThisItem:
                    SubInteractionOnThisItem(mainInteraction, subInteraction, thisCharaAI);
                    break;
                case EItemLocation.Any:
                    SubInteractionOnAnyItem(mainInteraction, subInteraction, thisCharaAI);
                    break;
                case EItemLocation.OnItemCreatedByInteraction:
                    SubInteractionOnCreatedItem(mainInteraction, subInteraction, thisCharaAI);
                    break;
                case EItemLocation.OnMainItem:
                    SubInteractionOnMainItem(mainInteraction, subInteraction, thisCharaAI);
                    break;
                case EItemLocation.OnHeldItem:
                    SubInteractionOnHeldItem(mainInteraction, subInteraction, thisCharaAI);
                    break;
                case EItemLocation.OnMainItemSlot:
                    SubInteractionOnMainItemSlot(mainInteraction, subInteraction, thisCharaAI);
                    break;
                default:
                    break;
            }

            i++;
        }
        StartSubInteraction(thisCharaAI.SubInteractionQueue[0]);
    }

    private void SubInteractionOnThisItem(ActiveInteraction mainInteraction, SubInteraction subInteraction, CharacterAI thisCharaAI)
    {
        throw new NotImplementedException();
    }

    private void SubInteractionOnAnyItem(ActiveInteraction mainInteraction, SubInteraction subInteraction, CharacterAI charaAI)
    {
        StoredInteraction retrySub = lotManager.FindSuitableStoredInteractionOnLot(subInteraction.InteractionSO, mainInteraction.ThisCharacter.ThisLot);
        ActiveInteraction activeSubInteraction = NewActiveInteraction(mainInteraction.ThisCharacter, retrySub);
        activeSubInteraction.MakeIntoSubInteraction(mainInteraction);
        charaAI.AddSubInteration(activeSubInteraction);
    }
    private void SubInteractionOnCreatedItem(ActiveInteraction mainInteraction, SubInteraction subInteraction, CharacterAI charaAI)
    {
        StoredInteraction retrySub = itemManager.GetInteractionOnInteractable(subInteraction.InteractionSO, itemManager.GetItemCreatedByInteraction(mainInteraction.ThisCharacter));
        //MakeSubinteractionActive(mainInteraction, retrySub);
        ActiveInteraction activeSubInteraction = NewActiveInteraction(mainInteraction.ThisCharacter, retrySub);
        activeSubInteraction.MakeIntoSubInteraction(mainInteraction);
        charaAI.AddSubInteration(activeSubInteraction);
    }
    private void SubInteractionOnMainItem(ActiveInteraction mainInteraction, SubInteraction subInteraction, CharacterAI charaAI)
    {
        Debug.LogError($"SubInteractionOnMainItem {mainInteraction.InteractionName} / {subInteraction.InteractionSO.InteractionName}");
    }
    private void SubInteractionOnHeldItem(ActiveInteraction mainInteraction, SubInteraction subInteraction, CharacterAI charaAI)
    {
        Debug.LogError($"SubInteractionOnHeldItem {mainInteraction.InteractionName} / {subInteraction.InteractionSO.InteractionName}");

    }
    private void SubInteractionOnMainItemSlot(ActiveInteraction mainInteraction, SubInteraction subInteraction, CharacterAI charaAI)
    {
        foreach (Item_Slot slot in (mainInteraction.InteractionSource as ItemBase).ItemSlotsOnItem)
        {
            if (slot.ItemInSlot == null)
                continue;

            StoredInteraction retrySub = itemManager.GetInteractionOnInteractable(subInteraction.InteractionSO, slot.ItemInSlot);
            if (retrySub == null)
                continue;
            //else
            //the thingy
        }
    }

    private void MakeSubinteractionActive(ActiveInteraction mainInteraction, StoredInteraction subInteraction)
    {
        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[mainInteraction.ThisCharacter];
        ActiveInteraction activeSubInteraction = NewActiveInteraction(mainInteraction.ThisCharacter, subInteraction);
        activeSubInteraction.MakeIntoSubInteraction(mainInteraction);
        thisCharaAI.AddSubInteration(activeSubInteraction);
    }

    private void HandleRoutine(ActiveInteraction interaction, List<SubInteraction> routineInteractions)
    {

    }
    private void StartSubInteraction(ActiveInteraction subInteraction)
    {
        CharacterAI thisCharaAI = characterAIHandler.CharactersAIsByCharacter[subInteraction.parentInteraction.ThisCharacter];
        thisCharaAI.RemoveSubInteraction(thisCharaAI.SubInteractionQueue[0]);
        thisCharaAI.NewCurrentSubInteraction(subInteraction);
        StartNewInteraction(subInteraction);

    }

    //Actions
    public ActiveInteraction BuildAction(StoredInteraction storedItneraction, Character thisCharacter, ActiveInteraction parentInteraction)
    {
        ActiveInteraction action = new ActiveInteraction(thisCharacter, storedItneraction);
        action.parentInteraction = parentInteraction;
        action.isAction = true;
        parentInteraction.actions.Add(action);
        return action;

    }

    //DataConversions
    public Item_InstructionData ItemInstructionSOToData(Item_InstructionSO itemInstructionSO)
    {
        return new Item_InstructionData(itemInstructionSO);
    }
}