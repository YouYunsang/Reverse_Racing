using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int laneCount = 5;
    [SerializeField] private int currentLane = 2;
    [SerializeField] private float laneSpacing = 1f;

    [SerializeField] private float laneChangeSpeed = 6f;
    [SerializeField] private float forwardMoveSpeed = 15f;

    [SerializeField] private float speedRecoverRate = 3f;
    [SerializeField] private float laneRecoverRate = 6f;

    private float boostForwardMultiplier = 1f;
    private float boostLaneMultiplier = 1f;

    private float defaultForwardMoveSpeed;
    private float defaultLaneChangeSpeed;

    [SerializeField] private float knockbackDamping = 12f;
    [SerializeField] private float laneReattachThreshold = 0.01f;
    [SerializeField] private float finalForwardSpeed;
    [SerializeField] private float finalLaneSpeed;

    private Rigidbody rb;
    private float targetX;

    private Vector3 knockbackVelocity;
    private bool wasInKnockback;

    public int LaneCount => laneCount;

    public int CurrentLane => currentLane;
    public float ForwardMoveSpeed => forwardMoveSpeed;
    public float LaneChangeSpeed => laneChangeSpeed;
    public Rigidbody RB => rb;
    public float DefaultForwardMoveSpeed => defaultForwardMoveSpeed;
    public float DefaultLaneChangeSpeed => defaultLaneChangeSpeed;

    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        defaultForwardMoveSpeed = forwardMoveSpeed;
        defaultLaneChangeSpeed = laneChangeSpeed;

        currentLane = Mathf.Clamp(currentLane, 0, laneCount - 1);
        targetX = GetLaneWorldX(currentLane);

        Vector3 pos = transform.position;
        pos.x = targetX;
        transform.position = pos;
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            return;

        HandleLaneInput();
    }

    void FixedUpdate()
    {
        RecoverMovementStats();
        MoveForward();
        HandleLaneMovement();
        UpdateKnockback();
    }

    private void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(1);
        }
    }

    public void MoveForward()
    {
        Vector3 velocity = rb.linearVelocity;
        finalForwardSpeed = forwardMoveSpeed * boostForwardMultiplier;

        velocity.z = forwardMoveSpeed + knockbackVelocity.z;
        velocity.x = knockbackVelocity.x;
        velocity.y = 0f;

        rb.linearVelocity = velocity;
    }

    public void MoveLane(int direction)
    {
        int nextLane = Mathf.Clamp(currentLane + direction, 0, laneCount - 1);

        if (nextLane == currentLane)
            return;

        currentLane = nextLane;
        targetX = GetLaneWorldX(currentLane);
    }

    private void HandleLaneMovement()
    {
        Vector3 position = rb.position;
        finalLaneSpeed = laneChangeSpeed * boostLaneMultiplier;

        float newX = Mathf.Lerp(position.x, targetX, finalLaneSpeed * Time.fixedDeltaTime);
        position.x = newX;
        rb.MovePosition(position);
    }

    private void RecoverMovementStats()
    {
        forwardMoveSpeed = Mathf.MoveTowards(forwardMoveSpeed, defaultForwardMoveSpeed, speedRecoverRate * Time.fixedDeltaTime);

        laneChangeSpeed = Mathf.MoveTowards(laneChangeSpeed, defaultLaneChangeSpeed, laneRecoverRate * Time.fixedDeltaTime);
    }

    private void UpdateKnockback()
    {
        if (knockbackVelocity.sqrMagnitude > 0.0001f)
        {
            wasInKnockback = true;

            knockbackVelocity = Vector3.MoveTowards(knockbackVelocity, Vector3.zero, knockbackDamping * Time.fixedDeltaTime);
        }
        else
        {
            knockbackVelocity = Vector3.zero;

            if (wasInKnockback)
            {
                wasInKnockback = false;
                SnapToNearestLaneTarget();
            }
        }
    }

    public void ApplySpeedPenalty(float forwardMultiflier, float laneMultiplier)
    {
        forwardMoveSpeed *= forwardMultiflier;
        laneChangeSpeed *= laneMultiplier;

        //속도가 0 밑으로는 내려가지 않도록 clamp 사용
        forwardMoveSpeed = Mathf.Clamp(forwardMoveSpeed, 0f, defaultForwardMoveSpeed);
        laneChangeSpeed = Mathf.Clamp(laneChangeSpeed, 0f, defaultLaneChangeSpeed);

        Debug.LogFormat("forward : {0}, lane : {1}", forwardMoveSpeed, laneChangeSpeed);
    }

    public void AddKnockback(Vector3 knokback)
    {
        knockbackVelocity += knokback;
        Debug.Log("Knokback!");
    }

    private void SnapToNearestLaneTarget()
    {
        float nearestDistance = float.MaxValue;
        int nearestLane = currentLane;

        for (int i = 0; i < laneCount; i++)
        {
            float laneX = GetLaneWorldX(i);
            float distance = Mathf.Abs(transform.position.x - laneX);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestLane = i;
            }
        }

        currentLane = nearestLane;
        targetX = GetLaneWorldX(currentLane);

        if (Mathf.Abs(transform.position.x - targetX) < laneReattachThreshold) {
            Vector3 pos = transform.position;
            pos.x = targetX;
            transform.position = pos;
        }
    }

    public float GetLaneWorldX(int laneIndex)
    {
        float centerOffset = (laneCount - 1) * 0.5f;
        return (laneIndex - centerOffset) * laneSpacing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        float centerOffset = (laneCount - 1) * 0.5f;
        for (int i = 0; i < laneCount; i++)
        {
            float x = (i - centerOffset) * laneSpacing;
            Vector3 lanePos = new Vector3(x, transform.position.y, transform.position.z);
            Gizmos.DrawWireSphere(lanePos, 0.2f);
        }
    }

    public void SetBoostMultipliers(float forwardMultiplier, float laneMultiplier)
    {
        boostForwardMultiplier = Mathf.Max(0f, forwardMultiplier);
        boostLaneMultiplier = Mathf.Max(0f, laneMultiplier);
    }

    public void SetForwardMoveSpeed(float newSpeed)
    {
        forwardMoveSpeed = Mathf.Max(0f, newSpeed);
    }

    public void SetLaneChangeSpeed(float newSpeed)
    {
        laneChangeSpeed = Mathf.Max(0f, newSpeed);
    }

    public void StopAllMovementImmediate()
    {
        forwardMoveSpeed = 0f;
        laneChangeSpeed = 0f;
        knockbackVelocity = Vector3.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}