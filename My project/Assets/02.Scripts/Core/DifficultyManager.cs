using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    [Header("Obstacle Difficulty")]
    [SerializeField] private float obstacleBaseSpawnRate = 0.20f;
    [SerializeField] private float obstacleIncreasePerStep = 0.04f;
    [SerializeField] private float obstacleMaxSpawnRate = 0.40f;

    [Header("Missile Difficulty")]
    [SerializeField] private float missileBaseMinChance = 0.10f;
    [SerializeField] private float missileIncreasePerStep = 0.05f;
    [SerializeField] private float missileMaxMinChance = 0.35f;
    [SerializeField] private float missileMaxChance = 0.50f;

    [Header("Progress")]
    [SerializeField] private float distancePerStep = 500f;

    private float startZ = 0f;

    public float CurrentObstacleSpawnRate => CalculateObstacleSpawnRate();
    public float CurrentMissileMinChance => CalculateMissileMinChance();
    public float CurrentMissileMaxChance => missileMaxChance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerTransform == null && PlayerMovement.Instance != null)
        {
            playerTransform = PlayerMovement.Instance.transform;
        }
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (playerTransform == null && PlayerMovement.Instance != null)
        {
            playerTransform = PlayerMovement.Instance.transform;
        }

        startZ = playerTransform != null ? playerTransform.position.z : 0f;
    }

    public int GetDifficultyStep()
    {
        if (playerTransform == null)
            return 0;

        float traveledZ = Mathf.Max(0f, playerTransform.position.z - startZ);
        return Mathf.FloorToInt(traveledZ / distancePerStep);
    }

    private float CalculateObstacleSpawnRate()
    {
        int step = GetDifficultyStep();
        float rate = obstacleBaseSpawnRate + (obstacleIncreasePerStep * step);
        return Mathf.Clamp(rate, obstacleBaseSpawnRate, obstacleMaxSpawnRate);
    }

    private float CalculateMissileMinChance()
    {
        int step = GetDifficultyStep();
        float chance = missileBaseMinChance + (missileIncreasePerStep * step);
        return Mathf.Clamp(chance, missileBaseMinChance, missileMaxMinChance);
    }
}