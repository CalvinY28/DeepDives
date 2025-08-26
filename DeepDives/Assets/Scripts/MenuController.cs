using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public List<MenuOption> options;
    public List<TextMeshProUGUI> optionTexts;

    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private int currentIndex = 0;
    private bool isActive = false;

    void Start()
    {
        UpdateVisuals();
    }

    void Update()
    {
        if (!isActive) return;

        // Block movement input when menu is open
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            // Do nothing — eat the input
            return;
        }

        // Menu navigation logic
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % options.Count;
            UpdateVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + options.Count) % options.Count;
            UpdateVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            options[currentIndex].onSelect.Invoke();
        }
    }

    public void ActivateMenu(bool active)
    {
        isActive = active;
        Time.timeScale = active ? 0 : 1; // Pause game when menu is up
        
        gameObject.SetActive(active);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < optionTexts.Count; i++)
        {
            optionTexts[i].text = options[i].label;
            optionTexts[i].color = (i == currentIndex) ? highlightColor : normalColor;
        }
    }
}
