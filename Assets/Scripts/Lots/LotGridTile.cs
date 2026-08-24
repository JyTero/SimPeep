using System;
using UnityEngine;

public class LotGridTile
{
    private WorldLot partOfLot;
    public WorldLot PartOfLot { get { return partOfLot; } }

    private int thisX;
    public int X { get { return thisX; } }

    private int thisY;
    public int Y { get { return thisY; } }

    private int travelCost;
    public int TravelCost { get { return travelCost; } }

    private Vector3 tilePos;
    public Vector3 TilePos { get { return tilePos; } }

    public Item_Slot itemSlotOnTile;
    public ItemBase itemOnTile;
    public bool walkable;
    //public bool containsItem;

    public LotGridTile(WorldLot partOfLot, int tileX, int tileY)
    {
        this.partOfLot = partOfLot;

        thisX = tileX;
        thisY = tileY;
        walkable = true;


    }
    public void InitialiseTile(Vector3 tilePos, int trvlCost)
    {
        this.tilePos = tilePos;
        this.travelCost = trvlCost;
    }


    public void PlaceItemToTile(ItemBase item)
    {
        walkable = false;
        itemOnTile = item;
    }
    public void SlotOnTile(Item_Slot slot)
    {
        itemSlotOnTile = slot;
    }
}
