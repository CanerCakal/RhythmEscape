using UnityEngine;

public class BeatVisualizer : MonoBehaviour
{
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        BeatManager.OnBeat += Flash;
    }

    void OnDisable()
    {
        BeatManager.OnBeat -= Flash;
    }

    void Flash()
    {
        objectRenderer.material.color = Random.ColorHSV();
    }
}