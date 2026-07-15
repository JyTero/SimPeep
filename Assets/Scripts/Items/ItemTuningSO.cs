using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemTuningSO", menuName = "Scriptable Objects/ItemTuningSO")]

public class ItemTuningSO : ScriptableObject
{
    [SerializeField]
    private string itemName;
    public string ItemName {  get { return itemName; } set { itemName = value; } }

    [SerializeField]
    private string itemDescription;
    public string ItemDescription {  get { return itemDescription; } set { itemDescription = value; } }

    [SerializeField]
    private ItemType itemType;
    public ItemType ItemType {  get { return itemType; } set { itemType = value; } }

    [SerializeField]
    private int itemPrice;
    public int ItemPrice {  get { return itemPrice; } set { itemPrice = value; } }

    [SerializeField]
    private List<InteractionSO> allInteractions = new();
    public List<InteractionSO> AllInteractions { get { return allInteractions; } }
}
