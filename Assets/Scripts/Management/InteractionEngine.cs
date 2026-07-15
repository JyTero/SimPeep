using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class InteractionEngine : ManagementCore
{
    private List<ActiveInteraction> activeInteractions = new();

    private Debuglandia debuglandia;

    [SerializeField]
    private bool IsDebug;

    protected void Start()
    {
        base.Start();
        debuglandia = FindAnyObjectByType<Debuglandia>();
    }

    public void deebug(Character chara)
    {
        //Gather
        ItemBase item = FindAnyObjectByType<ItemBase>();
        List<InteractionSO> interactionSOs = item.AllInteractions;
        //Convert
        List<ActiveInteraction> interactions = new();
        foreach (InteractionSO actionSO in interactionSOs)
        {
            //TODO: Make "StoredInteraction" that holds interactionSO and item and is given here, instead of passing SOs.
            interactions.Add(new ActiveInteraction(chara, actionSO, item));
        }
        //Validity
        //Score
        //Pick best
        StartNewInteraction(interactions[0]);
    }
    public void StartNewInteraction(ActiveInteraction interaction)
    {
        interaction.interactionState = InteractionState.Starting;

        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} started interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");
        activeInteractions.Add(interaction);

        //Route
        interaction.interactionState = InteractionState.Moving;
        characterRouting.StartRouting(interaction);

    }
    public void OnInteractionDestinationArrival(ActiveInteraction interaction)
    {
        //Later, make more complicated for multi step interactions (They shall be a "container of interactions"
        interaction.interactionState = InteractionState.Running;
        SendInteractionInstructions(interaction);
    }
    private void SendInteractionInstructions(ActiveInteraction interaction)
    {
        //DEBUG
        List<Need_Instruction> needInstructions = new();
        foreach (Need_InstructionSO niso in interaction.InteractionTuningSO.Need_InteractionInstructions)
        {
            Need_Instruction ni = new(niso, interaction.ThisCharacter, interaction.InteractionLength);
            needInstructions.Add(ni);
        }
        FindAnyObjectByType<NeedsEngine>().NewInstructions(needInstructions);
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
                case InteractionState.Running:
                    break;
                case InteractionState.Ending:
                    EndInteraction(interaction);
                    break;
                case InteractionState.Default:
                    break;
            }

            interaction.interactionLenghtAccumulation += deltaTime + updateInterval;
            if (interaction.interactionLenghtAccumulation > interaction.InteractionLength)
            {
                interaction.interactionState = InteractionState.Ending;
            }
        }
    }

    private void EndInteraction(ActiveInteraction interaction)
    {
        activeInteractions.Remove(interaction);
        if (IsDebug)
            Debug.Log($"{interaction.ThisCharacter.ItemName} finished interaction {interaction.InteractionName} (of {interaction.InteractionSource.ItemName})");

        characterAIHandler.OnInteractionEnd(interaction.ThisCharacter);
    }

}

//Route
//Begin
//Loop
//Quit
//Cleanup