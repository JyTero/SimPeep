using UnityEngine;

public class Item_InstructionData
{
    //BASE
    private string instructionName;
    public string InstructionName { get { return instructionName; } }

    private EItem_InstructionType instructionType;
    public EItem_InstructionType InstructionType { get { return instructionType; } }

    //Spawn
    private bool spawnItem;
    public bool SpawnItem { get { return spawnItem; } }

    private GameObject itemToSpawn;
    public GameObject ItemToSpawn { get { return itemToSpawn; } }

    private EItemDestination whereToSpawnItem;
    public EItemDestination WhereToSpawnItem { get { return whereToSpawnItem; } }

    private EItemLocation slotParentItemLocationSpwn;
    public EItemLocation SlotParentItemLocationSpwn { get { return slotParentItemLocationSpwn; } }

    private SlotTypeSO slotTypeSOSpwn;
    public SlotTypeSO SlotTypeSOSpwn { get { return slotTypeSOSpwn; } }


    //Destroy
    [SerializeField]
    private bool destroyItem;
    public bool DestroyItem { get { return destroyItem; } }

    //MoveThis
    [SerializeField]
    private bool moveThisItem;
    public bool MoveThisItem { get { return moveThisItem; } }


    private EItemDestination whereToMoveItem;
    public EItemDestination WhereToMoveItem { get { return whereToMoveItem; } }


    private EItemLocation slotParentItemLocationMove;
    public EItemLocation SlotParentItemLocationMove { get { return slotParentItemLocationMove; } }


    private ItemSO targetItemType;
    public ItemSO TargetItemType { get { return targetItemType; } }


    private SlotTypeSO targetSlotType;
    public SlotTypeSO TargetSlotType { get { return targetSlotType; } }

    //MoveFromThisItem's Slot
    private bool moveFromThisItemSlot;
    public bool MoveFromThisItemSlot { get { return moveFromThisItemSlot; } }


    private SlotTypeSO slotTypeToPickFrom;
    public SlotTypeSO SlotTypeToPickFrom { get { return slotTypeToPickFrom; } }
    //USES ItemDestination whereToMoveItem

    //Replace with
    private bool replaceItem;
    public bool ReplaceItem { get { return replaceItem; } }


    private EItemLocation thisInstructionTargetItem;
    public EItemLocation ThisInstructionTargetItem { get { return thisInstructionTargetItem; } }

    private GameObject newItemPrefab;
    public GameObject NewItemPrefab { get { return newItemPrefab; } }


    //Call Routine
    [SerializeField]
    private bool runRoutine;
    public bool RunRoutine { get { return runRoutine; } }

    private ERoutine routine;
    public ERoutine Routine { get { return routine; } }


    //RunInteractionAsInstruction
    private bool runInteractionAsInstruction;
    public bool RunInteractionAsInstruction { get { return runInteractionAsInstruction; } }

    private InteractionSO interactionToRunSO;
    public InteractionSO InteractionToRunSO { get { return interactionToRunSO; } }

    private StoredInteraction interactionToRunStored;
    public StoredInteraction InteractionToRunStored {  get { return interactionToRunStored; } }

    public Slot knownSlot;
    public ItemBase knownItem;
    public LotGridTile knownTile;


    public Item_InstructionData(Item_InstructionSO so)
    {
        instructionName = so.name;
        instructionType = so.InstructionType;

        spawnItem = so.SpawnItem;
        itemToSpawn = so.ItemToSpawn;
        whereToSpawnItem = so.WhereToSpawnItem;
        slotParentItemLocationSpwn = so.SlotParentItemLocationSpwn;
        slotTypeSOSpwn = so.SlotTypeSOSpwn;

        destroyItem = so.DestroyItem;

        moveThisItem = so.MoveThisItem;
        whereToMoveItem = so.WhereToMoveItem;
        slotParentItemLocationMove = so.SlotParentItemLocationMove;
        targetItemType = so.TargetItemType;
        targetSlotType = so.TargetSlotType;


        moveFromThisItemSlot = so.MoveFromThisItemSlot;
        slotTypeToPickFrom = so.SlotTypeToPickFrom;

        replaceItem = so.ReplaceItem;
        thisInstructionTargetItem = so.ThisInstructionTargetItem;
        newItemPrefab = so.NewItemPrefab;

        runRoutine = so.RunRoutine;
        routine = so.Routine;

        runInteractionAsInstruction = so.RunInteractionAsInstruction;
        interactionToRunSO = so.InteractionToRunSO;

    }

    public Item_InstructionData()
    {
    }

    public Item_InstructionData(EItem_InstructionType instructionType, EItemDestination destination, EItemLocation destinationSource)
    {
        HandleEItem_InstructionType(instructionType);
        this.instructionType = instructionType;
        whereToSpawnItem = destination;
        whereToMoveItem = destination;
        slotParentItemLocationSpwn = destinationSource;
        slotParentItemLocationMove = destinationSource;
    }

    public Item_InstructionData(EItem_InstructionType instructionType, StoredInteraction storedInteraction)
    {
        HandleEItem_InstructionType(instructionType);
        this.instructionType = instructionType;
        interactionToRunStored = storedInteraction;
    }

    private void HandleEItem_InstructionType(EItem_InstructionType type)
    {
        switch (type)
        {
            case EItem_InstructionType.Default:
                break;
            case EItem_InstructionType.Spawn:
                spawnItem = true;
                break;
            case EItem_InstructionType.Destroy:
                destroyItem = true;
                break;
            case EItem_InstructionType.MoveThis:
                moveThisItem = true;
                break;
            case EItem_InstructionType.MoveFromThis:
                moveFromThisItemSlot = true;
                break;
            case EItem_InstructionType.Replace:
                replaceItem = true;
                break;
            case EItem_InstructionType.Routine:
                runRoutine = true;
                break;
            case EItem_InstructionType.RunInteraction:
                runInteractionAsInstruction = true;
                break;
            default:
                break;
        }
    }

}
