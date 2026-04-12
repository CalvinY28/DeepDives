using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class NPCInteract : MonoBehaviour
{
    public CanvasGroup talkPromptGroup;
    public SpeechBubble speechBubble;
    public NPCAudioController audioController;

    [TextArea(2, 5)]
    public string[] npcDialogueLines;

    private bool playerInRange = false;
    private Coroutine currentFade;
    private int currentLineIndex = 0;

    public EventReference Line1;
    public EventReference Line2;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            if (speechBubble.IsActive())
            {
                currentLineIndex++;
                speechBubble.AdvanceDialogue();
                PlayCurrentVoiceLine();
            }
            else
            {
                currentLineIndex = 0;
                speechBubble.StartDialogue(npcDialogueLines);

                if (audioController != null)
                    audioController.StartTalking();

                PlayCurrentVoiceLine();
            }
        }
    }
    void PlayCurrentVoiceLine()
    {
        if (currentLineIndex == 0)
        {
            RuntimeManager.PlayOneShot(Line1, transform.position);
        }
        else if (currentLineIndex == 1)
        {
            RuntimeManager.PlayOneShot(Line2, transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            StartFade(1f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StartFade(0f);
            speechBubble.EndDialogue();

            if (audioController != null)
                audioController.StopTalking();

            currentLineIndex = 0;
        }
    }

    void StartFade(float targetAlpha)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeCanvasGroup(talkPromptGroup, targetAlpha));
    }

    System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;
        float fadeDuration = 0.3f;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
