using System;
using UnityEngine;

public class DynamicDifficultyScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RhythmManager rhythmManager;
    [SerializeField] private BeatObstacleSpawner obstacleSpawner;
    [SerializeField] private DifficultyManager difficultyManager;

    [Header("Scaling Settings")]
    [SerializeField] private bool enableDynamicScaling = true;

    [Tooltip("İlk tempo artışının kaç saniye sonra geleceği.")]
    [SerializeField] private float firstIncreaseDelay = 35f;

    [Tooltip("İlk artıştan sonraki tempo artışları arasındaki süre.")]
    [SerializeField] private float increaseInterval = 35f;

    [Tooltip("Her tempo level sonrası interval biraz uzasın mı? Oyunun sürekli acele etmesini engeller.")]
    [SerializeField] private bool increaseIntervalOverTime = true;

    [SerializeField] private float intervalIncreasePerLevel = 3f;
    [SerializeField] private float maxIncreaseInterval = 55f;

    [Header("Tempo Warning Settings")]
    [SerializeField] private bool useTempoWarning = true;
    [SerializeField] private float warningTimeBeforeIncrease = 4f;

    [Header("Speed Scaling")]
    [SerializeField] private float speedIncreaseAmount = 0.35f;
    [SerializeField] private float easyMaxSpeed = 6.2f;
    [SerializeField] private float normalMaxSpeed = 7.6f;
    [SerializeField] private float hardMaxSpeed = 9.0f;

    [Header("Rhythm Tolerance Scaling")]
    [SerializeField] private float toleranceDecreaseAmount = 0.008f;
    [SerializeField] private float easyMinBeatTolerance = 0.16f;
    [SerializeField] private float normalMinBeatTolerance = 0.11f;
    [SerializeField] private float hardMinBeatTolerance = 0.075f;

    [Header("Obstacle Scaling")]
    [SerializeField] private int reduceSpawnEveryBeatsEveryLevel = 2;
    [SerializeField] private int minimumSpawnEveryBeats = 2;

    [Header("Spawn Distance Scaling")]
    [SerializeField] private float spawnDistanceIncreaseAmount = 1.8f;
    [SerializeField] private float maxSpawnDistanceAhead = 50f;

    [Header("Current Scaling State")]
    [SerializeField] private int difficultyLevel = 0;
    [SerializeField] private float survivedTime = 0f;
    [SerializeField] private float nextIncreaseTime = 0f;
    [SerializeField] private bool warningTriggeredForCurrentLevel = false;

    public event Action<int> OnDynamicDifficultyIncreased;
    public event Action<int, float> OnTempoIncreaseWarning;

    private void Start()
    {
        nextIncreaseTime = firstIncreaseDelay;
        warningTriggeredForCurrentLevel = false;
    }

    private void Update()
    {
        if (!enableDynamicScaling)
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

        survivedTime += Time.deltaTime;

        CheckTempoWarning();

        if (survivedTime >= nextIncreaseTime)
        {
            IncreaseDifficulty();
            ScheduleNextIncrease();
        }
    }

    private void CheckTempoWarning()
    {
        if (!useTempoWarning)
        {
            return;
        }

        if (warningTriggeredForCurrentLevel)
        {
            return;
        }

        float remainingTime = nextIncreaseTime - survivedTime;

        if (remainingTime <= warningTimeBeforeIncrease && remainingTime > 0f)
        {
            warningTriggeredForCurrentLevel = true;

            int nextLevel = difficultyLevel + 1;

            Debug.Log("Tempo artışı yaklaşıyor. Yeni Level: " + nextLevel);

            OnTempoIncreaseWarning?.Invoke(nextLevel, remainingTime);
        }
    }

    private void ScheduleNextIncrease()
    {
        float nextInterval = increaseInterval;

        if (increaseIntervalOverTime)
        {
            nextInterval += difficultyLevel * intervalIncreasePerLevel;
            nextInterval = Mathf.Min(nextInterval, maxIncreaseInterval);
        }

        nextIncreaseTime = survivedTime + nextInterval;
        warningTriggeredForCurrentLevel = false;

        Debug.Log("Sonraki tempo artışı " + nextInterval + " saniye sonra.");
    }

    private void IncreaseDifficulty()
    {
        difficultyLevel++;

        IncreasePlayerSpeed();
        DecreaseBeatTolerance();
        IncreaseObstaclePressure();
        IncreaseSpawnDistance();

        Debug.Log("Dinamik zorluk arttı. Level: " + difficultyLevel);

        OnDynamicDifficultyIncreased?.Invoke(difficultyLevel);
    }

    private void IncreasePlayerSpeed()
    {
        if (playerMovement == null)
        {
            return;
        }

        float currentSpeed = playerMovement.GetForwardSpeed();
        float maxSpeed = GetMaxSpeedByDifficulty();

        float newSpeed = Mathf.Min(
            currentSpeed + speedIncreaseAmount,
            maxSpeed
        );

        playerMovement.SetForwardSpeed(newSpeed);

        Debug.Log("Yeni player hızı: " + newSpeed);
    }

    private void DecreaseBeatTolerance()
    {
        if (rhythmManager == null)
        {
            return;
        }

        float currentTolerance = rhythmManager.GetBeatTolerance();
        float minTolerance = GetMinBeatToleranceByDifficulty();

        float newTolerance = Mathf.Max(
            currentTolerance - toleranceDecreaseAmount,
            minTolerance
        );

        rhythmManager.SetBeatTolerance(newTolerance);

        Debug.Log("Yeni ritim toleransı: " + newTolerance);
    }

    private void IncreaseObstaclePressure()
    {
        if (obstacleSpawner == null)
        {
            return;
        }

        if (difficultyLevel % reduceSpawnEveryBeatsEveryLevel != 0)
        {
            return;
        }

        int currentSpawnEveryBeats = obstacleSpawner.GetSpawnEveryBeats();

        if (currentSpawnEveryBeats <= minimumSpawnEveryBeats)
        {
            return;
        }

        int newSpawnEveryBeats = currentSpawnEveryBeats - 1;

        obstacleSpawner.SetSpawnEveryBeats(newSpawnEveryBeats);

        Debug.Log("Yeni engel spawn aralığı: " + newSpawnEveryBeats + " beat");
    }

    private void IncreaseSpawnDistance()
    {
        if (obstacleSpawner == null)
        {
            return;
        }

        float currentDistance = obstacleSpawner.GetSpawnDistanceAhead();

        float newDistance = Mathf.Min(
            currentDistance + spawnDistanceIncreaseAmount,
            maxSpawnDistanceAhead
        );

        obstacleSpawner.SetSpawnDistanceAhead(newDistance);

        Debug.Log("Yeni engel doğma mesafesi: " + newDistance);
    }

    private float GetMaxSpeedByDifficulty()
    {
        if (difficultyManager == null)
        {
            return normalMaxSpeed;
        }

        switch (difficultyManager.SelectedDifficulty)
        {
            case GameDifficulty.Easy:
                return easyMaxSpeed;

            case GameDifficulty.Hard:
                return hardMaxSpeed;

            default:
                return normalMaxSpeed;
        }
    }

    private float GetMinBeatToleranceByDifficulty()
    {
        if (difficultyManager == null)
        {
            return normalMinBeatTolerance;
        }

        switch (difficultyManager.SelectedDifficulty)
        {
            case GameDifficulty.Easy:
                return easyMinBeatTolerance;

            case GameDifficulty.Hard:
                return hardMinBeatTolerance;

            default:
                return normalMinBeatTolerance;
        }
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }

    public float GetSurvivedTime()
    {
        return survivedTime;
    }

    public float GetNextIncreaseTime()
    {
        return nextIncreaseTime;
    }

    public float GetRemainingTimeToNextIncrease()
    {
        return Mathf.Max(0f, nextIncreaseTime - survivedTime);
    }
}