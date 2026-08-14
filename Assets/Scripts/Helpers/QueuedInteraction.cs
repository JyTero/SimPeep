using UnityEngine;

//InteractionInScoring (Lighter than ActiveInteraction, to be used during scoring (Lot of items generated, should probs be lighter))
public class QueuedInteraction
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
