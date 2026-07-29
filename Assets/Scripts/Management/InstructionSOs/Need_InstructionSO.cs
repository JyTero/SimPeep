using UnityEngine;

[CreateAssetMenu(fileName = "Need_InstructionSO", menuName = "Scriptable Objects/Instruction/Need_InstructionSO")]
public class Need_InstructionSO : InstructionSO
{
    [SerializeField]
    private NeedType needToAdjust;
    public NeedType NeedToAdjust { get { return needToAdjust; } }

    //These to separate using dropdown and hideif/showif

    //[SerializeField, Tooltip("Positive increases, negative decreases need.")]
    //private int needAdjustValueInitialBurst;
    //public int NeedAdjustValueInitialBurst { get { return needAdjustValueInitialBurst; } }

    //[SerializeField, Tooltip("Positive increases, negative decreases need.")]
    //private int needAdjustValueEndBurst;
    //public int NeedAdjustValueEndBurst { get { return needAdjustValueEndBurst; } }

    [SerializeField, Tooltip("Positive increases, negative decreases need.")]
    private int needAdjustValuePerTic;
    public int NeedAdjustValuePerTic { get { return needAdjustValuePerTic; } }
}
