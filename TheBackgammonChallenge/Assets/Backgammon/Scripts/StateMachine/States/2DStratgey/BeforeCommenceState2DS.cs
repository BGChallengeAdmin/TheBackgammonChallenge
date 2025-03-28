namespace Backgammon
{
    public class BeforeCommenceState2DS : GameState2DStrategy
    {
        public BeforeCommenceState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: BEFORE COMMENCE");

            Context.BeforeCommenceUI.SetActive(true);
            Context.BeforeCommenceUI.SetCommenceText(Context.Strategy.BeforeCommenceEnter(Context));
        }

        public override void UpdateState()
        {
            if (!Context.BeforeCommenceUI.CommenceClicked) return;

            Context.RollForGoesFirstUI.SetActive(false);
            Context.BeforeCommenceUI.SetActive(false);

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnConfigure;

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
    }
}