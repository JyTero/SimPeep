using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (interaction.ThisCharacter.CharacterPhysicalState != CharacterPhysicalStateEnum.Standing)
            StandUpFromSlot(interaction.ThisCharacter, interaction.ThisCharacter.OccupiedSlot);


        if (interaction.IsReaction)
            characterPathfinding.NewCharacterFindingPath(interaction.ThisCharacter, interaction.ThisCharacter.transform.position);
        else if (interaction.InteractionTuningSO.InteractionDestinationDifferentFromSource)
            characterPathfinding.NewCharacterFindingPath(interaction.ThisCharacter, lotManager.GetItemOnLotByType(interaction.ThisCharacter.ThisLot, interaction.InteractionTuningSO.DestinationItem).transform.position);
        else
            characterPathfinding.NewCharacterFindingPath(interaction.ThisCharacter, interaction.InteractionSource.transform.position);

        //if (interaction.IsReaction)
        //    interactionsRouting.Add(interaction, interaction.ThisCharacter.transform);
        //else
        //    interactionsRouting.Add(interaction, interaction.InteractionSource.transform);
    }

    public void RouteToTile(Character character, LotGridTile destinationTile)
    {
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
        foreach(LotGridTile tile in neighborTiles)
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
        if (neighborTiles.Any(neighborTile => neighborTile.itemOnTile == target))
            return true;
        else
            return false;
    }
    public bool IsOnItemWithinRange(Character character, ItemBase target)
    {
        List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(character.CurrentTile);
        foreach(LotGridTile lgt in neighborTiles)
        {
            if (lgt.itemOnTile)
                if (lgt.itemOnTile.ItemSlotsByItem.ContainsKey(target))
                    return true;
        }
            return false;
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
                interaction.PushInteractionState(EInteractionState.Moving);
                RouteToTile(character, lotManager.GetTileInteractableIsOn(item));
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
    public void HandleCharacterInstruction(Character_InstructionSO charaInstructionSO, ActiveInteraction interaction)
    {
        switch (charaInstructionSO.InstructionType)
        {
            case ECharacterInstruction.Default:
                break;
            case ECharacterInstruction.MoveCharacter:
                MoveCharacter(charaInstructionSO, interaction);
                break;
            case ECharacterInstruction.SitCharacter:
                SitCharacterToSlot(charaInstructionSO, interaction);
                break;
            default:
                break;
        }

    }

    private void MoveCharacter(Character_InstructionSO charaInstructionSO, ActiveInteraction interaction)
    {
        switch (charaInstructionSO.DestinationType)
        {
            case ECharacterInstructionDestination.Default:
                break;
            case ECharacterInstructionDestination.ThisItem:
                interaction.PushInteractionState(EInteractionState.Moving);
                characterControl.RouteToTile(interaction.ThisCharacter, lotManager.GetTileInteractableIsOn(interaction.InteractionSource));
                break;
            default:
                break;
        }
    }

    private void SitCharacterToSlot(Character_InstructionSO charaInstructionSO, ActiveInteraction interaction)
    {
        ItemBase sitItem;
        if (interaction.State.interactionSource != null)
            sitItem = interaction.State.interactionSource as ItemBase;
        else
            sitItem = interaction.InteractionSource as ItemBase;

        SitCharacterToSlot(interaction.ThisCharacter, sitItem.CharacterSlotsOnItem[0]);
    }
}

