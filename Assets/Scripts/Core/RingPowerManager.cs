using System;
using UnityEngine;

public class RingPowerManager : MonoBehaviour
{
    public static RingPowerManager Instance;

    [Header("Ring Power Settings")]
    [SerializeField] private int ringsRequiredForPower = 3;
    [SerializeField] private bool resetRingsAfterPowerStarts = true;

    [Header("Current State")]
    [SerializeField] private int currentRings = 0;

    public event Action<int, int> OnRingCountChanged;
    public event Action OnRingCollected;
    public event Action OnRingPowerReady;
    public event Action OnRingPowerActivated;

    public int CurrentRings => currentRings;
    public int RingsRequiredForPower => ringsRequiredForPower;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        NotifyRingCountChanged();
    }

    public void AddRing()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        currentRings++;

        if (currentRings > ringsRequiredForPower)
        {
            currentRings = ringsRequiredForPower;
        }

        Debug.Log("Ring toplandı: " + currentRings + " / " + ringsRequiredForPower);

        OnRingCollected?.Invoke();
        NotifyRingCountChanged();

        if (currentRings >= ringsRequiredForPower)
        {
            OnRingPowerReady?.Invoke();
            ActivateRingPower();
        }
    }

    private void ActivateRingPower()
    {
        if (PlayerInvincibility.Instance != null)
        {
            PlayerInvincibility.Instance.StartInvincibility();
        }
        else
        {
            Debug.LogWarning("PlayerInvincibility sahnede bulunamadı.");
        }

        if (resetRingsAfterPowerStarts)
        {
            currentRings = 0;
            NotifyRingCountChanged();
        }

        OnRingPowerActivated?.Invoke();

        Debug.Log("Ring Power aktif edildi.");
    }

    private void NotifyRingCountChanged()
    {
        OnRingCountChanged?.Invoke(currentRings, ringsRequiredForPower);
    }
}