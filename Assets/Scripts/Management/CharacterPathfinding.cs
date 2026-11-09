using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterPathfinding : ManagementCore
{
    [SerializeField]
    private float timeBetweenMovements;

    private Dictionary<Character, CharacterFindingPath> charactersFindingPathByCharacter = new();


    protected override void Start()
    {
        base.Start();
    }

    public PathfindingResult NewCharacterFindingPath(Character character, LotGridTile destinationTile)
    {
        if (destinationTile.Walkwability != ELGTWalkability.Yes)
            return new PathfindingResult(ERoutingResult.FailedDestinationUnWalkable, ERoutingResultReason.Default, character, destinationTile);

        if (character.CurrentTile == destinationTile)
        {
            characterAIHandler.AtDestination(character);
            return new PathfindingResult(ERoutingResult.Success, ERoutingResultReason.Success, character, destinationTile);
        }
        else
        {
            charactersFindingPathByCharacter.Add(character, new CharacterFindingPath(character, destinationTile));
            return new PathfindingResult(ERoutingResult.Incomplete, ERoutingResultReason.PathfindingOngoing, character, destinationTile);
        }
    }
    public PathfindingResult TestIfCanRouteToTile(Character character, LotGridTile destinationTile)
    {
        if (destinationTile.Walkwability != ELGTWalkability.Yes)
            return new PathfindingResult(ERoutingResult.FailedDestinationUnWalkable, ERoutingResultReason.Default, character, destinationTile);

        if (character.CurrentTile == destinationTile)
        {
            characterAIHandler.AtDestination(character);
            return new PathfindingResult(ERoutingResult.Success, ERoutingResultReason.Success, character, destinationTile);
        }
        else
        {
            CharacterFindingPath cfp = new CharacterFindingPath(character, destinationTile);
            PathfindingResult pfr = FindingPath(cfp);
            return pfr;
        }

    }
    public void CancelPathfinding(Character character)
    {
        charactersFindingPathByCharacter.Remove(character);
        interactionEngine.ActiveInteractionsByCharacter[character].PopInteractionState();
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        timeSinceLastUdate += deltaTime;
        if (TooEarlyForNextTick(updateInterval))
            return;
        if (charactersFindingPathByCharacter.Keys.Count == 0)
            timeSinceLastUdate = 0;
        else
        {
            List<CharacterFindingPath> charactersFindingPath = charactersFindingPathByCharacter.Values.ToList();
            for (int i = charactersFindingPath.Count - 1; i >= 0; i--)
            {
                PathfindingResult pfr = FindingPath(charactersFindingPath[i]); //Results currently unused
                AdvanceOnPath();
            }
        }
    }

    private PathfindingResult FindingPath(CharacterFindingPath character)
    {
        //For now, find entire path at once, future "Look 20 tiles ahead"/"Look this room" or smth
        while (!character.HasPath)
        {
            TileInScoring tis = character.GetNextTileToExplore();
            if (tis == null)
            {
                //Cant find path
                //characterAIHandler.CharacterRoutingResult(new PathfindingResult(ERoutingResult.FailedNoPath, ERoutingResultReason.NoRoutablePathFound, character.Character, character.Destination), character.Character);
                return new PathfindingResult(ERoutingResult.FailedNoPath, ERoutingResultReason.NoRoutablePathFound, character.Character, character.Destination);
                //break; //Break the while to continue the for loop 
            }

            if (tis.Tile == character.Destination)
            {
                //Yay
                //charactersFindingPath.Remove(character);
                character.Path = FormShortestPathToDestination(tis);
                character.HasPath = true;
                PathfindingResult pfr = new(ERoutingResult.Success, ERoutingResultReason.Success, character.Character, character.Destination);
                pfr.Path = character.Path;
                return pfr;
            }
            else
            {
                //Get all neighbors
                List<TileInScoring> neighborTiles = new();

                for (int neighborX = tis.Tile.Coordinates.x - 1; neighborX <= tis.Tile.Coordinates.x + 1; neighborX++)
                {
                    for (int neighborY = tis.Tile.Coordinates.y - 1; neighborY <= tis.Tile.Coordinates.y + 1; neighborY++)
                    {
                        LotGridTile neighborLgt = lotManager.GetLotTile(neighborX, neighborY);
                        if (neighborLgt == null)
                            continue;
                        if (neighborLgt == tis.Tile)
                            continue;
                        if (character.PathfindingVisitedTiles.Exists(x => x.Tile == neighborLgt))
                            continue;
                        if (neighborLgt.Walkwability == ELGTWalkability.No)
                        {
                            continue;
                        }

                        neighborTiles.Add(new TileInScoring(neighborLgt, character.Destination));
                    }
                }

                //IterateNeighbors
                foreach (TileInScoring neighborTis in neighborTiles)
                {
                    if (character.PathfindingTilesToVisit.Exists(x => x.Tile == neighborTis.Tile))
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
                        character.PathfindingTilesToVisit.Add(neighborTis);
                        neighborTis.CostToTravelHere = tis.CostToTravelHere + neighborTis.Tile.TravelCost;
                        neighborTis.ShortestRouteTile = tis;
                    }
                }
                character.PathfindingTilesToVisit.Remove(tis);
                character.PathfindingVisitedTiles.Add(tis);
            }

        }
        return null;
    }

    private List<TileInScoring> FormShortestPathToDestination(TileInScoring destination)
    {
        List<TileInScoring> path = new();
        bool pathDone = false;
        if (destination.ShortestRouteTile.Tile == destination.Tile)
            return path;
        path.Add(destination);
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

        List<Character> characters = charactersFindingPathByCharacter.Keys.ToList();


        //for (int i = charactersFindingPathByCharacter.Count - 1; i >= 0; i--)
        for (int i = characters.Count - 1; i >= 0; i--)
        {
            CharacterFindingPath character = charactersFindingPathByCharacter[characters[i]];

            if (!character.HasPath)
                continue;

            if (character.Path.Count == 0)
            {
                characterAIHandler.AtDestination(character.Character);
                //charactersFindingPathByCharacter.RemoveAt(i);
                charactersFindingPathByCharacter.Remove(characters[i]);
                continue;
            }

            LotGridTile tile = character.Path[character.Path.Count - 1].Tile;

            if (tile.Walkwability == ELGTWalkability.NoTemporary)
            {
                //Could you go around?
                    //HOW?
                //if not, if blocker is character, wait and ask them to move
                //if not character, error, shouldnt happen
                if (tile.InteractableOnTile is Character)
                {
                    characterAIHandler.RegisterToWait(character.Character, EWaitReason.WaitForPathToUnblock, character.Character);
                    Character blockingCharacter = tile.InteractableOnTile as Character;


                }

            }

            character.Character.CurrentTile.CharacterLeftTile();
            character.Character.transform.position = tile.TilePos;
            character.Character.ChangeCurrentTile(tile);
            tile.CharacterOnTile(character.Character);
            character.Path.RemoveAt(character.Path.Count - 1);

            //if next tile walkability == no temp
            //Wa

        }
    }


}
class CharacterFindingPath
{
    public Character Character;
    public LotGridTile Destination;
    public List<TileInScoring> PathfindingVisitedTiles = new();
    public List<TileInScoring> PathfindingTilesToVisit = new();
    public bool HasPath;
    public List<TileInScoring> Path = new();

    public CharacterFindingPath(Character character, LotGridTile destination)
    {
        this.Character = character;
        this.Destination = destination;
        HasPath = false;

        PathfindingTilesToVisit.Add(new(character.CurrentTile, destination));
        PathfindingTilesToVisit[0].ShortestRouteTile = new TileInScoring(PathfindingTilesToVisit[0].Tile, destination);
    }
    public TileInScoring GetNextTileToExplore()
    {
        if (PathfindingTilesToVisit.Count == 0)
            return null;
        TileInScoring lowestCostTile = PathfindingTilesToVisit[0];
        for (int i = 1; i < PathfindingTilesToVisit.Count; i++)
        {
            if (PathfindingTilesToVisit[i].CostToTravelHere < lowestCostTile.CostToTravelHere)
            {
                lowestCostTile = PathfindingTilesToVisit[i];
            }
        }
        return lowestCostTile;
    }
}
public class TileInScoring
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