using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class Item_Slot : Slot
{
    protected ItemBase itemInSlot;
    public ItemBase ItemInSlot { get { return itemInSlot; } }

    [SerializeField]
    private bool limitSlotToSpecificItemType = false; //Make better (A list of suitable item types
    [SerializeField, ShowIf("limitSlotToSpecificItemType")]
    private ScriptableObject validItemSO;
    public ScriptableObject ValidItemSO { get { return validItemSO; } }



    public void PlaceItemToSlot(ItemBase item)
    {
        itemInSlot = item;
        item.gameObject.transform.position = this.slotTransform.position;
    }
    public void ClearSlot(ItemBase item)
    {
        itemInSlot = null;
    }
}