using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Type Settings")]
    [SerializeField] private string obstacleDisplayName = "Obstacle";
    [SerializeField] private int extraPassedScore = 0;

    [Header("Pass Settings")]
    [SerializeField] private float passDistanceBehindPlayer = 1.5f;

    [Header("Dodge Bonus Settings")]
    [SerializeField] private bool useDodgeBonus = true;
    [SerializeField] private float nearMissDistance = 2.1f;
    [SerializeField] private float perfectDodgeDistance = 1.1f;

    [Header("Warning Visual Settings")]
    [SerializeField] private bool useWarningVisual = true;
    [SerializeField] private float warningDistance = 12f;
    [SerializeField] private float warningPulseScale = 1.15f;
    [SerializeField] private float warningPulseSpeed = 6f;

    [Header("Warning Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private Transform player;
    private bool hasPassedPlayer = false;

    private Renderer obstacleRenderer;
    private Vector3 originalScale;

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Awake()
    {
        originalScale = transform.localScale;

        obstacleRenderer = GetComponent<Renderer>();

        if (obstacleRenderer == null)
        {
            obstacleRenderer = GetComponentInChildren<Renderer>();
        }

        if (obstacleRenderer != null)
        {
            obstacleRenderer.material.color = normalColor;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        UpdateWarningVisual();
        CheckIfPassedPlayer();
    }

    private void UpdateWarningVisual()
    {
        if (!useWarningVisual)
        {
            return;
        }

        float distanceToPlayer = transform.position.z - player.position.z;

        if (distanceToPlayer > 0f && distanceToPlayer <= warningDistance)
        {
            float pulseValue = Mathf.Sin(Time.time * warningPulseSpeed);
            float normalizedPulse = (pulseValue + 1f) / 2f;

            float targetScaleMultiplier = Mathf.Lerp(
                1f,
                warningPulseScale,
                normalizedPulse
            );

            transform.localScale = originalScale * targetScaleMultiplier;

            if (obstacleRenderer != null)
            {
                obstacleRenderer.material.color = Color.Lerp(
                    normalColor,
                    warningColor,
                    normalizedPulse
                );
            }
        }
        else
        {
            transform.localScale = originalScale;

            if (obstacleRenderer != null)
            {
                obstacleRenderer.material.color = normalColor;
            }
        }
    }

    private void CheckIfPassedPlayer()
    {
        if (hasPassedPlayer)
        {
            return;
        }

        if (transform.position.z < player.position.z - passDistanceBehindPlayer)
        {
            hasPassedPlayer = true;

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddObstaclePassedScore(
                    extraPassedScore,
                    obstacleDisplayName
                );

                if (useDodgeBonus)
                {
                    AddDodgeBonusIfNeeded();
                }
            }

            Destroy(gameObject);
        }
    }

    private void AddDodgeBonusIfNeeded()
    {
        float xDistanceToPlayer = Mathf.Abs(transform.position.x - player.position.x);

        if (xDistanceToPlayer <= perfectDodgeDistance)
        {
            ScoreManager.Instance.AddPerfectDodgeBonus();
            Debug.Log("Perfect Dodge! X Distance: " + xDistanceToPlayer);
            return;
        }

        if (xDistanceToPlayer <= nearMissDistance)
        {
            ScoreManager.Instance.AddNearMissBonus();
            Debug.Log("Near Miss! X Distance: " + xDistanceToPlayer);
        }
    }
}