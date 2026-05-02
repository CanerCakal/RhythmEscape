using UnityEngine;

public class BeatLightPulse : MonoBehaviour
{
    private Light myLight;
    
    [Header("Ayarlar")]
    public float maxIntensity = 20f;
    public float fadeSpeed = 10f;

    void Start()
    {
        myLight = GetComponent<Light>();
        
        if (BeatManager.Instance != null)
        {
            // Burada da OnBeat olarak güncelledik
            BeatManager.Instance.OnBeat.AddListener(Flash);
        }
    }

    void Update()
    {
        if (myLight != null)
        {
            myLight.intensity = Mathf.Lerp(myLight.intensity, 0, Time.deltaTime * fadeSpeed);
        }
    }

    public void Flash()
    {
        if (myLight != null)
        {
            myLight.intensity = maxIntensity;
        }
    }
}