using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan bu satır önemli

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Skor Ayarları")]
    public int currentScore = 0;
    public int currentCombo = 0;
    public int highScore = 0;

    [Header("UI Referansları")]
    public TextMeshProUGUI scoreText; // Unity içinde sürükle bırak yapmalısın
    public TextMeshProUGUI comboText; // Unity içinde sürükle bırak yapmalısın

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
    }

    // InputTiming.cs bu fonksiyonu çağırıyor
    public void ProcessHit(InputTiming.TimingResult result)
    {
        if (result == InputTiming.TimingResult.PERFECT)
        {
            currentScore += 100;
            currentCombo++;
        }
        else if (result == InputTiming.TimingResult.GOOD)
        {
            currentScore += 50;
            currentCombo++;
        }
        else // MISS durumu
        {
            currentCombo = 0;
        }

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
        if (comboText != null) comboText.text = "Combo: " + currentCombo;
    }
}