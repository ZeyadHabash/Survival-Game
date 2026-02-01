using UnityEngine;

namespace Game.Player.States
{
    public class PlayerDashState : PlayerBaseState
    {
        private float dashTimer;

        public PlayerDashState(PlayerStateMachine stateMachine, PlayerController controller)
            : base(stateMachine, controller) { }

        public override void Enter()
        {
            dashTimer = controller.DashDuration;
            controller.RB.linearVelocity = new Vector2(controller.FacingDirection * controller.DashForce, 0);
            controller.Anim.SetTrigger("Dash"); // Optional: add dash animation
        }

        public override void LogicUpdate()
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                // Check if grounded to determine next state
                if (IsGrounded())
                {
                    // Check if there's movement input
                    if (Mathf.Abs(controller.InputReader.Movement.x) > 0.01f)
                    {
                        stateMachine.ChangeState(controller.WalkState);
                    }
                    else
                    {
                        stateMachine.ChangeState(controller.IdleState);
                    }
                }
                else
                {
                    // If in air after dash, go to jump state
                    stateMachine.ChangeState(controller.JumpState);
                }
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(controller.GroundCheck.position, controller.GroundCheckRadius, controller.GroundLayer);
        }
    }
}