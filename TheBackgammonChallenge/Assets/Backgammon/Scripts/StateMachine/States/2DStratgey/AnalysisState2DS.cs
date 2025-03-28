namespace Backgammon
{
    public class AnalysisState2DS : GameState2DStrategy
    {
        public AnalysisState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: ANALYSIS");

            Context.AnalysisUI.SetAnalysisText(Context.TurnConfig.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH");
            Context.AnalysisUI.SetActive(true);

            Context.AnalysisUI.SetPlayerRankText("#" + Context.TurnConfig.PlayerMatchedRankThisTurn.ToString());
            Context.AnalysisUI.SetProRankText("#" + Context.TurnConfig.ProMatchedRankThisTurn.ToString());

            // FOR A.I. VERSION
            var rank = Context.TurnConfig.PlayerMatchedRankThisTurn > 0 ? Context.TurnConfig.PlayerMatchedRankThisTurn : 5;
            var percent = .50f;
            percent = Context.TurnConfig.AIRankedMoves[rank - 1].probabilities.redWin;
            Context.AnalysisUI.SetAIRankText(("#" + Context.TurnConfig.PlayerMatchedRankThisTurn.ToString()), (percent * 100f));

            Context.TurnConfig.ClickedPlayerMoveAnalysis = false;
            Context.TurnConfig.ClickedProMoveAnalysis = false;
            Context.TurnConfig.ClickedOpponentMoveAnalysis = false;
            Context.TurnConfig.ClickedTopRankedMoveAnalysis = false;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            // ANALYSIS BUTTON INTERACTIONS
            if (!Context.AnalysisUI.IfClicked) return;

            Context.AnalysisUI.SetActive(false);

            // DEFAULT TO OBSERVE
            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ObserveBoard;

            if (Context.AnalysisUI.ClickedPlayerMove)
            {
                Context.TurnConfig.ClickedPlayerMoveAnalysis = true;
                Context.TurnConfig.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedProMove)
            {
                Context.TurnConfig.ClickedProMoveAnalysis = true;
                Context.TurnConfig.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedOpponentMove)
            {
                Context.TurnConfig.ClickedOpponentMoveAnalysis = true;
                Context.TurnConfig.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedTopRankedMove)
            {
                Context.TurnConfig.ClickedTopRankedMoveAnalysis = true;
                Context.TurnConfig.PlayerHasClickedAnalyse = true;
            }
            else if (Context.AnalysisUI.ClickedContinue)
            {
                Context.TurnConfig.PlayoutProMoves = true;
                Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayoutProMoves;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
            }
        }

        public override void ExitState()
        {
            Context.AnalysisUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}