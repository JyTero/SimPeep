using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionQueue_UIPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI currentInteractionField;
    [SerializeField]
    private TextMeshProUGUI interactionQueueField;

    private List<string> queuedInteractions = new();
    private string currentInteraction;

    protected override void Start()
    {
        base.Start();
        RefreshPanel();

    }

    protected override void OSCC()
    {
        ActivatePanel();
        // UpdatePanel();

    }

    public override void ActivatePanel()
    {
        base.ActivatePanel();
        RefreshPanel();

    }

    private void RefreshPanel()
    {
        currentInteractionField.text = "--> " + currentInteraction;
        string s = "";
        foreach (string interaction in queuedInteractions)
        {
            s += interaction + "\n";
        }
        interactionQueueField.text = s;
    }

    public void RefreshQueueData( List<string> interactionQueue)
    {
        queuedInteractions = interactionQueue;
        RefreshPanel();
    }
    public void RefreshCurrentInteractionData(string currentInteraction)
    {
        this.currentInteraction = currentInteraction;
        RefreshPanel();
    }

    public override void DisablePanel()
    {
        base.DisablePanel();
    }
}
