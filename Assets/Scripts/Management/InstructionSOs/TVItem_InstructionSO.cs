using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TVItem_InstructionSO", menuName = "Scriptable Objects/Instruction/TVItem_InstructionSO")]
public class TVItem_InstructionSO : ItemSpecific_InstructionSO
{
    [SerializeField]
    private ETVInstruction thisInstructionType;
    public ETVInstruction ThisInstructionType { get {return thisInstructionType;}}

    private bool channel, switchPower;

    [SerializeField, ShowIf("channel")]
    private TVChannelSO tvChannelSO;
    public TVChannelSO TVChannelSO { get { return tvChannelSO; } }



    private void OnValidate()
    {
        switch (thisInstructionType)
        {
            case ETVInstruction.Default:
                break;
            case ETVInstruction.Channel:
                channel = true;
                switchPower = false;
                break;
            case ETVInstruction.SwitchPowerOn:
                channel = false;
                switchPower = true;
                break;
            case ETVInstruction.SwitchPowerOff:
                channel = false;
                switchPower = true;
                break;
            default:
                break;
        }
    }
}
public enum ETVInstruction
{
    Default,
    Channel,
    SwitchPowerOn,
    SwitchPowerOff,
}