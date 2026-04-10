using UnityEngine;
using FMODUnity;

public class NPCAudioController : MonoBehaviour
{
    public StudioEventEmitter emitter;

    public void StartTalking()
    {
        if (emitter != null)
        {
            emitter.SetParameter("Talking", 1f);
        }
    }

    public void StopTalking()
    {
        if (emitter != null)
        {
            emitter.SetParameter("Talking", 0f);
        }
    }
}