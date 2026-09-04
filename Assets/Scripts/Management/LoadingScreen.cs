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
    protected Debuglandia debuglandia;


    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterRouting = FindAnyObjectByType<CharacterControl>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
        UIController = GetComponent<UIController>();
        relationshipsManager = GetComponent<CharacterRelationshipEngine>();
        debuglandia = GetComponent<Debuglandia>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!runStartup)
        {
            //Lots & Items
            lotManager.LoadingScreen();


            //Characters
            Character[] characters = FindObjectsByType<Character>();
            foreach (Character chara in characters)
            {
                NewCharacter(chara);

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

        chara.ChangeCurrentTile(lotManager.GetTileInteractableIsOn(chara));
        chara.transform.position = chara.CurrentTile.TilePos;

        //Relations
        relationshipsManager.NewCharacter(chara);

    }
}
