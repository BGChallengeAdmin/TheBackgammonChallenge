namespace Backgammon
{
    public class SelectPointFromState2DS : GameState2DStrategy
    {
        public SelectPointFromState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: SELECT POINT FROM");

            // CLEAR ALL POINTS - TEST FOR GAME PHASE
            Context.PointsManager.DeselectHomeInteractive();
            Context.PointsManager.DeselectAllPointsInteractive();
            Context.PointsManager.ResetPointInteractions();

            var playerColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            var opponentColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

            Context.TurnConfig.IfCounterOnBar = Context.BarManager.PlayerBar.Counters > 0 ? true : false;
            Context.TurnConfig.IfBearingOff = Context.PointsManager.TestIfBearingOff(playerColour);

            var dice1_original = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
            var dice2_original = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

            var dice1_current = Context.DiceManager.Dice1Played ? 0 : dice1_original;
            var dice2_current = Context.DiceManager.Dice2Played ? 0 : dice2_original;

            // IN CASE OF DOUBLES && SINGLE AVAILABLE MOVE
            if (Context.DiceManager.DoubleWasRolled) dice1_current = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;

            if (Context.TurnConfig.IfCounterOnBar)
            {
                var valid = Context.Strategy.SelectPointFromEntry(Context, bar: true);

                if (valid)
                {
                    Context.PointsManager.PlayerCounterOnBar();
                    Context.BarManager.SetPlayerCounterAsSelected(true);
                }
                else ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateAIRanks;
            }
            else if (Context.TurnConfig.IfBearingOff)
            {
                var valid = Context.Strategy.SelectPointFromEntry(Context, bearingOff: true);

                if (valid)
                {
                    Context.PointsManager.SetBearingOffPointsFromInteractive(playerColour, opponentColour,
                                                                             dice1_current, dice2_current,
                                                                             dice1_original, dice2_original);
                }
                else ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateAIRanks;
            }
            else
            {
                var valid = Context.Strategy.SelectPointFromEntry(Context, normal: true);

                if (valid)
                {
                    if (!Context.PointsManager.SetPointsFromInteractive(playerColour, opponentColour, 
                                                                        dice1_current, dice2_current,
                                                                        Context.DiceManager.DiceAvailable))
                    {
                        // ERROR - SOMETHING HAS GONE WRONG IN SELECT FROM - BAIL OUT
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateAIRanks;
                    }
                }
                else ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateAIRanks;
            }
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            // TEST FOR CONCEDES
            if (Context.TurnConfig.IsPlayersTurn && Context.TurnConfig.ConcedeTheGame)
            {
                Context.TurnConfig.PlayerConcedes = true;
                Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerConcedes;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                Context.GameConfig.Debug_debugObject.DebugMessage($"PLAYER CONCEDES");
            }

            // IF UNDO PLAYER MOVE
            if (Context.TurnConfig.UndoPlayerMove)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RestoreBoard;

            if (!Context.PointsManager.IfPointFromSelected) return;

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.SelectPointTo;
        }

        public override void ExitState()
        {
            Context.PointsManager.DeselectHomeInteractive();

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}
