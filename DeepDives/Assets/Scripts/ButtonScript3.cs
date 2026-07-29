using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript3 : MonoBehaviour
{
    public Button[] buttons;
    private int currentIndex;
    public Color highlightColor = Color.yellow;
    public Color defaultColor = Color.white;

    public EventReference Click;
    public EventReference Select;

    private TextMeshProUGUI[] buttonTexts;

    private void Start()
    {
        Cursor.visible = false;

        if (buttons == null || buttons.Length == 0)
        {
            enabled = false;
            return;
        }

        buttonTexts = new TextMeshProUGUI[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
                buttonTexts[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>(true);
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, buttons.Length - 1);
        UpdateButtonHighlight();
    }

    private void Update()
    {
        Button currentButton = buttons[currentIndex];
        if (currentButton == null || !currentButton.gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RuntimeManager.PlayOneShot(Select, transform.position);
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            UpdateButtonHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RuntimeManager.PlayOneShot(Select, transform.position);
            currentIndex = (currentIndex + 1) % buttons.Length;
            UpdateButtonHighlight();
        }

        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && buttons[currentIndex] != null)
            buttons[currentIndex].onClick.Invoke();
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttonTexts.Length; i++)
        {
            if (buttonTexts[i] != null)
                buttonTexts[i].color = i == currentIndex ? highlightColor : defaultColor;
        }
    }
}
