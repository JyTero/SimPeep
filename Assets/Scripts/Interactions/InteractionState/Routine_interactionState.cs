using UnityEngine;

public class Routine_interactionState : InteractionState
{
    public Routine_interactionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine) : base(characterAIHandler, interactionEngine)
    {
    }

    public override void OnStateEnter(ActiveInteraction interaction)
    {
        base.OnStateEnter(interaction);
    }

    public override void OnStateUpdate(ActiveInteraction interaction, float dt)
    {
        base.OnStateUpdate(interaction, dt);
    }

    public override void OnStateExit(ActiveInteraction interaction)
    {
        base.OnStateExit(interaction);
    }
}