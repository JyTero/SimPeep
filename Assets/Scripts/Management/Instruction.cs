using System;
using UnityEngine;

//[Serializable]
public abstract class Instruction
{  
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

public class RelationshipChange_Instruction : Instruction
{
    private Relationship_InstructionSO relInstructionSO;
    public Relationship_InstructionSO RelInstructionSO { get { return relInstructionSO; } }

    private Character sourceCharacter;
    public Character SourceCharacter { get { return sourceCharacter; } }

    private Character targetCharacter;
    public Character TargetCharacter { get { return targetCharacter; } }

    public RelationshipChange_Instruction(Relationship_InstructionSO relInstructionSO, Character sourceCharacter, Character targetCharacter)
    {
        this.relInstructionSO = relInstructionSO;
        this.sourceCharacter = sourceCharacter;
        this.targetCharacter = targetCharacter;
    }
}
public class Item_Instruction : Instruction
{
    private Item_InstructionSO itemInstructionSO;
    public Item_InstructionSO ItemInstructionSO { get { return itemInstructionSO; } }

    private Character thisCharacter;
    public Character ThisCharacter { get { return thisCharacter; } }

    private ItemBase thisItem;
    public ItemBase ThisItem { get { return thisItem; } }

    public Item_Instruction(Item_InstructionSO itemInstructionSO, Character thisCharacter, ItemBase thisItem)
    {
        this.itemInstructionSO = itemInstructionSO;
        this.thisCharacter = thisCharacter;
        this.thisItem = thisItem;
    }
}

