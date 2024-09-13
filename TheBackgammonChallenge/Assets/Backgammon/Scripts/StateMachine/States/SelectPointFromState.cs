namespace Backgammon
{
    public class SelectPointFromState : GameState
    {
        public SelectPointFromState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState()
        {
            // CLEAR ALL POINTS - TEST FOR GAME PHASE
            Context.PointsManager.DeselectHomeInteractive();
            Context.PointsManager.DeselectAllPointsInteractive();
            Context.PointsManager.ResetPointInteractions();

            var playerColour = Context.IfPlayerIsBlack ? Game.PlayerColour.BLACK : Game.PlayerColour.WHITE;
            var opponentColour = Context.IfPlayerIsBlack ? Game.PlayerColour.WHITE : Game.PlayerColour.BLACK;

            Context.IfCounterOnBar = Context.BarManager.PlayerBar.Counters > 0 ? true : false;
            Context.IfBearingOff = Context.PointsManager.TestIfBearingOff(playerColour);

            var dice1 = Context.DiceManager.Dice1Played ? 0 : Context.Dice1;
            var dice2 = Context.DiceManager.Dice2Played ? 0 : Context.Dice2;

            // IN CASE OF DOUBLES && SINGLE AVAILABLE MOVE
            if (Context.DiceManager.DoubleWasRolled) dice1 = Context.Dice1;

            if (Context.IfCounterOnBar) { Context.PointsManager.PlayerCounterOnBar(); }
            else if (Context.IfBearingOff)
            {
                Context.PointsManager.SetBearingOffPointsFromInteractive(playerColour, opponentColour, dice1, dice2,
                                                                            Context.Dice1, Context.Dice2);
            }
            else
            {
                if (!Context.PointsManager.SetPointsFromInteractive(playerColour, opponentColour, dice1, dice2,
                                                                Context.DiceManager.DiceAvailable))
                {
                    // ERROR - SOMETHING HAS GONE WRONG IN SELECT FROM - BAIL OUT
                    ActiveState = GameStateMachine.EGameState.EvaluatePlayer;
                }
            }
        }

        public override void UpdateState() 
        {
            if (!Context.PointsManager.IfPointFromSelected) return;

            ActiveState = GameStateMachine.EGameState.SelectPointTo;
        }

        public override void ExitState()
        {
            Context.PointsManager.DeselectHomeInteractive();

            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}
