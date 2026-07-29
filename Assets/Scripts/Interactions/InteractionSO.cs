using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionSO", menuName = "Scriptable Objects/InteractionSO")]
public class InteractionSO : ScriptableObject
{
    [SerializeField]
    private string interactionName;
    public string InteractionName { get { return interactionName; } }

    [SerializeField, Tooltip("Use this to leave notes about the interaction, such as what are its planned owner items")]
    private string Description;

    //Interaction Lenght Data    
    [SerializeField]
    private InteractionLengthType interactionLenghtType;

    private bool setTime = false;
    private bool untillNeedAtValue = false;

    [SerializeField, ShowIf("setTime")]
    private int interactionLenght;
    public int InteractionLenght { get { return interactionLenght; } }

    [SerializeField, ShowIf("untillNeedAtValue")]
    private NeedSO targetNeed;
    public NeedSO TargetNeed { get { return targetNeed; } }
    [SerializeField, ShowIf("untillNeedAtValue")]
    private int targetNeedValue;
    public int TargetNeedValue { get { return targetNeedValue; } }


    //Each instruction variant has its own list 
    [SerializeField]
    private List<Need_InstructionSO> need_InteractionInstructions = new();
    public List<Need_InstructionSO> Need_InteractionInstructions { get { return need_InteractionInstructions; } }

    [SerializeField]
    private List<Relationship_InstructionSO> relationshipChangeInstructions = new();
    public List<Relationship_InstructionSO> RelationshipChangeInstructions { get { return relationshipChangeInstructions; } }

    //List to make "choose one based on traits possible"?
    [SerializeField, ShowIf("IsSocial")]
    private List<InteractionSO> socialResponceInteractions = new();
    public List<InteractionSO> SocialResponceInteractions { get { return socialResponceInteractions; } }

    public bool Reaction;

    [SerializeField, HideIf("Reaction")]
    private List<InteractionScoringModifier> scoringModifiers = new();
    public List<InteractionScoringModifier> ScoringModifiers { get { return scoringModifiers; } }

    [HideIf("Reaction")]
    public bool IsSocial;

    private void OnValidate()
    {
        switch (interactionLenghtType)
        {
            case InteractionLengthType.Default:
                return;
            case InteractionLengthType.SetTime:
                setTime = true;
                untillNeedAtValue = false;
                break;
            case InteractionLengthType.UntillNeedAtValue:
                untillNeedAtValue = true;
                setTime = false;
                break;
        }
    }

    public enum InteractionLengthType
    {
        Default,
        SetTime,
        UntillNeedAtValue,
    }
}


