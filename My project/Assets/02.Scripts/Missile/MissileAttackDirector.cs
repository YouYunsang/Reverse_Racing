using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MissileAttackDirector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private MissileLaneWarning[] laneWarnings;

    [SerializeField] private float minSpawnInterval = 4f;
    [SerializeField] private float maxSpawnInterval = 10f;
    [SerializeField] private float warningDuration = 2f;

    [SerializeField, Range(0.1f, 1f)] private float minLaneFireChance = 0.1f;
    [SerializeField, Range(0.1f, 1f)] private float maxLaneFireChance = 0.5f;

    [SerializeField] private float missileExtraSpeed = 8f;
    [SerializeField] private float missileLifeTime = 5f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnForwardDistance = 40f;
    [SerializeField] private float groundY = 0f;

    private CancellationToken destroyToken;

    private void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        RunMissileLoop(destroyToken).Forget();
    }

    private async UniTaskVoid RunMissileLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                float interval = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
                await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);

                if (GameManager.Instance != null && !GameManager.Instance.IsPlaying()) continue;

                List<int> selectedLanes = PickLanes();

                ShowWarnings(selectedLanes, true);
                await UniTask.Delay(TimeSpan.FromSeconds(warningDuration), cancellationToken: token);
                ShowWarnings(selectedLanes, false);

                FireMissiles(selectedLanes);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private List<int> PickLanes()
    {
        List<int> selected = new List<int>();

        while (selected.Count == 0)
        {
            selected.Clear();

            for (int lane = 0; lane < PlayerMovement.Instance.LaneCount; lane++)
            {
                float chance = UnityEngine.Random.Range(minLaneFireChance, maxLaneFireChance);

                if (UnityEngine.Random.value <= chance)
                {
                    selected.Add(lane);
                }
            }
        }

        return selected;
    }

    private void ShowWarnings(List<int> lanes, bool visible)
    {
        if (laneWarnings == null) return;

        for (int i = 0; i < laneWarnings.Length; i++)
        {
            if (laneWarnings[i] != null) laneWarnings[i].SetVisible(false);
        }

        if (!visible) return;

        for (int i = 0; i < lanes.Count; i++)
        {
            int lane = lanes[i];
            if (lane >= 0 && lane < laneWarnings.Length && laneWarnings[lane] != null)
            {
                laneWarnings[lane].SetVisible(true);
            }
        }
    }

    private void FireMissiles(List<int> lanes)
    {
        float spawnZ = GetMissileSpawnZ();

        for (int i = 0; i < lanes.Count; i++)
        {
            int lane = lanes[i];

            float laneX = PlayerMovement.Instance.GetLaneWorldX(lane);
            Vector3 spawnPos = new Vector3(laneX, groundY, spawnZ);

            Missile missile = Instantiate(missilePrefab, spawnPos, Quaternion.identity);

            // 플레이어를 향해 날아오도록 뒤쪽 방향 설정
            missile.transform.forward = Vector3.back;

            float missileSpeed = Mathf.Max(
                PlayerMovement.Instance.ForwardMoveSpeed + missileExtraSpeed,
                PlayerMovement.Instance.ForwardMoveSpeed + 1f
            );

            missile.Initialize(missileSpeed, missileLifeTime);
        }
    }

    private float GetMissileSpawnZ()
    {
        return PlayerMovement.Instance.transform.position.z + spawnForwardDistance;
    }
}