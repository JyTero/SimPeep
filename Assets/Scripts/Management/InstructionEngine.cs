using System;
using UnityEngine;

public class InstructionEngine : ManagementCore
{

    protected override void Start()
    {
        base.Start();
    }

    //Build Instruction to Place Held to KnownSlot
    public Item_InstructionData BuildItemInstructionData(EItem_InstructionType instructionType, EItemDestination destination, EItemLocation destinationSource)
    {
        return new Item_InstructionData(instructionType, destination, destinationSource);
    }

    //Build Instruction to run givenStored Interaction
    public Item_InstructionData BuildItemInstructionData(EItem_InstructionType instructionType, StoredInteraction storedInteraction)
    {
        return new Item_InstructionData(instructionType, storedInteraction);
    }

}
