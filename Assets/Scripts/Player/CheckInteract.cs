using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

public class CheckInteract : MonoBehaviour
{
    [SerializeField] private Sprite interactKey;
    private InputSystem_Actions controls => InputManager.Instance.controls;
    private IInteractable currentInteractable;
    private bool isInteractionActive;

    [SerializeField]  public GameObject interactedObject;

    private void Update()
    {
        if (controls.Player.Interact.triggered && currentInteractable != null && currentInteractable.CanInteract)
        {
            TryInteract();
            currentInteractable = null;
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            interactable.OnRangeStay();
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
