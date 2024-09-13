using System;
using UnityEngine;

namespace Backgammon
{
    [Serializable]
    public class AIScoreData
    {
        public AIScoreData() { ResetPlayerScores(); }

        public int TEST = 0;

        // SCORES
        public int MatchesWon = 0;
        public int MatchesLost = 0;
        public int GamesWon = 0;
        public int GamesLost = 0;

        // EASY
        public int GamesPlayedEasy = 0;
        public int GamesWonEasy = 0;
        public int GamesLostEasy = 0;
        public int PointsWonEasy = 0;
        public int PointsLostEasy = 0;
        public int MatchesWonEasy = 0;
        public int MatchesLostEasy = 0;

        // MEDIUM
        public int GamesPlayedMedium = 0;
        public int GamesWonMedium = 0;
        public int GamesLostMedium = 0;
        public int PointsWonMedium = 0;
        public int PointsLostMedium = 0;
        public int MatchesWonMedium = 0;
        public int MatchesLostMedium = 0;

        // HARD
        public int GamesPlayedHard = 0;
        public int GamesWonHard = 0;
        public int GamesLostHard = 0;
        public int PointsWonHard = 0;
        public int PointsLostHard = 0;
        public int MatchesWonHard = 0;
        public int MatchesLostHard = 0;

        // PERFECT
        public int GamesPlayedPerfect = 0;
        public int GamesWonPerfect = 0;
        public int GamesLostPerfect = 0;
        public int PointsWonPerfect = 0;
        public int PointsLostPerfect = 0;
        public int MatchesWonPerfect = 0;
        public int MatchesLostPerfect = 0;

        public void ResetPlayerScores()
        {
            MatchesWon = 0;
            MatchesLost = 0;
            GamesWon = 0;
            GamesLost = 0;

            GamesPlayedEasy = 0;
            GamesWonEasy = 0;
            GamesLostEasy = 0;
            PointsWonEasy = 0;
            PointsLostEasy = 0;
            MatchesWonEasy = 0;
            MatchesLostEasy = 0;

            GamesPlayedMedium = 0;
            GamesWonMedium = 0;
            GamesLostMedium = 0;
            PointsWonMedium = 0;
            PointsLostMedium = 0;
            MatchesWonMedium = 0;
            MatchesLostMedium = 0;

            GamesPlayedHard = 0;
            GamesWonHard = 0;
            GamesLostHard = 0;
            PointsWonHard = 0;
            PointsLostHard = 0;
            MatchesWonHard = 0;
            MatchesLostHard = 0;

            GamesPlayedPerfect = 0;
            GamesWonPerfect = 0;
            GamesLostPerfect = 0;
            PointsWonPerfect = 0;
            PointsLostPerfect = 0;
            MatchesWonPerfect = 0;
            MatchesLostPerfect = 0;
        }
    }

    [Serializable]
    public class ReadJSONFromAIScoreFile
    {
        public AIScoreData aiScoreData;

        public void ReadData(string fileData)
        {
            aiScoreData = new AIScoreData();

            JsonUtility.FromJsonOverwrite(fileData, this);
        }

        public AIScoreData LoadData
        {
            get => aiScoreData;
        }
    }
}