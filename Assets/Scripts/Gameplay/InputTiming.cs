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

        TimingResult result;

        if (timeSinceBeat <= perfectWindow)
        {
            result = TimingResult.Perfect;
            UIManager.Instance.ShowTiming("PERFECT!");
        }
        else if (timeSinceBeat <= goodWindow)
        {
            result = TimingResult.Good;
            UIManager.Instance.ShowTiming("GOOD!");
        }
        else
        {
            result = TimingResult.Miss;
            UIManager.Instance.ShowTiming("MISS!");
        }

        ScoreManager.Instance.AddScore(result);
        return result;
    }
}