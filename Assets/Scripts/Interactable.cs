using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    protected string itemName;
    public string ItemName { get { return itemName; } set { itemName = value; } }

    protected WorldLot thisLot;
    public WorldLot ThisLot { get { return thisLot; } }

    protected LotGridTile currentTile;
    public LotGridTile CurrentTile { get { return currentTile; } }

    [SerializeField]
    protected Vector2Int footprint = new(1, 1);
    public Vector2Int Footprint { get { return footprint; } }


    protected List<StoredInteraction> storedInteractions = new();
    public List<StoredInteraction> StoredInteractions { get { return storedInteractions; } }

    [SerializeField]
    protected List<InteractionSO> interactionSOs = new();
    public List<InteractionSO> InteractionSOs { get { return interactionSOs; } }
  
    [SerializeField]
    protected Character_Slot interactionSlot;
    public Character_Slot InteractionSlot { get { return interactionSlot; } }

    protected virtual void Start()
    {

    }

    [Header("DEBUG")]
    [SerializeField]
    private bool DebugDrawFootprintGrid;
    [SerializeField]
    private bool debugDrawForwardLine;

    public void NewStoredInteraction(StoredInteraction sInteraction)
    {
        storedInteractions.Add(sInteraction);
    }
    public void ChangeCurrentLot(WorldLot lot)
    {
        thisLot = lot;
    }

    public void ChangeCurrentTile(LotGridTile tile)
    {
        currentTile = tile;
    }


    //public void ChangeCurrentTiles(List<LotGridTile> tiles)
    //{
    //    currentTile.Clear();
    //    currentTile = tiles;
    //}

    //protected void GenerateStoredInteractions()
    //{
    //    foreach(InteractionSO itso in interactionSOs)
    //    {
    //        allInteractions.Add(new StoredInteraction(itso, this));
    //    }
    //}

    private void OnDrawGizmos()
    {
        if (DebugDrawFootprintGrid)
        {
            for (int x = 0; x <= footprint.x; x++)
            {
                Vector3 start = transform.position +
                                new Vector3(x * 1, 0, 0);

                Vector3 end = transform.position +
                              new Vector3(x * 1, 0, footprint.y * 1);

                Gizmos.DrawLine(start, end);
            }

            for (int y = 0; y <= footprint.y; y++)
            {
                Vector3 start = transform.position +
                                new Vector3(0, 0, y * 1);

                Vector3 end = transform.position +
                              new Vector3(footprint.x * 1, 0, y * 1);

                Gizmos.DrawLine(start, end);
            }
        }
        if (debugDrawForwardLine)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
            transform.position + transform.up * 0.5f,
            transform.position + transform.forward * 0.8f + transform.up * 0.5f);
        }

    }
}

