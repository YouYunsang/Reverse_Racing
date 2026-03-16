using System;
using UnityEngine;

public class PlayerParryGauge : MonoBehaviour
{
    [SerializeField] private int maxGauge = 10;
    [SerializeField] private int currentGauge = 0;

    public int MaxGauge => maxGauge;
    public int CurrentGauge => currentGauge;
    public bool IsFull => currentGauge >= maxGauge;
    public float Normalized => maxGauge <= 0 ? 0f : (float)currentGauge/maxGauge;

    public event Action<int, int> OnGaugeChanged;

    public void AddGauge(int amount)
    {
        if (amount <= 0) return;

        if (IsFull) return;

        int prevGauge = currentGauge;
        currentGauge += amount;
        currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);

        if (prevGauge != currentGauge)
        {
            NotifyGaugeChanged();
        }
    }

    public bool IsMaxFull()
    {
        return currentGauge >= maxGauge;
    }

    public bool ConsumeAllGauge()
    {
        if(currentGauge <= 0) return false;

        currentGauge = 0;
        NotifyGaugeChanged();
        return true;
    }

    public bool ConsumeGauge(int amount)
    {
        if (amount <= 0)
            return false;

        if (currentGauge < amount)
            return false;

        currentGauge -= amount;
        currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);
        NotifyGaugeChanged();
        return true;
    }

    public void SetGauge(int amount)
    {
        currentGauge = Mathf.Clamp(amount, 0, maxGauge);
        NotifyGaugeChanged();
    }

    private void NotifyGaugeChanged()
    {
        OnGaugeChanged?.Invoke(currentGauge, maxGauge);
    }
}