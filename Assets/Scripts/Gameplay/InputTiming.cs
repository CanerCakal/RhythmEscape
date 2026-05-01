using UnityEngine;

public class InputTiming : MonoBehaviour
{
    public enum TimingResult
    {
        Perfect,
        Good,
        Miss
    }

    [Header("References")]
    public BeatManager beatManager;

    [Header("Timing Windows")]
    public float perfectWindow = 0.1f;
    public float goodWindow = 0.2f;

    public TimingResult CheckTiming()
    {
        float timeSinceBeat = Mathf.Abs(Time.time - beatManager.LastBeatTime);

        if (timeSinceBeat <= perfectWindow)
        {
            Debug.Log("PERFECT!");
            return TimingResult.Perfect;
        }
        else if (timeSinceBeat <= goodWindow)
        {
            Debug.Log("GOOD!");
            return TimingResult.Good;
        }

        Debug.Log("MISS!");
        return TimingResult.Miss;
    }
}