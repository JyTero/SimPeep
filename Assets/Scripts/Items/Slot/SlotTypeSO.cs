using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotTypeSO", menuName = "Scriptable Objects/SlotTypeSO")]
public class SlotTypeSO : ScriptableObject
{
    [SerializeField]
    private string typeName;
    public string TypeName { get { return typeName; } }

    [SerializeField, Tooltip("Slot is to be used only during specific interactions that call for it (spawn slots, cook slots etc)")]
    private bool limitedSlot;
    public bool LimitedSlot { get { return limitedSlot; } }


    [SerializeField]
    private bool limitSlotToSpecificItemType = false; //Make better (A list of suitable item types
    [SerializeField, ShowIf("limitSlotToSpecificItemType")]
    private List<ItemSO> validItemSO;
    public List<ItemSO> ValidItemSO { get { return validItemSO; } }
}
