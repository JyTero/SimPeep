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

    private float interactionLength;
    public float InteractionLength { get { return interactionLength; } }


    private List<NeedType> needsToWeight = new();
    public List<NeedType> NeedsToWeight { get { return needsToWeight; } }

    private List<InteractionScoringModifier> scoringModifiers = new();
    public List<InteractionScoringModifier> ScoringModifiers { get { return scoringModifiers; } }

    //RuntimeData
    public float interactionLenghtAccumulation;
    public InteractionState interactionState;

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
        interactionLength = interactionTuningSO.InteractionLenght;
        interactionLenghtAccumulation = 0;
        interactionState = InteractionState.Default;
        interactionScore = 0;

        scoringModifiers = interactionTuningSO.ScoringModifiers;

        foreach (Need_InstructionSO needInstructionSO in interactionTuningSO.Need_InteractionInstructions)
        {
            needsToWeight.Add(needInstructionSO.NeedToAdjust);
        }
    }



}

public class StoredInteraction
{
    private InteractionSO interactionTuningSO;
    public InteractionSO InteractionTuningSO { get { return interactionTuningSO; } }

    private Interactable interactionSource;
    public Interactable InteractionSource { get { return interactionSource; } }

    public StoredInteraction (InteractionSO interactionTuningSO, Interactable interactionSource)
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
    Running,
    Ending,
}
