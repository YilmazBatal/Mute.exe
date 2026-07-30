using Assets.Scripts.Managers;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChickenLaser : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float recoil = 0.25f;
    [SerializeField] private float damage = 20f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector3 targetPosition)
    {
        float randomRecoil = Random.Range(-recoil, recoil);
        Vector2 direction = ((targetPosition + (Vector3.one * randomRecoil)) - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        AudioManager.Instance.PlaySFX("EnemyShoot");

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BugMovement>(out BugMovement player))
        {
            player.TakeDamage(damage);
            player.ApplyKnockback((collision.transform.position - transform.position).normalized, 100 / player.GetComponent<Rigidbody2D>().mass);

            AudioManager.Instance.PlaySFX("PlayerHurt");


            GameManager.Instance.GetComponent<CinemachineImpulseSource>().DefaultVelocity = new Vector3(0.1f, 0.1f, 0);
            GameManager.Instance.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacles"))
        {
            GameManager.Instance.GetComponent<CinemachineImpulseSource>().DefaultVelocity = new Vector3(0.05f, 0.05f, 0);
            GameManager.Instance.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            Destroy(gameObject);
        }
        
    }
}
