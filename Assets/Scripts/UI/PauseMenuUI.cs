using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private bool isSubscribed = false;

    private void Start()
    {
        HidePausePanel();
        TrySubscribeToGameManager();

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
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
            GameManager.Instance.OnGamePaused -= ShowPausePanel;
            GameManager.Instance.OnGameResumed -= HidePausePanel;
            isSubscribed = false;
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToGameManager();
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

        GameManager.Instance.OnGamePaused += ShowPausePanel;
        GameManager.Instance.OnGameResumed += HidePausePanel;

        isSubscribed = true;

        Debug.Log("PauseMenuUI GameManager'a bağlandı.");
    }

    private void ShowPausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    private void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    private void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
    }
}