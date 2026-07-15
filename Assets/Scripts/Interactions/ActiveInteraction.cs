using System.Collections;
using UnityEngine;

public class ActiveInteraction
{
    [SerializeField]
    private InteractionSO interactionTuningSO;
    public InteractionSO InteractionTuningSO { get { return interactionTuningSO; } }

    private string interactionName;
    public string InteractionName { get { return interactionName; } }

    private Interactable interactionSource;
    public Interactable InteractionSource { get { return interactionSource; } }

    private Character thisCharacter;
    public Character ThisCharacter { get { return thisCharacter; } }

    private float interactionLength;
    public float InteractionLength { get { return interactionLength; } }

    //RuntimeData
    public float interactionLenghtAccumulation;
    public InteractionState interactionState;

    public float interactionScore;

    public ActiveInteraction(Character chara, InteractionSO itSO, Interactable interactable)
    {
        interactionTuningSO = itSO;
        interactionName = itSO.InteractionName;
        interactionSource = interactable;
        thisCharacter = chara;
        interactionLength = itSO.InteractionLenght;
        interactionLenghtAccumulation = 0;
        interactionState = InteractionState.Default;
        interactionScore = 0;
    }

    
}

public class StoredInteraction
{
    private InteractionSO interactionTuningSO;
    public InteractionSO InteractionTuningSO { get { return interactionTuningSO; } }

    private Interactable interactionSource;
    public Interactable InteractionSource { get { return interactionSource; } }

    public StoredInteraction (InteractionSO interactionTuningSO, Interactable interactionSource)
    {
        this.interactionTuningSO = interactionTuningSO;
        this.interactionSource = interactionSource;
    }
}

public enum InteractionState
{
    Default,
    Starting,
    Moving,
    AtDestination,
    Running,
    Ending,
}
