using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [Serializable]
    public class ShopItem
    {
        public string itemId = "item";
        public string displayName = "Placeholder Item";
        [Min(0)] public int buyPrice = 5;
        [Min(0)] public int sellPrice = 2;
        [Min(0)] public int startingStock = 5;

        [NonSerialized] public int stock;
        [NonSerialized] public int owned;

        public void Initialize()
        {
            stock = Mathf.Max(0, startingStock);
            owned = 0;
        }
    }

    [Header("Catalog")]
    [SerializeField] private string shopTitle = "SHOP";
    [SerializeField] private List<ShopItem> items = new List<ShopItem>();

    [Header("Optional Custom UI")]
    [SerializeField] private GameObject shopRoot;
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text balanceLabel;
    [SerializeField] private TMP_Text catalogLabel;
    [SerializeField] private TMP_Text instructionLabel;
    [SerializeField] private TMP_Text statusLabel;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closeButton;

    [Header("Placeholder UI")]
    [SerializeField] private bool buildPlaceholderUI = true;
    [SerializeField] private Color panelColor = new Color(0.05f, 0.07f, 0.12f, 0.95f);
    [SerializeField] private Color buttonColor = new Color(0.15f, 0.28f, 0.42f, 1f);

    private GameObject placeholderCanvasObject;
    private int selectedIndex;
    private int openedFrame;
    private bool isOpen;
    private float previousTimeScale = 1f;

    public event Action Closed;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        EnsureCatalog();

        for (int i = 0; i < items.Count; i++)
            items[i].Initialize();

        if (shopRoot == null && buildPlaceholderUI)
            BuildPlaceholderUI();

        BindButtons();

        if (shopRoot != null)
            shopRoot.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen || Time.frameCount == openedFrame)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
        {
            CloseShop();
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectPrevious();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectNext();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SellSelected();
        }
        else if (Input.GetKeyDown(KeyCode.D) ||
                 Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.Z) ||
                 Input.GetKeyDown(KeyCode.Return))
        {
            BuySelected();
        }
    }

    public void OpenShop()
    {
        if (isOpen)
            return;

        if (shopRoot == null)
        {
            Debug.LogWarning("Shop UI is not assigned and placeholder UI is disabled.", this);
            return;
        }

        isOpen = true;
        openedFrame = Time.frameCount;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, items.Count - 1));
        shopRoot.SetActive(true);
        SetStatus("Choose an item to buy or sell.");
        RefreshUI();
    }

    public void CloseShop()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (shopRoot != null)
            shopRoot.SetActive(false);

        Time.timeScale = previousTimeScale;
        Closed?.Invoke();
    }

    public void SelectNext()
    {
        if (items.Count == 0)
            return;

        selectedIndex = (selectedIndex + 1) % items.Count;
        SetStatus(string.Empty);
        RefreshUI();
    }

    public void SelectPrevious()
    {
        if (items.Count == 0)
            return;

        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
        SetStatus(string.Empty);
        RefreshUI();
    }

    public void BuySelected()
    {
        ShopItem item = GetSelectedItem();
        if (item == null)
            return;

        if (item.stock <= 0)
        {
            SetStatus(item.displayName + " is sold out.");
            return;
        }

        if (CurrencyManager.Instance == null)
        {
            SetStatus("Currency manager is unavailable.");
            return;
        }

        if (!CurrencyManager.Instance.TrySpend(item.buyPrice))
        {
            SetStatus("Not enough currency.");
            return;
        }

        item.stock--;
        item.owned++;
        SetStatus("Bought " + item.displayName + ".");
        RefreshUI();
    }

    public void SellSelected()
    {
        ShopItem item = GetSelectedItem();
        if (item == null)
            return;

        if (item.owned <= 0)
        {
            SetStatus("You do not own " + item.displayName + ".");
            return;
        }

        if (CurrencyManager.Instance == null)
        {
            SetStatus("Currency manager is unavailable.");
            return;
        }

        item.owned--;
        item.stock++;
        CurrencyManager.Instance.Add(item.sellPrice);
        SetStatus("Sold " + item.displayName + ".");
        RefreshUI();
    }

    private ShopItem GetSelectedItem()
    {
        if (items.Count == 0 || selectedIndex < 0 || selectedIndex >= items.Count)
            return null;

        return items[selectedIndex];
    }

    private void RefreshUI()
    {
        if (titleLabel != null)
            titleLabel.text = shopTitle;

        if (balanceLabel != null)
        {
            int balance = CurrencyManager.Instance != null ? CurrencyManager.Instance.Balance : 0;
            balanceLabel.text = "Currency: " + balance;
        }

        if (catalogLabel != null)
        {
            StringBuilder builder = new StringBuilder();

            if (items.Count == 0)
            {
                builder.Append("No items configured.");
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ShopItem item = items[i];
                    builder.Append(i == selectedIndex ? "> " : "  ");
                    builder.Append(item.displayName);
                    builder.Append("   Buy: ");
                    builder.Append(item.buyPrice);
                    builder.Append("   Sell: ");
                    builder.Append(item.sellPrice);
                    builder.Append("   Stock: ");
                    builder.Append(item.stock);
                    builder.Append("   Owned: ");
                    builder.Append(item.owned);

                    if (i < items.Count - 1)
                        builder.AppendLine();
                }
            }

            catalogLabel.text = builder.ToString();
        }

        if (instructionLabel != null)
            instructionLabel.text = "W/S: Select   D/Z/Enter: Buy   A: Sell   X/Esc: Close";

        if (buyButton != null)
            buyButton.interactable = GetSelectedItem() != null && GetSelectedItem().stock > 0;

        if (sellButton != null)
            sellButton.interactable = GetSelectedItem() != null && GetSelectedItem().owned > 0;
    }

    private void SetStatus(string message)
    {
        if (statusLabel != null)
            statusLabel.text = message;
    }

    private void EnsureCatalog()
    {
        if (items == null)
            items = new List<ShopItem>();

        if (items.Count > 0)
            return;

        items.Add(CreatePlaceholderItem("health_potion", "Health Potion", 5, 2, 5));
        items.Add(CreatePlaceholderItem("rope", "Rope", 8, 4, 3));
        items.Add(CreatePlaceholderItem("lantern", "Lantern", 12, 6, 2));
    }

    private static ShopItem CreatePlaceholderItem(
        string itemId,
        string displayName,
        int buyPrice,
        int sellPrice,
        int stock)
    {
        return new ShopItem
        {
            itemId = itemId,
            displayName = displayName,
            buyPrice = buyPrice,
            sellPrice = sellPrice,
            startingStock = stock
        };
    }

    private void BindButtons()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(BuySelected);

        if (sellButton != null)
            sellButton.onClick.AddListener(SellSelected);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
    }

    private void UnbindButtons()
    {
        if (buyButton != null)
            buyButton.onClick.RemoveListener(BuySelected);

        if (sellButton != null)
            sellButton.onClick.RemoveListener(SellSelected);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseShop);
    }

    private void BuildPlaceholderUI()
    {
        placeholderCanvasObject = new GameObject(
            "Shop Placeholder Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Canvas canvas = placeholderCanvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = placeholderCanvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panelObject = new GameObject(
            "Shop Panel",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );
        panelObject.transform.SetParent(placeholderCanvasObject.transform, false);

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1100f, 680f);
        panelRect.anchoredPosition = Vector2.zero;

        panelObject.GetComponent<Image>().color = panelColor;

        titleLabel = CreateText(
            panelObject.transform,
            "Title",
            shopTitle,
            44f,
            TextAlignmentOptions.Left,
            new Vector2(0.06f, 0.86f),
            new Vector2(0.62f, 0.97f)
        );

        balanceLabel = CreateText(
            panelObject.transform,
            "Balance",
            "Currency: 0",
            30f,
            TextAlignmentOptions.Right,
            new Vector2(0.62f, 0.86f),
            new Vector2(0.94f, 0.97f)
        );

        catalogLabel = CreateText(
            panelObject.transform,
            "Catalog",
            string.Empty,
            30f,
            TextAlignmentOptions.TopLeft,
            new Vector2(0.07f, 0.34f),
            new Vector2(0.93f, 0.78f)
        );

        instructionLabel = CreateText(
            panelObject.transform,
            "Instructions",
            string.Empty,
            23f,
            TextAlignmentOptions.Center,
            new Vector2(0.07f, 0.25f),
            new Vector2(0.93f, 0.32f)
        );

        statusLabel = CreateText(
            panelObject.transform,
            "Status",
            string.Empty,
            25f,
            TextAlignmentOptions.Center,
            new Vector2(0.07f, 0.17f),
            new Vector2(0.93f, 0.24f)
        );

        sellButton = CreateButton(
            panelObject.transform,
            "Sell Button",
            "SELL",
            new Vector2(0.07f, 0.05f),
            new Vector2(0.31f, 0.14f)
        );

        buyButton = CreateButton(
            panelObject.transform,
            "Buy Button",
            "BUY",
            new Vector2(0.38f, 0.05f),
            new Vector2(0.62f, 0.14f)
        );

        closeButton = CreateButton(
            panelObject.transform,
            "Close Button",
            "CLOSE",
            new Vector2(0.69f, 0.05f),
            new Vector2(0.93f, 0.14f)
        );

        shopRoot = placeholderCanvasObject;
        EnsureEventSystem();
    }

    private TextMeshProUGUI CreateText(
        Transform parent,
        string objectName,
        string value,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchorMin,
        Vector2 anchorMax)
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        SetAnchors(rect, anchorMin, anchorMax);

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont != null)
            label.font = defaultFont;

        label.text = value;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.alignment = alignment;
        label.raycastTarget = false;
        label.overflowMode = TextOverflowModes.Overflow;

        return label;
    }

    private Button CreateButton(
        Transform parent,
        string objectName,
        string labelText,
        Vector2 anchorMin,
        Vector2 anchorMax)
    {
        GameObject buttonObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button)
        );
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        SetAnchors(rect, anchorMin, anchorMax);

        Image image = buttonObject.GetComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        CreateText(
            buttonObject.transform,
            "Label",
            labelText,
            28f,
            TextAlignmentOptions.Center,
            Vector2.zero,
            Vector2.one
        );

        return button;
    }

    private static void SetAnchors(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = new Vector2(8f, 8f);
        rect.offsetMax = new Vector2(-8f, -8f);
        rect.localScale = Vector3.one;
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null ||
            UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        new GameObject(
            "Shop EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule)
        );
    }

    private void OnDestroy()
    {
        UnbindButtons();

        if (isOpen)
            Time.timeScale = previousTimeScale;

        if (placeholderCanvasObject != null)
            Destroy(placeholderCanvasObject);
    }
}
