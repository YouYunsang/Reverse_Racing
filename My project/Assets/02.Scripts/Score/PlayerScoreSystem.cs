using UnityEngine;

public class PlayerScoreSystem : MonoBehaviour
{
    [Header("Distance Score")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distanceScoreMultiplier = 1f;
    [SerializeField] private float startZ = 0f;

    [Header("Runtime")]
    [SerializeField] private int distanceScore = 0;
    [SerializeField] private int parryScore = 0;
    [SerializeField] private int totalScore = 0;

    private float maxReachedZ;

    public int DistanceScore => distanceScore;
    public int ParryScore => parryScore;
    public int TotalScore => totalScore;

    private void Awake()
    {
        if (playerTransform == null && PlayerMovement.Instance != null)
        {
            playerTransform = PlayerMovement.Instance.transform;
        }
    }

    private void Start()
    {
        InitializeScore();
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            return;

        UpdateDistanceScore();
        UpdateTotalScore();
    }

    public void InitializeScore()
    {
        if (playerTransform == null && PlayerMovement.Instance != null)
        {
            playerTransform = PlayerMovement.Instance.transform;
        }

        startZ = playerTransform != null ? playerTransform.position.z : 0f;
        maxReachedZ = startZ;

        distanceScore = 0;
        parryScore = 0;
        totalScore = 0;
    }

    private void UpdateDistanceScore()
    {
        if (playerTransform == null)
            return;

        if (playerTransform.position.z > maxReachedZ)
        {
            maxReachedZ = playerTransform.position.z;
        }

        float traveledDistance = maxReachedZ - startZ;
        traveledDistance = Mathf.Max(0f, traveledDistance);

        distanceScore = Mathf.FloorToInt(traveledDistance * distanceScoreMultiplier);
    }

    public void AddParryScore(int amount)
    {
        if (amount <= 0)
            return;

        parryScore += amount;
        UpdateTotalScore();
    }

    private void UpdateTotalScore()
    {
        totalScore = distanceScore + parryScore;
    }
}