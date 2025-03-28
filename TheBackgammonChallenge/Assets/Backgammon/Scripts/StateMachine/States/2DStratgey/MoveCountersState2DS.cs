using UnityEngine;

namespace Backgammon
{
    public class MoveCountersState2DS : GameState2DStrategy
    {
        public MoveCountersState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: MOVE COUNTERS");

            _delayTimer = -1;

            ActiveState = StateKey;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            // TEST FOR A.I. DATA
            if (!Context.TurnConfig.AIDataAvailable && !Context.GameConfig.IfFastForwarding)
            {
                Context.TurnConfig.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.TurnConfig.AIDataAvailable)
                {
                    Context.TurnConfig.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                    Context.TurnConfig.AITopRankedMove = Context.AIDataHandler.AIResponse();
                }
            }

            // DELAY BETWEEN OPPONENT MOVES
            if (!Context.TurnConfig.IsPlayersTurn && !Context.GameConfig.IfFastForwarding)
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer >= 0) return;
                _delayTimer = _timeDelay;
            }

            // GET ACTIVE COUNTER
            if ((Context.TurnConfig.IsPlayersMakingMoves && Context.TurnConfig.PlayerMoveIndex < Context.TurnConfig.CountersToMoveIndex) ||
                (!Context.TurnConfig.IsPlayersMakingMoves && Context.TurnConfig.CounterMoveIndex < Context.TurnConfig.CountersToMoveIndex))
            {
                if (!Context.GameConfig.IfFastForwarding)
                    if (Context.CountersManager.TestIfActiveCounterMoving()) return;

                Counter2DPrefab counter;
                var move = Context.TurnConfig.IsPlayersMakingMoves ? Context.TurnConfig.PlayerMovesInfo[Context.TurnConfig.PlayerMoveIndex++] :
                                                          Context.TurnConfig.RecordedMovesInfo[Context.TurnConfig.CounterMoveIndex++];

                if (Context.TurnConfig.IsPlayersTurn)
                {
                    Context.TurnConfig.PointFrom = move.pointFrom == 25 ? Context.BarManager.PlayerBar :
                                        Context.PointsManager.GetPlayerPointByID(move.pointFrom);
                    Context.TurnConfig.PointTo = move.pointTo == 0 ? Context.HomeManager.PlayerHome :
                                        Context.PointsManager.GetPlayerPointByID(move.pointTo);
                }
                else
                {
                    Context.TurnConfig.PointFrom = move.pointFrom == 25 ? Context.BarManager.OpponentBar :
                                        Context.PointsManager.GetOpponentPointByID(move.pointFrom);
                    Context.TurnConfig.PointTo = move.pointTo == 0 ? Context.HomeManager.OpponentHome :
                                        Context.PointsManager.GetOpponentPointByID(move.pointTo);
                }

                counter = Context.TurnConfig.PointFrom.PopCounter();
                Context.CountersManager.SetCounterAsActive(counter);

                // IF BLOT - MOVE BLOT COUNTER
                if (move.ifBlot)
                {
                    Counter2DPrefab blot;

                    if (Context.TurnConfig.IsPlayersTurn)
                    {
                        blot = Context.TurnConfig.PointTo.PopCounter();

                        if (Context.GameConfig.IfFastForwarding)
                            blot.SetCounterToMoveToBarInstant(Context.BarManager.OpponentBar.GetCounterOffsetPosition(), counter);
                        else
                            blot.SetCounterToMoveToBar(Context.BarManager.OpponentBar.GetCounterOffsetPosition(), counter);

                        Context.BarManager.OpponentBar.PushCounter(blot);
                    }
                    else
                    {
                        blot = Context.TurnConfig.PointTo.PopCounter();

                        if (Context.GameConfig.IfFastForwarding)
                            blot.SetCounterToMoveToBarInstant(Context.BarManager.PlayerBar.GetCounterOffsetPosition(), counter);
                        else
                            blot.SetCounterToMoveToBar(Context.BarManager.PlayerBar.GetCounterOffsetPosition(), counter);

                        Context.BarManager.PlayerBar.PushCounter(blot);
                    }
                }

                // MOVE ACTIVE COUNTER
                if (move.pointTo == 25)
                {
                    if (Context.GameConfig.IfFastForwarding)
                        counter.SetCounterToMoveToBarInstant(Context.TurnConfig.PointTo.GetCounterOffsetPosition());
                    else
                        counter.SetCounterToMoveToBar(Context.TurnConfig.PointTo.GetCounterOffsetPosition());
                }
                else if (move.pointTo == 0)
                {
                    if (Context.GameConfig.IfFastForwarding)
                    {
                        counter.SetCounterToMoveToHomeInstant(Context.TurnConfig.PointTo.GetCounterOffsetPosition());
                        Context.CountersManager.SetCounterSizeAndColour(counter, false);
                    }
                    else
                    {
                        counter.SetCounterToMoveToHome(Context.TurnConfig.PointTo.GetCounterOffsetPosition());
                        Context.CountersManager.SetCounterSizeAndColour(counter, false);
                    }
                }
                else if (Context.GameConfig.IfFastForwarding)
                    counter.SetCounterToMoveToPositionInstant(Context.PointsManager.GetAdjustedPointPosition(Context.TurnConfig.PointTo));
                else
                    counter.SetCounterToMoveToPosition(Context.PointsManager.GetAdjustedPointPosition(Context.TurnConfig.PointTo),
                                                       (Context.TurnConfig.IsPlayersTurn && !Context.TurnConfig.PlayoutProMoves));

                Context.TurnConfig.PointTo.PushCounter(counter);

                // SET WHICH DICE PLAYED
                var dice1PrePlayed = Context.DiceManager.Dice1Played;
                var number = Context.DiceManager.TestWhichDicePlayed(move.pointFrom, move.pointTo);

                Context.TurnConfig.MovesAvailable -= number;

                if (Context.DiceManager.DoubleWasRolled)
                {
                    Context.DiceRollsUI.SetDoubleDicePlayed(Context.TurnConfig.IsPlayersTurn, number);
                }
                else if (Context.DiceManager.IfDiceWereAdded)
                {
                    Context.DiceRollsUI.SetDicePlayed(Context.TurnConfig.IsPlayersTurn, true);
                    Context.DiceRollsUI.SetDicePlayed(Context.TurnConfig.IsPlayersTurn, false);
                }
                else
                {
                    // HAS DICE 1 NOW BEEN PLAYED
                    var dice1Played = Context.DiceManager.Dice1Played;
                    // WAS DICE 1 PREVIOUSLY PLAYED
                    var playedDice1 = dice1PrePlayed ? false : dice1Played ? true : false;
                    // SET DICE PLAYED
                    Context.DiceRollsUI.SetDicePlayed(Context.TurnConfig.IsPlayersTurn, playedDice1);
                }
            }

            // HANDLE EXIT
            if (!Context.CountersManager.TestIfActiveCounterMoving())
            {
                if (Context.TurnConfig.CounterMoveIndex == Context.TurnConfig.CountersToMoveIndex)
                {
                    // CLEAR UP U.I.
                    Context.DiceRollsUI.SetOpponentDiceRollsText(string.Empty, 0, 0, false);
                    Context.DiceRollsUI.SetAllDicePlayed();

                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateAIRanks;

                    if (Context.GameConfig.IfFastForwarding)
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;

                    // REPLAYING PRO MOVES                    
                    if (Context.TurnConfig.PlayoutProMoves)
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ObserveBoard;
                }
                else if (Context.TurnConfig.IsPlayersTurn)
                {
                    if (Context.TurnConfig.MovesAvailable > 0)
                    {
                        // PLAYER SELECTED MOVES
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.SelectPointFrom;
                    }
                    else
                    {
                        // CLEAR UP U.I.
                        Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, 0, 0, false);
                        Context.DiceRollsUI.SetAllDicePlayed();
                        Context.TurnConfig.UndoPlayerMove = false;

                        // EVALUATE PLAYER MOVES
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateAIRanks;
                    }
                }
            }
        }

        public override void ExitState() { }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private float _delayTimer = 1.5f;
        private float _timeDelay = 1.5f;
    }
}