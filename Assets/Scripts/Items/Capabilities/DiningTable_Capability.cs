using System.Collections.Generic;
using UnityEngine;

public class DiningTable_Capability : ItemCapability
{
    [SerializeField]
    private List<Item_Slot> chairSlots  = new();
    public List<Item_Slot> ChairSlots { get { return chairSlots; } }

    [SerializeField]
    private List<Item_Slot> onTableSlots = new();
    public List<Item_Slot> OnTableSlots { get { return onTableSlots; } }

    private Dictionary<Item_Slot, Item_Slot> tableSlotsByChairSlot = new();
    public Dictionary<Item_Slot, Item_Slot> TableSlotsByChairSlot {  get { return tableSlotsByChairSlot; } }

    public void NewChairTableSlotPair(Item_Slot cSlot, Item_Slot tSlot)
    {
        tableSlotsByChairSlot.Add(cSlot, tSlot);
    }
}
