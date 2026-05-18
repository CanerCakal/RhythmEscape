using UnityEngine;

public class RhythmGate : MonoBehaviour
{
    [Header("Bonus Gate Settings")]
    [SerializeField] private int bonusScore = 100;
    [SerializeField] private bool destroyAfterTrigger = true;
    [SerializeField] private float destroyDelay = 0.2f;

    [Header("Visual Feedback")]
    [SerializeField] private bool usePulse = true;
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color collectedColor = Color.yellow;

    private Renderer gateRenderer;
    private Vector3 originalScale;
    private bool isTriggered = false;
    private Transform player;

    private void Awake()
    {
        originalScale = transform.localScale;

        gateRenderer = GetComponent<Renderer>();

        if (gateRenderer == null)
        {
            gateRenderer = GetComponentInChildren<Renderer>();
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

        UpdatePulseVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered)
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

        isTriggered = true;

        CollectBonus();
    }

    private void CollectBonus()
    {
        ApplyColor(collectedColor);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddRhythmGateBonus(bonusScore);
        }

        Debug.Log("Bonus Gate toplandı. Bonus: +" + bonusScore);

        if (destroyAfterTrigger)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void UpdatePulseVisual()
    {
        if (!usePulse)
        {
            return;
        }

        if (isTriggered)
        {
            transform.localScale = originalScale * pulseScale;
            return;
        }

        float pulseValue = Mathf.Sin(Time.time * pulseSpeed);
        float normalizedPulse = (pulseValue + 1f) / 2f;

        float scaleMultiplier = Mathf.Lerp(
            1f,
            pulseScale,
            normalizedPulse
        );

        transform.localScale = originalScale * scaleMultiplier;
    }

    private void ApplyColor(Color color)
    {
        if (gateRenderer != null)
        {
            gateRenderer.material.color = color;
        }
    }

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetBonusScore(int newBonusScore)
    {
        bonusScore = Mathf.Max(0, newBonusScore);
    }
}