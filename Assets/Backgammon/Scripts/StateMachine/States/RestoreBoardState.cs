namespace Backgammon
{
    public class RestoreBoardState : GameState
    {
        public RestoreBoardState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState()
        {
            // TODO: UNDO LAST MOVE

            // IF BOARD HAS NOT BEEN RESET - UNDO EACH PLAYER MOVE
            if (!Context.BoardHasBeenRestored)
            {
                Context.BoardHasBeenRestored = true;
                for (int m = Context.PlayerMovesInfo.Length - 1; m >= 0; m--) 
                    UndoMove(Context.PlayerMovesInfo[m]);
            }

            if (Context.AnalysisUI.ClickedContinue)
            {
                Context.PlayoutProMoves = true;
                ActiveState = GameStateMachine.EGameState.GeneralInfo;
            }
            else ActiveState = GameStateMachine.EGameState.ObserveBoard;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void UndoMove(GameStateContext.MoveInfo move)
        {
            if (move.pointFrom == 0) return;

            Counter counter;

            // FROM TO
            PlayablePosition pointFrom = move.pointFrom == 25 ? Context.BarManager.PlayerBar :
                                                Context.PointsManager.GetPlayerPointByID(move.pointFrom);

            PlayablePosition pointTo = move.pointTo == 0 ? Context.HomeManager.PlayerHome :
                                Context.PointsManager.GetPlayerPointByID(move.pointTo);

            counter = pointTo.PopCounter();

            if (move.pointFrom == 25) counter.SetCounterToMoveToBar(Context.BarManager.PlayerBar.GetCounterOffsetPosition());
            else counter.SetCounterToMoveToPosition(pointFrom.GetCounterOffsetPosition());

            pointFrom.PushCounter(counter);

            // BLOT
            if (move.ifBlot)
            {
                counter = Context.BarManager.OpponentBar.PopCounter();
                counter.SetCounterToMoveToPosition(pointTo.GetCounterOffsetPosition());
                pointTo.PushCounter(counter);
            }

            UnityEngine.Debug.Log($"RESTORE: {pointTo.GetID()} -> {pointFrom.GetID()}");
        }
    }
}