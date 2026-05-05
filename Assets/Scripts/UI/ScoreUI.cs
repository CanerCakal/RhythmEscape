using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI highestComboText;

    [Header("Speed Boost Text")]
    [SerializeField] private TextMeshProUGUI speedBoostText;

    [Header("Combo Animation Settings")]
    [SerializeField] private float comboPunchScale = 1.25f;
    [SerializeField] private float comboPunchDuration = 0.15f;

    [Header("Speed Boost Settings")]
    [SerializeField] private int minComboForSpeedBoostText = 5;
    [SerializeField] private float speedBoostTextDuration = 0.6f;

    [Header("Colors")]
    [SerializeField] private Color normalComboColor = Color.white;
    [SerializeField] private Color boostedComboColor = Color.cyan;
    [SerializeField] private Color wrongComboColor = Color.red;

    private bool isSubscribed = false;

    private int previousCombo = 0;

    private Vector3 comboOriginalScale;

    private Coroutine comboPunchCoroutine;
    private Coroutine speedBoostCoroutine;
    private Coroutine wrongComboCoroutine;

    private void Awake()
    {
        if (comboText != null)
        {
            comboOriginalScale = comboText.transform.localScale;
            comboText.color = normalComboColor;
        }

        if (speedBoostText != null)
        {
            speedBoostText.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        TrySubscribeToScoreManager();
        UpdateUI(0, 0, 0);
    }

    private void OnEnable()
    {
        TrySubscribeToScoreManager();
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null && isSubscribed)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateUI;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToScoreManager();
        }
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

        ScoreManager.Instance.OnScoreChanged += UpdateUI;
        isSubscribed = true;

        UpdateUI(
            ScoreManager.Instance.Score,
            ScoreManager.Instance.Combo,
            ScoreManager.Instance.HighestCombo
        );

        Debug.Log("ScoreUI ScoreManager'a bağlandı.");
    }

    private void UpdateUI(int score, int combo, int highestCombo)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (comboText != null)
        {
            comboText.text = "Combo: x" + combo;
            UpdateComboColor(combo);
            HandleComboFeedback(combo);
        }

        if (highestComboText != null)
        {
            highestComboText.text = "Best Combo: x" + highestCombo;
        }

        previousCombo = combo;
    }

    private void UpdateComboColor(int combo)
    {
        if (comboText == null)
        {
            return;
        }

        if (combo >= minComboForSpeedBoostText)
        {
            comboText.color = boostedComboColor;
        }
        else
        {
            comboText.color = normalComboColor;
        }
    }

    private void HandleComboFeedback(int combo)
    {
        if (combo > previousCombo)
        {
            PlayComboPunch();

            if (combo >= minComboForSpeedBoostText && combo % minComboForSpeedBoostText == 0)
            {
                PlaySpeedBoostText();
            }
        }

        if (combo == 0 && previousCombo > 0)
        {
            PlayWrongComboFeedback();
        }
    }

    private void PlayComboPunch()
    {
        if (comboText == null)
        {
            return;
        }

        if (comboPunchCoroutine != null)
        {
            StopCoroutine(comboPunchCoroutine);
        }

        comboPunchCoroutine = StartCoroutine(ComboPunchRoutine());
    }

    private IEnumerator ComboPunchRoutine()
    {
        comboText.transform.localScale = comboOriginalScale * comboPunchScale;

        float timer = 0f;

        while (timer < comboPunchDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / comboPunchDuration;

            comboText.transform.localScale = Vector3.Lerp(
                comboOriginalScale * comboPunchScale,
                comboOriginalScale,
                t
            );

            yield return null;
        }

        comboText.transform.localScale = comboOriginalScale;
    }

    private void PlaySpeedBoostText()
    {
        if (speedBoostText == null)
        {
            return;
        }

        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostTextRoutine());
    }

    private IEnumerator SpeedBoostTextRoutine()
    {
        speedBoostText.gameObject.SetActive(true);
        speedBoostText.text = "SPEED BOOST!";

        float timer = 0f;

        while (timer < speedBoostTextDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        speedBoostText.gameObject.SetActive(false);
    }

    private void PlayWrongComboFeedback()
    {
        if (comboText == null)
        {
            return;
        }

        if (wrongComboCoroutine != null)
        {
            StopCoroutine(wrongComboCoroutine);
        }

        wrongComboCoroutine = StartCoroutine(WrongComboRoutine());
    }

    private IEnumerator WrongComboRoutine()
    {
        comboText.color = wrongComboColor;

        float timer = 0f;
        float duration = 0.25f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        comboText.color = normalComboColor;
    }
}