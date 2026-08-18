using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : Interactable
{
    [SerializeField]
    private ItemSO itemData;


    [SerializeField]
    private string itemDescription;
    public string ItemDescription { get { return itemDescription; } set { itemDescription = value; } }

    [SerializeField]
    private List<ItemCapabilites> capabilites;
    public List<ItemCapabilites> Capabilites { get { return capabilites; } }

    [SerializeField]
    private List<ItemCapability> capabilityComponents;
    public List<ItemCapability> CapabilityComponents { get { return capabilityComponents; } }



    private Dictionary<ItemCapabilites, ItemCapability> capabilitiesByEnum = new();
    public Dictionary<ItemCapabilites, ItemCapability> CapabilitiesByEnum { get { return capabilitiesByEnum; } }


    private ItemType itemType;
    public ItemType ItemType { get { return itemType; } set { itemType = value; } }


    private int itemPrice;
    public int ItemPrice { get { return itemPrice; } set { itemPrice = value; } }

    [HideInInspector]
    public bool itemInitialised = false;

    protected override void Start()
    {
        base.Start();

        if (itemName == "")
            itemName = itemData.ItemName;

        //if(itemDescription == "")
        //itemDescription  = itemData.ItemDescription;

        itemType = itemData.ItemType;
        itemPrice = itemData.ItemPrice;
        foreach (InteractionSO iso in itemData.AllInteractions)
        {
            interactionSOs.Add(iso);
        }

        itemInitialised = true;

        foreach (ItemCapabilites capability in Capabilites)
        {
            foreach (ItemCapability ic in capabilityComponents)
            {
                if (ic.ThisCapability == capability)
                    capabilitiesByEnum.Add(capability, ic);
            }
        }
    }
}
