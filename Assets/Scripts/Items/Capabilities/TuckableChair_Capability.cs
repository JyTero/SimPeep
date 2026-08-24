using UnityEngine;

public class TuckableChair_Capability : ItemCapability
{
    private bool chairTiedToTable;
    public bool ChairTiedToTable { get { return chairTiedToTable; } }

    private ItemBase table;
    public ItemBase Table { get { return table; } }

    public void TieChairToTable(ItemBase item)
    {
        chairTiedToTable = true;
        table = item;
    }
}
