using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreenCover;

    private bool runStartup = false;

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

    // Update is called once per frame
    void Update()
    {
        if (!runStartup)
        {
            Character[] characters = FindObjectsByType<Character>();
            foreach (Character chara in characters)
            {
                NewCharacter(chara);

            }
            //Lots & Items
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

            //UI


            FindAnyObjectByType<Simulation>().SetSimulationTimeScale(1);
            loadingScreenCover.gameObject.SetActive(false);

            gameObject.GetComponent<LoadingScreen>().enabled = false;
        }
    }


    private void NewCharacter(Character chara)
    {
        //CharacterAI / Interactions
        characterAIHandler.AddNewCharacter(chara);
        foreach (InteractionSO iso in chara.InteractionSOs)
        {
            chara.NewStoredInteraction(new StoredInteraction(iso, chara));

        }
        

            //Relations
            relationshipsManager.NewCharacter(chara);

    }
}
