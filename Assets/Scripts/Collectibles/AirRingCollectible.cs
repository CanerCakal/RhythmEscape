using UnityEngine;

public class AirRingCollectible : MonoBehaviour
{
    [Header("Ring Settings")]
    [SerializeField] private int scoreReward = 35;
    [SerializeField] private bool destroyAfterCollect = true;

    [Header("Visual Settings")]
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float floatAmplitude = 0.12f;
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color collectedColor = Color.cyan;

    private Renderer ringRenderer;
    private Vector3 startPosition;
    private bool isCollected = false;

    private void Awake()
    {
        startPosition = transform.position;

        ringRenderer = GetComponent<Renderer>();

        if (ringRenderer == null)
        {
            ringRenderer = GetComponentInChildren<Renderer>();
        }

        ApplyColor(normalColor);
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

        RotateRing();
        FloatRing();
    }

    private void RotateRing()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    private void FloatRing()
    {
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        Vector3 newPosition = startPosition;
        newPosition.y += offsetY;

        transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
        {
            return;
        }

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            playerMovement = other.GetComponentInParent<PlayerMovement>();
        }

        if (playerMovement == null)
        {
            return;
        }

        CollectRing();
    }

    private void CollectRing()
    {
        isCollected = true;

        ApplyColor(collectedColor);

        if (RingPowerManager.Instance != null)
        {
            RingPowerManager.Instance.AddRing();
        }
        else
        {
            Debug.LogWarning("RingPowerManager sahnede bulunamadı.");
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddRhythmGateBonus(scoreReward);
        }

        Debug.Log("Air Ring toplandı. Bonus Score: +" + scoreReward);

        if (destroyAfterCollect)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    private void ApplyColor(Color color)
    {
        if (ringRenderer != null)
        {
            ringRenderer.material.color = color;
        }
    }
}