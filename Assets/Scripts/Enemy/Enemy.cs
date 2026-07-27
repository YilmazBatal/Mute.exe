using Assets.Scripts.Managers;
using Pathfinding;
using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField, Range(0.02f, 0.5f)] protected float flashTime = 0.15f;
    [SerializeField, Range(0.01f, 1f)] protected float knockbackDuration = 0.15f;
    protected float currentHealth;

    [Header("VFX")]
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] protected Material flashMaterial;
    [SerializeField] protected ParticleSystem particle;

    protected Transform target;
    protected bool isDead = false;
    protected bool _hasValidPath = true; 
    protected AIPath path;
    protected Seeker seeker;
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    protected Animator animator;


    #region Enable & Disable
    protected virtual void OnEnable()
    {
        seeker.pathCallback += OnPathComplete;
        EventManager.GameEvents.OnPlayerSpawned += HandlePlayerSpawned;
    }
    protected virtual void OnDisable() {
        seeker.pathCallback -= OnPathComplete;
        EventManager.GameEvents.OnPlayerSpawned -= HandlePlayerSpawned;
    }
    private void HandlePlayerSpawned(Transform playerTransform) => target = playerTransform;

    #endregion

    protected virtual void Awake()
    {
        path = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        StartCoroutine(FlashEffect(flashMaterial));

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        isDead = true;

        sr.enabled = false; 
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        AudioManager.Instance.PlaySFX("EnemyDie");
        particle.Play();

        while (particle.IsAlive(true))
        {
            yield return null; 
        }

        Destroy(gameObject);
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
        {
            _hasValidPath = false;
        }
        else
        {
            _hasValidPath = true;
        }
    }

    protected abstract void Move();

    private IEnumerator FlashEffect(Material flashMaterial)
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(flashTime);
        sr.material = defaultMaterial;
    }

    public void ApplyKnockback(Vector2 forceDirection, float force)
    {
        StartCoroutine(KnockbackRoutine(forceDirection, force));
    }

    private IEnumerator KnockbackRoutine(Vector2 forceDirection, float force)
    {
        path.canMove = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(forceDirection.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        path.canMove = true;
    }
    protected bool IsTargetReachable()
    {
        GraphNode enemyNode = AstarPath.active.GetNearest(transform.position).node;
        GraphNode targetNode = AstarPath.active.GetNearest(target.position).node;

        return PathUtilities.IsPathPossible(enemyNode, targetNode);
    }
}