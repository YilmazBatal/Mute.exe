using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public InputSystem_Actions controls;

        bool submitPressed;

        private void Awake()
        {
            controls = new InputSystem_Actions();

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnEnable()
        {
            controls.Player.Enable();
        }

        void OnDisable()
        {
            controls.Player.Disable();
        }

        public void RegisterSubmitPressed()
        {
            submitPressed = false;
        }
        public bool GetSubmitPressed()
        {
            bool result = submitPressed;
            submitPressed = false;
            return result;
        }
        public void SubmitPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                submitPressed = true;
            }
            else if (context.canceled)
            {
                submitPressed = false;
            }
        }
    }
}
