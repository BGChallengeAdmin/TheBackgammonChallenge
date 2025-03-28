namespace Backgammon
{
    public class EvaluateAIRankedMovesState2DS : GameState2DStrategy
    {
        public EvaluateAIRankedMovesState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: EVALUATE A.I. RANKS");

            // NOTE: CLEAR UP U.I. - IN CASE OF BAIL OUT FROM A.I. GAME
            Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, 0, 0, false);

            Context.TurnConfig.AITurnWasAnalysed = false;

            if (Context.TurnConfig.AIDataAvailable)
            {
                ConvertAIDataToTopRanked();
                EvaluateAIRanks(Context.TurnConfig.IsPlayersTurn);
            }

            if (Context.TurnConfig.IsPlayersTurn)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluatePlayer;
            else
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.EvaluateOpponent;
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
            for (int aiM = 0; aiM < 4; aiM++) { Context.TurnConfig.TopRankedMovesInfo[aiM] = Context.TurnConfig.TopRankedMovesInfo[aiM].Reset(); }

            foreach (FromTo ft in Context.TurnConfig.AITopRankedMove.movePart)
            {
                int from = ft.from == "bar" ? 25 : int.Parse(ft.from);
                int to = ft.to == "off" ? 0 : int.Parse(ft.to);

                var topRanked = Context.TurnConfig.TopRankedMovesInfo[index];

                topRanked.pointFrom = from;
                topRanked.pointTo = to;

                Context.TurnConfig.TopRankedMovesInfo[index++] = topRanked;
            }

            Context.TurnConfig.AITurnWasAnalysed = true;
        }

        private void EvaluateAIRanks(bool ifPlayerTurn)
        {
            GameStateContext2D.BoardState boardState = new GameStateContext2D.BoardState();

            var debug = false;

            foreach (var aiRankedData in Context.TurnConfig.AIRankedMoves)
            {
                boardState.SetStateFromState(Context.TurnConfig.TurnBeginBoardState);

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
                    case 1: Context.TurnConfig.Rank1BoardState.SetStateFromState(boardState); break;
                    case 2: Context.TurnConfig.Rank2BoardState.SetStateFromState(boardState); break;
                    case 3: Context.TurnConfig.Rank3BoardState.SetStateFromState(boardState); break;
                    case 4: Context.TurnConfig.Rank4BoardState.SetStateFromState(boardState); break;
                    case 5: Context.TurnConfig.Rank5BoardState.SetStateFromState(boardState); break;
                }
            }
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}