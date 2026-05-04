using System.Collections.Generic;
using UnityEngine;

public class RoadSegmentVisual : MonoBehaviour
{
    [Header("Create Visual Parts")]
    [SerializeField] private bool createVisualsOnStart = true;

    [Header("Segment Size")]
    [SerializeField] private float segmentWidth = 6f;
    [SerializeField] private float segmentLength = 30f;

    [Header("Side Barrier Settings")]
    [SerializeField] private float barrierWidth = 0.15f;
    [SerializeField] private float barrierHeight = 0.7f;
    [SerializeField] private float barrierYPosition = 0f;

    [Header("Lane Line Settings")]
    [SerializeField] private float laneLineXPosition = 1f;
    [SerializeField] private float laneLineWidth = 0.06f;
    [SerializeField] private float laneLineHeight = 0.03f;
    [SerializeField] private float laneLineYPosition = -0.27f;

    [Header("Colors")]
    [SerializeField] private Color roadNormalColor = new Color(0.12f, 0.12f, 0.12f);
    [SerializeField] private Color roadBeatColor = new Color(0.25f, 0.25f, 0.25f);

    [SerializeField] private Color neonNormalColor = new Color(0f, 0.8f, 1f);
    [SerializeField] private Color neonBeatColor = Color.white;

    [Header("Beat Reaction")]
    [SerializeField] private float returnSpeed = 6f;

    private Renderer roadRenderer;
    private readonly List<Renderer> neonRenderers = new List<Renderer>();

    private bool visualsCreated = false;
    private bool isSubscribed = false;

    private void Awake()
    {
        roadRenderer = GetComponent<Renderer>();

        if (roadRenderer != null)
        {
            roadRenderer.material.color = roadNormalColor;
        }
    }

    private void Start()
    {
        if (createVisualsOnStart)
        {
            CreateVisuals();
        }

        TrySubscribeToRhythmManager();
    }

    private void OnEnable()
    {
        TrySubscribeToRhythmManager();
    }

    private void OnDisable()
    {
        UnsubscribeFromRhythmManager();
    }

    private void OnDestroy()
    {
        UnsubscribeFromRhythmManager();
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToRhythmManager();
        }

        UpdateColors();
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
    }

    private void UnsubscribeFromRhythmManager()
    {
        if (RhythmManager.Instance != null && isSubscribed)
        {
            RhythmManager.Instance.OnBeat -= HandleBeat;
            isSubscribed = false;
        }
    }

    private void CreateVisuals()
    {
        if (visualsCreated)
        {
            return;
        }

        visualsCreated = true;

        CreateSideBarrier("Left Barrier", -segmentWidth / 2f - barrierWidth / 2f);
        CreateSideBarrier("Right Barrier", segmentWidth / 2f + barrierWidth / 2f);

        CreateLaneLine("Left Lane Line", -laneLineXPosition);
        CreateLaneLine("Right Lane Line", laneLineXPosition);
    }

    private void CreateSideBarrier(string objectName, float xPosition)
    {
        Vector3 worldPosition = transform.position + new Vector3(
            xPosition,
            barrierYPosition,
            0f
        );

        Vector3 worldScale = new Vector3(
            barrierWidth,
            barrierHeight,
            segmentLength
        );

        GameObject barrier = CreateVisualCube(objectName, worldPosition, worldScale, neonNormalColor);
        neonRenderers.Add(barrier.GetComponent<Renderer>());
    }

    private void CreateLaneLine(string objectName, float xPosition)
    {
        Vector3 worldPosition = transform.position + new Vector3(
            xPosition,
            laneLineYPosition,
            0f
        );

        Vector3 worldScale = new Vector3(
            laneLineWidth,
            laneLineHeight,
            segmentLength
        );

        GameObject laneLine = CreateVisualCube(objectName, worldPosition, worldScale, neonNormalColor);
        neonRenderers.Add(laneLine.GetComponent<Renderer>());
    }

    private GameObject CreateVisualCube(string objectName, Vector3 worldPosition, Vector3 worldScale, Color color)
    {
        GameObject visualObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

        visualObject.name = objectName;
        visualObject.transform.position = worldPosition;
        visualObject.transform.localScale = worldScale;

        Renderer renderer = visualObject.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = color;
        }

        Collider collider = visualObject.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }

        visualObject.transform.SetParent(transform, true);

        return visualObject;
    }

    private void HandleBeat(int beatIndex)
    {
        if (roadRenderer != null)
        {
            roadRenderer.material.color = roadBeatColor;
        }

        for (int i = 0; i < neonRenderers.Count; i++)
        {
            if (neonRenderers[i] != null)
            {
                neonRenderers[i].material.color = neonBeatColor;
            }
        }
    }

    private void UpdateColors()
    {
        if (roadRenderer != null)
        {
            roadRenderer.material.color = Color.Lerp(
                roadRenderer.material.color,
                roadNormalColor,
                returnSpeed * Time.deltaTime
            );
        }

        for (int i = 0; i < neonRenderers.Count; i++)
        {
            if (neonRenderers[i] == null)
            {
                continue;
            }

            neonRenderers[i].material.color = Color.Lerp(
                neonRenderers[i].material.color,
                neonNormalColor,
                returnSpeed * Time.deltaTime
            );
        }
    }
}