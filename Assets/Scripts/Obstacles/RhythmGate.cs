using UnityEngine;

public class RhythmGate : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private int successBonusScore = 75;
    [SerializeField] private bool gameOverOnMiss = true;
    [SerializeField] private bool destroyAfterTrigger = true;

    [Header("Visual Feedback")]
    [SerializeField] private bool usePulse = true;
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color missColor = Color.red;

    private Renderer gateRenderer;
    private Vector3 originalScale;
    private bool isTriggered = false;

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

        bool isOnBeat = RhythmManager.Instance != null && RhythmManager.Instance.IsOnBeat();

        if (isOnBeat)
        {
            HandleSuccess();
        }
        else
        {
            HandleMiss();
        }
    }

    private void HandleSuccess()
    {
        ApplyColor(activeColor);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddRhythmGateBonus(successBonusScore);
        }

        Debug.Log("Rhythm Gate başarıyla geçildi. Bonus: +" + successBonusScore);

        if (destroyAfterTrigger)
        {
            Destroy(gameObject, 0.15f);
        }
    }

    private void HandleMiss()
    {
        ApplyColor(missColor);

        Debug.Log("Rhythm Gate yanlış zamanda geçildi.");

        if (gameOverOnMiss)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ResetComboWithPenalty();
            }
        }
    }

    private void UpdatePulseVisual()
    {
        if (!usePulse)
        {
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
}