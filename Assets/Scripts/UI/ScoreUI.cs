using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI highestComboText;

    private bool isSubscribed = false;

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
        }

        if (highestComboText != null)
        {
            highestComboText.text = "Best Combo: x" + highestCombo;
        }
    }
}