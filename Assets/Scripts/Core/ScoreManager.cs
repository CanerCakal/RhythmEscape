using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    [SerializeField] private int correctMoveScore = 10;
    [SerializeField] private int comboBonusPerMove = 2;
    [SerializeField] private int wrongInputPenalty = 5;

    [Header("Obstacle Score Settings")]
    [SerializeField] private int obstaclePassedScore = 25;
    [SerializeField] private int obstacleComboBonus = 5;

    [Header("Current Values")]
    [SerializeField] private int score = 0;
    [SerializeField] private int combo = 0;
    [SerializeField] private int highestCombo = 0;

    public event Action<int, int, int> OnScoreChanged;

    public int Score => score;
    public int Combo => combo;
    public int HighestCombo => highestCombo;

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
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        int earnedScore = obstaclePassedScore + (combo * obstacleComboBonus);
        score += earnedScore;

        Debug.Log("Engel başarıyla geçildi: +" + earnedScore);

        NotifyScoreChanged();
    }

    private void NotifyScoreChanged()
    {
        OnScoreChanged?.Invoke(score, combo, highestCombo);
    }
}