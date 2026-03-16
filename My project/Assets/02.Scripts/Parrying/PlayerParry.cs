using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    private enum ParryDirectionMode
    {
        Forward, Backward

    }
    [Header("Input")]
    [SerializeField] private readonly KeyCode parryKey = KeyCode.Space;
    [SerializeField] private readonly KeyCode forwardDirectionKey = KeyCode.UpArrow;
    [SerializeField] private readonly KeyCode backwardDirectionKey = KeyCode.DownArrow;

    [Header("Parry Detection")]
    [SerializeField] private LayerMask parryableLayer;
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 1f);
    //패링 박스의 중심과 플레이어 중심 사이의 거리
    [SerializeField] private float forwardOffset = 1f;
    [SerializeField] private float backwardOffset = 1f;
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

    [SerializeField] private ParryDirectionMode currentDirectionMode = ParryDirectionMode.Forward;

    [SerializeField] private PlayerParryGauge playerParryGauge;

    private float lastParryTime = -999f;
    private readonly Collider[] hitBuffer = new Collider[16];

    private void Awake()
    {
        if (playerBooster == null)
        {
            playerBooster = gameObject.GetComponent<PlayerBooster>();
        }

        if(playerParryGauge == null)
        {
            playerParryGauge = GetComponent<PlayerParryGauge>();
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

        HandleDirectionInput();
    }

    private void HandleDirectionInput()
    {
        if (Input.GetKeyDown(forwardDirectionKey))
        {
            currentDirectionMode = ParryDirectionMode.Forward;
        }
        else if (Input.GetKeyDown(backwardDirectionKey))
        {
            currentDirectionMode = ParryDirectionMode.Backward;
        }
    }

    private void TryParry()
    {
        if (Time.time < lastParryTime + parryCooldown) return;
        lastParryTime = Time.time;

        Vector3 parryDirection = GetCurrentParryDirection();
        // offset 변수 정상 적용
        float offset = currentDirectionMode == ParryDirectionMode.Forward ? forwardOffset : backwardOffset;
        Vector3 center = transform.position + parryDirection * offset;

        int hitCount = Physics.OverlapBoxNonAlloc(center, boxHalfExtents, hitBuffer, transform.rotation, parryableLayer);
        bool hitSomething = false;
        int totalGaugeReward = 0;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = hitBuffer[i];
            if (col == null) continue;

            IParryable parryable = col.GetComponentInParent<IParryable>();
            if (parryable == null) continue;

            parryable.OnParried(parryDirection, parryForce, upwardForce, torqueForce);
            hitSomething = true;

            // 인터페이스 형변환으로 보상 획득
            if (parryable is IParryGaugeReward gaugeReward)
            {
                int reward = gaugeReward.GetGaugeRewardAmount();
                totalGaugeReward += reward;
                Debug.Log($"보상 획득 예정: {reward}, 누적: {totalGaugeReward}");
            }
        }

        if (hitSomething)
        {
            if (playerBooster != null) playerBooster.ActivateBoost();

            if (playerParryGauge != null && totalGaugeReward > 0)
            {
                playerParryGauge.AddGauge(totalGaugeReward);
                Debug.Log($"게이지 {totalGaugeReward} 증가 완료!");
            }
            else if (playerParryGauge == null)
            {
                Debug.LogError("PlayerParryGauge 참조가 비어있습니다!");
            }
        }

        SpawnParryVFX(parryDirection, hitSomething ? Color.yellow : Color.white);
    }

    private Vector3 GetCurrentParryDirection()
    {
        return currentDirectionMode == ParryDirectionMode.Forward ? transform.forward : -transform.forward;
    }

    private void SpawnParryVFX(Vector3 forward, Color color)
    {
        if (parryMeshPrefab == null)
            return;

        ParryMesh parryMesh = Instantiate(parryMeshPrefab, transform.position, Quaternion.LookRotation(forward));
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