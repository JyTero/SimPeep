using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveInteraction
{

    private InteractionSO interactionSO;
    public InteractionSO InteractionSO { get { return interactionSO; } }

    private InteractionData interactionData;
    public InteractionData InteractionData { get { return interactionData; } }

    private string interactionName;
    public string InteractionName { get { return interactionName; } }

    private Interactable interactionSource;
    public Interactable InteractionSource { get { return interactionSource; } }

    private Character thisCharacter;
    public Character ThisCharacter { get { return thisCharacter; } }

    private InteractionEndingType interactionEndingType;
    public InteractionEndingType InteractionEndingType { get { return interactionEndingType; } }

    private float interactionLength;
    public float InteractionLength { get { return interactionLength; } }

    private NeedType interactionEndingTargetNeedType;
    public NeedType InteractionEndingTargetNeedType { get { return interactionEndingTargetNeedType; } }
    private int interactionEndingTargetNeedValue;
    public int InteractionEndingTargetNeedValue { get { return interactionEndingTargetNeedValue; } }

    private bool isReaction;
    public bool IsReaction { get { return isReaction; } }


    private List<NeedType> needsToWeight = new();
    public List<NeedType> NeedsToWeight { get { return needsToWeight; } }

    private List<InteractionScoringModifier> scoringModifiers = new();
    public List<InteractionScoringModifier> ScoringModifiers { get { return scoringModifiers; } }

    private List<InteractionSO> followupInteractionSOs;
    public List<InteractionSO> FollowupInteractionSOs { get { return followupInteractionSOs; } }

    //InteractionGroup
    public InteractionGroupSO InteractionGroupSO;

    //RuntimeData
    public List<SubInteraction> subInteractions = new();
    public bool isSubinteraction;
    public bool subInteractionsHaveRan = false;
    public ActiveInteraction parentInteraction;

    public bool socialInteractionInitialised = false;

    //public List<Interactable> InteractablesCreatedByThisInteraction = new();
    //public List<Interactable> InteractablesCreatedByThisInteraction { get { return  InteractablesCreatedByThisInteraction; } }

    public float interactionLenghtAccumulation;

    public bool allStateInteractionsSent = false;
    public InstructionSO currentInstruction = null;

    public Slot knownSlot;
    public ItemBase knownItem;
    public LotGridTile knownTile;

    public bool runningActions = false;
    public bool isAction = false;
    public List<ActiveInteraction> actions = new();

    // States
    private InteractionState state;
    public InteractionState State { get { return state; } }
    public Stack<InteractionState> interactionStateStack = new();

    private UIController uiController;

    public bool InteractionCancelled = false;


    public void PushInteractionState(ActiveInteraction interaction, InteractionState newState)
    {
        if (state != null)
            interactionStateStack.Push(state);

        state = newState;
        state.OnStateEnter(interaction);

        uiController.RefreshInteractionStateData(this, ThisCharacter);

    }
    public void PopInteractionState()
    {
        InteractionState interactionState = interactionStateStack.Pop();
        if (interactionState != null)
            ChangeCurrentState(interactionState);
        else
            Debug.LogError($"Null interaction state on {thisCharacter.ItemName} ({InteractionName})");

        uiController.RefreshInteractionStateData(this, ThisCharacter);
    }
    private void ChangeCurrentState(InteractionState newState)
    {
        if (state != null)
            state.OnStateExit(this);
        state = newState;
        state.OnStateEnter(this);
    }


    public List<Item_Instruction> ItemChangeInstructionSOsOnInteractionBegin = new();

    public bool allInstructionsDone = false;


    public float TimeSinceLastInstructionsSent;
    public float interactionScore;

    public ActiveInteraction(Character chara, StoredInteraction storedInteraction)
    {
        interactionSO = storedInteraction.InteractionSO;
        interactionData = storedInteraction.InteractionData;
        interactionSource = storedInteraction.InteractionSource;
        thisCharacter = chara;

        uiController = GameObject.FindAnyObjectByType<UIController>();


        CommonConstruct();
    }

    private void CommonConstruct()
    {
        interactionName = interactionData.InteractionName;
        BuildInteractionEnding();
        interactionLenghtAccumulation = 0;
        interactionScore = 0;
        isReaction = interactionData.Reaction;

        scoringModifiers = interactionData.ScoringModifiers;

        foreach (Need_InstructionSO needInstructionSO in interactionData.Need_InteractionInstructionsOnInteractionTick)
        {
            needsToWeight.Add(needInstructionSO.NeedToAdjust);
        }
        TimeSinceLastInstructionsSent = 0;
        followupInteractionSOs = interactionData.FollowupInteractionSOs;
    }

    public void MakeIntoSubInteraction(ActiveInteraction pi)
    {
        isSubinteraction = true;
        parentInteraction = pi;
    }

    private void BuildInteractionEnding()
    {
        interactionEndingType = interactionData.InteractionEndingType;
        switch (interactionData.InteractionEndingType)
        {
            case InteractionEndingType.Default:
                return;
            case InteractionEndingType.SetTime:
                interactionLength = interactionData.InteractionLenght;
                return;
            case InteractionEndingType.UntillNeedAtValue:
                interactionEndingTargetNeedType = interactionData.TargetNeedType;
                interactionEndingTargetNeedValue = interactionData.TargetNeedValue;
                return;
        }
    }
}

public class StoredInteraction
{
    private InteractionSO interactionSO;
    public InteractionSO InteractionSO { get { return interactionSO; } }

    private InteractionData interactionData;
    public InteractionData InteractionData { get { return interactionData; } }

    private Interactable interactionSource;
    public Interactable InteractionSource { get { return interactionSource; } }
    private InteractionGroupSO groupSO;
    public InteractionGroupSO GroupSO { get { return groupSO; } }


    //Invalid interactions will not be selectable by anyone (picking up already carried item)
    public bool InvalidInteraction;

    public StoredInteraction(InteractionSO interactionTuningSO, Interactable interactionSource)
    {
        interactionSO = interactionTuningSO;
        interactionData = new(interactionTuningSO);
        this.interactionSource = interactionSource;
        InvalidInteraction = interactionTuningSO.InvalidInteraction;
    }

    public void MakeIntoStoredInteractionGroup(InteractionGroupSO itgSO)
    {
        groupSO = itgSO;
    }
}

public enum EInteractionState
{
    Default,
    Starting,
    Moving,
    AtDestination,
    Waiting,
    Running,
    Ending,
    SubInteractions,
    Routine,
    Instruction,


}
