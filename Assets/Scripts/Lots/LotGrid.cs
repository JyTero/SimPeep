using System;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LotGrid
{
    public int Width { get; }
    public int Height { get; }
    public float TileSize { get; }
    public WorldLot ThisLot { get; }

    private LotGridTile[] tiles;
    public LotGridTile[] Tiles() { return tiles; }
    public LotGrid(int width, int height, float tileSize, WorldLot lot)
    {
        Width = width;
        Height = height;
        TileSize = tileSize;
        ThisLot = lot;

        tiles = new LotGridTile[width * height];
        for(int i = 0; i< tiles.Length; i++)
        {
            int tileY = i / Width;
            int tileX = i % Width;

            LotGridTile tile = new LotGridTile(lot,tileX, tileY);
            tiles[i] = tile;
        }
    }

    public LotGridTile GetTile(int x, int y)
    {
        if (x < 0 || y < 0)
            return null;
        if (x >= Width || y >= Height)
            return null;
        return tiles[y * Width + x];
    }

    public Vector2Int WorldToTile(Vector3 worldPosition)
    {
        Vector3 local = worldPosition - ThisLot.transform.position;

        return new Vector2Int(
            Mathf.FloorToInt(local.x / TileSize),
            Mathf.FloorToInt(local.z / TileSize)
        );
    }
}
