using System;
using UnityEngine;

[Serializable]
public class Slot : MonoBehaviour
{
    [SerializeField]
    protected Transform slotTransform;
    public Transform SlotTransform { get { return slotTransform; } }

    protected ItemBase parentItem;
    public ItemBase ParentItem { get { return parentItem; } }
    public virtual void InitialiseSlot(ItemBase parentItem)
    {
        this.parentItem = parentItem;
    }
}