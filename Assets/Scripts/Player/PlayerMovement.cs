using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 8f;
    public float laneDistance = 2f;
    public float laneChangeSpeed = 10f;

    private int currentLane = 1; // 0 = Sol, 1 = Orta, 2 = Sağ
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        MoveForward();
        MoveToLane();
    }
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentLane > 0)
                currentLane--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentLane < 2)
                currentLane++;
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }
    void MoveToLane()
    {
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 desiredPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            laneChangeSpeed * Time.deltaTime
        );
    }
}