using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 6f, -8f);
    [SerializeField] private float followSpeed = 8f;

    [Header("Lane Follow")]
    [SerializeField] private bool followPlayerX = true;

    [Header("Beat Camera Pulse")]
    [SerializeField] private bool useBeatPulse = true;
    [SerializeField] private float normalFieldOfView = 60f;
    [SerializeField] private float beatFieldOfView = 64f;
    [SerializeField] private float fovReturnSpeed = 8f;

    [Header("Shake Settings")]
    [SerializeField] private float hitShakeDuration = 0.35f;
    [SerializeField] private float hitShakeStrength = 0.35f;

    [SerializeField] private float correctMoveShakeDuration = 0.08f;
    [SerializeField] private float correctMoveShakeStrength = 0.08f;

    private Camera cameraComponent;

    private Vector3 smoothedFollowPosition;
    private Vector3 shakeOffset;

    private float shakeTimer = 0f;
    private float currentShakeDuration = 0f;
    private float currentShakeStrength = 0f;

    private bool rhythmSubscribed = false;
    private bool gameManagerSubscribed = false;
    private bool playerSubscribed = false;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();

        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = normalFieldOfView;
        }
    }

    private void Start()
    {
        if (target != null)
        {
            smoothedFollowPosition = target.position + offset;
            transform.position = smoothedFollowPosition;
        }

        TrySubscribeToEvents();
    }

    private void OnEnable()
    {
        TrySubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        if (!rhythmSubscribed || !gameManagerSubscribed || !playerSubscribed)
        {
            TrySubscribeToEvents();
        }

        UpdateFieldOfView();
        UpdateShake();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;

        if (!followPlayerX)
        {
            desiredPosition.x = smoothedFollowPosition.x;
        }

        smoothedFollowPosition = Vector3.Lerp(
            smoothedFollowPosition,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.position = smoothedFollowPosition + shakeOffset;
    }

    private void TrySubscribeToEvents()
    {
        if (!rhythmSubscribed && RhythmManager.Instance != null)
        {
            RhythmManager.Instance.OnBeat += HandleBeat;
            rhythmSubscribed = true;
        }

        if (!gameManagerSubscribed && GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += HandleGameOver;
            gameManagerSubscribed = true;
        }

        if (!playerSubscribed)
        {
            PlayerMovement.OnCorrectRhythmMove += HandleCorrectMove;
            playerSubscribed = true;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (RhythmManager.Instance != null && rhythmSubscribed)
        {
            RhythmManager.Instance.OnBeat -= HandleBeat;
            rhythmSubscribed = false;
        }

        if (GameManager.Instance != null && gameManagerSubscribed)
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
            gameManagerSubscribed = false;
        }

        if (playerSubscribed)
        {
            PlayerMovement.OnCorrectRhythmMove -= HandleCorrectMove;
            playerSubscribed = false;
        }
    }

    private void HandleBeat(int beatIndex)
    {
        if (!useBeatPulse)
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = beatFieldOfView;
        }
    }

    private void HandleCorrectMove()
    {
        StartShake(correctMoveShakeDuration, correctMoveShakeStrength);
    }

    private void HandleGameOver()
    {
        StartShake(hitShakeDuration, hitShakeStrength);
    }

    private void StartShake(float duration, float strength)
    {
        currentShakeDuration = duration;
        currentShakeStrength = strength;
        shakeTimer = duration;
    }

    private void UpdateFieldOfView()
    {
        if (cameraComponent == null)
        {
            return;
        }

        cameraComponent.fieldOfView = Mathf.Lerp(
            cameraComponent.fieldOfView,
            normalFieldOfView,
            fovReturnSpeed * Time.unscaledDeltaTime
        );
    }

    private void UpdateShake()
    {
        if (shakeTimer <= 0f)
        {
            shakeOffset = Vector3.zero;
            return;
        }

        shakeTimer -= Time.unscaledDeltaTime;

        float shakeProgress = shakeTimer / currentShakeDuration;
        float currentStrength = currentShakeStrength * shakeProgress;

        shakeOffset = new Vector3(
            Random.Range(-1f, 1f) * currentStrength,
            Random.Range(-1f, 1f) * currentStrength,
            0f
        );
    }
}