using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wall : MonoBehaviour
{
    protected GameManager gamemanager;
    protected TextMeshProUGUI TextLabel;
    public int Cost = 20;

    public int UpgradeCost = 25;
    protected int Health = 50;
    protected int MaxLevel = 5;
    public int Level = 1;

    public int CurrentHealth => Health;

    void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        TextLabel = GetComponentInChildren<TextMeshProUGUI>();
    }
    
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
        if (gamemanager.Currency >= UpgradeCost && Level < MaxLevel)
        {
            gamemanager.Currency -= UpgradeCost;
            Level++;
            Health *= 2;
            UpgradeCost *= 2;
            Debug.Log($"The {gameObject.name} has been upgraded, Its level is now {Level}");
        }
        else if (Level >= MaxLevel)
        {
            Debug.Log("Max Level");
        }
        else
        {
            int needMore = UpgradeCost - gamemanager.Currency;
            Debug.Log($"Need {needMore}$ more");
        }
    }

    void Update()
    {
        UpdateLabel();
        if (Level == MaxLevel && gameObject.name == "Wall(Clone)")
        {
            WallChoose.Instance.ShowUpgrade(this);
        }
    }
    public virtual void UpdateLabel()
    {
        if (TextLabel != null)
            TextLabel.text = $"Level : {Level}\n Health : {Health}";
    }
}
