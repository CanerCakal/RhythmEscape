using TMPro;
using UnityEngine;

public class RingPowerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ringCountText;
    [SerializeField] private TextMeshProUGUI invincibilityText;

    [Header("Text Settings")]
    [SerializeField] private string ringPrefix = "RINGS ";
    [SerializeField] private string invincibleText = "INVINCIBLE";

    [Header("Animation Settings")]
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float returnSpeed = 8f;

    private bool ringSubscribed = false;
    private bool invincibilitySubscribed = false;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (invincibilityText != null)
        {
            invincibilityText.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        TrySubscribeToRingPowerManager();
        TrySubscribeToInvincibility();
    }

    private void OnEnable()
    {
        TrySubscribeToRingPowerManager();
        TrySubscribeToInvincibility();
    }

    private void OnDisable()
    {
        if (RingPowerManager.Instance != null && ringSubscribed)
        {
            RingPowerManager.Instance.OnRingCountChanged -= UpdateRingText;
            RingPowerManager.Instance.OnRingCollected -= Punch;
            ringSubscribed = false;
        }

        if (PlayerInvincibility.Instance != null && invincibilitySubscribed)
        {
            PlayerInvincibility.Instance.OnInvincibilityStarted -= ShowInvincibility;
            PlayerInvincibility.Instance.OnInvincibilityTimeChanged -= UpdateInvincibilityTime;
            PlayerInvincibility.Instance.OnInvincibilityEnded -= HideInvincibility;
            invincibilitySubscribed = false;
        }
    }

    private void Update()
    {
        if (!ringSubscribed)
        {
            TrySubscribeToRingPowerManager();
        }

        if (!invincibilitySubscribed)
        {
            TrySubscribeToInvincibility();
        }

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale,
            returnSpeed * Time.unscaledDeltaTime
        );
    }

    private void TrySubscribeToRingPowerManager()
    {
        if (ringSubscribed)
        {
            return;
        }

        if (RingPowerManager.Instance == null)
        {
            return;
        }

        RingPowerManager.Instance.OnRingCountChanged += UpdateRingText;
        RingPowerManager.Instance.OnRingCollected += Punch;

        ringSubscribed = true;

        UpdateRingText(
            RingPowerManager.Instance.CurrentRings,
            RingPowerManager.Instance.RingsRequiredForPower
        );
    }

    private void TrySubscribeToInvincibility()
    {
        if (invincibilitySubscribed)
        {
            return;
        }

        if (PlayerInvincibility.Instance == null)
        {
            return;
        }

        PlayerInvincibility.Instance.OnInvincibilityStarted += ShowInvincibility;
        PlayerInvincibility.Instance.OnInvincibilityTimeChanged += UpdateInvincibilityTime;
        PlayerInvincibility.Instance.OnInvincibilityEnded += HideInvincibility;

        invincibilitySubscribed = true;
    }

    private void UpdateRingText(int currentRings, int requiredRings)
    {
        if (ringCountText == null)
        {
            return;
        }

        ringCountText.text = ringPrefix + currentRings + " / " + requiredRings;
    }

    private void ShowInvincibility(float duration)
    {
        if (invincibilityText == null)
        {
            return;
        }

        invincibilityText.gameObject.SetActive(true);
        invincibilityText.text = invincibleText + " " + duration.ToString("0.0") + "s";

        Punch();
    }

    private void UpdateInvincibilityTime(float remainingTime)
    {
        if (invincibilityText == null)
        {
            return;
        }

        if (remainingTime <= 0f)
        {
            invincibilityText.gameObject.SetActive(false);
            return;
        }

        invincibilityText.text = invincibleText + " " + remainingTime.ToString("0.0") + "s";
    }

    private void HideInvincibility()
    {
        if (invincibilityText != null)
        {
            invincibilityText.gameObject.SetActive(false);
        }

        Punch();
    }

    private void Punch()
    {
        transform.localScale = originalScale * punchScale;
    }
}