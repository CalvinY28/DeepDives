using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public event Action<int> OnBalanceChanged;

    [SerializeField] private int startingBalance;
    private const string SaveKey = "currency_balance";

    private int balance;
    private bool savePending;

    public int Balance => balance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void Add(int amount)
    {
        if (amount <= 0)
            return;

        balance += amount;
        QueueSave();
        OnBalanceChanged?.Invoke(balance);
    }

    public bool TrySpend(int cost)
    {
        if (cost < 0 || balance < cost)
            return false;

        balance -= cost;
        QueueSave();
        OnBalanceChanged?.Invoke(balance);
        return true;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SaveKey, balance);
        PlayerPrefs.Save();
        savePending = false;
    }

    public void Load()
    {
        balance = PlayerPrefs.HasKey(SaveKey) ? PlayerPrefs.GetInt(SaveKey) : startingBalance;
        savePending = false;
        OnBalanceChanged?.Invoke(balance);
    }

    private void QueueSave()
    {
        PlayerPrefs.SetInt(SaveKey, balance);
        savePending = true;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused && savePending)
            Save();
    }

    private void OnApplicationQuit()
    {
        if (savePending)
            Save();
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        if (savePending)
            Save();

        Instance = null;
    }
}
