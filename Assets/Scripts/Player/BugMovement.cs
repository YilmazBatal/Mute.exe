using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class BugMovement : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 0.5f;
    private float cooldownTimer = 0f; 

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float runSpeedMultiplier = 1.5f;
    private float currentSpeed;

    [Header("Running")]
    [Header("Stamina System")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 10f;
    [SerializeField] private float staminaRegenRate = 20f; 
    [SerializeField] private float regenDelay = 1.5f;

    private float currentStamina;
    private float regenTimer = 0f;
    private bool isRunning;

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

        currentStamina = maxStamina;
        staminaBar.fillAmount = 1;
        currentSpeed = walkSpeed;
    }

    float rotationSpeed = 720f; // Degrees per second
    void Update()
    {
        if (!DialogueManager.Instance.dialogueIsPlaying)
        {
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

            HandleStaminaAndRunning();
            GenerateBullet();
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * currentSpeed;

        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Set Animation
        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);
    }

    private void HandleStaminaAndRunning()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.001f;
        bool runRequested = controls.Player.Sprint.IsPressed();

        if (runRequested && isMoving && currentStamina > 0)
        {
            isRunning = true;
            currentSpeed = walkSpeed * runSpeedMultiplier;

            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

            regenTimer = regenDelay;

            UpdateStaminaUI();
        }
        else
        {
            isRunning = false;
            currentSpeed = walkSpeed;

            if (regenTimer > 0)
            {
                regenTimer -= Time.deltaTime;
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

                UpdateStaminaUI();
            }
        }
    }

    private void UpdateStaminaUI()
    {
        float targetFill = currentStamina / maxStamina;

        LeanTween.value(staminaBar.gameObject, staminaBar.fillAmount, targetFill, 0.1f)
            .setOnUpdate((float val) =>
            {
                staminaBar.fillAmount = val;
            });
    }

    void GenerateBullet()
    {
        if (controls.Player.Attack.IsPressed() && cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;

            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
            AudioManager.Instance.PlaySFX("Shooting");
        }
    }
}
