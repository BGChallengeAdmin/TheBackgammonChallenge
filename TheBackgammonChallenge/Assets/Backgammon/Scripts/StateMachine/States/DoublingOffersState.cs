namespace Backgammon
{
    public class DoublingOffersState : GameState
    {
        public DoublingOffersState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

        public override void EnterState() 
        {
            Context.DoublingUI.SetActive(true);

            var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
            var opponent = Context.IfPlayer1Turn ? Context.SelectedMatch.Player2 : Context.SelectedMatch.Player1;

            Context.DoublingUI.SetOffersDoubleText(player, opponent);
        }

        public override void UpdateState()
        {
            if (Context.IfFastForwarding)
            {
                Context.DoublingUI.SetActive(false);
                ActiveState = GameStateMachine.EGameState.TurnEnd;
            }

            // DOUBLING DIALOG
            if (!Context.DoublingUI.IfClicked) return;

            if (Context.DoublingUI.IfClickedYes || Context.DoublingUI.IfClickedNo)
                Context.DoublingUI.SetOfferedDoublesText(Context.AIDoublingData);

            if (!Context.DoublingUI.IfClickedConfirm) return;

            Context.DoublingUI.SetActive(false);

            ActiveState = GameStateMachine.EGameState.TurnEnd;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        private float _delayTimer = 2.0f;
        private float _timeDelay = 2.0f;
    }
}