using UnityEngine;

namespace Backgammon
{
    public class EvaluateOpponentMoveState2DS : GameState2DStrategy
    {
        public EvaluateOpponentMoveState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: EVALUATE OPPONENT");

            // DEFAULT TO END TURN
            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;

            // RESET AND COMPARE BOARD STATES
            Context.TurnConfig.OpponentBoardState.SetStateFromState(Context.TurnConfig.TurnBeginBoardState);

            foreach (GameStateContext2D.MoveInfo move in Context.TurnConfig.OpponentMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.TurnConfig.OpponentBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot, false);
            }

            Context.TurnConfig.OpponentMatchedRankThisTurn = 0;

            if (Context.TurnConfig.AIDataAvailable)
            {
                EvaluateOpponentTurn();
            }

            if (Context.TurnConfig.OpponentTurnWasAnalysed)
            {
                Context.GameStats.OpponentScoreThisGame += Context.TurnConfig.OpponentMatchedRankThisTurn != 0 ?
                                             Mathf.Clamp((4 - Context.TurnConfig.OpponentMatchedRankThisTurn), 0, 3) : 0;

                if (Context.TurnConfig.OpponentMatchedRankThisTurn == 1) Context.GameStats.OpponentTopRankedThisGame += 1;
                else if (Context.TurnConfig.OpponentMatchedRankThisTurn == 2) Context.GameStats.OpponentSecondRankedThisGame += 1;

                // NOTE: OPPONENET EVAULATION IS NOT SHOWN TO PLAYER
                //Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluateOpponentMoves;
                //ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        private void EvaluateOpponentTurn()
        {
            Context.TurnConfig.OpponentMatchedRankThisTurn = Context.TurnConfig.OpponentBoardState.CompareBoardState(Context.TurnConfig.Rank1BoardState) ? 1 :
                                                  Context.TurnConfig.OpponentBoardState.CompareBoardState(Context.TurnConfig.Rank2BoardState) ? 2 :
                                                  Context.TurnConfig.OpponentBoardState.CompareBoardState(Context.TurnConfig.Rank3BoardState) ? 3 :
                                                  Context.TurnConfig.OpponentBoardState.CompareBoardState(Context.TurnConfig.Rank4BoardState) ? 4 :
                                                  Context.TurnConfig.OpponentBoardState.CompareBoardState(Context.TurnConfig.Rank5BoardState) ? 5 :
                                                  0;

            Context.TurnConfig.OpponentTurnWasAnalysed = true;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}