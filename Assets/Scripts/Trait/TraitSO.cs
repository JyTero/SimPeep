using UnityEngine;

[CreateAssetMenu(fileName = "TraitSO", menuName = "Scriptable Objects/TraitSO")]
public class TraitSO : ScriptableObject
{
    [SerializeField]
    private string traitName;
    public string TraitName {  get { return traitName; } }

    [SerializeField]
    private string traitDescription;
    public string TraitDescription { get { return traitDescription; } }
}
