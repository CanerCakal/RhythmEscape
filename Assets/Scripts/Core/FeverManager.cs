using System;
using UnityEngine;

public class FeverManager : MonoBehaviour
{
    public static FeverManager Instance;

    [Header("Fever Meter Settings")]
    [SerializeField] private int maxFeverMeter = 100;
    [SerializeField] private int perfectGain = 18;
    [SerializeField] private int goodGain = 7;
    [SerializeField] private int missPenalty = 35;

    [Header("Combo Bonus Settings")]
    [SerializeField] private bool useComboBonusGain = true;
    [SerializeField] private int comboStepForBonus = 5;
    [SerializeField] private int bonusGainPerComboStep = 2;
    [SerializeField] private int maxComboBonusGain = 12;

    [Header("Fever Mode Settings")]
    [SerializeField] private float feverDuration = 8f;
    [SerializeField] private int feverScoreMultiplier = 2;
    [SerializeField] private bool resetMeterAfterFeverEnds = true;

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

    private void Start()
    {
        NotifyFeverMeterChanged();
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
                AddFeverMeter(perfectGain + GetComboBonusGain());
                break;

            case BeatAccuracy.Good:
                AddFeverMeter(goodGain + GetComboBonusGain());
                break;

            case BeatAccuracy.Miss:
                ReduceFeverMeter(missPenalty);
                break;
        }
    }

    private int GetComboBonusGain()
    {
        if (!useComboBonusGain)
        {
            return 0;
        }

        if (ScoreManager.Instance == null)
        {
            return 0;
        }

        int currentCombo = ScoreManager.Instance.Combo;

        if (currentCombo <= 0)
        {
            return 0;
        }

        int comboStep = currentCombo / comboStepForBonus;
        int calculatedBonus = comboStep * bonusGainPerComboStep;

        return Mathf.Clamp(calculatedBonus, 0, maxComboBonusGain);
    }

    private void AddFeverMeter(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

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
        if (amount <= 0)
        {
            return;
        }

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

        if (resetMeterAfterFeverEnds)
        {
            currentFeverMeter = 0;
        }

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

    public float GetFeverDuration()
    {
        return feverDuration;
    }
}