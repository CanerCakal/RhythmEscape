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

    [Header("Center Glow Line Settings")]
    [SerializeField] private bool createCenterGlowLine = true;
    [SerializeField] private float centerLineWidth = 0.08f;
    [SerializeField] private float centerLineHeight = 0.035f;
    [SerializeField] private float centerLineYPosition = -0.25f;

    [Header("Outer Rail Settings")]
    [SerializeField] private bool createOuterRails = true;
    [SerializeField] private float outerRailOffset = 0.35f;
    [SerializeField] private float outerRailWidth = 0.08f;
    [SerializeField] private float outerRailHeight = 0.15f;

    [Header("Side Block Settings")]
    [SerializeField] private bool createSideBlocks = true;
    [SerializeField] private int sideBlockCountPerSegment = 6;
    [SerializeField] private float sideBlockWidth = 0.22f;
    [SerializeField] private float sideBlockHeight = 0.18f;
    [SerializeField] private float sideBlockLength = 1.2f;
    [SerializeField] private float sideBlockOffset = 0.65f;

    [Header("Side Decoration Settings")]
    [SerializeField] private bool createSideDecorations = true;
    [SerializeField] private int sideDecorationCountPerSegment = 4;
    [SerializeField] private float sideDecorationOffset = 2.0f;
    [SerializeField] private float sideDecorationMinHeight = 0.8f;
    [SerializeField] private float sideDecorationMaxHeight = 2.2f;
    [SerializeField] private float sideDecorationWidth = 0.25f;
    [SerializeField] private float sideDecorationDepth = 0.25f;
    [SerializeField] private float sideDecorationYPosition = 0.4f;
    [SerializeField] private bool randomizeSideDecorationHeights = true;

    [Header("Side Floor Light Settings")]
    [SerializeField] private bool createSideFloorLights = true;
    [SerializeField] private int sideFloorLightCountPerSegment = 8;
    [SerializeField] private float sideFloorLightOffset = 1.2f;
    [SerializeField] private float sideFloorLightWidth = 0.12f;
    [SerializeField] private float sideFloorLightHeight = 0.04f;
    [SerializeField] private float sideFloorLightLength = 0.9f;
    [SerializeField] private float sideFloorLightYPosition = -0.22f;

    [Header("Colors")]
    [SerializeField] private Color roadNormalColor = new Color(0.12f, 0.12f, 0.12f);
    [SerializeField] private Color roadBeatColor = new Color(0.25f, 0.25f, 0.25f);

    [SerializeField] private Color neonNormalColor = new Color(0f, 0.8f, 1f);
    [SerializeField] private Color neonBeatColor = Color.white;

    [Header("Combo Atmosphere Colors")]
    [SerializeField] private bool useComboAtmosphere = true;
    [SerializeField] private int comboLevelOne = 5;
    [SerializeField] private int comboLevelTwo = 10;
    [SerializeField] private int comboLevelThree = 20;

    [SerializeField] private Color comboLevelOneNeonColor = new Color(0f, 1f, 1f);
    [SerializeField] private Color comboLevelTwoNeonColor = new Color(1f, 0f, 1f);
    [SerializeField] private Color comboLevelThreeNeonColor = new Color(1f, 0.75f, 0f);

    [SerializeField] private Color comboLevelOneRoadColor = new Color(0.10f, 0.16f, 0.18f);
    [SerializeField] private Color comboLevelTwoRoadColor = new Color(0.16f, 0.10f, 0.20f);
    [SerializeField] private Color comboLevelThreeRoadColor = new Color(0.20f, 0.16f, 0.08f);

    [Header("Beat Reaction")]
    [SerializeField] private float returnSpeed = 6f;

    [Header("Beat Scale Pulse")]
    [SerializeField] private bool useBeatScalePulse = true;
    [SerializeField] private float neonBeatScale = 1.15f;
    [SerializeField] private float comboExtraPulseAmount = 0.2f;
    [SerializeField] private float scaleReturnSpeed = 8f;

    private Renderer roadRenderer;
    private readonly List<Renderer> neonRenderers = new List<Renderer>();
    private readonly List<Transform> neonTransforms = new List<Transform>();
    private readonly List<Vector3> neonOriginalScales = new List<Vector3>();

    private bool visualsCreated = false;
    private bool rhythmSubscribed = false;
    private bool scoreSubscribed = false;

    private int currentCombo = 0;

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
        TrySubscribeToScoreManager();
    }

    private void OnEnable()
    {
        TrySubscribeToRhythmManager();
        TrySubscribeToScoreManager();
    }

    private void OnDisable()
    {
        UnsubscribeFromRhythmManager();
        UnsubscribeFromScoreManager();
    }

    private void OnDestroy()
    {
        UnsubscribeFromRhythmManager();
        UnsubscribeFromScoreManager();
    }

    private void Update()
    {
        if (!rhythmSubscribed)
        {
            TrySubscribeToRhythmManager();
        }

        if (!scoreSubscribed)
        {
            TrySubscribeToScoreManager();
        }

        UpdateColors();
        UpdateScales();
    }

    private void TrySubscribeToRhythmManager()
    {
        if (rhythmSubscribed)
        {
            return;
        }

        if (RhythmManager.Instance == null)
        {
            return;
        }

        RhythmManager.Instance.OnBeat += HandleBeat;
        rhythmSubscribed = true;
    }

    private void TrySubscribeToScoreManager()
    {
        if (scoreSubscribed)
        {
            return;
        }

        if (ScoreManager.Instance == null)
        {
            return;
        }

        ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
        scoreSubscribed = true;

        currentCombo = ScoreManager.Instance.Combo;
    }

    private void UnsubscribeFromRhythmManager()
    {
        if (RhythmManager.Instance != null && rhythmSubscribed)
        {
            RhythmManager.Instance.OnBeat -= HandleBeat;
            rhythmSubscribed = false;
        }
    }

    private void UnsubscribeFromScoreManager()
    {
        if (ScoreManager.Instance != null && scoreSubscribed)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            scoreSubscribed = false;
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

        if (createCenterGlowLine)
        {
            CreateCenterGlowLine();
        }

        if (createOuterRails)
        {
            CreateOuterRail("Outer Left Rail", -segmentWidth / 2f - outerRailOffset);
            CreateOuterRail("Outer Right Rail", segmentWidth / 2f + outerRailOffset);
        }

        if (createSideBlocks)
        {
            CreateSideBlocks();
        }
        if (createSideDecorations)
        {
            CreateSideDecorations();
        }
        if (createSideFloorLights)
        {
            CreateSideFloorLights();
        }
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

        GameObject barrier = CreateVisualCube(objectName, worldPosition, worldScale, GetCurrentNeonBaseColor());
        AddNeonVisual(barrier);
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

        GameObject laneLine = CreateVisualCube(objectName, worldPosition, worldScale, GetCurrentNeonBaseColor());
        AddNeonVisual(laneLine);
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

    private void AddNeonVisual(GameObject visualObject)
    {
        if (visualObject == null)
        {
            return;
        }

        Renderer renderer = visualObject.GetComponent<Renderer>();

        if (renderer != null)
        {
            neonRenderers.Add(renderer);
        }

        neonTransforms.Add(visualObject.transform);
        neonOriginalScales.Add(visualObject.transform.localScale);
    }

    private void HandleBeat(int beatIndex)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        if (roadRenderer != null)
        {
            roadRenderer.material.color = roadBeatColor;
        }

        Color beatColor = GetCurrentNeonBeatColor();

        for (int i = 0; i < neonRenderers.Count; i++)
        {
            if (neonRenderers[i] != null)
            {
                neonRenderers[i].material.color = beatColor;
            }
        }

        if (useBeatScalePulse)
        {
            PulseNeonScales();
        }
    }

    private void HandleScoreChanged(int score, int combo, int highestCombo)
    {
        currentCombo = combo;
    }

    private void PulseNeonScales()
    {
        float comboPulseBonus = 0f;

        if (useComboAtmosphere)
        {
            comboPulseBonus = Mathf.Clamp01(currentCombo / 20f) * comboExtraPulseAmount;
        }

        float targetScale = neonBeatScale + comboPulseBonus;

        for (int i = 0; i < neonTransforms.Count; i++)
        {
            if (neonTransforms[i] == null)
            {
                continue;
            }

            if (i >= neonOriginalScales.Count)
            {
                continue;
            }

            neonTransforms[i].localScale = neonOriginalScales[i] * targetScale;
        }
    }

    private void UpdateColors()
    {
        if (roadRenderer != null)
        {
            roadRenderer.material.color = Color.Lerp(
                roadRenderer.material.color,
                GetCurrentRoadBaseColor(),
                returnSpeed * Time.deltaTime
            );
        }

        Color targetNeonColor = GetCurrentNeonBaseColor();

        for (int i = 0; i < neonRenderers.Count; i++)
        {
            if (neonRenderers[i] == null)
            {
                continue;
            }

            neonRenderers[i].material.color = Color.Lerp(
                neonRenderers[i].material.color,
                targetNeonColor,
                returnSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateScales()
    {
        for (int i = 0; i < neonTransforms.Count; i++)
        {
            if (neonTransforms[i] == null)
            {
                continue;
            }

            if (i >= neonOriginalScales.Count)
            {
                continue;
            }

            neonTransforms[i].localScale = Vector3.Lerp(
                neonTransforms[i].localScale,
                neonOriginalScales[i],
                scaleReturnSpeed * Time.deltaTime
            );
        }
    }

    private Color GetCurrentNeonBaseColor()
    {
        if (!useComboAtmosphere)
        {
            return neonNormalColor;
        }

        if (currentCombo >= comboLevelThree)
        {
            return comboLevelThreeNeonColor;
        }

        if (currentCombo >= comboLevelTwo)
        {
            return comboLevelTwoNeonColor;
        }

        if (currentCombo >= comboLevelOne)
        {
            return comboLevelOneNeonColor;
        }

        return neonNormalColor;
    }

    private Color GetCurrentRoadBaseColor()
    {
        if (!useComboAtmosphere)
        {
            return roadNormalColor;
        }

        if (currentCombo >= comboLevelThree)
        {
            return comboLevelThreeRoadColor;
        }

        if (currentCombo >= comboLevelTwo)
        {
            return comboLevelTwoRoadColor;
        }

        if (currentCombo >= comboLevelOne)
        {
            return comboLevelOneRoadColor;
        }

        return roadNormalColor;
    }

    private Color GetCurrentNeonBeatColor()
    {
        if (!useComboAtmosphere)
        {
            return neonBeatColor;
        }

        Color comboColor = GetCurrentNeonBaseColor();

        return Color.Lerp(
            neonBeatColor,
            comboColor,
            0.45f
        );
    }

    private void CreateCenterGlowLine()
    {
        Vector3 worldPosition = transform.position + new Vector3(
            0f,
            centerLineYPosition,
            0f
        );

        Vector3 worldScale = new Vector3(
            centerLineWidth,
            centerLineHeight,
            segmentLength
        );

        GameObject centerLine = CreateVisualCube(
            "Center Glow Line",
            worldPosition,
            worldScale,
            GetCurrentNeonBaseColor()
        );

        AddNeonVisual(centerLine);
    }

    private void CreateOuterRail(string objectName, float xPosition)
    {
        Vector3 worldPosition = transform.position + new Vector3(
            xPosition,
            barrierYPosition + 0.15f,
            0f
        );

        Vector3 worldScale = new Vector3(
            outerRailWidth,
            outerRailHeight,
            segmentLength
        );

        GameObject outerRail = CreateVisualCube(
            objectName,
            worldPosition,
            worldScale,
            GetCurrentNeonBaseColor()
        );

        AddNeonVisual(outerRail);
    }

    private void CreateSideBlocks()
    {
        if (sideBlockCountPerSegment <= 0)
        {
            return;
        }

        float spacing = segmentLength / sideBlockCountPerSegment;

        for (int i = 0; i < sideBlockCountPerSegment; i++)
        {
            float zOffset = -segmentLength / 2f + spacing * i + spacing / 2f;

            CreateSideBlock(
                "Left Side Block " + i,
                -segmentWidth / 2f - sideBlockOffset,
                zOffset
            );

            CreateSideBlock(
                "Right Side Block " + i,
                segmentWidth / 2f + sideBlockOffset,
                zOffset
            );
        }
    }

    private void CreateSideBlock(string objectName, float xPosition, float zOffset)
    {
        Vector3 worldPosition = transform.position + new Vector3(
            xPosition,
            barrierYPosition + 0.15f,
            zOffset
        );

        Vector3 worldScale = new Vector3(
            sideBlockWidth,
            sideBlockHeight,
            sideBlockLength
        );

        GameObject sideBlock = CreateVisualCube(
            objectName,
            worldPosition,
            worldScale,
            GetCurrentNeonBaseColor()
        );

        AddNeonVisual(sideBlock);
    }

    private void CreateSideDecorations()
    {
        if (sideDecorationCountPerSegment <= 0)
        {
            return;
        }

        float spacing = segmentLength / sideDecorationCountPerSegment;

        for (int i = 0; i < sideDecorationCountPerSegment; i++)
        {
            float zOffset = -segmentLength / 2f + spacing * i + spacing / 2f;

            CreateSideDecoration(
                "Left Neon Pillar " + i,
                -segmentWidth / 2f - sideDecorationOffset,
                zOffset,
                i
            );

            CreateSideDecoration(
                "Right Neon Pillar " + i,
                segmentWidth / 2f + sideDecorationOffset,
                zOffset,
                i
            );
        }
    }

    private void CreateSideDecoration(string objectName, float xPosition, float zOffset, int index)
    {
        float decorationHeight = sideDecorationMaxHeight;

        if (randomizeSideDecorationHeights)
        {
            float noiseValue = Mathf.PerlinNoise(
                transform.position.z * 0.05f,
                index * 0.35f
            );

            decorationHeight = Mathf.Lerp(
                sideDecorationMinHeight,
                sideDecorationMaxHeight,
                noiseValue
            );
        }

        Vector3 worldPosition = transform.position + new Vector3(
            xPosition,
            sideDecorationYPosition + decorationHeight / 2f,
            zOffset
        );

        Vector3 worldScale = new Vector3(
            sideDecorationWidth,
            decorationHeight,
            sideDecorationDepth
        );

        GameObject decoration = CreateVisualCube(
            objectName,
            worldPosition,
            worldScale,
            GetCurrentNeonBaseColor()
        );

        AddNeonVisual(decoration);
    }
    private void CreateSideFloorLights()
    {
        if (sideFloorLightCountPerSegment <= 0)
        {
            return;
        }

        float spacing = segmentLength / sideFloorLightCountPerSegment;

        for (int i = 0; i < sideFloorLightCountPerSegment; i++)
        {
            float zOffset = -segmentLength / 2f + spacing * i + spacing / 2f;

            CreateSideFloorLight(
                "Left Floor Light " + i,
                -segmentWidth / 2f - sideFloorLightOffset,
                zOffset
            );

            CreateSideFloorLight(
                "Right Floor Light " + i,
                segmentWidth / 2f + sideFloorLightOffset,
                zOffset
            );
        }
    }

    private void CreateSideFloorLight(string objectName, float xPosition, float zOffset)
    {
        Vector3 worldPosition = transform.position + new Vector3(
            xPosition,
            sideFloorLightYPosition,
            zOffset
        );

        Vector3 worldScale = new Vector3(
            sideFloorLightWidth,
            sideFloorLightHeight,
            sideFloorLightLength
        );

        GameObject floorLight = CreateVisualCube(
            objectName,
            worldPosition,
            worldScale,
            GetCurrentNeonBaseColor()
        );

        AddNeonVisual(floorLight);
    }
}