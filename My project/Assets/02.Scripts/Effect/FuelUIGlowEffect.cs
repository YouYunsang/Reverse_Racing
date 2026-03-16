using UnityEngine;
using UnityEngine.UI;

public class FuelUIGlowEffect : MonoBehaviour
{
    [Header("Glow Targets")]
    [SerializeField] private Image backgroundGlowImage;
    [SerializeField] private Image fillGlowImage;

    [Header("Glow Colors")]
    [SerializeField] private Color backgroundGlowColorA = new Color(1f, 0.85f, 0.3f, 0.25f);
    [SerializeField] private Color backgroundGlowColorB = new Color(1f, 1f, 0.6f, 0.45f);

    [SerializeField] private Color fillGlowColorA = new Color(1f, 0.85f, 0.3f, 0.35f);
    [SerializeField] private Color fillGlowColorB = new Color(1f, 1f, 0.8f, 0.65f);

    [Header("Pulse")]
    [SerializeField] private float glowSpeed = 2.5f;
    [SerializeField] private bool useScalePulse = true;
    [SerializeField] private float scalePulseMultiplier = 1.03f;

    private RectTransform backgroundGlowRect;
    private RectTransform fillGlowRect;

    private Vector3 backgroundDefaultScale = Vector3.one;
    private Vector3 fillDefaultScale = Vector3.one;

    private void Awake()
    {
        if (backgroundGlowImage != null)
        {
            backgroundGlowRect = backgroundGlowImage.rectTransform;
            backgroundDefaultScale = backgroundGlowRect.localScale;
        }

        if (fillGlowImage != null)
        {
            fillGlowRect = fillGlowImage.rectTransform;
            fillDefaultScale = fillGlowRect.localScale;
        }
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * glowSpeed) + 1f) * 0.5f;

        UpdateGlowImage(backgroundGlowImage, backgroundGlowRect, backgroundGlowColorA, backgroundGlowColorB, backgroundDefaultScale, t);
        UpdateGlowImage(fillGlowImage, fillGlowRect, fillGlowColorA, fillGlowColorB, fillDefaultScale, t);
    }

    private void UpdateGlowImage(
        Image targetImage,
        RectTransform targetRect,
        Color colorA,
        Color colorB,
        Vector3 defaultScale,
        float t)
    {
        if (targetImage == null)
            return;

        targetImage.color = Color.Lerp(colorA, colorB, t);

        if (useScalePulse && targetRect != null)
        {
            float scale = Mathf.Lerp(1f, scalePulseMultiplier, t);
            targetRect.localScale = defaultScale * scale;
        }
    }
}