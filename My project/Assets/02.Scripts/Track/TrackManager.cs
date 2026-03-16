using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [Header("Chunk Prefabs")]
    [SerializeField] private TrackChunk startChunkPrefab;
    [SerializeField] private TrackChunk normalChunkPrefab;

    [Header("Settings")]
    [SerializeField] private int maintainChunkCount = 4;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float removeDistanceBehindPlayer = 30f;

    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private FuelTankSpawner fuelTankSpawner;

    private readonly Queue<TrackChunk> activeChunks = new Queue<TrackChunk>();
    private float nextSpawnZ = 0f;
    private bool initialized = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (initialized) return;
        initialized = true;

        SpawnStartChunk();

        for (int i = 0; i < maintainChunkCount; i++)
        {
            SpawnNormalChunk();
        }
    }

    private void Update()
    {
        if (!initialized || playerTransform == null) return;

        RecycleOldChunks();
        MaintainChunks();
    }

    private void SpawnStartChunk()
    {
        TrackChunk chunk = Instantiate(startChunkPrefab, new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);
        activeChunks.Enqueue(chunk);
        nextSpawnZ += chunk.ChunkLength;
    }

    private void SpawnNormalChunk()
    {
        TrackChunk chunk = Instantiate(normalChunkPrefab, new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);
        activeChunks.Enqueue(chunk);
        nextSpawnZ += chunk.ChunkLength;

        obstacleSpawner.SpawnObstacles(chunk);
        fuelTankSpawner.TrySpawnFuelTank(chunk);
    }

    private void MaintainChunks()
    {
        while (activeChunks.Count < maintainChunkCount + 1)
        {
            SpawnNormalChunk();
        }

        TrackChunk lastChunk = null;
        //큐를 한 바퀴 돌면서 가장 마지막에 들어온 청크를 찾음
        foreach (TrackChunk chunk in activeChunks)
        {
            lastChunk = chunk;
        }

        if (lastChunk == null) return;

        float distanceToChunkEnd = (lastChunk.transform.position.z + lastChunk.ChunkLength) - playerTransform.position.z;

        if (distanceToChunkEnd < lastChunk.ChunkLength * 3f)
        {
            SpawnNormalChunk();
        }
    }

    private void RecycleOldChunks()
    {
        while (activeChunks.Count > 0)
        {
            TrackChunk oldestChunk = activeChunks.Peek();
            float chunkEndZ = oldestChunk.transform.position.z + oldestChunk.ChunkLength * 0.5f;

            if (chunkEndZ < playerTransform.position.z - removeDistanceBehindPlayer)
            {
                oldestChunk.ResetChunk();
                activeChunks.Dequeue();
                Destroy(oldestChunk.gameObject);
            }
            else
            {
                break;
            }
        }
    }
}