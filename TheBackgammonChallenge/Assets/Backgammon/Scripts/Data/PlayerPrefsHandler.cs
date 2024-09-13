using UnityEngine;

namespace Backgammon
{
    public class PlayerPrefsHandler : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_loadPlayerPrefsData;

        internal void Init()
        {
            ScoreInfo.Reset();
            ScoreInfoPlayerVsPro.Reset();

            ScoreInfoContinueAIGame.Reset();
            ScoreInfoContinueProGame.Reset();

            LoadAppData();
        }

        internal void SaveAppData()
        {
            // TODO: Main.Instance.PlayerScoresHandler

            Main.Instance.PlayerScoresObj.WritePlayerScoreFileJSON();
            if (Game2D.IfGameConcluded)
            {
                Main.Instance.PlayerScoresObj.WritePlayerGraphStatsFileJSON();
                Main.Instance.PlayerScoresObj.WritePlayerAIScoreDataFileJSON();
            }

            // PLAYER STATISTICS
            PlayerPrefs.SetInt("movesMade", ScoreInfo.movesMade);
            PlayerPrefs.SetInt("movesMatched", ScoreInfo.movesMatched);
            PlayerPrefs.SetInt("movesMade_PlayerVsPro", ScoreInfoPlayerVsPro.movesMade);
            PlayerPrefs.SetInt("movesMatched_PlayerVsPro", ScoreInfoPlayerVsPro.movesMatched);

            PlayerPrefs.SetInt("proTopMatched", ScoreInfo.proTopMatched);
            PlayerPrefs.SetInt("playerTopMatched", ScoreInfo.playerTopMatched);

            PlayerPrefs.SetInt("aiTopMatched", ScoreInfo.aiTopMatched);
            PlayerPrefs.SetInt("aiSecondMatched", ScoreInfo.aiSecondMatched);
            PlayerPrefs.SetInt("aiThirdMatched", ScoreInfo.aiThirdMatched);

            PlayerPrefs.SetFloat("activeTimePlayed", ScoreInfo.activeTimePlayed);

            // PLAYER APP STATE
            PlayerPrefs.SetString("aiUserID", Main.Instance.AIUserID);
            PlayerPrefs.SetInt("ifFirstPlaythrough", Main.Instance.IfFirstPlaythrough ? 1 : 0);

            // PLATER SETTINGS

            int[] settings = Main.Instance.SettingsUI.GetPlayerSettings();
            PlayerPrefs.SetString("languageShortCode", Main.Instance.LanguageShortCode);
            PlayerPrefs.SetInt("playerColour", settings[0]);
            PlayerPrefs.SetInt("playingFromLHS", settings[1]);
            PlayerPrefs.SetInt("playbackSpeed", settings[2]);

            // UPDATE
            PlayerPrefs.SetString("CurrentVersionNumber", Main.Instance.CurrentVersionNumber);

            // CONTINUE GAME
            if (Main.Instance.IfPlayerVsPro)
            {
                if (ScoreInfoContinueProGame.IfMatchToContinue)
                {
                    // SAVE TO CHALLENGE PRO PREFS
                    debug_loadPlayerPrefsData.DebugMessage($"*********** SAVING CHALLENGE PREFS *************");

                    // CHECK THAT GAME HAS PLAYED '1' TURN
                    if ((ScoreInfoContinueProGame.PlayingAs >= 0) &&
                        (ScoreInfoContinueProGame.MatchIndex >= 0) &&
                        (ScoreInfoContinueProGame.GameIndex >= 0) &&
                        (ScoreInfoContinueProGame.TurnIndex >= 0))
                    {

                        PlayerPrefs.SetInt("ifProMatchToContinue", (ScoreInfoContinueProGame.IfMatchToContinue ? 1 : 0));
                        PlayerPrefs.SetInt("continuePlayingAs", ScoreInfoContinueProGame.PlayingAs);
                        PlayerPrefs.SetString("continueMatchTitle", ScoreInfoContinueProGame.MatchTitle);
                        PlayerPrefs.SetString("continueMatchName", ScoreInfoContinueProGame.MatchID);
                        PlayerPrefs.SetInt("continueMatchIndex", ScoreInfoContinueProGame.MatchIndex);
                        PlayerPrefs.SetInt("continueGameIndex", ScoreInfoContinueProGame.GameIndex);
                        PlayerPrefs.SetInt("continueTurnIndex", ScoreInfoContinueProGame.TurnIndex);
                        PlayerPrefs.SetInt("continueMovesMade", ScoreInfoContinueProGame.MovesMade);
                        PlayerPrefs.SetInt("continueMovesMatched", ScoreInfoContinueProGame.MovesMatched);
                        PlayerPrefs.SetInt("continueAITopMatched", ScoreInfoContinueProGame.AITopMatched);
                        PlayerPrefs.SetInt("continueProMovesMatched", ScoreInfoContinueProGame.ProMovesMatched);
                        PlayerPrefs.SetInt("continueOpponentMovesMatched", ScoreInfoContinueProGame.OpponentMovesMatched);
                        PlayerPrefs.SetInt("continuePlayerDoublingValue", ScoreInfoContinueProGame.PlayerDoublingValue);
                        PlayerPrefs.SetInt("continueProDoublingValue", ScoreInfoContinueProGame.ProDoublingValue);
                        PlayerPrefs.SetInt("continuePlayerTotalScore", ScoreInfoContinueProGame.PlayerTotalScore);
                        PlayerPrefs.SetInt("continueProTotalScore", ScoreInfoContinueProGame.ProTotalScore);
                        PlayerPrefs.SetInt("continueOpponentTotalScore", ScoreInfoContinueProGame.OpponentTotalScore);
                        PlayerPrefs.SetInt("continueDemoState", ScoreInfoContinueProGame.DemoState);
                        PlayerPrefs.SetString("continueBoardState", ScoreInfoContinueProGame.BoardState);
                    }
                }
            }

            if (Main.Instance.IfPlayerVsAI)
            {
                if (ScoreInfoContinueAIGame.IfMatchToContinue)
                {
                    // SAVE TO AI PREFS
                    debug_loadPlayerPrefsData.DebugMessage($"*********** SAVING AI PREFS *************");

                    PlayerPrefs.SetInt("ifAIMatchToContinue", (ScoreInfoContinueAIGame.IfMatchToContinue ? 1 : 0));
                    PlayerPrefs.SetInt("AIcontinuePlayingAs", ScoreInfoContinueAIGame.PlayingAs);
                    PlayerPrefs.SetString("AIcontinueMatchTitle", ScoreInfoContinueAIGame.MatchTitle);
                    PlayerPrefs.SetString("AIcontinueMatchName", ScoreInfoContinueAIGame.MatchID);
                    PlayerPrefs.SetInt("AIcontinueMatchIndex", ScoreInfoContinueAIGame.MatchIndex);
                    PlayerPrefs.SetInt("AIcontinueGameIndex", ScoreInfoContinueAIGame.GameIndex);
                    PlayerPrefs.SetInt("AIcontinueTurnIndex", ScoreInfoContinueAIGame.TurnIndex);
                    PlayerPrefs.SetInt("AIcontinueMovesMade", ScoreInfoContinueAIGame.MovesMade);
                    PlayerPrefs.SetInt("AIcontinueMovesMatched", ScoreInfoContinueAIGame.MovesMatched);
                    PlayerPrefs.SetInt("AIcontinueAITopMatched", ScoreInfoContinueAIGame.AITopMatched);
                    PlayerPrefs.SetInt("AIcontinueProMovesMatched", ScoreInfoContinueAIGame.ProMovesMatched);
                    PlayerPrefs.SetInt("AIcontinueOpponentMovesMatched", ScoreInfoContinueAIGame.OpponentMovesMatched);
                    PlayerPrefs.SetInt("AIcontinuePlayerDoublingValue", ScoreInfoContinueAIGame.PlayerDoublingValue);
                    PlayerPrefs.SetInt("AIcontinueProDoublingValue", ScoreInfoContinueAIGame.ProDoublingValue);
                    PlayerPrefs.SetInt("AIcontinuePlayerTotalScore", ScoreInfoContinueAIGame.PlayerTotalScore);
                    PlayerPrefs.SetInt("AIcontinueProTotalScore", ScoreInfoContinueAIGame.ProTotalScore);
                    PlayerPrefs.SetInt("AIcontinueIsPlayerTurn", (ScoreInfoContinueAIGame.IsPlayerTurn ? 1 : 0));
                    PlayerPrefs.SetString("AIBoardState", ScoreInfoContinueAIGame.BoardState);
                }
            }

            PlayerPrefs.Save();
        }

        internal void LoadAppData()
        {
            // PLAYER STATISTICS
            if (PlayerPrefs.HasKey("movesMade"))
            {
                ScoreInfo.movesMade = PlayerPrefs.GetInt("movesMade");
            }
            if (PlayerPrefs.HasKey("movesMatched"))
            {
                ScoreInfo.movesMatched = PlayerPrefs.GetInt("movesMatched");
            }
            if (PlayerPrefs.HasKey("movesMade_PlayerVsPro"))
            {
                ScoreInfoPlayerVsPro.movesMade = PlayerPrefs.GetInt("movesMade_PlayerVsPro");
            }
            if (PlayerPrefs.HasKey("movesMatched_PlayerVsPro"))
            {
                ScoreInfoPlayerVsPro.movesMatched = PlayerPrefs.GetInt("movesMatched_PlayerVsPro");
            }

            if (PlayerPrefs.HasKey("proTopMatched"))
            {
                ScoreInfo.proTopMatched = PlayerPrefs.GetInt("proTopMatched");
            }
            if (PlayerPrefs.HasKey("playerTopMatched"))
            {
                ScoreInfo.playerTopMatched = PlayerPrefs.GetInt("playerTopMatched");
            }

            if (PlayerPrefs.HasKey("aiTopMatched"))
            {
                ScoreInfo.aiTopMatched = PlayerPrefs.GetInt("aiTopMatched");
            }
            if (PlayerPrefs.HasKey("aiSecondMatched"))
            {
                ScoreInfo.aiSecondMatched = PlayerPrefs.GetInt("aiSecondMatched");
            }
            if (PlayerPrefs.HasKey("aiThirdMatched"))
            {
                ScoreInfo.aiThirdMatched = PlayerPrefs.GetInt("aiThirdMatched");
            }

            if (PlayerPrefs.HasKey("activeTimePlayed"))
            {
                ScoreInfo.activeTimePlayed = PlayerPrefs.GetInt("activeTimePlayed");
            }

            // PLAYER APP STATE
            if (PlayerPrefs.HasKey("aiUserID"))
            {
                Main.Instance.AIUserID = PlayerPrefs.GetString("aiUserID");
            }
            if (PlayerPrefs.HasKey("ifFirstPlaythrough"))
            {
                debug_loadPlayerPrefsData.DebugMessage("IFFIRST " + PlayerPrefs.GetInt("ifFirstPlaythrough"));

                Main.Instance.IfFirstPlaythrough = PlayerPrefs.GetInt("ifFirstPlaythrough") == 1 ? true : false;

                debug_loadPlayerPrefsData.DebugMessage("HAS KEY " + Main.Instance.IfFirstPlaythrough);
            }

            // PLAYER SETTINGS
            int[] settings = Main.Instance.SettingsUI.GetPlayerSettings();
            if (PlayerPrefs.HasKey("languageShortCode"))
            {
                Main.Instance.LanguageShortCode = PlayerPrefs.GetString("languageShortCode");
            }
            if (PlayerPrefs.HasKey("playerColour"))
            {
                settings[0] = PlayerPrefs.GetInt("playerColour");
            }
            if (PlayerPrefs.HasKey("playingFromLHS"))
            {
                settings[1] = PlayerPrefs.GetInt("playingFromLHS");
            }
            if (PlayerPrefs.HasKey("playbackSpeed"))
            {
                settings[2] = PlayerPrefs.GetInt("playbackSpeed");
            }
            Main.Instance.SettingsUI.SetPlayerSettings(settings[0], settings[1], settings[2]);

            // UPDATE
            if (PlayerPrefs.HasKey("CurrentVersionNumber"))
            {
                Main.Instance.CurrentVersionNumber = PlayerPrefs.GetString("CurrentVersionNumber");
                debug_loadPlayerPrefsData.DebugMessage("VERSION: " + Main.Instance.CurrentVersionNumber);
            }

            // CONTINUE GAME
            if (PlayerPrefs.HasKey("ifProMatchToContinue"))
            {
                var ifProMatch = PlayerPrefs.GetInt("ifProMatchToContinue");
                ScoreInfoContinueProGame.IfMatchToContinue = (ifProMatch == 1 ? true : false);
                debug_loadPlayerPrefsData.DebugMessage("PRO TO CONTINUE " + ScoreInfoContinueProGame.IfMatchToContinue);
            }
            if (PlayerPrefs.HasKey("ifAIMatchToContinue"))
            {
                var ifAIMatch = PlayerPrefs.GetInt("ifAIMatchToContinue");
                ScoreInfoContinueAIGame.IfMatchToContinue = (ifAIMatch == 1 ? true : false);
                debug_loadPlayerPrefsData.DebugMessage("AI TO CONTINUE " + ScoreInfoContinueAIGame.IfMatchToContinue);
            }

            // ONLY LOADS WHEN MATCH DATA HAS BEEN SAVED
            if (ScoreInfoContinueProGame.IfMatchToContinue)
            {
                debug_loadPlayerPrefsData.DebugMessage($"***** CONTINUE PLAYING PRO *****");

                if (PlayerPrefs.HasKey("continuePlayingAs"))
                {
                    ScoreInfoContinueProGame.PlayingAs = PlayerPrefs.GetInt("continuePlayingAs");
                    debug_loadPlayerPrefsData.DebugMessage("CONTINUE AS " + (ScoreInfoContinueProGame.PlayingAs == 1 ? "PLAYER1" : "PLAYER2"));
                }
                if (PlayerPrefs.HasKey("continueMatchTitle"))
                {
                    ScoreInfoContinueProGame.MatchTitle = PlayerPrefs.GetString("continueMatchTitle");
                    debug_loadPlayerPrefsData.DebugMessage("MATCH TITLE " + ScoreInfoContinueProGame.MatchTitle);
                }
                if (PlayerPrefs.HasKey("continueMatchName"))
                {
                    ScoreInfoContinueProGame.MatchID = PlayerPrefs.GetString("continueMatchName");
                    debug_loadPlayerPrefsData.DebugMessage("MATCH NAME " + ScoreInfoContinueProGame.MatchID);
                }
                if (PlayerPrefs.HasKey("continueMatchIndex"))
                {
                    ScoreInfoContinueProGame.MatchIndex = PlayerPrefs.GetInt("continueMatchIndex");
                    debug_loadPlayerPrefsData.DebugMessage("INDEX MATCH " + ScoreInfoContinueProGame.MatchIndex);
                }
                if (PlayerPrefs.HasKey("continueGameIndex"))
                {
                    ScoreInfoContinueProGame.GameIndex = PlayerPrefs.GetInt("continueGameIndex");
                    debug_loadPlayerPrefsData.DebugMessage("INDEX GAME " + ScoreInfoContinueProGame.GameIndex);
                }
                if (PlayerPrefs.HasKey("continueTurnIndex"))
                {
                    ScoreInfoContinueProGame.TurnIndex = PlayerPrefs.GetInt("continueTurnIndex");
                    debug_loadPlayerPrefsData.DebugMessage("INDEX TURN " + ScoreInfoContinueProGame.TurnIndex);
                }
                if (PlayerPrefs.HasKey("continueMovesMade"))
                {
                    ScoreInfoContinueProGame.MovesMade = PlayerPrefs.GetInt("continueMovesMade");
                    debug_loadPlayerPrefsData.DebugMessage("MOVES MADE " + ScoreInfoContinueProGame.MovesMade);
                }
                if (PlayerPrefs.HasKey("continueMovesMatched"))
                {
                    ScoreInfoContinueProGame.MovesMatched = PlayerPrefs.GetInt("continueMovesMatched");
                    debug_loadPlayerPrefsData.DebugMessage("MOVES MATCHED " + ScoreInfoContinueProGame.MovesMatched);
                }
                if (PlayerPrefs.HasKey("continueAITopMatched"))
                {
                    ScoreInfoContinueProGame.AITopMatched = PlayerPrefs.GetInt("continueAITopMatched");
                    debug_loadPlayerPrefsData.DebugMessage("AI MOVES MATCHED " + ScoreInfoContinueProGame.AITopMatched);
                }
                if (PlayerPrefs.HasKey("continueProMovesMatched"))
                {
                    ScoreInfoContinueProGame.ProMovesMatched = PlayerPrefs.GetInt("continueProMovesMatched");
                    debug_loadPlayerPrefsData.DebugMessage("PRO MOVES MATCHED " + ScoreInfoContinueProGame.ProMovesMatched);
                }
                if (PlayerPrefs.HasKey("continueOpponentMovesMatched"))
                {
                    ScoreInfoContinueProGame.OpponentMovesMatched = PlayerPrefs.GetInt("continueOpponentMovesMatched");
                    debug_loadPlayerPrefsData.DebugMessage("PRO MOVES MATCHED " + ScoreInfoContinueProGame.OpponentMovesMatched);
                }
                if (PlayerPrefs.HasKey("continuePlayerDoublingValue"))
                {
                    ScoreInfoContinueProGame.PlayerDoublingValue = PlayerPrefs.GetInt("continuePlayerDoublingValue");
                    debug_loadPlayerPrefsData.DebugMessage("PLAYER DOUBLING VALUE " + ScoreInfoContinueProGame.PlayerDoublingValue);
                }
                if (PlayerPrefs.HasKey("continueProDoublingValue"))
                {
                    ScoreInfoContinueProGame.ProDoublingValue = PlayerPrefs.GetInt("continueProDoublingValue");
                    debug_loadPlayerPrefsData.DebugMessage("PRO DOUBLING VALUE " + ScoreInfoContinueProGame.ProDoublingValue);
                }
                if (PlayerPrefs.HasKey("continuePlayerTotalScore"))
                {
                    ScoreInfoContinueProGame.PlayerTotalScore = PlayerPrefs.GetInt("continuePlayerTotalScore");
                    debug_loadPlayerPrefsData.DebugMessage("PLAYER TOTAL SCORE " + ScoreInfoContinueProGame.PlayerTotalScore);
                }
                if (PlayerPrefs.HasKey("continueProTotalScore"))
                {
                    ScoreInfoContinueProGame.ProTotalScore = PlayerPrefs.GetInt("continueProTotalScore");
                    debug_loadPlayerPrefsData.DebugMessage("PRO TOTAL SCORE " + ScoreInfoContinueProGame.ProTotalScore);
                }
                if (PlayerPrefs.HasKey("continueOpponentTotalScore"))
                {
                    ScoreInfoContinueProGame.OpponentTotalScore = PlayerPrefs.GetInt("continueOpponentTotalScore");
                    debug_loadPlayerPrefsData.DebugMessage("PRO TOTAL SCORE " + ScoreInfoContinueProGame.OpponentTotalScore);
                }
                if (PlayerPrefs.HasKey("continueDemoState"))
                {
                    ScoreInfoContinueProGame.DemoState = PlayerPrefs.GetInt("continueDemoState");
                    debug_loadPlayerPrefsData.DebugMessage("DEMO STATE " + ScoreInfoContinueProGame.DemoState);
                }
                if (PlayerPrefs.HasKey("continueBoardState"))
                {
                    ScoreInfoContinueProGame.BoardState = PlayerPrefs.GetString("continueBoardState");
                    debug_loadPlayerPrefsData.DebugMessage("BOARD STATE " + ScoreInfoContinueProGame.BoardState);
                }
            }
            if (ScoreInfoContinueAIGame.IfMatchToContinue)
            {
                debug_loadPlayerPrefsData.DebugMessage($"***** CONTINUE PLAYING AI *****");

                if (PlayerPrefs.HasKey("AIcontinuePlayingAs"))
                {
                    ScoreInfoContinueAIGame.PlayingAs = PlayerPrefs.GetInt("AIcontinuePlayingAs");
                    debug_loadPlayerPrefsData.DebugMessage("CONTINUE AS " + (ScoreInfoContinueAIGame.PlayingAs == 1 ? "PLAYER1" : "PLAYER2"));
                }
                if (PlayerPrefs.HasKey("AIcontinueMatchTitle"))
                {
                    ScoreInfoContinueAIGame.MatchTitle = PlayerPrefs.GetString("AIcontinueMatchTitle");
                    debug_loadPlayerPrefsData.DebugMessage("MATCH TITLE " + ScoreInfoContinueAIGame.MatchTitle);
                }
                if (PlayerPrefs.HasKey("AIcontinueMatchName"))
                {
                    ScoreInfoContinueAIGame.MatchID = PlayerPrefs.GetString("AIcontinueMatchName");
                    debug_loadPlayerPrefsData.DebugMessage("MATCH NAME " + ScoreInfoContinueAIGame.MatchID);
                }
                if (PlayerPrefs.HasKey("AIcontinueMatchIndex"))
                {
                    ScoreInfoContinueAIGame.MatchIndex = PlayerPrefs.GetInt("AIcontinueMatchIndex");
                    debug_loadPlayerPrefsData.DebugMessage("INDEX MATCH " + ScoreInfoContinueAIGame.MatchIndex);
                }
                if (PlayerPrefs.HasKey("AIcontinueGameIndex"))
                {
                    ScoreInfoContinueAIGame.GameIndex = PlayerPrefs.GetInt("AIcontinueGameIndex");
                    debug_loadPlayerPrefsData.DebugMessage("INDEX GAME " + ScoreInfoContinueAIGame.GameIndex);
                }
                if (PlayerPrefs.HasKey("AIcontinueTurnIndex"))
                {
                    ScoreInfoContinueAIGame.TurnIndex = PlayerPrefs.GetInt("AIcontinueTurnIndex");
                    debug_loadPlayerPrefsData.DebugMessage("INDEX TURN " + ScoreInfoContinueAIGame.TurnIndex);
                }
                if (PlayerPrefs.HasKey("AIcontinueMovesMade"))
                {
                    ScoreInfoContinueAIGame.MovesMade = PlayerPrefs.GetInt("AIcontinueMovesMade");
                    debug_loadPlayerPrefsData.DebugMessage("MOVES MADE " + ScoreInfoContinueAIGame.MovesMade);
                }
                if (PlayerPrefs.HasKey("AIcontinueMovesMatched"))
                {
                    ScoreInfoContinueAIGame.MovesMatched = PlayerPrefs.GetInt("AIcontinueMovesMatched");
                    debug_loadPlayerPrefsData.DebugMessage("MOVES MATCHED " + ScoreInfoContinueAIGame.MovesMatched);
                }
                if (PlayerPrefs.HasKey("AIcontinueAITopMatched"))
                {
                    ScoreInfoContinueAIGame.AITopMatched = PlayerPrefs.GetInt("AIcontinueAITopMatched");
                    debug_loadPlayerPrefsData.DebugMessage("AI MOVES MATCHED " + ScoreInfoContinueAIGame.AITopMatched);
                }
                if (PlayerPrefs.HasKey("AIcontinueProMovesMatched"))
                {
                    ScoreInfoContinueAIGame.ProMovesMatched = PlayerPrefs.GetInt("AIcontinueProMovesMatched");
                    debug_loadPlayerPrefsData.DebugMessage("PRO MOVES MATCHED " + ScoreInfoContinueAIGame.ProMovesMatched);
                }
                if (PlayerPrefs.HasKey("AIcontinueOpponentMovesMatched"))
                {
                    ScoreInfoContinueAIGame.ProMovesMatched = PlayerPrefs.GetInt("AIcontinueOpponentMovesMatched");
                    debug_loadPlayerPrefsData.DebugMessage("OPPONENT MOVES MATCHED " + ScoreInfoContinueAIGame.OpponentMovesMatched);
                }
                if (PlayerPrefs.HasKey("AIcontinuePlayerDoublingValue"))
                {
                    ScoreInfoContinueAIGame.PlayerDoublingValue = PlayerPrefs.GetInt("AIcontinuePlayerDoublingValue");
                    debug_loadPlayerPrefsData.DebugMessage("PLAYER DOUBLING VALUE " + ScoreInfoContinueAIGame.PlayerDoublingValue);
                }
                if (PlayerPrefs.HasKey("AIcontinueProDoublingValue"))
                {
                    ScoreInfoContinueAIGame.ProDoublingValue = PlayerPrefs.GetInt("AIcontinueProDoublingValue");
                    debug_loadPlayerPrefsData.DebugMessage("PRO DOUBLING VALUE " + ScoreInfoContinueAIGame.ProDoublingValue);
                }
                if (PlayerPrefs.HasKey("AIcontinuePlayerTotalScore"))
                {
                    ScoreInfoContinueAIGame.PlayerTotalScore = PlayerPrefs.GetInt("AIcontinuePlayerTotalScore");
                    debug_loadPlayerPrefsData.DebugMessage("PLAYER TOAL SCORE " + ScoreInfoContinueAIGame.PlayerTotalScore);
                }
                if (PlayerPrefs.HasKey("AIcontinueProTotalScore"))
                {
                    ScoreInfoContinueAIGame.ProTotalScore = PlayerPrefs.GetInt("AIcontinueProTotalScore");
                    debug_loadPlayerPrefsData.DebugMessage("PRO TOTAL SCORE " + ScoreInfoContinueAIGame.ProTotalScore);
                }
                if (PlayerPrefs.HasKey("AIcontinueIsPlayerTurn"))
                {
                    var turnComplete = PlayerPrefs.GetInt("AIcontinueIsPlayerTurn");
                    ScoreInfoContinueAIGame.IsPlayerTurn = (turnComplete == 1 ? true : false);
                    debug_loadPlayerPrefsData.DebugMessage("IS PLAYER TURN " + ScoreInfoContinueAIGame.IsPlayerTurn);
                }
                if (PlayerPrefs.HasKey("AIBoardState"))
                {
                    ScoreInfoContinueAIGame.BoardState = PlayerPrefs.GetString("AIBoardState");
                    debug_loadPlayerPrefsData.DebugMessage("BOARD STATE " + ScoreInfoContinueAIGame.BoardState);
                }
            }
        }

        internal void ResetAppData()
        {
            ScoreInfo.Reset();
            ScoreInfoPlayerVsPro.Reset();

            ScoreInfoContinueAIGame.Reset();
            ScoreInfoContinueProGame.Reset();

            PlayerPrefs.DeleteAll();

            // PLAYER APP STATE
            PlayerPrefs.SetString("aiUserID", Main.Instance.AIUserID);
            PlayerPrefs.SetInt("ifFirstPlaythrough", Main.Instance.IfFirstPlaythrough ? 1 : 0);

            // PLAYER SETTINGS
            int[] settings = Main.Instance.SettingsUI.GetPlayerSettings();
            PlayerPrefs.SetString("languageShortCode", Main.Instance.LanguageShortCode);
            PlayerPrefs.SetInt("playerColour", settings[0]);
            PlayerPrefs.SetInt("playingFromLHS", settings[1]);
            PlayerPrefs.SetInt("playbackSpeed", settings[2]);

            debug_loadPlayerPrefsData.DebugMessage("APP RESET...");

            PlayerPrefs.Save();

            LoadAppData();
        }

        // SCORE INFO

        public GameScoreInfo.ScoreInfo ScoreInfo;
        public GameScoreInfo.ScoreInfo ScoreInfoPlayerVsPro;

        public GameScoreInfo.ContinueGame ScoreInfoContinueProGame;
        public GameScoreInfo.ContinueGame ScoreInfoContinueAIGame;
    }
}