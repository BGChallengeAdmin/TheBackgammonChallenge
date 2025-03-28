public static class GameScoreInfo
{
    // CONTINUE GAME

    public struct ContinueGame
    {
        public string MatchTitle;
        public string MatchID;
        public int PlayingAs;
        public int MatchIndex;
        public string GameName;
        public int GameIndex;
        public int TurnIndex;
        public int MovesMade;
        public int OpponentMovesMade;
        public int MovesMatched;
        public int AITopMatched;
        public int PlayerSecondMatched;
        public int ProMovesMatched;
        public int ProSecondMatched;
        public int OpponentMovesMatched;
        public int OpponentSecondMatched;
        public int PlayerDoublingValue;
        public int ProDoublingValue;
        public int PlayerTotalScore;
        public int ProTotalScore;
        public int OpponentTotalScore;
        public bool IsPlayerTurn;
        public int DemoState;
        public bool IfMatchToContinue;
        public string BoardState;

        public void Reset()
        {
            MatchTitle = string.Empty;
            MatchID = string.Empty;
            PlayingAs = -1;
            MatchIndex = -1;
            GameName = string.Empty;
            GameIndex = -1;
            TurnIndex = -1;
            MovesMade = 0;
            OpponentMovesMade = 0;
            MovesMatched = 0;
            AITopMatched = 0;
            PlayerSecondMatched = 0;
            ProMovesMatched = 0;
            ProSecondMatched = 0;
            OpponentMovesMatched = 0;
            OpponentSecondMatched = 0;
            PlayerDoublingValue = 0;
            ProDoublingValue = 0;
            PlayerTotalScore = 0;
            ProTotalScore = 0;
            OpponentTotalScore = 0;
            IsPlayerTurn = false;
            DemoState = -1;
            IfMatchToContinue = false;
            BoardState = string.Empty;
        }
    }

    public struct ScoreInfo
    {
        public int movesMade;
        public int movesMatched;
        public int playerTopMatched;
        public int proTopMatched;
        public int playerDoublingValue;
        public int proDoublingValue;
        public int playerTotalScore;
        public int proTotalScore;
        public int aiMovesMatched;
        public int aiTopMatched;
        public int aiSecondMatched;
        public int aiThirdMatched;
        public float activeTimePlayed;

        public void Reset()
        {
            movesMade = 0;
            movesMatched = 0;
            playerTopMatched = 0;
            proTopMatched = 0;

            aiMovesMatched = 0;
            aiTopMatched = 0;
            aiSecondMatched = 0;
            aiThirdMatched = 0;
            activeTimePlayed = 0f;

            playerDoublingValue = 0;
            proDoublingValue = 0;
            playerTotalScore = 0;
            proTotalScore = 0;
        }
    }
}
