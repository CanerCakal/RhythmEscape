using UnityEngine;

public class InputTiming : MonoBehaviour
{
    public enum TimingResult { PERFECT, GOOD, MISS }

    [Header("Tolerans Ayarları")]
    public float perfectWindow = 0.15f; 
    public float goodWindow = 0.35f;

    public TimingResult CheckTiming()
    {
        if (BeatManager.Instance == null) return TimingResult.MISS;

        float timer = BeatManager.Instance.GetBeatTimer();
        float interval = BeatManager.Instance.GetBeatInterval();

        // En yakın vuruşa olan mesafeyi ölç
        float offset = timer;
        if (timer > interval / 2f)
        {
            offset = interval - timer;
        }

        TimingResult result = CalculateResult(offset);
        
        // Burası hata aldığın kısımdı, artık ScoreManager'da ProcessHit var
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ProcessHit(result);
        }

        return result;
    }

    private TimingResult CalculateResult(float offset)
    {
        if (offset <= perfectWindow) return TimingResult.PERFECT;
        else if (offset <= goodWindow) return TimingResult.GOOD;
        else return TimingResult.MISS;
    }
}