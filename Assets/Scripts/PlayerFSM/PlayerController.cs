using UnityEngine;
using Game.Player.States;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerStateMachine StateMachine { get; private set; }

        // State References
        public PlayerIdleState IdleState { get; private set; }
        public PlayerWalkState WalkState { get; private set; }
        public PlayerDashState DashState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }

        [Header("Components")]
        public Rigidbody2D RB;
        public Animator Anim;
        public PlayerInputReader InputReader; // Your Input System wrapper

        [Header("Settings")]
        public float DashDuration = 0.2f;
        public float DashForce = 20f;
        public int FacingDirection = 1;

        [Header("Physics Detection")]
        public Transform GroundCheck; // Create an empty GameObject as child, position it at player's feet
        public float GroundCheckRadius = 0.2f; // Create an empty GameObject as child, position it at player's feet
        public LayerMask GroundLayer; // Set this to your ground layer in the inspector

        private void Awake()
        {
            StateMachine = new PlayerStateMachine();

            // Initialize States
            IdleState = new PlayerIdleState(StateMachine, this);
            WalkState = new PlayerWalkState(StateMachine, this);
            DashState = new PlayerDashState(StateMachine, this);
            JumpState = new PlayerJumpState(StateMachine, this);
            AttackState = new PlayerAttackState(StateMachine, this);
        }

        private void Start()
        {
            // Make sure input is cleared before starting
            if (InputReader != null)
            {
                InputReader.ClearInputTriggers();
            }

            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            StateMachine.CurrentState.HandleInput();
            StateMachine.CurrentState.LogicUpdate();

            // Consume triggers so they don't fire again next frame
            InputReader.ClearInputTriggers();
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentState.PhysicsUpdate();
        }

        // Optional: Add this for debugging in the Scene view
        private void OnDrawGizmosSelected()
        {
            if (GroundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
            }
        }
    }
}