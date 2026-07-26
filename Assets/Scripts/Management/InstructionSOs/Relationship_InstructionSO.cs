using UnityEngine;

[CreateAssetMenu(fileName = "Relationship_InstructionSO", menuName = "Scriptable Objects/Instruction/RelationShip_InstructionSO")]
public class Relationship_InstructionSO : InstructionSO
{
    [SerializeField]
    protected int relationshipScoreChange;
    public int RelationshipScoreChange {  get { return relationshipScoreChange; } }

    //Set/remove/progress/hinder relationship flags (crush, love, hate, friend)
}
