using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    public static CameraShakeController Instance { get; private set; }

    [Header("Default Shake")]
    [SerializeField] private float defaultDuration = 0.12f;
    [SerializeField] private float defaultMagnitude = 0.08f;

    private Vector3 originalLocalPosition;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        originalLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        originalLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (shakeTimer > 0f)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            randomOffset.z = 0f;

            transform.localPosition = originalLocalPosition + randomOffset;

            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                shakeTimer = 0f;
                transform.localPosition = originalLocalPosition;
            }
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }

    public void PlayDefaultShake()
    {
        PlayShake(defaultDuration, defaultMagnitude);
    }

    public void PlayShake(float duration, float magnitude)
    {
        if (duration <= 0f || magnitude <= 0f)
            return;

        // 더 강한 쉐이크가 들어오면 갱신
        shakeTimer = Mathf.Max(shakeTimer, duration);
        shakeMagnitude = Mathf.Max(shakeMagnitude, magnitude);
    }
}