using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomLogic_InstructionSO", menuName = "Scriptable Objects/Instruction/CustomLogic_InstructionSO")]
public class CustomLogic_InstructionSO : InstructionSO
{

    [SerializeField]
    private ECustomLogicItem customlogicItem;
    public ECustomLogicItem CustomlogicItem { get { return customlogicItem; } }

    //Contains the item specific logic enum int value. This is used by the engine
    private int customLogicIndex;
    public int CustomLogicIndex { get { return customLogicIndex; } }


    bool customTV;
    [SerializeField, ShowIf("customTV")]
    private ETVCustomLogic tvCustomLogic;
    public ETVCustomLogic TvCustomLogic { get { return tvCustomLogic; } }



    private void OnValidate()
    {
        switch (customlogicItem)
        {
            case ECustomLogicItem.Default:
                customTV = false;
                break;
            case ECustomLogicItem.TV:
                customTV = true;
                customLogicIndex = (int)tvCustomLogic;
                break;
            default:
                break;
        }
    }
    public enum ECustomLogicItem
    {
        Default,
        TV,

    }
}
