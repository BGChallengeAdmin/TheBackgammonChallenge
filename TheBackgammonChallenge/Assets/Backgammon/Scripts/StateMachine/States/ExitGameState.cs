namespace Backgammon
{
    public class ExitGameState : GameState
    {
        public ExitGameState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState()
        {
            ActiveState = StateKey;

            if (Context.AIDataHandler.ServerConnected)
                Context.AIDataHandler.DisconnectFromTcpServer();

            Game.IfGameConcluded = true;
        }

        public override void UpdateState() { }

        public override void ExitState()  { }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}