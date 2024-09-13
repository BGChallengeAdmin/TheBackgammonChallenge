namespace Backgammon
{
    public class ObserveBoardState : GameState
    {
        public ObserveBoardState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            Context.ObserveBoardUI.SetActive(true);

            if (Context.ClickedPlayerMoveAnalysis)
            {
                Context.ObserveAnalysisManager.SetMarkersToPoints(Context.PlayerMovesInfo);
            }
            else if (Context.ClickedProMoveAnalysis)
            {
                Context.ObserveAnalysisManager.SetMarkersToPoints(Context.RecordedMovesInfo);
            }
            else if (Context.ClickedTopRankedMoveAnalysis)
            {
                Context.ObserveAnalysisManager.SetMarkersToPoints(Context.TopRankedMovesInfo);
            }
        }

        public override void UpdateState() 
        {
            if (!Context.ObserveBoardUI.ClickedContinue) return;

            Context.ObserveAnalysisManager.Reset();
            Context.ObserveBoardUI.SetActive(false);

            ActiveState = GameStateMachine.EGameState.Analysis;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}