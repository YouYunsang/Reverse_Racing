using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 0.2f;
    public GameObject obstaclePrefab;

    public void SpawnObstacles(TrackChunk chunk)
    {
        if (SpawnGate.Instance != null && !SpawnGate.Instance.IsSpawnAllowed)
            return;

        chunk.ResetGrid();

        for (int row = 0; row < TrackChunk.RowCount; row++)
        {
            if (Random.value > spawnRate)
                continue;

            int lane = Random.Range(0, TrackChunk.LaneCount);

            //int length = Random.value < 0.5f ? 2 : 3;
            int length = 2;
            float yPos = 0.15f;

            if (!chunk.IsAreaFree(lane, row, length))
                continue;

            chunk.OccupyArea(lane, row, length);

            Vector3 pos = chunk.GetWorldPosition(lane, row, yPos);

            Instantiate(obstaclePrefab, pos, Quaternion.identity, chunk.transform);
        }
    }
}