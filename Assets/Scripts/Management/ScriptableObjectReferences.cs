using UnityEngine;

public class ScriptableObjectReferences : MonoBehaviour
{
    [SerializeField]
    private SlotTypeSO sittingSlotTypeSO;
    public SlotTypeSO SittingSlotTypeSO { get { return sittingSlotTypeSO; } }
}
