namespace Game.Player.States
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateMachine stateMachine;
        protected PlayerController controller;

        public PlayerBaseState(PlayerStateMachine stateMachine, PlayerController controller)
        {
            this.stateMachine = stateMachine;
            this.controller = controller;
        }

        public virtual void Enter() { }
        public virtual void HandleInput() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }
    }
}