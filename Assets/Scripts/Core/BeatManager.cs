using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;
    public float bpm = 120f;
    public UnityEvent OnBeat;

    private float beatInterval;
    private float beatTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        beatInterval = 60f / bpm;
    }

    void Update()
    {
        beatTimer += Time.deltaTime;
        if (beatTimer >= beatInterval)
        {
            beatTimer -= beatInterval;
            TriggerBeat();
        }
    }

    void TriggerBeat()
    {
        if (OnBeat != null) OnBeat.Invoke();
    }

    // --- BU FONKSİYONU EKLEDİK ---
    // InputTiming'in ne kadar geciktiğimizi anlamasını sağlar
    public float GetBeatTimer() { return beatTimer; }
    public float GetBeatInterval() { return 60f / bpm; }
}