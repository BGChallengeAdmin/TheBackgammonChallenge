namespace Backgammon
{
    public class PlayoutProMovesState2DS : GameState2DStrategy
    {
        public PlayoutProMovesState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: PLAYOUT PRO MOVES");

            Context.TurnConfig.IsPlayersMakingMoves = false;
            Context.TurnConfig.CounterMoveIndex = 0;
            Context.TurnConfig.CountersToMoveIndex = Context.TurnConfig.TotalMovesThisTurn;

            Context.Strategy.PlayoutProMovesEntry(Context);

            Context.DiceRollsUI.ResetImageSizes();

            // DISPLAY PRO MOVES
            Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TurnConfig.RecordedMovesInfo, replayingPro: true);

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.MoveCounters;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}