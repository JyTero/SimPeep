using UnityEngine;

public class Starting_InteractionState : InteractionState
{
    public Starting_InteractionState(CharacterAIHandler characterAIHandler, InteractionEngine interactionEngine) : base(characterAIHandler, interactionEngine)
    {
    }

    public override void OnStateEnter(ActiveInteraction interaction)
    {
        base.OnStateEnter(interaction);



        //Social interaction specifics
        if (interaction.InteractionData.IsSocial && !interaction.socialInteractionInitialised)
        {
            CharacterAI target = characterAIHandler.CharactersAIsByCharacter[(Character)interaction.InteractionSource];
            if (target.CanMajorSocialise)
            {
                //Continue with interaction
            }
            else if (target.CanMinorSocialise)
            {
                if (interaction.InteractionData.SocialInteractionType == ESocialInteractionType.Major)
                {
                    if (interaction.InteractionData.WaitForInteractionEnd)
                    {
                        //WAIT
                    }
                    else
                    {
                        Debug.LogError($"Target of a social interaction is not available!");
                        return;

                    }
                }
                else
                {
                    //Continue with interaction
                }
            }
            else
            {
                if (interaction.InteractionData.WaitForInteractionEnd)
                {
                    //WAIT
                }
                else
                {
                    Debug.LogError($"Target of a social interaction is not available!");
                    return;

                }
            }
            if (interaction.InteractionData.IsSocialResponse)
            {
                interactionEngine.RegisterToWait(interaction, EWaitReason.WaitForSocialInteractionPartner);
                interaction.socialInteractionInitialised = true;
                return;
            }

            //Send Social Response to target
            characterAIHandler.QueueInteraction(new(interaction.InteractionSource as Character,
                new(interaction.InteractionData.SocialResponceInteractions[0], interaction.ThisCharacter)), InteractionQueuePriority.UserSelectNPCReaction);
            interaction.socialInteractionInitialised = true;

        }
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

        if (interaction.InteractionData.IsSocial && !interaction.InteractionData.IsSocialResponse)
            characterAIHandler.ContinueSocialResponse(interaction.InteractionSource as Character); 
    }
}

