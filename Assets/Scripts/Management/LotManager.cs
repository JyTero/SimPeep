using NaughtyAttributes;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class LotManager : ManagementCore
{
    [SerializeField]
    private float lotTileSize;
    [SerializeField]
    private int tileDefaultTravelCost;

    [SerializeField]
    private List<WorldLot> allLots = new();
    public List<WorldLot> AllLots { get { return allLots; } }

    private List<StoredInteraction> allStoredInteractions = new();
    private WorldLot lot;

    protected override void Start()
    {
        base.Start();
        lot = FindAnyObjectByType<WorldLot>();
        //AddNewLot(lot);

    }

    public void LoadingScreen()
    {
        InitialiseLots();

        List<ItemBase> allItems = lot.ItemsOnLot;
        foreach (ItemBase item in allItems)
        {
            item.ChangeCurrentLot(lot);

            //Gridding
            LotGridTile lgt = GetTileInteractableIsOn(item);
            if (lgt != null)
                PlaceItemToCenterOfTile(item, lgt);

            itemManager.InitialiseItem(item);
        }
        allStoredInteractions = GetAllInteractionsOnLot(lot);

    }
    private void InitialiseLots()
    {
        foreach (WorldLot lot in allLots)
        {
            //Generate Lot Grid
            lot.GenerateLotGrid(lotTileSize);
            foreach (LotGridTile tile in lot.LotGrid.Tiles())
            {
                tile.InitialiseTile(GetTileCenterInPosition(tile), tileDefaultTravelCost);
            }
        }
    }

    public void AddNewLot(WorldLot lot)
    {
        allLots.Add(lot);
    }

    public void NewItemOnLot(ItemBase item)
    {
        this.lot.AddItemToLot(item);
        foreach (StoredInteraction si in item.StoredInteractions)
            allStoredInteractions.Add(si);
    }

    public void RemoveItemFromLot(ItemBase item)
    {
        this.lot.RemoveItemFromLot(item);
        foreach (StoredInteraction si in item.StoredInteractions)
            allStoredInteractions.Remove(si);
    }

    public List<StoredInteraction> GetAllInteractionsOnLot(WorldLot lot)
    {
        List<StoredInteraction> interactions = new();
        foreach (ItemBase item in lot.ItemsOnLot)
        {
            foreach (StoredInteraction storedInteraction in item.StoredInteractions)
                interactions.Add(storedInteraction);
            //foreach (InteractionSO interactonSO in item.AllInteractions)
            //{
            //    interactions.Add(new StoredInteraction(interactonSO, item));
            //}
        }
        foreach (Character chara in lot.CharactersOnLot)
        {
            foreach (StoredInteraction storedInteraction in chara.StoredInteractions)
                interactions.Add(storedInteraction);
            //foreach (InteractionSO interactonSO in chara.AllInteractions)
            //{
            //    interactions.Add(new StoredInteraction(interactonSO, chara));
            //}
        }
        return interactions;
    }

    public StoredInteraction FindSuitableStoredInteractionOnLot(InteractionSO itso, WorldLot lot)
    {
        foreach (StoredInteraction si in allStoredInteractions)
        {
            if (si.InteractionTuningSO == itso)
                return si;
        }
        return null;
    }

    public List<StoredInteraction> GetAllStoredInteractionsOnLot(WorldLot lot)
    {
        return allStoredInteractions;
    }

    public void RemoveLot(WorldLot lot)
    {
        allLots.Remove(lot);
    }

    //LotGrid
    public LotGridTile GetTileInteractableIsOn(Interactable interactable)
    {
        WorldLot lot = interactable.ThisLot;
        return GetLotTile(interactable.transform.position);
    }

    public LotGridTile GetLotTile(Vector3 pos)
    {
        Vector2Int gridPos = lot.LotGrid.WorldToTile(pos);
        return lot.LotGrid.GetTile(gridPos.x, gridPos.y);

    }
    public LotGridTile GetLotTile(int x, int y)
    {

        return lot.LotGrid.GetTile(x, y);
    }

    public void PlaceItemToCenterOfTile(ItemBase item, LotGridTile lotTile)
    {
        Vector3 centerPos = GetTileCenterInPosition(lotTile);
        item.transform.position = centerPos;
        lotTile.PlaceItemToTile(item);
    }
    public Vector3 GetTileCenterInPosition(LotGridTile lotTile)
    {
        return lotTile.PartOfLot.transform.position + new Vector3((lotTile.X + 0.5f) * lotTileSize, 0f, (lotTile.Y + 0.5f) * lotTileSize);
    }

    //DEBUG
    private void OnDrawGizmos()
    {
        foreach (WorldLot lot in allLots)
        {
            Vector3 bottomLeft = transform.position;
            Vector3 bottomRight = bottomLeft + new Vector3(lot.LotSizeX, 0, 0);
            Vector3 topLeft = bottomLeft + new Vector3(0, 0, lot.LotSizeY);
            Vector3 topRight = bottomLeft + new Vector3(lot.LotSizeX, 0, lot.LotSizeY);

            Gizmos.color = Color.white;

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);



            if (lot.LotGrid == null)
                return;

            if (lot.LotGrid.Width <= 0 || lot.LotGrid.Height <= 0)
                return;

            Gizmos.color = Color.gray;

            //Highlight Tile
            //DebugHighlightTile(4, 6);

            //Walkability
            DebugVisualiseTileWalkability();

            //GridTileLines
            //DebugDrawLotGrid();
        }
    }

    private void DebugVisualiseTileWalkability()
    {
        for (int y = 0; y < lot.LotGrid.Height; y++)
        {
            for (int x = 0; x < lot.LotGrid.Width; x++)
            {
                LotGridTile tile = lot.LotGrid.GetTile(x, y);

                Gizmos.color = tile.walkable
                    ? Color.green
                    : Color.red;


                Vector3 center = GetTileCenterInPosition(tile);

                Gizmos.DrawCube(
                    center,
                    new Vector3(
                        lotTileSize,
                        0.05f,
                        lotTileSize
                    )
                );
            }
        }
    }
    private void DebugDrawLotGrid()
    {
        for (int x = 0; x <= lot.LotGrid.Width; x++)
        {
            Vector3 start = transform.position +
                            new Vector3(x * lot.LotGrid.TileSize, 0, 0);

            Vector3 end = transform.position +
                          new Vector3(x * lot.LotGrid.TileSize, 0, lot.LotGrid.Height * lot.LotGrid.TileSize);

            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= lot.LotGrid.Height; y++)
        {
            Vector3 start = transform.position +
                            new Vector3(0, 0, y * lot.LotGrid.TileSize);

            Vector3 end = transform.position +
                          new Vector3(lot.LotGrid.Width * lot.LotGrid.TileSize, 0, y * lot.LotGrid.TileSize);

            Gizmos.DrawLine(start, end);
        }
    }
    private void DebugHighlightTile(int x, int y)
    {
        foreach (LotGridTile tile in lot.LotGrid.Tiles())
        {
            if (tile.X == x && tile.Y == y)
            {
                Vector3 center = GetTileCenterInPosition(tile);

                Gizmos.DrawCube(
                    center,
                    new Vector3(
                        lotTileSize,
                        0.05f,
                        lotTileSize
                    )
                );
            }
        }
    }
}