using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressedScale = 0.97f;
    [SerializeField] private float animationSpeed = 12f;

    [Header("Color Settings")]
    [SerializeField] private bool animateColor = true;
    [SerializeField] private Color hoverColor = Color.white;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private Image buttonImage;
    private Color originalColor;
    private Color targetColor;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            targetColor = originalColor;
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            animationSpeed * Time.unscaledDeltaTime
        );

        if (animateColor && buttonImage != null)
        {
            buttonImage.color = Color.Lerp(
                buttonImage.color,
                targetColor,
                animationSpeed * Time.unscaledDeltaTime
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        if (animateColor && buttonImage != null)
        {
            targetColor = Color.Lerp(originalColor, hoverColor, 0.25f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;

        if (animateColor && buttonImage != null)
        {
            targetColor = originalColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }
}