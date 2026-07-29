using System.Collections;
using FMODUnity;
using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public int amount = 1;
    public AudioClip pickupSfx;
    public EventReference coinPickup;

    [Header("Collection Animation")]
    [SerializeField, Min(0.05f)] private float animationDuration = 0.45f;
    [SerializeField, Min(0f)] private float jumpHeight = 0.6f;
    [SerializeField] private float spinDegrees = 360f;
    [SerializeField, Range(0f, 0.9f)] private float fadeStart = 0.2f;

    private Collider2D[] pickupColliders;
    private SpriteRenderer[] spriteRenderers;
    private Color[] startingColors;
    private bool collected;

    private void Awake()
    {
        pickupColliders = GetComponents<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        startingColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
            startingColors[i] = spriteRenderers[i].color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;

        for (int i = 0; i < pickupColliders.Length; i++)
            pickupColliders[i].enabled = false;

        CurrencyManager.Instance?.Add(amount);

        if (pickupSfx != null)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position);

        RuntimeManager.PlayOneShot(coinPickup, transform.position);
        StartCoroutine(PlayCollectionAnimation());
    }

    private IEnumerator PlayCollectionAnimation()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            float jumpOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            float fadeProgress = Mathf.InverseLerp(fadeStart, 1f, progress);

            transform.position = startPosition + Vector3.up * jumpOffset;
            transform.rotation = startRotation * Quaternion.Euler(0f, 0f, spinDegrees * easedProgress);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color color = startingColors[i];
                color.a *= 1f - fadeProgress;
                spriteRenderers[i].color = color;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
