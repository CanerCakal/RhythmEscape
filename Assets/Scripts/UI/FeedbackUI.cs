using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackUI : MonoBehaviour
{
    [Header("Text Reference")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Message Settings")]
    [SerializeField] private string perfectMessage = "Perfect!";
    [SerializeField] private string missMessage = "Miss!";

    [Header("Color Settings")]
    [SerializeField] private Color perfectColor = Color.green;
    [SerializeField] private Color missColor = Color.red;

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 0.35f;
    [SerializeField] private float startScale = 1.5f;
    [SerializeField] private float endScale = 1f;

    private Coroutine feedbackCoroutine;

    private void Awake()
    {
        HideFeedback();
    }

    private void OnEnable()
    {
        PlayerMovement.OnCorrectRhythmMove += ShowPerfect;
        PlayerMovement.OnWrongRhythmInput += ShowMiss;
    }

    private void OnDisable()
    {
        PlayerMovement.OnCorrectRhythmMove -= ShowPerfect;
        PlayerMovement.OnWrongRhythmInput -= ShowMiss;
    }

    private void ShowPerfect()
    {
        ShowFeedback(perfectMessage, perfectColor);
    }

    private void ShowMiss()
    {
        ShowFeedback(missMessage, missColor);
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null)
        {
            Debug.LogWarning("FeedbackUI: Feedback Text atanmamış.");
            return;
        }

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }

        feedbackCoroutine = StartCoroutine(FeedbackRoutine(message, color));
    }

    private IEnumerator FeedbackRoutine(string message, Color color)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;

        float timer = 0f;

        while (timer < showDuration)
        {
            timer += Time.deltaTime;

            float t = timer / showDuration;

            float currentScale = Mathf.Lerp(startScale, endScale, t);
            feedbackText.transform.localScale = Vector3.one * currentScale;

            Color currentColor = feedbackText.color;
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            feedbackText.color = currentColor;

            yield return null;
        }

        HideFeedback();
    }

    private void HideFeedback()
    {
        if (feedbackText == null)
        {
            return;
        }

        feedbackText.gameObject.SetActive(false);
        feedbackText.transform.localScale = Vector3.one;
    }
}