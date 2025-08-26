using UnityEngine;

public class MenuTester : MonoBehaviour
{
    public MenuController menu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // if your MenuController has an IsOpen flag, prefer that over activeSelf
            bool isOpen = menu.gameObject.activeSelf;
            menu.ActivateMenu(!isOpen);
        }
    }
}
