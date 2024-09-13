namespace Backgammon
{
    public class AnalysisState2D : GameState2D
    {
        public AnalysisState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: ANALYSIS");

            Context.AnalysisUI.SetAnalysisText(Context.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH");
            Context.AnalysisUI.SetActive(true);

            Context.AnalysisUI.SetPlayerRankText("#" + Context.PlayerMatchedRankThisTurn.ToString());
            Context.AnalysisUI.SetProRankText("#" + Context.ProMatchedRankThisTurn.ToString());

            // FOR A.I. VERSION
            var rank = Context.PlayerMatchedRankThisTurn > 0 ? Context.PlayerMatchedRankThisTurn : 5;
            var percent = .50f;
            percent = Context.AIRankedMoves[rank - 1].probabilities.redWin;
            Context.AnalysisUI.SetAIRankText(("#" + Context.PlayerMatchedRankThisTurn.ToString()), (percent * 100f));

            Context.ClickedPlayerMoveAnalysis = false;
            Context.ClickedProMoveAnalysis = false;
            Context.ClickedOpponentMoveAnalysis = false;
            Context.ClickedTopRankedMoveAnalysis = false;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            // ANALYSIS BUTTON INTERACTIONS
            if (!Context.AnalysisUI.IfClicked) return;

            Context.AnalysisUI.SetActive(false);

            // DEFAULT TO OBSERVE
            ActiveState = GameStateMachine2D.EGameState2D.ObserveBoard;

            if (Context.AnalysisUI.ClickedPlayerMove)
            {
                Context.ClickedPlayerMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedProMove)
            {
                Context.ClickedProMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedOpponentMove)
            {
                Context.ClickedOpponentMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedTopRankedMove)
            {
                Context.ClickedTopRankedMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedContinue)
            {
                Context.PlayoutProMoves = true;
                ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
            }
        }

        public override void ExitState()
        {
            Context.AnalysisUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}