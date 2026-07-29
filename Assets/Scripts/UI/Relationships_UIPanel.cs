using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Relationships_UIPanel : UIPanel
{
    [SerializeField]
    private GameObject relationshipUIItemPrefab;

    private List<GameObject> activeRelationshipUIItemGOs = new();
    private List<GameObject> relationshipUIItemGOPool = new();

    private List<RelationshipUIItem> relationshipUIItems = new();

    //public override void OnSelectCharacterChange()
    //{
    //    base.OnSelectCharacterChange();
    //}
    protected override void OCSS()
    {
        ActivatePanel();
        // UpdatePanel();

    }

    public override void ActivatePanel()
    {
        base.ActivatePanel();

        List<CharacterRelationship> relationships = relationshipsManager.RelationshipsByCharacter[uiController.SelectedCharacter].Values.ToList();
        DisableAllActiveRelationshipUIItems();
        //Confirm pool
        if (!(relationshipUIItemGOPool.Count >= relationships.Count))
            MakeRelationshipObjectGOs(relationships.Count - relationshipUIItemGOPool.Count);

        //Populate
        int i = 0;
        foreach (CharacterRelationship relationship in relationships)
        {
            GameObject relationshipGO = relationshipUIItemGOPool[i];
            activeRelationshipUIItemGOs.Add(relationshipGO);
            relationshipGO.SetActive(true);
            relationshipGO.GetComponent<RelationshipUIItem>().Initialise(relationship, this);
            i++;
        }
    }

    private void MakeRelationshipObjectGOs(int amount)
    {
        while (amount > 0)
        {
            GameObject go = Instantiate(relationshipUIItemPrefab, transform);
            go.SetActive(false);
            relationshipUIItemGOPool.Add(go);
            amount--;

        }
    }

    public void UpdatePanel()
    {
        if (this.gameObject.activeSelf)
            ActivatePanel();
    }

    public override void DisablePanel()
    {
        base.DisablePanel();

        DisableAllActiveRelationshipUIItems();
    }
    private void DisableAllActiveRelationshipUIItems()
    {
        for (int j = activeRelationshipUIItemGOs.Count - 1; j >= 0; j--)
        {
            GameObject rshipItemGO = activeRelationshipUIItemGOs[j];

            rshipItemGO.SetActive(false);
            relationshipUIItemGOPool.Add(rshipItemGO);
            activeRelationshipUIItemGOs.RemoveAt(j);
        }
    }
}
