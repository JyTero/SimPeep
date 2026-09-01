//using NaughtyAttributes;
//using System;
//using UnityEngine;

//[Serializable]
//public class ItemInstructionData
//{

//    //Spawn
//    [SerializeField, Foldout("Integers")]
//    private bool spawnItem;
//    public bool SpawnItem { get { return spawnItem; } }

//    [SerializeField, ShowIf("spawnItem")]
//    private GameObject itemToSpawn;
//    public GameObject ItemToSpawn { get { return itemToSpawn; } }

//    //Destroy
//    [SerializeField]
//    private bool destroyItem;
//    public bool DestroyItem { get { return destroyItem; } }

//    //Move
//    [SerializeField]
//    private bool moveThisItem;
//    public bool MoveThisItem { get { return moveThisItem; } }

//    [SerializeField, ShowIf("moveThisItem")]
//    private ItemDestination whereToMoveItem;
//    public ItemDestination WhereToMoveItem { get { return whereToMoveItem; } }
//    private bool moveToLotSpace, moveToWorldSpace, moveToInCharacter, moveToOnCharacter = false;

//    //Replace with
//    [SerializeField]
//    private bool replaceItem;
//    public bool ReplaceItem { get { return replaceItem; } }

//    [SerializeField, ShowIf(EConditionOperator.Or, "replaceItem", "moveThisItem")]
//    private ItemLocation thisInstructionTargetItem;
//    public ItemLocation ThisInstructionTargetItem { get { return thisInstructionTargetItem; } }
   
//    [SerializeField, ShowIf("replaceItem")]
//    private GameObject newItemPrefab;
//    public GameObject NewItemPrefab { get { return newItemPrefab; } }
//}
