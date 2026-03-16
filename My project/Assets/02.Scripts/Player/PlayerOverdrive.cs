using UnityEngine;

public class PlayerOverdrive : MonoBehaviour
{
    [SerializeField] private readonly KeyCode skillKey = KeyCode.F;

    [SerializeField] private PlayerParryGauge playerParryGauge;
    [SerializeField] private PlayerBooster playerBooster;

    [SerializeField] private int requiredGauge = 10;
    [SerializeField] private float gaugeConsumeInterval = 0.15f;

    [SerializeField] private float overdriveForwardMultiplier = 2.2f;
    [SerializeField] private float overdriveLaneMultiplier = 1.8f;
    [SerializeField] private float overdriveFOV = 85f;

    [SerializeField] private float collisionParryForce = 14f;
    [SerializeField] private float collisionUpwardForce = 5f;
    [SerializeField] private float collisionTorqueForce = 14f;

    private bool isOverdriveActive = false;
    private float gaugeConsumeTimer = 0f;

    public bool IsOverdriveActive => isOverdriveActive;

    private void Awake()
    {
        if (playerParryGauge == null)
            playerParryGauge = GetComponent<PlayerParryGauge>();

        if (playerBooster == null)
            playerBooster = GetComponent<PlayerBooster>();
    }

    private void Update()
    {
        HandleSkillInput();

        if (isOverdriveActive)
        {
            UpdateOverdrive();
        }
    }

    private void HandleSkillInput()
    {
        if (!Input.GetKeyDown(skillKey))
            return;

        if (isOverdriveActive)
            return;

        if (playerParryGauge == null)
            return;

        if (playerParryGauge.CurrentGauge < requiredGauge)
            return;

        StartOverdrive();
    }

    private void StartOverdrive()
    {
        isOverdriveActive = true;
        Debug.LogFormat("{0} overdrive", isOverdriveActive);
        gaugeConsumeTimer = gaugeConsumeInterval;

        if (playerBooster != null)
        {
            playerBooster.SetOverdriveState(
                true,
                overdriveForwardMultiplier,
                overdriveLaneMultiplier,
                overdriveFOV
            );
        }
    }

    private void UpdateOverdrive()
    {
        gaugeConsumeTimer -= Time.deltaTime;

        if(gaugeConsumeTimer <= 0)
        {
            gaugeConsumeTimer += gaugeConsumeInterval;

            bool consumed = playerParryGauge.ConsumeGauge(1);
            if(!consumed || playerParryGauge.CurrentGauge <= 0)
            {
                StopOverdrive();
            }
        }
    }

    public void StopOverdrive()
    {
        if (!isOverdriveActive) return;

        isOverdriveActive = false;

        if(playerBooster != null)
        {
            playerBooster.SetOverdriveState(false, 1f, 1f, 0f);
        }
    }

    public void TryParryOnCollision(Collider other)
    {
        Debug.Log("overdrive parry");
        if (!isOverdriveActive)
            return;

        if (other == null)
            return;

        IParryable parryable = other.GetComponentInParent<IParryable>();
        if (parryable == null)
        {
            return;
        }

        parryable.OnParried(
            transform.forward,
            collisionParryForce,
            collisionUpwardForce,
            collisionTorqueForce
        );
    }
}
