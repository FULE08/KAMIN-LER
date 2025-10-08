using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int Currency = 100;
    public int Health = 10;
    public int Waves = 0;

    [Header("Wave Settings")]
    public int totalWaves = 10;
    public int baseEnemiesPerWave = 1;
    public int enemyIncreasePerWave = 2;
    public float timeBetweenEnemySpawns = 1f;
    public float timeBetweenWaves = 5f;

    [Header("Enemy Scaling")]
    public int wavesPerEnemyLevelUp = 5;
    public float enemyHealthMultiplier = 1.5f;
    public float enemyDamageMultiplier = 1.3f;

    [Header("Income Settings")]
    public int baseIncome = 50;
    public float incomeMultiplier = 2f;

    [Header("References")]
    public Map map;
    public Camera mainCamera;

    [Header("Prefabs")]
    public GameObject[] wallPrefabs;
    public GameObject[] unitPrefabs;
    public GameObject enemyPrefab;

    public Vector2 MouseLoc { get; private set; }

    private HashSet<Vector2> occupiedCells = new HashSet<Vector2>();
    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool isGameOver = false;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (isGameOver) return;

        TrackMouse();

        if (Input.GetMouseButtonDown(0)) LeftClickAction();
        if (Input.GetMouseButtonDown(1)) RightClickAction();
    }

    IEnumerator GameLoop()
    {
        while (Waves < totalWaves && !isGameOver)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            if (!isGameOver)
            {
                yield return StartCoroutine(SpawnWave());
                yield return new WaitUntil(() => AllEnemiesDefeated());

                if (!isGameOver) GiveIncome();
            }
        }

        if (!isGameOver && Health > 0) Victory();
    }

    IEnumerator SpawnWave()
    {
        Waves++;
        int enemiesToSpawn = baseEnemiesPerWave + (enemyIncreasePerWave * (Waves - 1));
        int enemyLevel = 1 + (Waves - 1) / wavesPerEnemyLevelUp;

        Debug.Log($"Wave {Waves}/{totalWaves} | Enemies: {enemiesToSpawn} | Enemy Level: {enemyLevel}");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (isGameOver) break;
            SpawnEnemy(map.EnemyBasePos, enemyLevel);
            yield return new WaitForSeconds(timeBetweenEnemySpawns);
        }
    }

    private bool AllEnemiesDefeated()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count == 0;
    }

    public void EnemyEntered(int damage)
    {
        Health -= damage;
        Debug.Log($"Base hit! Health: {Health}");

        if (Health <= 0 && !isGameOver) GameOver();
    }

    public void EnemyDied(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        Debug.Log($"Enemy defeated! Remaining: {activeEnemies.Count}");
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log($"=== GAME OVER ===\nSurvived {Waves}/{totalWaves} waves");
    }

    private void Victory()
    {
        isGameOver = true;
        Debug.Log($"=== VICTORY ===\nCompleted all {totalWaves} waves!\nHealth: {Health} | Currency: {Currency}");
    }

    void LeftClickAction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Wall wall = hit.collider.GetComponentInParent<Wall>();
            if (wall != null)
            {
                UpgradeWall(wall);
                return;
            }
        }
        PlaceWall(MouseLoc);
    }

    void RightClickAction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.collider.GetComponentInParent<Unit>();
            if (unit != null)
            {
                UpgradeUnit(unit);
                return;
            }

            Wall wall = hit.collider.GetComponentInParent<Wall>();
            if (wall != null)
            {
                Unit existingUnit = wall.GetComponentInChildren<Unit>();
                if (existingUnit != null)
                    UpgradeUnit(existingUnit);
                else
                    PlaceUnitOnWall(wall);
            }
        }
    }

    void PlaceWall(Vector2 pos)
    {
        if (wallPrefabs.Length == 0) return;

        GameObject prefab = wallPrefabs[0];
        Wall wallData = prefab.GetComponent<Wall>();
        if (wallData == null) return;

        Vector2 snappedPos = map.GetPosition(pos);

        if (!map.IsInsideMap(snappedPos) ||
            snappedPos == map.MainBasePos ||
            snappedPos == map.EnemyBasePos ||
            occupiedCells.Contains(snappedPos))
        {
            Debug.Log("Cannot place wall here!");
            return;
        }

        if (Currency >= wallData.Cost)
        {
            GameObject wall = Instantiate(prefab, snappedPos, Quaternion.identity, map.transform);
            wall.transform.localScale = new Vector3(map.cellSize, map.cellSize, 1);
            Currency -= wallData.Cost;
            occupiedCells.Add(snappedPos);
        }
        else
        {
            Debug.Log($"Not enough currency! Need: {wallData.Cost}, Have: {Currency}");
        }
    }

    void UpgradeWall(Wall wall)
    {
        if (Currency >= wall.UpgradeCost)
        {
            Currency -= wall.UpgradeCost;
            wall.Upgraded();
            Debug.Log($"Wall upgraded to Level {wall.Level}");
        }
        else
        {
            Debug.Log($"Not enough currency! Need: {wall.UpgradeCost}, Have: {Currency}");
        }
    }

    void PlaceUnitOnWall(Wall wall)
    {
        if (unitPrefabs.Length == 0)
        {
            Debug.LogWarning("No unit prefabs assigned!");
            return;
        }

        GameObject unitPrefab = unitPrefabs[0];
        Unit unitData = unitPrefab.GetComponent<Unit>();
        if (unitData == null) return;

        if (Currency >= unitData.PlacePrice)
        {
            GameObject unitObj = Instantiate(unitPrefab, wall.transform.position, Quaternion.identity, wall.transform);
            unitObj.transform.localScale = Vector3.one;
            Currency -= unitData.PlacePrice;
            Debug.Log($"Unit placed on wall. Cost: {unitData.PlacePrice}");
        }
        else
        {
            Debug.Log($"Not enough currency! Need: {unitData.PlacePrice}, Have: {Currency}");
        }
    }

    void UpgradeUnit(Unit unit)
    {
        if (Currency >= unit.Price)
        {
            Currency -= unit.Price;
            unit.Upgrade();
            Debug.Log($"Unit upgraded to Level {unit.Level}");
        }
        else
        {
            Debug.Log($"Not enough currency! Need: {unit.Price}, Have: {Currency}");
        }
    }

    void SpawnEnemy(Vector2 position, int level)
    {
        if (enemyPrefab == null) return;

        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
        enemyObj.transform.localScale = new Vector3(map.cellSize, map.cellSize, 1);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.MoveSpeed = 2f;
            enemy.Damage = Mathf.RoundToInt(1 * Mathf.Pow(enemyDamageMultiplier, level - 1));
            enemy.Health = Mathf.RoundToInt(enemy.Health * Mathf.Pow(enemyHealthMultiplier, level - 1));
            activeEnemies.Add(enemy);
        }
    }

    private void GiveIncome()
    {
        int income = Mathf.RoundToInt(baseIncome * Mathf.Pow(incomeMultiplier, Waves - 1));
        Currency += income;
        Debug.Log($"Wave complete! +{income} currency. Total: {Currency}");
    }

    private void TrackMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -mainCamera.transform.position.z;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        MouseLoc = new Vector2(mouseWorld.x, mouseWorld.y);
    }
}