using UnityEngine;

[CreateAssetMenu(fileName = "NewMusicTrack", menuName = "Rhythm Escape/Music Track")]
public class MusicTrackData : ScriptableObject
{
    [Header("Track Info")]
    public string trackName = "New Track";
    public string artistName = "Unknown Artist";

    [Header("Audio")]
    public AudioClip audioClip;

    [Header("Rhythm Settings")]
    public float bpm = 120f;

    [Header("Gameplay Info")]
    public string difficultyLabel = "Normal";

    [Header("Track Gameplay Profile")]
    [Range(0.75f, 1.5f)]
    public float playerSpeedMultiplier = 1f;

    [Range(2, 8)]
    public int obstacleSpawnEveryBeats = 4;

    [Range(8, 48)]
    public int rhythmGateEveryBeats = 24;

    [Range(0, 20)]
    public int minimumComboForRhythmGate = 0;

    [Header("Rhythm Gate Rules")]
    public bool rhythmGateGameOverOnMiss = false;

    [Header("Theme")]
    public Color themeColor = Color.cyan;
}