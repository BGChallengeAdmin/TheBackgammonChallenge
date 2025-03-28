namespace Backgammon
{
    public class ExitGameState2DS : GameState2DStrategy
    {
        public ExitGameState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: EXIT GAME");

            ActiveState = StateKey;

            if (Context.AIDataHandler.ServerConnected)
                Context.AIDataHandler.DisconnectFromTcpServer();

            Game2DStrategy.IfGameConcluded = true;
        }

        public override void UpdateState() { }

        public override void ExitState() { }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}