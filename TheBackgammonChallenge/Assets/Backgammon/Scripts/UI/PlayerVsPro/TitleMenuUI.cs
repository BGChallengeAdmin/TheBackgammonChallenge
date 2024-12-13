using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class TitleMenuUI : MonoBehaviour
    {
        [Header("BUTTONS")]
        [SerializeField] private TMP_Text TitleHeader;
        [SerializeField] private Button DownloadNewMatchesButton;
        [SerializeField] private Button PlayNewMatchButton;
        [SerializeField] private Button PlayRandomGameButton;
        [SerializeField] private Button ContinueGameButton;
        [SerializeField] private Button PlayDemoButton;
        [SerializeField] private Button StatisticsButton;
        [SerializeField] private Button ExitAppButton;

        [Header("BACKGROUND IMAGE")]
        [SerializeField] private Image _backgroundImage1;
        [SerializeField] private Image _backgroundImage2;

        [Header("DEBUG UI")]
        [SerializeField] private GameObject turnSelectContainer;
        [SerializeField] private Slider turnSelectSlider;
        [SerializeField] private InputField turnSelectInput;

        protected void Awake()
        {
            turnSelectSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
            turnSelectInput.onValueChanged.AddListener(delegate { OnInputValueChanged(); });
        }

        protected void OnEnable()
        {
            // DEBUG - PRINT ALL PLAYER NAMES

            //var scores = Main.Instance.PlayerScoresObj;
            //var dict = scores.GetPlayerScoresDict();

            //Debug.Log($"************************** NAMES");

            //var namesList = new List<string>();
            //var names = string.Empty;

            //foreach (var match in dict.Values)
            //{
            //    if (!namesList.Contains(match.player1Name)) namesList.Add(match.player1Name);
            //    if (!namesList.Contains(match.player2Name)) namesList.Add(match.player2Name);
            //}

            //foreach (var name in namesList)
            //    names += name + "\n";

            //Debug.Log(names);
            //Debug.Log($"************************** NAMES");

            // LEADERBOARD UPDATE 
            //Main.Instance.leaderboardObj.UpdateLeaderboardScores();

            if (Main.Instance.IfDemoIsInPlay)
            {
                Main.Instance.IfDemoIsInPlay = false;
            }

            if (Main.Instance.IfMatchedPlay)
            {
                Main.Instance.IfMatchedPlay = false;
            }

            if (Main.Instance.IfPlayerVsPro)
            {
                GameListUI.IndexGame = 0;
                MatchSelectUI.Match = null;

                Main.Instance.IfPlayerVsPro = false;
            }

            if (Main.Instance.IfPlayerVsAI)
            {
                GameListUI.IndexGame = 0;
                GameListUI.IndexTurn = 0;
                MatchSelectUI.Match = null;

                Main.Instance.IfPlayerVsAI = false;
            }

            if (Main.Instance.IfPlayerVsHistoricAI)
            {
                GameListUI.IndexGame = 0;
                GameListUI.IndexTurn = 0;
                MatchSelectUI.Match = null;

                Game.AIIfUsingHistoricDice = false;
                Game.AIHistoricPlayingAsPlayer1 = false;

                Main.Instance.IfPlayerVsHistoricAI = false;
            }

            if (Main.Instance.IfRandomSingleTurn)
            {
                GameListUI.IndexGame = 0;
                GameListUI.IndexTurn = 0;
                MatchSelectUI.Match = null;

                Main.Instance.IfRandomSingleTurn = false;
            }

            if (Main.Instance.IfSelectSpecificMove)
            {
                GameListUI.IndexGame = 0;
                GameListUI.IndexTurn = 0;
                MatchSelectUI.Match = null;

                Main.Instance.IfSelectSpecificMove = false;
            }

            ifClicked = false;
            ifSelectNewMatch = false;
            ifContinueMatch = false;
            ifPlayerVsPro = false;
            ifRandomGame = false;
            ifSelectSpecificMove = false;
            ifDownloadMatches = false;
            ifPlayDemo = false;
            ifViewStatistics = false;
            ifDailyChallenges = false;
            ifExitApp = false;

            Main.Instance.IfClickedContinuePro = false;
            Main.Instance.IfClickedContinueAI = false;

            ContinueGameButton.gameObject.SetActive(true);
            PlayRandomGameButton.gameObject.SetActive(true);
            //PlayRandomMoveButton.gameObject.SetActive(true);

            if (Main.Instance.IfFirstPlaythrough)
            {
                // JAMES - DISABLE BUTTONS
                Main.Instance.IfDemoIsInPlay = true;
                ContinueGameButton.interactable = false;
                PlayRandomGameButton.interactable = false;

                TitleHeader.text = "Please click the 'Select New Match' button.";
            }
            else
            {
                ContinueGameButton.interactable = true;
                PlayRandomGameButton.interactable = true;

                // NO GAME TO CONTINUE
                if (!Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame.IfMatchToContinue)
                    ContinueGameButton.interactable = false;

                TitleHeader.text = "Backgammon: Challenge";
            }

            // CONFIGURE BASED ON LANGUAGE

            LanguageScriptableObject languagesSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languagesSO != null)
            {
                TitleHeader.GetComponentInChildren<TMP_Text>().text = languagesSO.titleMenuTitle;

                DownloadNewMatchesButton.GetComponentInChildren<Text>().text = languagesSO.titleMenuDownload;
                PlayNewMatchButton.GetComponentInChildren<Text>().text = languagesSO.titleMenuNewMatch;
                PlayRandomGameButton.GetComponentInChildren<Text>().text = languagesSO.titleMenuRandomGame;
                ContinueGameButton.GetComponentInChildren<Text>().text = languagesSO.titleMenuContinueMatch;
                PlayDemoButton.GetComponentInChildren<Text>().text = languagesSO.titleMenuPlayDemo;
                StatisticsButton.GetComponentInChildren<Text>().text = languagesSO.titleMenuStatistics;
                ExitAppButton.GetComponentInChildren<Text>().text = languagesSO.Back;

                //TODO: SET BY LANGUAGE SO
                //TitleHeader.text = "Backgammon: Challenge";
                //TitleHeader.text = "Please click the 'Select New Match' button.";
            }

            //PlayRandomGameButton.interactable = false;
            PlayDemoButton.interactable = false;
            StatisticsButton.interactable = false;
            
            // DEBUG
            //if (Main.DEBUG_MODE)
            //{
            //    turnSelectContainer.gameObject.SetActive(true);

            //    MatchScoreData matchScoreData = Main.Instance.PlayerScoresObj.GetPlayerMatchScore(Main.Instance.ContinueMatchTitle + " " + Main.Instance.ContinueMatchName);
            //    if (matchScoreData != null)
            //    {
            //        if (Main.Instance.ContinueMatchTitle != "AI")
            //        {
            //            GameScoreData gameScoreData = matchScoreData.games[Main.Instance.ContinueGameIndex];

            //            maxNumberOfMoves = int.Parse(gameScoreData.numberOfTurns.ToString("F0"));
            //        }
            //    }
            //    else
            //        Debug.Log("*********** CONTINUE MATCH NULL");

            //    ResetSliderValues(maxNumberOfMoves);
            //    turnSelectSlider.value = Main.Instance.ContinueTurnIndex;

            //    Debug.Log($"TITLE MENU DEBUG: MOVES -> {maxNumberOfMoves} - TURN -> {Main.Instance.ContinueTurnIndex}");
            //}
        }

        protected void OnDestroy()
        {
            turnSelectSlider.onValueChanged.RemoveAllListeners();
            turnSelectInput.onValueChanged.RemoveAllListeners();
        }

        // ----------------------------------------- DEBUG HELPERS ------------------------------------------------------

        public void ResetSliderValues(float maxValue)
        {
            turnSelectSlider.maxValue = maxValue;
            turnSelectSlider.value = 0;
        }

        public void OnSliderValueChanged()
        {
            turnSelectInput.text = turnSelectSlider.value.ToString("F0");
            continueButtonIndexTurn = int.Parse(turnSelectSlider.value.ToString("F0"));
        }

        public void OnInputValueChanged()
        {
            if (turnSelectInput.text == string.Empty)
                turnSelectInput.text = "0";

            float inputTurn = float.Parse(turnSelectInput.text);

            if (inputTurn > maxNumberOfMoves)
            {
                inputTurn = maxNumberOfMoves;
                turnSelectInput.text = inputTurn.ToString("F0");
            }

            turnSelectSlider.value = inputTurn;
            continueButtonIndexTurn = int.Parse(inputTurn.ToString("F0"));
        }

        // ----------------------------------------- BUTTON CLICK FUNCTIONS --------------------------------------------------

        public void OnClickStartNewMatch()
        {
            ifClicked = true;
            ifSelectNewMatch = true;
            Main.Instance.IfPlayerVsPro = true;
            Main.Instance.IfMatchedPlay = true;
        }

        public void OnClickContinueMatch()
        {
            ifClicked = true;
            ifContinueMatch = true;

            Main.Instance.IfPlayerVsPro = true;
            Main.Instance.IfMatchedPlay = true;
        }

        public void OnClickedContinuePlayingPro()
        {
            ifClicked = true;
            ifContinueMatch = true;

            Main.Instance.IfClickedContinuePro = true;
            Main.Instance.IfPlayerVsPro = true;
            Main.Instance.IfMatchedPlay = true;
        }

        public void OnClickedContinuePopupAI()
        {
            ifContinueMatch = true;

            Main.Instance.IfClickedContinueAI = true;
            Main.Instance.IfPlayerVsPro = true;
            Main.Instance.IfMatchedPlay = true;
        }

        public void OnClickPlayRandomGame()
        {
            ifClicked = true;
            ifRandomGame = true;
            Main.Instance.IfMatchedPlay = true;

            SelectRandomMatchGame();
        }

        public void OnClickPlayRandomMove()
        {
            ifClicked = true;
            ifSelectSpecificMove = true;
            Main.Instance.IfSelectSpecificMove = true;
            Main.Instance.IfMatchedPlay = true;

            //SelectRandomSingleGameMove();
        }

        public void OnClickDownloadNewMatches()
        {
            ifClicked = true;
            ifDownloadMatches = true;
        }

        public void OnClickPlayDemo()
        {
            ifClicked = true;
            ifPlayDemo = true;
        }

        public void OnClickViewStats()
        {
            ifClicked = true;
            ifViewStatistics = true;
        }

        public void OnClickDailyChallenges()
        {
            ifClicked = true;
            ifDailyChallenges = true;
        }

        void SelectRandomMatchGame()
        {
            if (matchReplayDLCs == null)
            {
                matchReplayDLCs = FindObjectsOfType<Backgammon_Asset.MatchReplayDLC>();
            }
            if (matchReplayDLCs != null)
            {
                if (matchCount == 0) // first count number of matches...
                {
                    for (int i = 0; i < matchReplayDLCs.Length; ++i)
                    {
                        Backgammon_Asset.MatchReplayDLC matchReplayDLC = matchReplayDLCs[i];

                        // SKIP DEMO
                        if (matchReplayDLC.ID == "DEMO")
                            continue;

                        matchCount += matchReplayDLC.MatchCount;
                    }
                }
                if (matchCount > 0) // select a game from a match at random...
                {
                    int indexMatchToSelect = Random.Range(0, matchCount);
                    int indexMatch = 0;
                    for (int i = 0; i < matchReplayDLCs.Length; ++i)
                    {
                        Backgammon_Asset.MatchReplayDLC matchReplayDLC = matchReplayDLCs[i];

                        // SKIP DEMO
                        if (matchReplayDLC.ID == "DEMO" || matchReplayDLC.ID == "AI")
                            continue;

                        if (indexMatchToSelect < indexMatch + matchReplayDLC.MatchCount)
                        {
                            MatchSelectUI.Match = matchReplayDLC.Match(indexMatchToSelect - indexMatch);

                            break; // as found match...
                        }
                        indexMatch += matchReplayDLC.MatchCount;
                    }

                    GameListUI.IndexGame = Random.Range(0, MatchSelectUI.Match.GameCount);
                    GameListUI.IndexTurn = 0;

                    GameListUI._playingAs2D = MatchSelectUI.Match.Winner() == 1 ? Game2D.PlayingAs.PLAYER_1 : Game2D.PlayingAs.PLAYER_2;

                    Debug.Log($"MATCH: {MatchSelectUI.Match.name}");
                    Debug.Log("GAME: " + GameListUI.IndexGame);
                }
            }
        }

        // JAMES - CALCULATE AND MANAGE ALL RANDOM SINGLE MOVES
        public void SelectRandomSingleGameMove()
        {
            //DEBUG
            Debug.Log("SRT SELECTED");

            if (matchReplayDLCs == null)
            {
                matchReplayDLCs = FindObjectsOfType<Backgammon_Asset.MatchReplayDLC>();
            }
            if (matchReplayDLCs != null)
            {
                if (matchCount == 0) // first, count number of matches...
                {
                    for (int i = 0; i < matchReplayDLCs.Length; ++i)
                    {
                        Backgammon_Asset.MatchReplayDLC matchReplayDLC = matchReplayDLCs[i];
                        matchCount += matchReplayDLC.MatchCount;
                    }
                }
                if (matchCount > 0) // select a game from a match at random...
                {
                    int indexMatchToSelect = Random.Range(0, matchCount);
                    int indexMatch = 0;
                    int indexGame = 0;
                    int indexTurn = 0;
                    for (int i = 0; i < matchReplayDLCs.Length; ++i)
                    {
                        Backgammon_Asset.MatchReplayDLC matchReplayDLC = matchReplayDLCs[i];
                        if (indexMatchToSelect < (indexMatch + matchReplayDLC.MatchCount))
                        {
                            Backgammon_Asset.MatchData match = matchReplayDLC.Match(indexMatchToSelect - indexMatch);
                            MatchSelectUI.Match = match;

                            //TODO: CREATE AN ARRAY OF PREVIOUSLY PLAYED MATCH / GAME / TURN
                            //Test against previous 'n' matches / games / moves array - do not allow repeats within 'n'

                            // JAMES - Identify single game move to select - indexTurn (-2) - avoid playing the last turns
                            indexGame = Random.Range(0, match.GameCount);
                            int totalMovesForTheGame = match.GameDataArray[indexGame].NumberOfMoves;
                            indexTurn = Random.Range(0, totalMovesForTheGame - 2);
                            string move = match.GameDataArray[indexGame].GetPlayerMove(indexTurn);

                            // TODO: CHECK WHAT THE TURN LOOKS LIKE - IF CANNOT MOVE ...

                            Debug.Log("RANDOM MOVE: DLC  " + i + " " + matchReplayDLCs[i].name +
                                    "  MATCH: " + (indexMatchToSelect - indexMatch) +
                                    "  GAME: " + indexGame +
                                    "  MOVE: " + indexTurn + "  " + move);

                            break; // as move match...
                        }

                        indexMatch += matchReplayDLC.MatchCount;
                    }

                    //SET MATCH AND GAME - MOVE TO DISPLAY
                    GameListUI.IndexGame = indexGame;
                    GameListUI.IndexTurn = indexTurn;

                    // DEBUG - COMMENT THESE OUT FOR RADNOM GAME PLAY
                    //int DBDLC = 0;
                    //int DBDLCmatch = 0;
                    //int DBindexGame = 2;
                    //int DBindexTurn = 20;
                    //MatchSelectUI.match = matchReplayDLCs[DBDLC].Match(DBDLCmatch);
                    //MatchSelectUI.match = GameObject.Find("MatchReplayDLC_ (12)").GetComponentInChildren<Backgammon_Asset.MatchReplayDLC>().Match(DBDLCmatch);
                    //GameListUI.indexGame = DBindexGame;
                    //GameListUI.indexTurn = DBindexTurn;
                    //Debug.Log("DEBUG CONFIG:  " + DBDLC + "  MATCH: " + DBDLCmatch + "  GAME: " + DBindexGame + "  MOVE: " + DBindexTurn);
                    // DEBUG END

                    Main.Instance.IfRandomSingleTurn = true;
                }
            }
        }

        public void EnableDefaultBackground(bool enable)
        {
            _backgroundImage1.enabled = enable;
            _backgroundImage2.enabled = enable;
        }

        public void OnClickExitApp()
        {
            ifClicked = true;
            ifExitApp = true;
        }

        public void ResetMatchCount()
        {
            matchCount = 0;
        }

        [Header("FIELDS")]
        public bool ifClicked;
        public bool ifContinueMatch;
        public bool ifSelectNewMatch;
        public bool ifPlayerVsPro;
        public bool ifRandomGame;
        public bool ifSelectSpecificMove;
        public bool ifDownloadMatches;
        public bool ifPlayDemo;
        public bool ifViewStatistics;
        public bool ifDailyChallenges;
        public bool ifExitApp;

        public static int continueButtonIndexTurn = 0;
        public int maxNumberOfMoves = 0;

        private Backgammon_Asset.MatchReplayDLC[] matchReplayDLCs = null;
        int matchCount = 0;
    }
}