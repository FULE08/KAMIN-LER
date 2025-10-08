using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int Currency = 100;
    public int Health = 10;
    public int Waves = 0;

    [Header("References")]
    public Map map;                 
    public Camera mainCamera;       

    [Header("Units")]
    public GameObject[] wallPrefabs;
    public GameObject enemyPrefab;

    public Vector2 MouseLoc { get; private set; }
    private HashSet<Vector2> occupiedCells = new HashSet<Vector2>();

    void Start()
    {
        Wave(); 
    }

    void Update()
    {
        TrackMouse();

        if (Input.GetMouseButtonDown(0))
        {
            CellClicked(MouseLoc, 0);
        }
    }

    public void EnemyEntered(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    public void CellClicked(Vector2 pos, int wallIndex)
    {
        if (wallIndex < 0 || wallIndex >= wallPrefabs.Length) return;

        GameObject prefab = wallPrefabs[wallIndex];
        Wall wallData = prefab.GetComponent<Wall>();
        if (wallData == null) return;

        Vector2 snappedPos = map.GetPosition(pos);

        if (!map.IsInsideMap(snappedPos))
        {
            Debug.Log("Invalid position: outside map!");
            return;
        }

        if (snappedPos == map.MainBasePos || snappedPos == map.EnemyBasePos)
        {
            Debug.Log("Cannot place wall on base!");
            return;
        }

        if (occupiedCells.Contains(snappedPos))
        {
            Debug.Log("Cell already occupied!");
            CheckWall();
            return;
        }

        if (Currency >= wallData.Cost)
        {
            GameObject unit = Instantiate(prefab, snappedPos, Quaternion.identity, map.transform);
            unit.transform.localScale = new Vector3(map.cellSize, map.cellSize, 1);
            Currency -= wallData.Cost;

            occupiedCells.Add(snappedPos); // mark cell occupied
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }
    void CheckWall()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Wall wall = hit.collider.GetComponentInParent<Wall>();
            if (wall != null)
            {
                Debug.Log($"Clicked on wall: {wall.name}");
                wall.Upgraded();
            }
        }
    }
    
    public void SpawnEnemy(Vector2 position)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab not assigned.");
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
        enemyObj.transform.localScale = new Vector3(map.cellSize, map.cellSize, 1);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.MoveSpeed = 2f;
            enemy.Damage = 10;
        }
    }

    public void Wave()
    {
        Waves++;
        Debug.Log("Starting wave: " + Waves);
        SpawnEnemy(map.EnemyBasePos);
    }

    private void Paycheck(int income)
    {
        Currency += income;
    }

    private void TrackMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -mainCamera.transform.position.z;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        MouseLoc = new Vector2(mouseWorld.x, mouseWorld.y);
    }
}
