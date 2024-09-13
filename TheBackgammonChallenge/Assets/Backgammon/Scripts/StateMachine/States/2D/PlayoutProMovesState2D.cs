namespace Backgammon
{
    public class PlayoutProMovesState2D : GameState2D
    {
        public PlayoutProMovesState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: PLAYOUT PRO MOVES");

            Context.IsPlayersMakingMoves = false;
            Context.CounterMoveIndex = 0;
            Context.CountersToMoveIndex = Context.TotalMovesThisTurn;

            // A.I. GAME - PLAYER CAN USE LESS THAN MOVES AVAILABLE
            // NO HISTORIC DATA TO FALL BACK ON IN MOVE_COUNTERS
            if (Main.Instance.IfPlayerVsAI)
            {
                Context.CountersToMoveIndex = 0;

                foreach (var m in Context.PlayerMovesInfo)
                {
                    if (m.pointFrom == 0) break;
                    else Context.CountersToMoveIndex += 1;
                }
            }

            Context.DiceRollsUI.ResetImageSizes();

            // DISPLAY PRO MOVES
            //Context.ObserveAnalysisManager.SetMarkersToPoints(Context.RecordedMovesInfo);
            Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.RecordedMovesInfo, false, true);

            ActiveState = GameStateMachine2D.EGameState2D.MoveCounters;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}