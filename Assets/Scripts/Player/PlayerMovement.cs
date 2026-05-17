using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action OnCorrectRhythmMove;
    public static event Action OnWrongRhythmInput;
    public static event Action<BeatAccuracy> OnRhythmInputJudged;

    [Header("Forward Movement")]
    [SerializeField] private float forwardSpeed = 5f;

    [Header("Lane Movement")]
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float laneChangeSpeed = 10f;

    private int currentLane = 1;
    // 0 = sol, 1 = orta, 2 = sağ

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        MoveForward();
        MoveToTargetLane();
        HandleInput();
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMoveToLane(-1);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMoveToLane(1);
        }
    }

    private void TryMoveToLane(int direction)
    {
        if (RhythmManager.Instance == null)
        {
            Debug.LogWarning("RhythmManager sahnede bulunamadı.");
            return;
        }

        BeatAccuracy beatAccuracy = RhythmManager.Instance.GetBeatAccuracy();

        OnRhythmInputJudged?.Invoke(beatAccuracy);

        if (beatAccuracy == BeatAccuracy.Miss)
        {
            OnWrongRhythmInput?.Invoke();
            Debug.Log("Yanlış zamanda bastın! Accuracy: " + beatAccuracy);
            return;
        }

        int nextLane = currentLane + direction;

        if (nextLane < 0 || nextLane > 2)
        {
            Debug.Log("Bu yönde şerit yok.");
            return;
        }

        currentLane = nextLane;
        UpdateTargetPosition();

        OnCorrectRhythmMove?.Invoke();
        Debug.Log("Ritimli hareket! Accuracy: " + beatAccuracy);
    }

    private void UpdateTargetPosition()
    {
        float targetX = (currentLane - 1) * laneDistance;

        targetPosition = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );
    }

    private void MoveToTargetLane()
    {
        Vector3 newPosition = new Vector3(
            targetPosition.x,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            newPosition,
            laneChangeSpeed * Time.deltaTime
        );
    }
    public void SetForwardSpeed(float newSpeed)
    {
        forwardSpeed = newSpeed;
    }

    public float GetForwardSpeed()
    {
        return forwardSpeed;
    }
}