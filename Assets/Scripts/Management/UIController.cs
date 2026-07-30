using NUnit.Framework;
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
            characterAIHandler.QueueInteraction(new ActiveInteraction(SelectedCharacter, storedInteraction), CharacterAIHandler.InteractionQueueSource.UserSelect);

        }
    }

    public void ChangeSelectCharacter(Character character)
    {
        selectedCharacter = character;
        selectCharacterName.text = character.ItemName;

        needsUIPanel.OnSelectCharacterChange();
        relationshipsUIPanel.OnSelectCharacterChange();

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


}
