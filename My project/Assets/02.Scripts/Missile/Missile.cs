using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Missile : MonoBehaviour, IParryable, IParryGaugeReward, IParryEffectProvider
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float lifeTime = 2f;

    [Header("Tags")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string obstacleTag = "Obstacle";

    [Header("Obstacle Hit")]
    [SerializeField] private float obstacleCollideForce = 10f;
    [SerializeField] private float obstacleUpwardForce = 4f;
    [SerializeField] private float obstacleTorqueForce = 12f;

    [SerializeField] private float fuelDamageOnHit = 15f;

    [Header("Parried Missile")]
    [SerializeField] private float parriedMoveSpeed = 24f;
    [SerializeField] private float parriedLifeTime = 1.5f;

    [SerializeField] private int gaugeRewardAmount = 3;

    private bool isActiveMissile = false;
    private bool isParried = false;

    private Vector3 moveDirection = Vector3.forward;
    private CancellationToken destroyToken;
    private CancellationTokenSource lifeCts;

    private void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
    }

    public void Initialize(float speed, float newLifeTime)
    {
        moveSpeed = speed;
        lifeTime = newLifeTime;

        isActiveMissile = true;
        //isParried = false;

        moveDirection = transform.forward;

        RestartLifeTimer(lifeTime);
    }

    private void Update()
    {
        if (!isActiveMissile)
            return;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public ParryEffectType GetParryEffectType()
    {
        return ParryEffectType.MissileImpact;
    }

    public void OnParried(Vector3 parryDirection, float parryForce, float upwardForce, float torqueForce)
    {
        if (!isActiveMissile)
            return;

        if (isParried)
            return;

        if (CameraShakeController.Instance != null)
        {
            CameraShakeController.Instance.PlayShake(0.12f, 0.08f);
        }

        if (ParryEffectManager.Instance != null)
        {
            ParryEffectManager.Instance.PlayEffect(
                GetParryEffectType(),
                transform.position + Vector3.forward,
                Quaternion.LookRotation(parryDirection)
            );
        }

        // 패링 방향으로 미사일 진행 방향 전환
        moveDirection = parryDirection.normalized;

        // 패링 후에는 조금 더 빠르게 날아가게
        moveSpeed = Mathf.Max(moveSpeed, parriedMoveSpeed);

        // 패링된 미사일은 너무 오래 남지 않게 별도 수명 사용
        RestartLifeTimer(parriedLifeTime);
    }

    public int GetGaugeRewardAmount()
    {
        return gaugeRewardAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveMissile)
            return;

        if (other.CompareTag(playerTag))
        {
            PlayerFuel playerFuel = other.GetComponentInParent<PlayerFuel>();
            if (playerFuel != null)
            {
                playerFuel.ConsumeFuel(fuelDamageOnHit);
            }

            DestroyMissile();
            return;
        }

        IParryable parryable = other.GetComponentInParent<IParryable>();

        if (parryable != null && other.CompareTag(obstacleTag)) //parryable != (IParryable)this
        {
            parryable.OnParried(
                moveDirection,
                obstacleCollideForce,
                obstacleUpwardForce,
                obstacleTorqueForce
            );
        }

        return;
    }

    private void RestartLifeTimer(float delaySeconds)
    {
        lifeCts?.Cancel();
        lifeCts?.Dispose();

        lifeCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken);
        RunLifeTimer(delaySeconds, lifeCts.Token).Forget();
    }

    private async UniTaskVoid RunLifeTimer(float delaySeconds, CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: token);

            if (this != null && gameObject != null)
            {
                DestroyMissile();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void DestroyMissile()
    {
        if (!isActiveMissile)
            return;

        isActiveMissile = false;

        lifeCts?.Cancel();
        lifeCts?.Dispose();
        lifeCts = null;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        lifeCts?.Cancel();
        lifeCts?.Dispose();
        lifeCts = null;
    }
}