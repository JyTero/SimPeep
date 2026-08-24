using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldLot : MonoBehaviour
{
    [SerializeField]
    private List<ItemBase> itemsOnLot = new();
    public List<ItemBase> ItemsOnLot { get { return itemsOnLot; } }

    [SerializeField]
    private List<Character> charactersOnLot = new();
    public List<Character> CharactersOnLot { get { return charactersOnLot; } }

    [SerializeField]
    private int lotSizeX;
    public int LotSizeX { get { return lotSizeX; } }
    [SerializeField]
    private int lotSizeY;
    public int LotSizeY { get { return lotSizeY; } }

    private LotGrid lotGrid;
    public LotGrid LotGrid { get { return lotGrid; } }


    public void GenerateLotGrid(float tileSize)
    {
        lotGrid = new LotGrid(lotSizeX, lotSizeY, tileSize, this);
    }
    public void AddItemToLot(ItemBase item)
    {
        itemsOnLot.Add(item);
    }
    public void RemoveItemFromLot(ItemBase item)
    {
        ItemsOnLot.Remove(item);
    }

    
}
