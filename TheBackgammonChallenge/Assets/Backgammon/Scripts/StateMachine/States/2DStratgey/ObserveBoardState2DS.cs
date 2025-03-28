namespace Backgammon
{
    public class ObserveBoardState2DS : GameState2DStrategy
    {
        public ObserveBoardState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: OBSERVE BOARD");

            Context.ObserveBoardUI.SetActive(true);

            if (!Context.ObserveAnalysisManager.CurrentlyActive)
            {
                if (Context.TurnConfig.ClickedPlayerMoveAnalysis)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TurnConfig.PlayerMovesInfo);
                }
                else if (Context.TurnConfig.ClickedProMoveAnalysis)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TurnConfig.RecordedMovesInfo);
                }
                else if (Context.TurnConfig.ClickedTopRankedMoveAnalysis && Context.TurnConfig.AITurnWasAnalysed)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TurnConfig.TopRankedMovesInfo);
                }
                else if (Context.TurnConfig.ClickedOpponentMoveAnalysis)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TurnConfig.OpponentMovesInfo, showingOpponent: true);
                }
                else if (Context.TurnConfig.ShowOpponentRank1Move && Context.TurnConfig.AITurnWasAnalysed)
                {
                    var blackDice = (Context.TurnConfig.IsPlayersTurn && Context.GameConfig.IfPlayerIsBlack) ||
                            (!Context.TurnConfig.IsPlayersTurn && !Context.GameConfig.IfPlayerIsBlack);

                    Context.DiceRollsUI.SetActive(true);
                    Context.DiceRollsUI.SetOpponentDiceFaceValues(!blackDice, Context.TurnConfig.PreviousDice1, Context.TurnConfig.PreviousDice2);

                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TurnConfig.TopRankedMovesInfo, showingOpponent: true);
                }
            }
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            if (!Context.ObserveBoardUI.ClickedContinue) return;

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.Analysis;

            if (Context.TurnConfig.PlayoutProMoves)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
            else if (Context.TurnConfig.ShowOpponentRank1Move)
            {
                Context.DiceRollsUI.SetActive(false);
                Game2D.Context.ShowOpponentRank1Move = false;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;
            }
        }

        public override void ExitState()
        {
            Context.ObserveAnalysisManager.Reset();
            Context.ObserveBoardUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}