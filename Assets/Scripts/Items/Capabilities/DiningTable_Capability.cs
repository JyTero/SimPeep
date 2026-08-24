using System.Collections.Generic;
using UnityEngine;

public class DiningTable_Capability : ItemCapability
{
    [SerializeField]
    private List<Item_Slot> chairSlots  = new();
    public List<Item_Slot> ChairSlots { get { return chairSlots; } }
}
