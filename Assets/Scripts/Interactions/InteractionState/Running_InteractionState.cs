using UnityEngine;

public class Running_InteractionState : InteractionState
{
    public Running_InteractionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine) : base(characterAIHandler, interactionEngine)
    {
    }

    public override void OnStateEnter(ActiveInteraction interaction)
    {
        base.OnStateEnter(interaction);
    }

    public override void OnStateUpdate(ActiveInteraction interaction, float dt)
    {
        base.OnStateUpdate(interaction, dt);

        //if (!interaction.OState.HasReceivedInstructions)
        //{
        //    interaction.OState.stateNeedInstructionSOs = interaction.InteractionData.Need_InteractionInstructionsOnInteractionTick;
        //    //RelationInstructions
        //    //interaction.InteractionState.stateItemInstructionSOs = interaction.InteractionTuningSO. ONTICK LIST;
        //    interaction.OState.ReceivedInstructions();
        //    return;
        //}
        //else
        //{       //Should probs be reworked, currently sends instructsions once / second
            if (interaction.TimeSinceLastInstructionsSent > interactionEngine.OneUnitOfTime)
            {
                bool tooMuchTime = true;
                while (tooMuchTime)
                {
                    interaction.TimeSinceLastInstructionsSent -= interactionEngine.OneUnitOfTime;
                    interactionEngine.HandleInteractionStateInstructions(interaction);

                    //GUMMY (Refersh Instructions since on tick = send one set of instructions/tick for the duration of interaction)
                    if (interaction.State.StateInstructionsDone())
                        RefreshStateInstructions();


                    if (interaction.TimeSinceLastInstructionsSent < interactionEngine.OneUnitOfTime)
                        tooMuchTime = false;
                }
            }
            else
                interaction.TimeSinceLastInstructionsSent += dt;
        //}

        if (interactionEngine.InteractionShouldEnd(interaction))
        {
            interaction.PopInteractionState();
        }
    }

    private void RefreshStateInstructions()
    {
        needIndex = 0;
        needInstructionsDone = false;

        relationshipIndex = 0;
        relationshipInstructionsDone = false;

        itemIndex = 0;
        itemInstructionsDone = false;

        characterIndex = 0;
        characterInstructionsDone = false;

        itemSpecificInstructionIndex = 0;
        itemSpecificInstructionsDone = false;
    }


    public override void OnStateExit(ActiveInteraction interaction)
    {
        base.OnStateExit(interaction);
    }
}