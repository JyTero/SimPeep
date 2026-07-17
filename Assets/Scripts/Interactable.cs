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
    private List<InteractionSO> allInteractions = new();
    public List<InteractionSO> AllInteractions { get { return allInteractions; } }
}
