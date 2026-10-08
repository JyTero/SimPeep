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

    private void OnDrawGizmos()
    {
        if (characterInSlot == null)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 0.15f);

        // Optional: show orientation
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * 0.4f
        );
    }
}