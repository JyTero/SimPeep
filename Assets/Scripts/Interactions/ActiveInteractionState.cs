using System.Collections.Generic;
using UnityEngine;

public class ActiveInteractionState
{
    public EInteractionState thisState;

    private bool hasReceivedInstructions = false;
    public bool HasReceivedInstructions { get { return hasReceivedInstructions; } }

    public int needIndex = 0;
    public bool needInstructionsDone = false;
    public List<Need_InstructionSO> stateNeedInstructionSOs = new();
    public int relationshipIndex = 0;
    public bool relationshipInstructionsDone = false;
    public List<Relationship_InstructionSO> stateRelationshipInstructionSOs = new();
    public int itemIndex = 0;
    public bool itemInstructionsDone = false;
    public List<Item_InstructionData> stateItemInstructionDatas = new();
    public int characterIndex = 0;
    public bool characterInstructionsDone = false;
    public List<Character_InstructionSO> stateCharacterInstructionSOs = new();

    public Interactable interactionSource;

    public ActiveInteractionState(EInteractionState thisState)
    {
        this.thisState = thisState;
    }
    //public bool StateInstructionsDone()
    //{
    //    if (!hasReceivedInstructions)
    //        return false;
    //    if (stateNeedInstructionSOs.Count == 0 || needIndex == (stateNeedInstructionSOs.Count - 1))
    //        if (stateRelationshipInstructionSOs.Count == 0 || relationshipIndex == (stateRelationshipInstructionSOs.Count - 1))
    //            if (stateItemInstructionSOs.Count == 0 || itemIndex == (stateItemInstructionSOs.Count - 1))
    //                return true;
    //            else
    //                return false;
    //        else
    //            return false;
    //    else
    //        return false;

    //}

    public bool StateInstructionsDone()
    {
        if (!hasReceivedInstructions)
            return false;

        if (needInstructionsDone)
            if (relationshipInstructionsDone)
                if (itemInstructionsDone)
                    if (characterInstructionsDone)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        else
            return false;

    }
    public void RefreshStateInstructions()
    {
        needIndex = 0;
        needInstructionsDone = false;

        relationshipIndex = 0;
        relationshipInstructionsDone = false;

        itemIndex = 0;
        itemInstructionsDone = false;

        characterIndex = 0;
        characterInstructionsDone = false;

    }


    public void ReceivedInstructions()
    {
        hasReceivedInstructions = true;

        if (stateNeedInstructionSOs.Count == 0)
            needInstructionsDone = true;
        if (stateRelationshipInstructionSOs.Count == 0)
            relationshipInstructionsDone = true;
        if (stateItemInstructionDatas.Count == 0)
            itemInstructionsDone = true;
        if(stateCharacterInstructionSOs.Count == 0)
            characterInstructionsDone = true;


    }


}
