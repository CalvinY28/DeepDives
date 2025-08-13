using UnityEngine;

public class Chest : MonoBehaviour
{
    public Inventory chestInventory;
    public ItemData[] initialItems; // Placeholder items
    public int inventorySize = 8;
    private InventoryUI inventoryUI;
    private PlayerInventory player;

    void Awake()
    {
        chestInventory = new Inventory(inventorySize);
        // Initialize with placeholder items
        foreach (ItemData item in initialItems)
        {
            chestInventory.AddItem(item, Random.Range(1, item.maxStackSize));
        }
    }

    void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        player = FindObjectOfType<PlayerInventory>();
    }

    public void Interact()
    {
        inventoryUI.Setup(player.inventory, chestInventory);
        inventoryUI.Show();
    }
}