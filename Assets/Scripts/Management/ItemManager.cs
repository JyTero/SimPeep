using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Item_InstructionSO;

public class ItemManager : ManagementCore
{
    private List<ItemBase> allItems = new();
    public List<ItemBase> AllItems { get { return allItems; } }

    private Dictionary<ItemBase, Transform> itemsMoving = new();
    public Dictionary<ItemBase, Transform> ItemsMoving { get { return itemsMoving; } }

    private Dictionary<Character, ItemBase> itemsCreatedByInteraction = new();

    private List<ItemBase> unreadyItems = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

    }

    public ItemBase SpawnNewItem(GameObject prefab, WorldLot lot, Vector3 spawnPos)
    {
        ItemBase returne = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<ItemBase>();
        returne.ChangeCurrentLot(lot);
        InitialiseItem(returne);

        // unreadyItems.Add(returne);
        return returne;

    }
    public void DestroyItem(ItemBase item)
    {
        allItems.Remove(item);
        lotManager.RemoveItemFromLot(item);
        Destroy(item.gameObject);
    }

    public void InitialiseItem(ItemBase item) //Move to ItemManager
    {
        ItemSO itemData = item.ItemData;
        if (item.ItemName == "")

            item.ItemName = itemData.ItemName;

        //if(itemDescription == "")
        //itemDescription  = itemData.ItemDescription;

        item.ItemPrice = itemData.ItemPrice;
        foreach (InteractionSO iso in itemData.AllInteractions)
        {
            item.InteractionSOs.Add(iso);
        }

        capabilityHandler.InitialiseItemCapabilities(item);

        BuildStoredInteractions(item);
        //foreach (InteractionSO itso in item.InteractionSOs)
        //{
        //    item.NewStoredInteraction(new StoredInteraction(itso, item));
        //}

        //IF(WithinLotGrid)
        //Place onto center of nearest tile

        //Slot
        foreach (Item_Slot slot in item.ItemSlotsOnItem)
        {
            slot.InitialiseSlot(item);
            item.InitialiseSlot(slot);
        }

        item.itemInitialised = true;
    }

    public StoredInteraction GetInteractionOnInteractable(InteractionSO itsoTemplate, Interactable interactable)
    {
        foreach (StoredInteraction storedInteraction in interactable.StoredInteractions)
        {
            if (storedInteraction.InteractionTuningSO == itsoTemplate)
                return storedInteraction;
        }
        return null;
    }

    public void BuildStoredInteractions(ItemBase item)
    {
        foreach (InteractionSO itso in item.InteractionSOs)
        {
            item.NewStoredInteraction(new StoredInteraction(itso, item));
        }

        foreach(InteractionGroupSO itgSO in item.ItemData.InteractionGroupSOs)
        {
            StoredInteraction si = new StoredInteraction(itgSO.GroupedInteractions[0].InteractionSO, item);
            si.MakeIntoStoredInteractionGroup(itgSO);
            item.NewStoredInteraction(si);
        }
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);

        if (ItemsMoving.Count > 0)
            UpdateMovingItems();
        if (unreadyItems.Count > 0)
            UpdateUnreadyItems();
    }

    private void UpdateMovingItems()
    {
        foreach (ItemBase item in ItemsMoving.Keys)
        {
            item.transform.position = ItemsMoving[item].transform.position;
        }
    }
    private void UpdateUnreadyItems()
    {
        for (int i = unreadyItems.Count - 1; i >= 0; i--)
        {
            ItemBase item = unreadyItems[i];

            if (!item.itemInitialised)
                continue;
            else
            {
                allItems.Add(item);
                BuildStoredInteractions(item);
                lotManager.NewItemOnLot(item);
                unreadyItems.Remove(item);
            }
        }
    }

    //public void HandleItemInstructionSO(Item_InstructionSO instructionSO, ActiveInteraction interaction)
    //{
    //    HandleItemInstruction(new Item_Instruction(instructionSO, interaction.ThisCharacter, interaction.InteractionSource as ItemBase), interaction);
    //}
    public void HandleItemInstructionData(Item_InstructionData instructionData, ActiveInteraction interaction)
    {
        HandleItemInstruction(new Item_Instruction(instructionData, interaction.ThisCharacter, interaction.InteractionSource as ItemBase), interaction);
    }

    private void HandleItemInstruction(Item_Instruction itemInstruction, ActiveInteraction interaction)
    {
        Character thisCharacter = itemInstruction.ThisCharacter;
        ItemBase thisItem = itemInstruction.ThisItem;
        Item_InstructionData instructionData = itemInstruction.ItemInstructionData;

        switch (itemInstruction.ItemInstructionData.InstructionType)
        {
            case EItem_InstructionType.Default:
                Debug.LogError($"Unknown item instruction {instructionData.InstructionName}");
                break;
            case EItem_InstructionType.Spawn:
                SpawnItem(instructionData, thisCharacter, thisItem);
                break;
            case EItem_InstructionType.Destroy:
                DestroyItem(thisItem);
                break;
            case EItem_InstructionType.MoveThis:
                MoveThisItem(instructionData, interaction);
                break;
            case EItem_InstructionType.MoveFromThis:
                MoveFromThisItemSlot(itemInstruction, interaction);
                break;
            case EItem_InstructionType.Replace:
                ReplaceItem(instructionData, thisCharacter, thisItem);
                break;
            case EItem_InstructionType.Routine:
                RunRoutine(instructionData, interaction);
                break;
            case EItem_InstructionType.RunInteraction:
                RunInteraction(instructionData, interaction);
                break;
            default:
                break;
        }

        //if (instructionData.SpawnItem)
        //{
        //    //HandleItemSPawning
        //    SpawnItem(instructionData, thisCharacter, thisItem);
        //}
        //else if (instructionData.DestroyItem)
        //{
        //    DestroyItem(thisItem);
        //}
        //else if (instructionData.MoveThisItem)
        //{
        //    MoveThisItem(instructionData, interaction);
        //}
        //else if (instructionData.MoveFromThisItemSlot)
        //{
        //    MoveFromThisItemSlot(itemInstruction, interaction);
        //}
        //else if (instructionData.ReplaceItem)
        //{
        //    ReplaceItem(instructionData, thisCharacter, thisItem);
        //}
        //else if (instructionData.RunRoutine)
        //{
        //    RunRoutine(instructionData, interaction);
        //}
        //else if (instructionData.RunInteractionAsInstruction)
        //{
        //    RunInteraction(instructionData, interaction);
        //}
        //else
        //    Debug.LogError($"Unknown item instruction {instructionData.InstructionName}");


    }

    public void HandleItemInstructions(List<Item_Instruction> itemInstructions, ActiveInteraction interaction)
    {
        foreach (Item_Instruction itemInstruction in itemInstructions)
        {
            HandleItemInstruction(itemInstruction, interaction);
        }
    }

    private void SpawnItem(Item_InstructionData instructionData, Character character, ItemBase item)
    {
        ItemBase ib = SpawnNewItem(instructionData.ItemToSpawn, character.ThisLot, item.transform.position);
        itemsCreatedByInteraction.Add(character, ib);
        switch (instructionData.WhereToSpawnItem)
        {
            case EItemDestination.Default:
                break;
            case EItemDestination.LotSpace:
                break;
            case EItemDestination.WorldSpace:
                break;
            case EItemDestination.InCharactacter:
                break;
            case EItemDestination.OnCharacter:
                break;
            case EItemDestination.ItemSlot:
                Item_Slot slot = GetSlotOnItemByType(instructionData.SlotTypeSOSpwn, item);
                PlaceItemToSlot(ib, slot);
                break;
            default:
                break;
        }

    }

    private void MoveThisItem(Item_InstructionData instructionData, ActiveInteraction interaction)
    {
        Character thisCharacter = interaction.ThisCharacter;
        ItemBase thisItem = interaction.InteractionSource as ItemBase;

        switch (instructionData.WhereToMoveItem)
        {
            case EItemDestination.Default:
                break;
            case EItemDestination.LotSpace:
                if (thisItem == thisCharacter.CarriedItem)
                    characterControl.SeparateItemFromHand(thisCharacter);
                lotManager.PlaceItemOntoLot(thisCharacter.ThisLot, thisCharacter.CurrentTile, thisItem);
                break;
            case EItemDestination.WorldSpace:

                break;
            case EItemDestination.InCharactacter:
                break;
            case EItemDestination.OnCharacter:
                characterControl.PickupItem(interaction, thisItem);
                break;
            case EItemDestination.ItemSlot:
                MoveItemToItemSlot(instructionData, interaction);
                break;
            default:
                break;
        }
    }
    private void MoveItemToItemSlot(Item_InstructionData instructionData, ActiveInteraction interaction)
    {

        Item_Slot slot;
        ItemBase slotParent;
        Character thisCharacter = interaction.ThisCharacter;
        ItemBase thisItem = interaction.InteractionSource as ItemBase;

        if (instructionData.TargetSlotType != null)
        {
            slotParent = GetItemByItemLocationEnum(instructionData, thisCharacter, thisItem);
            slot = slotParent.ItemSlotsByType[instructionData.TargetSlotType][0];
        }
        else if (instructionData.SlotParentItemLocationMove == EItemLocation.RuntimeKnown)
        {
            slot = interaction.knownSlot as Item_Slot;
        }
        else
        {
            List<Item_Slot> suitableSlots = lotManager.FindSuitableSlotsOnLot(thisItem, thisCharacter.ThisLot);
            slot = suitableSlots[0];
        }

        if (thisCharacter.CarriedItem == thisItem)
        {
            if (lotManager.GetNeighboringTiles(thisCharacter.CurrentTile).Contains(slot.ParentItem.CurrentTile))
                PlaceCarriedItemToSlot(thisCharacter, slot); // Place
            else
                MoveForSlot(interaction, slot);
            //ActiveInteraction action = interactionEngine.BuildAction(thisItem. )
        }
        else
            PlaceItemToSlot(thisItem, slot);
    }
    private void MoveFromThisItemSlot(Item_Instruction itemInstruction, ActiveInteraction interaction)
    {
        Character thisCharacter = itemInstruction.ThisCharacter;
        ItemBase thisItem = itemInstruction.ThisItem;
        Item_InstructionData instructionData = itemInstruction.ItemInstructionData;

        ItemBase movingItem = GetItemOnSlotTypeSlot(thisItem, instructionData.SlotTypeToPickFrom);
        Item_Slot slot = thisItem.ItemSlotsByType[instructionData.SlotTypeToPickFrom][0]; //TODO, Logic to select slot
        switch (instructionData.WhereToMoveItem)
        {
            case EItemDestination.Default:
                break;
            case EItemDestination.LotSpace:
                break;
            case EItemDestination.WorldSpace:
                break;
            case EItemDestination.InCharactacter:
                break;
            case EItemDestination.OnCharacter:
                characterControl.PickupItem(interaction, movingItem);
                RemoveItemFromSlot(slot, movingItem);
                break;
            case EItemDestination.ItemSlot:
                Item_Slot dSlot = GetSlotOnItemByType(instructionData.SlotTypeSOSpwn, movingItem);
                PlaceItemToSlot(movingItem, dSlot);
                break;
            default:
                break;
        }
    }
    private void ReplaceItem(Item_InstructionData instructionData, Character thisCharacter, ItemBase thisItem)
    {
        //item on slot
        if (thisItem.OccupiedSlot != null)
        {
            Item_Slot slot = thisItem.OccupiedSlot;

            ItemBase newItem = SpawnNewItem(instructionData.NewItemPrefab, thisItem.ThisLot, NegSpawnPos);
            RemoveItemFromSlot(slot, thisItem);
            DestroyItem(thisItem);
            PlaceItemToSlot(newItem, slot);
        }
        else
        {
            LotGridTile itemTile = lotManager.GetTileInteractableIsOn(thisItem);
            if (itemTile.itemOnTile == thisItem)
            {
                ItemBase newItem = SpawnNewItem(instructionData.NewItemPrefab, thisItem.ThisLot, NegSpawnPos);

                lotManager.RemoveItemFromTile(thisItem, itemTile);
                DestroyItem(thisItem);
                lotManager.PlaceItemToTile(newItem, itemTile);
            }
        }
    }

    private void MoveForSlot(ActiveInteraction interaction, Item_Slot slot)
    {
        interaction.State.itemIndex--;
        interaction.PushInteractionState(EInteractionState.Moving);
        characterControl.RouteToTile(interaction.ThisCharacter, slot.ParentItem.CurrentTile);

    }

    private void RunRoutine(Item_InstructionData instructionData, ActiveInteraction interaction)
    {
        interaction.PushInteractionState(EInteractionState.Routine);

        switch (instructionData.Routine)
        {
            case ERoutine.Default:
                Debug.LogError($"Default ERoutine on {instructionData.InstructionName} of {interaction.InteractionSource.ItemName}");
                break;
            case ERoutine.UseTableWithSeating:
                RunUseTableWithSeatingRoutine(interaction);
                break;
        }
    }
    private void RunInteraction(Item_InstructionData instructionData, ActiveInteraction interaction)
    {
       // interaction.PushInteractionState(EInteractionState.Instruction);
        if (instructionData.InteractionToRunSO != null)
        {
            StoredInteraction si = lotManager.FindSuitableStoredInteractionOnLot(instructionData.InteractionToRunSO, interaction.ThisCharacter.ThisLot);
            interactionEngine.PrepareInstructionInteraction(interaction, si.InteractionTuningSO, si.InteractionSource);
        }
        else
        {
            interactionEngine.PrepareInstructionInteraction(interaction, instructionData.InteractionToRunStored.InteractionTuningSO, instructionData.InteractionToRunStored.InteractionSource);
        }
    }


    private void RunUseTableWithSeatingRoutine(ActiveInteraction interaction)
    {
        //DEBUG
        SeatingWithTableData swtd = characterAIHandler.FindSeatWithTable(interaction.ThisCharacter);
        interaction.knownItem = swtd.Chair;
        interaction.knownSlot = swtd.OnTableSlot;

        StoredInteraction pickUpStored = interaction.InteractionSource.StoredInteractions.First(si => si.InteractionTuningSO.name == "PickUp_InteractionSO"); //Food, PickUP 
        StoredInteraction sitStored = interaction.knownItem.StoredInteractions.First(si => si.InteractionTuningSO.name == "Sit_InteractionSO"); //Chair, SitSO


        //Build&queue routineInteractions
        //Item_InstructionData pickUp = instructionEngine.BuildItemInstructionData(EItem_InstructionType.MoveThis, pickUpStored);
        Item_InstructionData pickUp = instructionEngine.BuildItemInstructionData(EItem_InstructionType.RunInteraction, pickUpStored);
        Item_InstructionData placeToSlot = instructionEngine.BuildItemInstructionData(EItem_InstructionType.MoveThis, EItemDestination.ItemSlot, EItemLocation.RuntimeKnown);
        //FIGURE OUT: HOW TO HAVE INTERACTIONSO HERE (Database?)-->
        Item_InstructionData sitOnChair = instructionEngine.BuildItemInstructionData(EItem_InstructionType.RunInteraction, sitStored);
        // Eat and EatSitting will be the first splitting interaction, TBD)
        //Item_InstructionData eatSittingOnDiningChair = instructionEngine.BuildItemInstructionData(EItem_InstructionType.RunInteraction, INTERACTIONSO);

        List<Item_InstructionData> itemInstructionDatas = new() { pickUp, placeToSlot,sitOnChair };

        //DEBUG
        interactionEngine.ReceiveStateInstructions(interaction, itemInstructionDatas);

        //interaction.PopInteractionState();
    }

    //These two are to be replaced with proper interaction requirements system, which evaluates each interaction as needed instead of this syste
    public void OnItemPickUp(ItemBase item, Character character)
    {
        StoredInteraction si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Put Down To Lot"); //PutItemDown_InteractionSO
        if (si != null)
            si.InvalidInteraction = false;

        si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Put Down To Any Slot");
        if (si != null)
            si.InvalidInteraction = false;

        si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Pick Up");
        if (si != null)
            si.InvalidInteraction = true;

    }
    public void OnItemPutDown(ItemBase item, Character character)
    {
        StoredInteraction si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Put Down To Lot"); //PutItemDown_InteractionSO
        if (si != null)
            si.InvalidInteraction = true;

        si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Put Down To Any Slot");
        if (si != null)
            si.InvalidInteraction = false;


        si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Pick Up");
        if (si != null)
            si.InvalidInteraction = false;
    }

    public ItemBase GetItemCreatedByInteraction(Character character)
    {
        ItemBase item = itemsCreatedByInteraction[character];
        itemsCreatedByInteraction.Remove(character);
        return item;
    }

    public void RegisterMovingItem(ItemBase item, Transform anchor)
    {
        ItemsMoving.Add(item, anchor);
    }
    public void DeregisterMovingItem(ItemBase item)
    {
        ItemsMoving.Remove(item);
    }

    public ItemBase GetItemByItemLocationEnum(Item_InstructionData instructionData, Character thisCharacter, ItemBase thisItem)
    {
        EItemLocation location = instructionData.SlotParentItemLocationMove;
        switch (location)
        {
            case EItemLocation.ThisItem:
                return thisItem as ItemBase;
            case EItemLocation.Any:
                return allItems[0]; //Very unsure implementation
            case EItemLocation.AnyOfType:
                return lotManager.GetItemOnLotByType(thisCharacter.ThisLot, instructionData.TargetItemType);
            case EItemLocation.OnItemCreatedByInteraction:
                return itemManager.GetItemCreatedByInteraction(thisCharacter);
            case EItemLocation.OnMainItem:
                return thisItem;
            case EItemLocation.OnHeldItem:
                return thisCharacter.CarriedItem;
            case EItemLocation.OnMainItemSlot:
                Debug.LogError($"Unhandeled GetItemByItemLocationEnum! Character: {thisCharacter.ItemName} | Item: {thisItem.ItemName}, {location}");
                return null;
            //case ItemLocation.RuntimeKnown:
            //    return
            default:
                Debug.LogError($"Unhandeled GetItemByItemLocationEnum! Character: {thisCharacter.ItemName} | Item: {thisItem.ItemName}, {location}");
                return null;
        }
    }

    //Slots
    public Item_Slot GetSlotOnItemByType(SlotTypeSO slotType, ItemBase item)
    {
        return item.ItemSlotsByType[slotType][0];
    }
    public ItemBase GetItemOnSlotTypeSlot(ItemBase slotParentItem, SlotTypeSO slotType)
    {
        Item_Slot slot = GetSlotOnItemByType(slotType, slotParentItem);
        return slot.ItemInSlot;
    }

    public void PlaceCarriedItemToSlot(Character character, Item_Slot slot)
    {
        ItemBase item = character.CarriedItem;
        if (!IsSlotValidForItem(item, slot))
            Debug.LogError($"{character.ItemName} tried to place item ({item.ItemName}) to invalid slot {slot.name} (On {slot.ParentItem.ItemName})");

        PlaceItemToSlot(item, slot);
        DeregisterMovingItem(item);
        character.PutItemDown();

    }
    public void PlaceItemToSlot(ItemBase item, Item_Slot slot)
    {
        //if (!IsSlotValidForItem(item, slot))
        //    return;
        slot.PlaceItemToSlot(item);
        slot.ParentItem.PlaceItemToSlot(slot, item);
        item.PlaceThisItemToSlot(slot);
    }
    public void RemoveItemFromSlot(Item_Slot slot, ItemBase removedItem)
    {
        slot.ParentItem.RemoveItemFromSlot(slot);
        slot.ClearSlot();
        removedItem.RemoveThisItemFromSlot();

    }

    public bool IsSlotValidForItem(ItemBase item, Item_Slot slot)
    {
        if (slot.SlotType.ValidItemSO.Count == 0)
            return true;
        else if (slot.SlotType.ValidItemSO.Contains(item.ItemData))
            return true;
        else
            return false;
    }
}




