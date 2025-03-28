namespace Backgammon
{
    public class RollForGoesFirstState2DS : GameState2DStrategy
    {
        public RollForGoesFirstState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: ROLL FOR GOES FIRST");

            Context.RollForGoesFirstUI.SetActive(true);

            var playerGoesFirst = (Context.TurnConfig.IfPlayer1GoesFirst && Context.GameConfig.IfPlayingAsPlayer1) ||
                            (!Context.TurnConfig.IfPlayer1GoesFirst && !Context.GameConfig.IfPlayingAsPlayer1);

            var dice = Context.GameConfig.SelectedGame.GetPlayerMove((Context.TurnConfig.IfPlayer1GoesFirst ? 0 : 1)).Split(":")[0];
            int playerDice = 0, opponentDice = 0;

            Context.Strategy.RollForGoesFirstEnter(Context, playerGoesFirst, ref playerDice, ref opponentDice);
            Context.RollForGoesFirstUI.RollOffForGoesFirst(playerGoesFirst, playerDice, opponentDice);
        }

        public override void UpdateState()
        {
            if (Context.RollForGoesFirstUI.Animating) return;

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.BeforeCommence;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}