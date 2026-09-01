using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class Item_Slot : Slot
{
    [SerializeField]
    private SlotTypeSO slotType;
    public SlotTypeSO SlotType { get { return slotType; } }

    protected ItemBase itemInSlot;
    public ItemBase ItemInSlot { get { return itemInSlot; } }





    public void PlaceItemToSlot(ItemBase item)
    {
        itemInSlot = item;
        item.gameObject.transform.position = this.slotTransform.position;
    }
    public void ClearSlot()
    {
        itemInSlot = null;
    }
    public bool IsEmpty()
    {
        return itemInSlot == null;
    }
    private void OnDrawGizmos()
    {
        if (itemInSlot == null)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 0.15f);

        // Optional: show orientation
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * 0.4f
        );
    }
}