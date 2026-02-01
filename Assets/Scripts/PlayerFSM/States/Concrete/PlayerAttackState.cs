using UnityEngine;

namespace Game.Player.States
{
    public class PlayerAttackState : PlayerBaseState
    {
        private float _attackDuration = 0.4f;
        private float _timer;

        public PlayerAttackState(PlayerStateMachine stateMachine, PlayerController controller)
            : base(stateMachine, controller) { }

        public override void Enter()
        {
            _timer = _attackDuration;
            controller.RB.linearVelocity = Vector2.zero; // Stop movement during attack
            controller.Anim.SetTrigger("Attack"); // Play animation
        }

        public override void LogicUpdate()
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
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
                    // If in air, go to jump state (falling)
                    stateMachine.ChangeState(controller.JumpState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            // Keep player stationary during attack
            controller.RB.linearVelocity = new Vector2(0, controller.RB.linearVelocityY);
        }

        public override void Exit()
        {
            // Cleanup if needed (e.g., turning off hitboxes)
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(controller.GroundCheck.position, controller.GroundCheckRadius, controller.GroundLayer);
        }
    }
}