using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Backgammon
{
    public class MatchSelectUI : MonoBehaviour
    {
        [SerializeField] private Text matchSelectTitleText;
        [SerializeField] private Text matchSelectHeaderText;
        [SerializeField] private Text backButtonText;
        [SerializeField] private Text pageNumber = null;
        [SerializeField] private TMP_InputField sortedListInputField = null;

        [SerializeField] private MatchSelectOptionButton[] matchOptionButtons = null;
        [SerializeField] private GameObject matchStatsPopUpObj;
        [SerializeField] private GameObject matchStatsPopupContentContainer;

        [SerializeField] private GameObject demoMatchHighlight;

        [Header("BACKGROUND IMAGE")]
        [SerializeField] private Image _backgroundImage1;
        [SerializeField] private Image _backgroundImage2;

        private float timer = 0f;
        private int m_NUM_MATCHES_PER_PAGE;

        LanguageScriptableObject languageSO;

        protected void OnEnable()
        {
            ifBack = false;
            Match = null;

            optionCount = 0;
            indexOption01 = 0;
            matchReplayDLCs = null;

            sortedListInputField.text = "";
            sortedListTextUpper = "";
            sortedListTextLower = "";
            ifSortedList = false;

            m_NUM_MATCHES_PER_PAGE = matchOptionButtons.Length;

            BuildMenu();

            demoMatchHighlight.gameObject.SetActive(false);

            // JAMES - Manage Demo TEXT
            if (Main.Instance.IfDemoIsInPlay)
            {
                Main.Instance.IfDemoIsInPlay = false;
            }

            // CONFIGURE LANGUAGE BY REGION
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (Main.Instance.IfFirstPlaythrough)
            {
                Main.Instance.IfDemoIsInPlay = true;

                if (languageSO != null)
                {
                    matchSelectTitleText.text = languageSO.matchSelectTitle;
                }
                else matchSelectTitleText.text = "Tap on the 'DEMO' to see how the game plays";

                demoMatchHighlight.gameObject.SetActive(true);
                timer = 1.0f;
            }
            else
            {
                if (languageSO != null)
                {
                    matchSelectTitleText.text = languageSO.matchSelectTitle;
                    matchSelectHeaderText.text = languageSO.matchSelectHeader;
                }
                else matchSelectTitleText.text = "Select a match below";
            }

            if (languageSO != null)
            {
                sortedListInputField.placeholder.GetComponentInChildren<TMP_Text>().text = languageSO.matchSelectSearch;
                backButtonText.text = languageSO.Back;
            }
        }

        protected void Update()
        {
            // JAMES - HIGHLIGHT UPDATE - ONLY ON FIRST PAGE - I.E. DEMO IS AT TOP
            if (Main.Instance.IfFirstPlaythrough && indexOption01 < m_NUM_MATCHES_PER_PAGE)
            {
                if (demoMatchHighlight.gameObject.activeInHierarchy)
                {
                    timer -= Time.deltaTime;

                    if (timer < 0)
                        timer = 1.0f;

                    demoMatchHighlight.gameObject.transform.localScale = new Vector3(1 + (timer / 30), 1 + (timer / 30), 1);
                }
                else
                {
                    demoMatchHighlight.gameObject.SetActive(true);
                }
            }
            else
            {
                demoMatchHighlight.gameObject.SetActive(false);
            }
        }

        public void OnClickOption01() { doOption(1); }

        public void OnClickOption02() { doOption(2); }

        public void OnClickOption03() { doOption(3); }

        public void OnClickOption04() { doOption(4); }

        public void OnClickOption05() { doOption(5); }

        public void OnClickOption06() { doOption(6); }

        public void OnClickOption07() { doOption(7); }

        public void OnClickOption08() { doOption(8); }

        public void OnClickOption09() { doOption(9); }

        public void OnInputFieldValueChanged(string inputText)
        {
            if (inputText == "")
            {
                ifSortedList = false;

                sortedListTextUpper = "";
                sortedListTextLower = "";
            }
            else
            {
                ifSortedList = true;

                sortedListTextUpper = inputText.Substring(0, 1).ToUpper() + inputText.Substring(1).ToLower();
                sortedListTextLower = inputText.Substring(0, 1).ToLower() + inputText.Substring(1).ToLower();
            }

            // ENABLE DEBUG TOOLKIT
            if (inputText == "DEBUG_TOOLKIT_ON") Main.Instance.IfUsingDebugToolkit = true;
            else if (inputText == "DEBUG_TOOLKIT_OFF") Main.Instance.IfUsingDebugToolkit = false;

            matchReplayDLCs = null;

            // RESET TO START OF LIST
            indexOption01 = 0;

            BuildMenu();
        }

        public void OnClickClearInputField()
        {
            sortedListInputField.Select();
            sortedListInputField.text = "";
        }

        public void OnClickPrev()
        {
            if (indexOption01 >= m_NUM_MATCHES_PER_PAGE)
            {
                indexOption01 -= m_NUM_MATCHES_PER_PAGE;
                BuildMenu();
            }
        }

        public void OnClickNext()
        {
            if (indexOption01 + m_NUM_MATCHES_PER_PAGE < optionCount)
            {
                indexOption01 += m_NUM_MATCHES_PER_PAGE;
                BuildMenu();
            }
        }

        public void OnBack()
        {
            ifBack = true;
        }

        private void doOption(int option)
        {
            int d = option - 1;
            if (matches[d] == null)
                return; // empty option
            Match = matches[d];
            GameListUI.IndexGame = 0;
            GameListUI.IndexTurn = 0;

            if (Match.Title == "DEMO")
            {
                Main.Instance.IfDemoIsInPlay = true;
                Debug.Log("DEMO MATCH SELECTED");
            }
        }

        public void OnMatchStatsButtonClick(int option)
        {
            matchStatsPopUpObj.gameObject.SetActive(true);

            var match = matches[option];
            var matchScore = Main.Instance.PlayerScoresObj.GetPlayerMatchScore(match.Title + " " + match.ID);

            var lastHighestGamePlayedP1 = matchScore.lastHighestGamePlayedP1;
            var lastHighestGamePlayedP2 = matchScore.lastHighestGamePlayedP2;
            var p1IndexMovesSeen = 0f;
            var p2IndexMovesSeen = 0f;
            var p1MovesMade = 0f;
            var p2MovesMade = 0f;
            var p1MovesMatched = 0f;
            var p2MovesMatched = 0f;
            var p1AIMovesMatched = 0f;
            var p2AIMovesMatched = 0f;

            for (int game = 0; game < matchScore.games.Length; game++)
            {
                var gameScore = matchScore.games[game];

                p1IndexMovesSeen += gameScore.player1BestIndexTurn;
                p2IndexMovesSeen += gameScore.player2BestIndexTurn;
                p1MovesMade += gameScore.player1BestMovesMade;
                p2MovesMade += gameScore.player2BestMovesMade;
                p1MovesMatched += gameScore.player1BestMovesMatched;
                p2MovesMatched += gameScore.player2BestMovesMatched;
                p1AIMovesMatched += gameScore.player1BestTopMatched;
                p2AIMovesMatched += gameScore.player2BestTopMatched;
            }

            float p1MatchCompletion = Mathf.Round(p1IndexMovesSeen / matchScore.totalNumberOfTurns * 100f);
            p1MatchCompletion = float.IsNaN(p1MatchCompletion) ? 0 : p1MatchCompletion;

            float p2MatchCompletion = Mathf.Round(p2IndexMovesSeen / matchScore.totalNumberOfTurns * 100f);
            p2MatchCompletion = float.IsNaN(p2MatchCompletion) ? 0 : p2MatchCompletion;

            float p1CurrentMatchPer = Mathf.Round(p1MovesMatched / p1MovesMade * 100f);
            p1CurrentMatchPer = float.IsNaN(p1CurrentMatchPer) ? 0 : p1CurrentMatchPer;

            float p2CurrentMatchPer = Mathf.Round(p2MovesMatched / p2MovesMade * 100f);
            p2CurrentMatchPer = float.IsNaN(p2CurrentMatchPer) ? 0 : p2CurrentMatchPer;

            float p1AICurrentMatchPer = Mathf.Round(p1AIMovesMatched / p1MovesMade * 100f);
            p1AICurrentMatchPer = float.IsNaN(p1AICurrentMatchPer) ? 0 : p1AICurrentMatchPer;

            float p2AICurrentMatchPer = Mathf.Round(p2AIMovesMatched / p2MovesMade * 100f);
            p2AICurrentMatchPer = float.IsNaN(p2AICurrentMatchPer) ? 0 : p2AICurrentMatchPer;

            var lastGamePlayed = matchStatsPopupContentContainer.transform.Find("LastGamePlayed");
            var lastGamePlayedAs = matchStatsPopupContentContainer.transform.Find("LastGamePlayedAs");
            var playerNames = matchStatsPopupContentContainer.transform.Find("PlayerNames");
            var lastHighestGame = matchStatsPopupContentContainer.transform.Find("LastHighestGame");
            var matchCompletion = matchStatsPopupContentContainer.transform.Find("MatchCompletion");
            var movesMatchedPercent = matchStatsPopupContentContainer.transform.Find("MatchPercentage");
            var AIMovesMatchedPercent = matchStatsPopupContentContainer.transform.Find("AIMatchPercentage");

            lastGamePlayed.transform.GetComponentInChildren<Text>().text =
                languageSO != null ? languageSO.matchStatsLastGamePlayed : "Last game played " + matchScore.lastGamePlayed + "/" + matchScore.gameScoresDict.Count;
            lastGamePlayedAs.transform.GetComponentInChildren<Text>().text =
                languageSO != null ? languageSO.matchStatsPlayingAs : "Playing as " + (matchScore.lastPlayedAs == 1 ? match.Player1 : match.Player2);

            playerNames.transform.Find("1").transform.GetComponentInChildren<Text>().text = match.Player1;
            playerNames.transform.Find("2").transform.GetComponentInChildren<Text>().text = match.Player2;

            lastHighestGame.transform.Find("1").transform.GetComponentInChildren<Text>().text = lastHighestGamePlayedP1.ToString();
            lastHighestGame.transform.Find("2").transform.GetComponentInChildren<Text>().text = lastHighestGamePlayedP2.ToString();
            lastHighestGame.transform.Find("text").transform.GetComponentInChildren<Text>().text =
                languageSO != null ? languageSO.matchStatsHighestGame : "HIGHEST GAME";

            matchCompletion.transform.Find("1").transform.GetComponentInChildren<Text>().text = p1MatchCompletion.ToString("F1") + "%";
            matchCompletion.transform.Find("2").transform.GetComponentInChildren<Text>().text = p2MatchCompletion.ToString("F1") + "%";
            matchCompletion.transform.Find("text").transform.GetComponentInChildren<Text>().text =
                languageSO != null ? languageSO.matchStatsOverallCompletion : "OVERALL COMPLETION";

            movesMatchedPercent.transform.Find("1").transform.GetComponentInChildren<Text>().text = p1CurrentMatchPer.ToString("F0") + "%";
            movesMatchedPercent.transform.Find("2").transform.GetComponentInChildren<Text>().text = p2CurrentMatchPer.ToString("F0") + "%";
            movesMatchedPercent.transform.Find("text").transform.GetComponentInChildren<Text>().text =
                languageSO != null ? languageSO.matchStatsMovesMatched : "MOVES MATCHED";

            AIMovesMatchedPercent.transform.Find("1").transform.GetComponentInChildren<Text>().text = p1AICurrentMatchPer.ToString("F0") + "%";
            AIMovesMatchedPercent.transform.Find("2").transform.GetComponentInChildren<Text>().text = p2AICurrentMatchPer.ToString("F0") + "%";
            AIMovesMatchedPercent.transform.Find("text").transform.GetComponentInChildren<Text>().text =
                languageSO != null ? languageSO.matchStatsAIMovesMatched : "A.I. MOVES MATCHED";

            if (languageSO != null)
            {
                backButtonText.text = languageSO.Close;
            }
        }

        public void OnClickCloseMatchStatsButton()
        {
            matchStatsPopUpObj.gameObject.SetActive(false);
        }

        private void BuildMenu()
        {
            for (int i = 0; i < m_NUM_MATCHES_PER_PAGE; ++i)
            {
                //options[i].text = string.Empty;

                matchOptionButtons[i].SetMatchTitle("");
                matchOptionButtons[i].SetPlayer1NameAndRanking("");
                matchOptionButtons[i].SetPlayer2NameAndRanking("");
                matchOptionButtons[i].DefaultFlagToNone();
                matchOptionButtons[i].EnableMatchStatsButton(false);

                matches[i] = null;
                matchIndexes[i] = 0;
                //matchSelectButtons[i].gameObject.SetActive(false);
            }

            if (matchReplayDLCs == null)
            {
                matchReplayDLCList = new List<Backgammon_Asset.MatchReplayDLC>();
                matchReplayDLCs = FindObjectsOfType<Backgammon_Asset.MatchReplayDLC>();

                foreach (Backgammon_Asset.MatchReplayDLC match in matchReplayDLCs)
                {
                    // DO NOT INCLUDE 'AI' MATCH IN OPTIONS
                    if (match.ID == "AI") continue;

                    if (ifSortedList && (!match.Match(0).name.Contains(sortedListTextUpper)
                        && !match.Match(0).name.Contains(sortedListTextLower))) continue;

                    matchReplayDLCList.Add(match);
                }

                // NOTE - ID SHOULD BE NUMBERED - 'DEMO' WILL ALWAYS BE LAST - I.E. TOP OF THE LIST
                matchReplayDLCList = matchReplayDLCList.OrderByDescending(replayDLC => replayDLC.ID).ToList();
                matchReplayDLCs = matchReplayDLCList.ToArray();
            }

            var _moves10 = 0;
            var _moves15 = 0;
            var _moves20 = 0;
            var _moves30 = 0;
            var _moves40 = 0;
            var _totalGames = 0;

            if (matchReplayDLCs != null)
            {
                if (!Main.Instance.IfFirstPlaythrough)
                {
                    // DEBUG DEFAULT COLOUR
                    //Color defaultColour = options[0].color;

                    MatchScoreData matchScore = null;
                    int indexReplay = 0;
                    for (int matchDLCListCounter = 0; matchDLCListCounter < matchReplayDLCs.Length; ++matchDLCListCounter)
                    {
                        Backgammon_Asset.MatchReplayDLC matchReplayDLC = matchReplayDLCs[matchDLCListCounter];
                        for (int matchInDLCCounter = 0; matchInDLCCounter < matchReplayDLC.MatchCount; ++matchInDLCCounter)
                        {
                            Backgammon_Asset.MatchData match = matchReplayDLC.Match(matchInDLCCounter);

                            _totalGames += match.GameCount;

                            for (var game = 0; game < match.GameDataArray.Length; game++)
                            {
                                var _moves = match.GameDataArray[game].Moves.Length;

                                if (_moves >= 40) _moves40++;
                                else if (_moves >= 30) _moves30++;
                                else if (_moves >= 20) _moves20++;
                                else if (_moves >= 15) _moves15++;
                                else if (_moves < 15) _moves10++;
                            }

                            int matchOptionCounter = indexReplay - indexOption01;
                            if (matchOptionCounter >= 0 && matchOptionCounter < m_NUM_MATCHES_PER_PAGE)
                            {
                                if (!match.Round.Contains('0') && match.Round != "Unknown")
                                    matchOptionButtons[matchOptionCounter].SetMatchTitle(match.Event + " " + match.Round);
                                else
                                    matchOptionButtons[matchOptionCounter].SetMatchTitle(match.Event);

                                matchOptionButtons[matchOptionCounter].SetPlayer1NameAndRanking(match.Player1, match.Winner() == 1);
                                matchOptionButtons[matchOptionCounter].SetPlayer2NameAndRanking(match.Player2, match.Winner() == 2);

                                matchOptionButtons[matchOptionCounter].SetPlayer1Flag(GetPlayerFlagSprite(match.Player1));
                                matchOptionButtons[matchOptionCounter].SetPlayer2Flag(GetPlayerFlagSprite(match.Player2));

                                //Debug.Log($"ROUND {match.Round}");

                                matches[matchOptionCounter] = match;

                                if (match.Title != "DEMO")
                                {
                                    // HAS USER PLAYED ANY PART OF MATCH
                                    matchScore = Main.Instance.PlayerScoresObj.GetPlayerMatchScore(match.Title + " " + match.ID);

                                    if (matchScore != null && matchScore.lastGamePlayed > 0)
                                    {
                                        //matchSelectButtons[d].gameObject.SetActive(true);

                                        // TODO : MATCH STATS BUTTON
                                        //matchOptionButtons[matchOptionCounter].EnableMatchStatsButton(true);

                                        if (languageSO != null)
                                            matchOptionButtons[matchOptionCounter].SetMatchStatsButtonText(languageSO.matchSelectMatchStats);
                                    }
                                }

                                //if (Main.DEBUG_MODE)
                                //{
                                //    if (matchScore == null)
                                //    {
                                //        //Debug.Log("NULL ID " + options[d].text);
                                //        Debug.Log("NULL ID " + optionButtons[matchOptionCounter].GetMatchTitle());
                                //    }
                                //}

                                matchIndexes[matchOptionCounter] = matchInDLCCounter;
                            }
                            ++indexReplay;
                        }
                    }
                    optionCount = indexReplay;
                }
                else
                {
                    Backgammon_Asset.MatchData match = matchReplayDLCs[0].Match(0);

                    matchOptionButtons[0].SetMatchTitle(match.Event + " " + match.Round);
                    matchOptionButtons[0].SetPlayer1NameAndRanking(match.Player1);
                    matchOptionButtons[0].SetPlayer2NameAndRanking(match.Player2);
                    matchOptionButtons[0].SetPlayer1Flag(GetPlayerFlagSprite(match.Player1));
                    matchOptionButtons[0].SetPlayer2Flag(GetPlayerFlagSprite(match.Player2));

                    matches[0] = match;
                    matchIndexes[0] = 0;
                }
            }

            //Debug.Log($"TOTAL AVAILABLE MATCHES {matchReplayDLCs.Length}");

            //Debug.Log($"TOTAL MOVES: {_totalGames}, 40+ {_moves40}, 30+ {_moves30}, 20+ {_moves20}, 15+ {_moves15}, 10+ {_moves10}");

            pageNumber.text = "Page " + (optionCount > 0 ? (indexOption01 / m_NUM_MATCHES_PER_PAGE + 1).ToString() + "/" + ((optionCount / m_NUM_MATCHES_PER_PAGE) + ((optionCount % m_NUM_MATCHES_PER_PAGE) > 0 ? 1 : 0)).ToString() : "1/1");
        }

        private Sprite GetPlayerFlagSprite(string playerName)
        {
            return Main.Instance.WorldRegionObj.GetFlagByCountryName(Main.Instance.WorldInfoObj.GetPlayerInfoByName(playerName).nationality);
        }

        public static void SetContinueMatchByID(string continueMatchID)
        {
            matchReplayDLCs = FindObjectsOfType<Backgammon_Asset.MatchReplayDLC>();

            foreach (Backgammon_Asset.MatchReplayDLC _match in matchReplayDLCs)
            {
                foreach (Backgammon_Asset.MatchData data in _match.MatchData)
                {
                    if (continueMatchID == data.ID)
                    {
                        Match = data;
                        return;
                    }
                }
            }
        }

        public static void SetMatch(Backgammon_Asset.MatchData match)
        {
            Match = match;
        }
        public static void SetMatch_3D(Backgammon_Asset.MatchData match)
        {
            Match_3D = match;
        }

        public void EnableDefaultBackground(bool enable)
        {
            _backgroundImage1.enabled = enable;
            _backgroundImage2.enabled = enable;
        }

        public bool ifBack;
        public static Backgammon_Asset.MatchData Match = null;
        public static Backgammon_Asset.MatchData Match_3D = null;

        private bool ifSortedList = false;
        private string sortedListTextUpper = "";
        private string sortedListTextLower = "";
        private int optionCount = 0;
        private int indexOption01 = 0;
        private List<Backgammon_Asset.MatchReplayDLC> matchReplayDLCList = null;
        private static Backgammon_Asset.MatchReplayDLC[] matchReplayDLCs = null;
        private Backgammon_Asset.MatchData[] matches = new Backgammon_Asset.MatchData[8];
        private int[] matchIndexes = new int[8];
    }
}