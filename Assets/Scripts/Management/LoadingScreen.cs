using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class LoadingScreen : MonoBehaviour
{
    private bool runStartup = false;

    protected CharacterAIHandler characterAIHandler;
    protected CharacterRouting characterRouting;
    protected InteractionEngine interactionEngine;
    protected NeedsEngine needsEngine;
    protected LotManager lotManager;

    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterRouting>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!runStartup)
        {
            FindAnyObjectByType<Needs_UIPanel>().Debuglandia();

            Character[] characters = FindObjectsByType<Character>();
            foreach (Character chara in characters)
            {
                characterAIHandler.AddNewCharacter(chara);
                foreach (InteractionSO iso in chara.InteractionSOs)
                {
                    chara.NewStoredInteraction(new StoredInteraction(iso, chara));

                }
            }

            FindAnyObjectByType<Simulation>().SetSimulationTimeScale(1);
            gameObject.GetComponent<LoadingScreen>().enabled = false;


            foreach (WorldLot lot in lotManager.AllLots)
            {
                foreach (ItemBase item in lot.ItemsOnLot)
                {
                    foreach (InteractionSO iso in item.InteractionSOs)
                    {
                        item.NewStoredInteraction(new StoredInteraction(iso, item));
                    }
                }
            }

        }
    }
}
