using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    [Serializable]
    public class GraphStatsData
    {
        public string matchKey = string.Empty;
        public string player1 = string.Empty;
        public string player2 = string.Empty;
        public string playingAs = string.Empty;
        public float totalMovesMade = 0;
        public List<GraphStatsGameData> gameDataList = new List<GraphStatsGameData>();
        public GraphStatsGameData[] gameData = null;
    }

    [Serializable]
    public class GraphStatsGameData
    {
        public string matchKey = string.Empty;
        public string player1 = string.Empty;
        public string player2 = string.Empty;
        public string playingAs = string.Empty;
        public string gameName = string.Empty;
        public float movesMade = 0;
        public float movesMatched = 0;
        public float topMatched = 0;
        public float proTopMatched = 0;
    }

    [Serializable]
    public class ReadJSONFromGraphStatsFile
    {
        public GraphStatsData[] graphDataPoints;

        public void ReadData(string fileData)
        {
            graphDataPoints = new GraphStatsData[Main.Instance.PlayerScoresObj.NumberOfPlayerScoreMatches];

            JsonUtility.FromJsonOverwrite(fileData, this);
        }

        public GraphStatsData[] LoadedDataPoints
        {
            get => graphDataPoints;
        }
    }
}