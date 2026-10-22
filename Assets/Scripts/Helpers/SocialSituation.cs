using System;
using UnityEngine;

public class SocialSituation
{
    private Character initiator;
    public Character Initiator { get { return initiator; } }

    private Character responder;
    public Character Responder { get { return responder; } }

    private bool initiatorReady;
    public bool InitiatorReady { get { return initiatorReady; } }

    private bool responderReady;
    public bool ResponderReady { get { return responderReady; } }

    public event Action<Character> OnResponderReady;
    public event Action<Character> OnInitiatorReady;

    public SocialSituation(Character initiator, Character responder)
    {
        this.initiator = initiator;
        this.responder = responder;

        this.responderReady = false;
        this.initiatorReady = false;
    }

    public void InitiatorIsReady()
    {
        initiatorReady = true;
    }
    public void ResponderIsReady()
    {
        responderReady = true;
        OnResponderReady.Invoke(responder);

    }

    public bool BothReady()
    {
        if (!initiatorReady || !responderReady)
            return false;
        else
            return true;
    }
}
