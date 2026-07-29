using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : Interactable
{
    [SerializeField]
    private ItemTuningSO itemData;

  
    private string itemDescription;
    public string ItemDescription { get { return itemDescription; } set { itemDescription = value; } }
   
    private ItemType itemType;
    public ItemType ItemType { get { return itemType; } set { itemType = value; } }

    private int itemPrice;
    public int ItemPrice { get { return itemPrice; } set { itemPrice = value; } }


    private void Awake()
    {
        itemName = itemData.ItemName;
        itemDescription  = itemData.ItemDescription;
        itemType = itemData.ItemType;
        itemPrice = itemData.ItemPrice;
        foreach(InteractionSO iso in itemData.AllInteractions)
        {
            interactionSOs.Add(iso);
        }

       // allInteractions = itemData.AllInteractions;
    }
}
