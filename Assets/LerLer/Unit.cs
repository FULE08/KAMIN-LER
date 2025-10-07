using UnityEngine;
using System.Collections.Generic;
public enum AttackPattern { Circle, Cross, Diagonal }

public class Unit : MonoBehaviour
{
    private Vector2 Position => transform.position;
    private float Rate;
    private int Damage;
    private float attackCooldown;
    public int Level { get; private set; }
    public int Health { get; private set; }
    public int Price { get; private set; }
    public int PlacePrice { get; private set; }
    public AttackPattern Pattern { get; private set; }

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int baseDamage = 15;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float range = 2f;

    private void Awake()
    {
        Health = maxHealth;
        Damage = baseDamage;
        Rate = attackRate;
        Level = 1;
        Pattern = AttackPattern.Circle; // default
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;
        List<Enemy> enemies = FindEnemies();

        if (enemies.Count > 0 && attackCooldown <= 0f)
        {
            foreach (var enemy in enemies)
                Attack(enemy);

            attackCooldown = Rate;
        }
    }

    private List<Enemy> FindEnemies()
    {
        List<Enemy> found = new List<Enemy>();
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (var e in enemies)
        {
            float dist = Vector2.Distance(Position, e.transform.position);
            if (IsEnemyInPattern(e.transform.position, dist))
                found.Add(e);
        }

        return found;
    }

    private bool IsEnemyInPattern(Vector2 enemyPos, float dist)
    {
        Vector2 dir = (enemyPos - Position).normalized;

        switch (Pattern)
        {
            case AttackPattern.Circle:
                return dist <= range;

            case AttackPattern.Cross:
                return (Mathf.Abs(enemyPos.x - Position.x) <= range && Mathf.Approximately(enemyPos.y, Position.y)) ||
                       (Mathf.Abs(enemyPos.y - Position.y) <= range && Mathf.Approximately(enemyPos.x, Position.x));

            case AttackPattern.Diagonal:
                return Mathf.Abs(enemyPos.x - Position.x) == Mathf.Abs(enemyPos.y - Position.y)
                       && dist <= range;

            default:
                return false;
        }
    }

    public void Attack(Enemy enemy)
    {
        if (enemy != null)
            enemy.OnDamaged(Damage);
    }

    public void OnDamaged(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }

    public void Upgrade()
    {
        Level++;
        Damage += 5;
        Health += 20;
        if (Level == 2) Pattern = AttackPattern.Cross;
        else if (Level == 3) Pattern = AttackPattern.Diagonal;
    }
}
