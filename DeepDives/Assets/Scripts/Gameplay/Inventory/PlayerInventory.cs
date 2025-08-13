using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;
    public ItemData[] initialItems; // Placeholder items
    public int inventorySize = 16;

    void Awake()
    {
        inventory = new Inventory(inventorySize);
        // Initialize with placeholder items
        foreach (ItemData item in initialItems)
        {
            inventory.AddItem(item, Random.Range(1, item.maxStackSize));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Check for nearby chest
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach (Collider2D collider in colliders)
            {
                Chest chest = collider.GetComponent<Chest>();
                if (chest != null)
                {
                    chest.Interact();
                    return;
                }
            }
        }
    }
}