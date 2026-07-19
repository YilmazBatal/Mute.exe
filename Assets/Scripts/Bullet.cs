using Assets.Scripts.Managers;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;       // Merminin uçuş hızı
    [SerializeField] private float lifeTime = 2f;     // Kaç saniye sonra yok olacağı
    private ParticleSystem explosion;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = transform.up * speed;

        InstantiateAnimation();

        Destroy(gameObject, lifeTime);
    }
    private void InstantiateAnimation()
    {
        LeanTween.value(gameObject, 0f, 1f, 0.2f)
            .setEaseInOutCubic()
            .setOnUpdate((float value) =>
            {
                transform.localScale = new Vector3(value, value, value);
            });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacles"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("ToxicWall"))
        {
            explosion = collision.transform.GetChild(0).GetComponent<ParticleSystem>();
            explosion.Play();

            if (collision.transform.childCount > 0)
                collision.transform.DetachChildren();

            AudioManager.Instance.PlaySFX("Explosion");
            GameManager.Instance.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

            DestroyDoor(1f, 0f, 0.2f, collision.gameObject);
        }
    }

    private void DestroyDoor(float from, float to, float duration, GameObject objToDestroy)
    {
        SpriteRenderer sr = objToDestroy.GetComponent<SpriteRenderer>();
        Collider2D[] cl = objToDestroy.GetComponents<Collider2D>();
        foreach (var item in cl)
        {
            if (item.isTrigger == true)
                item.enabled = false;
        }

        if (sr == null)
            return;
        LeanTween.value(objToDestroy, from, to, duration)
            .setEaseInOutCubic()
            .setOnUpdate((float value) =>
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, value);
            })
            .setOnComplete(() =>
            {
                Destroy(objToDestroy);
                Destroy(gameObject);
            }); ;
    }
}