using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class NeedMeter : MonoBehaviour, INeedAlertable
{

    [SerializeField]
    private TextMeshProUGUI needNameField;
    [SerializeField]
    private TextMeshProUGUI needValueField;

    private Needs_UIPanel needsUIPanel;

    //"Session Data" (changes when selected character changes
    private Need thisNeed;
    private string needName;
    private int needValue;
    private int needMaxValue;


    //public void LateStart()
    //{

    //    selectedCharacter = FindAnyObjectByType<Character>();
    //    thisNeed = selectedCharacter.Needs[NeedType.Fun];
    //    needName = thisNeed.NeedType.ToString();

    //    RegisterToNeedAlert();
    //    RefreshVisuals();
    //}
    public void Initialise(Need need, Needs_UIPanel nuip)
    {
        this.needsUIPanel = nuip;
        UpdateData(need);
        RegisterToNeedAlert();
    }

    public void UpdateData(Need need)
    {
        thisNeed = need;
        needName = need.NeedType.ToString();
        needValue = thisNeed.NeedValue;
        needMaxValue = thisNeed.NeedMaxValue;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        needNameField.text = needName;
        needValueField.text = $"{needValue} / {needMaxValue}";
    }
    public void RegisterToNeedAlert()
    {
        needsUIPanel.RegisterToNeedAlert(this, thisNeed);
    }


    public void DeRegisterFromNeedAlert()
    {
        needsUIPanel.RegisterToNeedAlert(this, thisNeed);
    }
}
