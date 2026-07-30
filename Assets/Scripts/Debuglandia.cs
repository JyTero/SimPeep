using UnityEngine;

public class Debuglandia : MonoBehaviour
{
    protected CharacterAIHandler characterAIHandler;
    protected CharacterRouting characterRouting;
    protected InteractionEngine interactionEngine;
    protected NeedsEngine needsEngine;
    protected LotManager lotManager;
    protected UIController UIController;
    protected CharacterRelationshipsManager relationshipsManager;

    protected Simulation simulation;

    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterRouting>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
        UIController = GetComponent<UIController>();
        relationshipsManager = GetComponent<CharacterRelationshipsManager>();

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
}
