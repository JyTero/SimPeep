using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    public bool CanItemInstructionRun(Item_InstructionSO instructionSO, Character character)
    {
        switch (instructionSO.InstructionType)
        {
            case EItem_InstructionType.Default:
                Debug.LogError($"Unknown item instruction {instructionSO.name}");
                return false;
            //case EItem_InstructionType.Spawn:
            //    break;
            //case EItem_InstructionType.Destroy:
            //    break;
            //case EItem_InstructionType.MoveThis:
            //    break;
            //case EItem_InstructionType.MoveFromThis:
            //    break;
            //case EItem_InstructionType.Replace:
            //    break;
            case EItem_InstructionType.Routine:
                return CanRunRoutineInstruction(instructionSO, character);
            //case EItem_InstructionType.RunInteraction:
            //    break;
        }
        return false;
    }

    private bool CanRunRoutineInstruction(Item_InstructionSO instructionSO, Character character)
    {
        switch (instructionSO.Routine)
        {
            case ERoutine.Default:
                Debug.LogError($"Default ERoutine on {instructionSO.name}");
                return false;
            case ERoutine.UseTableWithSeating:
                return RunUseTableWithSeatingRoutine(instructionSO, character);
        }
        return false;
    }

    private bool RunUseTableWithSeatingRoutine(Item_InstructionSO instructionSO, Character character)
    {
        SeatingWithTableData swtd = characterAIHandler.FindSeatWithTable(character);
        if (swtd == null)
            return false;
        else if (!swtd.Table || !swtd.Chair || !swtd.OnTableSlot)
            return false;
        else
            return true;
    }
}
