namespace Backgammon
{
    public class SelectPointToState : GameState
    {
        public SelectPointToState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.PointsManager.DeselectHomeInteractive();
            
            var playerColour = Context.IfPlayerIsBlack ? Game.PlayerColour.BLACK : Game.PlayerColour.WHITE;
            var opponentColour = Context.IfPlayerIsBlack ? Game.PlayerColour.WHITE : Game.PlayerColour.BLACK;

            var dice1 = Context.DiceManager.Dice1Played ? 0 : Context.Dice1;
            var dice2 = Context.DiceManager.Dice2Played ? 0 : Context.Dice2;

            // IN CASE OF DOUBLES - SINGLE AVAILABLE MOVE
            if (Context.DiceManager.DoubleWasRolled) dice1 = Context.Dice1;

            if (Context.IfBearingOff)
            {
                if (Context.PointsManager.IfChangingSelectedPointFrom)
                {
                    Context.PointsManager.DeselectAllPointsInteractive();

                    Context.PointsManager.SetBearingOffPointsFromInteractive(playerColour, opponentColour, dice1, dice2,
                                                                                Context.Dice1, Context.Dice2);
                }

                Context.PointsManager.SetBearingOffPointsToInteractive(playerColour, opponentColour, dice1, dice2,
                                                                   Context.DiceManager.DiceAvailable);
            }
            else
            {
                if (Context.PointsManager.IfChangingSelectedPointFrom)
                {
                    Context.PointsManager.DeselectAllPointsInteractive();

                    if (!Context.PointsManager.SetPointsFromInteractive(playerColour, opponentColour, dice1, dice2,
                                                                    Context.DiceManager.DiceAvailable))
                    {
                        // ERROR - SOMETHING HAS GONE WRONG IN SELECT FROM - BAIL OUT
                        ActiveState = GameStateMachine.EGameState.EvaluatePlayer;
                    }
                }

                UnityEngine.Debug.Log($"** SET POINTS TO **");
                Context.PointsManager.SetPointsToInteractive(playerColour, opponentColour, dice1, dice2,
                                                                Context.DiceManager.DiceAvailable);
            }
        }

        public override void UpdateState()
        {
            if (!Context.PointsManager.IfPointFromSelected)
            {
                // IF CANCEL MOVE
                ActiveState = GameStateMachine.EGameState.SelectPointFrom;
            }
            else if (Context.PointsManager.IfChangingSelectedPointFrom)
            {
                // PLAYER SELECTED AN ALTERNATE POINT FROM
                EnterState();
            }
            else if (Context.PointsManager.IfPointToSelected)
            {
                //IF SELECT POINT
                var pointFrom = Context.PointsManager.SelectedPointFrom;
                var pointTo = Context.PointsManager.SelectedPointTo;
                var opponentColour = Context.IfPlayerIsBlack ? Game.PlayerColour.WHITE : Game.PlayerColour.BLACK;

                Context.PlayerMovesInfo[Context.PlayerMoveIndex].pointFrom = pointFrom;
                Context.PlayerMovesInfo[Context.PlayerMoveIndex].pointTo = pointTo;
                Context.PlayerMovesInfo[Context.PlayerMoveIndex].ifBlot = Context.PointsManager.IfBlot(pointTo, opponentColour);
                Context.CountersToMoveIndex += 1;

                ActiveState = GameStateMachine.EGameState.MoveCounters;
            }
        }

        public override void ExitState()
        {
            Context.PointsManager.DeselectHomeInteractive();
            Context.PointsManager.DeselectAllPointsInteractive();
            Context.PointsManager.ResetPointInteractions();

            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}