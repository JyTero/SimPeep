using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreenCover;

    private bool runStartup = false;

    protected CharacterAIHandler characterAIHandler;
    protected CharacterControl characterRouting;
    protected InteractionEngine interactionEngine;
    protected NeedsEngine needsEngine;
    protected LotManager lotManager;
    protected UIController UIController;
    protected CharacterRelationshipEngine relationshipsManager;

    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterControl>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
        UIController = GetComponent<UIController>();
        relationshipsManager = GetComponent<CharacterRelationshipEngine>();
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
                    foreach (InteractionSO itso in item.InteractionSOs)
                    {
                        item.NewStoredInteraction(new StoredInteraction(itso, item));
                    }
                }
            }

            lotManager.LoadingScreen();

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
