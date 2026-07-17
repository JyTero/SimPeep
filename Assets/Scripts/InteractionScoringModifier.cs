using System;
using UnityEngine;

[Serializable]
public class InteractionScoringModifier
{
    //Replace with something generic when needed
    [SerializeField]
    private TraitSO traitSO;
    public TraitSO TraitSO {  get { return traitSO; } }

    [SerializeField]
    private int traitBonus;

    public int TraitBonus { get { return traitBonus; } }
}
