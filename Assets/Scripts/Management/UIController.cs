using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIController : ManagementCore
{
    [SerializeField]
    private TextMeshProUGUI selectCharacterName;
    [SerializeField]
    private Button needsPanelButton;
    [SerializeField]
    private Button relationshipsPanelButton;

    [SerializeField]
    private GameObject SelectionParent;

    [SerializeField]
    private InteractionQueue_UIPanel interactionQueue_UIPanel;

    [SerializeField]
    private InteractionStates_UIPanel interactionStates_UIPanel;

    [SerializeField]
    private GameObject buttonPrefab;
    private List<GameObject> buttonPool = new();
    private List<GameObject> activeButtons = new();

    private Needs_UIPanel needsUIPanel;
    private Relationships_UIPanel relationshipsUIPanel;


    //Should probably be moved to a more global/general manager
    protected Character selectedCharacter;
    public Character SelectedCharacter { get { return selectedCharacter; } }

    protected override void Start()
    {
        base.Start();
        needsUIPanel = FindAnyObjectByType<Needs_UIPanel>();
        relationshipsUIPanel = FindAnyObjectByType<Relationships_UIPanel>();

        needsPanelButton.onClick.AddListener(delegate { NeedsPanelButtonClick(); });
        relationshipsPanelButton.onClick.AddListener(delegate { RelationshipsPanelButtonClick(); });

        interactionStates_UIPanel = FindAnyObjectByType<InteractionStates_UIPanel>();
    }

    public void InitialiseUI()
    {
        needsUIPanel.DisablePanel();
        relationshipsUIPanel.DisablePanel();
    }

    public void ShowListOfInteractions(List<StoredInteraction> storedInteractions)
    {
        for (int j = activeButtons.Count - 1; j >= 0; j--)
        {
            GameObject button = activeButtons[j];
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.SetActive(false);
            buttonPool.Add(button);
            activeButtons.RemoveAt(j);
        }
        //Confirm pool
        if (buttonPool.Count < storedInteractions.Count)
            MakeButtonObjects(storedInteractions.Count - buttonPool.Count);

        //Populate
        int i = 0;
        foreach (StoredInteraction storedInteraction in storedInteractions)
        {
            if (storedInteraction.InteractionTuningSO.HiddenInteraction)
                continue;
            if (storedInteraction.InvalidInteraction)
                continue;

            GameObject buttonGO = buttonPool[i];

            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = storedInteraction.InteractionTuningSO.InteractionName;
            buttonGO.SetActive(true);
            buttonGO.GetComponent<Button>().onClick.AddListener(delegate { InteractionSelectButtonPress(storedInteraction); });
            buttonPool.Remove(buttonGO);
            activeButtons.Add(buttonGO);
            //i++;
        }
    }

    private void MakeButtonObjects(int amount)
    {
        while (amount > 0)
        {
            GameObject go = Instantiate(buttonPrefab, SelectionParent.transform);
            go.SetActive(false);
            buttonPool.Add(go);
            amount--;
        }
    }

    private void InteractionSelectButtonPress(StoredInteraction storedInteraction)
    {
        if (SelectedCharacter)
        {
            //Debug.Log($"Chose interaction " + storedInteraction.InteractionTuningSO.InteractionName);
            characterAIHandler.QueueInteraction(NewActiveInteraction(SelectedCharacter, storedInteraction), InteractionQueuePriority.UserSelect);

        }
    }

    public void ChangeSelectCharacter(Character character)
    {
        selectedCharacter = character;
        selectCharacterName.text = character.ItemName;

        needsUIPanel.OnSelectCharacterChange();
        relationshipsUIPanel.OnSelectCharacterChange();


    }

    public CharacterAI GetCharacterAIByCharacter(Character character)
    {
        return characterAIHandler.CharactersAIsByCharacter[character];
    }


    public void NeedsPanelButtonClick()
    {
        needsUIPanel.gameObject.SetActive(true);
        needsUIPanel.ActivatePanel();

        //Disable rest (Make general)
        relationshipsUIPanel.gameObject.SetActive(false);
    }
    public void RelationshipsPanelButtonClick()
    {

        relationshipsUIPanel.gameObject.SetActive(true);
        relationshipsUIPanel.ActivatePanel();

        needsUIPanel.DisablePanel();
    }

    //INTERACTIONQUEUE
    public void RefreshInteractionQueueData(CharacterAI chara)
    {
        List<string> interactionQueue = MakeQueuedInteractionNamesList(chara);
        interactionQueue_UIPanel.RefreshQueueData(interactionQueue);
    }
    private List<string> MakeQueuedInteractionNamesList(CharacterAI chara)
    {
        //UI (/DEBUG)
        List<string> qi = new();
        foreach (InteractionQueuePriority iqp in (InteractionQueuePriority[])Enum.GetValues(typeof(InteractionQueuePriority)))
        {
            if (!chara.InteractionQueuesByPriority.ContainsKey(iqp))
                continue;
            else
            {
                foreach (QueuedInteraction queInteraction in chara.InteractionQueuesByPriority[iqp])
                {
                    qi.Add(queInteraction.interaction.InteractionName);
                }
            }
        }

        return qi;

    }

    public void RefreshCurrentInteractionData(string currentInteraction)
    {
        interactionQueue_UIPanel.RefreshCurrentInteractionData(currentInteraction);
    }


    //DEBUG
    //InteractionStateQueue
    public void RefreshInteractionStateData(ActiveInteraction interaction)
    {

        List<string> queuedStates = MakeQueuedInteractionStatesList(interaction);
        interactionStates_UIPanel.RefreshQueueData(queuedStates);
        interactionStates_UIPanel.RefreshCurrentInteractionData(interaction.State.thisState.ToString());
    }

    private List<string> MakeQueuedInteractionStatesList(ActiveInteraction interaction)
    {
        //UI (/DEBUG)
        List<string> queuedStates = new();
        foreach(ActiveInteractionState ais in interaction.previousInteractionStates)
            queuedStates.Add(ais.thisState.ToString());

        return queuedStates;

    }
}
