using Assets.Scripts.Managers;
using UnityEngine;

public class BugMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 5f;
    [HideInInspector] private AudioSource audioSource;
    public Rigidbody2D rb;
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
}