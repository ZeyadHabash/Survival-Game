using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Player/Input Reader")]
    public class PlayerInputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {
        public Vector2 Movement { get; private set; }
        public bool JumpTriggered { get; private set; }
        public bool DashTriggered { get; private set; }
        public bool AttackTriggered { get; private set; }

        private PlayerInputActions _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new PlayerInputActions();
                _controls.Player.SetCallbacks(this);
            }

            // Initialize all inputs to safe defaults
            Movement = Vector2.zero;
            JumpTriggered = false;
            DashTriggered = false;
            AttackTriggered = false;

            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            if (_controls != null)
            {
                _controls.Player.Disable();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // This will automatically be Vector2.zero when button is released
            Movement = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Only set to true on performed, automatically false on cancel
            if (context.performed)
                JumpTriggered = true;
            else if (context.canceled)
                JumpTriggered = false;
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
                DashTriggered = true;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                AttackTriggered = true;
        }

        // Call this at the end of the frame in PlayerController to consume one-shot triggers
        public void ClearInputTriggers()
        {
            JumpTriggered = false;
            DashTriggered = false;
            AttackTriggered = false;
        }

        // Empty implementations for unused actions
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnInteract(InputAction.CallbackContext context) { }
        public void OnCrouch(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        // public void OnSprint(InputAction.CallbackContext context) { }
    }
}