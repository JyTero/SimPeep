using System;
using UnityEngine;

public class LotGridTile
{
    private WorldLot partOfLot;
    public WorldLot PartOfLot { get { return partOfLot; } }


    private Vector2Int tileCoordinates;
    public Vector2Int Coordinates { get { return tileCoordinates; } }

    //private int thisX;
    //public int X { get { return thisX; } }

    //private int thisY;
    //public int Y { get { return thisY; } }

    private int travelCost;
    public int TravelCost { get { return travelCost; } }

    private Vector3 tilePos;
    public Vector3 TilePos { get { return tilePos; } }

    public Item_Slot itemSlotOnTile;
    
    private Interactable interactableOnTile;
    public Interactable InteractableOnTile { get { return interactableOnTile; } }

    private ELGTWalkability walkability;
    public ELGTWalkability Walkwability { get { return walkability; } }
    //public bool containsItem;

    public LotGridTile(WorldLot partOfLot, int tileX, int tileY)
    {
        this.partOfLot = partOfLot;

        tileCoordinates = new(tileX, tileY);
        walkability = ELGTWalkability.Yes;


    }
    public void InitialiseTile(Vector3 tilePos, int trvlCost)
    {
        this.tilePos = tilePos;
        this.travelCost = trvlCost;
    }


    public void PlaceItemToTile(ItemBase item)
    {
        walkability = item.ItemTileWalkability;
        interactableOnTile = item;
    }
    public void RemoveItemFromTile(ItemBase item)
    {
        walkability = ELGTWalkability.Yes;
        interactableOnTile = null;
    }

    public void SlotOnTile(Item_Slot slot)
    {
        itemSlotOnTile = slot;
    }

    public void CharacterOnTile(Character character)
    {
        walkability = ELGTWalkability.NoTemporary;
        interactableOnTile = character;
    }

    public void CharacterLeftTile()
    {
        walkability = ELGTWalkability.Yes;
        interactableOnTile = null;
    }
}
