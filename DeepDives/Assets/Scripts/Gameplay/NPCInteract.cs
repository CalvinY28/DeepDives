using System.Collections;
using FMODUnity;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public CanvasGroup talkPromptGroup;
    public SpeechBubble speechBubble;
    public NPCAudioController audioController;
    public ShopController shopController;

    [TextArea(2, 5)]
    public string[] npcDialogueLines;

    [TextArea(2, 5)]
    public string[] repeatDialogueLines;

    public EventReference Line1;
    public EventReference Line2;
    public EventReference[] repeatVoiceLines;

    private bool playerInRange;
    private bool hasCompletedConversation;
    private bool usingRepeatDialogue;
    private Coroutine currentFade;

    private void Awake()
    {
        if (shopController == null)
            shopController = GetComponent<ShopController>();
    }

    private void OnEnable()
    {
        if (speechBubble != null)
        {
            speechBubble.LineStarted += HandleLineStarted;
            speechBubble.DialogueCompleted += HandleDialogueCompleted;
        }

        if (shopController != null)
            shopController.Closed += HandleShopClosed;
    }

    private void OnDisable()
    {
        if (speechBubble != null)
        {
            speechBubble.LineStarted -= HandleLineStarted;
            speechBubble.DialogueCompleted -= HandleDialogueCompleted;
        }

        if (shopController != null)
            shopController.Closed -= HandleShopClosed;
    }

    private void Update()
    {
        if (!playerInRange || speechBubble == null || !Input.GetKeyDown(KeyCode.Z))
            return;

        if (shopController != null && shopController.IsOpen)
            return;

        if (speechBubble.IsActive())
            speechBubble.AdvanceDialogue();
        else
            StartConversation();
    }

    private void StartConversation()
    {
        usingRepeatDialogue = hasCompletedConversation &&
                              repeatDialogueLines != null &&
                              repeatDialogueLines.Length > 0;

        string[] dialogue = usingRepeatDialogue ? repeatDialogueLines : npcDialogueLines;
        StartFade(0f);

        if (audioController != null)
            audioController.StartTalking();

        speechBubble.StartDialogue(dialogue);
    }

    private void HandleLineStarted(int lineIndex)
    {
        if (usingRepeatDialogue)
        {
            if (repeatVoiceLines != null && lineIndex < repeatVoiceLines.Length)
                RuntimeManager.PlayOneShot(repeatVoiceLines[lineIndex], transform.position);

            return;
        }

        if (lineIndex == 0)
            RuntimeManager.PlayOneShot(Line1, transform.position);
        else if (lineIndex == 1)
            RuntimeManager.PlayOneShot(Line2, transform.position);
    }

    private void HandleDialogueCompleted()
    {
        hasCompletedConversation = true;
        usingRepeatDialogue = false;
        StartFade(0f);

        if (audioController != null)
            audioController.StopTalking();

        if (shopController != null)
            shopController.OpenShop();
        else if (playerInRange)
            StartFade(1f);
    }

    private void HandleShopClosed()
    {
        if (playerInRange)
            StartFade(1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (shopController == null || !shopController.IsOpen)
            StartFade(1f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        StartFade(0f);

        if (speechBubble != null)
            speechBubble.EndDialogue();

        if (audioController != null)
            audioController.StopTalking();

        if (shopController != null)
            shopController.CloseShop();

        usingRepeatDialogue = false;
    }

    private void StartFade(float targetAlpha)
    {
        if (talkPromptGroup == null)
            return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeCanvasGroup(targetAlpha));
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        const float fadeDuration = 0.3f;
        float startAlpha = talkPromptGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            talkPromptGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        talkPromptGroup.alpha = targetAlpha;
        currentFade = null;
    }
}
