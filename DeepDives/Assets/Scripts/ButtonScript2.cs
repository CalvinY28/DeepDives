using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript2 : MonoBehaviour
{
    public void OnButtonClickedStart()
    {
        SceneManager.LoadSceneAsync("1");
    }

    public void OnButtonClickedOptions()
    {
        Debug.Log("Button was clicked!"); // Open a new pop up with some options - controls - Continue - 
    }

    public void OnButtonClickedQuit()
    {
        Application.Quit();
        Debug.Log("Quited"); // App.quit only quits in the standalone make sure to include a new pop up, are you sure? y n
    }

}
