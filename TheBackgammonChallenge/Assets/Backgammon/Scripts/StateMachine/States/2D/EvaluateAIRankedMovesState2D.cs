using UnityEngine;

namespace Backgammon
{
    public class EvaluateAIRankedMovesState2D : GameState2D
    {
        public EvaluateAIRankedMovesState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: EVALUATE A.I. RANKS");

            // NOTE: CLEAR UP U.I. - IN CASE OF BAIL OUT FROM A.I. GAME
            Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, 0, 0, false);

            Context.AITurnWasAnalysed = false;

            if (Context.AIDataAvailable)
            {
                ConvertAIDataToTopRanked();
                EvaluateAIRanks(Context.IsPlayersTurn);
            }

            if (Context.IsPlayersTurn)
                ActiveState = GameStateMachine2D.EGameState2D.EvaluatePlayer;
            else 
                ActiveState = GameStateMachine2D.EGameState2D.EvaluateOpponent;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        private void ConvertAIDataToTopRanked()
        {
            var index = 0;

            // RESET PREVIOUS MOVES TO PREVENT RESIDUALS
            for (int aiM = 0; aiM < 4; aiM++) { Context.TopRankedMovesInfo[aiM] = Context.TopRankedMovesInfo[aiM].Reset(); }

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

        private void EvaluateAIRanks(bool ifPlayerTurn)
        {
            GameStateContext2D.BoardState boardState = new GameStateContext2D.BoardState();

            var debug = false;

            foreach (var aiRankedData in Context.AIRankedMoves)
            {
                boardState.SetStateFromState(Context.TurnBeginBoardState);

                if (debug)
                {
                    UnityEngine.Debug.Log($"RANK {aiRankedData.rank}");
                }

                foreach (var ft in aiRankedData.movePart)
                {
                    int from = ft.from == "bar" ? 25 : int.Parse(ft.from);
                    int to = ft.to == "off" ? 0 : int.Parse(ft.to);
                    bool ifBlot = false;

                    if (to > 0)
                    {
                        var point = ifPlayerTurn ? Context.PointsManager.GetPlayerPointByID(to) :
                                                   Context.PointsManager.GetOpponentPointByID(to);
                        ifBlot = boardState.GetPointCounterValue(point.GetID()) == (ifPlayerTurn ? -1 : 1);
                    }

                    boardState.PopPushCounter(from, to, ifBlot, ifPlayerTurn);

                    if (debug)
                    {
                        UnityEngine.Debug.Log($"MOVE {ft.from}/{ft.to} - > {ifBlot}");
                    }
                }

                switch (aiRankedData.rank)
                {
                    case 1: Context.Rank1BoardState.SetStateFromState(boardState); break;
                    case 2: Context.Rank2BoardState.SetStateFromState(boardState); break;
                    case 3: Context.Rank3BoardState.SetStateFromState(boardState); break;
                    case 4: Context.Rank4BoardState.SetStateFromState(boardState); break;
                    case 5: Context.Rank5BoardState.SetStateFromState(boardState); break;
                }
            }
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}