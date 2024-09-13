namespace Backgammon
{
    public class ExitGameState2D : GameState2D
    {
        public ExitGameState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: EXIT GAME");

            ActiveState = StateKey;

            if (Context.AIDataHandler.ServerConnected)
                Context.AIDataHandler.DisconnectFromTcpServer();

            Game2D.IfGameConcluded = true;
        }

        public override void UpdateState() { }

        public override void ExitState() { }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}