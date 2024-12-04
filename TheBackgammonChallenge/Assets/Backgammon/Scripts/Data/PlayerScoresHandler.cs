using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Backgammon
{
    public class PlayerScoresHandler : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_loadScoresData;

        private string playerScoresDirectoryAbs = string.Empty;
        private string playerScoresDirectory = "/BackgammonChallenge/PlayerScoreData";

        // PLAYER SCORES
        private string playerScoresFilename = "/PlayerScoresFile.txt";
        private string playerScoresFilenameJSON = "/PlayerScoresFileJSON.txt";
        private Dictionary<string, PlayerScoreData> _playerScoreDict;
        private List<Backgammon_Asset.MatchReplayDLC> matchReplayDLCList = null;
        private Backgammon_Asset.MatchReplayDLC[] matchReplayDLCs = null;

        // STATS GRAPH
        //private string playerGraphStatsFilenameJSON = "/PlayerGraphStatsFileJSON.txt";
        //private List<GraphStatsData> playerGraphStatsList;
        //private Dictionary<string, float> playerStatisticsDict = null;

        // AI SCORES
        private string playerAIScoresFilenameJSON = "/PlayerAIScoresFileJSON.txt";
        private AIScoreData aiScoreData = null;

        // READ HANDLERS
        private ReadJSONFromScoreFile readJSONFromScoreFile;
        //private ReadJSONFromGraphStatsFile readJSONFromGraphStatsFile;
        private ReadJSONFromAIScoreFile readJSONFromAIScoreDataFile;

        private LanguageScriptableObject languageSO;

        private void Awake()
        {
            playerScoresDirectoryAbs = Application.persistentDataPath + playerScoresDirectory;
            _playerScoreDict = new Dictionary<string, PlayerScoreData>();
            //playerGraphStatsList = new List<GraphStatsData>();
            //playerStatisticsDict = new Dictionary<string, float>();
            aiScoreData = new AIScoreData();

            readJSONFromScoreFile = new ReadJSONFromScoreFile();
            //readJSONFromGraphStatsFile = new ReadJSONFromGraphStatsFile();
            readJSONFromAIScoreDataFile = new ReadJSONFromAIScoreFile();

            //Debug.Log(playerScoresDirectoryAbs);
        }

        internal void SetUseDebugPlayerData(bool useDebugLogging)
        {
            debug_loadScoresData.ShowMesssage = useDebugLogging;
        }

        internal bool AttemptToLoadPlayerData()
        {
            // ADD ANY LOADED MATCHES BEFORE PLAYER DATA IS READ
            PrePopulateScoreDictionary();
            PrePopulateGraphStatsList();

            // TEST IF DIRECTORY EXISTS
            if (!Directory.Exists(playerScoresDirectoryAbs))
            {
                Directory.CreateDirectory(playerScoresDirectoryAbs);

                if (Directory.Exists(playerScoresDirectoryAbs))
                {
                    debug_loadScoresData.DebugMessage("PLAYER SCORES DIRECTORY CREATED SUCCESSFULLY");
                    ScoreFileLoaded = LoadPlayerData();
                }
            }
            else
            {
                debug_loadScoresData.DebugMessage("PLAYER SCORES DIRECTORY ALREADY EXISTS");
                ScoreFileLoaded = LoadPlayerData();
            }

            return ScoreFileLoaded;
        }

        private bool LoadPlayerData()
        {
            bool fileExists = false;

            if (Directory.GetFiles(playerScoresDirectoryAbs).Length == 0)
            {
                debug_loadScoresData.DebugMessage("PLAYERSCORE FILE DOES NOT EXIST - NEW FILE CREATED");
                WritePlayerScoreFileJSON();
                WritePlayerGraphStatsFileJSON();
                WritePlayerAIScoreDataFileJSON();
            }
            else
            {
                debug_loadScoresData.DebugMessage("DIRECTORY CONTAINS PLAYERSCORE FILE");
                ReadPlayerScoreFileJSON();
                ReadPlayerGraphStatsFileJSON();
                ReadPlayerAIScoreDataFileJSON();

                fileExists = true;
            }

            return fileExists;
        }

        // ------------------------------------------------- FILE OPERATIONS -------------------------------------------------

        private void ReadPlayerScoreFileJSON()
        {
            foreach (string file in Directory.GetFiles(playerScoresDirectoryAbs, "*.txt"))
            {
                // SKIP OVER THE TXT FILE AND GRAPH STATS
                string filename = file.Substring((playerScoresDirectoryAbs.Length + 1), (file.Length - playerScoresDirectoryAbs.Length - 1));
                if (filename == playerScoresFilename.Substring(1, (playerScoresFilename.Length - 1)))
                    continue;
                //if (filename == playerGraphStatsFilenameJSON.Substring(1, (playerGraphStatsFilenameJSON.Length - 1)))
                //    continue;
                if (filename == playerAIScoresFilenameJSON.Substring(1, (playerAIScoresFilenameJSON.Length - 1)))
                    continue;

                PlayerScoreData loadedScoreData = null;
                MatchScoreData loadedMatchData = null;

                using StreamReader sr = new StreamReader(file);
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();

                    readJSONFromScoreFile.ReadData(line);
                }

                PlayerScoreData[] loadedMatches = readJSONFromScoreFile.LoadedMatches;

                for (int match = 0; match < loadedMatches.Length; match++)
                {
                    loadedScoreData = loadedMatches[match];

                    if (_playerScoreDict.ContainsKey(loadedScoreData.matchKey))
                    {
                        PlayerScoreData scoreData = _playerScoreDict[loadedScoreData.matchKey];

                        // UPDATE MATCH DATA
                        loadedMatchData = loadedScoreData.matchScores;
                        loadedMatchData.gameScoresDict = _playerScoreDict[loadedScoreData.matchKey].matchScores.gameScoresDict;
                        loadedMatchData.gameTurnsDict = _playerScoreDict[loadedScoreData.matchKey].matchScores.gameTurnsDict;

                        // UPDATE GAMES DATA
                        foreach (GameScoreData game in loadedMatchData.games)
                        {
                            if (loadedMatchData.gameScoresDict.ContainsKey(game.name))
                            {
                                loadedMatchData.gameScoresDict[game.name] = game;
                                loadedMatchData.gameTurnsDict[game.index] = game.turnDatas;
                            }
                        }

                        scoreData.matchScores = loadedMatchData;

                        // UPDATE DICT OBJECT
                        _playerScoreDict[loadedScoreData.matchKey] = scoreData;
                    }
                }
            }
        }

        public void WritePlayerScoreFileJSON()
        {
            string playerScoreDataJSON = string.Empty;

            foreach (KeyValuePair<string, PlayerScoreData> scoreData in _playerScoreDict)
            {
                scoreData.Value.matchScores.games = scoreData.Value.matchScores.gameScoresDict.Values.ToArray<GameScoreData>();
                playerScoreDataJSON += JsonUtility.ToJson(scoreData.Value) + ",";
            }

            // TRIM COMMA
            if (playerScoreDataJSON != string.Empty)
                playerScoreDataJSON = playerScoreDataJSON.Remove(playerScoreDataJSON.Length - 1);
            string JSONToWrite = "{\"matches\":[" + playerScoreDataJSON + "]}";

            var writer = new StreamWriter(playerScoresDirectoryAbs + "/" + playerScoresFilenameJSON, false);
            writer.Write(JSONToWrite);
            writer.Flush();
            writer.Close();
        }

        private void ReadPlayerGraphStatsFileJSON()
        {
            foreach (string file in Directory.GetFiles(playerScoresDirectoryAbs, "*.txt"))
            {
                // SKIP OVER THE TXT FILE AND PLAYER SCORES
                string filename = file.Substring((playerScoresDirectoryAbs.Length + 1), (file.Length - playerScoresDirectoryAbs.Length - 1));
                if (filename == playerScoresFilename.Substring(1, (playerScoresFilename.Length - 1)))
                    continue;
                if (filename == playerScoresFilenameJSON.Substring(1, (playerScoresFilenameJSON.Length - 1)))
                    continue;
                if (filename == playerAIScoresFilenameJSON.Substring(1, (playerAIScoresFilenameJSON.Length - 1)))
                    continue;

                //if (filename == playerGraphStatsFilenameJSON.Substring(1, (playerGraphStatsFilenameJSON.Length - 1)))
                //{
                //    using StreamReader sr = new StreamReader(file);
                //    while (sr.Peek() >= 0)
                //    {
                //        string line = sr.ReadLine();
                //        readJSONFromGraphStatsFile.ReadData(line);
                //    }

                //    GraphStatsData[] loadedDataPoints = readJSONFromGraphStatsFile.LoadedDataPoints;

                //    Debug.Log("LOADING PLAYER STATS GRAPH " + loadedDataPoints.Length + " " + playerGraphStatsList.Count);

                //    for (int graphData = 0; graphData < loadedDataPoints.Length; graphData++)
                //    {
                //        var graphDataPoint = loadedDataPoints[graphData];

                //        if (graphDataPoint.matchKey != string.Empty)
                //        {
                //            playerGraphStatsList.Add(graphDataPoint);
                //        }
                //    }
                //}
            }
        }

        public void WritePlayerGraphStatsFileJSON()
        {
            string playerGraphStatsDataJSON = string.Empty;

            //foreach (GraphStatsData graphData in playerGraphStatsList)
            //{
            //    graphData.gameData = graphData.gameDataList.ToArray<GraphStatsGameData>();
            //    playerGraphStatsDataJSON += JsonUtility.ToJson(graphData) + ",";
            //}

            //// TRIM COMMA
            //if (playerGraphStatsDataJSON != string.Empty)
            //    playerGraphStatsDataJSON = playerGraphStatsDataJSON.Remove(playerGraphStatsDataJSON.Length - 1);
            //string JSONToWrite = "{\"graphDataPoints\":[" + playerGraphStatsDataJSON + "]}";

            //Debug.Log("WRITING PLAYER STATS " + JSONToWrite);

            //var writer = new StreamWriter(playerScoresDirectoryAbs + "/" + playerGraphStatsFilenameJSON, false);
            //writer.Write(JSONToWrite);
            //writer.Flush();
            //writer.Close();
        }

        private void ReadPlayerAIScoreDataFileJSON()
        {
            foreach (string file in Directory.GetFiles(playerScoresDirectoryAbs, "*.txt"))
            {
                // SKIP OVER THE TXT FILE AND PLAYER SCORES
                string filename = file.Substring((playerScoresDirectoryAbs.Length + 1), (file.Length - playerScoresDirectoryAbs.Length - 1));
                if (filename == playerScoresFilename.Substring(1, (playerScoresFilename.Length - 1)))
                    continue;
                if (filename == playerScoresFilenameJSON.Substring(1, (playerScoresFilenameJSON.Length - 1)))
                    continue;
                //if (filename == playerGraphStatsFilenameJSON.Substring(1, (playerGraphStatsFilenameJSON.Length - 1)))
                //    continue;

                if (filename == playerAIScoresFilenameJSON.Substring(1, (playerAIScoresFilenameJSON.Length - 1)))
                {
                    using StreamReader sr = new StreamReader(file);
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        readJSONFromAIScoreDataFile.ReadData(line);
                    }

                    aiScoreData = readJSONFromAIScoreDataFile.LoadData;
                }
            }
        }

        public void WritePlayerAIScoreDataFileJSON()
        {
            string playerAIScoreDataJSON = JsonUtility.ToJson(aiScoreData);
            string JSONToWrite = "{\"aiScoreData\":" + playerAIScoreDataJSON + "}";

            debug_loadScoresData.DebugMessage($"WRITING AI DATA: {JSONToWrite}");

            var writer = new StreamWriter(playerScoresDirectoryAbs + "/" + playerAIScoresFilenameJSON, false);
            writer.Write(JSONToWrite);
            writer.Flush();
            writer.Close();
        }

        // ------------------------------------------------ DICTIONARY OPERATIONS ----------------------------------------------

        public void PrePopulateScoreDictionary()
        {
            matchReplayDLCList = new List<Backgammon_Asset.MatchReplayDLC>();
            matchReplayDLCs = FindObjectsOfType<Backgammon_Asset.MatchReplayDLC>();

            foreach (Backgammon_Asset.MatchReplayDLC match in matchReplayDLCs)
            {
                matchReplayDLCList.Add(match);
            }

            matchReplayDLCList = matchReplayDLCList.OrderByDescending(replayDLC => replayDLC.ID).ToList();

            foreach (Backgammon_Asset.MatchReplayDLC orderedMatch in matchReplayDLCList)
            {
                foreach (Backgammon_Asset.MatchData match in orderedMatch.MatchData)
                {
                    PlayerScoreData scoreData = new PlayerScoreData();
                    MatchScoreData matchData = new MatchScoreData();
                    matchData.games = new GameScoreData[match.GameCount];
                    int totalTurnsInMatch = 0;
                    int index = 0;

                    foreach (Backgammon_Asset.GameData game in match.GameDataArray)
                    {
                        GameScoreData gameData = new GameScoreData();

                        gameData.name = game.name;
                        gameData.index = index + 1;
                        gameData.numberOfTurns = game.NumberOfMoves;
                        gameData.turnDatas = new TurnData[game.NumberOfMoves];
                        totalTurnsInMatch += game.NumberOfMoves;

                        matchData.gameScoresDict.Add(gameData.name, gameData);
                        matchData.games[index++] = gameData;
                        matchData.gameTurnsDict.Add(gameData.index, gameData.turnDatas);
                    }

                    matchData.name = match.Title;
                    matchData.ID = match.ID;
                    matchData.totalNumberOfTurns = totalTurnsInMatch;

                    //Debug.Log("TURNS " + totalTurnsInMatch);

                    scoreData.matchKey = matchData.name + " " + match.ID;
                    scoreData.matchKey = scoreData.matchKey.TrimEnd();
                    scoreData.matchScores = matchData;
                    scoreData.player1Name = match.Player1;
                    scoreData.player2Name = match.Player2;

                    if (!_playerScoreDict.ContainsKey(scoreData.matchKey))
                        _playerScoreDict.Add(scoreData.matchKey, scoreData);
                    else
                    {
                        _playerScoreDict[scoreData.matchKey] = scoreData;
                        debug_loadScoresData.DebugMessage("REPEAT ID: " + orderedMatch.name + " NAME: " + scoreData.matchScores.name + " KEY: " + scoreData.matchKey);
                    }

                    if (orderedMatch.name == "MatchReplayDLC Variant(Clone)")
                    {
                        debug_loadScoresData.DebugMessage("ASSET MATCH " + scoreData.matchKey);
                    }
                }
            }

            debug_loadScoresData.DebugMessage("DICTIONARY POPULATED " + matchReplayDLCList.Count() + " " + _playerScoreDict.Count());
        }

        public void PrePopulateGraphStatsList()
        {
            // CLEAR THE LIST AND ADD ZERO POINT
            //playerGraphStatsList.Clear();
            //var gameStatsData = new GraphStatsData();
            //gameStatsData.gameDataList.Add(new GraphStatsGameData());
            //playerGraphStatsList.Add(gameStatsData);
        }

        public PlayerScoreData GetPlayerScoreData(string matchKey)
        {
            if (_playerScoreDict.ContainsKey(matchKey))
            {
                debug_loadScoresData.DebugMessage("***** MATCH EXISTS *****");
                return _playerScoreDict[matchKey];
            }
            else
            {
                debug_loadScoresData.DebugMessage("***** NEW MATCH CREATED *****");
                return new PlayerScoreData();
            }
        }

        public void SetPlayerScoreData(PlayerScoreData data)
        {
            MatchScoreData currentMatch = data.matchScores;

            // NOTE: PLAYING AGAINST AI - ONLY SINGLE GAME IN MATCH - DO NOT RESET ON SAVE
            // MATCH IS RESET FROM AI MATCH SELECT
            currentMatch.totalNumberOfMovesSeen = 0;
            currentMatch.movesMade = 0;
            currentMatch.movesMatched = 0;
            currentMatch.topMatched = 0;
            currentMatch.proTopMatched = 0;
            currentMatch.indexTurns = 0f;
            currentMatch.activeTimePlayed = 0;
            currentMatch.totalPointsScoredPlayer = 0;
            currentMatch.totalPointsScoredPro = 0;

            currentMatch.player1PossibleMoves = 0f;
            currentMatch.player1MovesMade = 0f;
            currentMatch.player1MovesMatched = 0f;
            currentMatch.player1TopMatched = 0f;
            currentMatch.player1ProTopMatched = 0f;

            currentMatch.player1BestMovesMade = 0f;
            currentMatch.player1BestMovesMatched = 0f;
            currentMatch.player1BestTopMatched = 0f;
            currentMatch.player1BestProTopMatched = 0f;

            currentMatch.player2PossibleMoves = 0f;
            currentMatch.player2MovesMade = 0f;
            currentMatch.player2MovesMatched = 0f;
            currentMatch.player2TopMatched = 0f;
            currentMatch.player2ProTopMatched = 0f;

            currentMatch.player2BestMovesMade = 0f;
            currentMatch.player2BestMovesMatched = 0f;
            currentMatch.player2BestTopMatched = 0f;
            currentMatch.player2BestProTopMatched = 0f;

            foreach (KeyValuePair<string, GameScoreData> game in currentMatch.gameScoresDict)
            {
                if (data.matchScores.name == "DEMO")
                    debug_loadScoresData.DebugMessage("DEMO " + game.Value.totalCounterMovesSeenPlayed);

                // MATCH STATS
                currentMatch.totalNumberOfMovesSeen += game.Value.totalCounterMovesSeenPlayed;
                currentMatch.movesMade += game.Value.fullCumulativeMovesMade;
                currentMatch.movesMatched += game.Value.fullCumulativeMovesMatched;
                currentMatch.topMatched += game.Value.fullCumulativeTopMatchedMoves;
                currentMatch.proTopMatched += game.Value.fullCumulativeProTopMatchedMoves;
                currentMatch.indexTurns += game.Value.cumulativeIndexTurnsPlayed;
                currentMatch.activeTimePlayed += game.Value.activeTimePlayed;
                currentMatch.totalPointsScoredPlayer += game.Value.totalPointsScoredPlayer;
                currentMatch.totalPointsScoredPro += game.Value.totalPointsScoredPro;

                // SEPARATED STATS
                currentMatch.player1PossibleMoves += game.Value.player1PossibleCounterMoves;
                currentMatch.player1MovesMade += game.Value.player1MovesMade;
                currentMatch.player1MovesMatched += game.Value.player1MovesMatched;
                currentMatch.player1TopMatched += game.Value.player1TopMatched;
                currentMatch.player1ProTopMatched += game.Value.player1ProTopMatched;

                currentMatch.player1BestMovesMade += game.Value.player1BestMovesMade;
                currentMatch.player1BestMovesMatched += game.Value.player1BestMovesMatched;
                currentMatch.player1BestTopMatched += game.Value.player1BestTopMatched;
                currentMatch.player1BestProTopMatched += game.Value.player1BestProTopMatched;

                currentMatch.player2PossibleMoves += game.Value.player2PossibleCounterMoves;
                currentMatch.player2MovesMade += game.Value.player2MovesMade;
                currentMatch.player2MovesMatched += game.Value.player2MovesMatched;
                currentMatch.player2TopMatched += game.Value.player2TopMatched;
                currentMatch.player2ProTopMatched += game.Value.player2ProTopMatched;

                currentMatch.player2BestMovesMade += game.Value.player2BestMovesMade;
                currentMatch.player2BestMovesMatched += game.Value.player2BestMovesMatched;
                currentMatch.player2BestTopMatched += game.Value.player2BestTopMatched;
                currentMatch.player2BestProTopMatched += game.Value.player2BestProTopMatched;

                // GAME TURNS
                currentMatch.gameTurnsDict[game.Value.index] = game.Value.turnDatas;
            }

            //Debug.Log("DATA " + currentMatch.movesMade + " " + currentMatch.movesMatched + " " + currentMatch.topMatched + " " + currentMatch.proTopMatched);
            //Debug.Log("DATA1 " + currentMatch.player1BestMovesMade + " " + currentMatch.player1BestMovesMatched + " " + currentMatch.player1BestTopMatched + " " + currentMatch.player1BestProTopMatched);

            currentMatch.percentageMatched = Mathf.Round((currentMatch.topMatched / currentMatch.movesMade) * 100f);
            currentMatch.percentageComplete = Mathf.Round((currentMatch.indexTurns / currentMatch.totalNumberOfTurns) * 100f);

            data.moveCompletionPercentage = currentMatch.percentageComplete;
            data.matchCompletionPercentage = Mathf.Round((currentMatch.topMatched / currentMatch.totalNumberOfMovesSeen) * 100f);
            data.matchScores = currentMatch;

            if (_playerScoreDict.ContainsKey(data.matchKey))
                _playerScoreDict[data.matchKey] = data;
            else
                _playerScoreDict.Add(data.matchKey, data);
        }

        //public void SetGraphStatsData(GraphStatsGameData gameData)
        //{
        //    // CHECK PREVIOUS ENTRY IN DICTIONARY - IF SAME MATCH ADD NEW GAME DATA
        //    if (playerGraphStatsList.Last<GraphStatsData>().matchKey == gameData.matchKey)
        //    {
        //        var matchData = playerGraphStatsList.Last<GraphStatsData>();

        //        matchData.totalMovesMade += gameData.movesMade;
        //        matchData.gameDataList.Add(gameData);
        //        playerGraphStatsList[playerGraphStatsList.Count - 1] = matchData;
        //    }
        //    else
        //    {
        //        var gameStatsData = new GraphStatsData();
        //        gameStatsData.matchKey = gameData.matchKey;
        //        gameStatsData.player1 = gameData.player1;
        //        gameStatsData.player2 = gameData.player2;
        //        gameStatsData.playingAs = gameData.playingAs;
        //        gameStatsData.totalMovesMade = gameData.movesMade;
        //        gameStatsData.gameDataList.Add(gameData);

        //        playerGraphStatsList.Add(gameStatsData);
        //    }
        //}

        public AIScoreData GetAIScoreData()
        {
            return aiScoreData;
        }

        // ---------------------------------------------- GET DICTIONARY STATISTICS ---------------------------------------------

        public Dictionary<string, PlayerScoreData> GetPlayerScoresDict()
        {
            return _playerScoreDict;
        }

        //public Dictionary<string, float> GetPlayerStatisticsDict()
        //{
        //    int availableMatches = 0;
        //    int availabaleGames = 0;

        //    float totalTurnsAllMatches = 0f;
        //    float totalMovesSeenPlayedAllMatches = 0f;
        //    float totalPlayerTurnsPlayed = 0f;
        //    float totalPlayerMovesMade = 0f;

        //    float proMatches = 0f;
        //    float totalAIMatchesMade = 0f;
        //    float totalProAIMatchesMade = 0f;

        //    float overallCompletion = 0f;
        //    float activeTimePlayed = 0f;

        //    foreach (KeyValuePair<string, PlayerScoreData> score in _playerScoreDict)
        //    {
        //        MatchScoreData match = score.Value.matchScores;

        //        // DO NOT INCLUDE THE DEMO IN THE OVERALL STATS
        //        if (match.name == "DEMO")
        //            continue;

        //        availableMatches++;
        //        totalTurnsAllMatches += match.totalNumberOfTurns;

        //        foreach (KeyValuePair<string, GameScoreData> game in match.gameScoresDict)
        //        {
        //            availabaleGames++;

        //            totalMovesSeenPlayedAllMatches += game.Value.totalCounterMovesSeenPlayed;
        //            totalPlayerTurnsPlayed += (game.Value.player1IndexTurn >= game.Value.player2IndexTurn ? game.Value.player1IndexTurn : game.Value.player2IndexTurn);
        //            totalPlayerMovesMade += game.Value.fullCumulativeMovesMade;

        //            proMatches += game.Value.fullCumulativeMovesMatched;
        //            totalAIMatchesMade += game.Value.fullCumulativeTopMatchedMoves;
        //            totalProAIMatchesMade += game.Value.fullCumulativeProTopMatchedMoves;

        //            overallCompletion = (totalPlayerMovesMade / (totalMovesSeenPlayedAllMatches != 0 ? totalMovesSeenPlayedAllMatches : 1)) * 100f;
        //            activeTimePlayed += game.Value.activeTimePlayed;
        //        }
        //    }

        //    if (playerStatisticsDict.ContainsKey("availableMatches"))
        //        playerStatisticsDict["availableMatches"] = availableMatches;
        //    else playerStatisticsDict.Add("availableMatches", availableMatches);

        //    if (playerStatisticsDict.ContainsKey("availabaleGames"))
        //        playerStatisticsDict["availabaleGames"] = availabaleGames;
        //    else playerStatisticsDict.Add("availabaleGames", availabaleGames);

        //    if (playerStatisticsDict.ContainsKey("totalTurnsAllMatches"))
        //        playerStatisticsDict["totalTurnsAllMatches"] = totalTurnsAllMatches;
        //    else playerStatisticsDict.Add("totalTurnsAllMatches", totalTurnsAllMatches);

        //    if (playerStatisticsDict.ContainsKey("totalMovesAllMatches"))
        //        playerStatisticsDict["totalMovesAllMatches"] = totalMovesSeenPlayedAllMatches;
        //    else playerStatisticsDict.Add("totalMovesAllMatches", totalMovesSeenPlayedAllMatches);

        //    if (playerStatisticsDict.ContainsKey("totalPlayerTurnsPlayed"))
        //        playerStatisticsDict["totalPlayerTurnsPlayed"] = totalPlayerTurnsPlayed;
        //    else playerStatisticsDict.Add("totalPlayerTurnsPlayed", totalPlayerTurnsPlayed);

        //    if (playerStatisticsDict.ContainsKey("totalPlayerMovesMade"))
        //        playerStatisticsDict["totalPlayerMovesMade"] = totalPlayerMovesMade;
        //    else playerStatisticsDict.Add("totalPlayerMovesMade", totalPlayerMovesMade);

        //    if (playerStatisticsDict.ContainsKey("proMatches"))
        //        playerStatisticsDict["proMatches"] = proMatches;
        //    else playerStatisticsDict.Add("proMatches", proMatches);

        //    if (playerStatisticsDict.ContainsKey("totalAIMatchesMade"))
        //        playerStatisticsDict["totalAIMatchesMade"] = totalAIMatchesMade;
        //    else playerStatisticsDict.Add("totalAIMatchesMade", totalAIMatchesMade);

        //    if (playerStatisticsDict.ContainsKey("totalProAIMatchesMade"))
        //        playerStatisticsDict["totalProAIMatchesMade"] = totalProAIMatchesMade;
        //    else playerStatisticsDict.Add("totalProAIMatchesMade", totalProAIMatchesMade);

        //    if (playerStatisticsDict.ContainsKey("overallCompletion"))
        //        playerStatisticsDict["overallCompletion"] = overallCompletion;
        //    else playerStatisticsDict.Add("overallCompletion", overallCompletion);

        //    if (playerStatisticsDict.ContainsKey("activeTimePlayed"))
        //        playerStatisticsDict["activeTimePlayed"] = activeTimePlayed;
        //    else playerStatisticsDict.Add("activeTimePlayed", activeTimePlayed);

        //    return playerStatisticsDict;
        //}

        public MatchScoreData GetPlayerMatchScore(string matchKey)
        {
            // NOTE: matchReference = matchScores.name + " " + matchScores.ID
            MatchScoreData matchScoreData = null;

            if (_playerScoreDict.ContainsKey(matchKey))
                matchScoreData = _playerScoreDict[matchKey].matchScores;

            return matchScoreData;
        }

        public GameScoreData GetPlayerGameScore(string _matchID, string _gameID)
        {
            GameScoreData gameScoreData = null;

            gameScoreData = GetPlayerMatchScore(_matchID).gameScoresDict[_gameID];

            return gameScoreData;
        }

        //public List<GraphStatsData> GetPlayerGraphStatsData()
        //{
        //    return playerGraphStatsList;
        //}

        public string GetPlayerRanking(float _percentageMatched)
        {
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            return _percentageMatched >= 90f ? languageSO.endInternational :
                    _percentageMatched >= 80f ? languageSO.endMaster :
                    _percentageMatched >= 65f ? languageSO.endExceptional :
                    _percentageMatched >= 40f ? languageSO.endIntermediate :
                    _percentageMatched >= 10f ? languageSO.endBeginner : languageSO.endUnranked;
        }

        public string GetPlayerRankingGrammar(float _percentageMatched)
        {
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            return _percentageMatched >= 90f ? languageSO.an :
                    _percentageMatched >= 80f ? languageSO.a :
                    _percentageMatched >= 65f ? languageSO.an :
                    _percentageMatched >= 40f ? languageSO.an :
                    _percentageMatched >= 10f ? languageSO.a : languageSO.an;
        }

        public string GetPlayerDifficultyString(string _difficulty)
        {
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;
            var difficult = string.Empty;

            if (_difficulty == "EASY")
            {
                difficult = (languageSO.an + " " + languageSO.matchAISelectEASY);
            }
            else if (_difficulty == "MEDIUM")
            {
                difficult = (languageSO.a + " " + languageSO.matchAISelectMEDIUM);
            }
            else if (_difficulty == "HARD")
            {
                difficult = (languageSO.a + " " + languageSO.matchAISelectHARD);
            }
            else if (_difficulty == "PERFECT")
            {
                difficult = (languageSO.a + " " + languageSO.matchAISelectPERFECT);
            }
            else if (_difficulty == "RANDOM")
            {
                difficult = (languageSO.a + " " + languageSO.matchAISelectRandom);
            }

            return difficult;
        }

        public string GetPlayerAIDifficultlyString(string _difficulty)
        {
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;
            var difficult = string.Empty;

            if (_difficulty == "EASY")
            {
                difficult = languageSO.matchAISelectEASY;
            }
            else if (_difficulty == "MEDIUM")
            {
                difficult = languageSO.matchAISelectMEDIUM;
            }
            else if (_difficulty == "HARD")
            {
                difficult = languageSO.matchAISelectHARD;
            }
            else if (_difficulty == "PERFECT")
            {
                difficult = languageSO.matchAISelectPERFECT;
            }

            return difficult;

        }

        // RESET THE DICITONARY AND FILE
        internal void ResetPlayerScoreData()
        {
            _playerScoreDict.Clear();
            //playerGraphStatsList.Clear();
            aiScoreData.ResetPlayerScores();

            if (Directory.Exists(playerScoresDirectoryAbs))
            {
                Directory.Delete(playerScoresDirectoryAbs, true);
                debug_loadScoresData.DebugMessage("PLAYER SCORE DIRECTORY DELETED");
            }

            AttemptToLoadPlayerData();
        }

        //// ------------------------------------------------- GETTERS && SETTERS -----------------------------------------------------

        public bool ScoreFileLoaded
        {
            get { return doesScoresFileExist; }
            set { doesScoresFileExist = value; }
        }

        public int NumberOfPlayerScoreMatches
        {
            get => _playerScoreDict.Count;
        }

        private bool doesScoresFileExist = false;

        public string errorMessage = string.Empty;
    }
}
