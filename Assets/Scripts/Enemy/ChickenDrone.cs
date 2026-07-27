using Assets.Scripts.Managers;
using Pathfinding;
using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Patrol,
    Idle,
    Chase,
    Attack
}

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
public class ChickenDrone : Enemy
{
    [Header("FSM Settings")]
    [SerializeField] private EnemyState currentState = EnemyState.Idle;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.15f;

    [Header("Combat Ranges")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float patrolRange = 5f;      
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 3f;  
    [SerializeField] private float attackCooldown = 1.5f;
    private Vector3 _startPosition;                     
    private Vector3 _patrolTarget;
    private float _patrolTimer;
    private float nextAttackTime;
    private bool _hasPatrolTarget;
    private float distanceToTarget;

    

    protected override void Awake()
    {
        base.Awake();
        path = GetComponent<AIPath>();
        path.maxSpeed = moveSpeed;

        _startPosition = transform.position;
    }

    private void Start()
    {
        ChangeState(EnemyState.Idle); // start by idle

        if (target == null)
            GetTarget();
    }

    private void GetTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
    }

    private void Update()
    {
        distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (!isDead)
        {
            CheckStateTransitions();
            ExecuteCurrentState();
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }
    }
    private void SetNewPatrolTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRange;
        _patrolTarget = _startPosition + new Vector3(randomCircle.x, randomCircle.y, 0);

        _hasPatrolTarget = true;
        path.isStopped = false;
        path.destination = _patrolTarget;
    }
    #region Finite State Machine (FSM)
    private void CheckStateTransitions()
    {
        if (distanceToTarget <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distanceToTarget <= chaseRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            if (currentState == EnemyState.Chase || currentState == EnemyState.Attack) // to avoid repetetive state change
            {
                ChangeState(EnemyState.Patrol);
            }
        }
    }
    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                path.isStopped = true;
                _patrolTimer = 0f;
                break;

            case EnemyState.Patrol:
                path.isStopped = false;
                _hasPatrolTarget = false; 
                break;

            case EnemyState.Chase:
                path.isStopped = false;
                break;

            case EnemyState.Attack:
                path.isStopped = true;
                break;
        }
    }
    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                _patrolTimer += Time.deltaTime;
                if (_patrolTimer >= patrolWaitTime)
                    ChangeState(EnemyState.Patrol);
                break;

            case EnemyState.Patrol:
                ExecutePatrol();
                break;

            case EnemyState.Chase:
                Move();
                break;

            case EnemyState.Attack:
                PerformAttack();
                break;
        }
    }
    #endregion
    private void ExecutePatrol()
    {
        Debug.Log("Patrol target : " + _hasPatrolTarget);
        if (!_hasPatrolTarget)
            SetNewPatrolTarget();
        if (Vector2.Distance(transform.position, _patrolTarget) < 1.3f)
        {
            
        }

        if (path.reachedDestination || Vector2.Distance(transform.position, _patrolTarget) < 0.3f) // hard code here
        {
            _hasPatrolTarget = false;
            ChangeState(EnemyState.Idle);
        }
    }

    #region Chicken State Commands
    protected override void Move()
    {
        path.maxSpeed = moveSpeed;
        path.destination = target.position;

        if (!IsTargetReachable())
        {
            path.isStopped = true;
            return;
        }

        path.isStopped = false;
        path.destination = target.position;

        if (path.desiredVelocity.x < -0.01f)
            sr.flipX = true;
        else if (path.desiredVelocity.x > 0.01f)
            sr.flipX = false;
    }

    private void PerformAttack()
    {
        if (nextAttackTime <= 0f)
        {
            StartCoroutine(BurstAttackRoutine());

            nextAttackTime = attackCooldown;
        }
        else
        {
            nextAttackTime -= Time.deltaTime;
        }
    }
    private IEnumerator BurstAttackRoutine()
    {
        for (int i = 0; i < burstCount; i++)
        {
            GameObject bullet = Instantiate(laserPrefab, shootingPoint.position, shootingPoint.rotation);
            AudioManager.Instance.PlaySFX("ShootingLaser");

            bullet.GetComponent<ChickenLaser>().Initialize(target.position);

            yield return new WaitForSeconds(burstInterval);
        }
    }
    #endregion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
    }
}