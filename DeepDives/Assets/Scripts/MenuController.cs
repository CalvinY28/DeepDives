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

    private static MenuController openMenu;
    private int currentIndex = 0;
    private bool isActive = false;

    void OnEnable() // On enable to make it always be the top option
    {
        currentIndex = 0;
        UpdateVisuals();
    }

    void OnDisable()
    {
        if (openMenu == this) openMenu = null;
        if (openMenu == null) Time.timeScale = 1;
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            return;
        }

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
        if (active)
        {
            if (openMenu != null && openMenu != this) return;
            openMenu = this;
        }
        else
        {
            if (openMenu == this) openMenu = null;
        }

        isActive = active;
        Time.timeScale = (openMenu != null) ? 0 : 1;

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
