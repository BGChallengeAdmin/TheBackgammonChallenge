namespace Backgammon
{
    public class SelectPointToState2DS : GameState2DStrategy
    {
        public SelectPointToState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: SELECT POINT TO");

            Context.PointsManager.DeselectHomeInteractive();

            var playerColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            var opponentColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

            var dice1 = Context.DiceManager.Dice1Played ? 0 : Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
            var dice2 = Context.DiceManager.Dice2Played ? 0 : Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

            // IN CASE OF DOUBLES - SINGLE AVAILABLE MOVE
            if (Context.DiceManager.DoubleWasRolled) dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;

            if (Context.TurnConfig.IfBearingOff)
            {
                Context.PointsManager.SetBearingOffPointsToInteractive(playerColour, opponentColour, dice1, dice2,
                                                                   Context.DiceManager.DiceAvailable);
            }
            else
            {
                Context.PointsManager.SetPointsToInteractive(playerColour, opponentColour, dice1, dice2,
                                                                Context.DiceManager.DiceAvailable);
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

            if (!Context.PointsManager.IfPointFromSelected)
            {
                // IF CANCEL MOVE
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.SelectPointFrom;
            }
            else if (Context.PointsManager.IfChangingSelectedPointFrom)
            {
                _bailOutAndReEnterState = true;
                goto bailOut;
            }
            else if (Context.PointsManager.IfPointToSelected)
            {
                //IF SELECT POINT
                var pointFrom = Context.PointsManager.SelectedPointFrom;
                var pointTo = Context.PointsManager.SelectedPointTo;
                var opponentColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

                Context.TurnConfig.PlayerMovesInfo[Context.TurnConfig.PlayerMoveIndex].pointFrom = pointFrom;
                Context.TurnConfig.PlayerMovesInfo[Context.TurnConfig.PlayerMoveIndex].pointTo = pointTo;
                if (pointTo > 0) 
                    Context.TurnConfig.PlayerMovesInfo[Context.TurnConfig.PlayerMoveIndex].ifBlot = 
                        Context.PointsManager.IfBlot(pointTo, opponentColour);
                
                Context.TurnConfig.CountersToMoveIndex += 1;

                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.MoveCounters;
            }

            bailOut:;

            if (_bailOutAndReEnterState)
            {
                _bailOutAndReEnterState = false;
                EnterState();
            }
        }

        public override void ExitState()
        {
            if (Context.TurnConfig.IfCounterOnBar)
            {
                Context.BarManager.SetPlayerCounterAsSelected(false);
            }

            Context.PointsManager.DeselectHomeInteractive();
            Context.PointsManager.DeselectAllPointsInteractive();
            Context.PointsManager.ResetPointInteractions();

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private bool _bailOutAndReEnterState = false;
    }
}