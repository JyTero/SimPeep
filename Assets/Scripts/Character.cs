using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Character : Interactable
{
    //[SerializeField]
    //private string characterName;
    //public string CharacterName { get { return characterName; } } 

    //Debug
    public int characterSpeed = 2;

    private NeedsEngine needsEngine;

    private Dictionary<NeedType, Need> needs = new();
    public Dictionary<NeedType, Need> Needs { get { return needs; } }

    [SerializeField]
    private List<TraitSO> traits = new();
    public List<TraitSO> Traits { get { return traits; } }

    //DEBUG
    [SerializeField]
    private List<NeedSO> needSOs = new();

    private void Awake()
    {
        MakeNeedSOsToObjects();
        
    }
    protected override void Start()
    {
        base.Start();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        needsEngine.RegisterToNeedsEngine(this);

        //DEBUG
        thisLot = FindAnyObjectByType<WorldLot>();
    }

    //DEBUG
    private void MakeNeedSOsToObjects()
    {
        foreach (NeedSO needSO in needSOs)
        {
            Need need = new Need(needSO, this);
            needs.Add(needSO.NeedType, need);
        }
    }


    private void OnDestroy()
    {
        needsEngine.DeRegisterFromNeedsEngine(this);
    }
}
