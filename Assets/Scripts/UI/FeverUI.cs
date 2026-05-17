using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image feverFillImage;
    [SerializeField] private TextMeshProUGUI feverText;
    [SerializeField] private TextMeshProUGUI feverPercentText;

    [Header("Text Settings")]
    [SerializeField] private string normalText = "FEVER";
    [SerializeField] private string activeText = "FEVER MODE";

    [Header("Animation Settings")]
    [SerializeField] private float fillLerpSpeed = 8f;
    [SerializeField] private float activePulseScale = 1.08f;
    [SerializeField] private float activePulseSpeed = 6f;

    [Header("Color Settings")]
    [SerializeField] private Color normalFillColor = Color.cyan;
    [SerializeField] private Color activeFillColor = Color.yellow;

    private float targetFillAmount = 0f;
    private bool isSubscribed = false;
    private Vector3 originalScale;
    private Image panelImage;

    private void Awake()
    {
        originalScale = transform.localScale;
        panelImage = GetComponent<Image>();

        SetupVerticalFillImage();
        UpdateText(false);
        UpdatePercentText(0f);
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
        UpdateActiveTimerText();
    }

    private void SetupVerticalFillImage()
    {
        if (feverFillImage == null)
        {
            return;
        }

        feverFillImage.type = Image.Type.Filled;
        feverFillImage.fillMethod = Image.FillMethod.Vertical;
        feverFillImage.fillOrigin = 0;
        feverFillImage.fillAmount = 0f;
        feverFillImage.color = normalFillColor;
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
        UpdatePercentText(targetFillAmount);

        Debug.Log("FeverUI FeverManager'a bağlandı.");
    }

    private void HandleFeverMeterChanged(int currentValue, int maxValue)
    {
        if (maxValue <= 0)
        {
            targetFillAmount = 0f;
            UpdatePercentText(0f);
            return;
        }

        targetFillAmount = (float)currentValue / maxValue;
        UpdatePercentText(targetFillAmount);
    }

    private void HandleFeverStarted()
    {
        targetFillAmount = 1f;

        if (feverFillImage != null)
        {
            feverFillImage.color = activeFillColor;
        }

        UpdateText(true);
        UpdatePercentText(1f);
    }

    private void HandleFeverEnded()
    {
        targetFillAmount = 0f;

        if (feverFillImage != null)
        {
            feverFillImage.color = normalFillColor;
        }

        UpdateText(false);
        UpdatePercentText(0f);

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

    private void UpdateText(bool isFeverActive)
    {
        if (feverText == null)
        {
            return;
        }

        feverText.text = isFeverActive ? activeText : normalText;
    }

    private void UpdatePercentText(float normalizedValue)
    {
        if (feverPercentText == null)
        {
            return;
        }

        int percent = Mathf.RoundToInt(normalizedValue * 100f);
        feverPercentText.text = percent + "%";
    }

    private void UpdateActiveTimerText()
    {
        if (FeverManager.Instance == null)
        {
            return;
        }

        if (!FeverManager.Instance.IsFeverActive)
        {
            return;
        }

        if (feverPercentText == null)
        {
            return;
        }

        float remainingTime = FeverManager.Instance.GetRemainingFeverTime();
        feverPercentText.text = remainingTime.ToString("0.0") + "s";
    }
}