using System.Collections.Generic;
using UnityEngine;

public class Need
{
    [SerializeField]
    private NeedType needType;
    public NeedType NeedType { get { return needType; } set { needType = value; } }

    [SerializeField]
    private float needValue;
    public float NeedValue { get { return needValue; } set { needValue = value; } }

    [SerializeField]
    private int needMaxValue;
    public int NeedMaxValue { get { return needMaxValue; } set { needMaxValue = value; } }

    [SerializeField]
    private float needDecay;
    public float NeedDecay { get { return needDecay; } set { needDecay = value; } }

    public float TimeSinceLastNeedDecay;

    [SerializeField]
    private AnimationCurve needWeightOnInteractionScoring;
    public AnimationCurve NeedWeightOnInteractionScoring { get { return needWeightOnInteractionScoring; } }

    //Runtime Data
    public Character Owner;

    private List<INeedAlertable> alertables = new();
    public List<INeedAlertable> Alertables { get { return alertables; } }

    public Need(NeedSO needSO, Character _owner)
    {
        needType = needSO.NeedType;
        needValue = needSO.NeedValue;
        needMaxValue = needSO.NeedMaxValue;
        needDecay = -needSO.NeedDecay;
        Owner = _owner;
        needWeightOnInteractionScoring = needSO.NeedWeightOnInteractionScoring;
        TimeSinceLastNeedDecay = 0;
    }

    public void AddAlertable(INeedAlertable alertable)
    {
        alertables.Add(alertable);
    }
    public void RemoveAlertable(INeedAlertable alertable)
    {
        alertables.Remove(alertable);
    }
}
