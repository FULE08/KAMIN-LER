using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int Currency = 100;
    public int Health = 10;
    public int Waves = 0;

    [Header("References")]
    public Map Map;
    public Camera MainCamera;

    [Header("Units")]
    public GameObject[] WallPrefabs; 

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
        Health -= damage;
        if (Health <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    public void PlaceWall(Vector2 pos, int wallIndex)
    {
        if (wallIndex < 0 || wallIndex >= WallPrefabs.Length) return;

        GameObject prefab = WallPrefabs[wallIndex];
        Wall wall = prefab.GetComponent<Wall>();
        if (wall == null) return;
        if (!Map.IsInsideMap(pos))
        {
            Debug.Log("Invalid position: outside map!");
            return;
        }

        if (Currency >= wall.Cost)
        {
            Vector2 snappedPos = Map.GetPosition(pos);
            GameObject unit = Instantiate(prefab, snappedPos, Quaternion.identity);
            unit.transform.localScale = new Vector3(Map.cellSize, Map.cellSize, 1);
            Currency -= wall.Cost;
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    public void Wave()
    {
        Waves++;
        Debug.Log("Starting wave: " + Waves);
    }

    private void Paycheck(int income)
    {
        Currency += income;
    }

    private void TrackMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -MainCamera.transform.position.z;
        Vector3 mouseWorld = MainCamera.ScreenToWorldPoint(mouseScreen);
        MouseLoc = new Vector2(mouseWorld.x, mouseWorld.y);
    }
}
