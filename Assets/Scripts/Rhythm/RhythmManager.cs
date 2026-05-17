using System;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Beat Settings")]
    [SerializeField] private float bpm = 120f;
    [SerializeField] private float beatTolerance = 0.15f;

    [Header("Beat Accuracy Settings")]
    [SerializeField] private float perfectTolerance = 0.06f;
    [SerializeField] private float goodTolerance = 0.15f;

    public event Action<int> OnBeat;

    private float secondsPerBeat;
    private float songStartTime;
    private int lastBeatIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ApplySelectedMusic();

        secondsPerBeat = 60f / bpm;

        if (musicSource != null)
        {
            musicSource.Play();
            songStartTime = Time.time;
        }
        else
        {
            Debug.LogWarning("RhythmManager: MusicSource atanmamış.");
            songStartTime = Time.time;
        }
    }

    private void ApplySelectedMusic()
    {
        if (!SelectedMusicManager.HasSelectedTrack())
        {
            Debug.LogWarning("RhythmManager: Seçilen müzik bulunamadı. Inspector BPM ve AudioSource kullanılacak.");
            return;
        }

        MusicTrackData selectedTrack = SelectedMusicManager.SelectedTrack;

        if (selectedTrack == null)
        {
            return;
        }

        bpm = selectedTrack.bpm;

        if (musicSource != null && selectedTrack.audioClip != null)
        {
            musicSource.clip = selectedTrack.audioClip;
        }

        Debug.Log("RhythmManager seçilen müziği uyguladı: " + selectedTrack.trackName + " | BPM: " + bpm);
    }

    private void Update()
    {
        CheckBeat();
    }

    private void CheckBeat()
    {
        float songTime = GetSongTime();

        int currentBeatIndex = Mathf.FloorToInt(songTime / secondsPerBeat);

        if (currentBeatIndex != lastBeatIndex)
        {
            lastBeatIndex = currentBeatIndex;
            OnBeat?.Invoke(currentBeatIndex);

            Debug.Log("Beat: " + currentBeatIndex);
        }
    }

    public bool IsOnBeat()
    {
        return GetTimeToNearestBeat() <= beatTolerance;
    }

    public BeatAccuracy GetBeatAccuracy()
    {
        float timeToNearestBeat = GetTimeToNearestBeat();

        if (timeToNearestBeat <= perfectTolerance)
        {
            return BeatAccuracy.Perfect;
        }

        if (timeToNearestBeat <= goodTolerance)
        {
            return BeatAccuracy.Good;
        }

        return BeatAccuracy.Miss;
    }

    private float GetTimeToNearestBeat()
    {
        float songTime = GetSongTime();

        float beatPosition = songTime / secondsPerBeat;
        float nearestBeat = Mathf.Round(beatPosition);

        float timeToNearestBeat = Mathf.Abs(beatPosition - nearestBeat) * secondsPerBeat;

        return timeToNearestBeat;
    }

    public float GetSongTime()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            return musicSource.time;
        }

        return Time.time - songStartTime;
    }

    public float GetSecondsPerBeat()
    {
        return secondsPerBeat;
    }
    public void SetBeatTolerance(float newTolerance)
    {
        beatTolerance = newTolerance;
    }
    public float GetBeatTolerance()
    {
        return beatTolerance;
    }

    public void SetPerfectTolerance(float newTolerance)
    {
        perfectTolerance = Mathf.Max(0.01f, newTolerance);
    }

    public float GetPerfectTolerance()
    {
        return perfectTolerance;
    }

    public void SetGoodTolerance(float newTolerance)
    {
        goodTolerance = Mathf.Max(perfectTolerance, newTolerance);
    }

    public float GetGoodTolerance()
    {
        return goodTolerance;
    }
}