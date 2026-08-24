using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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
    }
    private void InitialiseTuckableChairCapability(TuckableChair_Capability tc, ItemBase item)
    {
        tc.CapabilityName = "TuckableChair_Capability";
        //If this chair is close enough to itemSlot made for dining chairs only
        LotGridTile lgt = lotManager.GetTileInteractableIsOn(item);
        if (lgt.itemSlotOnTile != null)
        {
            if (lgt.itemSlotOnTile.ValidItemSO == item.ItemData)
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
    public void HandleCapability(ItemCapability capability, Character character)
    {
        switch (capability)
        {
            case StoveCapability stove:
                HandleStoveCapability(stove, character);
                break;
            case Sittable_Capability sittable:
                HandleSittableCapability(sittable, character);
                break;
            case DiningTable_Capability dining:
                HandleDiningTableCapability(dining, character);
                break;
            case TuckableChair_Capability tuck:
                HandleTuckableChairCapability(tuck, character);
                break;
            case SpawnItem_Capability spawnItem:
                HandleSpawnCapability(spawnItem, character);
                break;
        }
    }
    private void HandleStoveCapability(StoveCapability stoveCapability, Character character)
    {
        itemManager.PlaceCarriedItemToSlot(character, stoveCapability.StoveCookSlot);
    }
    private void HandleSittableCapability(Sittable_Capability sittable, Character character)
    {
        //TODO: Check if slot is free, us another if not, cancel interaction should all else fail
        if (character.CharacterPhysicalState == CharacterPhysicalStateEnum.SittingOnObject)
            CharacterControl.StandUpFromSlot(character, sittable.SitSlot);
        else
            CharacterControl.SitCharacterToSlot(character, sittable.SitSlot);
    }
    private void HandleDiningTableCapability(DiningTable_Capability dining, Character character)
    {

    }
    private void HandleTuckableChairCapability(TuckableChair_Capability tc, Character character)
    {

    }
    private void HandleSpawnCapability(SpawnItem_Capability spawnItem, Character character)
    {
        ItemBase spawnedItem = itemManager.GetItemCreatedByInteraction(character);
        if(spawnedItem != null)
        {
            itemManager.PlaceItemToSlot(spawnedItem, spawnItem.SpawnSlot);
            character.PickupItem(spawnedItem);
        }
    }
}
