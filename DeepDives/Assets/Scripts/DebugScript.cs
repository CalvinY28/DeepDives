using UnityEngine;

public class MenuTester : MonoBehaviour
{
    public MenuController menu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.ActivateMenu(true);
        }
    }
}
