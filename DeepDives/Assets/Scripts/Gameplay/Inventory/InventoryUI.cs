using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform playerInventoryGrid;
    public Transform chestInventoryGrid;
    public GameObject slotPrefab;
    private Inventory playerInventory;
    private Inventory chestInventory;

    void Awake()
    {
        inventoryPanel.SetActive(false);
    }

    public void Setup(Inventory player, Inventory chest)
    {
        playerInventory = player;
        chestInventory = chest;
        ClearGrid(playerInventoryGrid);
        ClearGrid(chestInventoryGrid);
        PopulateGrid(playerInventory, playerInventoryGrid);
        PopulateGrid(chestInventory, chestInventoryGrid);
    }

    void ClearGrid(Transform grid)
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
    }

    void PopulateGrid(Inventory inventory, Transform grid)
    {
        for (int i = 0; i < inventory.maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, grid);
            Image icon = slotObj.transform.Find("ItemIcon").GetComponent<Image>();
            TextMeshProUGUI quantityText = slotObj.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();

            if (inventory.slots[i] != null)
            {
                icon.sprite = inventory.slots[i].item.icon;
                icon.color = Color.white;
                quantityText.text = inventory.slots[i].quantity.ToString();
            }
            else
            {
                icon.color = new Color(1, 1, 1, 0); // Transparent
                quantityText.text = "";
            }
        }
    }

    public void Show()
    {
        inventoryPanel.SetActive(true);
    }

    public void Hide()
    {
        inventoryPanel.SetActive(false);
    }
}