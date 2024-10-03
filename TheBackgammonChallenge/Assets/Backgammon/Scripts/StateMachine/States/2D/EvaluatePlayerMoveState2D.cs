using UnityEngine;

namespace Backgammon
{
    public class EvaluatePlayerMoveState2D : GameState2D
    {
        public EvaluatePlayerMoveState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: EVALUATE PLAYER");

            // RESET AND COMPARE BOARD STATES
            Context.PlayerBoardState.SetStateFromState(Context.TurnBeginBoardState);
            Context.ProBoardState.SetStateFromState(Context.TurnBeginBoardState);

            // DEFAULT TO GENERAL INFO - PLAYER WAS NOT EVALUATED
            Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluatePlayerMovesNoAIData;
            ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;

            // A.I. GAME - SET THE MOVE DATA TO THE PLAYER BEFORE ANALYSIS
            if (Main.Instance.IfPlayerVsAI && Context.IsPlayersTurn)
            {
                Context.RecordedMovesInfo = Context.PlayerMovesInfo;
            }

            foreach (GameStateContext2D.MoveInfo move in Context.PlayerMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.PlayerBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot);
            }

            foreach (GameStateContext2D.MoveInfo move in Context.RecordedMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.ProBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot);
            }

            Context.PlayerMatchedRankThisTurn = 0;
            Context.ProMatchedRankThisTurn = 0;

            // SCORE PLAYER TURN AGAINST A.I. AND PRO
            if (Context.AIDataAvailable && !Context.PlayerHasClickedAnalyse)
            {
                EvaluatePlayerTurn();
                EvaluatePlayerProTurn();

                if (Context.PlayerTurnWasAnalysed)
                {
                    Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluatePlayerMoves;

                    Context.PlayerScoreThisGame += Context.PlayerMatchedRankThisTurn != 0 ?
                                                    Mathf.Clamp((4 - Context.PlayerMatchedRankThisTurn), 0, 3) : 0;

                    if (Context.PlayerMatchedRankThisTurn == 1) Context.PlayerTopRankedThisGame += 1;
                    else if (Context.PlayerMatchedRankThisTurn == 2) Context.PlayerSecondRankedThisGame += 1;
                }

                if (Context.ProTurnWasAnalysed && !Context.ReplayPlayerMove)
                {
                    Context.ProScoreThisGame += Context.ProMatchedRankThisTurn != 0 ?
                                                 Mathf.Clamp((4 - Context.ProMatchedRankThisTurn), 0, 3) : 0;

                    if (Context.ProMatchedRankThisTurn == 1) Context.ProTopRankedThisGame += 1;
                    else if (Context.ProMatchedRankThisTurn == 2) Context.ProSecondRankedThisGame += 1;
                }
            }

            Context.PlayerMatchedProMove = Context.PlayerBoardState.CompareBoardState(Context.ProBoardState);

            if (Context.PlayerMatchedProMove) Context.TotalValidPlayerMatchesThisGame++;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        private void EvaluatePlayerTurn()
        {
            Context.PlayerMatchedRankThisTurn = Context.PlayerBoardState.CompareBoardState(Context.Rank1BoardState) ? 1 :
                                                Context.PlayerBoardState.CompareBoardState(Context.Rank2BoardState) ? 2 :
                                                Context.PlayerBoardState.CompareBoardState(Context.Rank3BoardState) ? 3 :
                                                Context.PlayerBoardState.CompareBoardState(Context.Rank4BoardState) ? 4 :
                                                Context.PlayerBoardState.CompareBoardState(Context.Rank5BoardState) ? 5 :
                                                0;

            Context.PlayerTurnWasAnalysed = true;
        }

        private void EvaluatePlayerProTurn()
        {
            Context.ProMatchedRankThisTurn = Context.ProBoardState.CompareBoardState(Context.Rank1BoardState) ? 1 :
                                             Context.ProBoardState.CompareBoardState(Context.Rank2BoardState) ? 2 :
                                             Context.ProBoardState.CompareBoardState(Context.Rank3BoardState) ? 3 :
                                             Context.ProBoardState.CompareBoardState(Context.Rank4BoardState) ? 4 :
                                             Context.ProBoardState.CompareBoardState(Context.Rank5BoardState) ? 5 :
                                             0;

            Context.ProTurnWasAnalysed = true;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}