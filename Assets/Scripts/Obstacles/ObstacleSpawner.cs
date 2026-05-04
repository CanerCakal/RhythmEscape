using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform player;

    public float spawnDistanceAhead = 30f;
    public float laneDistance = 2f;

    public float spawnInterval = 1f;

    private float timer;

    void Update()
    {
        if (player == null || obstaclePrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        int randomLane = Random.Range(0, 3);
        float xPosition = (randomLane - 1) * laneDistance;

        Vector3 spawnPosition = new Vector3(
            xPosition,
            0.5f,
            player.position.z + spawnDistanceAhead
        );

        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}