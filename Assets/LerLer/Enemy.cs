using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int Health { get; set; }
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
    private bool isMoving;
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

        targetPos = GetCellCenter(transform.position);
        transform.position = targetPos;
    }

    private void Update()
    {
        moveTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (isAttacking || isMoving)
            return;

        if (moveTimer >= moveDelay)
        {
            MoveOneStep();
            moveTimer = 0f;
        }
    }

    private void MoveOneStep()
    {
        if (mainBaseTransform == null)
            return;

        Vector3 current = GetCellCenter(transform.position);
        Vector3 target = GetCellCenter(mainBaseTransform.position);

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

            targetPos = GetCellCenter(bestMove);
            targetPos.z = transform.position.z;

            StartCoroutine(MoveToCell(targetPos));
        }
    }

    private IEnumerator MoveToCell(Vector3 destination)
    {
        isMoving = true;

        while ((transform.position - destination).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                MoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = GetCellCenter(destination);
        isMoving = false;
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

    private Vector3 GetCellCenter(Vector3 pos)
    {
        float x = Mathf.Floor(pos.x / gridSize) * gridSize + gridSize / 2f;
        float y = Mathf.Floor(pos.y / gridSize) * gridSize + gridSize / 2f;
        return new Vector3(x, y, pos.z);
    }

    public void OnDamaged(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }
    void OnDestroy()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.EnemyDied(this);
        }
    }
}
