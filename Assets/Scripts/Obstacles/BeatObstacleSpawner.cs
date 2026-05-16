using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleSpawnData
{
    public int beatIndex;

    [Range(0, 2)]
    public int laneIndex;

    [Header("Optional Obstacle Type")]
    public bool useSpecificObstacleType = false;
    public int obstacleTypeIndex = 0;
}

[System.Serializable]
public class ObstacleTypeData
{
    public string obstacleName;
    public GameObject obstaclePrefab;

    [Range(1, 100)]
    public int spawnWeight = 10;

    public int minComboToSpawn = 0;
}

public class BeatObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform player;

    [Header("Obstacle Types")]
    [SerializeField] private bool useMultipleObstacleTypes = true;
    [SerializeField] private List<ObstacleTypeData> obstacleTypes = new List<ObstacleTypeData>();

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistanceAhead = 35f;
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float obstacleYPosition = 0.5f;

    [Header("Spawn Mode")]
    [SerializeField] private bool usePatternSpawning = true;

    [Header("Pattern Spawn Settings")]
    [SerializeField] private List<ObstacleSpawnData> obstaclePattern = new List<ObstacleSpawnData>();

    [Header("Infinite Spawn Settings")]
    [SerializeField] private bool continueRandomAfterPatternEnds = true;
    [SerializeField] private bool repeatPatternAfterEnd = false;

    [Header("Random Spawn Settings")]
    [SerializeField] private int firstSpawnBeat = 4;
    [SerializeField] private int spawnEveryBeats = 4;
    [SerializeField] private bool avoidSameLaneTwice = true;

    [Header("Rhythm Gate Spawn Settings")]
    [SerializeField] private bool enableRhythmGateSpawning = true;
    [SerializeField] private GameObject rhythmGatePrefab;
    [SerializeField] private int firstRhythmGateBeat = 16;
    [SerializeField] private int rhythmGateEveryBeats = 24;
    [SerializeField] private int minimumComboForRhythmGate = 0;
    [SerializeField] private float rhythmGateYPosition = 0.9f;
    [SerializeField] private bool blockNormalObstacleOnGateBeat = true;

    private bool isSubscribed = false;
    private int lastLaneIndex = -1;
    private int lastPatternBeatIndex = -1;
    private int currentCombo = 0;
    private bool scoreSubscribed = false;

    private void Start()
    {
        CalculateLastPatternBeatIndex();
        TrySubscribeToRhythmManager();
        TrySubscribeToScoreManager();
    }

    private void OnEnable()
    {
        CalculateLastPatternBeatIndex();
        TrySubscribeToRhythmManager();
        TrySubscribeToScoreManager();
    }

    private void OnDisable()
    {
        if (RhythmManager.Instance != null && isSubscribed)
        {
            RhythmManager.Instance.OnBeat -= HandleBeat;
            isSubscribed = false;
        }

        if (ScoreManager.Instance != null && scoreSubscribed)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            scoreSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToRhythmManager();
        }

        if (!scoreSubscribed)
        {
            TrySubscribeToScoreManager();
        }
    }

    private void TrySubscribeToRhythmManager()
    {
        if (isSubscribed)
        {
            return;
        }

        if (RhythmManager.Instance == null)
        {
            return;
        }

        RhythmManager.Instance.OnBeat += HandleBeat;
        isSubscribed = true;

        Debug.Log("BeatObstacleSpawner RhythmManager'a bağlandı.");
    }

    private void TrySubscribeToScoreManager()
    {
        if (scoreSubscribed)
        {
            return;
        }

        if (ScoreManager.Instance == null)
        {
            return;
        }

        ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
        scoreSubscribed = true;

        currentCombo = ScoreManager.Instance.Combo;
    }

    private void HandleScoreChanged(int score, int combo, int highestCombo)
    {
        currentCombo = combo;
    }

    private void HandleBeat(int beatIndex)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("BeatObstacleSpawner: Player atanmamış.");
            return;
        }

        bool spawnedRhythmGate = TrySpawnRhythmGate(beatIndex);

        if (spawnedRhythmGate && blockNormalObstacleOnGateBeat)
        {
            return;
        }

        if (GetFallbackObstaclePrefab() == null)
        {
            Debug.LogWarning("BeatObstacleSpawner: Obstacle prefab atanmamış.");
            return;
        }

        if (usePatternSpawning)
        {
            SpawnFromPattern(beatIndex);
        }
        else
        {
            SpawnRandomlyByBeat(beatIndex);
        }
    }

    private void SpawnFromPattern(int beatIndex)
    {
        if (obstaclePattern == null || obstaclePattern.Count == 0)
        {
            SpawnRandomlyByBeat(beatIndex);
            return;
        }

        bool spawnedFromPattern = false;

        int checkedBeatIndex = beatIndex;

        if (repeatPatternAfterEnd && lastPatternBeatIndex > 0)
        {
            checkedBeatIndex = beatIndex % (lastPatternBeatIndex + 1);
        }

        for (int i = 0; i < obstaclePattern.Count; i++)
        {
            ObstacleSpawnData spawnData = obstaclePattern[i];

            if (spawnData.beatIndex == checkedBeatIndex)
            {
                GameObject selectedPrefab = null;

                if (spawnData.useSpecificObstacleType)
                {
                    selectedPrefab = GetObstaclePrefabByIndex(spawnData.obstacleTypeIndex);
                }

                if (selectedPrefab == null)
                {
                    selectedPrefab = GetRandomObstaclePrefab();
                }

                SpawnObstacleAtLane(spawnData.laneIndex, beatIndex, selectedPrefab);
                spawnedFromPattern = true;
            }
        }

        if (!spawnedFromPattern && continueRandomAfterPatternEnds)
        {
            if (beatIndex > lastPatternBeatIndex)
            {
                SpawnRandomlyByBeat(beatIndex);
            }
        }
    }

    private void SpawnRandomlyByBeat(int beatIndex)
    {
        if (beatIndex < firstSpawnBeat)
        {
            return;
        }

        if ((beatIndex - firstSpawnBeat) % spawnEveryBeats != 0)
        {
            return;
        }

        int randomLane = GetRandomLaneIndex();
        GameObject selectedPrefab = GetRandomObstaclePrefab();

        SpawnObstacleAtLane(randomLane, beatIndex, selectedPrefab);
    }

    private bool TrySpawnRhythmGate(int beatIndex)
    {
        if (!enableRhythmGateSpawning)
        {
            return false;
        }

        if (rhythmGatePrefab == null)
        {
            return false;
        }

        if (beatIndex < firstRhythmGateBeat)
        {
            return false;
        }

        if ((beatIndex - firstRhythmGateBeat) % rhythmGateEveryBeats != 0)
        {
            return false;
        }

        if (currentCombo < minimumComboForRhythmGate)
        {
            return false;
        }

        SpawnRhythmGate(beatIndex);
        return true;
    }

    private void SpawnRhythmGate(int beatIndex)
    {
        Vector3 spawnPosition = new Vector3(
            0f,
            rhythmGateYPosition,
            player.position.z + spawnDistanceAhead
        );

        GameObject spawnedGate = Instantiate(
            rhythmGatePrefab,
            spawnPosition,
            Quaternion.identity
        );

        RhythmGate rhythmGate = spawnedGate.GetComponent<RhythmGate>();

        if (rhythmGate != null)
        {
            rhythmGate.Initialize(player);
        }

        Debug.Log("Rhythm Gate spawn edildi. Beat: " + beatIndex);
    }

    private void SpawnObstacleAtLane(int laneIndex, int beatIndex, GameObject selectedObstaclePrefab)
    {
        laneIndex = Mathf.Clamp(laneIndex, 0, 2);

        if (selectedObstaclePrefab == null)
        {
            selectedObstaclePrefab = GetFallbackObstaclePrefab();
        }

        if (selectedObstaclePrefab == null)
        {
            Debug.LogWarning("BeatObstacleSpawner: Spawn edilecek obstacle prefab bulunamadı.");
            return;
        }

        float xPosition = (laneIndex - 1) * laneDistance;

        Vector3 spawnPosition = new Vector3(
            xPosition,
            obstacleYPosition,
            player.position.z + spawnDistanceAhead
        );

        GameObject spawnedObstacle = Instantiate(
            selectedObstaclePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Obstacle obstacle = spawnedObstacle.GetComponent<Obstacle>();

        if (obstacle != null)
        {
            obstacle.Initialize(player);
        }
        else
        {
            Debug.LogWarning("Spawn edilen obstacle üzerinde Obstacle.cs yok.");
        }

        Debug.Log("Obstacle spawn edildi. Beat: " + beatIndex + " Lane: " + laneIndex + " Prefab: " + selectedObstaclePrefab.name);
    }

    private GameObject GetRandomObstaclePrefab()
    {
        if (!useMultipleObstacleTypes || obstacleTypes == null || obstacleTypes.Count == 0)
        {
            return GetFallbackObstaclePrefab();
        }

        int totalWeight = 0;

        for (int i = 0; i < obstacleTypes.Count; i++)
        {
            ObstacleTypeData obstacleType = obstacleTypes[i];

            if (obstacleType == null)
            {
                continue;
            }

            if (obstacleType.obstaclePrefab == null)
            {
                continue;
            }

            if (currentCombo < obstacleType.minComboToSpawn)
            {
                continue;
            }

            totalWeight += obstacleType.spawnWeight;
        }

        if (totalWeight <= 0)
        {
            return GetFallbackObstaclePrefab();
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        for (int i = 0; i < obstacleTypes.Count; i++)
        {
            ObstacleTypeData obstacleType = obstacleTypes[i];

            if (obstacleType == null)
            {
                continue;
            }

            if (obstacleType.obstaclePrefab == null)
            {
                continue;
            }

            if (currentCombo < obstacleType.minComboToSpawn)
            {
                continue;
            }

            currentWeight += obstacleType.spawnWeight;

            if (randomWeight < currentWeight)
            {
                return obstacleType.obstaclePrefab;
            }
        }

        return GetFallbackObstaclePrefab();
    }

    private GameObject GetObstaclePrefabByIndex(int obstacleTypeIndex)
    {
        if (obstacleTypes == null || obstacleTypes.Count == 0)
        {
            return GetFallbackObstaclePrefab();
        }

        if (obstacleTypeIndex < 0 || obstacleTypeIndex >= obstacleTypes.Count)
        {
            return GetFallbackObstaclePrefab();
        }

        if (obstacleTypes[obstacleTypeIndex] == null)
        {
            return GetFallbackObstaclePrefab();
        }

        if (obstacleTypes[obstacleTypeIndex].obstaclePrefab == null)
        {
            return GetFallbackObstaclePrefab();
        }

        return obstacleTypes[obstacleTypeIndex].obstaclePrefab;
    }

    private GameObject GetFallbackObstaclePrefab()
    {
        if (obstaclePrefab != null)
        {
            return obstaclePrefab;
        }

        if (obstacleTypes != null && obstacleTypes.Count > 0)
        {
            for (int i = 0; i < obstacleTypes.Count; i++)
            {
                if (obstacleTypes[i] != null && obstacleTypes[i].obstaclePrefab != null)
                {
                    return obstacleTypes[i].obstaclePrefab;
                }
            }
        }

        return null;
    }

    private int GetRandomLaneIndex()
    {
        int laneIndex = Random.Range(0, 3);

        if (avoidSameLaneTwice && laneIndex == lastLaneIndex)
        {
            laneIndex = (laneIndex + 1) % 3;
        }

        lastLaneIndex = laneIndex;
        return laneIndex;
    }

    private void CalculateLastPatternBeatIndex()
    {
        lastPatternBeatIndex = -1;

        if (obstaclePattern == null)
        {
            return;
        }

        for (int i = 0; i < obstaclePattern.Count; i++)
        {
            if (obstaclePattern[i].beatIndex > lastPatternBeatIndex)
            {
                lastPatternBeatIndex = obstaclePattern[i].beatIndex;
            }
        }
    }

    public void SetSpawnDistanceAhead(float newDistance)
    {
        spawnDistanceAhead = newDistance;
    }
    public float GetSpawnDistanceAhead()
    {
        return spawnDistanceAhead;
    }
    public void SetFirstSpawnBeat(int newFirstSpawnBeat)
    {
        firstSpawnBeat = Mathf.Max(0, newFirstSpawnBeat);
    }

    public void SetSpawnEveryBeats(int newSpawnEveryBeats)
    {
        spawnEveryBeats = Mathf.Max(1, newSpawnEveryBeats);
    }
    public int GetSpawnEveryBeats()
    {
        return spawnEveryBeats;
    }

    public void SetAvoidSameLaneTwice(bool value)
    {
        avoidSameLaneTwice = value;
    }

    public void SetUsePatternSpawning(bool value)
    {
        usePatternSpawning = value;
    }

    public void SetContinueRandomAfterPatternEnds(bool value)
    {
        continueRandomAfterPatternEnds = value;
    }

    public void SetRepeatPatternAfterEnd(bool value)
    {
        repeatPatternAfterEnd = value;
    }

    public void SetRhythmGateEveryBeats(int newRhythmGateEveryBeats)
    {
        rhythmGateEveryBeats = Mathf.Max(1, newRhythmGateEveryBeats);
    }

    public void SetMinimumComboForRhythmGate(int newMinimumCombo)
    {
        minimumComboForRhythmGate = Mathf.Max(0, newMinimumCombo);
    }

    public void SetRhythmGateGameOverOnMiss(bool value)
    {
        if (rhythmGatePrefab == null)
        {
            return;
        }

        RhythmGate rhythmGate = rhythmGatePrefab.GetComponent<RhythmGate>();

        if (rhythmGate != null)
        {
            rhythmGate.SetGameOverOnMiss(value);
        }
    }
}