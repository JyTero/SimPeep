using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionGroupSO", menuName = "Scriptable Objects/InteractionGroupSO")]
public class InteractionGroupSO : ScriptableObject
{
    [SerializeField]
    private string interactionName;
    public string InteractionName { get { return interactionName; } }

    [SerializeField, Tooltip("Use this to leave notes about the interaction, such as what are its planned owner items")]
    private string Description;

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
    private List<Item_InstructionSO> itemInstructionSOsOnInteractionBegin = new();
    public List<Item_InstructionSO> ItemInstructionSOsOnInteractionBegin { get { return itemInstructionSOsOnInteractionBegin; } }
    [SerializeField, Foldout("ON INTERACTION END")]
    private List<Item_InstructionSO> itemInstructionSOsOnInteractionEnd = new();
    public List<Item_InstructionSO> ItemInstructionSOsOnInteractionEnd { get { return itemInstructionSOsOnInteractionEnd; } }

    [SerializeField, Foldout("ON INTERACTION BEGIN")]
    private List<Character_InstructionSO> characterInstructionSOsOnInteractionBegin = new();
    public List<Character_InstructionSO> CharacterInstructionSOsOnInteractionBegin { get { return characterInstructionSOsOnInteractionBegin; } }
    [SerializeField, Foldout("ON INTERACTION END")]
    private List<Character_InstructionSO> characterInstructionSOsOnInteractionEnd = new();
    public List<Character_InstructionSO> CharacterInstructionSOsOnInteractionEnd { get { return characterInstructionSOsOnInteractionEnd; } }

    [SerializeField]
    private List<GroupedInteraction> groupedInteractions = new();
    public List<GroupedInteraction> GroupedInteractions { get { return groupedInteractions; } }
}
