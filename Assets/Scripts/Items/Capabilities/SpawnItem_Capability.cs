using UnityEngine;

public class SpawnItem_Capability : ItemCapability
{
    [SerializeField]
    private Item_Slot spawnSlot;
    public Item_Slot SpawnSlot { get { return spawnSlot; } }
}
