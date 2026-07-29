using System;
using UnityEngine;

//[Serializable]
public abstract class Instruction
{
    //remember to add new lists to InteractionSO when creating new sub classes
    
    //[SerializeField]
    //protected float instructionTriggerTime;
    //public float InstructionTriggerTime { get { return instructionTriggerTime; } }

    
    public float InstructionLifetime;
    public float InstructionLenght;
    public float TimeSinceLastUse;
    

}
[Serializable]
public class Need_Instruction : Instruction
{
    [SerializeField]
    protected NeedType needToAdjust;
    public NeedType NeedToAdjust { get { return needToAdjust; } }

    //These to separate subclass, make all three subclass of needInstructin 
    //[SerializeField]
    //protected int needAdjustValueInitialBurst;
    //public int NeedAdjustValueInitialBurst { get { return needAdjustValueInitialBurst; } }

    //[SerializeField]
    //protected int needAdjustValueEndBurst;
    //public int NeedAdjustValueEndBurst { get { return needAdjustValueEndBurst; } }

    [SerializeField]
    protected int needAdjustValuePerTic;
    public int NeedAdjustValuePerTic { get { return needAdjustValuePerTic; } }

    //Runtime Data
    private Character needOwner;
    public Character NeedOwner { get { return needOwner; } }



    public Need_Instruction(Need_InstructionSO niso, Character chara)
    {
        needToAdjust = niso.NeedToAdjust;
        needAdjustValuePerTic = niso.NeedAdjustValuePerTic;
        needOwner = chara;

        InstructionLifetime = 0;
        InstructionLenght = 0; //Might get used later for something
        TimeSinceLastUse = 0;
    }
}