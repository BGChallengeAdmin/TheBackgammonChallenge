namespace Backgammon
{
    public class DoublingInGameState2DS : GameState2DStrategy
    {
        public DoublingInGameState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING IN GAME");

            Context.DoublingUI.SetActive(true);
            Context.DoublingUI.IfOfferedDoubles();

            Context.TurnConfig.AIPlayerAcceptsDoubles = false;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            if (!Context.DoublingUI.IfClicked) return;

            Context.DoublingUI.SetOfferedDoublesText(Context.TurnConfig.AIDoublingData, Context.TurnConfig.PlayerOfferedDoubles);

            if (!Context.DoublingUI.IfClickedConfirm) return;

            ActiveState = Context.Strategy.DoublingInGameState(Context);
        }

        public override void ExitState()
        {
            Context.DoublingUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}