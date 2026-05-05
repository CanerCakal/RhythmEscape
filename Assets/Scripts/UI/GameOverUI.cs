using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalComboText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI newRecordText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;

    private bool isSubscribed = false;

    private void Start()
    {
        HidePanel();
        TrySubscribeToGameManager();

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void OnEnable()
    {
        TrySubscribeToGameManager();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && isSubscribed)
        {
            GameManager.Instance.OnGameOver -= ShowGameOverPanel;
            isSubscribed = false;
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToGameManager();
        }

        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }

    private void TrySubscribeToGameManager()
    {
        if (isSubscribed)
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.OnGameOver += ShowGameOverPanel;
        isSubscribed = true;

        Debug.Log("GameOverUI GameManager'a bağlandı.");
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        int finalScore = 0;
        int bestScore = 0;
        int bestCombo = 0;
        bool isNewRecord = false;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveBestScoreIfNeeded();

            finalScore = ScoreManager.Instance.Score;
            bestScore = ScoreManager.Instance.BestScore;
            bestCombo = ScoreManager.Instance.HighestCombo;
            isNewRecord = ScoreManager.Instance.IsNewBestScore;
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score: " + bestScore;
        }

        if (finalComboText != null)
        {
            finalComboText.text = "Best Combo: x" + bestCombo;
        }

        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(isNewRecord);
            newRecordText.text = "NEW RECORD!";
        }
    }

    private void HidePanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false);
        }
    }

    private void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}