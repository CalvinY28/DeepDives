using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    public Animator animator;
    public CanvasGroup promptGroup;

    private bool playerInRange = false;
    private bool isOpen = false;
    private Coroutine currentFade;

    void Start()
    {
        if (promptGroup != null)
            promptGroup.alpha = 0f; // Hide at start
    }

    void Update()
    {
        if (!playerInRange || animator == null)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool isBusy = stateInfo.IsName("ChestOpen") || stateInfo.IsName("ChestClose");

        if (!isBusy && Input.GetKeyDown(KeyCode.Z))
        {
            isOpen = true;
            animator.SetBool("isOpen", true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            StartFade(1f); // Show "Press Z" prompt
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StartFade(0f); // Hide prompt

            if (isOpen)
            {
                isOpen = false;
                animator.SetBool("isOpen", false); // Trigger chest close
            }
        }
    }

    void StartFade(float targetAlpha)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeCanvasGroup(promptGroup, targetAlpha));
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
