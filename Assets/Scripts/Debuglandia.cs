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

    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterRouting>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
        UIController = GetComponent<UIController>();
        relationshipsManager = GetComponent<CharacterRelationshipsManager>();
    }

    //Do things that a need to be done after everything is ready but before play
    private void LateStart()
    {
        FindAnyObjectByType<Needs_UIPanel>().Debuglandia();
        
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
    }
}
