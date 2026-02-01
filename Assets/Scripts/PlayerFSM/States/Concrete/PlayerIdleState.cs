using UnityEngine;

namespace Game.Player.States
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine stateMachine, PlayerController controller)
            : base(stateMachine, controller) { }

        public override void Enter()
        {
            base.Enter();
            controller.Anim.SetBool("isWalking", false);
            // Stop horizontal movement when entering idle
            controller.RB.linearVelocity = new Vector2(0, controller.RB.linearVelocityY);
        }

        public override void HandleInput()
        {
            // Check if we're grounded first - if not, we shouldn't be in idle state
            if (!IsGrounded())
            {
                // We're in the air somehow, transition to jump state to handle falling
                stateMachine.ChangeState(controller.JumpState);
                return;
            }

            // Transition to Walk if there's movement input
            if (Mathf.Abs(controller.InputReader.Movement.x) > 0.01f)
            {
                stateMachine.ChangeState(controller.WalkState);
                return;
            }

            // Only allow jump if grounded
            if (controller.InputReader.JumpTriggered && IsGrounded())
            {
                stateMachine.ChangeState(controller.JumpState);
                return;
            }

            // Transition to Dash
            if (controller.InputReader.DashTriggered && IsGrounded())
            {
                stateMachine.ChangeState(controller.DashState);
                return;
            }

            // Transition to Attack
            if (controller.InputReader.AttackTriggered && IsGrounded())
            {
                stateMachine.ChangeState(controller.AttackState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            // Ensure player stays stopped horizontally
            controller.RB.linearVelocity = new Vector2(0, controller.RB.linearVelocityY);
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(controller.GroundCheck.position, controller.GroundCheckRadius, controller.GroundLayer);
        }
    }
}