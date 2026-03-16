using UnityEngine;

public class SpawnGate : MonoBehaviour
{
    public static SpawnGate Instance { get; private set; }

    public bool IsGameplayStarted { get; private set; } = false;
    public bool IsSpawnAllowed { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetGameplayStarted(bool value)
    {
        IsGameplayStarted = value;
    }

    public void SetSpawnAllowed(bool value)
    {
        IsSpawnAllowed = value;
    }
}