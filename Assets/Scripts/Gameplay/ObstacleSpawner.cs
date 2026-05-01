using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject obstaclePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnDistance = 30f;
    public float laneDistance = 2f;
    public int beatsPerSpawn = 2;

    private int beatCount = 0;

    void OnEnable()
    {
        BeatManager.OnBeat += HandleBeat;
    }

    void OnDisable()
    {
        BeatManager.OnBeat -= HandleBeat;
    }

    void HandleBeat()
    {
        beatCount++;

        if (beatCount >= beatsPerSpawn)
        {
            SpawnObstacle();
            beatCount = 0;
        }
    }

    void SpawnObstacle()
    {
        int lane = Random.Range(0, 3);
        float xPosition = (lane - 1) * laneDistance;

        Vector3 spawnPosition = new Vector3(
            xPosition,
            0.75f,
            player.position.z + spawnDistance
        );

        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}