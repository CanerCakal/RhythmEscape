using System.Collections.Generic;
using UnityEngine;

public class RoadSegmentManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject roadSegmentPrefab;

    [Header("Road Settings")]
    [SerializeField] private float segmentLength = 30f;
    [SerializeField] private int initialSegmentCount = 6;
    [SerializeField] private float spawnAheadDistance = 90f;
    [SerializeField] private float cleanupDistanceBehindPlayer = 45f;

    private readonly List<GameObject> activeSegments = new List<GameObject>();
    private float nextSpawnZ = 0f;

    private void Start()
    {
        if (player == null || roadSegmentPrefab == null)
        {
            Debug.LogWarning("RoadSegmentManager: Player veya Road Segment Prefab atanmamış.");
            return;
        }

        CreateInitialRoad();
    }

    private void Update()
    {
        if (player == null || roadSegmentPrefab == null)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        SpawnRoadIfNeeded();
        CleanupOldSegments();
    }

    private void CreateInitialRoad()
    {
        nextSpawnZ = -segmentLength;

        for (int i = 0; i < initialSegmentCount; i++)
        {
            SpawnSegment();
        }
    }

    private void SpawnRoadIfNeeded()
    {
        while (player.position.z + spawnAheadDistance > nextSpawnZ)
        {
            SpawnSegment();
        }
    }

    private void SpawnSegment()
    {
        Vector3 spawnPosition = new Vector3(
            0f,
            -0.5f,
            nextSpawnZ
        );

        GameObject newSegment = Instantiate(
            roadSegmentPrefab,
            spawnPosition,
            Quaternion.identity
        );

        activeSegments.Add(newSegment);

        nextSpawnZ += segmentLength;
    }

    private void CleanupOldSegments()
    {
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            GameObject segment = activeSegments[i];

            if (segment == null)
            {
                activeSegments.RemoveAt(i);
                continue;
            }

            float segmentEndZ = segment.transform.position.z + (segmentLength / 2f);

            if (segmentEndZ < player.position.z - cleanupDistanceBehindPlayer)
            {
                activeSegments.RemoveAt(i);
                Destroy(segment);
            }
        }
    }
}