namespace Backgammon
{
    public class RestoreBoardState2D : GameState2D
    {
        public RestoreBoardState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: RESTORE BOARD");

            // IF BOARD HAS NOT BEEN RESET - UNDO EACH PLAYER MOVE
            if (!Context.BoardHasBeenRestored)
            {
                Context.BoardHasBeenRestored = true;
                for (int m = Context.PlayerMovesInfo.Length - 1; m >= 0; m--)
                    UndoMove(Context.PlayerMovesInfo[m]);
            }

            // DEFAULT TO ANALYSIS OR CONTINUE
            ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;

            // VIEWING ANALYSIS
            if (Context.AnalysisOrContinueUI.IfAnalysis)
                ActiveState = GameStateMachine2D.EGameState2D.Analysis;

            // NO A.I. - TIDY UP GENERAL INFO
            if (!Context.AIDataAvailable)
            {
                Context.PlayoutProMoves = true;
                ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
            }

            // TEST IF PLAYER IS REPLAYING THE TURN
            if (Context.GeneralInfoUI.IfReplayMove || Context.UndoPlayerMove)
            {
                for (int playerM = 0; playerM < 4; playerM++)
                    Context.PlayerMovesInfo[playerM] = Context.PlayerMovesInfo[playerM].Reset();

                Context.BoardHasBeenRestored = false;
                Context.UndoPlayerMove = false;

                Context.MovesAvailable = Context.TotalMovesThisTurn;
                Context.PlayerMoveIndex = 0;
                Context.CountersToMoveIndex = 0;

                Context.DiceManager.SetDiceValues(Context.Dice1, Context.Dice2);
                Context.DiceRollsUI.ResetImageSizes();

                ActiveState = GameStateMachine2D.EGameState2D.SelectPointFrom;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void UndoMove(GameStateContext2D.MoveInfo move)
        {
            if (move.pointFrom == 0) return;

            Counter2DPrefab counter;

            // FROM TO
            PlayablePosition2D pointFrom = move.pointFrom == 25 ? Context.BarManager.PlayerBar :
                                                Context.PointsManager.GetPlayerPointByID(move.pointFrom);

            PlayablePosition2D pointTo = move.pointTo == 0 ? Context.HomeManager.PlayerHome :
                                Context.PointsManager.GetPlayerPointByID(move.pointTo);

            counter = pointTo.PopCounter();

            if (move.pointFrom == 25) counter.SetCounterToMoveToBar(Context.BarManager.PlayerBar.GetCounterOffsetPosition());
            else counter.SetCounterToMoveToPositionInstant(Context.PointsManager.GetAdjustedPointPosition(pointFrom), false);

            Context.CountersManager.SetCounterSizeAndColour(counter, true);

            pointFrom.PushCounter(counter);

            // BLOT
            if (move.ifBlot)
            {
                counter = Context.BarManager.OpponentBar.PopCounter();
                counter.SetCounterToMoveToPositionInstant(Context.PointsManager.GetAdjustedPointPosition(pointTo), false);
                pointTo.PushCounter(counter);
            }
        }
    }
}