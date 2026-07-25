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

    [Header("Materials")]
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] protected Material flashMaterial;

    protected AIPath path;
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Awake()
    {
        path = GetComponent<AIPath>();
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
        Destroy(gameObject);
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

}