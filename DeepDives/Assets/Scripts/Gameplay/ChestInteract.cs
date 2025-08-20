using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    public Animator animator;
    public CanvasGroup promptGroup;

    [Header("Reference to Existing Menu in Scene")]
    public MenuController menuController; // Drag the menu GameObject's MenuController script here

    private bool playerInRange = false;
    private bool isOpen = false;
    private Coroutine currentFade;

    void Start()
    {
        if (promptGroup != null)
            promptGroup.alpha = 0f;

        if (menuController != null)
            menuController.gameObject.SetActive(false); // Hide menu on start
    }

    void Update()
    {
        if (!playerInRange || animator == null)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isBusy = stateInfo.IsName("ChestOpen") || stateInfo.IsName("ChestClose");

        if (!isBusy && Input.GetKeyDown(KeyCode.Z))
        {
            if (!isOpen)
            {
                isOpen = true;
                animator.SetBool("isOpen", true);
                StartCoroutine(OpenMenuAfterAnimation());
            }
            else
            {
                // Chest is already open; reopen the menu if needed
                if (menuController != null)
                    menuController.ActivateMenu(true);
            }
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

            if (isOpen)
            {
                isOpen = false;
                animator.SetBool("isOpen", false);

                // Optional: close menu when player leaves
                if (menuController != null)
                    menuController.ActivateMenu(false);
            }
        }
    }

    void StartFade(float targetAlpha)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeCanvasGroup(promptGroup, targetAlpha));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
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

    IEnumerator OpenMenuAfterAnimation()
    {
        // Wait until the ChestOpen animation is done
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("ChestOpen"));

        if (menuController != null)
        {
            menuController.gameObject.SetActive(true);
            menuController.ActivateMenu(true);
        }

        StartFade(0f); // Hide prompt
    }
}
