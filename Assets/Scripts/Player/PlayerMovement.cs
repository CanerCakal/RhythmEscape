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

    [Header("Jump Settings")]
    [SerializeField] private bool enableJump = true;
    [SerializeField] private float jumpHeight = 2.4f;
    [SerializeField] private float jumpDuration = 0.55f;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Jump Input Alternatives")]
    [SerializeField] private bool useAlternativeJumpKeys = true;

    private int currentLane = 1;
    // 0 = sol, 1 = orta, 2 = sağ

    private Vector3 targetPosition;

    private float baseYPosition;
    private bool isJumping = false;
    private float jumpTimer = 0f;

    public bool IsJumping => isJumping;

    private void Start()
    {
        targetPosition = transform.position;
        baseYPosition = transform.position.y;
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
        HandleJump();
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

        if (ShouldJump())
        {
            TryJump();
        }
    }

    private bool ShouldJump()
    {
        if (!enableJump)
        {
            return false;
        }

        if (Input.GetKeyDown(jumpKey))
        {
            return true;
        }

        if (!useAlternativeJumpKeys)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
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

    private void TryJump()
    {
        if (isJumping)
        {
            return;
        }

        isJumping = true;
        jumpTimer = 0f;

        Debug.Log("Player zıpladı.");
    }

    private void HandleJump()
    {
        if (!isJumping)
        {
            return;
        }

        jumpTimer += Time.deltaTime;

        float normalizedTime = jumpTimer / jumpDuration;

        if (normalizedTime >= 1f)
        {
            isJumping = false;

            Vector3 groundedPosition = transform.position;
            groundedPosition.y = baseYPosition;
            transform.position = groundedPosition;

            return;
        }

        float jumpCurve = Mathf.Sin(normalizedTime * Mathf.PI);
        float currentY = baseYPosition + (jumpCurve * jumpHeight);

        Vector3 newPosition = transform.position;
        newPosition.y = currentY;
        transform.position = newPosition;
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

    public int GetCurrentLane()
    {
        return currentLane;
    }

    public float GetLaneDistance()
    {
        return laneDistance;
    }
}