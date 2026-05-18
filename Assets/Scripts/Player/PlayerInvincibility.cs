using System;
using UnityEngine;

public class PlayerInvincibility : MonoBehaviour
{
    public static PlayerInvincibility Instance;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 6f;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color invincibleColor = Color.cyan;
    [SerializeField] private float pulseSpeed = 8f;

    [Header("Current State")]
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float remainingTime = 0f;

    public event Action<float> OnInvincibilityStarted;
    public event Action<float> OnInvincibilityTimeChanged;
    public event Action OnInvincibilityEnded;

    public bool IsInvincible => isInvincible;
    public float RemainingTime => remainingTime;
    public float InvincibilityDuration => invincibilityDuration;

    private Material playerMaterial;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }

        if (playerRenderer != null)
        {
            playerMaterial = playerRenderer.material;
            normalColor = playerMaterial.color;
        }
    }

    private void Update()
    {
        if (!isInvincible)
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            EndInvincibility();
            return;
        }

        UpdateInvincibleVisual();

        OnInvincibilityTimeChanged?.Invoke(remainingTime);
    }

    public void StartInvincibility()
    {
        isInvincible = true;
        remainingTime = invincibilityDuration;

        Debug.Log("Dokunulmazlık başladı. Süre: " + invincibilityDuration);

        OnInvincibilityStarted?.Invoke(invincibilityDuration);
        OnInvincibilityTimeChanged?.Invoke(remainingTime);
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        remainingTime = 0f;

        ResetVisual();

        Debug.Log("Dokunulmazlık bitti.");

        OnInvincibilityTimeChanged?.Invoke(remainingTime);
        OnInvincibilityEnded?.Invoke();
    }

    private void UpdateInvincibleVisual()
    {
        if (playerMaterial == null)
        {
            return;
        }

        float pulse = Mathf.Sin(Time.time * pulseSpeed);
        float normalizedPulse = (pulse + 1f) / 2f;

        playerMaterial.color = Color.Lerp(
            normalColor,
            invincibleColor,
            normalizedPulse
        );
    }

    private void ResetVisual()
    {
        if (playerMaterial == null)
        {
            return;
        }

        playerMaterial.color = normalColor;
    }
}