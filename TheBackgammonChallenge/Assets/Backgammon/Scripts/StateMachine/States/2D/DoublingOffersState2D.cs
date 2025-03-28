namespace Backgammon
{
    public class DoublingOffersState2D : GameState2D
    {
        public DoublingOffersState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING OFFERS");

            Context.DoublingUI.SetActive(true);

            var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
            var opponent = Context.IfPlayer1Turn ? Context.SelectedMatch.Player2 : Context.SelectedMatch.Player1;

            if (Main.Instance.IfPlayerVsAI)
            {
                player = Context.SelectedMatch.Player2;
                opponent = Context.SelectedMatch.Player1;
            }

            Context.DoublingUI.SetOffersDoubleText(player, opponent);
            Context.AIPlayerAcceptsDoubles = false;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            if (Context.IfFastForwarding)
            {
                Context.DoublingUI.SetActive(false);
                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
            }

            // DOUBLING DIALOG
            if (!Context.DoublingUI.IfClicked) return;

            Context.DoublingUI.SetOfferedDoublesText(Context.AIDoublingData, Context.PlayerOfferedDoubles);

            if (Main.Instance.IfPlayerVsAI && Context.DoublingUI.IfClickedYes)
                Context.AIPlayerAcceptsDoubles = true;

            if (!Context.DoublingUI.IfClickedConfirm) return;

            Context.DoublingUI.SetActive(false);

            ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
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