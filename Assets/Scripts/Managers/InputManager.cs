using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public InputSystem_Actions controls;

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

    }
}
