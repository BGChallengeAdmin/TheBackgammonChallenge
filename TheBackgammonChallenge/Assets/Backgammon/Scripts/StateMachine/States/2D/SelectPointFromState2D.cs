namespace Backgammon
{
    public class SelectPointFromState2D : GameState2D
    {
        public SelectPointFromState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: SELECT POINT FROM");

            // CLEAR ALL POINTS - TEST FOR GAME PHASE
            Context.PointsManager.DeselectHomeInteractive();
            Context.PointsManager.DeselectAllPointsInteractive();
            Context.PointsManager.ResetPointInteractions();

            var playerColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            var opponentColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

            Context.IfCounterOnBar = Context.BarManager.PlayerBar.Counters > 0 ? true : false;
            Context.IfBearingOff = Context.PointsManager.TestIfBearingOff(playerColour);

            var dice1 = Context.DiceManager.Dice1Played ? 0 : Context.Dice1;
            var dice2 = Context.DiceManager.Dice2Played ? 0 : Context.Dice2;

            // IN CASE OF DOUBLES && SINGLE AVAILABLE MOVE
            if (Context.DiceManager.DoubleWasRolled) dice1 = Context.Dice1;

            if (Context.IfCounterOnBar)
            {
                var valid = Main.Instance.IfPlayerVsAI ? false : true;

                // A.I. GAME - SAFEGUARD AGAINST MULTIPLE COUNTERS ON BAR
                // ENSURE THERE IS A VALID MOVE
                if (Main.Instance.IfPlayerVsAI)
                {
                    var step1 = 25 - Context.Dice1;
                    var step2 = 25 - Context.Dice2;

                    var valid1 = Context.DiceManager.Dice1Played ? false : Context.PointsManager.TestIfCanMoveToPoint(step1, opponentColour);
                    var valid2 = Context.DiceManager.Dice2Played ? false : Context.PointsManager.TestIfCanMoveToPoint(step2, opponentColour);

                    valid = valid1 || valid2;
                }

                if (valid)
                {
                    Context.PointsManager.PlayerCounterOnBar();
                    Context.BarManager.SetPlayerCounterAsSelected(true);
                }
                else ActiveState = GameStateMachine2D.EGameState2D.EvaluateAIRanks;
            }
            else if (Context.IfBearingOff)
            {
                var valid = Main.Instance.IfPlayerVsAI ? false : true;

                // A.I. GAME - SAFEGUARD AGAINST SINGLE COUNTER REMAINING
                // ENSURE THERE IS A VALID MOVE
                if (Main.Instance.IfPlayerVsAI)
                {
                    if (Context.HomeManager.PlayerHome.Counters != 15) valid = true;
                }

                if (valid)
                {
                    Context.PointsManager.SetBearingOffPointsFromInteractive(playerColour, opponentColour, dice1, dice2,
                                                                                Context.Dice1, Context.Dice2);
                }
                else ActiveState = GameStateMachine2D.EGameState2D.EvaluateAIRanks;
            }
            else
            {
                var valid = Main.Instance.IfPlayerVsAI ? false : true;

                // A.I. GAME - SAFEGUARD AGAINST INVALID MULTIPLE MOVES
                if (Main.Instance.IfPlayerVsAI)
                {
                    for (int p = 1; p < 25; p++)
                    {
                        var point = Context.PointsManager.GetPlayerPointByID(p);
                        if (point.Owner != Game2D.PlayingAs.PLAYER_1 || point.Counters == 0) continue;

                        var step1 = p - Context.Dice1;
                        var step2 = p - Context.Dice2;

                        var valid1 = !Context.DiceManager.DoubleWasRolled && Context.DiceManager.Dice1Played ?
                                        false : Context.PointsManager.TestIfCanMoveToPoint(step1, opponentColour);
                        var valid2 = Context.DiceManager.Dice2Played ? false : Context.PointsManager.TestIfCanMoveToPoint(step2, opponentColour);

                        valid = valid1 || valid2;

                        if (valid) break;
                    }
                }

                if (valid)
                {
                    if (!Context.PointsManager.SetPointsFromInteractive(playerColour, opponentColour, dice1, dice2,
                                                                Context.DiceManager.DiceAvailable))
                    {
                        // ERROR - SOMETHING HAS GONE WRONG IN SELECT FROM - BAIL OUT
                        ActiveState = GameStateMachine2D.EGameState2D.EvaluateAIRanks;
                    }
                }
                else ActiveState = GameStateMachine2D.EGameState2D.EvaluateAIRanks;
            }
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            // TEST FOR CONCEDES
            if (Context.IsPlayersTurn && Context.ConcedeTheGame)
            {
                Context.PlayerConcedes = true;
                Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerConcedes;
                ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                Context.Debug_debugObject.DebugMessage($"PLAYER CONCEDES");
            }

            // IF UNDO PLAYER MOVE
            if (Context.UndoPlayerMove)
                ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;

            if (!Context.PointsManager.IfPointFromSelected) return;

            ActiveState = GameStateMachine2D.EGameState2D.SelectPointTo;
        }

        public override void ExitState()
        {
            Context.PointsManager.DeselectHomeInteractive();

            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}
