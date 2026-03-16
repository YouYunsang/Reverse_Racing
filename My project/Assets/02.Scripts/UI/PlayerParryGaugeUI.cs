using UnityEngine;
using UnityEngine.UI;

public class PlayerParryGaugeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerParryGauge playerParryGauge;
    [SerializeField] private PlayerOverdrive playerOverdrive;
    [SerializeField] private Image[] gaugeCells;

    [Header("Colors")]
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color filledColor = new Color(1f, 0.85f, 0.2f, 1f);

    [Header("Full Gauge Glow")]
    [SerializeField] private bool useFullGaugeGlow = true;
    [SerializeField] private float glowSpeed = 4f;
    [SerializeField] private Color fullGlowColorA = new Color(1f, 0.9f, 0.4f, 1f);
    [SerializeField] private Color fullGlowColorB = new Color(1f, 1f, 1f, 1f);

    [Header("Full Gauge Pulse")]
    [SerializeField] private bool useFullGaugePulse = true;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseScaleMultiplier = 1.12f;

    private bool isGaugeFull = false;
    private bool isGlowOrPulseActive = false;
    private int currentGaugeCount = 0;

    private Vector3[] defaultScales;

    private void Awake()
    {
        if (playerParryGauge == null)
        {
            playerParryGauge = FindFirstObjectByType<PlayerParryGauge>();
        }

        if (playerOverdrive == null)
        {
            playerOverdrive = FindFirstObjectByType<PlayerOverdrive>();
        }

        CacheDefaultScales();
    }

    private void OnEnable()
    {
        if (playerParryGauge != null)
        {
            playerParryGauge.OnGaugeChanged += HandleGaugeChanged;
        }
    }

    private void Start()
    {
        if (playerParryGauge != null)
        {
            HandleGaugeChanged(playerParryGauge.CurrentGauge, playerParryGauge.MaxGauge);
        }
        else
        {
            RefreshGaugeVisual(0, 10);
        }
    }

    private void OnDisable()
    {
        if (playerParryGauge != null)
        {
            playerParryGauge.OnGaugeChanged -= HandleGaugeChanged;
        }
    }

    private void Update()
    {
        bool isOverdriveActive = playerOverdrive != null && playerOverdrive.IsOverdriveActive;
        isGlowOrPulseActive = isGaugeFull || isOverdriveActive;

        // 매 프레임 기본 상태를 먼저 반영
        ApplyBaseVisual();

        if (!isGlowOrPulseActive)
            return;

        if (useFullGaugeGlow)
        {
            UpdateGlow();
        }

        if (useFullGaugePulse)
        {
            UpdatePulse();
        }
    }

    private void HandleGaugeChanged(int currentGauge, int maxGauge)
    {
        RefreshGaugeVisual(currentGauge, maxGauge);
    }

    private void RefreshGaugeVisual(int currentGauge, int maxGauge)
    {
        currentGaugeCount = Mathf.Clamp(currentGauge, 0, gaugeCells.Length);
        isGaugeFull = currentGauge >= maxGauge && maxGauge > 0;

        ApplyBaseVisual();
    }

    private void ApplyBaseVisual()
    {
        if (gaugeCells == null)
            return;

        for (int i = 0; i < gaugeCells.Length; i++)
        {
            if (gaugeCells[i] == null)
                continue;

            bool isActiveCell = i < currentGaugeCount;

            gaugeCells[i].color = isActiveCell ? filledColor : emptyColor;
            gaugeCells[i].rectTransform.localScale = defaultScales[i];
        }
    }

    private void UpdateGlow()
    {
        float t = (Mathf.Sin(Time.unscaledTime * glowSpeed) + 1f) * 0.5f;
        Color glowColor = Color.Lerp(fullGlowColorA, fullGlowColorB, t);

        for (int i = 0; i < gaugeCells.Length; i++)
        {
            if (gaugeCells[i] == null)
                continue;

            // 현재 남아 있는 칸만 glow
            if (i < currentGaugeCount)
            {
                gaugeCells[i].color = glowColor;
            }
        }
    }

    private void UpdatePulse()
    {
        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
        float scaleMultiplier = Mathf.Lerp(1f, pulseScaleMultiplier, t);

        for (int i = 0; i < gaugeCells.Length; i++)
        {
            if (gaugeCells[i] == null)
                continue;

            // 현재 남아 있는 칸만 pulse
            if (i < currentGaugeCount)
            {
                gaugeCells[i].rectTransform.localScale = defaultScales[i] * scaleMultiplier;
            }
        }
    }

    private void CacheDefaultScales()
    {
        if (gaugeCells == null)
            return;

        defaultScales = new Vector3[gaugeCells.Length];

        for (int i = 0; i < gaugeCells.Length; i++)
        {
            if (gaugeCells[i] != null)
            {
                defaultScales[i] = gaugeCells[i].rectTransform.localScale;
            }
            else
            {
                defaultScales[i] = Vector3.one;
            }
        }
    }
}