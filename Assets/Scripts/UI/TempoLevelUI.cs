using System.Collections;
using TMPro;
using UnityEngine;

public class TempoLevelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DynamicDifficultyScaler dynamicDifficultyScaler;
    [SerializeField] private TextMeshProUGUI tempoLevelText;

    [Header("Message Settings")]
    [SerializeField] private string titlePrefix = "TEMPO LEVEL ";
    [SerializeField] private string subtitle = "Tempo yükseldi!";

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 1.4f;
    [SerializeField] private float startScale = 1.35f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float moveUpDistance = 40f;

    [Header("Visual Settings")]
    [SerializeField] private Color messageColor = Color.yellow;

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
        isSubscribed = true;

        Debug.Log("TempoLevelUI DynamicDifficultyScaler'a bağlandı.");
    }

    private void ShowTempoLevel(int level)
    {
        Debug.Log("TempoLevelUI mesaj gösteriyor. Level: " + level);

        if (tempoLevelText == null)
        {
            Debug.LogWarning("TempoLevelUI: Tempo Level Text atanmamış.");
            return;
        }

        if (tempoCoroutine != null)
        {
            StopCoroutine(tempoCoroutine);
        }

        tempoCoroutine = StartCoroutine(TempoLevelRoutine(level));
    }

    private IEnumerator TempoLevelRoutine(int level)
    {
        tempoLevelText.gameObject.SetActive(true);

        tempoLevelText.text = titlePrefix + level + "\n" + subtitle;

        tempoLevelText.transform.localPosition = originalPosition;
        tempoLevelText.transform.localScale = originalScale * startScale;

        Color startColor = messageColor;
        startColor.a = 1f;
        tempoLevelText.color = startColor;

        Vector3 startPosition = originalPosition;
        Vector3 endPosition = originalPosition + new Vector3(0f, moveUpDistance, 0f);

        float timer = 0f;

        while (timer < showDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / showDuration;

            tempoLevelText.transform.localPosition = Vector3.Lerp(
                startPosition,
                endPosition,
                t
            );

            tempoLevelText.transform.localScale = Vector3.Lerp(
                originalScale * startScale,
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

        Color resetColor = messageColor;
        resetColor.a = 1f;
        tempoLevelText.color = resetColor;
    }
}