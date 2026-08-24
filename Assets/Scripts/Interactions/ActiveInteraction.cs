using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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

    //RuntimeData
    public List<SubInteraction> subInteractions = new();
    public bool isSubinteraction;
    public bool subInteractionsHaveRan = false;
    public ActiveInteraction parentInteraction;

    //public List<Interactable> InteractablesCreatedByThisInteraction = new();
    //public List<Interactable> InteractablesCreatedByThisInteraction { get { return  InteractablesCreatedByThisInteraction; } }

    public float interactionLenghtAccumulation;
    private InteractionState interactionState;
    public InteractionState InteractionState { get { return interactionState; } }
    public void SetInteractionState(InteractionState intrctState)
    {
        //Debug.Log($"InteractionStateChange: {InteractionName} had state {interactionState}, new state: {intrctState}");
        interactionState = intrctState;
    }
    public bool allInstructionsDone = false;


    public float TimeSinceLastInstructionsSent;
    public float interactionScore;

    public ActiveInteraction(Character chara, StoredInteraction storedInteraction)
    {
        interactionTuningSO = storedInteraction.InteractionTuningSO;
        interactionSource = storedInteraction.InteractionSource;
        thisCharacter = chara;

        CommonConstruct();
    }

    private void CommonConstruct()
    {
        interactionName = interactionTuningSO.InteractionName;
        BuildInteractionEnding();
        interactionLenghtAccumulation = 0;
        interactionState = InteractionState.Default;
        interactionScore = 0;
        isReaction = interactionTuningSO.Reaction;

        scoringModifiers = interactionTuningSO.ScoringModifiers;

        foreach (Need_InstructionSO needInstructionSO in interactionTuningSO.Need_InteractionInstructions)
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

    //Invalid interactions will not be selectable by anyone (picking up already carried item)
    public bool InvalidInteraction;

    public StoredInteraction(InteractionSO interactionTuningSO, Interactable interactionSource)
    {
        this.interactionTuningSO = interactionTuningSO;
        this.interactionSource = interactionSource;
        InvalidInteraction = interactionTuningSO.InvalidInteraction;
    }
}

public enum InteractionState
{
    Default,
    Starting,
    Moving,
    AtDestination,
    Waiting,
    Running,
    Ending,
    SubInteractions,
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
