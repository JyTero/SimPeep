using System.Collections.Generic;
using UnityEngine;

public class ManagementCore : ManagerMono
{
    [SerializeField, Tooltip("Time, in seconds, between this system running")]
    protected float updateInterval;
    protected float timeSinceLastUdate;

    protected float deltaTime;



    protected override void Start()
    {
        base.Start();
    }

    protected bool TooEarlyForNextTick(float tickRate)
    {
        if (timeSinceLastUdate < tickRate)
            return true;
        else
        {
            timeSinceLastUdate -= tickRate;
            return false;
        }
    }

    public void MyUpdate(float dt)
    {
        TimedUpdate(dt);
    }

    protected virtual void TimedUpdate(float dt)
    {
        deltaTime = dt;
    }

    protected void RemoveListItemsFromAnotherList<T>(List<T> itemsToRemove, List<T> targetList)
    {
        foreach (T item in itemsToRemove)
        {
            targetList.Remove(item);
        }
    }
}
