namespace Backgammon
{
    public class AnalysisState : GameState
    {
        public AnalysisState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            Context.AnalysisUI.SetAnalysisText(Context.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH");
            Context.AnalysisUI.SetActive(true);

            Context.AnalysisUI.SetPlayerRankText("#" + Context.PlayerMatchedRankThisTurn.ToString());
            Context.AnalysisUI.SetProRankText("#" + Context.ProMatchedRankThisTurn.ToString());

            Context.ClickedPlayerMoveAnalysis = false;
            Context.ClickedProMoveAnalysis = false;
            Context.ClickedTopRankedMoveAnalysis = false;
        }

        public override void UpdateState()
        {
            // ANALYSIS BUTTON INTERACTIONS
            if (!Context.AnalysisUI.IfClicked) return;
            
            Context.AnalysisUI.SetActive(false);

            if (Context.AnalysisUI.ClickedPlayerMove)
            {
                Context.ClickedPlayerMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;

                ActiveState = GameStateMachine.EGameState.RestoreBoard;
            }
            else if (Context.AnalysisUI.ClickedProMove)
            {
                Context.ClickedProMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;

                ActiveState = GameStateMachine.EGameState.RestoreBoard;
            }
            else if (Context.AnalysisUI.ClickedTopRankedMove)
            {
                Context.ClickedTopRankedMoveAnalysis = true;
                Context.PlayerHasClickedAnalyse = true;

                ActiveState = GameStateMachine.EGameState.RestoreBoard;
            }
            else if (Context.AnalysisUI.ClickedContinue) 
            {
                if (Context.PlayerMatchedProMove && !Context.PlayerHasClickedAnalyse)
                    ActiveState = GameStateMachine.EGameState.TurnEnd;
                else ActiveState = GameStateMachine.EGameState.RestoreBoard;
            }
        }

        public override void ExitState()
        {
            Context.AnalysisUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}