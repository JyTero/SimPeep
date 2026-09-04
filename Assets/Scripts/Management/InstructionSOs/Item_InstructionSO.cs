using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_InstructionSO", menuName = "Scriptable Objects/Instruction/Item_InstructionSO")]
public class Item_InstructionSO : InstructionSO
{
    [SerializeField]
    private EItem_InstructionType instructionType;
    public EItem_InstructionType InstructionType { get { return  instructionType; } }

    //Spawn
    //[SerializeField, InfoBox("Choose only one of top level booleans", EInfoBoxType.Warning)]
    private bool spawnItem;
    public bool SpawnItem { get { return spawnItem; } }

    [SerializeField, ShowIf("spawnItem")]
    private GameObject itemToSpawn;
    public GameObject ItemToSpawn { get { return itemToSpawn; } }

    [SerializeField, ShowIf("spawnItem")]
    private EItemDestination whereToSpawnItem;
    public EItemDestination WhereToSpawnItem { get { return whereToSpawnItem; } }

    [SerializeField, ShowIf(EConditionOperator.And, "spawnItem", "spawnItemToSlot")]
    private EItemLocation slotParentItemLocationSpwn;
    public EItemLocation SlotParentItemLocationSpwn { get { return slotParentItemLocationSpwn; } }

    [SerializeField, ShowIf(EConditionOperator.And, "spawnItem", "spawnItemToSlot")]
    private SlotTypeSO slotTypeSOSpwn;
    public SlotTypeSO SlotTypeSOSpwn { get { return slotTypeSOSpwn; } }


    //Destroy
    //[SerializeField]
    private bool destroyItem;
    public bool DestroyItem { get { return destroyItem; } }

    //MoveThis
    //[SerializeField]
    private bool moveThisItem;
    public bool MoveThisItem { get { return moveThisItem; } }

    [SerializeField, ShowIf(EConditionOperator.Or, "moveThisItem","moveFromThisItemSlot")]
    private EItemDestination whereToMoveItem;
    public EItemDestination WhereToMoveItem { get { return whereToMoveItem; } }

    [SerializeField, ShowIf(EConditionOperator.And, "moveThisItem", "moveItemToSlot")]
    private EItemLocation slotParentItemLocationMove;
    public EItemLocation SlotParentItemLocationMove { get { return slotParentItemLocationMove; } }

    [SerializeField, ShowIf(EConditionOperator.And, "moveThisItem", "moveItemToSlot")]
    private ItemSO targetItemType;
    public ItemSO TargetItemType { get { return targetItemType; } }

    [SerializeField, ShowIf(EConditionOperator.And, "moveThisItem", "moveItemToSlotOnCertainItemType")]
    private SlotTypeSO targetSlotType;
    public SlotTypeSO TargetSlotType { get { return targetSlotType; } }

    //MoveFromThisItem's Slot
    //[SerializeField]
    private bool moveFromThisItemSlot;
    public bool MoveFromThisItemSlot { get { return moveFromThisItemSlot; } }

    [SerializeField, ShowIf("moveFromThisItemSlot")]
    private SlotTypeSO slotTypeToPickFrom;
    public SlotTypeSO SlotTypeToPickFrom {  get { return slotTypeToPickFrom; }}
    //USES ItemDestination whereToMoveItem

    //Replace with
    //[SerializeField]
    private bool replaceItem;
    public bool ReplaceItem { get { return replaceItem; } }

    [SerializeField, ShowIf("replaceItem")]
    private EItemLocation thisInstructionTargetItem;
    public EItemLocation ThisInstructionTargetItem { get { return thisInstructionTargetItem; } }
    [SerializeField, ShowIf("replaceItem")]
    private GameObject newItemPrefab;
    public GameObject NewItemPrefab { get { return newItemPrefab; } }


    //Call Routine
    //[SerializeField]
    private bool runRoutine;
    public bool RunRoutine { get { return runRoutine; } }

    [SerializeField, ShowIf("runRoutine")]
    private ERoutine routine;
    public ERoutine Routine {  get { return routine; } }

    //RunInteractionAsInstruction
    //[SerializeField]
    private bool runInteractionAsInstruction;
    public bool RunInteractionAsInstruction {  get { return runInteractionAsInstruction; } }

    [SerializeField, ShowIf("runInteractionAsInstruction")]
    private InteractionSO interactionToRunSO;
    public InteractionSO InteractionToRunSO {  get { return interactionToRunSO; } }



    //[SerializeField, ShowIf("moveToLotSpace")]
    //private WorldLot lotToMoveIn;
    //public WorldLot LotToMoveIn { get { return lotToMoveIn; } }


    //[SerializeField, ShowIf(EConditionOperator.Or, "moveToLotSpace", "moveToWorldSpace")]
    //private Vector3 destination;
    //public Vector3 Destination { get { return destination; } }

    private bool spawnItemToSlot, moveItemToSlot, moveItemToSlotOnCertainItemType = false;

    private List<bool> interactionTypes = new List<bool>() { };

    private void OnValidate()
    {
   
        switch (instructionType)
        {
            case EItem_InstructionType.Default:
                break;
            case EItem_InstructionType.Spawn:
                spawnItem = true;
                destroyItem = false;
                runRoutine = false;
                runInteractionAsInstruction = false;
                break;
            case EItem_InstructionType.Destroy:
                spawnItem = false;
                destroyItem = true;
                runRoutine = false;
                runInteractionAsInstruction = false;
                break;
            case EItem_InstructionType.MoveThis:
                spawnItem = false;
                destroyItem = false;
                moveThisItem = true;
                runRoutine = false;
                runInteractionAsInstruction = false;
                break;
            case EItem_InstructionType.MoveFromThis:
                spawnItem = false;
                destroyItem = false;
                moveFromThisItemSlot = true;
                runRoutine = false;
                runInteractionAsInstruction = false;
                break;
            case EItem_InstructionType.Replace:
                spawnItem = false;
                destroyItem = false;
                replaceItem = true;
                runRoutine = false;
                runInteractionAsInstruction = false;
                break;
            case EItem_InstructionType.Routine:
                spawnItem = false;
                destroyItem = false;
                runRoutine = true;
                runInteractionAsInstruction = false;
                break;
            case EItem_InstructionType.RunInteraction:
                spawnItem = false;
                destroyItem = false;
                runRoutine = false;
                runInteractionAsInstruction = true;
                break;
            default:
                break;
        }

        switch (WhereToSpawnItem)
        {
            case EItemDestination.Default:
                spawnItemToSlot = false;
                break;
            case EItemDestination.LotSpace:
                spawnItemToSlot = false;
                break;
            case EItemDestination.WorldSpace:
                spawnItemToSlot = false;
                break;
            case EItemDestination.InCharactacter:
                spawnItemToSlot = false;
                break;
            case EItemDestination.OnCharacter:
                spawnItemToSlot = false;
                break;
            case EItemDestination.ItemSlot:
                spawnItemToSlot = true;
                break;
            default:
                break;
        }
        switch (WhereToMoveItem)
        {
            case EItemDestination.Default:
                moveItemToSlot = false;
                break;
            case EItemDestination.LotSpace:
                moveItemToSlot = false;
                break;
            case EItemDestination.WorldSpace:
                moveItemToSlot = false;
                break;
            case EItemDestination.InCharactacter:
                moveItemToSlot = false;
                break;
            case EItemDestination.OnCharacter:
                moveItemToSlot = false;
                break;
            case EItemDestination.ItemSlot:
                moveItemToSlot = true;
                break;
            default:
                break;
        }
        switch (SlotParentItemLocationMove)
        {
            case EItemLocation.ThisItem:
                moveItemToSlotOnCertainItemType = false;
                break;
            case EItemLocation.Any:
                moveItemToSlotOnCertainItemType = false;
                break;
            case EItemLocation.AnyOfType:
                moveItemToSlotOnCertainItemType = true;
                break;
            case EItemLocation.OnItemCreatedByInteraction:
                moveItemToSlotOnCertainItemType = false;
                break;
            case EItemLocation.OnMainItem:
                moveItemToSlotOnCertainItemType = false;
                break;
            case EItemLocation.OnHeldItem:
                moveItemToSlotOnCertainItemType = false;
                break;
            case EItemLocation.OnMainItemSlot:
                moveItemToSlotOnCertainItemType = false;
                break;
            default:
                break;
        }
    }
}