using UnityEngine;

public class Routine_interactionState : InteractionState
{
    public Routine_interactionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine) : base(characterAIHandler, interactionEngine)
    {
    }

    public override void OnStateEnter(ActiveInteraction interaction)
    {
        base.OnStateEnter(interaction);
        if(interaction.InteractionSO.PreferSeating && interaction.ThisCharacter.CharacterPhysicalState != CharacterPhysicalStateEnum.SittingOnObject)
        {
            Debug.LogWarning($"Character {interaction.ThisCharacter.ItemName} should look for seating, but logic's not implemented (Interaction: {interaction.InteractionName}");
        }
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