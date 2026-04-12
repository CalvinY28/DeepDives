using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;


public class ButtonScript2 : MonoBehaviour
{
    public EventReference Click;
    public EventReference Select;
    public void OnButtonClickedStart()
    {
        RuntimeManager.PlayOneShot(Click, transform.position);
        SceneManager.LoadSceneAsync("1 4");
    }

    public void OnButtonClickedOptions()
    {
        RuntimeManager.PlayOneShot(Click, transform.position);
        Debug.Log("Button was clicked!"); // Open a new pop up with some options - controls - Continue - 
    }

    public void OnButtonClickedQuit()
    {
        RuntimeManager.PlayOneShot(Click, transform.position);
        Application.Quit();
        Debug.Log("Quited"); // App.quit only quits in the standalone make sure to include a new pop up, are you sure? y n
    }

}
