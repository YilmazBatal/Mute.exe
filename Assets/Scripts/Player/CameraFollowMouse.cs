using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowMouse : MonoBehaviour
{
    [Header("Tracking Targets")]
    [SerializeField] private Transform playerTransform;

    [Header("Limits")]
    [SerializeField] private float maxLookDistance = 4f;

    [Tooltip("0 = Camera on Player. 1 = Camera on Mouse.")]
    [SerializeField][Range(0f, 1f)] private float mouseInfluence = 0.25f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined;
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        //FaceToCursor();

        // 1. Farenin ekrandaki pozisyonunu al ve 2D dünya pozisyonuna çevir
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f; // 2D düzlemde tut

        // 2. Oyuncu ile fare arasındaki hedef pozisyonu hesapla (Lerp)
        Vector3 targetPosition = Vector3.Lerp(playerTransform.position, mouseWorldPos, mouseInfluence);

        // 3. CLAMP (Sınırlama): Oyuncudan olan mesafeyi hesapla ve sınırla
        Vector3 offset = targetPosition - playerTransform.position;
        offset = Vector3.ClampMagnitude(offset, maxLookDistance);

        // 4. Objenin yeni pozisyonunu ata
        transform.position = playerTransform.position + offset;
    }

    void FaceToCursor()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f; // 2D düzlemde tut

        Vector3 direction = (mouseWorldPos - playerTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Kamera objesini döndür
        playerTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}