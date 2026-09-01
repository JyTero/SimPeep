using UnityEngine;

public class SeatingWithTableData
{
    public ItemBase Table;
    public ItemBase Chair;
    public Item_Slot OnTableSlot;

    public SeatingWithTableData (ItemBase table, ItemBase chair, Item_Slot onTableSlot)
    {
        Table = table;
        Chair = chair;
        OnTableSlot = onTableSlot;
    }
}
