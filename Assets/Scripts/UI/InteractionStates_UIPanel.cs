using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InteractionStates_UIPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI currentStateField;
    [SerializeField]
    private TextMeshProUGUI stateQueueField;

    private List<string> queuedInteractions = new();
    private string currentState;

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
        currentStateField.text = "--> " + currentState;
        string s = "";
        foreach (string interaction in queuedInteractions)
        {
            s += interaction + "\n";
        }
        stateQueueField.text = s;
    }

    public void RefreshQueueData(List<string> stateStack)
    {
        queuedInteractions = stateStack;
        RefreshPanel();
    }
    public void RefreshCurrentInteractionData(string currentState)
    {
        this.currentState = currentState;
        RefreshPanel();
    }
    private void RefreshPanelData()
    {
        //if (uiController.SelectedCharacter.CharacterAI.CurrentInteraction != null)
        //    currentState = uiController.SelectedCharacter.CharacterAI.CurrentInteraction.State.ToString();
        //else
        //    currentState = "";
        uiController.RefreshInteractionStateData(uiController.SelectedCharacter.CharacterAI.CurrentInteraction, uiController.SelectedCharacter);
    }

    public override void DisablePanel()
    {
        base.DisablePanel();
    }
}
