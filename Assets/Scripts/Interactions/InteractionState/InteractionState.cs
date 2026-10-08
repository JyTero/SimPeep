using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionState
{
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
    public int itemSpecificInstructionIndex = 0;
    public bool itemSpecificInstructionsDone = false;
    public List<ItemSpecific_InstructionSO> stateItemSpecificInstructionSOs = new();


    protected CharacterAIHandler characterAIHandler;
    protected InteractionEngine interactionEngine;

    public InteractionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine)
    {
        this.characterAIHandler = characterAIHandler;
        this.interactionEngine = interactionEngine;
    }

    public virtual void OnStateEnter(ActiveInteraction interaction)
    {

    }
    public virtual void OnStateUpdate(ActiveInteraction interaction, float dt)
    {
        interactionEngine.OnStateUpdateInstructions(interaction);
    }
    public virtual void OnStateExit(ActiveInteraction interaction)
    {

    }

    public bool StateInstructionsDone()
    {
        if (needInstructionsDone)
            if (relationshipInstructionsDone)
                if (itemInstructionsDone)
                    if (characterInstructionsDone)
                        if (itemSpecificInstructionsDone)
                            return true;
                        else
                            return false;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        else
            return false;

    }
    public void ReceivedInstructions()
    {
        //hasReceivedInstructions = true;

        if (stateNeedInstructionSOs.Count == 0)
            needInstructionsDone = true;
        if (stateRelationshipInstructionSOs.Count == 0)
            relationshipInstructionsDone = true;
        if (stateItemInstructionDatas.Count == 0)
            itemInstructionsDone = true;
        if (stateCharacterInstructionSOs.Count == 0)
            characterInstructionsDone = true;
        if (stateItemSpecificInstructionSOs.Count == 0)
            itemSpecificInstructionsDone = true;

    }
}
