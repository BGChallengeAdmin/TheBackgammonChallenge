using UnityEngine;

namespace Backgammon
{
    [CreateAssetMenu(menuName = "ScriptableObjects/AISettings", fileName = "AISettings")]
    public class AISettingsScriptableObject : ScriptableObject
    {
        [Header("PLAYER SETTINGS")]
        public int PlayerRankLowest;
        public int PlayerRankHighest;
        public int PlayerPoints;
        public int PlayerPly;
        public float PlayerNoise;
        public string PlayerPreset;
        public int GamesPlayed;

        [Header("DEFAULT SETTINGS")]
        public int PointsMax;
        public int PointsMin;
        public int PlyMax;
        public int PlyMin;
        public float NoiseMax;
        public float NoiseMin;

        [Header("SCORES")]
        public int MatchesWon;
        public int MatchesLost;
        public int GamesWon;
        public int GamesLost;

        [Header("EASY")]
        public int GamesPlayedEasy;
        public int GamesWonEasy;
        public int GamesLostEasy;
        public int PointsWonEasy;
        public int PointsLostEasy;

        [Header("MEDIUM")]
        public int GamesPlayedMedium;
        public int GamesWonMedium;
        public int GamesLostMedium;
        public int PointsWonMedium;
        public int PointsLostMedium;

        [Header("HARD")]
        public int GamesPlayedHard;
        public int GamesWonHard;
        public int GamesLostHard;
        public int PointsWonHard;
        public int PointsLostHard;

        [Header("PERFECT")]
        public int GamesPlayedPerfect;
        public int GamesWonPerfect;
        public int GamesLostPerfect;
        public int PointsWonPerfect;
        public int PointsLostPerfect;

        [Header("CONTINUE")]
        public GameStateContext2D.BoardState ContinueBoardState;
        public Game2D.PlayingAs PlayerTurnID;
    }
}