using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPathfinding : ManagementCore
{
    [SerializeField]
    private float timeBetweenMovements;

    private List<CharacterFindingPath> charactersFindingPath = new();


    protected override void Start()
    {
        base.Start();
    }

    public void NewCharacterFindingPath(Character character, Vector3 destinationPos)
    {
        LotGridTile destination = lotManager.GetLotTile(destinationPos);
        if (destination == null)
            Debug.LogError("Destination out of grid!");
        else if (character.CurrentTile == destination)
            characterAIHandler.AtDestination(character);
        else
            charactersFindingPath.Add(new CharacterFindingPath(character, destination));

    }

    public void NewCharacterFindingPath(Character character, LotGridTile destinationTile)
    {
        if (character.CurrentTile == destinationTile)
            characterAIHandler.AtDestination(character);
        else
            charactersFindingPath.Add(new CharacterFindingPath(character, destinationTile));
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;

        FindingPath();
        AdvanceOnPath();
    }

    private void FindingPath()
    {
        for (int i = charactersFindingPath.Count - 1; i >= 0; i--)
        {
            CharacterFindingPath character = charactersFindingPath[i];
            int gridWidth = character.Character.ThisLot.LotSizeY;

            //GUMMY
            //If destination is neighbor, at destination
            List<LotGridTile> neighbors = lotManager.GetNeighboringTiles(character.Character.CurrentTile);
            if(neighbors.Contains(character.Destination))
            {
                characterAIHandler.AtDestination(character.Character);
                charactersFindingPath.RemoveAt(i);
                continue;
            }

            //For now, find entire path at once, future "Look 20 tiles ahead"/"Look this room" or smth
            while (!character.HasPath)
            {
                TileInScoring tis = character.GetNextTileToExplore();
                if (tis.Tile == character.Destination)
                {
                    //Yay
                    //charactersFindingPath.Remove(character);
                    character.Path = FormShortestPathToDestination(tis);
                    character.HasPath = true;
                }
                else
                {
                    //Get all neighbors
                    List<TileInScoring> neighborTiles = new();

                    for (int neighborX = tis.Tile.X - 1; neighborX <= tis.Tile.X + 1; neighborX++)
                    {
                        for (int neighborY = tis.Tile.Y - 1; neighborY <= tis.Tile.Y + 1; neighborY++)
                        {
                            LotGridTile neighborLgt = lotManager.GetLotTile(neighborX, neighborY);
                            if (neighborLgt == null)
                                continue;
                            if (neighborLgt == tis.Tile)
                                continue;
                            if (character.PathfindingVisitedTiles.Exists(x => x.Tile == neighborLgt))
                                continue;

                            neighborTiles.Add(new TileInScoring(neighborLgt, character.Destination));
                        }
                    }

                    //IterateNeighbors
                    foreach (TileInScoring neighborTis in neighborTiles)
                    {
                        if (character.PahtfindingTilesToVisit.Exists(x => x.Tile == neighborTis.Tile))
                        {
                            //Contains
                            //If the current lowest cost route to node is less thant the cost from curretn route, update
                            if (neighborTis.CostToTravelHere > (tis.CostToTravelHere += neighborTis.Tile.TravelCost))
                            {
                                neighborTis.CostToTravelHere += neighborTis.Tile.TravelCost;
                                neighborTis.ShortestRouteTile = tis;
                            }
                        }
                        else
                        {
                            character.PahtfindingTilesToVisit.Add(neighborTis);
                            neighborTis.CostToTravelHere = tis.CostToTravelHere + neighborTis.Tile.TravelCost;
                            neighborTis.ShortestRouteTile = tis;
                        }
                    }
                    character.PahtfindingTilesToVisit.Remove(tis);
                    character.PathfindingVisitedTiles.Add(tis);
                }
            }
        }
    }

    private List<TileInScoring> FormShortestPathToDestination(TileInScoring destination)
    {
        List<TileInScoring> path = new();
        bool pathDone = false;
        if (destination.ShortestRouteTile.Tile == destination.Tile)
            return path;

        path.Add(destination.ShortestRouteTile);
        TileInScoring nextTile = destination.ShortestRouteTile.ShortestRouteTile;

        while (!pathDone)
        {
            path.Add(nextTile);
            if (nextTile.ShortestRouteTile.Tile == nextTile.Tile)
                pathDone = true;
            else
                nextTile = nextTile.ShortestRouteTile;
        }
        return path;
    }

    private void AdvanceOnPath()
    {
        if (TooEarlyForNextTick(timeBetweenMovements))
            return;
        for (int i = charactersFindingPath.Count - 1; i >= 0; i--)
        {
            CharacterFindingPath character = charactersFindingPath[i];
            if(!character.HasPath)
                continue;

            if (character.Path.Count == 0)
            {
                characterAIHandler.AtDestination(character.Character);
                charactersFindingPath.RemoveAt(i);
                continue;
            }

            character.Character.transform.position = character.Path[character.Path.Count - 1].Tile.TilePos;
            character.Character.ChangeCurrentTile(character.Path[character.Path.Count - 1].Tile);
            character.Path.RemoveAt(character.Path.Count - 1);

        }
    }


}
class CharacterFindingPath
{
    public Character Character;
    public LotGridTile Destination;
    public List<TileInScoring> PathfindingVisitedTiles = new();
    public List<TileInScoring> PahtfindingTilesToVisit = new();
    public bool HasPath;
    public List<TileInScoring> Path = new();

    public CharacterFindingPath(Character character, LotGridTile destination)
    {
        this.Character = character;
        this.Destination = destination;
        HasPath = false;

        PahtfindingTilesToVisit.Add(new(character.CurrentTile, destination));
        PahtfindingTilesToVisit[0].ShortestRouteTile = new TileInScoring(PahtfindingTilesToVisit[0].Tile, destination);
    }
    public TileInScoring GetNextTileToExplore()
    {
        TileInScoring lowestCostTile = PahtfindingTilesToVisit[0];
        for (int i = 1; i < PahtfindingTilesToVisit.Count; i++)
        {
            if (PahtfindingTilesToVisit[i].CostToTravelHere < lowestCostTile.CostToTravelHere)
            {
                lowestCostTile = PahtfindingTilesToVisit[i];
            }
        }
        return lowestCostTile;
    }
}
class TileInScoring
{
    public LotGridTile Tile;
    public int DistanceToDestination;
    public int CostToTravelHere;
    public TileInScoring ShortestRouteTile = null;

    public TileInScoring(LotGridTile tile, LotGridTile destination)
    {
        this.Tile = tile;

        SetNodeDistanceToDestination(destination);
        CostToTravelHere = DistanceToDestination;
    }

    public void SetNodeDistanceToDestination(LotGridTile destination)
    {
        DistanceToDestination = (int)Vector3.Distance(Tile.TilePos, destination.TilePos);
    }
}