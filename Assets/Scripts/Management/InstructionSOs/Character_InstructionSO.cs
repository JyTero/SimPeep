using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_InstructionSO", menuName = "Scriptable Objects/Instruction/Character_InstructionSO")]
public class Character_InstructionSO : InstructionSO
{
    [SerializeField]
    private ECharacterInstruction instructionType;
    public ECharacterInstruction InstructionType { get { return instructionType; } }

    [SerializeField, ShowIf("moveCharacter")]
    private ECharacterInstructionDestination destinationType;
    public ECharacterInstructionDestination DestinationType { get { return destinationType; } }
    
    private bool moveCharacter = false;
    private bool sitCharacter = false;

    private void OnValidate()
    {
        switch (instructionType)
        {
            case ECharacterInstruction.Default:
                sitCharacter = false;
                moveCharacter = false;
                break;
            case ECharacterInstruction.MoveCharacter:
                moveCharacter = true;
                sitCharacter = false;
                break;
            case ECharacterInstruction.SitCharacter:
                sitCharacter = true;
                moveCharacter = false;
                break;
            default:
                break;
        }
    }
}


public enum ECharacterInstruction
{
    Default,
    MoveCharacter,
    SitCharacter,
}
public enum ECharacterInstructionDestination
{
    Default,
    ThisItem,
}

