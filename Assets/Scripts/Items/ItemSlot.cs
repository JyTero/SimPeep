using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [SerializeField]
    private Transform slotTransform;
    public Transform SlotTransform { get { return slotTransform; } }

    private ItemBase parentItem;
    public ItemBase ParentItem { get { return parentItem; } }

    private ItemBase itemInSlot;
    public ItemBase ItemInSlot { get { return itemInSlot; } }

    public void InitialiseSlot(ItemBase parentItem)
    {
        this.parentItem = parentItem;
    }

    public void PlaceItemToSlot(ItemBase item)
    {
        itemInSlot = item;
    }
    public void ClearSlot(ItemBase item)
    {
        itemInSlot = null;
    }

}
