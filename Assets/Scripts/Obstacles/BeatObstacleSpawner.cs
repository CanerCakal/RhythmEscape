using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleSpawnData
{
    public int beatIndex;

    [Range(0, 2)]
    public int laneIndex;
}

public class BeatObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistanceAhead = 35f;
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float obstacleYPosition = 0.5f;

    [Header("Spawn Mode")]
    [SerializeField] private bool usePatternSpawning = true;

    [Header("Pattern Spawn Settings")]
    [SerializeField] private List<ObstacleSpawnData> obstaclePattern = new List<ObstacleSpawnData>();

    [Header("Random Spawn Settings")]
    [SerializeField] private int firstSpawnBeat = 4;
    [SerializeField] private int spawnEveryBeats = 4;
    [SerializeField] private bool avoidSameLaneTwice = true;

    private bool isSubscribed = false;
    private int lastLaneIndex = -1;

    private void Start()
    {
        TrySubscribeToRhythmManager();
    }

    private void OnEnable()
    {
        TrySubscribeToRhythmManager();
    }

    private void OnDisable()
    {
        if (RhythmManager.Instance != null && isSubscribed)
        {
            RhythmManager.Instance.OnBeat -= HandleBeat;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToRhythmManager();
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

    private void HandleBeat(int beatIndex)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (obstaclePrefab == null || player == null)
        {
            Debug.LogWarning("BeatObstacleSpawner: Obstacle prefab veya Player atanmamış.");
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
        for (int i = 0; i < obstaclePattern.Count; i++)
        {
            ObstacleSpawnData spawnData = obstaclePattern[i];

            if (spawnData.beatIndex == beatIndex)
            {
                SpawnObstacleAtLane(spawnData.laneIndex, beatIndex);
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
        SpawnObstacleAtLane(randomLane, beatIndex);
    }

    private void SpawnObstacleAtLane(int laneIndex, int beatIndex)
    {
        laneIndex = Mathf.Clamp(laneIndex, 0, 2);

        float xPosition = (laneIndex - 1) * laneDistance;

        Vector3 spawnPosition = new Vector3(
            xPosition,
            obstacleYPosition,
            player.position.z + spawnDistanceAhead
        );

        GameObject spawnedObstacle = Instantiate(
            obstaclePrefab,
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

        Debug.Log("Obstacle spawn edildi. Beat: " + beatIndex + " Lane: " + laneIndex);
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
    public void SetSpawnDistanceAhead(float newDistance)
{
    spawnDistanceAhead = newDistance;
}
}