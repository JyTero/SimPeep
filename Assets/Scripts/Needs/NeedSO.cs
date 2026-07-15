using UnityEngine;

[CreateAssetMenu(fileName = "NeedSO", menuName = "Scriptable Objects/NeedSO")]
public class NeedSO : ScriptableObject
{
    [SerializeField]
    private NeedType needType;
    public NeedType NeedType { get { return needType; } }

    [SerializeField]
    private int needValue;
    public int NeedValue { get { return needValue; } }
    
    [SerializeField]
    private int needMaxValue;
    public int NeedMaxValue { get { return needMaxValue; } }

    [SerializeField]
    private int needDecay;
    public int NeedDecay { get { return needDecay; } }

    [SerializeField]
    private AnimationCurve needWeightOnInteractionScoring;

    public AnimationCurve NeedWeightOnInteractionScoring { get { return needWeightOnInteractionScoring; } }




}
