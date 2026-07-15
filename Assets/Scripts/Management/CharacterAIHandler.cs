using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIHandler : ManagementCore
{

    [SerializeField]
    private int characterAIIdleTimer; //This will change to be character dependant
    [SerializeField, Tooltip("Temp. replacement for interaction spesific base scores")]
    private int interactionBaseScore;

    private List<CharacterAI> idleCharacters = new();
    private List<CharacterAI> tasklessCharacters = new();
    private Dictionary<Character, CharacterAI> activeCharacters = new();
    private Dictionary<Character, CharacterAI> occupiedCharacters = new();

    private List<Character> charactersToRemove = new();

    private Debuglandia debuglandia;


    private void Start()
    {
        base.Start();
        debuglandia = FindAnyObjectByType<Debuglandia>();
    }

    public void AddNewCharacter(Character character)
    {
        idleCharacters.Add(new CharacterAI(character));

    }


    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);

        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;


        RunDecisionMaking(deltaTime);
    }

    private void RunDecisionMaking(float deltaTime)
    {
        //Idle
        for (int i = idleCharacters.Count - 1; i >= 0; i--)
        {
            idleCharacters[i].hasBeenIdleForTimer += deltaTime + updateInterval;
            if (idleCharacters[i].hasBeenIdleForTimer > characterAIIdleTimer)
            {
                idleCharacters[i].hasBeenIdleForTimer -= characterAIIdleTimer;
                tasklessCharacters.Add(idleCharacters[i]);
                idleCharacters.RemoveAt(i);
            }
        }

        //SearchInteraction
        for (int i = tasklessCharacters.Count - 1; i >= 0; i--)
        {
            //Gather
            CharacterAI characterAI = tasklessCharacters[i];
            List<StoredInteraction> interactionSOs = lotManager.GetAllInteractionsOnLot(characterAI.chara.ThisLot);

            //Convert
            List<ActiveInteraction> interactions = new();
            foreach (StoredInteraction storedInteraction in interactionSOs)
            {
                //TODO: Make "StoredInteraction" that holds interactionSO and item and is given here, instead of passing SOs.
                interactions.Add(new ActiveInteraction(tasklessCharacters[i].chara, storedInteraction.InteractionTuningSO, storedInteraction.InteractionSource));
            }
            //Validity
            //Score
            foreach(ActiveInteraction interaction in interactions)
            {
                int score = interactionBaseScore;

                //interaction.

            }
            //Pick best (/Random)
            int r = Random.Range(0, interactions.Count);
            StartInteraction(interactions[r], characterAI);
        }
    }


    private void StartInteraction(ActiveInteraction interaction, CharacterAI charaAI)
    {
        interactionEngine.StartNewInteraction(interaction);
        tasklessCharacters.Remove(charaAI);
        activeCharacters.Add(charaAI.chara, charaAI);
    }
    public void OnInteractionEnd(Character character)
    {
        CharacterAI charaAI = activeCharacters[character];
        activeCharacters.Remove(character);
        idleCharacters.Add(charaAI);
    }

    public void RemoveCharacter(Character character)
    {

    }

    private enum CharacterAIState
    {
        Default,
        Idle,
        Active,
        occupied,
    }
    private class CharacterAI
    {
        public Character chara;
        public CharacterAIState aiState;

        public float hasBeenIdleForTimer = 0;

        public CharacterAI(Character c)
        {
            chara = c;
            aiState = CharacterAIState.Idle;
        }

    }
}
