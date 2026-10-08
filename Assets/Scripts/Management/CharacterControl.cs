using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
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


    public void RouteToTile(Character character, LotGridTile destinationTile)
    {
        characterAIHandler.CharactersAIsByCharacter[character].CurrentInteraction.PushInteractionState(characterAIHandler.CharactersAIsByCharacter[character].CurrentInteraction, new Moving_InteractionState(characterAIHandler, interactionEngine));

        if (character.CharacterPhysicalState != CharacterPhysicalStateEnum.Standing)
            StandUpFromSlot(character, character.OccupiedSlot);

        characterPathfinding.NewCharacterFindingPath(character, destinationTile);

    }


    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        //CharacterRoutingUpdate();
        //InteractionRoutingUpdate(dt);
    }

    public bool IsCharacterNextToInteractionTarget(Character character, LotGridTile targetTile)
    {
        LotGridTile characterTile = character.CurrentTile;
        List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(characterTile);
        foreach (LotGridTile tile in neighborTiles)
        {
            if (tile == targetTile)
                return true;
        }
        return false;
    }

    //"other"(?) moving (When moving without tied interaction)

    public void CharacerAtDestination(Character chara)
    {
        //charactersAtDestination.Add(chara);
    }

    public void SitCharacterToSlot(Character character, Character_Slot slot)
    {
        character.transform.position = slot.SlotTransform.position;
        character.CharacterPhysicalState = CharacterPhysicalStateEnum.SittingOnObject;
        character.OccupiedSlot = slot;

        slot.PlaceCharacterToSlot(character);
    }
    public void StandUpFromSlot(Character character, Character_Slot slot)
    {
        character.CharacterPhysicalState = CharacterPhysicalStateEnum.Standing;
        character.OccupiedSlot = null;

        LotGridTile lgt = lotManager.GetNearbyFreeTile(lotManager.GetTileInteractableIsOn(character));
        character.ChangeCurrentTile(lgt);
        character.transform.position = character.CurrentTile.TilePos;

        slot.ClearSlot(character);
    }

    public bool IsWithinInteractionRange(Character character, ItemBase target)
    {
        List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(character.CurrentTile);
        if (neighborTiles.Any(neighborTile => neighborTile.ItemOnTile == target))
            return true;
        else
            return false;
    }
    public bool IsOnItemWithinRange(Character character, ItemBase target)
    {
        List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(character.CurrentTile);
        foreach (LotGridTile lgt in neighborTiles)
        {
            if (lgt.ItemOnTile)
                if (((ItemBase)lgt.ItemOnTile).ItemSlotsByItem.ContainsKey(target)) //Never called, no items on tiles?
                    return true;
        }
        return false;
    }

    public void MoveForSocialInteraction(ActiveInteraction interaction)
    {
        Character target = interaction.InteractionSource as Character;
    }

    //Inventory / Carrying
    public void PickupItem(ActiveInteraction interaction, ItemBase item)
    {
        Character character = interaction.ThisCharacter;

        if (character.CarriedItem == item)
            return;
        else if (character.CarriedItem == null)
        {
            if (IsWithinInteractionRange(character, item))
            {
                item.transform.position = character.CarrySlot.position;
                itemManager.RegisterMovingItem(item, character.CarrySlot);
                itemManager.OnItemPickUp(item, character);
                lotManager.PickItemUpFromLot(item);
                character.PickupItem(item);
            }
            else if (IsOnItemWithinRange(character, item))
            {
                Item_Slot currentSlot = item.OccupiedSlot;

                currentSlot.ParentItem.RemoveItemFromSlot(currentSlot);
                currentSlot.ClearSlot();

                item.transform.position = character.CarrySlot.position;
                item.RemoveThisItemFromSlot();
                itemManager.RegisterMovingItem(item, character.CarrySlot);
                itemManager.OnItemPickUp(item, character);
                character.PickupItem(item);
            }
            else
            {
                interaction.State.itemIndex--;
                interaction.PushInteractionState(interaction, new Moving_InteractionState(characterAIHandler, interactionEngine));
                List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(lotManager.GetTileInteractableIsOn(interaction.InteractionSource));
                RouteToTile(character, neighborTiles[0]);
            }
        }
        else
        {
            //Put item on hand down(to ground)
            lotManager.PlaceItemOntoLot(character.ThisLot, character.CurrentTile, character.CarriedItem);
            SeparateItemFromHand(character);
            //Pick the item up 
            PickupItem(interaction, item);
        }


    }

    public void SeparateItemFromHand(Character character)
    {
        if (character.CarriedItem == null)
        {
            Debug.LogWarning("Tried to put down item when hand is empty!");
            return;
        }

        ItemBase item = character.CarriedItem;
        itemManager.OnItemPutDown(item, character);
        //item.transform.position = character.transform.position; //Will be replaced with proper placement
        itemManager.DeregisterMovingItem(item);
        character.PutItemDown();
    }

    //CHARACTER INSTRUCTIONS
    public void HandleCharacterInstruction(Character_Instruction charaInstruction, ActiveInteraction interaction)
    {
        switch (charaInstruction.InstructionType)
        {
            case ECharacterInstruction.Default:
                break;
            case ECharacterInstruction.MoveCharacter:
                MoveCharacter(charaInstruction, interaction);
                break;
            case ECharacterInstruction.SitCharacter:
                SitCharacterToSlotFromInstruction(charaInstruction, interaction);
                break;
            default:
                break;
        }

    }

    private void MoveCharacter(Character_Instruction charaInstruction, ActiveInteraction interaction)
    {
        switch (charaInstruction.DestinationType)
        {
            case ECharacterInstructionDestination.Default:
                break;
            case ECharacterInstructionDestination.ThisItem:
                List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(lotManager.GetTileInteractableIsOn(interaction.InteractionSource));
                characterControl.RouteToTile(interaction.ThisCharacter, neighborTiles[0]);
                break;
            case ECharacterInstructionDestination.ThisItemInteractionSlot:
                characterControl.RouteToTile(interaction.ThisCharacter, lotManager.GetLotTile(interaction.InteractionSource.InteractionSlot.SlotTransform.position));
                break;
            default:
                break;
        }
    }

    private void SitCharacterToSlotFromInstruction(Character_Instruction charaInstruction, ActiveInteraction interaction)
    {
        ItemBase sitItem;
        if (interaction.InteractionSource != null)
            sitItem = interaction.InteractionSource as ItemBase;
        else
            sitItem = interaction.InteractionSource as ItemBase;

        SitCharacterToSlot(interaction.ThisCharacter, sitItem.CharacterSlotsOnItem[0]);
    }
}

