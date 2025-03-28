using static Backgammon.GameStateContext;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backgammon
{
    public class EvaluateOpponentMoveState : GameState
    {
        public EvaluateOpponentMoveState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            // DEFAULT TO END TURN
            ActiveState = GameStateMachine.EGameState.TurnEnd;

            AnalyseProTurn();
            
            if (Context.OpponentTurnWasAnalysed)
            {
                Context.OpponentScoreThisGame += Context.OpponentMatchedRankThisTurn != 0 ?
                                             Mathf.Clamp((4 - Context.OpponentMatchedRankThisTurn), 0, 3) : 0;

                if (Context.OpponentMatchedRankThisTurn == 1) Context.OpponentTopRankedThisGame += 1;
                else if (Context.OpponentMatchedRankThisTurn == 2) Context.OpponentSecondRankedThisGame += 1;

                Context.OpponentMoveEvaluated = false;
                ActiveState = GameStateMachine.EGameState.GeneralInfo;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
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
                Context.OpponentMatchedRankThisTurn = 0;
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
                    Context.OpponentMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
                // DOUBLE MOVE
                else if (Context.DiceManager.IfDiceWereAdded && (proMatchedFrom + proMatchedTo) == proMatchCounter)
                {
                    Context.OpponentMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
                // DOUBLE ROLLED
                else if (proMatchCounter > 2 && (proMatchedFrom + proMatchedTo) == proMatchCounter)
                {
                    Context.OpponentMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
                else if (proIntermediaryMatch)
                {
                    Context.OpponentMatchedRankThisTurn = rankedMove.rank;
                    goto BailOut;
                }
            }

            BailOut:;

            Context.OpponentTurnWasAnalysed = true;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}