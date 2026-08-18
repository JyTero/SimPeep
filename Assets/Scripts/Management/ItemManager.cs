using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : ManagementCore
{
    private List<ItemBase> allItems = new();
    public List<ItemBase> AllItems { get { return allItems; } }

    private Dictionary<ItemBase, Transform> itemsMoving = new();
    public Dictionary<ItemBase, Transform> ItemsMoving { get { return itemsMoving; } }

    private Dictionary<Character, ItemBase> itemsCreatedByCharacter = new();

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

        unreadyItems.Add(returne);
        return returne;

    }

    public StoredInteraction GetInteractionOnInteractable(InteractionSO itsoTemplate, Interactable interactable)
    {
        foreach (StoredInteraction storedInteraction in interactable.AllInteractions)
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
                ItemBase ib = itemManager.SpawnNewItem(itemInstructionSO.ItemToSpawn, thisCharacter.ThisLot, thisItem.transform.position);
                itemsCreatedByCharacter.Add(thisCharacter, ib);
                //newItem.gameObject.transform.position = thisItem.transform.position;
                //Spawned items appear "between farmes" (fixed update or smth), this should cause a frame of waiting for the character to have item ready
                continue;
            }
            else if (itemInstructionSO.MoveThisItem)
            {
                switch (itemInstructionSO.WhereToMoveItem)
                {
                    case ItemLocation.Default:
                        break;
                    case ItemLocation.LotSpace:
                        CharacterControl.PutItemDownGround(thisCharacter);
                        break;
                    case ItemLocation.WorldSpace:
                        break;
                    case ItemLocation.InCharactacter:
                        break;
                    case ItemLocation.OnCharacter:
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

    public void PlaceCarriedItemToSlot(Character character, ItemSlot slot)
    {
        character.PutItemDown();
        slot.PlaceItemToSlot(character.CarriedItem);
    }
    public ItemBase GetItemCreatedByInstuction(Character character)
    {
        return itemsCreatedByCharacter[character];
    }

    public void RegisterMovingItem(ItemBase item, Transform anchor)
    {
        ItemsMoving.Add(item, anchor);
    }
    public void DeregisterMovingItem(ItemBase item)
    {
        ItemsMoving.Remove(item);
    }
}




