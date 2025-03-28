namespace Backgammon
{
    public class RestoreBoardState2DS : GameState2DStrategy
    {
        public RestoreBoardState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: RESTORE BOARD");

            // IF BOARD HAS NOT BEEN RESET - UNDO EACH PLAYER MOVE
            if (!Context.TurnConfig.BoardHasBeenRestored)
            {
                Context.TurnConfig.BoardHasBeenRestored = true;
                for (int m = Context.TurnConfig.PlayerMovesInfo.Length - 1; m >= 0; m--)
                    UndoMove(Context.TurnConfig.PlayerMovesInfo[m]);
            }

            // DEFAULT TO ANALYSIS OR CONTINUE
            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.AnalysisOrContinue;

            // VIEWING ANALYSIS
            if (Context.AnalysisOrContinueUI.IfAnalysis)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.Analysis;

            // NO A.I. - TIDY UP GENERAL INFO
            if (!Context.TurnConfig.AIDataAvailable)
            {
                Context.TurnConfig.PlayoutProMoves = true;
                Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayoutProMoves;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
            }

            // TEST IF PLAYER IS REPLAYING THE TURN
            if (Context.GeneralInfoUI.IfReplayMove || Context.TurnConfig.UndoPlayerMove)
            {
                for (int playerM = 0; playerM < 4; playerM++)
                    Context.TurnConfig.PlayerMovesInfo[playerM] = Context.TurnConfig.PlayerMovesInfo[playerM].Reset();

                Context.TurnConfig.BoardHasBeenRestored = false;
                Context.TurnConfig.UndoPlayerMove = false;

                Context.TurnConfig.MovesAvailable = Context.TurnConfig.TotalMovesThisTurn;
                Context.TurnConfig.PlayerMoveIndex = 0;
                Context.TurnConfig.CountersToMoveIndex = 0;

                var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
                var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

                Context.DiceManager.SetDiceValues(dice1, dice2);
                Context.DiceRollsUI.ResetImageSizes();

                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.SelectPointFrom;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

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