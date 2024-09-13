using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    [Serializable]
    public class PlayerScoreData
    {
        public string matchKey = string.Empty; // NOTE matchKey = matchScores.name + matchScores.ID
        public float moveCompletionPercentage = 0f;
        public float matchCompletionPercentage = 0f;
        public string player1Name = string.Empty;
        public string player2Name = string.Empty;
        public MatchScoreData matchScores = new MatchScoreData();
    }

    [Serializable]
    public class MatchScoreData
    {
        public string name = string.Empty;
        public string ID = string.Empty;

        // MATCH STATS
        public int lastGamePlayed = 0;
        public int lastGameCompleted = 0;
        public int lastPlayedAs = 0;
        public int lastHighestGamePlayedP1 = 0;
        public int lastHighestGamePlayedP2 = 0;
        public int totalPointsScoredPlayer = 0;
        public int totalPointsScoredPro = 0;

        public float totalNumberOfMovesSeen = 0f;
        public float movesMade = 0f;
        public float movesMatched = 0f;
        public float topMatched = 0f;
        public float proTopMatched = 0f;
        public float indexTurns = 0f;
        public float totalNumberOfTurns = 0f;
        public float activeTimePlayed = 0f;
        public float percentageMatched = 0f;
        public float percentageTopMatched = 0f;
        public float percentageProTopMatched = 0f;
        public float percentageComplete = 0f;

        // SEPARATED STATS
        public float player1PossibleMoves = 0f;
        public float player1MovesMade = 0f;
        public float player1MovesMatched = 0f;
        public float player1TopMatched = 0f;
        public float player1ProTopMatched = 0f;

        public float player1BestMovesMade = 0f;
        public float player1BestMovesMatched = 0f;
        public float player1BestTopMatched = 0f;
        public float player1BestProTopMatched = 0f;

        public float player2PossibleMoves = 0f;
        public float player2MovesMade = 0f;
        public float player2MovesMatched = 0f;
        public float player2TopMatched = 0f;
        public float player2ProTopMatched = 0f;

        public float player2BestMovesMade = 0f;
        public float player2BestMovesMatched = 0f;
        public float player2BestTopMatched = 0f;
        public float player2BestProTopMatched = 0f;

        public Dictionary<string, GameScoreData> gameScoresDict = new Dictionary<string, GameScoreData>();
        public GameScoreData[] games = null;

        public Dictionary<int, TurnData[]> gameTurnsDict = new Dictionary<int, TurnData[]>();
    }

    [Serializable]
    public class GameScoreData
    {
        // GAME STATS
        public string name = string.Empty;
        public int index = 0;
        public float numberOfTurns = 0f;
        public float indexTurnPlayed = 0f;
        public float totalCounterMovesSeenPlayed = 0f;
        public float cumulativeIndexTurnsPlayed = 0f;
        public float fullCumulativeMovesMade = 0f;
        public float fullCumulativeMovesMatched = 0f;
        public float fullCumulativeTopMatchedMoves = 0f;
        public float fullCumulativeProTopMatchedMoves = 0f;
        public float activeTimePlayed = 0f;
        public int pointsScoredPlayer = 0;
        public int totalPointsScoredPlayer = 0;
        public int pointsScoredPro = 0;
        public int totalPointsScoredPro = 0;

        // SEPARATED STATS
        public float player1IndexTurn = 0f;
        public float player1PossibleCounterMoves = 0f;
        public float player1MovesMade = 0f;
        public float player1MovesMatched = 0f;
        public float player1TopMatched = 0f;
        public float player1ProTopMatched = 0f;

        public float player1BestIndexTurn = 0f;
        public float player1BestMovesMade = 0f;
        public float player1BestMovesMatched = 0f;
        public float player1BestTopMatched = 0f;
        public float player1BestProTopMatched = 0f;

        public float player2IndexTurn = 0f;
        public float player2PossibleCounterMoves = 0f;
        public float player2MovesMade = 0f;
        public float player2MovesMatched = 0f;
        public float player2TopMatched = 0f;
        public float player2ProTopMatched = 0f;

        public float player2BestIndexTurn = 0f;
        public float player2BestMovesMade = 0f;
        public float player2BestMovesMatched = 0f;
        public float player2BestTopMatched = 0f;
        public float player2BestProTopMatched = 0f;

        // SCORES FROM LAST PLAY THROUGH
        public int playingAs = 0;
        public float fullMovesMade = 0f;
        public float fullMovesMatched = 0f;
        public float fullTopMatched = 0f;
        public float fullProTopMatched = 0f;
        public float fullOpponentTopMatched = 0f;

        // TURN DATA
        public bool turnDataAvailable = false;
        public TurnData[] turnDatas = null;
    }

    [Serializable]
    public class TurnData
    {
        public bool dataAvailable = false;
        public bool playerTurn = false;
        public int dice1Roll = 0;
        public int dice2Roll = 0;
        public string action = string.Empty;
        public int playerRank = 0;
        public int proRank = 0;
    }

    [Serializable]
    public class ReadJSONFromScoreFile
    {
        public PlayerScoreData[] matches;

        public void ReadData(string fileData)
        {
            matches = new PlayerScoreData[Main.Instance.PlayerScoresObj.NumberOfPlayerScoreMatches];

            JsonUtility.FromJsonOverwrite(fileData, this);
        }

        public PlayerScoreData[] LoadedMatches
        {
            get => matches;
        }
    }
}