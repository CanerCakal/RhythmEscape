using System.Collections;
using UnityEngine;

public class PlayerVisualFeedback : MonoBehaviour
{
    [Header("Scale Feedback")]
    [SerializeField] private float correctScale = 1.15f;
    [SerializeField] private float scaleDuration = 0.15f;

    [Header("Color Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private float colorDuration = 0.15f;

    private Vector3 originalScale;
    private Renderer playerRenderer;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
        playerRenderer = GetComponent<Renderer>();

        if (playerRenderer != null)
        {
            playerRenderer.material.color = normalColor;
        }
    }

    private void OnEnable()
    {
        PlayerMovement.OnCorrectRhythmMove += PlayCorrectFeedback;
        PlayerMovement.OnWrongRhythmInput += PlayWrongFeedback;
    }

    private void OnDisable()
    {
        PlayerMovement.OnCorrectRhythmMove -= PlayCorrectFeedback;
        PlayerMovement.OnWrongRhythmInput -= PlayWrongFeedback;
    }

    private void PlayCorrectFeedback()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleFeedbackRoutine());
    }

    private void PlayWrongFeedback()
    {
        if (playerRenderer == null)
        {
            return;
        }

        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }

        colorCoroutine = StartCoroutine(ColorFeedbackRoutine());
    }

    private IEnumerator ScaleFeedbackRoutine()
    {
        transform.localScale = originalScale * correctScale;

        float timer = 0f;

        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;

            float t = timer / scaleDuration;

            transform.localScale = Vector3.Lerp(
                originalScale * correctScale,
                originalScale,
                t
            );

            yield return null;
        }

        transform.localScale = originalScale;
    }

    private IEnumerator ColorFeedbackRoutine()
    {
        playerRenderer.material.color = wrongColor;

        float timer = 0f;

        while (timer < colorDuration)
        {
            timer += Time.deltaTime;

            float t = timer / colorDuration;

            playerRenderer.material.color = Color.Lerp(
                wrongColor,
                normalColor,
                t
            );

            yield return null;
        }

        playerRenderer.material.color = normalColor;
    }
}