using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    protected string itemName;
    public string ItemName { get { return itemName; } set { itemName = value; } }

    protected WorldLot thisLot;
    public WorldLot ThisLot { get { return thisLot; } }

    protected virtual void Start()
    {
        //LateStartTimer();
    }
    private IEnumerator LateStartTimer()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        LateStart();
    }
    public void LateStart()
    {
       // thisLot = FindAnyObjectByType<WorldLot>();
    }

    [SerializeField]
    protected List<InteractionSO> interactionSOs = new();
    public List<InteractionSO> InteractionSOs { get { return interactionSOs; } }


    protected List<StoredInteraction> storedInteractions = new();
    public List<StoredInteraction> StoredInteractions { get { return storedInteractions; } }


    public void NewStoredInteraction(StoredInteraction sInteraction)
    {
        storedInteractions.Add(sInteraction);
    }
    public void ChangeCurrentLot(WorldLot lot)
    {
        thisLot = lot;
    }

    //protected void GenerateStoredInteractions()
    //{
    //    foreach(InteractionSO itso in interactionSOs)
    //    {
    //        allInteractions.Add(new StoredInteraction(itso, this));
    //    }
    //}
}
