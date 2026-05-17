using System.Collections;
using TMPro;
using UnityEngine;

public class TempoLevelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DynamicDifficultyScaler dynamicDifficultyScaler;
    [SerializeField] private TextMeshProUGUI tempoLevelText;

    [Header("Level Up Message Settings")]
    [SerializeField] private string titlePrefix = "TEMPO LEVEL ";
    [SerializeField] private string subtitle = "Speed up!";

    [Header("Warning Message Settings")]
    [SerializeField] private string warningTitle = "TEMPO RISING";
    [SerializeField] private string warningSubtitle = "Get ready!";

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 1.6f;
    [SerializeField] private float warningShowDuration = 1.2f;
    [SerializeField] private float startScale = 1.45f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float moveUpDistance = 45f;

    [Header("Visual Settings")]
    [SerializeField] private Color levelUpColor = Color.yellow;
    [SerializeField] private Color warningColor = Color.cyan;

    private bool isSubscribed = false;

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private Coroutine tempoCoroutine;

    private void Awake()
    {
        if (tempoLevelText == null)
        {
            tempoLevelText = GetComponent<TextMeshProUGUI>();
        }

        if (tempoLevelText != null)
        {
            originalPosition = tempoLevelText.transform.localPosition;
            originalScale = tempoLevelText.transform.localScale;

            tempoLevelText.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        TrySubscribeToDynamicDifficultyScaler();
    }

    private void OnEnable()
    {
        TrySubscribeToDynamicDifficultyScaler();
    }

    private void OnDisable()
    {
        if (dynamicDifficultyScaler != null && isSubscribed)
        {
            dynamicDifficultyScaler.OnDynamicDifficultyIncreased -= ShowTempoLevel;
            dynamicDifficultyScaler.OnTempoIncreaseWarning -= ShowTempoWarning;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToDynamicDifficultyScaler();
        }
    }

    private void TrySubscribeToDynamicDifficultyScaler()
    {
        if (isSubscribed)
        {
            return;
        }

        if (dynamicDifficultyScaler == null)
        {
            dynamicDifficultyScaler = FindObjectOfType<DynamicDifficultyScaler>();
        }

        if (dynamicDifficultyScaler == null)
        {
            return;
        }

        dynamicDifficultyScaler.OnDynamicDifficultyIncreased += ShowTempoLevel;
        dynamicDifficultyScaler.OnTempoIncreaseWarning += ShowTempoWarning;

        isSubscribed = true;

        Debug.Log("TempoLevelUI DynamicDifficultyScaler'a bağlandı.");
    }

    private void ShowTempoWarning(int nextLevel, float remainingTime)
    {
        if (tempoLevelText == null)
        {
            return;
        }

        string message = warningTitle + "\n" + warningSubtitle;

        PlayMessage(
            message,
            warningColor,
            warningShowDuration,
            startScale * 0.92f
        );
    }

    private void ShowTempoLevel(int level)
    {
        if (tempoLevelText == null)
        {
            Debug.LogWarning("TempoLevelUI: Tempo Level Text atanmamış.");
            return;
        }

        string message = titlePrefix + level + "\n" + subtitle;

        PlayMessage(
            message,
            levelUpColor,
            showDuration,
            startScale
        );
    }

    private void PlayMessage(string message, Color color, float duration, float scale)
    {
        if (tempoCoroutine != null)
        {
            StopCoroutine(tempoCoroutine);
        }

        tempoCoroutine = StartCoroutine(MessageRoutine(message, color, duration, scale));
    }

    private IEnumerator MessageRoutine(string message, Color color, float duration, float scale)
    {
        tempoLevelText.gameObject.SetActive(true);

        tempoLevelText.text = message;

        tempoLevelText.transform.localPosition = originalPosition;
        tempoLevelText.transform.localScale = originalScale * scale;

        Color startColor = color;
        startColor.a = 1f;
        tempoLevelText.color = startColor;

        Vector3 startPosition = originalPosition;
        Vector3 endPosition = originalPosition + new Vector3(0f, moveUpDistance, 0f);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / duration;

            tempoLevelText.transform.localPosition = Vector3.Lerp(
                startPosition,
                endPosition,
                t
            );

            tempoLevelText.transform.localScale = Vector3.Lerp(
                originalScale * scale,
                originalScale * endScale,
                t
            );

            Color currentColor = tempoLevelText.color;
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            tempoLevelText.color = currentColor;

            yield return null;
        }

        tempoLevelText.transform.localPosition = originalPosition;
        tempoLevelText.transform.localScale = originalScale;
        tempoLevelText.gameObject.SetActive(false);

        Color resetColor = color;
        resetColor.a = 1f;
        tempoLevelText.color = resetColor;
    }
}