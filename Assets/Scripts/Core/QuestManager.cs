using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Random Quest Settings")]
    [SerializeField] private float nextQuestDelay = 1.5f;
    [SerializeField] private int minimumRewardScore = 75;
    [SerializeField] private int maximumRewardScore = 250;

    [Header("Distance Quest Settings")]
    [SerializeField] private int minimumDistanceTarget = 100;
    [SerializeField] private int maximumDistanceTarget = 300;

    [Header("Rhythm Quest Settings")]
    [SerializeField] private int minimumCorrectMoveTarget = 5;
    [SerializeField] private int maximumCorrectMoveTarget = 15;

    [Header("Obstacle Quest Settings")]
    [SerializeField] private int minimumObstacleTarget = 3;
    [SerializeField] private int maximumObstacleTarget = 10;

    [Header("Current Quest")]
    [SerializeField] private Quest currentQuest;

    private QuestType lastQuestType;
    private bool hasLastQuestType = false;

    private float questStartZ;
    private int lastDistanceProgress;

    public event Action<Quest> OnQuestUpdated;
    public event Action<Quest> OnQuestCompleted;

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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnObstaclePassed += HandleObstaclePassed;
        }
    }

    private void OnDisable()
    {
        PlayerMovement.OnCorrectRhythmMove -= HandleCorrectRhythmMove;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnObstaclePassed -= HandleObstaclePassed;
        }
    }

    private void Start()
    {
        GenerateNewRandomQuest();
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        TrackDistanceQuest();
    }

    private void GenerateNewRandomQuest()
    {
        QuestType randomQuestType = GetRandomQuestType();

        currentQuest = CreateQuestByType(randomQuestType);

        lastQuestType = randomQuestType;
        hasLastQuestType = true;

        ResetDistanceTracking();

        Debug.Log("Yeni rastgele görev: " + currentQuest.questName);

        OnQuestUpdated?.Invoke(currentQuest);
    }

    private QuestType GetRandomQuestType()
    {
        int questTypeCount = Enum.GetValues(typeof(QuestType)).Length;

        QuestType randomType = (QuestType)UnityEngine.Random.Range(0, questTypeCount);

        if (!hasLastQuestType)
        {
            return randomType;
        }

        int safetyCounter = 0;

        while (randomType == lastQuestType && safetyCounter < 10)
        {
            randomType = (QuestType)UnityEngine.Random.Range(0, questTypeCount);
            safetyCounter++;
        }

        return randomType;
    }

    private Quest CreateQuestByType(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Distance:
                return CreateDistanceQuest();

            case QuestType.CorrectRhythmMoves:
                return CreateCorrectRhythmQuest();

            case QuestType.ObstaclesPassed:
                return CreateObstacleQuest();

            default:
                return CreateDistanceQuest();
        }
    }

    private Quest CreateDistanceQuest()
    {
        int targetAmount = UnityEngine.Random.Range(
            minimumDistanceTarget,
            maximumDistanceTarget + 1
        );

        int rewardScore = CalculateRewardScore(targetAmount, 1);

        return new Quest(
            "Yola Devam",
            targetAmount + " metre ilerle.",
            QuestType.Distance,
            targetAmount,
            rewardScore
        );
    }

    private Quest CreateCorrectRhythmQuest()
    {
        int targetAmount = UnityEngine.Random.Range(
            minimumCorrectMoveTarget,
            maximumCorrectMoveTarget + 1
        );

        int rewardScore = CalculateRewardScore(targetAmount, 12);

        return new Quest(
            "Ritmi Yakala",
            targetAmount + " kez doğru ritimde hareket et.",
            QuestType.CorrectRhythmMoves,
            targetAmount,
            rewardScore
        );
    }

    private Quest CreateObstacleQuest()
    {
        int targetAmount = UnityEngine.Random.Range(
            minimumObstacleTarget,
            maximumObstacleTarget + 1
        );

        int rewardScore = CalculateRewardScore(targetAmount, 20);

        return new Quest(
            "Engelleri Aş",
            targetAmount + " engeli başarıyla geç.",
            QuestType.ObstaclesPassed,
            targetAmount,
            rewardScore
        );
    }

    private int CalculateRewardScore(int targetAmount, int multiplier)
    {
        int calculatedReward = targetAmount * multiplier;

        return Mathf.Clamp(
            calculatedReward,
            minimumRewardScore,
            maximumRewardScore
        );
    }

    private void ResetDistanceTracking()
    {
        if (player != null)
        {
            questStartZ = player.position.z;
        }

        lastDistanceProgress = 0;
    }

    private void TrackDistanceQuest()
    {
        if (currentQuest == null)
        {
            return;
        }

        if (currentQuest.isCompleted)
        {
            return;
        }

        if (currentQuest.questType != QuestType.Distance)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        int currentDistance = Mathf.FloorToInt(player.position.z - questStartZ);

        if (currentDistance <= lastDistanceProgress)
        {
            return;
        }

        int progressDifference = currentDistance - lastDistanceProgress;
        lastDistanceProgress = currentDistance;

        AddProgressToCurrentQuest(progressDifference);
    }

    private void HandleCorrectRhythmMove()
    {
        if (!CanProgressQuest())
        {
            return;
        }

        if (currentQuest.questType != QuestType.CorrectRhythmMoves)
        {
            return;
        }

        AddProgressToCurrentQuest(1);
    }

    private void HandleObstaclePassed()
    {
        if (!CanProgressQuest())
        {
            return;
        }

        if (currentQuest.questType != QuestType.ObstaclesPassed)
        {
            return;
        }

        AddProgressToCurrentQuest(1);
    }

    private bool CanProgressQuest()
    {
        if (currentQuest == null)
        {
            return false;
        }

        if (currentQuest.isCompleted)
        {
            return false;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return false;
        }

        return true;
    }

    private void AddProgressToCurrentQuest(int amount)
    {
        currentQuest.AddProgress(amount);

        OnQuestUpdated?.Invoke(currentQuest);

        if (currentQuest.isCompleted)
        {
            CompleteCurrentQuest();
        }
    }

    private void CompleteCurrentQuest()
    {
        Debug.Log("Görev tamamlandı: " + currentQuest.questName);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddQuestRewardScore(
                currentQuest.rewardScore,
                currentQuest.questName
            );
        }

        OnQuestCompleted?.Invoke(currentQuest);

        Invoke(nameof(GenerateNewRandomQuest), nextQuestDelay);
    }

    public Quest GetCurrentQuest()
    {
        return currentQuest;
    }
}