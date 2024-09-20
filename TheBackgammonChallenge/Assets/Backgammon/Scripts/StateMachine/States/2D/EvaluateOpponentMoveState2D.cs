using static Backgammon.GameStateContext2D;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backgammon
{
    public class EvaluateOpponentMoveState2D : GameState2D
    {
        public EvaluateOpponentMoveState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: EVALUATE OPPONENT");

            // DEFAULT TO END TURN
            ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;

            // RESET AND COMPARE BOARD STATES
            Context.OpponentBoardState.SetStateFromState(Context.TurnBeginBoardState);

            foreach (GameStateContext2D.MoveInfo move in Context.OpponentMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.OpponentBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot, false);
            }

            Context.OpponentMatchedRankThisTurn = 0;

            if (Context.AIDataAvailable)
            {
                EvaluateOpponentTurn();
            }

            if (Context.OpponentTurnWasAnalysed)
            {
                Context.OpponentScoreThisGame += Context.OpponentMatchedRankThisTurn != 0 ?
                                             Mathf.Clamp((4 - Context.OpponentMatchedRankThisTurn), 0, 3) : 0;

                if (Context.OpponentMatchedRankThisTurn == 1) Context.OpponentTopRankedThisGame += 1;
                else if (Context.OpponentMatchedRankThisTurn == 2) Context.OpponentSecondRankedThisGame += 1;

                Context.OpponentMoveEvaluated = false;
                ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        private void EvaluateOpponentTurn()
        {
            Context.OpponentMatchedRankThisTurn = Context.OpponentBoardState.CompareBoardState(Context.Rank1BoardState) ? 1 :
                                                  Context.OpponentBoardState.CompareBoardState(Context.Rank2BoardState) ? 2 :
                                                  Context.OpponentBoardState.CompareBoardState(Context.Rank3BoardState) ? 3 :
                                                  Context.OpponentBoardState.CompareBoardState(Context.Rank4BoardState) ? 4 :
                                                  Context.OpponentBoardState.CompareBoardState(Context.Rank5BoardState) ? 5 :
                                                  0;

            Context.OpponentTurnWasAnalysed = true;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}