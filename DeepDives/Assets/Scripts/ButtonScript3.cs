using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;


public class ButtonScript3 : MonoBehaviour
{
    public Button[] buttons; // Array to hold buttons
    private int currentIndex = 0; // Track the currently selected button
    public Color highlightColor = Color.yellow;
    public Color defaultColor = Color.white;

    public EventReference Click;
    public EventReference Select;

    private void Start()
    {
        Cursor.visible = false; // Hide the mouse cursor
        UpdateButtonHighlight();
    }

    private void Update()
    {
        // Navigate through buttons
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RuntimeManager.PlayOneShot(Select, transform.position);
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length; // Move up
            UpdateButtonHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RuntimeManager.PlayOneShot(Select, transform.position);
            currentIndex = (currentIndex + 1) % buttons.Length; // Move down
            UpdateButtonHighlight();
        }

        // Activate button
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            RuntimeManager.PlayOneShot(Click, transform.position);
            buttons[currentIndex].onClick.Invoke(); // Trigger the button's OnClick event
        }
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = i == currentIndex ? highlightColor : defaultColor; // Highlight selected button
            }
        }
    }
}
