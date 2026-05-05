using System.Collections;
using UnityEngine;

public class ComboSpeedController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Combo Speed Settings")]
    [SerializeField] private int minComboForBoost = 5;
    [SerializeField] private int comboStepForNextBoost = 5;
    [SerializeField] private float speedIncreasePerStep = 0.5f;
    [SerializeField] private float maxBonusSpeed = 3f;

    [Header("Smooth Settings")]
    [SerializeField] private float speedChangeSmoothness = 4f;

    [Header("Debug Values")]
    [SerializeField] private float baseForwardSpeed;
    [SerializeField] private float currentTargetSpeed;
    [SerializeField] private float currentAppliedSpeed;
    [SerializeField] private int currentBoostLevel;

    private bool isInitialized = false;
    private bool isSubscribed = false;

    private void Start()
    {
        StartCoroutine(InitializeAfterFrame());
    }

    private void OnEnable()
    {
        TrySubscribeToScoreManager();
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null && isSubscribed)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        if (!isSubscribed)
        {
            TrySubscribeToScoreManager();
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        ApplySpeedSmoothly();
    }

    private IEnumerator InitializeAfterFrame()
    {
        yield return null;

        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }

        if (playerMovement == null)
        {
            Debug.LogWarning("ComboSpeedController: PlayerMovement bulunamadı.");
            yield break;
        }

        baseForwardSpeed = playerMovement.GetForwardSpeed();
        currentTargetSpeed = baseForwardSpeed;
        currentAppliedSpeed = baseForwardSpeed;
        currentBoostLevel = 0;

        TrySubscribeToScoreManager();

        isInitialized = true;

        Debug.Log("ComboSpeedController hazır. Base Speed: " + baseForwardSpeed);
    }

    private void TrySubscribeToScoreManager()
    {
        if (isSubscribed)
        {
            return;
        }

        if (ScoreManager.Instance == null)
        {
            return;
        }

        ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
        isSubscribed = true;

        HandleScoreChanged(
            ScoreManager.Instance.Score,
            ScoreManager.Instance.Combo,
            ScoreManager.Instance.HighestCombo
        );

        Debug.Log("ComboSpeedController ScoreManager'a bağlandı.");
    }

    private void HandleScoreChanged(int score, int combo, int highestCombo)
    {
        if (!isInitialized)
        {
            return;
        }

        UpdateTargetSpeed(combo);
    }

    private void UpdateTargetSpeed(int combo)
    {
        if (combo < minComboForBoost)
        {
            currentBoostLevel = 0;
            currentTargetSpeed = baseForwardSpeed;
            return;
        }

        currentBoostLevel = ((combo - minComboForBoost) / comboStepForNextBoost) + 1;

        float bonusSpeed = currentBoostLevel * speedIncreasePerStep;

        if (bonusSpeed > maxBonusSpeed)
        {
            bonusSpeed = maxBonusSpeed;
        }

        currentTargetSpeed = baseForwardSpeed + bonusSpeed;
    }

    private void ApplySpeedSmoothly()
    {
        currentAppliedSpeed = Mathf.Lerp(
            currentAppliedSpeed,
            currentTargetSpeed,
            speedChangeSmoothness * Time.deltaTime
        );

        playerMovement.SetForwardSpeed(currentAppliedSpeed);
    }

    public void ResetBaseSpeed(float newBaseSpeed)
    {
        baseForwardSpeed = newBaseSpeed;
        currentTargetSpeed = baseForwardSpeed;
        currentAppliedSpeed = baseForwardSpeed;
        currentBoostLevel = 0;

        if (playerMovement != null)
        {
            playerMovement.SetForwardSpeed(baseForwardSpeed);
        }
    }
}