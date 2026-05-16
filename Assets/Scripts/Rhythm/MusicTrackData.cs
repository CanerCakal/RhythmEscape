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
}