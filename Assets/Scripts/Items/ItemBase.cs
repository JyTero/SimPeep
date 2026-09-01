using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : Interactable
{
    [SerializeField]
    private ItemSO itemData;
    public ItemSO ItemData {  get { return itemData; } }


    [SerializeField]
    private string itemDescription;
    public string ItemDescription { get { return itemDescription; } set { itemDescription = value; } }

    [SerializeField]
    private List<ItemCapabilites> capabilites;
    public List<ItemCapabilites> Capabilites { get { return capabilites; } }

    [SerializeField]
    private List<ItemCapability> capabilityComponents;
    public List<ItemCapability> CapabilityComponents { get { return capabilityComponents; } }

   

    private int itemPrice;
    public int ItemPrice { get { return itemPrice; } set { itemPrice = value; } }

    private Item_Slot occupiedSlot;
    public Item_Slot OccupiedSlot { get { return occupiedSlot; } }

    [SerializeField]
    private List<Item_Slot> itemSlotsOnItem = new();
    public List<Item_Slot> ItemSlotsOnItem { get { return itemSlotsOnItem; }}

    private Dictionary<SlotTypeSO, List<Item_Slot>> itemSlotsByType= new();
    public Dictionary<SlotTypeSO, List<Item_Slot>> ItemSlotsByType { get { return itemSlotsByType; }}

    private Dictionary<ItemBase, List<Item_Slot>> itemSlotsByItem = new();
    public Dictionary<ItemBase, List<Item_Slot>> ItemSlotsByItem { get { return itemSlotsByItem; } }

    [SerializeField]
    private List<Character_Slot> characterSlotsOnItem = new();
    public List<Character_Slot> CharacterSlotsOnItem { get { return characterSlotsOnItem; }}

    private Dictionary<ItemCapabilites, ItemCapability> capabilitiesByEnum = new();
    public Dictionary<ItemCapabilites, ItemCapability> CapabilitiesByEnum { get { return capabilitiesByEnum; } }

    [HideInInspector]
    public bool itemInitialised = false;

    protected override void Start()
    {
        base.Start();

       
    }

    public void InitialiseSlot(Item_Slot slot)
    {
        SlotTypeSO type = slot.SlotType;

        if (itemName == "Matter Exciter")
            Debug.Log($"Stovin'");
        if (itemSlotsByType.ContainsKey(type))
            itemSlotsByType[type].Add(slot);
        else
            itemSlotsByType.Add(type, new List<Item_Slot> { slot });

        //if (itemSlotsByItem.ContainsKey(null))
        //    itemSlotsByItem[null].Add(slot);
        //else
        //    itemSlotsByItem.Add(null, new List<Item_Slot> { slot });
    }

    //TO a slot on THIS ITEM
    public void PlaceItemToSlot(Item_Slot slot, ItemBase item)
    {
        if (itemSlotsByItem.ContainsKey(item))
            itemSlotsByItem[item].Add(slot);
        else
            itemSlotsByItem.Add(item, new List<Item_Slot> { slot });
    }
    public void RemoveItemFromSlot(Item_Slot slot)
    {
        ItemBase item = slot.ItemInSlot;
        itemSlotsByItem[item].Remove(slot);
        if (itemSlotsByItem[item].Count == 0)
            itemSlotsByItem.Remove(item);
        //itemSlotsByItem[null].Add(slot);
    }

    //This item to ANOTHER ITEM SLOT
    public void PlaceThisItemToSlot(Item_Slot slot)
    {
        occupiedSlot = slot;
    }
    public void RemoveThisItemFromSlot()
    {
        occupiedSlot = null;
    }
}
