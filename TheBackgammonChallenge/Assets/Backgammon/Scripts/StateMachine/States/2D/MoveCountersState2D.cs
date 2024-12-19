using UnityEngine;

namespace Backgammon
{
    public class MoveCountersState2D : GameState2D
    {
        public MoveCountersState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: MOVE COUNTERS");

            _delayTimer = -1;

            ActiveState = StateKey;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            // TEST FOR A.I. DATA
            if (!Context.AIDataAvailable && !Context.IfFastForwarding)
            {
                Context.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.AIDataAvailable)
                {
                    Context.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                    Context.AITopRankedMove = Context.AIDataHandler.AIResponse();
                }
            }

            // DELAY BETWEEN OPPONENT MOVES
            if (!Context.IsPlayersTurn && !Context.IfFastForwarding)
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer >= 0) return;
                _delayTimer = _timeDelay;
            }

            // GET ACTIVE COUNTER
            if ((Context.IsPlayersMakingMoves && Context.PlayerMoveIndex < Context.CountersToMoveIndex) ||
                (!Context.IsPlayersMakingMoves && Context.CounterMoveIndex < Context.CountersToMoveIndex))
            {
                if (!Context.IfFastForwarding)
                    if (Context.CountersManager.TestIfActiveCounterMoving()) return;

                Counter2DPrefab counter;
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

                // IF BLOT - MOVE BLOT COUNTER
                if (move.ifBlot)
                {
                    Counter2DPrefab blot;

                    if (Context.IsPlayersTurn)
                    {
                        blot = Context.PointTo.PopCounter();

                        if (Context.IfFastForwarding)
                            blot.SetCounterToMoveToBarInstant(Context.BarManager.OpponentBar.GetCounterOffsetPosition(), counter);
                        else
                            blot.SetCounterToMoveToBar(Context.BarManager.OpponentBar.GetCounterOffsetPosition(), counter);

                        Context.BarManager.OpponentBar.PushCounter(blot);
                    }
                    else
                    {
                        blot = Context.PointTo.PopCounter();
                        
                        if (Context.IfFastForwarding)
                            blot.SetCounterToMoveToBarInstant(Context.BarManager.PlayerBar.GetCounterOffsetPosition(), counter);
                        else
                            blot.SetCounterToMoveToBar(Context.BarManager.PlayerBar.GetCounterOffsetPosition(), counter);
                        
                        Context.BarManager.PlayerBar.PushCounter(blot);
                    }
                }

                // MOVE ACTIVE COUNTER
                if (move.pointTo == 25)
                {
                    if (Context.IfFastForwarding)
                        counter.SetCounterToMoveToBarInstant(Context.PointTo.GetCounterOffsetPosition());
                    else
                        counter.SetCounterToMoveToBar(Context.PointTo.GetCounterOffsetPosition());
                }
                else if (move.pointTo == 0)
                {
                    if (Context.IfFastForwarding)
                    {
                        counter.SetCounterToMoveToHomeInstant(Context.PointTo.GetCounterOffsetPosition());
                        Context.CountersManager.SetCounterSizeAndColour(counter, false);
                    }
                    else
                    {
                        counter.SetCounterToMoveToHome(Context.PointTo.GetCounterOffsetPosition());
                        Context.CountersManager.SetCounterSizeAndColour(counter, false);
                    }
                }
                else if (Context.IfFastForwarding)
                    counter.SetCounterToMoveToPositionInstant(Context.PointsManager.GetAdjustedPointPosition(Context.PointTo));
                else
                    counter.SetCounterToMoveToPosition(Context.PointsManager.GetAdjustedPointPosition(Context.PointTo), 
                                                       (Context.IsPlayersTurn && !Context.PlayoutProMoves));

                Context.PointTo.PushCounter(counter);

                // SET WHICH DICE PLAYED
                var dice1PrePlayed = Context.DiceManager.Dice1Played;
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
                    // HAS DICE 1 NOW BEEN PLAYED
                    var dice1Played = Context.DiceManager.Dice1Played;
                    // WAS DICE 1 PREVIOUSLY PLAYED
                    var playedDice1 = dice1PrePlayed ? false : dice1Played ? true : false;
                    // SET DICE PLAYED
                    Context.DiceRollsUI.SetDicePlayed(Context.IsPlayersTurn, playedDice1);
                }
            }

            // HANDLE EXIT
            if (!Context.CountersManager.TestIfActiveCounterMoving())
            {
                if (Context.CounterMoveIndex == Context.CountersToMoveIndex)
                {
                    // CLEAR UP U.I.
                    Context.DiceRollsUI.SetOpponentDiceRollsText(string.Empty, 0, 0, false);
                    Context.DiceRollsUI.SetAllDicePlayed();

                    ActiveState = GameStateMachine2D.EGameState2D.EvaluateAIRanks;

                    if (Context.IfFastForwarding)
                        ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;

                    // REPLAYING PRO MOVES                    
                    if (Context.PlayoutProMoves)
                        ActiveState = GameStateMachine2D.EGameState2D.ObserveBoard;
                }
                else if (Context.IsPlayersTurn)
                {
                    if (Context.MovesAvailable > 0)
                    {
                        // PLAYER SELECTED MOVES
                        ActiveState = GameStateMachine2D.EGameState2D.SelectPointFrom;
                    }
                    else
                    {
                        // CLEAR UP U.I.
                        Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, 0, 0, false);
                        Context.DiceRollsUI.SetAllDicePlayed();
                        Context.UndoPlayerMove = false;

                        // EVALUATE PLAYER MOVES
                        ActiveState = GameStateMachine2D.EGameState2D.EvaluateAIRanks;
                    }
                }
            }
        }

        public override void ExitState() { }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private float _delayTimer = 1.5f;
        private float _timeDelay = 1.5f;
    }
}