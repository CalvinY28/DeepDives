using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public int amount = 1;
    public AudioClip pickupSfx;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CurrencyManager.Instance?.Add(amount);
        if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        Destroy(gameObject);
    }
}
