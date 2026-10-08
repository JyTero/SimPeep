using UnityEngine;

public class Ending_InteractionState : InteractionState
{
    public Ending_InteractionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine) : base(characterAIHandler, interactionEngine)
    {
    }

    public override void OnStateEnter(ActiveInteraction interaction)
    {
        base.OnStateEnter(interaction);
    }

    public override void OnStateUpdate(ActiveInteraction interaction, float dt)
    {
        base.OnStateUpdate(interaction, dt);
        if (StateInstructionsDone())
        {
            InteractionState interactionState = interaction.interactionStateStack.Peek();
            if (interactionState is not Default_InteractionState)
            {
                interaction.PopInteractionState();
                return;
            }

            interaction.interactionStateStack.Clear();
            interactionEngine.OnInteractionEnd(interaction);
        }
            //interaction.PopInteractionState();
    }

    public override void OnStateExit(ActiveInteraction interaction)
    {
        base.OnStateExit(interaction);




    }
}