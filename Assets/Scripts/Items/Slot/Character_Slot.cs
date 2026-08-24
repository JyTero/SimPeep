using UnityEngine;

public class Character_Slot : Slot
{
    protected Character characterInSlot;
    public Character CharacterInSlot { get { return characterInSlot; } }

    public void PlaceCharacterToSlot(Character character)
    {
        characterInSlot = character;
    }

    public void ClearSlot(Character character)
    {
        characterInSlot = null;
    }
}