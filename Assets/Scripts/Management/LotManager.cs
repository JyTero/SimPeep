using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static UnityEditor.PlayerSettings;

public class LotManager : ManagementCore
{
    private List<WorldLot> allLots = new();
    public List<WorldLot> AllLots { get { return allLots; } }

    private List<StoredInteraction> allStoredInteractions = new();
    private WorldLot lot;

    protected override void Start()
    {
        base.Start();
        lot = FindAnyObjectByType<WorldLot>();
        AddNewLot(lot);
        
    }

    public void LoadingScreen()
    {
        allStoredInteractions = GetAllInteractionsOnLot(lot);
    }
    public void AddNewLot(WorldLot lot)
    {
        allLots.Add(lot);
    }

    public void NewItemOnLot(ItemBase item, WorldLot lot)
    {
        this.lot.AddItemToLot(item);

        foreach(InteractionSO itso in item.InteractionSOs)
        {
            item.NewStoredInteraction(new StoredInteraction(itso, item));
        }
    }

    public List<StoredInteraction> GetAllInteractionsOnLot(WorldLot lot)
    {
        List<StoredInteraction> interactions = new();
        foreach (ItemBase item in lot.ItemsOnLot)
        {
            foreach (StoredInteraction storedInteraction in item.AllInteractions)
                interactions.Add(storedInteraction);
            //foreach (InteractionSO interactonSO in item.AllInteractions)
            //{
            //    interactions.Add(new StoredInteraction(interactonSO, item));
            //}
        }
        foreach (Character chara in lot.CharactersOnLot)
        {
            foreach (StoredInteraction storedInteraction in chara.AllInteractions)
                interactions.Add(storedInteraction);
            //foreach (InteractionSO interactonSO in chara.AllInteractions)
            //{
            //    interactions.Add(new StoredInteraction(interactonSO, chara));
            //}
        }
        return interactions;
    }

    public StoredInteraction FindSuitableStoredInteractionOnLot(InteractionSO itso, WorldLot lot)
    {
        foreach (StoredInteraction si in allStoredInteractions)
        {
            if (si.InteractionTuningSO == itso)
                return si;
        }
        return null;
    }

    public List<StoredInteraction> GetAllStoredInteractionsOnLot(WorldLot lot)
    {
        return allStoredInteractions;
    }

    public void RemoveLot(WorldLot lot)
    {
        allLots.Remove(lot);
    }
}
