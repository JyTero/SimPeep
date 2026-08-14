using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : ManagementCore
{
    private List<ItemBase> allItems = new();
    public List<ItemBase> AllItems { get { return allItems; } }

    private Dictionary<ItemBase, Transform> itemsMoving = new();
    public Dictionary<ItemBase, Transform> ItemsMoving { get { return itemsMoving; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

    }

    public ItemBase SpawnNewItem(GameObject prefab, WorldLot lot)
    {
        ItemBase returne = Instantiate(prefab, NegSpawnPos, Quaternion.identity).GetComponent<ItemBase>();
        allItems.Add(returne);
        lotManager.NewItemOnLot(returne, lot);
        return returne;

    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);

        if (ItemsMoving.Count > 0)
            UpdateMovingItems();
    }

    public void RegisterMovingItem(ItemBase item, Transform anchor)
    {
        ItemsMoving.Add(item, anchor);
    }
    
    private void UpdateMovingItems()
    {
        foreach(ItemBase item in ItemsMoving.Keys)
        {
            item.transform.position = ItemsMoving[item].transform.position;
        }
    }

    public void DeregisterMovingItem(ItemBase item)
    {
        ItemsMoving.Remove(item);
    }
}


