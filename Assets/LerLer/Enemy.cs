using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int Health { get; private set; }
    public int Damage { get; set; }
    public float MoveSpeed { get; set; }
    public int Level { get; private set; }

    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float moveDelay = 0.4f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float wallCheckRadius = 0.4f;

    private float moveTimer;
    private float attackTimer;
    private bool isAttacking;
    private Vector3 targetPos;
    private Transform mainBaseTransform;

    private void Awake()
    {
        Health = maxHealth;
        Damage = baseDamage;
        MoveSpeed = 4f;
        Level = 1;
    }

    private void Start()
    {
        GameObject mainBase = GameObject.Find("MainBase");
        if (mainBase != null)
            mainBaseTransform = mainBase.transform;

        targetPos = SnapToGrid(transform.position);
    }

    private void Update()
    {
        moveTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (isAttacking)
            return;

        // Move every "moveDelay" seconds
        if (moveTimer >= moveDelay)
        {
            MoveOneStep();
            moveTimer = 0f;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            MoveSpeed * Time.deltaTime
        );

        Vector3 dir = targetPos - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void MoveOneStep()
    {
        if (mainBaseTransform == null)
            return;

        Vector3 current = SnapToGrid(transform.position);
        Vector3 target = SnapToGrid(new Vector3(
            mainBaseTransform.position.x,
            mainBaseTransform.position.y,
            transform.position.z
        ));

        if (Vector3.Distance(current, target) < 0.1f)
            return;

        Vector3[] directions =
        {
            new Vector3(gridSize, 0, 0),
            new Vector3(-gridSize, 0, 0),
            new Vector3(0, gridSize, 0),
            new Vector3(0, -gridSize, 0)
        };

        Vector3 bestMove = current;
        float bestDistance = Vector3.Distance(current, target);

        foreach (var dir in directions)
        {
            Vector3 possibleMove = current + dir;
            float distanceToBase = Vector3.Distance(possibleMove, target);

            if (IsBlockedByStrongWall(possibleMove))
                continue;

            if (distanceToBase < bestDistance)
            {
                bestDistance = distanceToBase;
                bestMove = possibleMove;
            }
        }

        if (bestMove != current)
        {
            if (CheckForWall(bestMove))
                return;

            targetPos = bestMove;
            targetPos.z = transform.position.z;
        }
    }

    private bool CheckForWall(Vector3 nextPos)
    {
        Collider[] hits = Physics.OverlapSphere(nextPos, wallCheckRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Wall wall))
            {
                if (wall.Level <= Level)
                {
                    StartCoroutine(AttackWall(wall));
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsBlockedByStrongWall(Vector3 nextPos)
    {
        Collider[] hits = Physics.OverlapSphere(nextPos, wallCheckRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Wall wall))
            {
                if (wall.Level > Level)
                    return true;
            }
        }
        return false;
    }

    private IEnumerator AttackWall(Wall wall)
    {
        isAttacking = true;

        while (wall != null)
        {
            if (attackTimer >= attackDelay)
            {
                wall.OnDamaged(Damage);
                attackTimer = 0f;
            }

            yield return null;
        }

        isAttacking = false;
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            pos.z
        );
    }

    public void OnDamaged(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }
}
