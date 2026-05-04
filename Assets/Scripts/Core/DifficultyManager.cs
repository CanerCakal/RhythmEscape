using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}

public class DifficultyManager : MonoBehaviour
{
    [Header("Selected Difficulty")]
    [SerializeField] private GameDifficulty selectedDifficulty = GameDifficulty.Normal;

    [Header("Use Saved Difficulty")]
    [SerializeField] private bool useSavedDifficulty = true;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RhythmManager rhythmManager;
    [SerializeField] private BeatObstacleSpawner obstacleSpawner;

    private void Start()
    {
        LoadDifficulty();
        ApplyDifficulty();
    }

    private void LoadDifficulty()
    {
        if (!useSavedDifficulty)
        {
            return;
        }

        string savedDifficulty = PlayerPrefs.GetString("SelectedDifficulty", "Normal");

        switch (savedDifficulty)
        {
            case "Easy":
                selectedDifficulty = GameDifficulty.Easy;
                break;

            case "Hard":
                selectedDifficulty = GameDifficulty.Hard;
                break;

            default:
                selectedDifficulty = GameDifficulty.Normal;
                break;
        }
    }

    private void ApplyDifficulty()
    {
        switch (selectedDifficulty)
        {
            case GameDifficulty.Easy:
                ApplyEasySettings();
                break;

            case GameDifficulty.Normal:
                ApplyNormalSettings();
                break;

            case GameDifficulty.Hard:
                ApplyHardSettings();
                break;
        }

        Debug.Log("Zorluk uygulandı: " + selectedDifficulty);
    }

    private void ApplyEasySettings()
    {
        if (playerMovement != null)
        {
            playerMovement.SetForwardSpeed(4.5f);
        }

        if (rhythmManager != null)
        {
            rhythmManager.SetBeatTolerance(0.22f);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetSpawnDistanceAhead(38f);
        }
    }

    private void ApplyNormalSettings()
    {
        if (playerMovement != null)
        {
            playerMovement.SetForwardSpeed(5.5f);
        }

        if (rhythmManager != null)
        {
            rhythmManager.SetBeatTolerance(0.16f);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetSpawnDistanceAhead(35f);
        }
    }

    private void ApplyHardSettings()
    {
        if (playerMovement != null)
        {
            playerMovement.SetForwardSpeed(7f);
        }

        if (rhythmManager != null)
        {
            rhythmManager.SetBeatTolerance(0.10f);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetSpawnDistanceAhead(42f);
        }
    }
}