namespace Backgammon
{
    public class BeforeCommenceState : GameState
    {
        public BeforeCommenceState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

        public override void EnterState()
        {
            var text = (Context.IfPlayer1GoesFirst ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2) + " goes first.";

            Context.BeforeCommenceUI.SetActive(true);
            Context.BeforeCommenceUI.SetCommenceText(text);
        }

        public override void UpdateState()
        {
            if (!Context.BeforeCommenceUI.CommenceClicked) return;
            
            Context.BeforeCommenceUI.SetActive(false);

            if (Context.BeforeCommenceUI.FastForwardUserInput > 0)
            {
                Context.FastForwardTurnIndex = Context.BeforeCommenceUI.FastForwardUserInput;
                ActiveState = GameStateMachine.EGameState.ConfigureBoard;
            }
            else  ActiveState = GameStateMachine.EGameState.TurnBegin;

            //_delayTimer -= Time.deltaTime;

            //if (_delayTimer <= 0)
            //{
            //    Context.BeforeCommenceUI.SetActive(false);

            //    _delayTimer = _timeDelay;

            //    ActiveState = GameStateMachine.EGameState.TurnBegin;
            //}
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