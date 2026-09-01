using UnityEngine;

public class TuckableChair_Capability : ItemCapability
{
    private bool chairTiedToTable;
    public bool ChairTiedToTable { get { return chairTiedToTable; } }

    private ItemBase table;
    public ItemBase Table { get { return table; } }

    private Item_Slot chairSlot;
    public Item_Slot ChairSlot { get { return chairSlot; } }

    public void TieChairToTable(ItemBase item)
    {
        chairTiedToTable = true;
        table = item;
    }
}
