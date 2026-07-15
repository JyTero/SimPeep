using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class LotManager : ManagementCore                    
{
    private List<WorldLot> allLots = new();
    public List<WorldLot> AllLots { get { return allLots; } }

    protected override void Start()
    {
        base.Start();
        AddNewLot(FindAnyObjectByType<WorldLot>());
    }

    public void AddNewLot(WorldLot lot)
    {
        allLots.Add(lot);
    }

    public List<StoredInteraction> GetAllInteractionsOnLot(WorldLot lot)
    {
        List<StoredInteraction> interactions = new();
        foreach (ItemBase item in lot.ItemsOnLot)
        {
            foreach (InteractionSO interactonSO in item.AllInteractions)
            {
                interactions.Add(new StoredInteraction(interactonSO, item));
            }
        }
        foreach (Character chara in lot.CharactersOnLot)
        {
            foreach (InteractionSO interactonSO in chara.AllInteractions)
            {
                interactions.Add(new StoredInteraction(interactonSO, chara));
            }
        }
        return interactions;
    }
    public void RemoveLot(WorldLot lot)
    {
        allLots.Remove(lot);
    }
}
