using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class InteractionEngine : ManagementCore
{
    private List<ActiveInteraction> activeInteractions = new();

    private Debuglandia debuglandia;

    private Dictionary<Character, ActiveInteraction> waitingInteractionsByWaitee = new();

    protected void Start()
    {
        base.Start();
        debuglandia = FindAnyObjectByType<Debuglandia>();
    }

    public void LoadingScreen()
    {

    }

    public void StartNewInteraction(ActiveInteraction interaction)
    {
        interaction.interactionState = InteractionState.Starting;

        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} started interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
        activeInteractions.Add(interaction);

        //TODO: Check that intraction is ready to start (other character available, item has free slot (BONUS: When to wait wait vs fail)
        //if(IsTargetAvailable())
        //  Start

        if (interaction.InteractionSource is Character)
        {
            //Cue "ResponceInteraction to given SocialInteraction" (Via AIHandler)
            //On the other one, if this is reaction, alert system to continue
            if (interaction.IsReaction)
            {
                RouteToInteraction(interaction);
            }
            else
            {
                ActiveInteraction responce = new ActiveInteraction(interaction.InteractionSource as Character, new StoredInteraction(interaction.InteractionTuningSO.SocialResponceInteractions[0], interaction.ThisCharacter));
                characterAIHandler.QueueInteraction(responce, CharacterAIHandler.InteractionQueueSource.UserSelectNPCReaction);
                RouteToInteraction(interaction);
            }

        }
        else
            //Route
            RouteToInteraction(interaction);

    }

    private void RouteToInteraction(ActiveInteraction interaction)
    {
        interaction.interactionState = InteractionState.Moving;
        characterRouting.StartRouting(interaction);
    }

    public void OnInteractionDestinationArrival(ActiveInteraction interaction)
    {
        //Later, make more complicated for multi step interactions (They shall be a "container of interactions"

        //Queue Social Interactions to wait, router initialises on arrival
        if (interaction.InteractionSource is Character)
        {
            if (interaction.IsReaction)
                interaction.interactionState = InteractionState.Waiting;
            else
            {
                //Run interaction on both parties from the same orderr
                interaction.interactionState = InteractionState.Running;
                SendOnInteractionBeginInstructions(interaction);

                //Social Responce interaction handling
                waitingInteractionsByWaitee[interaction.InteractionSource as Character].interactionState = InteractionState.Running;
                SendOnInteractionBeginInstructions(waitingInteractionsByWaitee[interaction.InteractionSource as Character]);
            }

        }
        else
        {
            interaction.interactionState = InteractionState.Running;
            SendOnInteractionBeginInstructions(interaction);
        }
    }

    private void SendOnInteractionBeginInstructions(ActiveInteraction interaction)
    {
        //TBD
    }

    private void RegisterToWait(ActiveInteraction interaction)
    {
        if (waitingInteractionsByWaitee.ContainsKey(interaction.ThisCharacter))
            return;

        waitingInteractionsByWaitee.Add(interaction.ThisCharacter, interaction);
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;

        InteractionUpdate(deltaTime);
    }

    private void InteractionUpdate(float deltaTime)
    {

        for (int i = activeInteractions.Count - 1; i >= 0; i--)
        {
            ActiveInteraction interaction = activeInteractions[i];
            switch (interaction.interactionState)
            {
                case InteractionState.Starting:
                    break;
                case InteractionState.Moving:
                    break;
                case InteractionState.AtDestination:
                    OnInteractionDestinationArrival(interaction);
                    break;
                case InteractionState.Waiting:
                    RegisterToWait(interaction);
                    break;
                case InteractionState.Running:
                    InteractionOnTick(interaction, deltaTime);
                    break;
                case InteractionState.Ending:
                    EndInteraction(interaction);
                    break;
                case InteractionState.Default:
                    break;
            }

            if (InteractionShouldEnd(interaction))
            {
                interaction.interactionState = InteractionState.Ending;

            }

        }
    }

    private void InteractionOnTick(ActiveInteraction interaction, float dt)
    {
        //OnTick Instructions
        if (interaction.TimeSinceLastInstructionsSent > OneUnitOfTime)
        {

            interaction.TimeSinceLastInstructionsSent -= OneUnitOfTime;
            List<Need_Instruction> needInstructions = new();
            foreach (Need_InstructionSO niso in interaction.InteractionTuningSO.Need_InteractionInstructions)
            {
                Need_Instruction ni = new(niso, interaction.ThisCharacter);
                needInstructions.Add(ni);
            }
            needsEngine.NewInstructions(needInstructions);
        }
        else
            interaction.TimeSinceLastInstructionsSent += dt;
    }
    private bool InteractionShouldEnd(ActiveInteraction interaction)
    {
        switch (interaction.InteractionEndingType)

        {
            case InteractionEndingType.Default:
                return true;
            case InteractionEndingType.SetTime:
                interaction.interactionLenghtAccumulation += deltaTime + updateInterval;
                if (interaction.interactionLenghtAccumulation > interaction.InteractionLength)
                {
                    return true;
                }
                else
                    return false;
            case InteractionEndingType.UntillNeedAtValue:
                if (interaction.ThisCharacter.Needs[interaction.InteractionEndingTargetNeedType].NeedValue >= interaction.InteractionEndingTargetNeedValue)
                    return true;
                else
                    return false;
        }
        return true;
    }

    private void EndInteraction(ActiveInteraction interaction)
    {
        SendOnInteractionEndInstructions(interaction);

        activeInteractions.Remove(interaction);
        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} finished interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");

        characterAIHandler.OnInteractionEnd(interaction.ThisCharacter);
    }
    private void SendOnInteractionEndInstructions(ActiveInteraction interaction)
    {
        //Relationships
        foreach (Relationship_InstructionSO relso in interaction.InteractionTuningSO.RelationshipChangeInstructions)
        {
            Character thisCharacter = interaction.ThisCharacter;
            Character targetCharacter = interaction.InteractionSource as Character;
            if (relationshipsManager.HasExistingRelationship(thisCharacter, targetCharacter))
            {
                relationshipsManager.AdjustRelationship(thisCharacter, targetCharacter, relso.RelationshipScoreChange);
            }
            else
            {
                relationshipsManager.NewRelationship(thisCharacter, targetCharacter);
                relationshipsManager.AdjustRelationship(thisCharacter, targetCharacter, relso.RelationshipScoreChange);
            }


            if (IsDebug)
                Debug.Log($"Relations!({interaction.ThisCharacter.ItemName} towards {interaction.InteractionSource.ItemName})");
        }
    }
}

//Route
//Begin
//Loop
//Quit
//Cleanup