using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class StatisticsUI : MonoBehaviour
    {
        [SerializeField] private Text _titleText = null;
        [SerializeField] private Text _headerText = null;
        [SerializeField] private Text statistics = null;
        [SerializeField] private Text _backButtonText = null;
        [SerializeField] private Text _statisticsButtonText = null;
        [SerializeField] private Text _resetButtonText = null;

        public GameObject statisticsGraphs;
        LanguageScriptableObject languageSO;

        protected void OnEnable()
        {
            ifResetAppData = false;
            ifBack = false;

            // CONFIGURE LANGUAGE
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                _titleText.text = languageSO.statsTitle;
                _headerText.text = languageSO.statsHeader;
                _backButtonText.text = languageSO.Back;
                _statisticsButtonText.text = languageSO.statsStatistics;
                _resetButtonText.text = languageSO.statsResetData;
            }

            DisplayStatsData();

            // JAMES - ORIGINAL
            //scoreInfo = Main.Instance.scoreInfoPlayerVsPro;
            //movesSoFar = scoreInfo.movesMade;
            //matchPercentage = (movesSoFar > 0 ? (scoreInfo.movesMatched * 100) / (float)(movesSoFar) : 0);
            //statistics.text += "\n\n--- Player Vs Pro ---\nMoves: " + movesSoFar + "\nCorrect Moves: " + scoreInfo.movesMatched + "\nScore: " + matchPercentage.ToString("0.0") + "%";
        }

        private void DisplayStatsData()
        {
            // TOURNAMENT
            //Game.ScoreInfo scoreInfoPlayerVsPro = Main.Instance.scoreInfoPlayerVsPro;
            
            //int movesSoFar = scoreInfoPlayerVsPro.movesMade;
            //float matchPercentage = (movesSoFar > 0 ? (scoreInfoPlayerVsPro.movesMatched * 100) / (float)(movesSoFar) : 0);
            
            ////statistics.text = "--- Tournament ---\nMoves: " + movesSoFar + "\nCorrect Moves: " + scoreInfoPlayerVsPro.movesMatched + "\nScore: " + matchPercentage.ToString("0.0") + "%";

            //// RANDOM PLAY
            //Game.ScoreInfo scoreInfo = Main.Instance.scoreInfo;
            //movesSoFar = scoreInfo.movesMade;

            //float playerMatchPercentage = (movesSoFar > 0 ? (scoreInfo.playerTopMatched * 100) / (float)(movesSoFar) : 0);
            //float proMatchPercentage = (movesSoFar > 0 ? (scoreInfo.proTopMatched * 100) / (float)(movesSoFar) : 0);
            
            //statistics.text = "--- Recent Stats ---\nMoves: " + movesSoFar
            //          + "\nPlayer Matched: " + scoreInfo.playerTopMatched + " Score: " + playerMatchPercentage.ToString("0.0") + "%"
            //          + "\nPro Matched: " + scoreInfo.proTopMatched + " Score: " + proMatchPercentage.ToString("0.0") + "%";

            // NEW VERSION OF STATISTICS HANDLER

            string statisticsString = string.Empty;

            Dictionary<string, float> playerStatisticsDict = Main.Instance.PlayerScoresObj.GetPlayerStatisticsDict();

            float totalPlayerTurnsPlayed = playerStatisticsDict["totalPlayerTurnsPlayed"];
            float totalTurnsAllMatches = playerStatisticsDict["totalTurnsAllMatches"];
            float totalMovesAllMatches = playerStatisticsDict["totalMovesAllMatches"];
            float totalPlayerMovesMade = playerStatisticsDict["totalPlayerMovesMade"];
            
            float proMatches = playerStatisticsDict["proMatches"];
            float totalAIMatchesMade = playerStatisticsDict["totalAIMatchesMade"];
            float totalProAIMatchesMade = playerStatisticsDict["totalProAIMatchesMade"];

            float activeTimePlayed = playerStatisticsDict["activeTimePlayed"];
            float completionPercent = Mathf.Round(playerStatisticsDict["overallCompletion"]);

            int hours = TimeSpan.FromSeconds(activeTimePlayed).Hours;
            int minutes = TimeSpan.FromSeconds(activeTimePlayed).Minutes;
            int seconds = TimeSpan.FromSeconds(activeTimePlayed).Seconds;

            float avgRank1MatchTime = Mathf.Round(seconds / (totalAIMatchesMade == 0 ? 1 : totalAIMatchesMade));

            statisticsString = languageSO.statsTotalMovesPlayed + " " + totalPlayerMovesMade
                            + "\n"
                            + "\n" + languageSO.gameStatsMovesMatched + " " + proMatches + " " + languageSO.statsProMovesEquals + " " + (totalPlayerMovesMade == 0 ? 0 : (Mathf.Round((proMatches / totalPlayerMovesMade) * 100f))) + "%"
                            + "\n   " + languageSO.and + " " + totalAIMatchesMade + " " + languageSO.turnEndReviewRank1 + " = " + (totalPlayerMovesMade == 0 ? 0 : (Mathf.Round((totalAIMatchesMade / totalPlayerMovesMade) * 100f))) + "%"
                            //+ "\nWith an Accuracy of: " + (movesMade == 0 ? 0 : (Mathf.Round((proMatches / movesMade) * 100f))) + "%"
                            + "\n"
                            + "\n" + languageSO.gameStatsProMatched + " " + totalProAIMatchesMade + " " + languageSO.turnEndReviewRank1 + " = " + (totalPlayerMovesMade == 0 ? 0 : (Mathf.Round((totalProAIMatchesMade / totalPlayerMovesMade) * 100f))) + "%"
                            //+ "\nWith an Accuracy of: " + (movesMade == 0 ? 0 : (Mathf.Round((aiMatches / movesMade) * 100f))) + "%"
                            //+ "\nYour Avg. Rank 1 A.I. Match Time is: " + avgRank1MatchTime + " second" + (avgRank1MatchTime >= 1 ? statisticsString : string.Empty)
                            + "\n"
                            + "\n" + languageSO.statsYouHave + " " + playerStatisticsDict["availableMatches"] + " " + languageSO.statsAvailableMatches + " "
                            + playerStatisticsDict["availabaleGames"] + " " + languageSO.Games
                            + "\n"
                            //+ "\n--- Total Completion ---"
                            //+ "\n" + totalPlayerTurnsPlayed + "/" + totalTurnsAllMatches + " Turns For: "
                            //       + (completionPercent != 0 ? (completionPercent < 1 ? "~" : string.Empty) : string.Empty)
                            //       + completionPercent + "%"
                            //+ "\nAnd Have Played: "
                            //            + (hours != 0 ? (hours + " h ") : string.Empty)
                            //            + (minutes != 0 ? (minutes + " m ") : string.Empty)
                            //            + (seconds != 0 ? (seconds + " s") : "0 seconds");
                            + "\n";
                            
            statistics.text = statisticsString;
        }

        public void DisplayStatisticsGraphs()
        {
            statisticsGraphs.gameObject.SetActive(true);
        }

        public void OnResetAppData()
        {
            ifResetAppData = true;

            DisplayStatsData();
        }

        public void DisplayAppStatistics()
        {
            DisplayStatsData();
        }

        public void OnBack()
        {
            ifBack = true;
        }

        public bool ifResetAppData;
        public bool ifBack;
    }
}