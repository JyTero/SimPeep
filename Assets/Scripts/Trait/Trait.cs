using System;
using UnityEngine;


public class Trait
{
    private TraitSO traitSO;

    private string traitName;
    public string TraitName { get { return traitName; } }


    private string traitDescription;
    public string TraitDescription { get { return traitDescription; } }

    public Trait(TraitSO tso)
    {
        traitSO = tso;
        traitName = tso.TraitName;
        traitDescription = tso.TraitDescription;
    }

}

public enum TraitE
{
    Bookworm,
}
