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

    public List<Vector2> Grid { get; private set; }

    void Awake()
    {
        GenerateGrid();
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

    public Vector2 GetPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);
        return new Vector2(x * cellSize, y * cellSize);
    }
}
