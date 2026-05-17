using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuVisualController : MonoBehaviour
{
    [Header("Title Animation")]
    [SerializeField] private RectTransform titleTransform;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private float titlePulseScale = 1.035f;
    [SerializeField] private float titlePulseSpeed = 2.2f;

    [Header("Subtitle Animation")]
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private float subtitleFadeSpeed = 1.8f;
    [SerializeField] private float subtitleMinAlpha = 0.45f;
    [SerializeField] private float subtitleMaxAlpha = 1f;

    [Header("Background Lines")]
    [SerializeField] private RectTransform[] beatLines;
    [SerializeField] private float lineMoveAmount = 18f;
    [SerializeField] private float lineMoveSpeed = 0.7f;

    [Header("Glow")]
    [SerializeField] private RectTransform glowTransform;
    [SerializeField] private Image glowImage;
    [SerializeField] private float glowPulseScale = 1.12f;
    [SerializeField] private float glowPulseSpeed = 1.4f;
    [SerializeField] private float glowMinAlpha = 0.08f;
    [SerializeField] private float glowMaxAlpha = 0.22f;

    private Vector3 titleOriginalScale;
    private Vector3 glowOriginalScale;
    private Vector2[] beatLineStartPositions;

    private void Awake()
    {
        if (titleTransform != null)
        {
            titleOriginalScale = titleTransform.localScale;
        }

        if (glowTransform != null)
        {
            glowOriginalScale = glowTransform.localScale;
        }

        CacheBeatLinePositions();
    }

    private void Update()
    {
        AnimateTitle();
        AnimateSubtitle();
        AnimateBeatLines();
        AnimateGlow();
    }

    private void CacheBeatLinePositions()
    {
        if (beatLines == null)
        {
            return;
        }

        beatLineStartPositions = new Vector2[beatLines.Length];

        for (int i = 0; i < beatLines.Length; i++)
        {
            if (beatLines[i] != null)
            {
                beatLineStartPositions[i] = beatLines[i].anchoredPosition;
            }
        }
    }

    private void AnimateTitle()
    {
        if (titleTransform == null)
        {
            return;
        }

        float pulse = Mathf.Sin(Time.time * titlePulseSpeed);
        float normalizedPulse = (pulse + 1f) / 2f;

        float scale = Mathf.Lerp(1f, titlePulseScale, normalizedPulse);

        titleTransform.localScale = titleOriginalScale * scale;
    }

    private void AnimateSubtitle()
    {
        if (subtitleText == null)
        {
            return;
        }

        float pulse = Mathf.Sin(Time.time * subtitleFadeSpeed);
        float normalizedPulse = (pulse + 1f) / 2f;

        float alpha = Mathf.Lerp(subtitleMinAlpha, subtitleMaxAlpha, normalizedPulse);

        Color color = subtitleText.color;
        color.a = alpha;
        subtitleText.color = color;
    }

    private void AnimateBeatLines()
    {
        if (beatLines == null || beatLineStartPositions == null)
        {
            return;
        }

        for (int i = 0; i < beatLines.Length; i++)
        {
            if (beatLines[i] == null)
            {
                continue;
            }

            float offset = Mathf.Sin((Time.time * lineMoveSpeed) + i) * lineMoveAmount;

            beatLines[i].anchoredPosition = beatLineStartPositions[i] + new Vector2(offset, 0f);
        }
    }

    private void AnimateGlow()
    {
        if (glowTransform == null)
        {
            return;
        }

        float pulse = Mathf.Sin(Time.time * glowPulseSpeed);
        float normalizedPulse = (pulse + 1f) / 2f;

        float scale = Mathf.Lerp(1f, glowPulseScale, normalizedPulse);
        glowTransform.localScale = glowOriginalScale * scale;

        if (glowImage != null)
        {
            Color color = glowImage.color;
            color.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, normalizedPulse);
            glowImage.color = color;
        }
    }
}