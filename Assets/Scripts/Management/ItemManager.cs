using System.Collections.Generic;
using UnityEngine;

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

        foreach (InteractionSO itso in item.InteractionSOs)
        {
            item.NewStoredInteraction(new StoredInteraction(itso, item));
        }

        //IF(WithinLotGrid)
        //Place onto center of nearest tile

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


    public void HandleItemInstructions(List<Item_Instruction> itemInstructions)
    {
        foreach (Item_Instruction itemInstruction in itemInstructions)
        {
            Character thisCharacter = itemInstruction.ThisCharacter;
            ItemBase thisItem = itemInstruction.ThisItem;
            Item_InstructionSO itemInstructionSO = itemInstruction.ItemInstructionSO;

            if (itemInstructionSO.SpawnItem)
            {
                //HandleItemSPawning
                ItemBase ib = SpawnNewItem(itemInstructionSO.ItemToSpawn, thisCharacter.ThisLot, thisItem.transform.position);
                itemsCreatedByInteraction.Add(thisCharacter, ib);
                //newItem.gameObject.transform.position = thisItem.transform.position;
                //Spawned items appear "between farmes" (fixed update or smth), this should cause a frame of waiting for the character to have item ready
                continue;
            }
            else if(itemInstructionSO.DestroyItem)
            {
                DestroyItem(thisItem);
            }
            else if (itemInstructionSO.MoveThisItem)
            {
                switch (itemInstructionSO.WhereToMoveItem)
                {
                    case ItemLocation.Default:
                        break;
                    case ItemLocation.LotSpace:
                        OnItemPutDown(thisItem, thisCharacter);
                        CharacterControl.PutItemDownGround(thisCharacter);
                        break;
                    case ItemLocation.WorldSpace:
                        break;
                    case ItemLocation.InCharactacter:
                        break;
                    case ItemLocation.OnCharacter:
                        OnItemPickUp(thisItem, thisCharacter);
                        CharacterControl.PickupItem(thisCharacter, thisItem);
                        break;
                    case ItemLocation.ItemSlot:
                        break;
                    default:
                        break;
                }
            }

        }
    }

    private void OnItemPickUp(ItemBase item, Character character)
    {
        StoredInteraction si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Put Down"); //PutItemDown_InteractionSO
        if (si != null)
            si.InvalidInteraction = false;

        si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Pick Up");
        if (si != null)
            si.InvalidInteraction = true;

    }

    private void OnItemPutDown(ItemBase item, Character character)
    {
        StoredInteraction si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Put Down"); //PutItemDown_InteractionSO
        if (si != null)
            si.InvalidInteraction = true;

        si = item.StoredInteractions.Find(x => x.InteractionTuningSO.InteractionName == "Pick Up");
        if (si != null)
            si.InvalidInteraction = false;
    }

    public bool IsSlotValidForItem(ItemBase item, Item_Slot slot)
    {
        if (slot.ValidItemSO == null)
            return true;
        else if (slot.ValidItemSO == item.ItemData)
            return true;
        else 
            return false;
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

    //Slots
    public void PlaceCarriedItemToSlot(Character character, Item_Slot slot)
    {
        ItemBase item = character.CarriedItem;
        if (!IsSlotValidForItem(item, slot))
            return;
        DeregisterMovingItem(item);
        character.PutItemDown();
        slot.PlaceItemToSlot(item);

    }
    public void PlaceItemToSlot(ItemBase item, Item_Slot slot)
    {
        if (!IsSlotValidForItem(item, slot))
            return;
    }


}




