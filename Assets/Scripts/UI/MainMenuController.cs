using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

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
        PlayerPrefs.SetString("SelectedDifficulty", difficulty);
        PlayerPrefs.Save();

        Debug.Log("Seçilen zorluk: " + difficulty);

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkış yapıldı.");
        Application.Quit();
    }
}