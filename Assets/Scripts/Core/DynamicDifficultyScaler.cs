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
    [SerializeField] private float increaseInterval = 20f;

    [Header("Speed Scaling")]
    [SerializeField] private float speedIncreaseAmount = 0.25f;
    [SerializeField] private float easyMaxSpeed = 6.2f;
    [SerializeField] private float normalMaxSpeed = 7.6f;
    [SerializeField] private float hardMaxSpeed = 9.0f;

    [Header("Rhythm Tolerance Scaling")]
    [SerializeField] private float toleranceDecreaseAmount = 0.01f;
    [SerializeField] private float easyMinBeatTolerance = 0.16f;
    [SerializeField] private float normalMinBeatTolerance = 0.11f;
    [SerializeField] private float hardMinBeatTolerance = 0.075f;

    [Header("Obstacle Scaling")]
    [SerializeField] private int reduceSpawnEveryBeatsEveryLevel = 2;
    [SerializeField] private int minimumSpawnEveryBeats = 2;

    [Header("Current Scaling State")]
    [SerializeField] private int difficultyLevel = 0;
    [SerializeField] private float survivedTime = 0f;

    private float nextIncreaseTime;

    private void Start()
    {
        nextIncreaseTime = increaseInterval;
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

        if (survivedTime >= nextIncreaseTime)
        {
            IncreaseDifficulty();
            nextIncreaseTime += increaseInterval;
        }
    }

    private void IncreaseDifficulty()
    {
        difficultyLevel++;

        IncreasePlayerSpeed();
        DecreaseBeatTolerance();
        IncreaseObstaclePressure();

        Debug.Log("Dinamik zorluk arttı. Level: " + difficultyLevel);
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
}