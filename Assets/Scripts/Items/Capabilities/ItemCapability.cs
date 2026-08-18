using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class ItemCapability : MonoBehaviour
{

    [SerializeField]
    protected ItemCapabilites thisCapability;
    public ItemCapabilites ThisCapability { get { return thisCapability; } }

    protected string capablityName;
    protected List<List<Instruction>> instructionLists= new();


    //Each instruction variant has its own list 
    [SerializeField, Foldout("On Begin Instructions")]
    private List<Need_InstructionSO> onBeginNeed_InteractionInstructions = new();
    public List<Need_InstructionSO> OnBeginNeed_InteractionInstructions { get { return onBeginNeed_InteractionInstructions; } }

    [SerializeField, Foldout("On Begin Instructions")]
    private List<Relationship_InstructionSO> onBeginRelationshipChangeInstructions = new();
    public List<Relationship_InstructionSO> OnBeginRelationshipChangeInstructions { get { return onBeginRelationshipChangeInstructions; } }

    [SerializeField, Foldout("On Begin Instructions")]
    private List<Item_InstructionSO> onBeginItemChangeInstructions = new();
    public List<Item_InstructionSO> OnBeginItemChangeInstructions { get { return onBeginItemChangeInstructions; } }


    [SerializeField, Foldout("On Begin Instructions")]
    private List<SubInteraction> onBeginSubInteractionSOs = new();
    public List<SubInteraction> OnBeginSubInteractionSOs { get { return onBeginSubInteractionSOs; } }



    [SerializeField, Foldout("On End Instructions")]
    private List<Need_InstructionSO> onEndNeed_InteractionInstructions = new();
    public List<Need_InstructionSO> OnEndNeed_InteractionInstructions { get { return onEndNeed_InteractionInstructions; } }

    [SerializeField, Foldout("On End Instructions")]
    private List<Relationship_InstructionSO> onEndRelationshipChangeInstructions = new();
    public List<Relationship_InstructionSO> OnEndRelationshipChangeInstructions { get { return onEndRelationshipChangeInstructions; } }

    [SerializeField, Foldout("On End Instructions")]
    private List<Item_InstructionSO> onEndItemChangeInstructions = new();
    public List<Item_InstructionSO> OnEndItemChangeInstructions { get { return onEndItemChangeInstructions; } }

    [SerializeField, Foldout("On End Instructions")]
    private List<SubInteraction> onEndSubInteractionSOs = new();
    public List<SubInteraction> OnEndSubInteractionSOs { get { return onEndSubInteractionSOs; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

    }
}
