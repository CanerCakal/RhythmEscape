using UnityEngine;
using UnityEngine.UI;

public class BeatIndicatorUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image beatImage;

    [Header("Pulse Settings")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float beatScale = 1.6f;
    [SerializeField] private float returnSpeed = 8f;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color beatColor = Color.green;

    private RectTransform rectTransform;
    private bool isSubscribed = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (beatImage == null)
        {
            beatImage = GetComponent<Image>();
        }
    }

    private void Start()
    {
        TrySubscribeToRhythmManager();
    }

    private void OnEnable()
    {
        TrySubscribeToRhythmManager();
    }

    private void OnDisable()
    {
        if (RhythmManager.Instance != null && isSubscribed)
        {
            RhythmManager.Instance.OnBeat -= HandleBeat;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToRhythmManager();
        }

        if (rectTransform == null || beatImage == null)
        {
            return;
        }

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            Vector3.one * normalScale,
            returnSpeed * Time.deltaTime
        );

        beatImage.color = Color.Lerp(
            beatImage.color,
            normalColor,
            returnSpeed * Time.deltaTime
        );
    }

    private void TrySubscribeToRhythmManager()
    {
        if (isSubscribed)
        {
            return;
        }

        if (RhythmManager.Instance == null)
        {
            return;
        }

        RhythmManager.Instance.OnBeat += HandleBeat;
        isSubscribed = true;

        Debug.Log("BeatIndicatorUI RhythmManager'a bağlandı.");
    }

    private void HandleBeat(int beatIndex)
    {
        if (rectTransform == null || beatImage == null)
        {
            return;
        }

        rectTransform.localScale = Vector3.one * beatScale;
        beatImage.color = beatColor;

        Debug.Log("UI Beat aldı: " + beatIndex);
    }
}