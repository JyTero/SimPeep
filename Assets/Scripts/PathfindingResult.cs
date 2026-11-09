using System.Collections.Generic;
using UnityEngine;

public class PathfindingResult
{
    private Character character;
    public Character Character { get { return character; } }

    private LotGridTile destinationTile;
    public LotGridTile DestinationTile { get { return DestinationTile; } }


    private ERoutingResult routingResult;
    public ERoutingResult RoutingResult { get { return routingResult; } }

    private ERoutingResultReason routingReason;
    public ERoutingResultReason RoutingReason { get { return routingReason; } }

    public List<TileInScoring> Path = new();

    public PathfindingResult(ERoutingResult routingResult, ERoutingResultReason routingReason, Character character, LotGridTile destination)
    {
        this.routingResult = routingResult;
        this.routingReason = routingReason;
        this.character = character;
        this.destinationTile = destination;
    }
}
