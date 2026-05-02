using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;

    [Header("Rhythm System")]
    public InputTiming inputTiming;

    private int targetLane = 1; // 0 = Sol, 1 = Orta, 2 = Sağ

    void Update()
    {
        // 1. Sürekli ileri hareket
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // 2. Sol hareket (Ok tuşu ve Ritim Kontrolü)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (inputTiming.CheckTiming() != InputTiming.TimingResult.MISS)
            {
                if (targetLane > 0) targetLane--;
            }
        }

        // 3. Sağ hareket (Ok tuşu ve Ritim Kontrolü)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (inputTiming.CheckTiming() != InputTiming.TimingResult.MISS)
            {
                if (targetLane < 2) targetLane++;
            }
        }

        // 4. Hedef Pozisyonu Hesapla
        float targetX = (targetLane - 1) * laneDistance;
        // Z değerini transform.position.z yaparak ileri hareketi bozmuyoruz
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);

        // 5. Akıcı Şerit Değişimi (Sadece X ekseninde yumuşatma)
        Vector3 newPos = Vector3.Lerp(transform.position, targetPos, laneChangeSpeed * Time.deltaTime);
        newPos.z = transform.position.z; // Z eksenini koru
        transform.position = newPos;
    }
}