using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionQueue_UIPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI currentInteractionField;
    [SerializeField]
    private TextMeshProUGUI interactionQueueField;
    [SerializeField]
    private Button cancelInteractionButton;

    private List<string> queuedInteractions = new();
    private string currentInteraction;
    private bool interactionCancellable = false;

    protected override void Start()
    {
        base.Start();
        ActivatePanel();
        RefreshPanel();

    }

    protected override void OSCC()
    {
        RefreshPanelData();
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

        cancelInteractionButton.gameObject.SetActive(interactionCancellable);
    }

    public void RefreshQueueData(List<string> interactionQueue)
    {
        queuedInteractions = interactionQueue;
        RefreshPanel();
    }
    public void RefreshCurrentInteractionData(string currentInteraction, ActiveInteraction interaction)
    {
        this.currentInteraction = currentInteraction;

        if (interaction.InteractionData.InteractionEndingType == InteractionEndingType.UntillNeedAtValue)
            interactionCancellable = true;
        else
            interactionCancellable = false;

        RefreshPanel();
    }
    private void RefreshPanelData()
    {
        if (uiController.SelectedCharacter.CharacterAI.CurrentInteraction != null)
            currentInteraction = uiController.SelectedCharacter.CharacterAI.CurrentInteraction.InteractionName;
        else
            currentInteraction = "";
        uiController.RefreshInteractionQueueData(uiController.SelectedCharacter.CharacterAI);

    }

    public void CancelInteractionButtonPress()
    {
        uiController.InteractionCancelButtonPress();
    }

    public override void DisablePanel()
    {
        base.DisablePanel();
    }
}
