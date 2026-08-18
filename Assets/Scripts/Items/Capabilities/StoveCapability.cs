using UnityEngine;

public class StoveCapability : ItemCapability
{
    [SerializeField]
    private ItemSlot stoveCookSlot;
    public ItemSlot StoveCookSlot { get { return stoveCookSlot; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        capablityName = "StoveCapability";
        stoveCookSlot.InitialiseSlot(GetComponent<ItemBase>());
    }
}
