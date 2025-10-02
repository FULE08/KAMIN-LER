using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour
{
    private GameManager gamemanager;
    public int Cost = 20;
    private int UpgradeCost = 25;
    protected int Health = 50;
    public int Level = 1;

    public virtual void OnDamaged(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Upgraded()
    {
        if (gamemanager.Currency >= UpgradeCost && Level > 5)
        {
            gamemanager.Currency -= UpgradeCost;
            Level++;
            Health *= 2;
            UpgradeCost *= 2;
        }

        else if (gamemanager.Currency >= UpgradeCost)
        {
            Debug.Log($"Need {gamemanager.Currency}$ more");
        }

        else
        {
            Debug.Log("Max Level");
        }
    }
    void Update()
    {
        if (Level == 5)
            {
                WallChoose.Instance.ShowUpgrade(this);
            }
    }
}
