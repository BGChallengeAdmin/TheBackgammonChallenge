namespace Backgammon
{
    public class DoublingOffersState2DS : GameState2DStrategy
    {
        public DoublingOffersState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING OFFERS");

            Context.DoublingUI.SetActive(true);

            Context.Strategy.DoublingOffersEnter(Context);

            Context.TurnConfig.AIPlayerAcceptsDoubles = false;
        }

        public override void UpdateState()
        {
            if (Context.GameConfig.IfFastForwarding)
            {
                Context.DoublingUI.SetActive(false);
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
            }

            // DOUBLING DIALOG
            if (!Context.DoublingUI.IfClicked) return;

            Context.DoublingUI.SetOfferedDoublesText(Context.TurnConfig.AIDoublingData, Context.TurnConfig.PlayerOfferedDoubles);

            Context.Strategy.DoublingOffersUpdate(Context);

            if (!Context.DoublingUI.IfClickedConfirm) return;

            Context.DoublingUI.SetActive(false);

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;

            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private float _delayTimer = 2.0f;
        private float _timeDelay = 2.0f;
    }
}