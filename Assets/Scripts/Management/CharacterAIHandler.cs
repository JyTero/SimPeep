using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.TextCore.Text;

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

    private Dictionary<Character, CharacterAI> charactersAIsByCharacter = new();

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
            CharacterAI character = idleCharacters[i];
            //HasQueued Interactions
            ActiveInteraction queueInteraction = HandleQueue(character);
            if (queueInteraction == null)
                continue;
            else
                StartInteraction(queueInteraction, character);
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

            QueueInteraction(interactions[i], InteractionQueuePriority.NormalAISelect);
            tasklessCharacters.Remove(characterAI);
            idleCharacters.Add(characterAI);
            //StartInteraction(interactions[i], characterAI);
        }
    }

    private ActiveInteraction HandleQueue(CharacterAI character)
    {

        if (character.InteractionQueuesByPriority.Count == 0)
            return null;

        string S = $"Iterating: {character.chara.ItemName}\n";
        // Source - https://stackoverflow.com/a/105402
        // Posted by jop, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-07-30, License - CC BY-SA 4.0
        foreach (InteractionQueuePriority iqp in (InteractionQueuePriority[])Enum.GetValues(typeof(InteractionQueuePriority)))
        {
            S += $"Queue type {iqp.ToString()}\n";
            if (!character.InteractionQueuesByPriority.ContainsKey(iqp))
                continue;
            else
            {
                QueuedInteraction queueInteraction = character.InteractionQueuesByPriority[iqp][0];

                idleCharacters.Remove(character);
                //StartInteraction(queueInteraction.interaction, character);
                character.InteractionQueuesByPriority[iqp].Remove(queueInteraction);
                if (character.InteractionQueuesByPriority[iqp].Count == 0)
                    character.ClearQueuePart(iqp);
                //TODO: QUEUE CLEANUP (REMOVE DONE ACTION,
                if (IsDebug)
                    Debug.Log(S);
                return queueInteraction.interaction;
            }
        }
        if (IsDebug)
            Debug.Log(S);
        return null;
    }

    public void QueueInteraction(ActiveInteraction interaction, InteractionQueuePriority queuePriority)
    {
        //Handle queueing interaction prio / source 

        charactersAIsByCharacter[interaction.ThisCharacter].QueueNewInteraction(interaction, queuePriority);

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

    public void OnInteractionEnd(Character character, ActiveInteraction interaction)
    {
        CharacterAI charaAI = activeCharacters[character];
        activeCharacters.Remove(character);
        idleCharacters.Add(charaAI);

        if (interaction.FollowupInteractionSO != null)
        {
            //Find nearby interaction of that type
            List<StoredInteraction> storedInteractions = lotManager.GetAllInteractionsOnLot(charaAI.chara.ThisLot);
            foreach (StoredInteraction stoIn in storedInteractions)
            {
                if (stoIn.InteractionTuningSO == interaction.FollowupInteractionSO)
                    charaAI.QueueNewInteraction(new ActiveInteraction(character, stoIn), InteractionQueuePriority.SuggestedFollowup);
            }
        }

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
    public enum InteractionQueuePriority
    {
        UrgentReaction, //Fire, emergency
        AINeedFixing,
        SuggestedFollowup,
        UserSelectNPCReaction,
        UserSelect,
        NormalAISelect,
    }
    private class CharacterAI
    {
        public Character chara;
        public CharacterAIState aiState;

        public float hasBeenIdleForTimer = 0;

        //private List<QueuedInteraction> interactionQueue = new();
        //public List<QueuedInteraction> InteractionQueue { get { return interactionQueue; } }

        private Dictionary<InteractionQueuePriority, List<QueuedInteraction>> interactionQueuesByPriority = new();
        public Dictionary<InteractionQueuePriority, List<QueuedInteraction>> InteractionQueuesByPriority { get { return interactionQueuesByPriority; } }

        public CharacterAI()
        {
            interactionQueuesByPriority.Add(InteractionQueuePriority.UrgentReaction, new List<QueuedInteraction>());
            interactionQueuesByPriority.Add(InteractionQueuePriority.AINeedFixing, new List<QueuedInteraction>());
            interactionQueuesByPriority.Add(InteractionQueuePriority.UserSelectNPCReaction, new List<QueuedInteraction>());
            interactionQueuesByPriority.Add(InteractionQueuePriority.UserSelect, new List<QueuedInteraction>());
            interactionQueuesByPriority.Add(InteractionQueuePriority.SuggestedFollowup, new List<QueuedInteraction>());
            interactionQueuesByPriority.Add(InteractionQueuePriority.NormalAISelect, new List<QueuedInteraction>());
        }

        public void QueueNewInteraction(ActiveInteraction interaction, InteractionQueuePriority queuePriority)
        {
            if (interactionQueuesByPriority.ContainsKey(queuePriority))
                interactionQueuesByPriority[queuePriority].Add(new QueuedInteraction(interaction, queuePriority));
            else
                interactionQueuesByPriority.Add(queuePriority, new() { new QueuedInteraction(interaction, queuePriority) });
        }

        public void ClearQueuePart(InteractionQueuePriority quePrio)
        {
            interactionQueuesByPriority.Remove(quePrio);
        }
        //public void QueueToFirst(ActiveInteraction interaction, InteractionQueuePriority queueSource)
        //{
        //    interactionQueue.Insert(0, new QueuedInteraction(interaction, queueSource));
        //}
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

        private InteractionQueuePriority queuePriority;
        public InteractionQueuePriority QueuePriority { get { return queuePriority; } }


        public QueuedInteraction(ActiveInteraction interaction, InteractionQueuePriority queuePrio)
        {
            this.interaction = interaction;
            this.queuePriority = queuePrio;
        }
    }
}
