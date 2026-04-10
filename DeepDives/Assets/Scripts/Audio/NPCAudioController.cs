using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class NPCAudioController : MonoBehaviour
{
    public StudioEventEmitter emitter;
    public StudioEventEmitter voiceEmitter;

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

    public void PlayVoiceLine(string label)
    {
        if (voiceEmitter == null) return;

        voiceEmitter.Stop();

        EventInstance voiceInstance = voiceEmitter.EventInstance;
        if (voiceInstance.isValid())
        {
            voiceInstance.setParameterByNameWithLabel("LineIndex", label);
        }

        voiceEmitter.Play();
    }

    public void StopVoiceLine()
    {
        if (voiceEmitter != null)
        {
            voiceEmitter.Stop();
        }
    }
}