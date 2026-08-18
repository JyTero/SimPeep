using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterAI
{
    public Character chara;
    public CharacterAIState aiState;

    public float hasBeenIdleForTimer = 0;

    private ActiveInteraction currentInteraction;
    public ActiveInteraction CurrentInteraction {  get { return currentInteraction; } }

    private Dictionary<InteractionQueuePriority, List<QueuedInteraction>> interactionQueuesByPriority = new();
    public Dictionary<InteractionQueuePriority, List<QueuedInteraction>> InteractionQueuesByPriority { get { return interactionQueuesByPriority; } }

    private List<QueuedInteraction> interactionQueue = new();
    public List<QueuedInteraction> InteractionQueue { get { return interactionQueue; } }

    private List<ActiveInteraction> subInteractionQueue = new();
    public List<ActiveInteraction> SubInteractionQueue { get { return subInteractionQueue; } }

    public CharacterAI()
    {
        interactionQueuesByPriority.Add(InteractionQueuePriority.UrgentReaction, new List<QueuedInteraction>());
        interactionQueuesByPriority.Add(InteractionQueuePriority.AINeedFixing, new List<QueuedInteraction>());
        interactionQueuesByPriority.Add(InteractionQueuePriority.UserSelectNPCReaction, new List<QueuedInteraction>());
        interactionQueuesByPriority.Add(InteractionQueuePriority.UserSelect, new List<QueuedInteraction>());
        interactionQueuesByPriority.Add(InteractionQueuePriority.SuggestedFollowup, new List<QueuedInteraction>());
        interactionQueuesByPriority.Add(InteractionQueuePriority.NormalAISelect, new List<QueuedInteraction>());
    }

    public void NewCurrentInteraction(ActiveInteraction interaction)
    {
        currentInteraction = interaction;
    }

    public void QueueNewInteraction(ActiveInteraction interaction, InteractionQueuePriority queuePriority)
    {
        if (interactionQueuesByPriority.ContainsKey(queuePriority))
            interactionQueuesByPriority[queuePriority].Add(new QueuedInteraction(interaction, queuePriority));
        else
            interactionQueuesByPriority.Add(queuePriority, new() { new QueuedInteraction(interaction, queuePriority) });

        UpdateSimpleQueue();
    }

    private void UpdateSimpleQueue()
    {
        interactionQueue.Clear();
        foreach (InteractionQueuePriority iqp in (InteractionQueuePriority[])Enum.GetValues(typeof(InteractionQueuePriority)))
        {
            if (!InteractionQueuesByPriority.ContainsKey(iqp))
                continue;
            else
            {
                foreach (QueuedInteraction queInteraction in InteractionQueuesByPriority[iqp])
                {
                    interactionQueue.Add(queInteraction);
                }
            }
        }
    }

    public void ClearQueuePart(InteractionQueuePriority quePrio)
    {
        interactionQueuesByPriority.Remove(quePrio);
    }

    public CharacterAI(Character c)
    {
        chara = c;
        aiState = CharacterAIState.Idle;
    }

    public void AddSubInteration(ActiveInteraction sis)
    {
        subInteractionQueue.Add(sis);
    }

    public void RemoveSubInteraction(ActiveInteraction si)
    {
        subInteractionQueue.Remove(si);
    }

}
public enum CharacterAIState
{
    Default,
    Idle,
    Active,
    occupied,
}
