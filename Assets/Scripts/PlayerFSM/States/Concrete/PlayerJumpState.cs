using UnityEngine;

namespace Game.Player.States
{
    public class PlayerJumpState : PlayerBaseState
    {
        private float _jumpForce = 12f;
        private float _airMoveSpeed = 5f;
        private bool _hasLanded;
        private bool _hasJumped; // Track if we've already executed the jump

        public PlayerJumpState(PlayerStateMachine stateMachine, PlayerController controller)
            : base(stateMachine, controller) { }

        public override void Enter()
        {
            _hasLanded = false;
            _hasJumped = false;

            // Only execute jump if we're grounded (entering from ground state)
            if (IsGrounded())
            {
                Jump();
                _hasJumped = true;
            }
            // If entering while already in air (fell off platform), don't jump
        }

        public override void LogicUpdate()
        {
            // IMPORTANT: Do NOT allow jumping while already in air
            // Ignore jump input completely while in this state

            // Allow mid-air dashing if your game design permits
            if (controller.InputReader.DashTriggered)
            {
                stateMachine.ChangeState(controller.DashState);
                return;
            }

            // Allow mid-air attack
            if (controller.InputReader.AttackTriggered)
            {
                stateMachine.ChangeState(controller.AttackState);
                return;
            }

            // Check if we have landed
            CheckTransition();
        }

        public override void PhysicsUpdate()
        {
            // Add air horizontal control
            float moveInput = controller.InputReader.Movement.x;

            if (Mathf.Abs(moveInput) > 0.01f)
            {
                controller.RB.linearVelocity = new Vector2(moveInput * _airMoveSpeed, controller.RB.linearVelocityY);

                // Update facing direction while in air
                if (moveInput > 0.01f)
                {
                    controller.FacingDirection = 1;
                    controller.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (moveInput < -0.01f)
                {
                    controller.FacingDirection = -1;
                    controller.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        private void Jump()
        {
            // Reset vertical velocity for a consistent jump height
            controller.RB.linearVelocity = new Vector2(controller.RB.linearVelocityX, 0);
            controller.RB.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            controller.Anim.SetTrigger("Jump");
        }

        private void CheckTransition()
        {
            // Only check for landing when we're falling (negative Y velocity)
            if (controller.RB.linearVelocityY <= 0f && !_hasLanded)
            {
                if (IsGrounded())
                {
                    _hasLanded = true;

                    // Transition based on input
                    if (Mathf.Abs(controller.InputReader.Movement.x) > 0.01f)
                    {
                        stateMachine.ChangeState(controller.WalkState);
                    }
                    else
                    {
                        stateMachine.ChangeState(controller.IdleState);
                    }
                }
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(controller.GroundCheck.position, controller.GroundCheckRadius, controller.GroundLayer);
        }
    }
}