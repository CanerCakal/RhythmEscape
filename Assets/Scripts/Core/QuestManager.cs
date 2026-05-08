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

    [Header("Difficulty Reference")]
    [SerializeField] private DifficultyManager difficultyManager;

    [Header("Dynamic Difficulty Reference")]
    [SerializeField] private DynamicDifficultyScaler dynamicDifficultyScaler;

    [Header("Tempo Quest Scaling")]
    [SerializeField] private float targetIncreasePerTempoLevel = 0.08f;
    [SerializeField] private float rewardIncreasePerTempoLevel = 0.10f;
    [SerializeField] private int maxExtraTargetFromTempo = 20;
    [SerializeField] private int maxExtraRewardFromTempo = 300;

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
        TryFindMissingReferences();
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

        Debug.Log(
    "Yeni rastgele görev: " + currentQuest.questName +
    " | Hedef: " + currentQuest.targetAmount +
    " | Ödül: " + currentQuest.rewardScore +
    " | Tempo Level: " + GetCurrentTempoLevel()
);
        OnQuestUpdated?.Invoke(currentQuest);
    }
    private int GetCurrentTempoLevel()
    {
        if (dynamicDifficultyScaler == null)
        {
            return 0;
        }

        return dynamicDifficultyScaler.GetDifficultyLevel();
    }
    private void TryFindMissingReferences()
    {
        if (difficultyManager == null)
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
        }

        if (dynamicDifficultyScaler == null)
        {
            dynamicDifficultyScaler = FindObjectOfType<DynamicDifficultyScaler>();
        }

        if (player == null)
        {
            PlayerMovement foundPlayerMovement = FindObjectOfType<PlayerMovement>();

            if (foundPlayerMovement != null)
            {
                player = foundPlayerMovement.transform;
            }
        }
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

        targetAmount = ApplyDifficultyToTarget(targetAmount);
        targetAmount = ApplyTempoLevelToTarget(targetAmount);

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

        targetAmount = ApplyDifficultyToTarget(targetAmount);
        targetAmount = ApplyTempoLevelToTarget(targetAmount);

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

        targetAmount = ApplyDifficultyToTarget(targetAmount);
        targetAmount = ApplyTempoLevelToTarget(targetAmount);

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

        if (difficultyManager != null)
        {
            switch (difficultyManager.SelectedDifficulty)
            {
                case GameDifficulty.Easy:
                    calculatedReward = Mathf.RoundToInt(calculatedReward * 0.85f);
                    break;

                case GameDifficulty.Hard:
                    calculatedReward = Mathf.RoundToInt(calculatedReward * 1.35f);
                    break;
            }
        }

        calculatedReward = ApplyTempoLevelToReward(calculatedReward);

        return Mathf.Clamp(
            calculatedReward,
            minimumRewardScore,
            maximumRewardScore + maxExtraRewardFromTempo
        );
    }
    private int ApplyTempoLevelToReward(int baseReward)
    {
        if (dynamicDifficultyScaler == null)
        {
            return baseReward;
        }

        int tempoLevel = dynamicDifficultyScaler.GetDifficultyLevel();

        if (tempoLevel <= 0)
        {
            return baseReward;
        }

        int increasedReward = Mathf.RoundToInt(
            baseReward * (1f + tempoLevel * rewardIncreasePerTempoLevel)
        );

        int extraReward = increasedReward - baseReward;

        if (extraReward > maxExtraRewardFromTempo)
        {
            increasedReward = baseReward + maxExtraRewardFromTempo;
        }

        return increasedReward;
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
    private int ApplyDifficultyToTarget(int baseTarget)
    {
        if (difficultyManager == null)
        {
            return baseTarget;
        }

        switch (difficultyManager.SelectedDifficulty)
        {
            case GameDifficulty.Easy:
                return Mathf.Max(1, Mathf.RoundToInt(baseTarget * 0.8f));

            case GameDifficulty.Hard:
                return Mathf.Max(1, Mathf.RoundToInt(baseTarget * 1.25f));

            default:
                return baseTarget;
        }
    }
    private int ApplyTempoLevelToTarget(int baseTarget)
    {
        if (dynamicDifficultyScaler == null)
        {
            return baseTarget;
        }

        int tempoLevel = dynamicDifficultyScaler.GetDifficultyLevel();

        if (tempoLevel <= 0)
        {
            return baseTarget;
        }

        int increasedTarget = Mathf.RoundToInt(
            baseTarget * (1f + tempoLevel * targetIncreasePerTempoLevel)
        );

        int extraTarget = increasedTarget - baseTarget;

        if (extraTarget > maxExtraTargetFromTempo)
        {
            increasedTarget = baseTarget + maxExtraTargetFromTempo;
        }

        return Mathf.Max(1, increasedTarget);
    }
}