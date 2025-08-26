using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public event Action<int> OnBalanceChanged;

    [SerializeField] private int startingBalance = 0;
    private const string SaveKey = "currency_balance";
    private int balance;

    public int Balance => balance;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        balance += amount;
        OnBalanceChanged?.Invoke(balance);
        Save();
    }

    public bool TrySpend(int cost)
    {
        if (cost < 0) return false;
        if (balance < cost) return false;
        balance -= cost;
        OnBalanceChanged?.Invoke(balance);
        Save();
        return true;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SaveKey, balance);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        balance = PlayerPrefs.HasKey(SaveKey) ? PlayerPrefs.GetInt(SaveKey) : startingBalance;
        OnBalanceChanged?.Invoke(balance);
    }
}
