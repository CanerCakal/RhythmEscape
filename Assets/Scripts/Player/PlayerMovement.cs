using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 2f;
    public float laneChangeSpeed = 10f;

    [Header("Rhythm System")]
    public InputTiming inputTiming;

    private int targetLane = 1; // 0 = Sol, 1 = Orta, 2 = Sağ

    void Update()
    {
        // Sürekli ileri hareket
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Sol hareket
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (inputTiming.CheckTiming() != InputTiming.TimingResult.Miss)
            {
                targetLane--;
            }
        }

        // Sağ hareket
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (inputTiming.CheckTiming() != InputTiming.TimingResult.Miss)
            {
                targetLane++;
            }
        }

        // Şerit sınırları
        targetLane = Mathf.Clamp(targetLane, 0, 2);

        // Hedef pozisyon
        Vector3 targetPosition = transform.position;
        targetPosition.x = (targetLane - 1) * laneDistance;

        // Akıcı geçiş
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            laneChangeSpeed * Time.deltaTime
        );
    }
}