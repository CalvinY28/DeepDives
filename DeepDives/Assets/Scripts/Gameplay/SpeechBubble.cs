using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI speechText;
    public float typingSpeed = 0.03f;
    public AudioClip typingSound;
    public float soundCooldown = 0.02f;

    private string[] dialogueLines;
    private int currentLineIndex = 0;
    private bool isActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private AudioSource audioSource;
    private float lastSoundTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartDialogue(string[] lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;
        isActive = true;
        gameObject.SetActive(true);
        ShowCurrentLine();
    }

    public void AdvanceDialogue()
    {
        if (!isActive) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            speechText.text = dialogueLines[currentLineIndex];
            isTyping = false;
        }
        else
        {
            currentLineIndex++;

            if (currentLineIndex < dialogueLines.Length)
            {
                ShowCurrentLine();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        speechText.text = "";
        lastSoundTime = 0f;

        foreach (char letter in line.ToCharArray())
        {
            speechText.text += letter;

            if (typingSound != null && Time.time - lastSoundTime > soundCooldown)
            {
                audioSource.PlayOneShot(typingSound);
                lastSoundTime = Time.time;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        gameObject.SetActive(false);
        isActive = false;
        isTyping = false;
    }

    public bool IsActive()
    {
        return isActive;
    }
}
