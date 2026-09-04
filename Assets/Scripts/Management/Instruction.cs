using System;
using UnityEngine;

//[Serializable]
public abstract class Instruction
{
    protected string instructionName = "";
    public string InstructionName { get { return instructionName; } }

    public float InstructionLifetime;
    public float InstructionLenght;
    public float TimeSinceLastUse;
    
protected void InstructName(string name)
    {
        instructionName = name;
    }
}

[Serializable]
public class Need_Instruction : Instruction
{
    public Need_InstructionSO sourceSO;

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



    public Need_Instruction(Need_InstructionSO niso, Character chara, string itemAndInteractionName)
    {
        needToAdjust = niso.NeedToAdjust;
        needAdjustValuePerTic = niso.NeedAdjustValuePerTic;
        needOwner = chara;

        InstructionLifetime = 0;
        InstructionLenght = 0; //Might get used later for something
        TimeSinceLastUse = 0;
        this.sourceSO = niso;

        string n = chara.ItemName + itemAndInteractionName;
        InstructName(n);
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

        string n = sourceCharacter.ItemName + targetCharacter; //Not as relevant (for now) hence lackluster implementation
        InstructName(n);
    }
}
public class Item_Instruction : Instruction
{
    private Item_InstructionData itemInstructionData;
    public Item_InstructionData ItemInstructionData { get { return itemInstructionData; } }

    private Character thisCharacter;
    public Character ThisCharacter { get { return thisCharacter; } }

    private ItemBase thisItem;
    public ItemBase ThisItem { get { return thisItem; } }

    //public Item_Instruction(Item_InstructionSO itemInstructionSO, Character thisCharacter, ItemBase thisItem)
    //{
    //    this.itemInstructionData = itemInstructionSO;
    //    this.thisCharacter = thisCharacter;
    //    this.thisItem = thisItem;
    //}
    public Item_Instruction(Item_InstructionData itemInstructionData, Character thisCharacter, ItemBase thisItem)
    {
        this.itemInstructionData = itemInstructionData;
        this.thisCharacter = thisCharacter;
        this.thisItem = thisItem;
    }
}

