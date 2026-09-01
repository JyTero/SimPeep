using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Debuglandia : MonoBehaviour
{
    protected CharacterAIHandler characterAIHandler;
    protected CharacterControl characterRouting;
    protected InteractionEngine interactionEngine;
    protected NeedsEngine needsEngine;
    protected LotManager lotManager;
    protected UIController UIController;
    protected CharacterRelationshipEngine relationshipsManager;

    protected Simulation simulation;


    private List<Slot> slots = new();

    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterControl>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
        UIController = GetComponent<UIController>();
        relationshipsManager = GetComponent<CharacterRelationshipEngine>();

        simulation = FindAnyObjectByType<Simulation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            relationshipsManager.PrintRelations();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
        }


        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            simulation.SetSimulationTimeScale(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            simulation.SetSimulationTimeScale(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            simulation.SetSimulationTimeScale(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            simulation.SetSimulationTimeScale(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            simulation.SetSimulationTimeScale(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            simulation.SetSimulationTimeScale(20);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            simulation.SetSimulationTimeScale(100);
        }

    }

    //private void OnDrawGizmos()
    //{

    //}

    //public void GetAllSlots()
    //{
    //    List<ItemBase> items = lotManager.GetAllItemsOnLot(FindAnyObjectByType<WorldLot>());
    //    foreach (ItemBase item in items)
    //    {
    //        if (item.ItemSlotsOnItem.Count == 0)
    //            continue;
    //        else
    //        {
    //            foreach (Item_Slot slot in item.ItemSlotsOnItem)
    //            {
    //                slots.Add(slot);
    //            }
    //        }
    //    }
    //}
    //public void SlotVisualiser()
    //{
    //    Gizmos.color = Color.gray;
    //    foreach (Slot slot in slots)
    //    {
    //        Gizmos.DrawWireSphere(transform.position, 0.15f);

    //    }

    //    //// Optional: show orientation
    //    //Gizmos.DrawLine(
    //    //    transform.position,
    //    //    transform.position + transform.forward * 0.4f
    //    //);
    //}
}
