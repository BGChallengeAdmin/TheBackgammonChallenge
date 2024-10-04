namespace Backgammon
{
    public class SelectPointToState2D : GameState2D
    {
        public SelectPointToState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: SELECT POINT TO");

            Context.PointsManager.DeselectHomeInteractive();

            var playerColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            var opponentColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

            var dice1 = Context.DiceManager.Dice1Played ? 0 : Context.Dice1;
            var dice2 = Context.DiceManager.Dice2Played ? 0 : Context.Dice2;

            // IN CASE OF DOUBLES - SINGLE AVAILABLE MOVE
            if (Context.DiceManager.DoubleWasRolled) dice1 = Context.Dice1;

            if (Context.IfBearingOff)
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
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            // IF UNDO PLAYER MOVE
            if (Context.UndoPlayerMove)
                ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;

            if (!Context.PointsManager.IfPointFromSelected)
            {
                // IF CANCEL MOVE
                ActiveState = GameStateMachine2D.EGameState2D.SelectPointFrom;
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
                var opponentColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

                Context.PlayerMovesInfo[Context.PlayerMoveIndex].pointFrom = pointFrom;
                Context.PlayerMovesInfo[Context.PlayerMoveIndex].pointTo = pointTo;
                if (pointTo > 0) Context.PlayerMovesInfo[Context.PlayerMoveIndex].ifBlot = Context.PointsManager.IfBlot(pointTo, opponentColour);
                Context.CountersToMoveIndex += 1;

                ActiveState = GameStateMachine2D.EGameState2D.MoveCounters;
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
            if (Context.IfCounterOnBar)
            {
                Context.BarManager.SetPlayerCounterAsSelected(false);
            }

            Context.PointsManager.DeselectHomeInteractive();
            Context.PointsManager.DeselectAllPointsInteractive();
            Context.PointsManager.ResetPointInteractions();

            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private bool _bailOutAndReEnterState = false;
    }
}