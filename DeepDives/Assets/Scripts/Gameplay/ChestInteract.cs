using System.Collections;
using FMODUnity;
using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    public Animator animator;
    public CanvasGroup promptGroup;

    [Header("Header for menucontroller")]
    public MenuController menuController;

    private static readonly int IsOpenHash = Animator.StringToHash("isOpen");
    private static readonly int ChestOpenHash = Animator.StringToHash("ChestOpen");
    private static readonly int ChestCloseHash = Animator.StringToHash("ChestClose");

    private bool playerInRange;
    private bool isOpen;
    private Coroutine currentFade;

    public EventReference openSound;
    public EventReference closeSound;

    private void Start()
    {
        if (promptGroup != null)
            promptGroup.alpha = 0f;

        if (menuController != null)
            menuController.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || animator == null || !Input.GetKeyDown(KeyCode.Z))
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isBusy = stateInfo.shortNameHash == ChestOpenHash || stateInfo.shortNameHash == ChestCloseHash;
        if (isBusy)
            return;

        if (!isOpen)
        {
            isOpen = true;
            animator.SetBool(IsOpenHash, true);
            StartCoroutine(OpenMenuAfterAnimation());
            RuntimeManager.PlayOneShot(openSound, transform.position);
        }
        else if (menuController != null)
        {
            menuController.ActivateMenu(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        StartFade(1f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        StartFade(0f);

        if (!isOpen)
            return;

        isOpen = false;
        if (animator != null)
            animator.SetBool(IsOpenHash, false);

        RuntimeManager.PlayOneShot(closeSound, transform.position);

        if (menuController != null)
            menuController.ActivateMenu(false);
    }

    private void StartFade(float targetAlpha)
    {
        if (promptGroup == null)
            return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeCanvasGroup(targetAlpha));
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        const float fadeDuration = 0.3f;
        float startAlpha = promptGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            promptGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        promptGroup.alpha = targetAlpha;
        currentFade = null;
    }

    private IEnumerator OpenMenuAfterAnimation()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash != ChestOpenHash);

        if (menuController != null)
        {
            menuController.gameObject.SetActive(true);
            menuController.ActivateMenu(true);
        }

        StartFade(0f);
    }
}
