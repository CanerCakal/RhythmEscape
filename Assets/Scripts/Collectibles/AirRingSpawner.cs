using UnityEngine;

public class AirRingSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private bool enableRingSpawning = true;
    [SerializeField] private int firstRingBeat = 12;
    [SerializeField] private int ringEveryBeats = 20;
    [SerializeField] private float spawnDistanceAhead = 32f;
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float ringYPosition = 2.7f;

    [Header("Random Settings")]
    [SerializeField] private bool useRandomLane = true;
    [SerializeField] private int fixedLaneIndex = 1;
    [SerializeField] private bool avoidSameLaneTwice = true;

    private bool isSubscribed = false;
    private int lastLaneIndex = -1;

    private void Start()
    {
        TryFindPlayer();
        TrySubscribeToRhythmManager();
    }

    private void OnEnable()
    {
        TryFindPlayer();
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

        if (player == null)
        {
            TryFindPlayer();
        }
    }

    private void TryFindPlayer()
    {
        if (player != null)
        {
            return;
        }

        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement != null)
        {
            player = playerMovement.transform;
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

        Debug.Log("AirRingSpawner RhythmManager'a bağlandı.");
    }

    private void HandleBeat(int beatIndex)
    {
        if (!enableRingSpawning)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (ringPrefab == null || player == null)
        {
            return;
        }

        if (beatIndex < firstRingBeat)
        {
            return;
        }

        if ((beatIndex - firstRingBeat) % ringEveryBeats != 0)
        {
            return;
        }

        SpawnRing(beatIndex);
    }

    private void SpawnRing(int beatIndex)
    {
        int laneIndex = GetLaneIndex();
        float xPosition = (laneIndex - 1) * laneDistance;

        Vector3 spawnPosition = new Vector3(
            xPosition,
            ringYPosition,
            player.position.z + spawnDistanceAhead
        );

        Instantiate(
            ringPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Debug.Log("Air Ring spawn edildi. Beat: " + beatIndex + " Lane: " + laneIndex);
    }

    private int GetLaneIndex()
    {
        if (!useRandomLane)
        {
            return Mathf.Clamp(fixedLaneIndex, 0, 2);
        }

        int laneIndex = Random.Range(0, 3);

        if (avoidSameLaneTwice && laneIndex == lastLaneIndex)
        {
            laneIndex = (laneIndex + 1) % 3;
        }

        lastLaneIndex = laneIndex;

        return laneIndex;
    }

    public void SetRingEveryBeats(int newValue)
    {
        ringEveryBeats = Mathf.Max(1, newValue);
    }

    public void SetSpawnDistanceAhead(float newValue)
    {
        spawnDistanceAhead = Mathf.Max(5f, newValue);
    }
}