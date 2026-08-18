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

    public bool Reaction;
   

    [HideIf("Reaction")]
    public bool IsSocial;

    [SerializeField ,Tooltip("Hidden interactions alern't selectable by user or normal interaction selection")]
    private bool hiddenInteraction;
    public bool HiddenInteraction {  get { return hiddenInteraction; } }

    [SerializeField, Tooltip("Skip moving stage on interaction execution, runs interaction immediately instead")]
    private bool skipMovement;
    public bool SkipMovement {  get { return skipMovement; } }

    //Interaction Ending Data    
    [SerializeField]
    private InteractionEndingType interactionEndingType;
    public InteractionEndingType InteractionEndingType { get { return interactionEndingType; } }

    private bool setTime = false;
    private bool untillNeedAtValue = false;

    [SerializeField, ShowIf("setTime")]
    private int interactionLenght;
    public int InteractionLenght { get { return interactionLenght; } }

    [SerializeField, ShowIf("untillNeedAtValue")]
    private NeedType targetNeedType;
    public NeedType TargetNeedType { get { return targetNeedType; } }
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

    [SerializeField]
    private List <Item_InstructionSO> itemChangeInstructions = new();
    public List<Item_InstructionSO > ItemChangeInstructions { get {return itemChangeInstructions; } }

    [SerializeField, Tooltip("Way to use pre-existing interactions to build new ones. Example: Fridge spawns Food. Food has Pick Up interaction, which can be plased here to automatically  pick up the food on creation")]
    private List<SubInteraction> subInteractionSOs = new();
    public List<SubInteraction> SubInteractionSOs { get { return subInteractionSOs; } }

    [SerializeField, Tooltip("Capabilities the interaction utilises")]
    private List<ItemCapabilites> requiredItemCapabilities = new();
    public List<ItemCapabilites> RequiredItemCapabilities { get { return requiredItemCapabilities; } }

    //List to make "choose one based on traits possible"?
    [SerializeField, ShowIf("IsSocial")]
    private List<InteractionSO> socialResponceInteractions = new();
    public List<InteractionSO> SocialResponceInteractions { get { return socialResponceInteractions; } }


    [SerializeField, HideIf("Reaction")]
    private List<InteractionScoringModifier> scoringModifiers = new();
    public List<InteractionScoringModifier> ScoringModifiers { get { return scoringModifiers; } }

    //Suggested Follow-Up Interaction
    [SerializeField, Tooltip("Wheter this interactions is designed to be followed by another. Example: Make Dinner -> Cook Dinner -> Eat Dinner")]
    private bool hasFollowup = false;
    public bool HasFollowup { get { return hasFollowup; } }

    [SerializeField, ShowIf("hasFollowup")]
    private List<InteractionSO> followupInteractionSOs;
    public List<InteractionSO> FollowupInteractionSOs {  get { return followupInteractionSOs; } }



    private void OnValidate()
    {
        switch (interactionEndingType)
        {
            case InteractionEndingType.Default:
                setTime = false;
                untillNeedAtValue = false;
                return;
            case InteractionEndingType.SetTime:
                setTime = true;
                untillNeedAtValue = false;
                break;
            case InteractionEndingType.UntillNeedAtValue:
                untillNeedAtValue = true;
                setTime = false;
                break;
        }
    }

   
}


