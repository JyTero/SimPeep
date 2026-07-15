using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class Needs_UIPanel : UIPanel
{

    [SerializeField]
    private List<NeedMeter> needMeters = new();

    private Dictionary<Need, NeedMeter> needMetersByNeed = new();
    
    private Character selectedCharacter;

    protected void Start()
    {
        base.Start();
        selectedCharacter = FindAnyObjectByType<Character>();
        //AssignNeedMeters();
        
    }
    public void Debuglandia()
    {
        AssignNeedMeters();
    }
    private void AssignNeedMeters()
    {
        int i = 0;
        foreach(Need need in selectedCharacter.Needs.Values.ToList<Need>())
        {
            needMetersByNeed.Add(need, needMeters[i]);
            needMeters[i].Initialise(need, this);

            ++i;
        }
    }

    public void UpdatePanel()
    {

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

