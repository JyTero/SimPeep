using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Permissions;
using Unity.VisualScripting;
using UnityEngine;


public class InteractionEngine : ManagementCore
{
    //CREATE: Dictionary activeInteractionsByCharacter
    private List<ActiveInteraction> activeInteractions = new();

    private Dictionary<Character, ActiveInteraction> activeInteractionsByCharacter = new();
    public Dictionary<Character, ActiveInteraction> ActiveInteractionsByCharacter { get { return activeInteractionsByCharacter; } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;

        //InteractionUpdate(deltaTime);
        InteractionStateUpdate(dt);
    }

    public void StartNewInteraction(ActiveInteraction interaction)
    {

        if (debugLog)
            Debug.Log($"{interaction.ThisCharacter.ItemName} started interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
        interaction.PushInitialisationState(interaction, new Default_InteractionState(characterAIHandler, this));
        activeInteractions.Add(interaction);
        activeInteractionsByCharacter.Add(interaction.ThisCharacter, interaction);
        PrepareInteractionInstructions(interaction);
        interaction.InitialiseState();
        return;

    }
    private void PrepareInteractionInstructions(ActiveInteraction interaction)
    {
        InteractionState state = new Ending_InteractionState(characterAIHandler, this);
        interaction.PushInitialisationState(interaction, state);
        SetStateInstructions(state, interaction.InteractionData.Need_InteractionInstructionsOnInteractionEnd,
     interaction.InteractionData.RelationshipChangeInstructionsOnInteractionEnd,
     interaction.InteractionData.ItemChangeInstructionSOsOnInteractionEnd,
     interaction.InteractionData.CharacterInstructionSOsOnInteractionEnd,
     interaction.InteractionData.SpecificItemInstructionSOsOnInteractionEnd);

        state = new Running_InteractionState(characterAIHandler, this);
        interaction.PushInitialisationState(interaction, state);
        state.stateNeedInstructionSOs = interaction.InteractionData.Need_InteractionInstructionsOnInteractionTick;

        state = new Starting_InteractionState(characterAIHandler, this);
        interaction.PushInitialisationState(interaction, state);
        SetStateInstructions(state, interaction.InteractionData.Need_InteractionInstructionsOnInteractionBegin,
    interaction.InteractionData.RelationshipChangeInstructionsOnInteractionBegin,
    interaction.InteractionData.ItemChangeInstructionSOsOnInteractionBegin,
    interaction.InteractionData.CharacterInstructionSOsOnInteractionBegin,
    interaction.InteractionData.SpecificItemInstructionSOsOnInteractionBegin);
    }


    private void Wait(ActiveInteraction interaction)
    {
        //TODO: timeout if waiting for too long
    }
    private void SetStateInstructions(InteractionState state, List<Need_InstructionSO> needInstructionSOs,
    List<Relationship_InstructionSO> relationshipInstructionSOs, List<Item_InstructionSO> itemInstructionSOs,
    List<Character_InstructionSO> characterInstructionSOs, List<ItemSpecific_InstructionSO> itemSpecificInstructionSOs)
    {
        //NeedInstructions
        state.stateNeedInstructionSOs = needInstructionSOs;
        //RelationInstructions
        state.stateRelationshipInstructionSOs = relationshipInstructionSOs;
        //ItemInstructions
        List<Item_InstructionData> item_InstructionDatas = new();
        foreach (Item_InstructionSO iiSO in itemInstructionSOs)
            item_InstructionDatas.Add(ItemInstructionSOToData(iiSO));
        state.stateItemInstructionDatas = item_InstructionDatas;
        //Character
        state.stateCharacterInstructionSOs = characterInstructionSOs;

        //ItemSpecific
        state.stateItemSpecificInstructionSOs = itemSpecificInstructionSOs;

        state.ReceivedInstructions();
    }

    public void ReceiveStateInstructions(ActiveInteraction interaction, List<Item_InstructionData> itemInstructionDatas)
    {
        interaction.State.stateItemInstructionDatas.AddRange(itemInstructionDatas);
        interaction.State.ReceivedInstructions();
    }


    public void HandleInteractionStateInstructions(ActiveInteraction interaction)
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


        //SendRelations

        if (interaction.State.relationshipIndex > interaction.State.stateRelationshipInstructionSOs.Count - 1
            || interaction.State.stateRelationshipInstructionSOs.Count == 0)
            interaction.State.relationshipInstructionsDone = true;
        else
        {
            Relationship_InstructionSO relChangeInstructionSO = interaction.State.stateRelationshipInstructionSOs[interaction.State.relationshipIndex];
            interaction.State.relationshipIndex++;
            RelationshipChange_Instruction relChangeInstruction = new(relChangeInstructionSO, interaction.ThisCharacter, interaction.InteractionSource as Character);
            relationshipEngine.HandleRelationshipInstruction(relChangeInstruction);
        }

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
            Character_Instruction charInstruction = new(interaction.State.stateCharacterInstructionSOs[interaction.State.characterIndex]);
            interaction.State.characterIndex++;
            characterControl.HandleCharacterInstruction(charInstruction, interaction);

        }

        //Prehandle Specifics
        //Will be sent to SpecificInstructionHandler (Another dumping ground for item stuff)
        if (interaction.State.itemSpecificInstructionIndex > interaction.State.stateItemSpecificInstructionSOs.Count - 1
    || interaction.State.stateItemSpecificInstructionSOs.Count == 0)
            interaction.State.itemSpecificInstructionsDone = true;
        else
        {
            ItemSpecific_InstructionSO itemSpecificInstructionSO = interaction.State.stateItemSpecificInstructionSOs[interaction.State.itemSpecificInstructionIndex];
            interaction.State.itemSpecificInstructionIndex++;
            specificInstructionHandler.HandleItemSpecificInstruction(itemSpecificInstructionSO, interaction.InteractionSource as ItemBase);

        }

    }

    public void PrepareInstructionInteraction(ActiveInteraction interaction, InteractionData instructionInteractionData, Interactable instructionInteractionSource)
    {
        InteractionState state = new Ending_InteractionState(characterAIHandler, this);
        interaction.PushInteractionState(interaction, state);
        SetStateInstructions(state, instructionInteractionData.Need_InteractionInstructionsOnInteractionEnd,
     instructionInteractionData.RelationshipChangeInstructionsOnInteractionEnd,
     instructionInteractionData.ItemChangeInstructionSOsOnInteractionEnd,
     instructionInteractionData.CharacterInstructionSOsOnInteractionEnd,
     instructionInteractionData.SpecificItemInstructionSOsOnInteractionEnd);

        state = new Running_InteractionState(characterAIHandler, this);
        interaction.PushInteractionState(interaction, state);
        interaction.State.stateNeedInstructionSOs = instructionInteractionData.Need_InteractionInstructionsOnInteractionTick;

        state = new Starting_InteractionState(characterAIHandler, this);
        interaction.PushInteractionState(interaction, state);
        SetStateInstructions(state, instructionInteractionData.Need_InteractionInstructionsOnInteractionBegin,
    instructionInteractionData.RelationshipChangeInstructionsOnInteractionBegin,
    instructionInteractionData.ItemChangeInstructionSOsOnInteractionBegin,
    instructionInteractionData.CharacterInstructionSOsOnInteractionBegin,
        instructionInteractionData.SpecificItemInstructionSOsOnInteractionBegin);

    }

    public void OnStateUpdateInstructions(ActiveInteraction interaction)
    {
        if (interaction.State.StateInstructionsDone())
        {

            return;
        }
        else
        {
            HandleInteractionStateInstructions(interaction);
            return;
        }
    }

    public void RegisterToWait(ActiveInteraction interaction, EWaitReason waitReason)
    {
        characterAIHandler.RegisterToWait(interaction.ThisCharacter, waitReason, interaction.InteractionSource);
        interaction.PushInteractionState(interaction, new Waiting_InteractionState(characterAIHandler, this));
    }



    private void InteractionStateUpdate(float dt)
    {
        for (int i = activeInteractions.Count - 1; i >= 0; i--)
        {
            ActiveInteraction interaction = activeInteractions[i];
            if (interaction.InteractionCancelled)
                CancelInteraction(interaction);
            interaction.State.OnStateUpdate(interaction, dt);
        }
    }

    private void CancelInteraction(ActiveInteraction interaction)
    {
        interaction.PopInteractionState();
        if (interaction.State is not Ending_InteractionState)
            CancelInteraction(interaction);

    }

    public bool InteractionShouldEnd(ActiveInteraction interaction)
    {
        switch (interaction.InteractionEndingType)
        {
            case InteractionEndingType.Default:
                return false;
            case InteractionEndingType.SetTime:
                if (interaction.State is not Running_InteractionState)
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


    public void OnInteractionEnd(ActiveInteraction interaction)
    {
        activeInteractions.Remove(interaction);
        activeInteractionsByCharacter.Remove(interaction.ThisCharacter);
        characterAIHandler.OnInteractionEnd(interaction.ThisCharacter, interaction);
        itemManager.OnInteractionEnd(interaction, interaction.InteractionSource as ItemBase);
        if (debugLog)
            Debug.Log($"{interaction.ThisCharacter.ItemName} finished interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
        UIController.RefreshInteractionStateData(interaction, interaction.ThisCharacter);

    }

    public void CancelInteraction(CharacterAI character)
    {
        character.CurrentInteraction.InteractionCancelled = true;
    }

    //DataConversions
    public Item_InstructionData ItemInstructionSOToData(Item_InstructionSO itemInstructionSO)
    {
        return new Item_InstructionData(itemInstructionSO);
    }


}