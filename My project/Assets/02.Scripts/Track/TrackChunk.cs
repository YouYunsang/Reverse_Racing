using System.Collections.Generic;
using UnityEngine;

public class TrackChunk : MonoBehaviour
{
    [Header("Chunk Info")]
    [SerializeField] private float chunkLength = 20f;
    public const int LaneCount = 5;
    public const int RowCount = 20;

    //5*20 2차원 bool 배열
    private bool[,] occupancyGrid = new bool[LaneCount, RowCount];

    public float cellLength = 1f;
    public float laneWidth = 1f;

    private readonly List<ObstacleBase> spawnedObstacles = new List<ObstacleBase>();

    public float ChunkLength => chunkLength;

    public void ResetGrid()
    {
        for (int x = 0; x < LaneCount; x++)
        {
            for (int z = 0; z < RowCount; z++)
            {
                occupancyGrid[x, z] = false;
            }
        }
    }

    //시작 지점으로 부터 세로로 정해진 length 만큼 검사하며 적의 존재 여부를 판단한다.
    //적이 존재하지 않으면 true 반환
    public bool IsAreaFree(int lane, int startRow, int length)
    {
        if(startRow + length > RowCount)
        {
            return false;
        }

        for(int i = 0; i < length; i++)
        {
            if (occupancyGrid[lane, startRow + i]) return false;
        }

        return true;
    }


    //이 곳엔 장애물을 배치했다고 표시해준다.
    public void OccupyArea(int lane, int startRow, int length)
    {
        for (int i = 0; i < length; i++)
        {
            occupancyGrid[lane, startRow + i] = true;
        }
    }

    public Vector3 GetWorldPosition(int lane, int row, float yPos)
    {
        float x = (lane - 2) * laneWidth;
        float z = row * cellLength;

        if(row > 9)
        {
            z = -z;
        }

        return transform.position + new Vector3(x, yPos, z);
    }

    public void RegisterObstacle(ObstacleBase obstacle)
    {
        if (obstacle == null)
            return;

        if (!spawnedObstacles.Contains(obstacle))
        {
            spawnedObstacles.Add(obstacle);
        }
    }

    public void ClearObstacles()
    {
        for (int i = 0; i < spawnedObstacles.Count; i++)
        {
            if (spawnedObstacles[i] != null)
            {
                spawnedObstacles[i].ReleaseSelf();
            }
        }

        spawnedObstacles.Clear();
    }

    public void ResetChunk()
    {
        ClearObstacles();
    }
}