using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterControl : ManagementCore
{
    [SerializeField]
    private float routingMargin;

    private Dictionary<Character, Transform> charactersRouting = new();
    private Dictionary<ActiveInteraction, Transform> interactionsRouting = new();
    // private List<Character> charactersAtDestination = new();


    protected override void Start()
    {
        base.Start();

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
            Vector3 frameDest = Vector3.MoveTowards(character.transform.position,
                                interactionsRouting[interaction].position, character.characterSpeed * dt);
            //character.transform.LookAt(frameDest);
            character.transform.position = frameDest;
            // character.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.x, 0); //Gummy

            //IfAtDest?
            if (Vector3.Distance(character.transform.position, interactionsRouting[interaction].position) < routingMargin)
            {
                charactersAtDest.Add(interaction);
                if (IsDebug)
                    Debug.Log($"{character.ItemName} reached destination ({interaction.InteractionSource})");
            }

            else if (IsDebug)
                Debug.Log($"{character.ItemName} routes towards {interaction.InteractionSource}");
        }
        foreach (ActiveInteraction interaction in charactersAtDest)
        {
            CharacerAtDestination(interaction.ThisCharacter);
            interaction.SetInteractionState(InteractionState.AtDestination);

            interactionsRouting.Remove(interaction);

        }
    }
    private void CharacerAtDestination(Character chara)
    {
        //charactersAtDestination.Add(chara);
    }


    //Inventory / Carrying

    public void PickupItem(Character character, ItemBase item)
    {
        if (character.CarriedItem == item)
            return;
        else if (character.CarriedItem == null)
        {
            item.transform.position = character.CarrySlot.position;
            itemManager.RegisterMovingItem(item, character.CarrySlot);
            character.PickupItem(item);
        }
        else
        {
            //Put item on hand down(to ground)
            PutItemDownGround(character);
            //Pick the item up 
            PickupItem(character, item);
        }


    }

    public void PutItemDownGround(Character character)
    {
        if (character.CarriedItem == null)
        {
            Debug.LogWarning("Tried to put down item when hand is empty!");
            return;
        }

        ItemBase item = character.CarriedItem;
        item.transform.position = character.transform.position; //Will be replaced with proper placement
        itemManager.DeregisterMovingItem(item);
        character.PutItemDown();
    }
}

