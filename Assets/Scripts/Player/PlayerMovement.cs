using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float laneDistance = 2f;
    public float laneChangeSpeed = 10f;

    private int currentLane = 1; // 0=sol, 1=orta, 2=sağ

    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
            currentLane--;

        if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2)
            currentLane++;

        Vector3 targetPosition = transform.position;
        targetPosition.x = (currentLane - 1) * laneDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            laneChangeSpeed * Time.deltaTime
        );
    }
}