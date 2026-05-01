using UnityEngine;
using System;

public class BeatManager : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioSource musicSource;
    public float bpm = 120f;

    [Header("Beat Settings")]
    public float beatInterval;
    public float LastBeatTime { get; private set; }
    private float nextBeatTime;

    public static event Action OnBeat;

    void Start()
    {
        beatInterval = 60f / bpm;
        nextBeatTime = beatInterval;
    }

    void Update()
    {
        if (musicSource.isPlaying && musicSource.time >= nextBeatTime)
        {
            LastBeatTime = Time.time;
            OnBeat?.Invoke();
            Debug.Log("BEAT!");
            nextBeatTime += beatInterval;
        }
    }
}