using System;
using UnityEngine;

public class PlayerFuel : MonoBehaviour
{
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float currentFuel;
    [SerializeField] private float fuelDrainPerSecond = 1f;

    public float MaxFuel => maxFuel;
    public float CurrentFuel => currentFuel;
    public float FuelNormalized => maxFuel <= 0f ? 0f : currentFuel / maxFuel;

    public event Action<float, float> OnFuelChanged;

    private bool isGameOverTriggered = false;

    private void Start()
    {
        currentFuel = maxFuel;
        NotifyFuelChanged();
    }

    private void Update()
    {
        DrainFuelOverTime();
        CheckGameOver();
    }

    private void DrainFuelOverTime()
    {
        float prevFuel = currentFuel;
        currentFuel -= fuelDrainPerSecond * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        if (!Mathf.Approximately(prevFuel, currentFuel))
        {
            NotifyFuelChanged();
        }
    }

    private void CheckGameOver()
    {
        if (isGameOverTriggered) return;

        if(currentFuel <= 0f)
        {
            isGameOverTriggered = true;

            GameManager.Instance.GameOver();

            Debug.Log("연료가 모두 소모되어 게임 오버");
        }
    }

    public void ConsumeFuel(float amount)
    {
        if (amount <= 0f) return;

        float prevFuel = currentFuel;

        currentFuel -= amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        if (!Mathf.Approximately(prevFuel, currentFuel))
        {
            NotifyFuelChanged();
        }

        CheckGameOver();
    }

    public void RecoverFuel(float amount)
    {
        if (amount <= 0f) return;

        float prevFuel = currentFuel;

        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        if(!Mathf.Approximately(prevFuel, currentFuel))
        {
            NotifyFuelChanged();
        }
    }

    public void SetFuelDrainRate(float newDrainRate)
    {
        fuelDrainPerSecond = Mathf.Max(0f, newDrainRate);
    }

    private void NotifyFuelChanged()
    {
        OnFuelChanged?.Invoke(currentFuel, maxFuel);
    }
}
