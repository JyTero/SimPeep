using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CapabilityHandler : ManagementCore
{
    protected override void Start()
    {
        base.Start();

    }

    public void InitialiseItemCapabilities(ItemBase item)
    {
        foreach (ItemCapabilites capability in item.Capabilites)
        {
            foreach (ItemCapability ic in item.CapabilityComponents)
            {
                if (ic.ThisCapability == capability)
                {
                    item.CapabilitiesByEnum.Add(capability, ic);
                    InitialiseCapability(ic, item);

                }
            }
        }
    }

    private void InitialiseCapability(ItemCapability capability, ItemBase item)
    {
        switch (capability)
        {
            case StoveCapability stove:
                InitialiseStoveCapability(stove, item);
                break;
            case Sittable_Capability sittable:
                InitialiseSittableCapability(sittable, item);
                break;
            case DiningTable_Capability dining:
                InitialiseDiningTableCapability(dining, item);
                break;
            case TuckableChair_Capability tuckableChair:
                InitialiseTuckableChairCapability(tuckableChair, item);
                break;
            case SpawnItem_Capability spawnItem:
                InitialiseSpawnCapability(spawnItem, item);
                break;
        }
    }
    private void InitialiseStoveCapability(StoveCapability stoveCapability, ItemBase item)
    {
        stoveCapability.CapabilityName = "StoveCapability";
        stoveCapability.StoveCookSlot.InitialiseSlot(item);

        item.ItemSlotsOnItem.Add(stoveCapability.StoveCookSlot);
    }
    private void InitialiseSittableCapability(Sittable_Capability sittable, ItemBase item)
    {
        sittable.CapabilityName = "SittableCapability";
        sittable.SitSlot.InitialiseSlot(item);

        item.CharacterSlotsOnItem.Add(sittable.SitSlot);
    }
    private void InitialiseDiningTableCapability(DiningTable_Capability dining, ItemBase item)
    {
        dining.CapabilityName = "DiningTable_Capability";
        foreach (Item_Slot iSlot in dining.ChairSlots)
        {
            iSlot.InitialiseSlot(item);
            item.ItemSlotsOnItem.Add(iSlot);
            LotGridTile lgt = lotManager.GetLotTile(iSlot.SlotTransform.position);
            lgt.SlotOnTile(iSlot);
        }
        int i = 0;
        foreach(Item_Slot slot in dining.OnTableSlots)
        {
            slot.InitialiseSlot(item);
            item.ItemSlotsOnItem.Add(slot);

            dining.NewChairTableSlotPair(dining.ChairSlots[i], slot);
            i++;
        }
    }
    private void InitialiseTuckableChairCapability(TuckableChair_Capability tc, ItemBase item)
    {
        tc.CapabilityName = "TuckableChair_Capability";
        //If this chair is close enough to itemSlot made for dining chairs only
        LotGridTile lgt = lotManager.GetTileInteractableIsOn(item);
        if (lgt.itemSlotOnTile != null)
        {
            if (lgt.itemSlotOnTile.SlotType.ValidItemSO.Contains(item.ItemData))
            {
                lgt.itemSlotOnTile.PlaceItemToSlot(item);
                tc.TieChairToTable(lgt.itemSlotOnTile.ParentItem);
            }

        }
    }
    private void InitialiseSpawnCapability(SpawnItem_Capability spawnItem, ItemBase item)
    {
        spawnItem.CapabilityName = "SpawnItem_Capability";
        spawnItem.SpawnSlot.InitialiseSlot(item);

        item.ItemSlotsOnItem.Add(spawnItem.SpawnSlot);
    }

    public void HandleCapabilityBegin(ItemCapability capability, Character character)
    {
        switch (capability)
        {
            case StoveCapability stove:
                HandleStoveCapabilityBegin(stove, character);
                break;
            case Sittable_Capability sittable:
                HandleSittableCapabilityBegin(sittable, character);
                break;
            case DiningTable_Capability dining:
                HandleDiningTableCapabilityBegin(dining, character);
                break;
            case TuckableChair_Capability tuck:
                HandleTuckableChairCapabilityBegin(tuck, character);
                break;
            case SpawnItem_Capability spawnItem:
                HandleSpawnCapabilityBegin(spawnItem, character);
                break;
        }
    }
    private void HandleStoveCapabilityBegin(StoveCapability stoveCapability, Character character)
    {
        itemManager.PlaceCarriedItemToSlot(character, stoveCapability.StoveCookSlot);
    }
    private void HandleSittableCapabilityBegin(Sittable_Capability sittable, Character character)
    {
        //TODO: Check if slot is free, us another if not, cancel interaction should all else fail
        if (character.CharacterPhysicalState == CharacterPhysicalStateEnum.SittingOnObject)
            characterControl.StandUpFromSlot(character, sittable.SitSlot);
        else
            characterControl.SitCharacterToSlot(character, sittable.SitSlot);
    }
    private void HandleDiningTableCapabilityBegin(DiningTable_Capability dining, Character character)
    {

    }
    private void HandleTuckableChairCapabilityBegin(TuckableChair_Capability tc, Character character)
    {

    }
    private void HandleSpawnCapabilityBegin(SpawnItem_Capability spawnItem, Character character)
    {
        ItemBase spawnedItem = itemManager.GetItemCreatedByInteraction(character);
        if(spawnedItem != null)
        {
            itemManager.PlaceItemToSlot(spawnedItem, spawnItem.SpawnSlot);
            character.PickupItem(spawnedItem);
        }
    }

    public void HandleCapabilityEnd(ItemCapability capability, Character character)
    {
        switch (capability)
        {
            case StoveCapability stove:
                break;
            case Sittable_Capability sittable:
                break;
            case DiningTable_Capability dining:
                break;
            case TuckableChair_Capability tuck:
                break;
            case SpawnItem_Capability spawnItem:
                break;
        }
    }
}
