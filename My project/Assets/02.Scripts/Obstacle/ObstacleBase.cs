using UnityEngine;

public class ObstacleBase : MonoBehaviour, IParryable, IParryGaugeReward, IParryEffectProvider
{
    [Header("Grid Size")]
    [SerializeField] protected int width = 1;
    [SerializeField] protected int height = 2;
    [SerializeField] private int gaugeRewardAmount = 1;

    public int Width => width;
    public int Height => height;

    [Header("Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] colliders;

    [Header("State")]
    [SerializeField] private bool isParried = false;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (colliders == null || colliders.Length == 0)
            colliders = GetComponentsInChildren<Collider>();
    }

    public int GetGaugeRewardAmount()
    {
        return gaugeRewardAmount;
    }

    public ParryEffectType GetParryEffectType()
    {
        return ParryEffectType.SmallImpact;
    }

    public void OnParried(Vector3 parryDirection, float parryForce, float upwardForce, float torqueForce)
    {
        if (isParried)
            return;

        isParried = true;

        if (ParryEffectManager.Instance != null)
        {
            ParryEffectManager.Instance.PlayEffect(
                GetParryEffectType(),
                transform.position + Vector3.forward,
                Quaternion.LookRotation(parryDirection)
            );
        }

        // 플레이어와 바로 다시 충돌하지 않게 막기
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;

            Vector3 launchDir = (parryDirection + Vector3.up * 0.35f).normalized;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(launchDir * parryForce, ForceMode.VelocityChange);
            rb.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);
            rb.AddTorque(Vector3.right * torqueForce + Vector3.up * (torqueForce * 0.5f), ForceMode.VelocityChange);
        }
    }

    public virtual void ReleaseSelf()
    {
        gameObject.SetActive(false);
    }
}