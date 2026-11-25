using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        //Interactions/State Prep
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

        //SitSlots
        List<LotGridTile> interactionAreaTiles = lotManager.GetTilesInItemInteractionArea(item);
        List<ItemBase> handledItems = new();

        //foreach(LotGridTile lgt in interactionAreaTiles)
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go.GetComponent<MeshRenderer>().material.color = Color.blue;
        //    go.transform.position = lgt.TilePos;
        //}

        foreach (LotGridTile lgt in interactionAreaTiles)
        {
            if (lgt.InteractableOnTile == null)
                continue;

            if (lgt.InteractableOnTile is not ItemBase)
                continue;

            ItemBase itemOnTile = lgt.InteractableOnTile as ItemBase;
            if (handledItems.Contains(itemOnTile))
                continue;
            if (itemOnTile.CharacterSlotsOnItem.Count == 0)
                continue;
            foreach (Character_Slot cSlot in itemOnTile.CharacterSlotsOnItem)
            {
                if (cSlot.SlotType == scriptableObjectReferences.SittingSlotTypeSO)
                    tv.NewSittingSlotsInArea(cSlot);

            }
            handledItems.Add(itemOnTile);
        }
    }

    //GET INTERACTIONS
    public List<StoredInteraction> GetItemInteractions(ItemBase item)
    {
        List<StoredInteraction> itemInteractions = new();
        switch (item)
        {
            case TV_Item tvItem:
                itemInteractions.AddRange(GetTVInteractions(tvItem));
                break;
        }
        return itemInteractions;
    }

    private List<StoredInteraction> GetTVInteractions(ItemBase item)
    {
        TV_Item tv = (TV_Item)item;
        List<StoredInteraction> validInteractions = new();
        if (tv.TvState == ETVState.Off)
        {
            validInteractions.Add(new(tv.TVTurnOnSO, item));
            validInteractions.Add(new(tv.WatchTVSO, item));
        }
        else
        {
            validInteractions.Add(new(tv.TVTurnOffSO, item));
            validInteractions.Add(new(tv.WatchTVSO, item));

            foreach (TVItem_InstructionSO tvInstruction in tv.ChangeChannelInteractions.Keys)
            {
                if (tv.CurrentTVChannel == tvInstruction.TVChannelSO)
                    continue;
                validInteractions.Add(tv.ChangeChannelInteractions[tvInstruction]);

            }
        }
        return validInteractions;
    }

    //CUSTOM LOGIC
    public void RunCustomLogic(CustomLogic_InstructionSO customLogicSO, ActiveInteraction interaction)
    {
        int id = customLogicSO.CustomLogicIndex;
        //Figure out item

        switch (customLogicSO.CustomlogicItem)
        {
            case CustomLogic_InstructionSO.ECustomLogicItem.Default:
                Debug.LogWarning($"Tried to call Custom Logic, Default item type! Was this intentional?\n" +
    $"Interaction: {interaction.InteractionName} Character: {interaction.ThisCharacter.ItemName} Source: {interaction.InteractionSource} CustomLogic ID: {id}");
                break;
            case CustomLogic_InstructionSO.ECustomLogicItem.TV:
                if (interaction.InteractionSource is TV_Item)
                {
                    HandleTVCustomLogic(id, interaction);
                    break;
                }
                else
                    throw new Exception($"Mismatching CustomLogicItem and interactionsource ({customLogicSO.CustomlogicItem} vs {interaction.InteractionSource.GetType()})");
            default:
                break;
        }

        //Figure out Custom logic
    }
    private void HandleTVCustomLogic(int id, ActiveInteraction interaction)
    {
        switch ((ETVCustomLogic)id)
        {
            case ETVCustomLogic.Default:
                throw new Exception($"Default CustomLogic ID on TV interaction: {interaction.InteractionName}, character: {interaction.ThisCharacter}");
            case ETVCustomLogic.TurnTVOnIfNot:
                TurnTVOnIfNot(interaction);
                break;
        }
    }
    private void TurnTVOnIfNot(ActiveInteraction interaction)
    {
        TV_Item tv = interaction.InteractionSource as TV_Item;
        if (tv.TvState == ETVState.Off)
        {
            Debug.Log($"This is custom logic! It is here to handle turning TV on within Watch interaction");
        }
    }
}

public enum ETVCustomLogic
{
    Default,
    TurnTVOnIfNot,

}
