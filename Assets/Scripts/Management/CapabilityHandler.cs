using UnityEngine;

public class CapabilityHandler : ManagementCore
{
    protected override void Start()
    {
        base.Start();

    }

    public void HandleCapability(ItemCapability capability, Character character)
    {
        switch (capability)
        {
            case StoveCapability stove:
                HandleStoveCapability(stove, character);
                break;

        }
    }

    private void HandleStoveCapability(StoveCapability stoveCapability, Character character)
    {
        itemManager.PlaceCarriedItemToSlot(character, stoveCapability.StoveCookSlot);
    }
}
