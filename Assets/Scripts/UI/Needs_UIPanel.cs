using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class Needs_UIPanel : UIPanel
{
    [SerializeField]
    private GameObject needMeterPrefab;
    
    private List<NeedMeter> needMeters = new();

    private Dictionary<Need, NeedMeter> needMetersByNeed = new();

    private List<GameObject> activeNeedMeterGOs = new();
    private List<GameObject> NeedMeterGOPool = new();


    protected void Start()
    {
        base.Start();

        foreach (Transform child in transform)
        {
            NeedMeter needMeter = child.gameObject.GetComponent<NeedMeter>();
            if (needMeter)
                needMeters.Add(needMeter);
        }

        //AssignNeedMeters();

    }
    //public override void OnSelectCharacterChange()
    //{
    //    base.OnSelectCharacterChange();

    //}
    protected override void OCSS()
    {
       ActivatePanel();
       // UpdatePanel();

    }

    public override void ActivatePanel()
    {
        base.ActivatePanel();

        List<Need> needs = UIController.SelectedCharacter.Needs.Values.ToList();
        DisableAllActiveNeedMeters();

        //Confirm pool
        if (!(NeedMeterGOPool.Count >= needs.Count))
            MakeNeedUIObjectGOs(needs.Count - NeedMeterGOPool.Count);

        //Populate
        int i = 0;
        foreach (Need need in needs)
        {
            GameObject relationshipGO = NeedMeterGOPool[i];
            activeNeedMeterGOs.Add(relationshipGO);
            relationshipGO.SetActive(true);
            relationshipGO.GetComponent<NeedMeter>().Initialise(need, this);
            i++;
        }
    }

    private void MakeNeedUIObjectGOs(int amount)
    {
        while (amount > 0)
        {
            GameObject go = Instantiate(needMeterPrefab, transform);
            go.SetActive(false);
            NeedMeterGOPool.Add(go);
            amount--;

        }
    }

    public void UpdatePanel()
    {
            ActivatePanel();
        //needMetersByNeed.Clear();
        //AssignNeedMeters();
    }

    //////////////////


    private void AssignNeedMeters()
    {
        int i = 0;
        foreach (Need need in uiController.SelectedCharacter.Needs.Values.ToList<Need>())
        {
            needMetersByNeed.Add(need, needMeters[i]);
            needMeters[i].Initialise(need, this);

            ++i;
        }
    }
    
    private void DisableAllActiveNeedMeters()
    {
        for (int j = activeNeedMeterGOs.Count - 1; j >= 0; j--)
        {
            GameObject needMeterGo = activeNeedMeterGOs[j];
            needMeterGo.GetComponent<NeedMeter>().DeRegisterFromNeedAlert();

            needMeterGo.SetActive(false);
            NeedMeterGOPool.Add(needMeterGo);
            activeNeedMeterGOs.RemoveAt(j);
        }
    }

    public override void DisablePanel()
    {
        base.DisablePanel();

        needMetersByNeed.Clear();
        DisableAllActiveNeedMeters();
    }

    public void RegisterToNeedAlert(INeedAlertable alertable, Need need)
    {
        needsEngine.RegisterForNeedChangeAlert(alertable, need);
    }

    public void DeRegisterFromNeedAlert(INeedAlertable alertable, Need need)
    {
        needsEngine.DeRegisterToNeedChangeAlert(alertable, need);
    }
}

