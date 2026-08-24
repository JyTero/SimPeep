using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : Interactable
{
    [SerializeField]
    private ItemSO itemData;
    public ItemSO ItemData {  get { return itemData; } }


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

    private int itemPrice;
    public int ItemPrice { get { return itemPrice; } set { itemPrice = value; } }

    private List<Item_Slot> itemSlotsOnItem = new();
    public List<Item_Slot> ItemSlotsOnItem { get { return itemSlotsOnItem; }}

    private List<Character_Slot> characterSlotsOnItem = new();
    public List<Character_Slot> CharacterSlotsOnItem { get { return characterSlotsOnItem; }}

    [HideInInspector]
    public bool itemInitialised = false;

    protected override void Start()
    {
        base.Start();

       
    }

    
}
