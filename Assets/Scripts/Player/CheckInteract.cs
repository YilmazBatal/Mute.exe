using Assets.Scripts;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class CheckInteract : MonoBehaviour
{
    [SerializeField] private Sprite interactKey;
    private InputSystem_Actions controls;
    private IInteractable currentInteractable;
    private bool isInteractionActive;
    public Minigames minigame;

    [SerializeField]  public GameObject interactedObject;
    [SerializeField]  public GameObject lastActivePuzzle;

    #region Input Management
    private void Awake() => controls = new InputSystem_Actions();
    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();
    #endregion

    private void Update()
    {
        if (controls.Player.Interact.triggered && !isInteractionActive)
        {
            TryInteract();
            isInteractionActive = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            
            interactable.OnRangeEnter();
        }
    }
    
    private void OnTriggerExit2D (Collider2D collision)
    {
        if ((collision.TryGetComponent<IInteractable>(out IInteractable interactable)))
        {
            currentInteractable = null;
            interactable.OnRangeExit();
        }
    }

    private void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }
}
