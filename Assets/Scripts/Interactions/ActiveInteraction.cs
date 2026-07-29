using NUnit.Framework;
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

    //RuntimeData
    public float interactionLenghtAccumulation;
    public InteractionState interactionState;

    public float TimeSinceLastInstructionsSent;
    public float interactionScore;

    public ActiveInteraction(Character chara, InteractionSO itSO, Interactable interactable)
    {
        interactionTuningSO = itSO;
        interactionSource = interactable;
        thisCharacter = chara;

        CommonConstruct();
    }
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

    public StoredInteraction(InteractionSO interactionTuningSO, Interactable interactionSource)
    {
        this.interactionTuningSO = interactionTuningSO;
        this.interactionSource = interactionSource;
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
}
