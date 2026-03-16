using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedEffectController : MonoBehaviour
{
    private enum SpeedEffectState
    {
        None,
        Boost,
        Overdrive
    }

    [Header("References")]
    [SerializeField] private PlayerBooster playerBooster;
    [SerializeField] private PlayerOverdrive playerOverdrive;

    [Header("Particles")]
    [SerializeField] private ParticleSystem boostSpeedParticle;
    [SerializeField] private ParticleSystem overdriveSpeedParticle;

    [Header("Particle Intensity")]
    [SerializeField] private float boostEmissionRate = 35f;
    [SerializeField] private float overdriveEmissionRate = 120f;
    [SerializeField] private float emissionLerpSpeed = 8f;

    [Header("URP Volume Blur")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float normalMotionBlur = 0f;
    [SerializeField] private float overdriveMotionBlur = 0.65f;
    [SerializeField] private float blurLerpSpeed = 6f;

    private SpeedEffectState currentState = SpeedEffectState.None;
    private MotionBlur motionBlur;

    private void Awake()
    {
        if (playerBooster == null)
            playerBooster = GetComponent<PlayerBooster>();

        if (playerOverdrive == null)
            playerOverdrive = GetComponent<PlayerOverdrive>();

        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out motionBlur);
        }
    }

    private void Start()
    {
        SetParticleImmediate(boostSpeedParticle, 0f, false);
        SetParticleImmediate(overdriveSpeedParticle, 0f, false);

        if (motionBlur != null)
        {
            motionBlur.intensity.Override(normalMotionBlur);
        }
    }

    private void Update()
    {
        UpdateState();
        UpdateParticles();
        UpdateBlur();
    }

    private void UpdateState()
    {
        if (playerOverdrive != null && playerOverdrive.IsOverdriveActive)
        {
            currentState = SpeedEffectState.Overdrive;
        }
        else if (playerBooster != null && playerBooster.IsBoosting)
        {
            currentState = SpeedEffectState.Boost;
        }
        else
        {
            currentState = SpeedEffectState.None;
        }
    }

    private void UpdateParticles()
    {
        switch (currentState)
        {
            case SpeedEffectState.None:
                UpdateParticleEmission(boostSpeedParticle, 0f, false);
                UpdateParticleEmission(overdriveSpeedParticle, 0f, false);
                break;

            case SpeedEffectState.Boost:
                UpdateParticleEmission(boostSpeedParticle, boostEmissionRate, true);
                UpdateParticleEmission(overdriveSpeedParticle, 0f, false);
                break;

            case SpeedEffectState.Overdrive:
                UpdateParticleEmission(boostSpeedParticle, 0f, false);
                UpdateParticleEmission(overdriveSpeedParticle, overdriveEmissionRate, true);
                break;
        }
    }

    private void UpdateBlur()
    {
        if (motionBlur == null)
            return;

        float targetBlur = currentState == SpeedEffectState.Overdrive
            ? overdriveMotionBlur
            : normalMotionBlur;

        float current = motionBlur.intensity.value;
        float next = Mathf.Lerp(current, targetBlur, blurLerpSpeed * Time.deltaTime);
        motionBlur.intensity.Override(next);
    }

    private void UpdateParticleEmission(ParticleSystem ps, float targetRate, bool shouldPlay)
    {
        if (ps == null)
            return;

        var emission = ps.emission;
        float currentRate = emission.rateOverTime.constant;
        float nextRate = Mathf.Lerp(currentRate, targetRate, emissionLerpSpeed * Time.deltaTime);
        emission.rateOverTime = nextRate;

        if (shouldPlay)
        {
            if (!ps.isPlaying)
                ps.Play();
        }
        else
        {
            if (targetRate <= 0.01f && ps.isPlaying && nextRate <= 0.1f)
                ps.Stop();
        }
    }

    private void SetParticleImmediate(ParticleSystem ps, float rate, bool play)
    {
        if (ps == null)
            return;

        var emission = ps.emission;
        emission.rateOverTime = rate;

        if (play) ps.Play();
        else ps.Stop();
    }
}