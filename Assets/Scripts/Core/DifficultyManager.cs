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

    [Header("Easy Settings")]
    [SerializeField] private float easyForwardSpeed = 4.3f;
    [SerializeField] private float easyBeatTolerance = 0.24f;
    [SerializeField] private float easySpawnDistanceAhead = 38f;
    [SerializeField] private int easyFirstSpawnBeat = 6;
    [SerializeField] private int easySpawnEveryBeats = 5;
    [SerializeField] private bool easyAvoidSameLaneTwice = true;

    [Header("Normal Settings")]
    [SerializeField] private float normalForwardSpeed = 5.5f;
    [SerializeField] private float normalBeatTolerance = 0.16f;
    [SerializeField] private float normalSpawnDistanceAhead = 35f;
    [SerializeField] private int normalFirstSpawnBeat = 4;
    [SerializeField] private int normalSpawnEveryBeats = 4;
    [SerializeField] private bool normalAvoidSameLaneTwice = true;

    [Header("Hard Settings")]
    [SerializeField] private float hardForwardSpeed = 7.2f;
    [SerializeField] private float hardBeatTolerance = 0.10f;
    [SerializeField] private float hardSpawnDistanceAhead = 42f;
    [SerializeField] private int hardFirstSpawnBeat = 3;
    [SerializeField] private int hardSpawnEveryBeats = 3;
    [SerializeField] private bool hardAvoidSameLaneTwice = false;

    public GameDifficulty SelectedDifficulty => selectedDifficulty;

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

        ApplySelectedTrackGameplayProfile();

        Debug.Log("Zorluk uygulandı: " + selectedDifficulty);
    }

    private void ApplySelectedTrackGameplayProfile()
    {
        if (!SelectedMusicManager.HasSelectedTrack())
        {
            return;
        }

        MusicTrackData selectedTrack = SelectedMusicManager.SelectedTrack;

        if (selectedTrack == null)
        {
            return;
        }

        if (playerMovement != null)
        {
            float currentSpeed = playerMovement.GetForwardSpeed();
            playerMovement.SetForwardSpeed(currentSpeed * selectedTrack.playerSpeedMultiplier);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetSpawnEveryBeats(selectedTrack.obstacleSpawnEveryBeats);
            obstacleSpawner.SetRhythmGateEveryBeats(selectedTrack.rhythmGateEveryBeats);
            obstacleSpawner.SetMinimumComboForRhythmGate(selectedTrack.minimumComboForRhythmGate);
            obstacleSpawner.SetRhythmGateGameOverOnMiss(selectedTrack.rhythmGateGameOverOnMiss);
        }

        Debug.Log(
            "Track gameplay profile uygulandı: " +
            selectedTrack.trackName +
            " | Speed Multiplier: " +
            selectedTrack.playerSpeedMultiplier +
            " | Obstacle Every Beats: " +
            selectedTrack.obstacleSpawnEveryBeats +
            " | Gate Every Beats: " +
            selectedTrack.rhythmGateEveryBeats
        );
    }

    private void ApplyEasySettings()
    {
        ApplyPlayerAndRhythmSettings(
            easyForwardSpeed,
            easyBeatTolerance
        );

        ApplyObstacleSettings(
            easySpawnDistanceAhead,
            easyFirstSpawnBeat,
            easySpawnEveryBeats,
            easyAvoidSameLaneTwice
        );
    }

    private void ApplyNormalSettings()
    {
        ApplyPlayerAndRhythmSettings(
            normalForwardSpeed,
            normalBeatTolerance
        );

        ApplyObstacleSettings(
            normalSpawnDistanceAhead,
            normalFirstSpawnBeat,
            normalSpawnEveryBeats,
            normalAvoidSameLaneTwice
        );
    }

    private void ApplyHardSettings()
    {
        ApplyPlayerAndRhythmSettings(
            hardForwardSpeed,
            hardBeatTolerance
        );

        ApplyObstacleSettings(
            hardSpawnDistanceAhead,
            hardFirstSpawnBeat,
            hardSpawnEveryBeats,
            hardAvoidSameLaneTwice
        );
    }

    private void ApplyPlayerAndRhythmSettings(float forwardSpeed, float beatTolerance)
    {
        if (playerMovement != null)
        {
            playerMovement.SetForwardSpeed(forwardSpeed);
        }

        if (rhythmManager != null)
        {
            rhythmManager.SetBeatTolerance(beatTolerance);
        }
    }

    private void ApplyObstacleSettings(
        float spawnDistanceAhead,
        int firstSpawnBeat,
        int spawnEveryBeats,
        bool avoidSameLaneTwice
    )
    {
        if (obstacleSpawner == null)
        {
            return;
        }

        obstacleSpawner.SetSpawnDistanceAhead(spawnDistanceAhead);
        obstacleSpawner.SetFirstSpawnBeat(firstSpawnBeat);
        obstacleSpawner.SetSpawnEveryBeats(spawnEveryBeats);
        obstacleSpawner.SetAvoidSameLaneTwice(avoidSameLaneTwice);

        obstacleSpawner.SetUsePatternSpawning(true);
        obstacleSpawner.SetContinueRandomAfterPatternEnds(true);
        obstacleSpawner.SetRepeatPatternAfterEnd(false);
    }
}