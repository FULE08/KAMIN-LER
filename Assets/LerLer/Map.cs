using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 6;
    public int height = 7;
    public float cellSize = 1f;

    [Header("Prefab Settings")]
    public GameObject cellPrefab;
    public GameObject enemyBasePrefab;
    public GameObject mainBasePrefab;

    public List<Vector2> Grid { get; private set; }
    public Vector2 EnemyBasePos { get; private set; }
    public Vector2 MainBasePos { get; private set; }

    void Awake()
    {
        GenerateGrid();
        PlaceBases();
    }

    private void GenerateGrid()
    {
        Grid = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 cellPos = new Vector2(x * cellSize, y * cellSize);
                Grid.Add(cellPos);
                if (cellPrefab != null)
                {
                    GameObject tile = Instantiate(cellPrefab, cellPos, Quaternion.identity, transform);
                    tile.transform.localScale = new Vector3(cellSize, cellSize, 1);
                    tile.name = $"Cell_{x}_{y}";
                }
            }
        }
    }

    private void PlaceBases()
    {
        EnemyBasePos = new Vector2(0 * cellSize, (height - 1) * cellSize);
        if (enemyBasePrefab != null)
        {
            GameObject enemyBase = Instantiate(enemyBasePrefab, EnemyBasePos, Quaternion.identity, transform);
            enemyBase.transform.localScale = new Vector3(cellSize, cellSize, 1);
            enemyBase.name = "EnemyBase";
        }
        MainBasePos = new Vector2((width - 1) * cellSize, 0 * cellSize);
        if (mainBasePrefab != null)
        {
            GameObject mainBase = Instantiate(mainBasePrefab, MainBasePos, Quaternion.identity, transform);
            mainBase.transform.localScale = new Vector3(cellSize, cellSize, 1);
            mainBase.name = "MainBase";
        }
    }

    public bool IsInsideMap(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);
        return (x >= 0 && x < width && y >= 0 && y < height);
    }


    public Vector2 GetPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);
        return new Vector2(x * cellSize, y * cellSize);
    }
}
