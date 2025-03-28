using UnityEngine;

namespace Backgammon
{
    public class EvaluatePlayerMoveState2DS : GameState2DStrategy
    {
        public EvaluatePlayerMoveState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: EVALUATE PLAYER");

            // RESET AND COMPARE BOARD STATES
            Context.TurnConfig.PlayerBoardState.SetStateFromState(Context.TurnConfig.TurnBeginBoardState);
            Context.TurnConfig.ProBoardState.SetStateFromState(Context.TurnConfig.TurnBeginBoardState);

            // DEFAULT TO GENERAL INFO - PLAYER WAS NOT EVALUATED
            Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluatePlayerMovesNoAIData;
            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;

            Context.Strategy.EvaluatePlayerMoveEntry(Context);

            foreach (GameStateContext2D.MoveInfo move in Context.TurnConfig.PlayerMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.TurnConfig.PlayerBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot);
            }

            foreach (GameStateContext2D.MoveInfo move in Context.TurnConfig.RecordedMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.TurnConfig.ProBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot);
            }

            Context.TurnConfig.PlayerMatchedRankThisTurn = 0;
            Context.TurnConfig.ProMatchedRankThisTurn = 0;

            // SCORE PLAYER TURN AGAINST A.I. AND PRO
            if (Context.TurnConfig.AIDataAvailable && !Context.TurnConfig.PlayerHasClickedAnalyse)
            {
                EvaluatePlayerTurn();
                EvaluatePlayerProTurn();

                if (Context.TurnConfig.PlayerTurnWasAnalysed)
                {
                    Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluatePlayerMoves;

                    Context.GameStats.PlayerScoreThisGame += Context.TurnConfig.PlayerMatchedRankThisTurn != 0 ?
                                                    Mathf.Clamp((4 - Context.TurnConfig.PlayerMatchedRankThisTurn), 0, 3) : 0;

                    if (Context.TurnConfig.PlayerMatchedRankThisTurn == 1) Context.GameStats.PlayerTopRankedThisGame += 1;
                    else if (Context.TurnConfig.PlayerMatchedRankThisTurn == 2) Context.GameStats.PlayerSecondRankedThisGame += 1;
                }

                if (Context.TurnConfig.ProTurnWasAnalysed && !Context.TurnConfig.ReplayPlayerMove)
                {
                    Context.GameStats.ProScoreThisGame += Context.TurnConfig.ProMatchedRankThisTurn != 0 ?
                                                 Mathf.Clamp((4 - Context.TurnConfig.ProMatchedRankThisTurn), 0, 3) : 0;

                    if (Context.TurnConfig.ProMatchedRankThisTurn == 1) Context.GameStats.ProTopRankedThisGame += 1;
                    else if (Context.TurnConfig.ProMatchedRankThisTurn == 2) Context.GameStats.ProSecondRankedThisGame += 1;
                }
            }

            Context.TurnConfig.PlayerMatchedProMove = Context.TurnConfig.PlayerBoardState.CompareBoardState(Context.TurnConfig.ProBoardState);

            if (Context.TurnConfig.PlayerMatchedProMove) Context.GameStats.TotalValidPlayerMatchesThisGame++;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        private void EvaluatePlayerTurn()
        {
            Context.TurnConfig.PlayerMatchedRankThisTurn = Context.TurnConfig.PlayerBoardState.CompareBoardState(Context.TurnConfig.Rank1BoardState) ? 1 :
                                                Context.TurnConfig.PlayerBoardState.CompareBoardState(Context.TurnConfig.Rank2BoardState) ? 2 :
                                                Context.TurnConfig.PlayerBoardState.CompareBoardState(Context.TurnConfig.Rank3BoardState) ? 3 :
                                                Context.TurnConfig.PlayerBoardState.CompareBoardState(Context.TurnConfig.Rank4BoardState) ? 4 :
                                                Context.TurnConfig.PlayerBoardState.CompareBoardState(Context.TurnConfig.Rank5BoardState) ? 5 :
                                                0;

            Context.TurnConfig.PlayerTurnWasAnalysed = true;
        }

        private void EvaluatePlayerProTurn()
        {
            Context.TurnConfig.ProMatchedRankThisTurn = Context.TurnConfig.ProBoardState.CompareBoardState(Context.TurnConfig.Rank1BoardState) ? 1 :
                                             Context.TurnConfig.ProBoardState.CompareBoardState(Context.TurnConfig.Rank2BoardState) ? 2 :
                                             Context.TurnConfig.ProBoardState.CompareBoardState(Context.TurnConfig.Rank3BoardState) ? 3 :
                                             Context.TurnConfig.ProBoardState.CompareBoardState(Context.TurnConfig.Rank4BoardState) ? 4 :
                                             Context.TurnConfig.ProBoardState.CompareBoardState(Context.TurnConfig.Rank5BoardState) ? 5 :
                                             0;

            Context.TurnConfig.ProTurnWasAnalysed = true;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}