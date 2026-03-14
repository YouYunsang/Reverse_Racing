using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private string obstacleTag = "Obstacle";

    //[SerializeField] private float knokbackForce = 50f;
    [SerializeField] private float knockbackSpeed = 6f;

    [SerializeField, Range(0f, 1f)] private float forwardRateOnHit = 0f;
    [SerializeField, Range(0f, 1f)] private float laneRateInHit = 0f;

    [SerializeField] private float hitCooldown = 0.5f;

    private PlayerMovement playerMovement;
    private float lastHitTime = -999f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        TryHandleCollision(collision.transform);
    }


    private void TryHandleCollision(Transform obstacle)
    {
        if (!enabled)
        {
            Debug.Log("1");
            return;
        }

        if (Time.time < lastHitTime + hitCooldown)
        {
            Debug.Log("2");
            return;
        }

        if (!obstacle.CompareTag(obstacleTag))
        {
            Debug.Log("3");
            return;
        }

        lastHitTime = Time.time;

        Vector3 hitDirection = obstacle.position - transform.position;
        hitDirection.y = 0f;

        if (hitDirection.sqrMagnitude < 0.0001f)
        {
            hitDirection = transform.forward;
            Debug.Log("hitDirection.sqrMagnitude > 0.0001f");
        }

        Vector3 knokbackDirection = -hitDirection.normalized;
        Vector3 knokbackVelocity = knokbackDirection * knockbackSpeed;

        playerMovement.AddKnockback(knokbackVelocity);
        playerMovement.ApplySpeedPenalty(forwardRateOnHit, laneRateInHit);
    }
}
