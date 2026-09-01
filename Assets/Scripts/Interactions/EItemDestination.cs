using UnityEngine;

public enum EItemDestination
{
    Default,
    LotSpace,           //Basic on lot object
    WorldSpace,         //Item outside of lots but in world
    InCharactacter,     //In character inventory (TBD)
    OnCharacter,        //Character carrying item
    ItemSlot,           //ItemSlot on item
}
