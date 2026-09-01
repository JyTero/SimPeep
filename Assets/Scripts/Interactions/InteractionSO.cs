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

    [SerializeField, Tooltip("Hidden interactions alern't selectable by user or normal interaction selection")]
    private bool hiddenInteraction;
    public bool HiddenInteraction { get { return hiddenInteraction; } }

    [SerializeField, HideIf("interactionDestinationDifferentFromSource"),
        Tooltip("Skip moving stage on interaction execution, runs interaction immediately instead")]
    private bool skipMovement;
    public bool SkipMovement { get { return skipMovement; } }

    //InteractionDestination differ form Item (Cooking RawPlate should be done at stove, not at plate)
    [SerializeField, HideIf("skipMovement")]
    private bool interactionDestinationDifferentFromSource;
    public bool InteractionDestinationDifferentFromSource { get { return interactionDestinationDifferentFromSource; } }

    [SerializeField, ShowIf("interactionDestinationDifferentFromSource")]
    private ItemSO destinationItem;
    public ItemSO DestinationItem { get { return destinationItem; } }

    [SerializeField, Tooltip("Invalid interactions will not be selectable by anyone/thing (picking up already carried item).\nThis is the start state")]
    private bool invalidInteraction;
    public bool InvalidInteraction { get { return invalidInteraction; } }


    //Interaction Ending Data    
    [SerializeField]
    private InteractionEndingType interactionEndingType;
    public InteractionEndingType InteractionEndingType { get { return interactionEndingType; } }

    private bool setTime = false;
    private bool untillNeedAtValue = false;

    [SerializeField, ShowIf("setTime")]
    private float interactionLenght;
    public float InteractionLenght { get { return interactionLenght; } }

    [SerializeField, ShowIf("untillNeedAtValue")]
    private NeedType targetNeedType;
    public NeedType TargetNeedType { get { return targetNeedType; } }
    [SerializeField, ShowIf("untillNeedAtValue")]
    private int targetNeedValue;
    public int TargetNeedValue { get { return targetNeedValue; } }


    //Each instruction variant has its own list 
    [SerializeField, Foldout("ON INTERACTION BEGIN")]
    private List<Need_InstructionSO> need_InteractionInstructionsOnInteractionBegin = new();
    public List<Need_InstructionSO> Need_InteractionInstructionsOnInteractionBegin { get { return need_InteractionInstructionsOnInteractionBegin; } }
    [SerializeField, Foldout("ON INTERACTION TICK")]
    private List<Need_InstructionSO> need_InteractionInstructionsOnInteractionTick = new();
    public List<Need_InstructionSO> Need_InteractionInstructionsOnInteractionTick { get { return need_InteractionInstructionsOnInteractionTick; } }
    [SerializeField, Foldout("ON INTERACTION END")]
    private List<Need_InstructionSO> need_InteractionInstructionsOnInteractionEnd = new();
    public List<Need_InstructionSO> Need_InteractionInstructionsOnInteractionEnd { get { return need_InteractionInstructionsOnInteractionEnd; } }

    [SerializeField, Foldout("ON INTERACTION BEGIN")]
    private List<Relationship_InstructionSO> relationshipChangeInstructionsOnInteractionBegin = new();
    public List<Relationship_InstructionSO> RelationshipChangeInstructionsOnInteraction { get { return relationshipChangeInstructionsOnInteractionBegin; } }
    [SerializeField, Foldout("ON INTERACTION END")]
    private List<Relationship_InstructionSO> relationshipChangeInstructionsOnInteractionEnd = new();
    public List<Relationship_InstructionSO> RelationshipChangeInstructionsOnInteractionEnd { get { return relationshipChangeInstructionsOnInteractionEnd; } }

    [SerializeField, Foldout("ON INTERACTION BEGIN")] 
    private List<Item_InstructionSO> itemChangeInstructionSOsOnInteractionBegin = new();
    public List<Item_InstructionSO> ItemChangeInstructionSOsOnInteractionBegin { get { return itemChangeInstructionSOsOnInteractionBegin; } }
    [SerializeField, Foldout("ON INTERACTION END")]
    private List<Item_InstructionSO> itemChangeInstructionSOsOnInteractionEnd = new();
    public List<Item_InstructionSO> ItemChangeInstructionSOsOnInteractionEnd { get { return itemChangeInstructionSOsOnInteractionEnd; } }

    [SerializeField, Foldout("ON INTERACTION BEGIN")]
    private List<Character_InstructionSO> characterInstructionSOsOnInteractionBegin = new();
    public List<Character_InstructionSO> CharacterInstructionSOsOnInteractionBegin { get { return characterInstructionSOsOnInteractionBegin; } }
    [SerializeField, Foldout("ON INTERACTION END")]
    private List<Character_InstructionSO> characterInstructionSOsOnInteractionEnd = new();
    public List<Character_InstructionSO> CharacterInstructionSOsOnInteractionEnd { get { return characterInstructionSOsOnInteractionEnd; } }


    //[SerializeField]
    //private List<InstructionData> itemInstructionDatas = new();
    //public List<InstructionData> ItemInstructionDatas { get { return itemInstructionDatas; } }


    [SerializeField, Tooltip("TBH KINDA DEPRICATED NGL! Way to use pre-existing interactions to build new ones. Example: Fridge spawns Food. Food has Pick Up interaction, which can be plased here to automatically  pick up the food on creation")]
    private List<SubInteraction> subInteractions = new();
    public List<SubInteraction> SubInteractions { get { return subInteractions; } }

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
    public List<InteractionSO> FollowupInteractionSOs { get { return followupInteractionSOs; } }



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


