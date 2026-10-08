using UnityEngine;

public class WaitingCharacter
{
    private Character character;
    public Character Character { get { return character; } }

    private EWaitReason waitReason;
    public EWaitReason WaitReason { get { return waitReason; } }

    private Interactable waitTarget;
    public Interactable WaitTarget { get { return waitTarget; } }


    public WaitingCharacter(Character waitingCharacter, EWaitReason reason, Interactable target)
    {
        this.character = waitingCharacter;
        this.waitReason = reason;
        this.waitTarget = target;
    }
    }
