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
        LateStartTimer();
    }
    private IEnumerator LateStartTimer()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        LateStart();
    }
    public void LateStart()
    {
        thisLot = FindAnyObjectByType<WorldLot>();
    }

    [SerializeField]
    protected List<InteractionSO> interactionSOs = new();
    public List<InteractionSO> InteractionSOs { get { return interactionSOs; } }


    protected List<StoredInteraction> allInteractions = new();
    public List<StoredInteraction> AllInteractions { get { return allInteractions; } }

    public void NewStoredInteraction(StoredInteraction sInteraction)
    {
        allInteractions.Add(sInteraction);
    }
}
