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
        switch (tvInstruction.ThisInstructionType)
        {
            case ETVInstruction.Default:
                break;
            case ETVInstruction.Channel:
                ChangeTVChannel(tvInstruction, item);
                break;
            case ETVInstruction.SwitchPowerOn:
                TurnTvOn(tvInstruction, item);
                break;
            case ETVInstruction.SwitchPowerOff:
                TurnTvOff(tvInstruction, item);
                break;
            default:
                break;
        }

    }

    private void ChangeTVChannel(TVItem_InstructionSO tvInstruction, ItemBase item)
    {
        TV_Item tv = item as TV_Item;
        tv.ChangeTVChannel(tvInstruction.TVChannelSO);

    }
    private void TurnTvOff(TVItem_InstructionSO tvInstruction, ItemBase item)
    {
        TV_Item tv = item as TV_Item;
        if(tv.TvState == ETVState.OnChannel)
            tv.TurnTvOff();

    }
    private void TurnTvOn(TVItem_InstructionSO tvInstruction, ItemBase item)
    {
        TV_Item tv = item as TV_Item;
        if (tv.TvState == ETVState.Off)
        {
            tv.TurnTvOn();
        }

    }

}
