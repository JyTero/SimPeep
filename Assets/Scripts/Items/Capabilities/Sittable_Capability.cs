using UnityEngine;

public class Sittable_Capability : ItemCapability
{
    [SerializeField]
    private Character_Slot sitSlot;
    public Character_Slot SitSlot {  get { return sitSlot; } }
}
