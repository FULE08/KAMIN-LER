using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int currency = 100;
    public int health = 10;
    public int waves = 0;

    [Header("References")]
    public Map map;
    public Camera mainCamera;

    [Header("Units")]
    public GameObject[] wallPrefabs; 

    public Vector2 MouseLoc { get; private set; }

    void Update()
    {
        TrackMouse();

        if (Input.GetMouseButtonDown(0))
        {
            PlaceWall(MouseLoc, 0);
        }
    }

    public void EnemyEntered(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    public void PlaceWall(Vector2 pos, int wallIndex)
    {
        if (wallIndex < 0 || wallIndex >= wallPrefabs.Length) return;

        GameObject prefab = wallPrefabs[wallIndex];
        Wall wall = prefab.GetComponent<Wall>();
        if (wall == null) return;
        if (!map.IsInsideMap(pos))
        {
            Debug.Log("Invalid position: outside map!");
            return;
        }

        if (currency >= wall.cost)
        {
            Vector2 snappedPos = map.GetPosition(pos);
            GameObject unit = Instantiate(prefab, snappedPos, Quaternion.identity);
            unit.transform.localScale = new Vector3(map.cellSize, map.cellSize, 1);
            currency -= wall.cost;
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    public void Wave()
    {
        waves++;
        Debug.Log("Starting wave: " + waves);
    }

    private void Paycheck(int income)
    {
        currency += income;
    }

    private void TrackMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -mainCamera.transform.position.z;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        MouseLoc = new Vector2(mouseWorld.x, mouseWorld.y);
    }
}
