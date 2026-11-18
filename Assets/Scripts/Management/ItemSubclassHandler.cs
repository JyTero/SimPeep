using System.Collections.Generic;
using UnityEngine;

public class ItemSubclassHandler : ManagerMono
{
    protected override void Start()
    {
        base.Start();

    }

    //INITIALISE
    public void InitialiseItem(ItemBase item)
    {
        switch (item)
        {
            case TV_Item tvItem:
                InitialiseTVItem(tvItem);
                break;
            default:
                break;
        }
    }

    private void InitialiseTVItem(ItemBase item)
    {
        TV_Item tv = (TV_Item)item;
        tv.RecordDefaultColor();

        int i = 0;
        foreach (TVItem_InstructionSO changeChannelInstructionSO in tv.ChangeChannelInstructionSOs)
        {

            StoredInteraction cc = new StoredInteraction(tv.TVChangeChannelSO, item);
            cc.InteractionData.SpecificItemInstructionSOsOnInteractionEnd.Add(changeChannelInstructionSO);

            if (i == 0)
                tv.ChangeTVChannel(changeChannelInstructionSO.TVChannelSO);

            //TODO: Rename the interaction
            tv.NewChangeChannelInteraction(changeChannelInstructionSO, cc);
            //item.NewStoredInteraction(cc);

            i++;
        }
        tv.TurnTvOff();
    }

    public List<StoredInteraction> GetItemInteractions(ItemBase item)
    {
        List<StoredInteraction> itemInteractions = new();
        switch (item)
        {
            case TV_Item tvItem:
                itemInteractions.AddRange(GetTVCapabilityInteractions(tvItem));
                break;
        }
        return itemInteractions;
    }

    //GET INTERACTIONS
    private List<StoredInteraction> GetTVCapabilityInteractions(ItemBase item)
    {
        TV_Item tv = (TV_Item)item;
        List<StoredInteraction> validInteractions = new();
        if (tv.TvState == ETVState.Off)
        {
            validInteractions.Add(new(tv.TVTurnOnSO, item));
        }
        else
        {
            validInteractions.Add(new(tv.TVTurnOffSO, item));
            foreach (TVItem_InstructionSO tvInstruction in tv.ChangeChannelInteractions.Keys)
            {
                if (tv.CurrentTVChannel == tvInstruction.TVChannelSO)
                    continue;
                validInteractions.Add(tv.ChangeChannelInteractions[tvInstruction]);

            }
        }
        return validInteractions;
    }
}
