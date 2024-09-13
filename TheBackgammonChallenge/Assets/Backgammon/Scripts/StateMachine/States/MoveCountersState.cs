using UnityEngine;

namespace Backgammon
{
    public class MoveCountersState : GameState
    {
        public MoveCountersState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

        public override void EnterState() 
        {
            ActiveState = StateKey;
        }

        public override void UpdateState()
        {
            // TEST FOR A.I. DATA
            if (!Context.AIDataAvailable)
            {
                Context.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.AIDataAvailable)
                {
                    Context.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                    Context.AITopRankedMove = Context.AIDataHandler.AIResponse();
                }
            }

            // HANDLE COUNTER MOVEMENT
            if ((Context.IsPlayersMakingMoves && Context.PlayerMoveIndex < Context.CountersToMoveIndex) ||
                (!Context.IsPlayersMakingMoves && Context.CounterMoveIndex < Context.CountersToMoveIndex))
            {
                // DEBUG - COMMENT LINE -> ALL COUNTERS MOVE ON TURN
                if (!Context.IfFastForwarding)
                    if (Context.CountersManager.TestIfActiveCounterMoving()) return;

                Counter counter;
                var move = Context.IsPlayersMakingMoves ? Context.PlayerMovesInfo[Context.PlayerMoveIndex++] :
                                                          Context.RecordedMovesInfo[Context.CounterMoveIndex++];

                if (Context.IsPlayersTurn)
                {
                    Context.PointFrom = move.pointFrom == 25 ? Context.BarManager.PlayerBar :
                                        Context.PointsManager.GetPlayerPointByID(move.pointFrom);
                    Context.PointTo = move.pointTo == 0 ? Context.HomeManager.PlayerHome :
                                        Context.PointsManager.GetPlayerPointByID(move.pointTo);
                }
                else
                {
                    Context.PointFrom = move.pointFrom == 25 ? Context.BarManager.OpponentBar :
                                        Context.PointsManager.GetOpponentPointByID(move.pointFrom);
                    Context.PointTo = move.pointTo == 0 ? Context.HomeManager.OpponentHome :
                                        Context.PointsManager.GetOpponentPointByID(move.pointTo);
                }

                counter = Context.PointFrom.PopCounter();
                Context.CountersManager.SetCounterAsActive(counter);

                // IF BLOT
                if (move.ifBlot)
                {
                    Counter blot;

                    if (Context.IsPlayersTurn)
                    {
                        blot = Context.PointTo.PopCounter();
                        blot.SetCounterToMoveToBar(Context.BarManager.OpponentBar.GetCounterOffsetPosition(), counter);
                        Context.BarManager.OpponentBar.PushCounter(blot);
                    }
                    else
                    {
                        blot = Context.PointTo.PopCounter();
                        blot.SetCounterToMoveToBar(Context.BarManager.PlayerBar.GetCounterOffsetPosition(), counter);
                        Context.BarManager.PlayerBar.PushCounter(blot);
                    }
                }

                if (move.pointTo == 25) counter.SetCounterToMoveToBar(Context.PointTo.GetCounterOffsetPosition());
                else if (move.pointTo == 0) counter.SetCounterToMoveToHome(Context.PointTo.GetCounterOffsetPosition());
                else if (Context.IfFastForwarding) counter.SetCounterToMoveToPositionInstant(Context.PointTo.GetCounterOffsetPosition());
                else counter.SetCounterToMoveToPosition(Context.PointTo.GetCounterOffsetPosition());

                Context.PointTo.PushCounter(counter);

                //UnityEngine.Debug.Log($"{Context.CounterMoveIndex}: {move.pointFrom}/{move.pointTo} {(move.ifBlot ? "*" : "")}");

                // SET WHICH DICE PLAYED
                var dice1PrePlayed = Context.DiceManager.Dice1Played;
                var dice2PrePlayed = Context.DiceManager.Dice2Played;
                var number = Context.DiceManager.TestWhichDicePlayed(move.pointFrom, move.pointTo);

                Context.MovesAvailable -= number;

                if (Context.DiceManager.DoubleWasRolled)
                {
                    Context.DiceRollsUI.SetDoubleDicePlayed(Context.IsPlayersTurn, number);
                }
                else if (Context.DiceManager.IfDiceWereAdded)
                {
                    Context.DiceRollsUI.SetDicePlayed(Context.IsPlayersTurn, true);
                    Context.DiceRollsUI.SetDicePlayed(Context.IsPlayersTurn, false);
                }
                else
                {
                    var dice1Played = Context.DiceManager.Dice1Played;
                    var dice2Played = Context.DiceManager.Dice2Played;

                    var playedDice1 = dice1PrePlayed ? false : dice1Played ? true : false;
                    Context.DiceRollsUI.SetDicePlayed(Context.IsPlayersTurn, playedDice1);
                }
            }

            // HANDLE EXIT
            if (!Context.CountersManager.TestIfActiveCounterMoving())
            {
                Debug.Log($"MOVE END -> {Context.DiceManager.DiceAvailable}");

                if (Context.CounterMoveIndex == Context.CountersToMoveIndex)
                {
                    ActiveState = GameStateMachine.EGameState.EvaluateOpponent;

                    // REPLAYING PRO MOVES
                    if (Context.IsPlayersTurn || Context.IfFastForwarding)
                        ActiveState = GameStateMachine.EGameState.TurnEnd;
                }
                else if (Context.IsPlayersTurn)
                {
                    if (Context.MovesAvailable > 0)
                    {
                        // PLAYER SELECTED MOVES
                        ActiveState = GameStateMachine.EGameState.SelectPointFrom;
                    }
                    else
                    {
                        // EVALUATE PLAYER MOVES
                        ActiveState = GameStateMachine.EGameState.EvaluatePlayer;
                    }
                }
            }
        }

        public override void ExitState() {}

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}