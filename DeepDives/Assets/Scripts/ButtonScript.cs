using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; // Replace with TextMeshProUGUI
    public Color highlightColor = Color.yellow; // Color when highlighted
    private Color originalColor; // To store the original color of the text

    void Start()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (buttonText != null)
        {
            originalColor = buttonText.color; // Store the original text color
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ww");
        if (buttonText != null)
        {
            buttonText.color = highlightColor; // Change text color on hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("wwighouwer");
        if (buttonText != null)
        {
            buttonText.color = originalColor; // Revert text color on exit
        }
    }
}
