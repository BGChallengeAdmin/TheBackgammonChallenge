using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Backgammon.GameStateContext;

namespace Backgammon
{
    public class EvaluatePlayerMoveState : GameState
    {
        public EvaluatePlayerMoveState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            // COMPARE BOARD STATES
            Context.PlayerBoardState.SetStateFromState(Context.TurnBeginBoardState);
            Context.ProBoardState.SetStateFromState(Context.TurnBeginBoardState);

            foreach (GameStateContext.MoveInfo move in Context.PlayerMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.PlayerBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot);
            }

            foreach (GameStateContext.MoveInfo move in Context.RecordedMovesInfo)
            {
                if (move.pointFrom == 0) break;

                Context.ProBoardState.PopPushCounter(move.pointFrom, move.pointTo, move.ifBlot);
            }

            // SCORE PLAYER TURN AGAINST A.I. AND PRO
            if (Context.AIDataAvailable && !Context.PlayerHasClickedAnalyse)
            {
                AnalysePlayerTurn();
                AnalyseProTurn();
                ConvertAIData();

                if (Context.PlayerTurnWasAnalysed)
                {
                    Context.PlayerScoreThisGame += Context.PlayerMatchedRankThisTurn != 0 ?
                                                    Mathf.Clamp((4 - Context.PlayerMatchedRankThisTurn), 0, 3) : 0;

                    if (Context.PlayerMatchedRankThisTurn == 1) Context.PlayerTopRankedThisGame += 1;
                    else if (Context.PlayerMatchedRankThisTurn == 2) Context.PlayerSecondRankedThisGame += 1;

                    //Context.GameScreenUI.SetPlayerScore(Context.PlayerScoreThisGame);
                    //Context.GameScreenUI.SetPlayerTopMatches(Context.PlayerTopRankedThisGame);
                    //Context.GameScreenUI.SetPlayerMatchedPercentage(100 * (Context.PlayerTopRankedThisGame /
                    //                                                Context.TotalValidPlayerMovesThisGame));
                    //Context.GameScreenUI.SetAndAnimatePlayerRank("#" + Context.PlayerMatchedRankThisTurn.ToString());
                }

                if (Context.ProTurnWasAnalysed)
                {
                    Context.ProScoreThisGame += Context.ProMatchedRankThisTurn != 0 ?
                                                 Mathf.Clamp((4 - Context.ProMatchedRankThisTurn), 0, 3) : 0;

                    if (Context.ProMatchedRankThisTurn == 1) Context.ProTopRankedThisGame += 1;
                    else if (Context.ProMatchedRankThisTurn == 2) Context.ProSecondRankedThisGame += 1;

                    //Context.GameScreenUI.SetOpponentScore(Context.ProScoreThisGame);
                    //Context.GameScreenUI.SetOpponentTopMatches(Context.ProTopRankedThisGame);
                    //Context.GameScreenUI.SetOpponentMatchedPercentage(100 * (Context.ProTopRankedThisGame /
                    //                                                  Context.TotalValidPlayerMovesThisGame));
                    //Context.GameScreenUI.SetAndAnimateOpponentRank("#" + Context.ProMatchedRankThisTurn.ToString());
                }
            }

            Context.PlayerMoveEvaluated = false;
            Context.PlayerProMoveEvaluated = false;
            Context.PlayerMatchedProMove = Context.PlayerBoardState.CompareBoardState(Context.ProBoardState);

            ActiveState = GameStateMachine.EGameState.GeneralInfo;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        private void AnalysePlayerTurn()
        {
            var movesMade = Context.TotalMovesThisTurn;
            Move[] aiRankedMoves = new Move[Context.AIRankedMoves.Length];
            aiRankedMoves = Context.AIRankedMoves;

            List<MoveInfo> playerMoveInfoList = new List<MoveInfo>();
            MoveInfo[] playerMovesCopy = new MoveInfo[4];

            Context.PlayerMatchedRankThisTurn = 0;

            foreach (Move aiRankedMove in aiRankedMoves)
            {
                playerMoveInfoList.Clear();

                for (int move = 0; move < 4; move++)
                {
                    playerMoveInfoList.Add(Context.PlayerMovesInfo[move]);
                }

                playerMoveInfoList = playerMoveInfoList.OrderByDescending(move => move.pointFrom).ToList();
                playerMovesCopy = playerMoveInfoList.ToArray();

                int playerMatches = 0;
                bool intermediaryMatch = false, missedBlot = false;
                int playerMatchedFrom = 0, playerMatchedTo = 0, playerMatchCounter = 0, playerIntermediaryStep = 0;
                int playerDoubleMovesFrom = 0;

                foreach (FromTo ft in aiRankedMove.movePart)
                {
                    playerMatchCounter++;
                    int from = ft.from == "bar" ? 25 : int.Parse(ft.from);
                    int to = ft.to == "off" ? 0 : int.Parse(ft.to);

                    for (int j = 0; j < 4; j++)
                    {
                        MoveInfo playerMove = playerMovesCopy[j];

                        if (playerMove.pointFrom == 0)
                            continue;
                        else
                        {
                            // EXACT MATCH
                            if (playerMove.pointFrom == from && playerMove.pointTo == to)
                            {
                                playerMatches++;
                                playerMatchedFrom++;
                                playerMatchedTo++;

                                playerMovesCopy[j].pointFrom = 0;
                                goto NextAIMove;
                            }
                            // POINT FROM
                            else if (playerMove.pointFrom == from)
                            {
                                playerMatchedFrom++;
                                playerIntermediaryStep = playerMove.pointTo;

                                foreach (MoveInfo rankedMove in Context.RecordedMovesInfo)
                                {
                                    if (rankedMove.pointFrom == from && rankedMove.ifBlot && !playerMove.ifBlot)
                                    {
                                        missedBlot = true;
                                    }
                                }

                                // MULTIPLE DOUBLE MOVES
                                if (Context.DiceManager.DoubleWasRolled)
                                {
                                    var number = Context.DiceManager.NumberOfDoublesUsed;
                                    if (number > 1)
                                    {
                                        // HAVE ALREADY PLAYED FROM THIS POSITION
                                        if (number == 2 && from == playerDoubleMovesFrom)
                                        {
                                            playerMatchedFrom = 3;
                                            playerMatchedTo = 1;
                                        }

                                        playerDoubleMovesFrom = from;
                                    }
                                }
                            }
                            // ALTERNATE MOVE
                            else if (!Context.DiceManager.IfDiceWereAdded && !missedBlot &&
                                        playerMove.pointFrom == playerIntermediaryStep && playerMove.pointTo == to)
                            {
                                playerMatchedFrom++;
                                playerMatchedTo += 2;
                                playerIntermediaryStep = 0;
                                intermediaryMatch = true;

                                playerMovesCopy[j].pointFrom = 0;
                            }
                            // POINT TO
                            else if (playerMove.pointTo == to)
                            {
                                playerMatchedTo++;

                                // MULTIPLE DOUBLE MOVES
                                if (playerDoubleMovesFrom > 0)
                                {
                                    var number = Context.DiceManager.NumberOfDoublesUsed;

                                    if (playerDoubleMovesFrom - (number * Context.Dice1) == to)
                                    {
                                        playerMatchedFrom += number - 1;
                                        playerMatchedTo += number - 1;
                                        playerDoubleMovesFrom = 0;
                                    }
                                }

                                playerMovesCopy[j].pointFrom = 0;
                            }
                        }
                    }

                    NextAIMove:;
                }

                // EXACT MATCH
                if (playerMatches == movesMade)
                {
                    Context.PlayerMatchedRankThisTurn = aiRankedMove.rank;
                    goto FinishEval;
                }
                // DOUBLE MOVE
                else if (Context.DiceManager.IfDiceWereAdded && !missedBlot && (playerMatchedFrom + playerMatchedTo) == playerMatchCounter)
                {
                    Context.PlayerMatchedRankThisTurn = aiRankedMove.rank;
                    goto FinishEval;
                }
                //// DOUBLE ROLLED
                //else if (Context.DiceManager.DoubleWasRolled && (playerMatchedFrom + playerMatchedTo) == playerMatchCounter)
                //{
                //    Debug.Log($"DOUBLE ROLLED");
                //    Context.PlayerMatchedRankThisTurn = aiRankedMove.rank;
                //    goto FinishEval;
                //}
                // SAME OUT-COME DIFFERENT ORDER
                else if (intermediaryMatch)
                {
                    Context.PlayerMatchedRankThisTurn = aiRankedMove.rank;
                    goto FinishEval;
                }
                // MULTIPLE DOUBLE MOVES
                else if (Context.DiceManager.IfDiceWereAddedInDouble && (playerMatchedFrom == playerMatchedTo) &&
                            (playerMatchedFrom == playerMatchCounter))
                {
                    Context.PlayerMatchedRankThisTurn = aiRankedMove.rank;
                    goto FinishEval;
                }
            }

            FinishEval:;

            Context.PlayerTurnWasAnalysed = true;
        }

        private void AnalyseProTurn()
        {
            var movesMade = Context.TotalMovesThisTurn;
            Move[] aiRankedMoves = new Move[Context.AIRankedMoves.Length];
            aiRankedMoves = Context.AIRankedMoves;

            List<MoveInfo> proMoveInfoList = new List<MoveInfo>();
            MoveInfo[] proMovesCopy = new MoveInfo[4];

            if (Main.Instance.IfPlayerVsAI) goto BailOut;
            else
            {
                Context.ProMatchedRankThisTurn = 0;
            }

            foreach (Move rankedMove in aiRankedMoves)
            {
                proMoveInfoList.Clear();

                for (int move = 0; move < 4; move++)
                {
                    proMoveInfoList.Add(Context.RecordedMovesInfo[move]);
                }

                proMoveInfoList = proMoveInfoList.OrderByDescending(moveFrom => moveFrom.pointFrom).ToList();
                proMovesCopy = proMoveInfoList.ToArray();

                int proMatches = 0;
                bool proIntermediaryMatch = false;
                int proMatchedFrom = 0, proMatchedTo = 0, proMatchCounter = 0, proIntermediaryStep = 0;

                foreach (FromTo ft in rankedMove.movePart)
                {
                    int from = ft.from == "bar" ? 25 : int.Parse(ft.from);
                    int to = ft.to == "off" ? 0 : int.Parse(ft.to);
                    proMatchCounter++;

                    for (int i = 0; i < 4; i++)
                    {
                        MoveInfo proMove = proMovesCopy[i];

                        if (proMove.pointFrom == 0)
                            continue;
                        else
                        {
                            if (proMove.pointFrom == from && proMove.pointTo == to)
                            {
                                proMovesCopy[i].pointFrom = 0;
                                proMatches++;
                                goto NextProMove;
                            }
                            else if (proMove.pointFrom == from)
                            {
                                proMatchedFrom++;
                                proIntermediaryStep = proMove.pointTo;
                            }
                            else if (!Context.DiceManager.IfDiceWereAdded &&
                                        proMove.pointFrom == proIntermediaryStep && proMove.pointTo == to)
                            {
                                proMatchedFrom++;
                                proMatchedTo += 2;
                                proIntermediaryStep = 0;
                                proIntermediaryMatch = true;
                                goto NextProMove;
                            }
                            else if (proMove.pointTo == to)
                            {
                                proMatchedTo++;
                            }
                        }
                    }

                    NextProMove:;
                }

                // EXACT MATCH
                if (proMatches == movesMade)
                {
                    Context.ProMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
                // DOUBLE MOVE
                else if (Context.DiceManager.IfDiceWereAdded && (proMatchedFrom + proMatchedTo) == proMatchCounter)
                {
                    Context.ProMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
                // DOUBLE ROLLED
                else if (proMatchCounter > 2 && (proMatchedFrom + proMatchedTo) == proMatchCounter)
                {
                    Context.ProMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
                else if (proIntermediaryMatch)
                {
                    Context.ProMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
            }

            BailOut:;

            Context.ProTurnWasAnalysed = true;
        }

        private void ConvertAIData()
        {
            var index = 0;

            foreach (FromTo ft in Context.AITopRankedMove.movePart)
            {
                int from = ft.from == "bar" ? 25 : int.Parse(ft.from);
                int to = ft.to == "off" ? 0 : int.Parse(ft.to);

                var topRanked = Context.TopRankedMovesInfo[index];

                topRanked.pointFrom = from;
                topRanked.pointTo = to;

                Context.TopRankedMovesInfo[index++] = topRanked;
            }

            Context.AITurnWasAnalysed = true;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}