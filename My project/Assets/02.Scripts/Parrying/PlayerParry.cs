using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode parryKey = KeyCode.Space;

    [Header("Parry Detection")]
    [SerializeField] private LayerMask parryableLayer;
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 1f);
    [SerializeField] private float forwardOffset = 1f;
    [SerializeField] private float parryCooldown = 0.2f;

    [Header("Parry Force")]
    [SerializeField] private float parryForce = 10f;
    [SerializeField] private float upwardForce = 4f;
    [SerializeField] private float torqueForce = 12f;

    [Header("Parry VFX")]
    [SerializeField] private ParryMesh parryMeshPrefab;
    [SerializeField] private float vfxLifeTime = 0.1f;
    [SerializeField] private float vfxRange = 3f;
    [SerializeField] private float vfxAngle = 30f;

    [SerializeField] private PlayerBooster playerBooster;
    [SerializeField] private PlayerParryGauge playerParryGauge;

    private float lastParryTime = -999f;
    private readonly Collider[] hitBuffer = new Collider[16];

    [SerializeField] private PlayerScoreSystem playerScoreSystem;

    private void Awake()
    {
        if (playerBooster == null)
        {
            playerBooster = GetComponent<PlayerBooster>();
        }

        if (playerParryGauge == null)
        {
            playerParryGauge = GetComponent<PlayerParryGauge>();
        }

        if (playerScoreSystem == null)
        {
            playerScoreSystem = GetComponent<PlayerScoreSystem>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            return;

        if (Input.GetKeyDown(parryKey))
        {
            TryParry();
        }
    }

    private void TryParry()
    {
        if (Time.time < lastParryTime + parryCooldown)
            return;

        lastParryTime = Time.time;

        Vector3 parryDirection = transform.forward;
        Vector3 center = transform.position + parryDirection * forwardOffset;

        int hitCount = Physics.OverlapBoxNonAlloc(
            center,
            boxHalfExtents,
            hitBuffer,
            transform.rotation,
            parryableLayer
        );

        bool hitSomething = false;
        int totalGaugeReward = 0;
        int totalScoreReward = 0;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = hitBuffer[i];
            if (col == null) continue;

            IParryable parryable = col.GetComponentInParent<IParryable>();
            if (parryable == null) continue;

            parryable.OnParried(parryDirection, parryForce, upwardForce, torqueForce);
            hitSomething = true;

            if (parryable is IParryGaugeReward gaugeReward)
            {
                totalGaugeReward += gaugeReward.GetGaugeRewardAmount();
            }

            if (parryable is IParryScoreReward scoreReward)
            {
                totalScoreReward += scoreReward.GetParryScoreReward();
            }
        }

        if (hitSomething)
        {
            if (playerBooster != null)
            {
                playerBooster.ActivateBoost();
            }

            if (playerParryGauge != null && totalGaugeReward > 0)
            {
                playerParryGauge.AddGauge(totalGaugeReward);
            }

            if (playerScoreSystem != null && totalScoreReward > 0)
            {
                playerScoreSystem.AddParryScore(totalScoreReward);
            }
        }

        SpawnParryVFX(parryDirection, hitSomething ? Color.yellow : Color.white);
    }

    private void SpawnParryVFX(Vector3 forward, Color color)
    {
        if (parryMeshPrefab == null)
            return;

        ParryMesh parryMesh = Instantiate(
            parryMeshPrefab,
            transform.position,
            Quaternion.LookRotation(forward)
        );

        parryMesh.Setup(vfxRange, vfxAngle, vfxLifeTime, color);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position + transform.forward * forwardOffset;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
        Gizmos.matrix = oldMatrix;
    }
}