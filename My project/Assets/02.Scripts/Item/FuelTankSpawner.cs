using System.Collections.Generic;
using UnityEngine;

public class FuelTankSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnChancePerChunk = 0.4f;

    [Header("Prefab")]
    [SerializeField] private FuelTankItem fuelTankPrefab;

    private FuelTankItem currentFuelTank;

    public bool HasActiveFuelTank => currentFuelTank != null;

    public void TrySpawnFuelTank(TrackChunk chunk)
    {
        if (SpawnGate.Instance != null && !SpawnGate.Instance.IsSpawnAllowed)
            return;

        if (chunk == null)
            return;

        if (fuelTankPrefab == null)
            return;

        if (HasActiveFuelTank)
            return;

        if (Random.value > spawnChancePerChunk)
            return;

        List<Vector2Int> emptyCells = CollectEmptyCells(chunk);
        if (emptyCells.Count == 0)
            return;

        int randomIndex = Random.Range(0, emptyCells.Count);
        Vector2Int cell = emptyCells[randomIndex];

        int lane = cell.x;
        int row = cell.y;
        float yPos = 0.4f;

        if (!chunk.IsAreaFree(lane, row, 1))
            return;

        chunk.OccupyArea(lane, row, 1);

        Vector3 spawnPos = chunk.GetWorldPosition(lane, row, yPos);
        FuelTankItem spawnedTank = Instantiate(fuelTankPrefab, spawnPos, Quaternion.identity, chunk.transform);
        spawnedTank.Initialize(this);

        currentFuelTank = spawnedTank;
    }

    public void NotifyFuelTankRemoved(FuelTankItem item)
    {
        if (currentFuelTank == item)
        {
            currentFuelTank = null;
        }
    }

    private List<Vector2Int> CollectEmptyCells(TrackChunk chunk)
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for (int lane = 0; lane < TrackChunk.LaneCount; lane++)
        {
            for (int row = 0; row < TrackChunk.RowCount; row++)
            {
                if (chunk.IsAreaFree(lane, row, 1))
                {
                    emptyCells.Add(new Vector2Int(lane, row));
                }
            }
        }

        return emptyCells;
    }
}