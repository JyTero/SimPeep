using UnityEngine;

public class Simulation : MonoBehaviour
{
    private CharacterAIHandler characterAIHandler;
    private CharacterRouting characterRouting;
    private InteractionEngine interactionEngine;
    private NeedsEngine needsEngine;

    private void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterRouting>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        characterAIHandler.MyUpdate(dt);
        characterRouting.MyUpdate(dt);
        interactionEngine.MyUpdate(dt);
        needsEngine.MyUpdate(dt);

    }
}
