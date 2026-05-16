using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public const string BestScoreKey = "BestScore";

    [Header("Score Settings")]
    [SerializeField] private int correctMoveScore = 10;
    [SerializeField] private int comboBonusPerMove = 2;
    [SerializeField] private int wrongInputPenalty = 5;

    [Header("Obstacle Score Settings")]
    [SerializeField] private int obstaclePassedScore = 25;
    [SerializeField] private int obstacleComboBonus = 5;

    [Header("Dodge Bonus Settings")]
    [SerializeField] private int nearMissBonusScore = 25;
    [SerializeField] private int perfectDodgeBonusScore = 50;

    [Header("Current Values")]
    [SerializeField] private int score = 0;
    [SerializeField] private int combo = 0;
    [SerializeField] private int highestCombo = 0;

    private bool isNewBestScore = false;

    public event Action<int, int, int> OnScoreChanged;
    public event Action<string, int> OnDodgeBonusAwarded;
    public event Action OnObstaclePassed;
    public int Score => score;
    public int Combo => combo;
    public int HighestCombo => highestCombo;
    public int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);
    public bool IsNewBestScore => isNewBestScore;

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
        PlayerMovement.OnCorrectRhythmMove += HandleCorrectRhythmMove;
        PlayerMovement.OnWrongRhythmInput += HandleWrongRhythmInput;
    }

    private void OnDisable()
    {
        PlayerMovement.OnCorrectRhythmMove -= HandleCorrectRhythmMove;
        PlayerMovement.OnWrongRhythmInput -= HandleWrongRhythmInput;
    }

    private void Start()
    {
        NotifyScoreChanged();
    }

    private void HandleCorrectRhythmMove()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        combo++;

        if (combo > highestCombo)
        {
            highestCombo = combo;
        }

        int earnedScore = correctMoveScore + (combo * comboBonusPerMove);
        score += earnedScore;

        Debug.Log("Doğru ritim hareketi: +" + earnedScore + " | Combo: " + combo);

        NotifyScoreChanged();
    }

    private void HandleWrongRhythmInput()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        combo = 0;
        score -= wrongInputPenalty;

        if (score < 0)
        {
            score = 0;
        }

        Debug.Log("Yanlış giriş! Combo sıfırlandı. Ceza: -" + wrongInputPenalty);

        NotifyScoreChanged();
    }

    public void AddObstaclePassedScore()
    {
        AddObstaclePassedScore(0, "Obstacle");
    }

    public void AddObstaclePassedScore(int extraScore, string obstacleDisplayName)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        int earnedScore = obstaclePassedScore + (combo * obstacleComboBonus) + extraScore;
        score += earnedScore;

        Debug.Log(obstacleDisplayName + " geçildi: +" + earnedScore);

        NotifyScoreChanged();

        OnObstaclePassed?.Invoke();

        if (extraScore > 0)
        {
            OnDodgeBonusAwarded?.Invoke(obstacleDisplayName.ToUpper() + " BONUS", extraScore);
        }
    }

    public void AddNearMissBonus()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        score += nearMissBonusScore;

        Debug.Log("Near Miss Bonus: +" + nearMissBonusScore);

        NotifyScoreChanged();
        OnDodgeBonusAwarded?.Invoke("NEAR MISS", nearMissBonusScore);
    }

    public void AddPerfectDodgeBonus()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        score += perfectDodgeBonusScore;

        Debug.Log("Perfect Dodge Bonus: +" + perfectDodgeBonusScore);

        NotifyScoreChanged();
        OnDodgeBonusAwarded?.Invoke("PERFECT DODGE", perfectDodgeBonusScore);
    }
    public void AddQuestRewardScore(int rewardScore, string questName)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (rewardScore <= 0)
        {
            return;
        }

        score += rewardScore;

        Debug.Log("Görev ödülü kazanıldı: " + questName + " +" + rewardScore);

        NotifyScoreChanged();
    }

    public void SaveBestScoreIfNeeded()
    {
        int currentBestScore = BestScore;

        if (score > currentBestScore)
        {
            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();

            isNewBestScore = true;

            Debug.Log("Yeni en yüksek skor: " + score);
        }
        else
        {
            isNewBestScore = false;
        }
    }

    public void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(BestScoreKey);
        PlayerPrefs.Save();

        isNewBestScore = false;

        Debug.Log("Best Score sıfırlandı.");
    }

    public void AddRhythmGateBonus(int bonusScore)
{
    if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
    {
        return;
    }

    if (bonusScore <= 0)
    {
        return;
    }

    score += bonusScore;
    combo++;

    if (combo > highestCombo)
    {
        highestCombo = combo;
    }

    Debug.Log("Rhythm Gate Bonus: +" + bonusScore + " | Combo: " + combo);

    NotifyScoreChanged();
    OnDodgeBonusAwarded?.Invoke("RHYTHM GATE", bonusScore);
}

public void ResetComboWithPenalty()
{
    if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
    {
        return;
    }

    combo = 0;
    score -= wrongInputPenalty;

    if (score < 0)
    {
        score = 0;
    }

    Debug.Log("Rhythm Gate kaçırıldı. Combo sıfırlandı.");

    NotifyScoreChanged();
}

    private void NotifyScoreChanged()
    {
        OnScoreChanged?.Invoke(score, combo, highestCombo);
    }
}