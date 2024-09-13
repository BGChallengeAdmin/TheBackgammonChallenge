namespace Backgammon
{
    public class BeforeCommenceState2D : GameState2D
    {
        public BeforeCommenceState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: BEFORE COMMENCE");
         
            var text = (Context.IfPlayer1GoesFirst ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2) + " goes first.";

            if (Main.Instance.IfPlayerVsAI)
                text = Context.IfPlayer1GoesFirst ? (Context.SelectedMatch.Player1 + " go first.") : 
                                                    (Context.SelectedMatch.Player2 + " goes first.");

            Context.BeforeCommenceUI.SetActive(true);
            Context.BeforeCommenceUI.SetCommenceText(text);
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            if (!Context.BeforeCommenceUI.CommenceClicked) return;

            Context.RollForGoesFirstUI.SetActive(false);
            Context.BeforeCommenceUI.SetActive(false);
            
            ActiveState = GameStateMachine2D.EGameState2D.TurnConfigure;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private float _delayTimer = 2.0f;
        private float _timeDelay = 2.0f;
    }
}