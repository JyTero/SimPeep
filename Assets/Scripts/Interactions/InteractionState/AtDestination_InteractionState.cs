using UnityEngine;

public class AtDestination_InteractionState : InteractionState
{
    public AtDestination_InteractionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine) : base(characterAIHandler, interactionEngine)
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
            interaction.PopInteractionState();
    }

    public override void OnStateExit(ActiveInteraction interaction)
    {
        base.OnStateExit(interaction);
    }
}
