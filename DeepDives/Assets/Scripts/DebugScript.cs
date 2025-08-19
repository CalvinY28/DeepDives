using UnityEngine;

public class MenuTester : MonoBehaviour
{
    public MenuController menu;
    public MenuController menu2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            menu.ActivateMenu(true);
            menu2.ActivateMenu(true);
        }
    }
}
