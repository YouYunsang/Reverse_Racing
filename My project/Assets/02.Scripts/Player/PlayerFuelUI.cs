using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFuelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFuel playerFuel;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TMP_Text fuelText;

    private void Awake()
    {
        if (playerFuel == null)
        {
            playerFuel = FindFirstObjectByType<PlayerFuel>();
        }
    }

    private void OnEnable()
    {
        if (playerFuel != null)
        {
            playerFuel.OnFuelChanged += HandleFuelChanged;
        }
    }

    private void Start()
    {
        if (playerFuel != null)
        {
            HandleFuelChanged(playerFuel.CurrentFuel, playerFuel.MaxFuel);
        }
    }

    private void OnDisable()
    {
        if (playerFuel != null)
        {
            playerFuel.OnFuelChanged -= HandleFuelChanged;
        }
    }

    private void HandleFuelChanged(float currentFuel, float maxFuel)
    {
        if (fuelSlider != null)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = currentFuel;
        }

        if (fuelText != null)
        {
            fuelText.text = $"{Mathf.CeilToInt(currentFuel)} / {Mathf.CeilToInt(maxFuel)}";
        }
    }
}