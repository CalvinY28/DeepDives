using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI speechText;
    public float typingSpeed = 0.03f;
    public AudioClip typingSound;
    public float soundCooldown = 0.02f;

    private string[] dialogueLines;
    private int currentLineIndex;
    private bool isActive;
    private bool isTyping;
    private Coroutine typingCoroutine;

    private AudioSource audioSource;
    private float lastSoundTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0 || speechText == null)
        {
            EndDialogue();
            return;
        }

        dialogueLines = lines;
        currentLineIndex = 0;
        isActive = true;
        gameObject.SetActive(true);
        ShowCurrentLine();
    }

    public void AdvanceDialogue()
    {
        if (!isActive)
            return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            speechText.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
            ShowCurrentLine();
        else
            EndDialogue();
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speechText.text = dialogueLines[currentLineIndex];
        speechText.maxVisibleCharacters = 0;
        speechText.ForceMeshUpdate();
        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        lastSoundTime = 0f;

        int characterCount = speechText.textInfo.characterCount;
        WaitForSeconds delay = new WaitForSeconds(typingSpeed);

        for (int visibleCharacters = 1; visibleCharacters <= characterCount; visibleCharacters++)
        {
            speechText.maxVisibleCharacters = visibleCharacters;

            if (typingSound != null && audioSource != null && Time.time - lastSoundTime > soundCooldown)
            {
                audioSource.PlayOneShot(typingSound);
                lastSoundTime = Time.time;
            }

            yield return delay;
        }

        speechText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
        typingCoroutine = null;
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (speechText != null)
            speechText.maxVisibleCharacters = int.MaxValue;

        isActive = false;
        isTyping = false;
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return isActive;
    }
}
