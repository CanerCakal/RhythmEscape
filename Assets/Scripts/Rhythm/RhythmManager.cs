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
        float songTime = GetSongTime();

        float beatPosition = songTime / secondsPerBeat;
        float nearestBeat = Mathf.Round(beatPosition);

        float timeToNearestBeat = Mathf.Abs(beatPosition - nearestBeat) * secondsPerBeat;

        return timeToNearestBeat <= beatTolerance;
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
}