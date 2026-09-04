using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupedInteraction
{
    [SerializeField]
    private InteractionSO interactionSO;
    public InteractionSO InteractionSO { get { return interactionSO; } }

    [SerializeField]
    private int basePreferenceScore;
    public int BasePreferenceScore { get { return basePreferenceScore; } }

    //Requirements
    [SerializeField, InfoBox("These instructions must be able to be run for the interaction to be considered valid", EInfoBoxType.Normal)]
    private List<Item_InstructionSO> requiredItemInstructionSOs = new();
    public List<Item_InstructionSO> RequiredItemInstructionSOs { get { return requiredItemInstructionSOs; } }
}
