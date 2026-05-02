using UnityEngine;

public class BeatVisualizer : MonoBehaviour
{
    public float pulseSize = 1.2f;
    public float returnSpeed = 5f;
    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;

        // HATA BURADAYDI: BeatManager.Instance eklendi
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat.AddListener(OnBeatTrigger);
        }
    }

    void Update()
    {
        // Objeyi yavaşça normal boyutuna geri döndürür
        transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.deltaTime * returnSpeed);
    }

    void OnBeatTrigger()
    {
        // Ritim geldiğinde objeyi büyütür
        transform.localScale = startScale * pulseSize;
    }
}