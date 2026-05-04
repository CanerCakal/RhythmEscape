using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    [SerializeField] private bool isGameOver = false;
    [SerializeField] private bool isPaused = false;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public event Action OnGameOver;
    public event Action OnGamePaused;
    public event Action OnGameResumed;

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    public bool IsPaused
    {
        get { return isPaused; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        isPaused = false;

        Debug.Log("Oyun bitti!");

        if (musicSource != null)
        {
            musicSource.Pause();
        }

        OnGameOver?.Invoke();

        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        if (isGameOver)
        {
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isGameOver || isPaused)
        {
            return;
        }

        isPaused = true;

        if (musicSource != null)
        {
            musicSource.Pause();
        }

        Time.timeScale = 0f;

        Debug.Log("Oyun duraklatıldı.");

        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (isGameOver || !isPaused)
        {
            return;
        }

        isPaused = false;

        Time.timeScale = 1f;

        if (musicSource != null)
        {
            musicSource.UnPause();
        }

        Debug.Log("Oyun devam ediyor.");

        OnGameResumed?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkış yapıldı.");
        Application.Quit();
    }
}