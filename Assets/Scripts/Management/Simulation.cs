using UnityEngine;

public class Simulation : MonoBehaviour
{
    //[SerializeField]
    private int timeScale;
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
        SetSimulationTimeScale(0);
    }

    private void Update()
    {
        float dt = Time.deltaTime * timeScale;
        characterAIHandler.MyUpdate(dt);
        characterRouting.MyUpdate(dt);
        interactionEngine.MyUpdate(dt);
        needsEngine.MyUpdate(dt);

    }

    public void SetSimulationTimeScale(int speed)
    {
        timeScale = speed;
    }
}
