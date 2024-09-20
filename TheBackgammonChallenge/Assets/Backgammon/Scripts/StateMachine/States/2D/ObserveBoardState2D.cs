namespace Backgammon
{
    public class ObserveBoardState2D : GameState2D
    {
        public ObserveBoardState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: OBSERVE BOARD");

            Context.ObserveBoardUI.SetActive(true);

            if (!Context.ObserveAnalysisManager.CurrentlyActive)
            {
                if (Context.ClickedPlayerMoveAnalysis)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.PlayerMovesInfo);
                }
                else if (Context.ClickedProMoveAnalysis)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.RecordedMovesInfo);
                }
                else if (Context.ClickedTopRankedMoveAnalysis && Context.AITurnWasAnalysed)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TopRankedMovesInfo);
                }
                else if (Context.ClickedOpponentMoveAnalysis)
                {
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.OpponentMovesInfo, true);
                }
                else if (Context.ShowOpponentRank1Move && Context.AITurnWasAnalysed)
                {
                    var blackDice = (Context.IsPlayersTurn && Context.IfPlayerIsBlack) ||
                            (!Context.IsPlayersTurn && !Context.IfPlayerIsBlack);
                    
                    Context.DiceRollsUI.SetActive(true);
                    Context.DiceRollsUI.SetOpponentDiceFaceValues(!blackDice, Context.PreviousDice1, Context.PreviousDice2);
                    
                    Context.ObserveAnalysisManager.SetPointPositionsToPoints(Context.TopRankedMovesInfo, true);
                }
            }
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            if (!Context.ObserveBoardUI.ClickedContinue) return;

            ActiveState = GameStateMachine2D.EGameState2D.Analysis;

            if (Context.PlayoutProMoves)
                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
            else if (Context.ShowOpponentRank1Move)
            {
                Context.DiceRollsUI.SetActive(false);
                Game2D.Context.ShowOpponentRank1Move = false;
                ActiveState = GameStateMachine2D.EGameState2D.RollDice;
            }
        }

        public override void ExitState()
        {
            Context.ObserveAnalysisManager.Reset();
            Context.ObserveBoardUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}