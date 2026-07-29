using TMPro;
using UnityEngine;

public class RelationshipUIItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI relationshipTargetNameField;
    [SerializeField]
    private TextMeshProUGUI relationshipScoreField;

    private Relationships_UIPanel relationshipPanel;

    private string targetName;
    private int relationshipScore;

    public void Initialise(CharacterRelationship relationship, Relationships_UIPanel ruip)
    {
        relationshipPanel = ruip;
        UpdateData(relationship);
    }

    public void UpdateData(CharacterRelationship relationship)
    {
        targetName = relationship.Target.ItemName;
        relationshipScore = relationship.RelationshipScore;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        relationshipTargetNameField.text = targetName;
        relationshipScoreField.text = relationshipScore.ToString();
    }
}
