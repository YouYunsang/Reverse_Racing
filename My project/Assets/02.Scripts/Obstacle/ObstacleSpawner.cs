using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 0.2f;
    public GameObject obstaclePrefab;

    public void SpawnObstacles(TrackChunk chunk)
    {
        chunk.ResetGrid();

        for (int row = 0; row < TrackChunk.RowCount; row++)
        {
            if (Random.value > spawnRate)
                continue;

            int lane = Random.Range(0, TrackChunk.LaneCount);

            //int length = Random.value < 0.5f ? 2 : 3;
            int length = 2;

            if (!chunk.IsAreaFree(lane, row, length))
                continue;

            chunk.OccupyArea(lane, row, length);

            Vector3 pos = chunk.GetWorldPosition(lane, row);

            Instantiate(obstaclePrefab, pos, Quaternion.identity, chunk.transform);
        }
    }
}