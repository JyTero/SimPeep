using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TVItem_InstructionSO", menuName = "Scriptable Objects/Instruction/TVItem_InstructionSO")]
public class TVItem_InstructionSO : ItemSpecific_InstructionSO
{
    [SerializeField]
    private TVChannelSO tvChannelSO;
    public TVChannelSO TVChannelSO { get { return tvChannelSO; } }
}
