using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : ManagementCore
{
    [SerializeField]
    private TextMeshProUGUI selectCharacterName;
    private Needs_UIPanel needUIPanel;

    [SerializeField]
    private GameObject SelectionParent;

    [SerializeField]
    private GameObject buttonPrefab;
    private List<GameObject> buttonPool = new();
    private List<GameObject> activeButtons = new();

    //Should probably be moved to a more global/general manager
    protected Character selectedCharacter;
    public Character SelectedCharacter { get { return selectedCharacter; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        base.Start();
        needUIPanel = FindAnyObjectByType<Needs_UIPanel>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowListOfInteractions(List<StoredInteraction> storedInteractions)
    {
        //Clear old
        foreach (GameObject button in activeButtons)
        {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.SetActive(false);
        }
        //Confirm pool
        if (!(buttonPool.Count >= storedInteractions.Count))
            MakeButtonObjects(storedInteractions.Count - buttonPool.Count);

        //Populate
        int i = 0;
        foreach (StoredInteraction storedInteraction in storedInteractions)
        {
            GameObject buttonGO = buttonPool[i];
            activeButtons.Add(buttonGO);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = storedInteraction.InteractionTuningSO.InteractionName;
            buttonGO.SetActive(true);
            buttonGO.GetComponent<Button>().onClick.AddListener(delegate { InteractionSelectButtonPress(storedInteraction); });
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
            characterAIHandler.QueueInteraction(new ActiveInteraction(SelectedCharacter, storedInteraction));

        }
    }


    public void ChantgeUICharacterInfo(Character character)
    {
        selectedCharacter = character;
        needUIPanel.OnChangeSelectedCharacter();
        selectCharacterName.text = character.ItemName;
    }
}
