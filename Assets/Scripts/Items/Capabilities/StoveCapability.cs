using UnityEngine;

public class StoveCapability : ItemCapability
{
    [SerializeField]
    private Item_Slot stoveCookSlot;
    public Item_Slot StoveCookSlot { get { return stoveCookSlot; } }


}
