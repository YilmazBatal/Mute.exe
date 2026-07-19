using Assets.Scripts.Managers;
using UnityEngine;

public class BugMovement : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 0.5f;
    private float cooldownTimer = 0f; 

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 5f;

    [HideInInspector] private AudioSource audioSource;
    [HideInInspector] public Rigidbody2D rb;
    private Animator animator;
    private InputSystem_Actions controls => InputManager.Instance.controls;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    float rotationSpeed = 720f; // Degrees per second
    void Update()
    {
        if (!DialogueManager.Instance.dialogueIsPlaying)
        {
            // Cooldown süresini zamanla azalt
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            moveInput = controls.Player.Move.ReadValue<Vector2>();

            rb.angularVelocity = 0;
            if (moveInput.sqrMagnitude > 0.001f)
            {
                float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, -targetAngle);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }

            GenerateBullet();
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Set Animation
        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);
    }

    void GenerateBullet()
    {
        // controls.Player.Attack.IsPressed() -> Tuşa basılı tutulduğu sürece true döner
        if (controls.Player.Attack.IsPressed() && cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;

            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
            AudioManager.Instance.PlaySFX("Shooting");

        }
    }
}