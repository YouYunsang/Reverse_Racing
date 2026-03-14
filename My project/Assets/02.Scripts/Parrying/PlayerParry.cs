using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode parryKey = KeyCode.Space;

    [Header("Parry Detection")]
    [SerializeField] private LayerMask parryableLayer;
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);
    //패링 박스의 중심과 플레이어 중심 사이의 거리
    [SerializeField] private float forwardOffset = 1f;
    [SerializeField] private float parryCooldown = 0.2f;

    [Header("Parry Force")]
    [SerializeField] private float parryForce = 10f;
    [SerializeField] private float upwardForce = 4f;
    [SerializeField] private float torqueForce = 12f;

    [Header("Parry VFX")]
    [SerializeField] private ParryMesh parryMeshPrefab;
    [SerializeField] private float vfxLifeTime = 0.1f;
    [SerializeField] private float vfxRange = 1f;
    [SerializeField] private float vfxAngle = 30f;

    private float lastParryTime = -999f;
    private readonly Collider[] hitBuffer = new Collider[16];

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
        Debug.Log("TryParry");
        if (Time.time < lastParryTime + parryCooldown)
        {
            Debug.Log("쿨타임 중");
            return;
        }

        lastParryTime = Time.time;

        Vector3 forward = transform.forward;
        Vector3 center = transform.position + forward * forwardOffset;

        int hitCount = Physics.OverlapBoxNonAlloc(center, boxHalfExtents, hitBuffer, transform.rotation, parryableLayer);

        bool hitSomething = false;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = hitBuffer[i];
            if (col == null) continue;

            IParryable parryable = col.GetComponentInParent<IParryable>();
            if (parryable == null) continue;

            parryable.OnParried(forward, parryForce, upwardForce, torqueForce);
            hitSomething = true;
        }

        SpawnParryVFX(forward, hitSomething ? Color.yellow : Color.white);
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