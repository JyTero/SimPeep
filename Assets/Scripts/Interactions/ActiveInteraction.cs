using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveInteraction
{

    private InteractionSO interactionTuningSO;
    public InteractionSO InteractionTuningSO { get { return interactionTuningSO; } }

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

    //public List<Interactable> InteractablesCreatedByThisInteraction = new();
    //public List<Interactable> InteractablesCreatedByThisInteraction { get { return  InteractablesCreatedByThisInteraction; } }

    public float interactionLenghtAccumulation;
    private EInteractionState opreviousNonMoveState;
    public EInteractionState OPreviousNonMoveState { get { return opreviousNonMoveState; } }
    private EInteractionState OinteractionState;
    public EInteractionState OInteractionState { get { return OinteractionState; } }
    public bool allStateInteractionsSent = false;
    public InstructionSO currentInstruction = null;

    public Slot knownSlot;
    public ItemBase knownItem;
    public LotGridTile knownTile;

    public bool runningActions = false;
    public bool isAction = false;
    public List<ActiveInteraction> actions = new();


    public void SetInteractionStateOLD(EInteractionState newState)
    {
        //Debug.Log($"InteractionStateChange: {InteractionName} had state {interactionState}, new state: {intrctState}");
        if (OinteractionState != EInteractionState.Moving)
            opreviousNonMoveState = OinteractionState;
        OinteractionState = newState;
    }

    //NewStates
    private ActiveInteractionState state; //Push, peak, pop
    public ActiveInteractionState State { get { return state; } }
    public ActiveInteractionState previousNonMoveState;
    public Stack<ActiveInteractionState> previousInteractionStates = new();



    private UIController uiController;
    public void PushInteractionState(EInteractionState newState)
    {
        previousInteractionStates.Push(state);
        state = new(newState);

        uiController.RefreshInteractionStateData(this);

    }
    public void PopInteractionState()
    {
       ActiveInteractionState ais = previousInteractionStates.Pop();
        if (ais != null)
            state = ais;
        else
            Debug.LogError($"Null interaction state on {thisCharacter.ItemName} ({InteractionName})");

        uiController.RefreshInteractionStateData(this);
    }

    //public void ChangeToPreviousState()
    //{
    //    interactionState = previousNonMoveState;

    //}


    public List<Item_Instruction> ItemChangeInstructionSOsOnInteractionBegin = new();

    public bool allInstructionsDone = false;


    public float TimeSinceLastInstructionsSent;
    public float interactionScore;

    public ActiveInteraction(Character chara, StoredInteraction storedInteraction)
    {
        interactionTuningSO = storedInteraction.InteractionTuningSO;
        interactionSource = storedInteraction.InteractionSource;
        thisCharacter = chara;

        uiController= GameObject.FindAnyObjectByType<UIController>();

        state = new(EInteractionState.Default);


        CommonConstruct();
    }

    private void CommonConstruct()
    {
        interactionName = interactionTuningSO.InteractionName;
        BuildInteractionEnding();
        interactionLenghtAccumulation = 0;
        OinteractionState = EInteractionState.Default;
        interactionScore = 0;
        isReaction = interactionTuningSO.Reaction;

        scoringModifiers = interactionTuningSO.ScoringModifiers;

        foreach (Need_InstructionSO needInstructionSO in interactionTuningSO.Need_InteractionInstructionsOnInteractionTick)
        {
            needsToWeight.Add(needInstructionSO.NeedToAdjust);
        }
        TimeSinceLastInstructionsSent = 0;
        followupInteractionSOs = InteractionTuningSO.FollowupInteractionSOs;
    }

    public void PrepareSubInteractions(LotManager lotManager, WorldLot thisLot)
    {
        foreach (SubInteraction subInteraction in interactionTuningSO.SubInteractions)
        {
            //if(subInteraction.InteractionOnCreatedObject)
            //  continue;

            //SubInteraction subSi = lotManager.FindSuitableStoredInteractionOnLot(subInteraction.StoredInteractionSO, thisLot);
            //subInteractions.Add(subSi);
            subInteractions.Add(subInteraction);
        }
    }
    public void MakeIntoSubInteraction(ActiveInteraction pi)
    {
        isSubinteraction = true;
        parentInteraction = pi;
    }

    private void BuildInteractionEnding()
    {
        interactionEndingType = interactionTuningSO.InteractionEndingType;
        switch (InteractionTuningSO.InteractionEndingType)
        {
            case InteractionEndingType.Default:
                return;
            case InteractionEndingType.SetTime:
                interactionLength = InteractionTuningSO.InteractionLenght;
                return;
            case InteractionEndingType.UntillNeedAtValue:
                interactionEndingTargetNeedType = InteractionTuningSO.TargetNeedType;
                interactionEndingTargetNeedValue = InteractionTuningSO.TargetNeedValue;
                return;
        }
    }
}

public class StoredInteraction
{
    private InteractionSO interactionTuningSO;
    public InteractionSO InteractionTuningSO { get { return interactionTuningSO; } }

    private Interactable interactionSource;
    public Interactable InteractionSource { get { return interactionSource; } }
    private InteractionGroupSO groupSO;
    public InteractionGroupSO GroupSO { get { return groupSO; } }


    //Invalid interactions will not be selectable by anyone (picking up already carried item)
    public bool InvalidInteraction;

    public StoredInteraction(InteractionSO interactionTuningSO, Interactable interactionSource)
    {
        this.interactionTuningSO = interactionTuningSO;
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
//NEXT UP:
// Implement Destroy Item       Done
// Destroy Raw food             Done
// Spawn Cooked food
// Place onto stove
// Pick up cooked food
// (
//      Implement Dining Table and Chairs
//      Use them to eat.
// )
// Eat.
