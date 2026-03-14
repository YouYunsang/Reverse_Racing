using UnityEngine;

public class PlayerBooster : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Camera targetCamera;

    [SerializeField] private float boostForwardMulti = 1.35f;
    [SerializeField] private float boostLaneMulti = 1.25f;

    [SerializeField] private float boostDuration = 1.2f;
    [SerializeField] private float boostAddAmount = 0.5f;
    //[SerializeField] private float maxBoostTime = 2.5f;

    [SerializeField] private float normalFov = 60f;
    [SerializeField] private float boostedFov = 72f;
    [SerializeField] private float fovLerpSpeed = 8f;

    private float boostTimer = 0f;
    private bool isBoosting = false;

    public bool IsBoosting => isBoosting;

    private void Awake()
    {
        if(playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if(targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        UpdateBoostTimer();
        UpdateCameraFOV();
        ApplyBoostToMovement();
    }

    public void ActivateBoost()
    {
        if (!isBoosting)
        {
            isBoosting = true;
            boostTimer = boostDuration;
        }
        else
        {
            boostTimer += boostAddAmount;
            //부스트를 중복 사용해도 맥스 부스트 타임으로 시간 제한
            //boostTimer = Mathf.Min(boostTimer, maxBoostTime);
        }
    }

    private void UpdateBoostTimer()
    {
        if (!isBoosting) return;

        boostTimer -= Time.deltaTime;

        if(boostTimer <= 0)
        {
            boostTimer = 0f;
            isBoosting = false;
        }
    }

    private void ApplyBoostToMovement()
    {
        if (isBoosting)
        {
            playerMovement.SetBoostMultipliers(boostForwardMulti, boostLaneMulti);
        }
        else
        {
            playerMovement.SetBoostMultipliers(1f, 1f);
        }
    }

    private void UpdateCameraFOV()
    {
        float targetFov = isBoosting ? boostedFov : normalFov;
        targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
    }
}
