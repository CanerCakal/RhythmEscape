using UnityEngine;

public class ObjectPulser : MonoBehaviour
{
    public float pulseSize = 1.1f; // Ne kadar büyüyecek?
    public float returnSpeed = 5f; // Eski haline ne kadar hızlı dönecek?
    private Vector3 startScale;

    void Start() {
        startScale = transform.localScale;
    }

    void Update() {
        // Her karede yavaşça orijinal boyuta dön
        transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.deltaTime * returnSpeed);
        
        // Eğer BeatManager'dan bir vuruş gelirse büyü (Bu kısmı senin BeatManager olayına bağlayacağız)
        // Şimdilik test için boşluk tuşuna basınca büyüsün:
        if(Input.GetKeyDown(KeyCode.Space)) Pulse();
    }

    public void Pulse() {
        transform.localScale = startScale * pulseSize;
    }
}