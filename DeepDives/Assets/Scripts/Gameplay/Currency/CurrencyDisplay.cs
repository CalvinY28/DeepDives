using UnityEngine;
using TMPro;
using System.Collections;

public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    void Awake()
    {
        // Auto-grab the TMP on this object if not assigned
        if (!label) label = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        StartCoroutine(WaitAndHook());
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnBalanceChanged -= UpdateLabel;
    }

    private IEnumerator WaitAndHook()
    {
        // Wait until the manager exists (handles load-order issues)
        while (CurrencyManager.Instance == null) yield return null;

        CurrencyManager.Instance.OnBalanceChanged += UpdateLabel;
        UpdateLabel(CurrencyManager.Instance.Balance); // force initial draw
    }

    private void UpdateLabel(int value)
    {
        if (label) label.text = $"{value}";
    }
}
