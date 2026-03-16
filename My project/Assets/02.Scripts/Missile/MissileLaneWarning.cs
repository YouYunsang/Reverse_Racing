using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class MissileLaneWarning : MonoBehaviour
{
    [SerializeField] private GameObject warningVisual;

    [Header("Position Follow")]
    [SerializeField] private float warning2player = 4f;

    [Header("Warning FX")]
    [SerializeField] private float blinkDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.12f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.08f;
    [SerializeField] private float shakeFrequency = 40f;

    private float playerZPos;
    private float warningZPos;

    private Vector3 baseLocalPosition;
    private CancellationToken destroyToken;

    private void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
        baseLocalPosition = transform.localPosition;
        SetVisible(false);
    }

    private void Start()
    {
        if (PlayerMovement.Instance != null)
        {
            playerZPos = PlayerMovement.Instance.transform.position.z;
        }
    }

    private void Update()
    {
        if (this == null || !gameObject || !isActiveAndEnabled)
            return;

        if (PlayerMovement.Instance == null)
            return;

        playerZPos = PlayerMovement.Instance.transform.position.z;
        warningZPos = playerZPos + warning2player;
        transform.position = new Vector3(transform.position.x, 0.3f, warningZPos);
    }

    public async UniTask PlayWarningSequence(CancellationToken externalToken = default)
    {
        if (this == null || !gameObject || warningVisual == null)
            return;

        using CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(destroyToken, externalToken);

        CancellationToken token = linkedCts.Token;

        try
        {
            if (this == null || !gameObject)
                return;

            baseLocalPosition = transform.localPosition;

            float elapsed = 0f;
            bool visible = true;

            while (elapsed < blinkDuration)
            {
                if (this == null || !gameObject || warningVisual == null)
                    return;

                warningVisual.SetActive(visible);
                visible = !visible;

                await UniTask.Delay(
                    TimeSpan.FromSeconds(blinkInterval),
                    cancellationToken: token
                );

                if (this == null || !gameObject)
                    return;

                elapsed += blinkInterval;
            }

            if (this == null || !gameObject || warningVisual == null)
                return;

            warningVisual.SetActive(true);

            float shakeElapsed = 0f;

            while (shakeElapsed < shakeDuration)
            {
                if (this == null || !gameObject)
                    return;

                float offsetX = Mathf.Sin(Time.time * shakeFrequency) * shakeMagnitude;
                transform.localPosition = baseLocalPosition + new Vector3(offsetX, 0f, 0f);

                await UniTask.Yield(PlayerLoopTiming.Update, token);

                if (this == null || !gameObject)
                    return;

                shakeElapsed += Time.deltaTime;
            }

            if (this == null || !gameObject)
                return;

            transform.localPosition = baseLocalPosition;

            if (warningVisual != null)
            {
                warningVisual.SetActive(false);
            }
        }
        catch (OperationCanceledException)
        {
            if (this == null || !gameObject)
                return;

            transform.localPosition = baseLocalPosition;

            if (warningVisual != null)
            {
                warningVisual.SetActive(false);
            }
        }
    }

    public void SetVisible(bool visible)
    {
        if (this == null || !gameObject)
            return;

        if (warningVisual != null)
        {
            warningVisual.SetActive(visible);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}