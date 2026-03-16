using UnityEngine;

public class PlayerBooster : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Camera targetCamera;

    [SerializeField] private float boostForwardMulti = 1.35f;
    [SerializeField] private float boostLaneMulti = 1.25f;

    [SerializeField] private float boostDuration = 1.2f;
    [SerializeField] private float boostAddAmount = 0.5f;

    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float boostedFOV = 72f;
    [SerializeField] private float fovLerpSpeed = 8f;

    private bool isOverdriveActive = false;
    private float overdriveForwardMultiplier = 1f;
    private float overdriveLaneMultiplier = 1f;
    private float overdriveFOV = 60f;

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

    public void SetOverdriveState(bool active, float forwardMultiplier, float laneMultiplier, float targetFov)
    {
        isOverdriveActive = active;

        if (active)
        {
            overdriveForwardMultiplier = forwardMultiplier;
            overdriveLaneMultiplier = laneMultiplier;
            overdriveFOV = targetFov;
        }
        else
        {
            overdriveForwardMultiplier = 1f;
            overdriveLaneMultiplier = 1f;
            overdriveFOV = normalFOV;
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
        float forwardMult = 1f;
        float laneMult = 1f;

        if (isBoosting)
        {
            forwardMult *= boostForwardMulti;
            laneMult *= boostLaneMulti;
        }
        else if (isOverdriveActive)
        {
            forwardMult *= overdriveForwardMultiplier;
            laneMult *= overdriveLaneMultiplier;
        }
        else
        {
            playerMovement.SetBoostMultipliers(1f, 1f);
        }
        
        playerMovement.SetBoostMultipliers(forwardMult, laneMult);
    }

    private void UpdateCameraFOV()
    {
        float targetFov = normalFOV;

        if (isBoosting)
        {
            targetFov = boostedFOV;
        }

        if (isOverdriveActive)
        {
            targetFov = overdriveFOV;
        }

        targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
    }
}
