using UnityEngine;

namespace Game.Player.States
{
    public class PlayerJumpState : PlayerBaseState
    {
        // Jump Physics Settings
        private float _jumpForce = 50f;
        private float _airMoveSpeed = 5f;

        // Gravity Modifiers for realistic feel
        private float _gravityScale = 3f;        // Normal falling gravity
        private float _fallGravityScale = 4f;    // Faster falling when moving down
        private float _jumpCutGravityScale = 6f; // Extra gravity when releasing jump early

        // Jump Control
        private bool _hasLanded;
        private bool _hasJumped;
        private bool _isJumpCut;  // Track if player released jump button early

        public PlayerJumpState(PlayerStateMachine stateMachine, PlayerController controller)
            : base(stateMachine, controller) { }

        public override void Enter()
        {
            _hasLanded = false;
            _hasJumped = false;
            _isJumpCut = false;

            // Only execute jump if we're grounded (entering from ground state)
            if (IsGrounded())
            {
                Jump();
                _hasJumped = true;
            }
            // If entering while already in air (fell off platform), don't jump

            // Set default gravity scale
            controller.RB.gravityScale = _gravityScale;
        }

        public override void LogicUpdate()
        {
            // JUMP CUT - Release jump button early to fall faster (realistic feel)
            if (_hasJumped && !_isJumpCut && controller.RB.linearVelocityY > 0)
            {
                // If player releases jump button while still going up
                if (!controller.InputReader.JumpTriggered)
                {
                    _isJumpCut = true;
                    // Cut the jump short by reducing upward velocity
                    controller.RB.linearVelocity = new Vector2(
                        controller.RB.linearVelocityX,
                        controller.RB.linearVelocityY * 0.5f  // Cut jump height in half
                    );
                }
            }

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
            // REALISTIC GRAVITY - Apply different gravity based on vertical velocity
            ApplyRealisticGravity();

            // Air horizontal control
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

        public override void Exit()
        {
            // Reset gravity to default when leaving jump state
            controller.RB.gravityScale = _gravityScale;
        }

        private void Jump()
        {
            // Reset vertical velocity for a consistent jump height
            controller.RB.linearVelocity = new Vector2(controller.RB.linearVelocityX, 0);

            // Apply jump force as impulse for instant burst
            controller.RB.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            controller.Anim.SetTrigger("Jump");
        }

        private void ApplyRealisticGravity()
        {
            // Apply different gravity scales based on vertical velocity for more realistic feel

            if (_isJumpCut)
            {
                // Player released jump button - fall faster
                controller.RB.gravityScale = _jumpCutGravityScale;
            }
            else if (controller.RB.linearVelocityY < 0)
            {
                // Falling - apply stronger gravity for snappier feel
                controller.RB.gravityScale = _fallGravityScale;
            }
            else if (controller.RB.linearVelocityY > 0)
            {
                // Rising - normal gravity
                controller.RB.gravityScale = _gravityScale;
            }

            // Optional: Cap falling speed to prevent falling too fast
            float maxFallSpeed = -20f;
            if (controller.RB.linearVelocityY < maxFallSpeed)
            {
                controller.RB.linearVelocity = new Vector2(controller.RB.linearVelocityX, maxFallSpeed);
            }
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