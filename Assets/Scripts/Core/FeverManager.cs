using System;
using UnityEngine;

public class FeverManager : MonoBehaviour
{
    public static FeverManager Instance;

    [Header("Fever Settings")]
    [SerializeField] private int maxFeverMeter = 100;
    [SerializeField] private int perfectGain = 20;
    [SerializeField] private int goodGain = 8;
    [SerializeField] private int missPenalty = 30;
    [SerializeField] private float feverDuration = 8f;
    [SerializeField] private int feverScoreMultiplier = 2;

    [Header("Current State")]
    [SerializeField] private int currentFeverMeter = 0;
    [SerializeField] private bool isFeverActive = false;
    [SerializeField] private float feverTimer = 0f;

    public event Action<int, int> OnFeverMeterChanged;
    public event Action OnFeverStarted;
    public event Action OnFeverEnded;

    public bool IsFeverActive => isFeverActive;
    public int FeverScoreMultiplier => isFeverActive ? feverScoreMultiplier : 1;
    public int CurrentFeverMeter => currentFeverMeter;
    public int MaxFeverMeter => maxFeverMeter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        PlayerMovement.OnRhythmInputJudged += HandleRhythmInputJudged;
    }

    private void OnDisable()
    {
        PlayerMovement.OnRhythmInputJudged -= HandleRhythmInputJudged;
    }

    private void Update()
    {
        if (!isFeverActive)
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        feverTimer -= Time.deltaTime;

        if (feverTimer <= 0f)
        {
            EndFever();
        }
    }

    private void HandleRhythmInputJudged(BeatAccuracy beatAccuracy)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (isFeverActive)
        {
            return;
        }

        switch (beatAccuracy)
        {
            case BeatAccuracy.Perfect:
                AddFeverMeter(perfectGain);
                break;

            case BeatAccuracy.Good:
                AddFeverMeter(goodGain);
                break;

            case BeatAccuracy.Miss:
                ReduceFeverMeter(missPenalty);
                break;
        }
    }

    private void AddFeverMeter(int amount)
    {
        currentFeverMeter += amount;

        if (currentFeverMeter >= maxFeverMeter)
        {
            currentFeverMeter = maxFeverMeter;
            NotifyFeverMeterChanged();
            StartFever();
            return;
        }

        NotifyFeverMeterChanged();
    }

    private void ReduceFeverMeter(int amount)
    {
        currentFeverMeter -= amount;

        if (currentFeverMeter < 0)
        {
            currentFeverMeter = 0;
        }

        NotifyFeverMeterChanged();
    }

    private void StartFever()
    {
        if (isFeverActive)
        {
            return;
        }

        isFeverActive = true;
        feverTimer = feverDuration;

        Debug.Log("FEVER MODE BAŞLADI!");

        OnFeverStarted?.Invoke();
    }

    private void EndFever()
    {
        if (!isFeverActive)
        {
            return;
        }

        isFeverActive = false;
        feverTimer = 0f;
        currentFeverMeter = 0;

        Debug.Log("FEVER MODE BİTTİ.");

        NotifyFeverMeterChanged();
        OnFeverEnded?.Invoke();
    }

    private void NotifyFeverMeterChanged()
    {
        OnFeverMeterChanged?.Invoke(currentFeverMeter, maxFeverMeter);
    }

    public float GetFeverNormalizedValue()
    {
        if (maxFeverMeter <= 0)
        {
            return 0f;
        }

        return (float)currentFeverMeter / maxFeverMeter;
    }

    public float GetRemainingFeverTime()
    {
        return feverTimer;
    }
}