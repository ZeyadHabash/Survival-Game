using UnityEngine;

namespace Game.Player.States
{
    public class PlayerWalkState : PlayerBaseState
    {
        private float _walkSpeed = 8f;

        public PlayerWalkState(PlayerStateMachine stateMachine, PlayerController controller)
            : base(stateMachine, controller) { }

        public override void Enter()
        {
            base.Enter();
            controller.Anim.SetBool("isWalking", true);
        }

        public override void LogicUpdate()
        {
            // Check if we're grounded - if not, go to jump state
            if (!IsGrounded())
            {
                stateMachine.ChangeState(controller.JumpState);
                return;
            }

            // Transition to Idle if no input
            if (Mathf.Abs(controller.InputReader.Movement.x) < 0.01f)
            {
                stateMachine.ChangeState(controller.IdleState);
                return;
            }

            // Transition to Jump (only if grounded - double check)
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
            float moveSpeed = controller.InputReader.Movement.x * _walkSpeed;
            controller.RB.linearVelocity = new Vector2(moveSpeed, controller.RB.linearVelocityY);

            // Flip sprite based on movement direction
            if (moveSpeed > 0.01f)
            {
                controller.FacingDirection = 1;
                controller.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveSpeed < -0.01f)
            {
                controller.FacingDirection = -1;
                controller.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public override void Exit()
        {
            controller.Anim.SetBool("isWalking", false);
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(controller.GroundCheck.position, controller.GroundCheckRadius, controller.GroundLayer);
        }
    }
}