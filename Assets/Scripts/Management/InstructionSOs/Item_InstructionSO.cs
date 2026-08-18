using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_InstructionSO", menuName = "Scriptable Objects/Instruction/Item_InstructionSO")]
public class Item_InstructionSO : InstructionSO
{
    //Spawn
    [SerializeField, InfoBox("Choose only one of top level booleans", EInfoBoxType.Warning)]
    private bool spawnItem;
    public bool SpawnItem { get { return spawnItem; } }

    [SerializeField, ShowIf("spawnItem")]
    private GameObject itemToSpawn;
    public GameObject ItemToSpawn { get { return itemToSpawn; } }

    //Move
    [SerializeField]
    private bool moveThisItem;
    public bool MoveThisItem { get { return moveThisItem; } }

    [SerializeField, ShowIf("moveThisItem")]
    private ItemLocation whereToMoveItem;
    public ItemLocation WhereToMoveItem { get { return whereToMoveItem; } }
    private bool moveToLotSpace, moveToWorldSpace, moveToInCharacter, moveToOnCharacter = false;

    //[SerializeField, ShowIf("moveToLotSpace")]
    //private WorldLot lotToMoveIn;
    //public WorldLot LotToMoveIn { get { return lotToMoveIn; } }


    //[SerializeField, ShowIf(EConditionOperator.Or, "moveToLotSpace", "moveToWorldSpace")]
    //private Vector3 destination;
    //public Vector3 Destination { get { return destination; } }


    private void OnValidate()
    {
        switch (WhereToMoveItem)
        {
            case ItemLocation.Default:
                moveToLotSpace = false;
                moveToWorldSpace = false;
                moveToInCharacter = false;
                moveToOnCharacter = false;
                break;
            case ItemLocation.LotSpace:
                moveToLotSpace = true;
                moveToWorldSpace = false;
                moveToInCharacter = false;
                moveToOnCharacter = false;
                break;
            case ItemLocation.WorldSpace:
                moveToLotSpace = false;
                moveToWorldSpace = true;
                moveToInCharacter = false;
                moveToOnCharacter = false;
                break;
            case ItemLocation.InCharactacter:
                moveToLotSpace = false;
                moveToWorldSpace = false;
                moveToInCharacter = true;
                moveToOnCharacter = false;
                break;
            case ItemLocation.OnCharacter:
                moveToLotSpace = false;
                moveToWorldSpace = false;
                moveToInCharacter = false;
                moveToOnCharacter = true;
                break;
            default:
                break;
        }
    }
}


public enum ItemLocation
{
    Default,
    LotSpace,           //Basic on lot object
    WorldSpace,         //Item outside of lots but in world
    InCharactacter,     //In character inventory (TBD)
    OnCharacter,        //Character carrying item
    ItemSlot,           //ItemSlot on item
}
