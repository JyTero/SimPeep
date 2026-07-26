using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRouting : ManagementCore
{
    [SerializeField]
    private float routingMargin;

    private Debuglandia debuglandia;
    private Dictionary<Character, Transform> charactersRouting = new();
    private Dictionary<ActiveInteraction, Transform> interactionsRouting = new();
    // private List<Character> charactersAtDestination = new();


    private void Start()
    {
        debuglandia = FindAnyObjectByType<Debuglandia>();
    }

    public void StartRouting(Character character, Transform destination)
    {
        charactersRouting.Add(character, destination);
    }
    public void StartRouting(ActiveInteraction interaction)
    {
        if (interaction.IsReaction)
            interactionsRouting.Add(interaction, interaction.ThisCharacter.transform);
        else
            interactionsRouting.Add(interaction, interaction.InteractionSource.transform);
    }

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        CharacterRoutingUpdate();
        InteractionRoutingUpdate(dt);
    }

    //"other"(?) moving (When moving without tied interaction)
    private void CharacterRoutingUpdate()
    {
        List<Character> charactersAtDest = new();
        foreach (Character character in charactersRouting.Keys)
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position,
                    charactersRouting[character].position, character.characterSpeed * Time.deltaTime);

            //IfAtDest?
            if (Vector3.Distance(character.transform.position, charactersRouting[character].position) < routingMargin)
            {
                charactersAtDest.Add(character);
            }

        }
        foreach (Character chara in charactersAtDest)
        {
            CharacerAtDestination(chara);
            charactersRouting.Remove(chara);

        }
    }
    //In interaction (moving for interaction purposes)
    private void InteractionRoutingUpdate(float dt)
    {
        List<ActiveInteraction> charactersAtDest = new();
        foreach (ActiveInteraction interaction in interactionsRouting.Keys)
        {
            Character character = interaction.ThisCharacter;
            character.transform.position = Vector3.MoveTowards(character.transform.position,
                    interactionsRouting[interaction].position, character.characterSpeed * dt);

            //IfAtDest?
            if (Vector3.Distance(character.transform.position, interactionsRouting[interaction].position) < routingMargin)
            {
                charactersAtDest.Add(interaction);
            }

        }
        foreach (ActiveInteraction interaction in charactersAtDest)
        {
            CharacerAtDestination(interaction.ThisCharacter);
            interaction.interactionState = InteractionState.AtDestination;

            interactionsRouting.Remove(interaction);

        }
    }
    private void CharacerAtDestination(Character chara)
    {
        //charactersAtDestination.Add(chara);
    }
}

