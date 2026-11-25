using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

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
            StandUpFromSlot(character);

        PathfindingResult pathfindingResult = characterPathfinding.NewCharacterFindingPath(character, destinationTile);
        //Handle Result
        characterAIHandler.CharacterRoutingResult(pathfindingResult, character);
    }

    public void TurnTowardsTile(Character character, LotGridTile tile)
    {
        Vector3 targetPosition = tile.TilePos;
        targetPosition.y = character.transform.position.y;
        character.transform.LookAt(targetPosition);
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



    public void SitCharacterToSlot(Character character, Character_Slot slot)
    {
        character.transform.position = slot.SlotTransform.position;
        character.CharacterPhysicalState = CharacterPhysicalStateEnum.SittingOnObject;
        character.OccupiedSlot = slot;

        slot.PlaceCharacterToSlot(character);
    }
    public void StandUpFromSlot(Character character)
    {
        Character_Slot occupiedSlot = character.OccupiedSlot;
        LotGridTile interactionSlotTile = null;

        character.CharacterPhysicalState = CharacterPhysicalStateEnum.Standing;
        character.OccupiedSlot.ClearSlot(character);
        character.OccupiedSlot = null;


        if (occupiedSlot.ParentItem.ItemCharacterSlotInteractionSlotPairs.Count == 0)
        {
            interactionSlotTile = lotManager.GetLotTile(occupiedSlot.ParentItem.InteractionSlots[0].SlotTransform.position);
        }
        else
        {
            foreach (ItemCharacterSlotInteractionSlotPair pair in occupiedSlot.ParentItem.ItemCharacterSlotInteractionSlotPairs)
            {
                if (pair.CharacterSlot == occupiedSlot)
                    interactionSlotTile = lotManager.GetLotTile(pair.InteractionSlot.SlotTransform.position);


            }
        }


        character.ChangeCurrentTile(interactionSlotTile);
        character.transform.position = character.CurrentTile.TilePos;

        //LotGridTile lgt = lotManager.GetNearbyFreeTile(lotManager.GetTileInteractableIsOn(character)); //Places onto random tile, causing interaction slot tile to be reserved
        //character.ChangeCurrentTile(lgt);

    }

    public bool IsWithinInteractionRange(Character character, ItemBase target)
    {
        List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(character.CurrentTile);
        if (neighborTiles.Any(neighborTile => neighborTile.InteractableOnTile == target))
            return true;
        else
            return false;
    }
    public bool IsOnItemWithinRange(Character character, ItemBase target)
    {
        List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(character.CurrentTile);
        foreach (LotGridTile lgt in neighborTiles)
        {
            if (lgt.InteractableOnTile)
                if (((ItemBase)lgt.InteractableOnTile).ItemSlotsByItem.ContainsKey(target)) //Never called, no items on tiles?
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
                //interaction.PushInteractionState(interaction, new Moving_InteractionState(characterAIHandler, interactionEngine));
                List<LotGridTile> neighborTiles = lotManager.GetNeighboringTiles(lotManager.GetTileInteractableIsOn(interaction.InteractionSource));
                foreach (LotGridTile ntile in neighborTiles)
                {
                    if (characterPathfinding.CanRouteToTile(character, ntile))
                    {
                        RouteToTile(character, ntile);
                        break;
                    }
                }
                // RouteToTile(character, neighborTiles[0]);
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
                //if (interaction.InteractionSource.InteractionSlots.Count != 0) //Item Uses Interaction Slots
                if (interaction.InteractionSource.UseInteractionSlot)
                {
                    LotGridTile destination = null;
                    foreach (Character_Slot interactionSlot in interaction.InteractionSource.InteractionSlots)
                    {
                        if (interactionSlot != null && lotManager.GetLotTile(interactionSlot.SlotTransform.position).Walkwability == ELGTWalkability.Yes)
                        {
                            if (((ItemBase)interaction.InteractionSource).ItemCharacterSlotInteractionSlotPairs.Count > 0)
                            {
                                ItemCharacterSlotInteractionSlotPair slotPair = ((ItemBase)interaction.InteractionSource).ItemCharacterSlotInteractionSlotPairs.FirstOrDefault(p => p.InteractionSlot == interactionSlot);
                                if (slotPair.CharacterSlot.CharacterInSlot == null)
                                {
                                    destination = lotManager.GetLotTile(interactionSlot.SlotTransform.position);
                                    break;
                                }

                            }
                            else
                                destination = lotManager.GetLotTile(interactionSlot.SlotTransform.position);
                        }

                    }
                    if (destination != null)
                    {
                        characterControl.RouteToTile(interaction.ThisCharacter, destination);

                    }
                    else
                        throw new Exception($"No available interaction slots on {interaction.InteractionSource.ItemName} for {interaction.ThisCharacter.ItemName} trying to {interaction.InteractionName}");
                }
                //else if (interaction.InteractionSource.InteractionArea != null) //Item Uses Interaction Area
                else if (interaction.InteractionSource.UseInteractionArea)
                    RouteToTileOnItemInteractionArea(interaction);
                else if (interaction.InteractionSource.UseInteractFromNearby)
                {
                    neighborTiles = lotManager.GetNeighboringTiles(interaction.InteractionSource.CurrentTile);
                    foreach (LotGridTile lgt in neighborTiles)
                    {
                        if (characterPathfinding.CanRouteToTile(interaction.ThisCharacter, lgt))
                            characterControl.RouteToTile(interaction.ThisCharacter, lgt);
                    }
                }
                else
                    throw new Exception($"Invalid item interaction spot/area on {interaction.InteractionSource.ItemName}");
                break;
            default:
                break;
        }
    }

    private void RouteToTileOnItemInteractionArea(ActiveInteraction interaction)
    {

        //Gummu/Temp solution. IF mulitple items see their stuff added here, TODO: Dedicated system
        if (interaction.InteractionSO.PreferSeating)
        {
            if (interaction.InteractionSource is TV_Item)
            {
                TV_Item tv = interaction.InteractionSource as TV_Item;
                foreach (Character_Slot sittingSlot in tv.SittingSlotsInArea)
                {
                    Character_Slot interactionSlot = tv.ItemCharacterSlotInteractionSlotPairs.FirstOrDefault(s => s.CharacterSlot == sittingSlot).InteractionSlot;
                    LotGridTile interactionSlotTile = lotManager.GetLotTile(interactionSlot.SlotTransform.position);
                    if (characterPathfinding.CanRouteToTile(interaction.ThisCharacter, interactionSlotTile))
                    {
                        RouteToTile(interaction.ThisCharacter, interactionSlotTile);
                        return;
                    }
                }
            }
        }

        Vector2Int forward = lotManager.GetItemForwardVector(interaction.InteractionSource);
        Vector2Int right = new(forward.y, -forward.x);
        LotGridTile tileInFront = lotManager.GetLotTile(interaction.InteractionSource.CurrentTile.Coordinates + forward);
        LotGridTile rightTile = lotManager.GetLotTile(interaction.InteractionSource.CurrentTile.Coordinates + right);

        //Debug.Log($"TV tile {interaction.InteractionSource.CurrentTile.Coordinates}, Tile infront: {tileInFront.Coordinates}, Right Tile: {rightTile.Coordinates} forward vector: {forward}, Right vector: {right} Tiles: {interaction.InteractionSource.CurrentTile.Coordinates + forward} & {interaction.InteractionSource.CurrentTile.Coordinates + right}");


        List<LotGridTile> tiles = lotManager.GetTilesInItemInteractionArea(interaction.InteractionSource as ItemBase);
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if (tiles[i].Walkwability != ELGTWalkability.Yes)
            {
                tiles.Remove(tiles[i]);
                continue;
            }

            if (!characterPathfinding.CanRouteToTile(interaction.ThisCharacter, tiles[i]))
                tiles.Remove(tiles[i]);
        }
        int random = UnityEngine.Random.Range(0, tiles.Count);
        LotGridTile dTile = tiles[random];
        RouteToTile(interaction.ThisCharacter, dTile);

    }

    private void SitCharacterToSlotFromInstruction(Character_Instruction charaInstruction, ActiveInteraction interaction)
    {
        ItemBase sitItem;
        if (interaction.InteractionSource != null)
            sitItem = interaction.InteractionSource as ItemBase;
        else
            sitItem = interaction.InteractionSource as ItemBase;

        Character_Slot sitSlot = sitItem.CharacterSlotsOnItem[0];
        if (sitItem.ItemCharacterSlotInteractionSlotPairs != null)
        {
            foreach (ItemCharacterSlotInteractionSlotPair slotPair in sitItem.ItemCharacterSlotInteractionSlotPairs)
            {
                if (lotManager.GetTileInteractableIsOn(interaction.ThisCharacter) == lotManager.GetLotTile(slotPair.InteractionSlot.SlotTransform.position))
                    sitSlot = slotPair.CharacterSlot;
            }
        }

        SitCharacterToSlot(interaction.ThisCharacter, sitSlot);
    }
}

