using System;
using UnityEngine;

public class ItemSpecificInstructionHandler : ManagerMono
{

    public void HandleItemSpecificInstruction(ItemSpecific_InstructionSO itemSpecificInstructionSO, ItemBase item)
    {
        switch (itemSpecificInstructionSO)
        {
            case TVItem_InstructionSO tvInstruction:
                HandleTVInstruction(tvInstruction, item);
                break;
        }
    }

    private void HandleTVInstruction(TVItem_InstructionSO tvInstruction, ItemBase item)
    {
        item.gameObject.GetComponent<MeshRenderer>().material.color = tvInstruction.TVChannelSO.ChannelColor;
    }
}
