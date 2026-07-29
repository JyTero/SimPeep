using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

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

    private Dictionary<Character, CharacterAI> charactersAIsByCharacter= new();

    private List<Character> charactersToRemove = new();

    private Debuglandia debuglandia;


    private void Start()
    {
        base.Start();
        debuglandia = FindAnyObjectByType<Debuglandia>();
    }

    public void AddNewCharacter(Character character)
    {
        CharacterAI cai = new CharacterAI(character);
        idleCharacters.Add(cai);
        charactersAIsByCharacter.Add(character, cai);

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
            CharacterAI idleCharacter = idleCharacters[i];
            //HasQueued Interactions
            if (idleCharacter.InteractionQueue.Count > 0)
            {
                idleCharacters.RemoveAt(i);
                StartInteraction(idleCharacter.InteractionQueue[0].interaction, idleCharacter);
                idleCharacter.InteractionQueue.RemoveAt(0);
                continue;
            }

            idleCharacter.hasBeenIdleForTimer += deltaTime + updateInterval;
            if (idleCharacter.hasBeenIdleForTimer > characterAIIdleTimer)
            {
                //Add check/logic to not find another interaction if queue exists

                idleCharacter.hasBeenIdleForTimer -= characterAIIdleTimer;
                tasklessCharacters.Add(idleCharacter);
                idleCharacters.RemoveAt(i);
            }
        }

        //SearchInteraction
        for (int i = tasklessCharacters.Count - 1; i >= 0; i--)
        {
            //Gather
            CharacterAI characterAI = tasklessCharacters[i];
            List<StoredInteraction> storedInteractions = lotManager.GetAllInteractionsOnLot(characterAI.chara.ThisLot);

            //Convert
            List<ActiveInteraction> interactions = new();
            foreach (StoredInteraction storedInteraction in storedInteractions)
            {
                //TODO: Make "StoredInteraction" that holds interactionSO and item and is given here, instead of passing SOs.
                interactions.Add(new ActiveInteraction(tasklessCharacters[i].chara, storedInteraction.InteractionTuningSO, storedInteraction.InteractionSource));
            }
            //Validity
            //Score
            foreach (ActiveInteraction interaction in interactions)
            {
                float score = 0;
                float needBonus = 0;
                Character thisCharacter = interaction.ThisCharacter;
                //NeedScoring
                foreach (NeedType needTypeToWeight in interaction.NeedsToWeight)
                {
                    float needPercentage = (float)thisCharacter.Needs[needTypeToWeight].NeedValue / 100f;
                    float needBasedMultiplier = thisCharacter.Needs[needTypeToWeight].NeedWeightOnInteractionScoring.Evaluate(needPercentage);
                    needBonus += interactionBaseScore * needBasedMultiplier;

                }
                score = needBonus;
                //Trait scoring
                float traitBonus = 0;
                foreach (InteractionScoringModifier modifier in interaction.ScoringModifiers)
                {
                    if (characterAI.chara.Traits.Contains(modifier.TraitSO))
                    {
                        traitBonus += modifier.TraitBonus;
                    }
                }
                score += traitBonus;

                interaction.interactionScore = score;
            }
            if (IsDebug)
                PrintInteractionScoring(interactions);

            //Pick best (/Random)
            //interactions = SortScoredInteractions(interactions);
            interactions.Sort((a, b) => b.interactionScore.CompareTo(a.interactionScore));

            QueueInteraction(interactions[i], InteractionQueueSource.NormalAISelect);
            tasklessCharacters.Remove(characterAI);
            idleCharacters.Add(characterAI);
            //StartInteraction(interactions[i], characterAI);
        }
    }

    public void QueueInteraction(ActiveInteraction interaction, InteractionQueueSource queueSource)
    {
        charactersAIsByCharacter[interaction.ThisCharacter].QueueNewInteraction(interaction, queueSource);
    }

    private void StartInteraction(ActiveInteraction interaction, CharacterAI charaAI)
    {
        interactionEngine.StartNewInteraction(interaction);
        activeCharacters.Add(charaAI.chara, charaAI);
    }

    private void PrintInteractionScoring(List<ActiveInteraction> interactions)
    {
        string s = $"{interactions[0].ThisCharacter.ItemName}'s Scoring Result:\n";
        foreach (ActiveInteraction interaction in interactions)
        {
            s += $"Interaction '{interaction.InteractionName}' (Item: {interaction.InteractionSource.ItemName} scored: {interaction.interactionScore})\n";
        }
        Debug.Log(s);
    }
    private List<ActiveInteraction> SortScoredInteractions(List<ActiveInteraction> interactions)
    {
        List<ActiveInteraction> sortedInteractions = interactions.OrderBy(i => i.interactionScore).ToList();
        return sortedInteractions;
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
    public enum InteractionQueueSource
    {
        Default,
        NormalAISelect,
        UserSelect,
        UserSelectNPCReaction,
        UrgentReaction,
    }
    private class CharacterAI
    {
        public Character chara;
        public CharacterAIState aiState;

        public float hasBeenIdleForTimer = 0;

        private List<QueuedInteraction> interactionQueue = new();
        public List<QueuedInteraction> InteractionQueue { get { return interactionQueue; } }

        public void QueueNewInteraction(ActiveInteraction interaction, InteractionQueueSource queueSource)
        {
            //TODO: Handle queueSource to prioritise interactions
            interactionQueue.Add(new QueuedInteraction(interaction, queueSource));
        }
        public CharacterAI(Character c)
        {
            chara = c;
            aiState = CharacterAIState.Idle;
        }

    }
    //InteractionInScoring (Lighter than ActiveInteraction, to be used during scoring (Lot of items generated, should probs be lighter))
    private class QueuedInteraction
    {
        public ActiveInteraction interaction;

        private InteractionQueueSource queueSource;
        public InteractionQueueSource QueueSource { get { return queueSource; } }


        public QueuedInteraction(ActiveInteraction interaction, InteractionQueueSource queueSource)
        {
            this.interaction = interaction;
            this.queueSource = queueSource;
        }
    }
}
