using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private const string BestScoreKey = "BestScore";
    private const string SelectedDifficultyKey = "SelectedDifficulty";

    [Header("Scene Settings")]
    [SerializeField] private string musicSelectSceneName = "MusicSelectScene";

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("Optional Buttons")]
    [SerializeField] private Button resetBestScoreButton;

    private void Start()
    {
        Time.timeScale = 1f;
        UpdateBestScoreText();

        if (resetBestScoreButton != null)
        {
            resetBestScoreButton.onClick.AddListener(ResetBestScore);
        }
    }

    private void OnDisable()
    {
        if (resetBestScoreButton != null)
        {
            resetBestScoreButton.onClick.RemoveListener(ResetBestScore);
        }
    }

    public void PlayEasy()
    {
        StartGameWithDifficulty("Easy");
    }

    public void PlayNormal()
    {
        StartGameWithDifficulty("Normal");
    }

    public void PlayHard()
    {
        StartGameWithDifficulty("Hard");
    }

    private void StartGameWithDifficulty(string difficulty)
    {
        PlayerPrefs.SetString(SelectedDifficultyKey, difficulty);
        PlayerPrefs.Save();

        Debug.Log("Seçilen zorluk: " + difficulty);

        SceneManager.LoadScene(musicSelectSceneName);
    }

    public void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(BestScoreKey);
        PlayerPrefs.Save();

        UpdateBestScoreText();

        Debug.Log("Main Menu üzerinden Best Score sıfırlandı.");
    }

    private void UpdateBestScoreText()
    {
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (bestScoreText != null)
        {
            bestScoreText.text = "BEST SCORE  " + bestScore;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkış yapıldı.");
        Application.Quit();
    }
}