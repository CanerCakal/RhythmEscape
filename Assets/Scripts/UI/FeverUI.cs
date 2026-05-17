using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image feverFillImage;
    [SerializeField] private TextMeshProUGUI feverText;

    [Header("Text Settings")]
    [SerializeField] private string normalText = "FEVER";
    [SerializeField] private string activeText = "FEVER MODE!";

    [Header("Animation Settings")]
    [SerializeField] private float fillLerpSpeed = 8f;
    [SerializeField] private float activePulseScale = 1.08f;
    [SerializeField] private float activePulseSpeed = 6f;

    private RectTransform rectTransform;
    private float targetFillAmount = 0f;
    private bool isSubscribed = false;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = transform.localScale;

        if (feverFillImage != null)
        {
            feverFillImage.type = Image.Type.Filled;
            feverFillImage.fillMethod = Image.FillMethod.Horizontal;
            feverFillImage.fillOrigin = 0;
            feverFillImage.fillAmount = 0f;
        }

        if (feverText != null)
        {
            feverText.text = normalText;
        }
    }

    private void Start()
    {
        TrySubscribeToFeverManager();
    }

    private void OnEnable()
    {
        TrySubscribeToFeverManager();
    }

    private void OnDisable()
    {
        if (FeverManager.Instance != null && isSubscribed)
        {
            FeverManager.Instance.OnFeverMeterChanged -= HandleFeverMeterChanged;
            FeverManager.Instance.OnFeverStarted -= HandleFeverStarted;
            FeverManager.Instance.OnFeverEnded -= HandleFeverEnded;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToFeverManager();
        }

        UpdateFillVisual();
        UpdatePulseVisual();
    }

    private void TrySubscribeToFeverManager()
    {
        if (isSubscribed)
        {
            return;
        }

        if (FeverManager.Instance == null)
        {
            return;
        }

        FeverManager.Instance.OnFeverMeterChanged += HandleFeverMeterChanged;
        FeverManager.Instance.OnFeverStarted += HandleFeverStarted;
        FeverManager.Instance.OnFeverEnded += HandleFeverEnded;

        isSubscribed = true;

        targetFillAmount = FeverManager.Instance.GetFeverNormalizedValue();

        Debug.Log("FeverUI FeverManager'a bağlandı.");
    }

    private void HandleFeverMeterChanged(int currentValue, int maxValue)
    {
        if (maxValue <= 0)
        {
            targetFillAmount = 0f;
            return;
        }

        targetFillAmount = (float)currentValue / maxValue;
    }

    private void HandleFeverStarted()
    {
        targetFillAmount = 1f;

        if (feverText != null)
        {
            feverText.text = activeText;
        }
    }

    private void HandleFeverEnded()
    {
        targetFillAmount = 0f;

        if (feverText != null)
        {
            feverText.text = normalText;
        }

        transform.localScale = originalScale;
    }

    private void UpdateFillVisual()
    {
        if (feverFillImage == null)
        {
            return;
        }

        feverFillImage.fillAmount = Mathf.Lerp(
            feverFillImage.fillAmount,
            targetFillAmount,
            fillLerpSpeed * Time.deltaTime
        );
    }

    private void UpdatePulseVisual()
    {
        if (FeverManager.Instance == null)
        {
            transform.localScale = originalScale;
            return;
        }

        if (!FeverManager.Instance.IsFeverActive)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale,
                fillLerpSpeed * Time.deltaTime
            );

            return;
        }

        float pulse = Mathf.Sin(Time.time * activePulseSpeed);
        float normalizedPulse = (pulse + 1f) / 2f;

        float scale = Mathf.Lerp(1f, activePulseScale, normalizedPulse);

        transform.localScale = originalScale * scale;
    }
}