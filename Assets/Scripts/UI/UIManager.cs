using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI timingText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateCombo(int combo)
    {
        comboText.text = "Combo: " + combo;
    }

    public void ShowTiming(string timing)
    {
        timingText.text = timing;
        CancelInvoke(nameof(ClearTiming));
        Invoke(nameof(ClearTiming), 0.5f);
    }

    public void ShowGameOver(int score, int highScore)
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score: " + score;
        highScoreText.text = "Best: " + highScore;
    }

    void ClearTiming()
    {
        timingText.text = "";
    }
}