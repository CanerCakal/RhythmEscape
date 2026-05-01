using UnityEngine;

public class InputTiming : MonoBehaviour
{
    [Header("References")]
    public BeatManager beatManager;

    [Header("Timing Windows")]
    public float perfectWindow = 0.1f;
    public float goodWindow = 0.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckTiming();
        }
    }

    void CheckTiming()
    {
        float timeSinceBeat = Mathf.Abs(Time.time - beatManager.LastBeatTime);

        if (timeSinceBeat <= perfectWindow)
        {
            Debug.Log("PERFECT!");
        }
        else if (timeSinceBeat <= goodWindow)
        {
            Debug.Log("GOOD!");
        }
        else
        {
            Debug.Log("MISS!");
        }
    }
}