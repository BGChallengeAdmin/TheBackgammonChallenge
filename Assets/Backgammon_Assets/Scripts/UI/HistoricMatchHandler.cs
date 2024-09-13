using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Backgammon_Asset
{
    public class HistoricMatchHandler : MonoBehaviour
    {
        //[SerializeField]
        //private MatchReplayDLC PrefabMatchReplayDLC = null;
        //private List<MatchReplayDLC> replayDLCList = new List<MatchReplayDLC>();
        //private MatchReplayDLC[] replayDLCArray = null;

        private static string textFolder = null;
        private static string buildFolder = null;

        private bool buildingAssets = false;
        private bool buildConfigured = false;
        private bool buildComplete = false;
        private bool buildFailed = false;

        private static bool usingWorldChampionship = false;

        private void Start()
        {
            BuildingAssets = false;
        }

        public void Building(bool building)
        {
            BuildingAssets = building;
        }

        public static void ConstructMatchAssets()
        {
#if UNITY_EDITOR
            // NOTE - IF ASSET ALREADY EXISTS IT WILL BE OVERWRITTEN AND ALL GAME DATA RECREATED

            int matchCounter = 0;
            float validationPercent = 0f;

            // NOTE - 0 AND 25 ARE BAR STATES FOR PLAYER2 AND PLAYER1 RESPECTIVELY - PLAYER1 (+) PLAYER2 (-)
            // [26] - PLAYER1 HOME [27] - PLAYER2 HOME
            int p1Bar = 25, p2Bar = 0, p1Home = 26, p2Home = 27;
            int minP1Length = 10;

            Debug.Log("Construct Matches");

            foreach (string file in Directory.GetFiles(TextFolder, "*.txt"))
            {
                //FILES TO SKIP
                //if (file == TextFolder + "Rd1-3 Kazuhiro Saito - Kiyokazu Nishikawa_1642518860.txt")
                //    continue;

                //SPECIFIC FILES
                //if (file != TextFolder + "goldbach7808-fairytails-2013Nov231937_1385205480.txt" &&
                //    file != TextFolder + "goldbach7808-fairytails-2013Nov232002_1385205543.txt") continue;

                //if (file != TextFolder + "ipetani - fazlimtiger 13pt Backgammon Studio 2022_06_11 13_34_45_1654962121.txt") continue;

                Debug.Log("********* MATCH " + ++matchCounter + " *********");
                Debug.Log(file);

                MatchData matchData = ScriptableObject.CreateInstance("MatchData") as MatchData;
                GameData gameData = null;
                List<MatchData> matchDataList = new List<MatchData>();

                string matchName = "Unknown";
                string matchID = "123456";
                string fallBackID = "";
                string eventName = "Unknown";
                string round = "Unknown";
                int points = 1;
                string player1Surname = "";
                string player1 = "X";
                string player2Surname = "";
                string player2 = "X";
                string crawford = "false";
                bool crawfordInPlay = false;
                List<GameData> gameDataList = new List<GameData>();

                string gameName = "Unknown";
                int p1PtsStart = 0;
                int p2PtsStart = 0;
                int p1PtsEnd = 0;
                int p2PtsEnd = 0;
                List<string> gameMovesList = new List<string>();

                int[] boardState = null;
                float boardValidation = 0f;

                ////GET SINGLE MATCH
                //string path = TextFolder + "Kiyokazu Nishikawa-Daisuke Katagami 25 point match 2022-05-08_1652083312.txt";
                //using StreamReader sr = new StreamReader(path, Encoding.GetEncoding("iso-8859-1"));
                // UTF ENCODING: http://www.architectshack.com/TextFileEncodingDetector.ashx

                using StreamReader sr = new StreamReader(file, Encoding.GetEncoding("iso-8859-1"));
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();

                    switch (line)
                    {
                        case string w when w.Contains("Wins"):
                            {
                                // POINTS AT END = POINTS AT START + WINS FOR X
                                p1PtsEnd = p1PtsStart;
                                p2PtsEnd = p2PtsStart;

                                string[] winsFor = line.Split(' ');
                                string pointsWon = "";
                                bool gameWon = false;
                                int gameLostCount = 0;

                                for (int wf = 0; wf < winsFor.Length; wf++)
                                {
                                    string s = winsFor[wf];

                                    // CHECK FOR PLAYER 1 CONDITIONS
                                    if (s == "Drops")
                                    {
                                        gameWon = true;
                                        gameMovesList.Add("Drops");
                                    }
                                    if (s.Contains("Concedes"))
                                    {
                                        gameWon = true;
                                        gameMovesList.Add("Concedes");
                                    }
                                    if (s.Contains('?') && !gameWon)
                                    {
                                        gameWon = true;
                                        gameMovesList.Add("Concedes");
                                    }
                                    if (s.Contains('/'))
                                    {
                                        // FUDGE FACTOR - IGNORE LAST MOVE
                                        gameWon = true;
                                        gameMovesList.Add("Concedes");
                                    }

                                    if (s == "Wins")
                                    {
                                        gameWon = true;
                                    }
                                    if (s != string.Empty)
                                    {
                                        gameLostCount = 0;
                                    }
                                    if (s == string.Empty && !gameWon && gameLostCount++ <= (minP1Length - 2))
                                    {
                                        if (gameLostCount >= (minP1Length - 2))
                                        {
                                            gameWon = true;
                                            gameMovesList.Add(":");
                                        }
                                    }

                                    // GAME WON FOR POINTS - SCENARIO WHERE PLAYER 1 HAS WON ON PREVIOUS LINE
                                    if (s != string.Empty && !s.Contains(')') && !s.Contains(':') && !s.Contains('/') && !s.Contains("Wins") && !s.Contains("point") && !s.Contains("and")
                                         && !s.Contains("the") && !s.Contains("match") && !s.Contains("Concedes") && !s.Contains("?") && !s.Contains("Drops"))
                                    {
                                        // POSITION OF THE ONLY VALUE
                                        pointsWon = s;

                                        //Debug.Log($"WINS FOR {pointsWon}");

                                        if (wf < minP1Length) p1PtsEnd += int.Parse(pointsWon);
                                        else p2PtsEnd += int.Parse(pointsWon);
                                    }
                                }

                                gameMovesList.Add("Wins " + pointsWon + " point");

                                gameData.GameConstructor(p1PtsStart, p2PtsStart, gameMovesList.ToArray(), p1PtsEnd, p2PtsEnd);
                                gameDataList.Add(gameData);

                                int validate = 0;
                                int counters = 0;
                                foreach (int pointValue in boardState)
                                {
                                    validate += pointValue;
                                    counters += Math.Abs(pointValue);
                                }

                                boardValidation += validate == 0 ? 1 : 0;
                                validationPercent = boardValidation / float.Parse(gameName.Split(' ').LastOrDefault());

                                // NOTE - PLAYER1 (+) PLAYER2 (-) ==> EQUAL NUMBER OF COUNTERS == 0 ==> BOARD IS VALID
                                //Debug.Log("VALIDATE GAME " + (validate == 0) + " " + validate + " " + counters);
                                //Debug.Log("PERCENT " + (validationPercent * 100) + "%");

                                if (counters != 30)
                                    Debug.Log("********** ERROR ********** " + counters);

                                if (gameData != null)
                                {
                                    // TEST IF FOLDER ALRADY EXISTS
                                    if (Directory.Exists(BuildFolder + matchName))
                                    {
                                        AssetDatabase.CreateAsset(gameData, BuildFolder + matchName + "/" + gameName + ".asset");
                                        AssetDatabase.Refresh();
                                    }
                                }
                                else Debug.Log($"GAME DATA NULL");

                                gameData = null;
                            }
                            break;
                        case string a when a.Contains("ID"):
                            {
                                var result = from Match match in Regex.Matches(line, "\"([^\"]*)\"")
                                             select match.ToString();
                                matchID = result.FirstOrDefault().Trim('"');

                                Debug.Log("ID " + matchID + " " + matchID.Length);
                            }
                            break;
                        case string b when b.Contains("Event"):
                            {
                                var result = from Match match in Regex.Matches(line, "\"([^\"]*)\"")
                                             select match.ToString();

                                // MATCH DOES NOT CONTAIN AN ID
                                if (matchID == "123456")
                                {
                                    if (line.Contains("Date"))
                                    {
                                        string[] _date = result.FirstOrDefault().Trim('"').Split('.');

                                        fallBackID = string.Empty;
                                        fallBackID = _date[1] + _date[2];

                                        break;
                                    }
                                    if (line.Contains("Time"))
                                    {
                                        //string[] _date = result.FirstOrDefault().Trim('"').Split('.');
                                        //fallBackID += _date[0] + _date[1];

                                        break;
                                    }
                                }
                                else if (line.Contains("Date") || line.Contains("Time"))
                                    break;

                                eventName = result.FirstOrDefault().Trim('"');

                                Debug.Log("ROUND " + eventName);
                            }
                            break;
                        case string c when c.Contains("Round"):
                            {
                                var result = from Match match in Regex.Matches(line, "\"([^\"]*)\"")
                                             select match.ToString();
                                round = result.FirstOrDefault().Trim('"');
                            }
                            break;
                        case string d when d.Contains("match"):
                            {
                                // FIND MATCH PLAYED FOR HOW MANY POINTS

                                // AS IS PART OF A MOVE
                                if (!line.Contains("point"))
                                    break;
                                // IS DIFFERENT END MATCH NOTATION
                                if (line.Contains("Wins"))
                                    break;

                                string[] playedFor = line.Split(' ');

                                points = int.Parse(playedFor[0] == string.Empty ? playedFor[1] : playedFor[0]);
                                
                                matchName = eventName + " " + player1 + " vs " + player2 + " " + round;

                                // AFTER POINTS CONSTRUCT THE MATCH AND SUB-FOLDER FOR GAMES
                                if (matchName.Length > 100) matchName = eventName + " " + player1Surname + " " + player2Surname +" " + round;
                                
                                matchName = matchName.TrimEnd();

                                // TEST IF FOLDER ALRADY EXISTS
                                if (!Directory.Exists(BuildFolder + matchName))
                                    Directory.CreateDirectory(BuildFolder + matchName);

                                AssetDatabase.Refresh();
                            }
                            break;
                        case string e when e.Contains("Player"):
                            {
                                if (line.Contains("Elo"))
                                    break;

                                string[] playerDetails;
                                var result = from Match match in Regex.Matches(line, "\"([^\"]*)\"")
                                             select match.ToString();
                                playerDetails = result.FirstOrDefault().Trim('"').Split(' ');

                                if (line.Contains("1"))
                                {
                                    player1 = playerDetails[0];

                                    if (playerDetails.Length > 1)
                                    {
                                        for (int i1 = 1; i1 < playerDetails.Length; i1++)
                                        {
                                            player1 += " " + playerDetails[i1];
                                        }
                                        player1Surname = playerDetails[playerDetails.Length - 1];
                                    }
                                }
                                else if (line.Contains("2"))
                                {
                                    player2 = playerDetails[0];
                                    if (playerDetails.Length > 1)
                                    {
                                        for (int i2 = 1; i2 < playerDetails.Length; i2++)
                                        {
                                            player2 += " " + playerDetails[i2];
                                        }
                                        player2Surname = playerDetails[playerDetails.Length - 1];
                                    }
                                }
                            }
                            break;
                        case string f when f.Contains("Crawford"):
                            {
                                crawford = "true";

                                var result = from Match match in Regex.Matches(line, "\"([^\"]*)\"")
                                             select match.ToString();
                                if (result.FirstOrDefault().Trim('"') != "On")
                                    crawford = "false";
                            }
                            break;
                        case string g when g.Contains("Game"):
                            {
                                // CONSTRUCT NEW GAME
                                if (gameData == null)
                                {
                                    gameData = ScriptableObject.CreateInstance("GameData") as GameData;
                                    gameMovesList.Clear();
                                }

                                gameName = line.Trim(' ');

                                boardState = new int[] { 0, -2, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, -5, 5, 0, 0, 0, -3, 0, -5, 0, 0, 0, 0, 2, 0, 0, 0 };

                                crawfordInPlay = true;

                                //Debug.Log("******* GAME ******* " + gameName);
                            }
                            break;
                        case string m when m.Contains(":"):
                            {
                                // MUST BE FIRST LINE OF GAME
                                if (!line.Contains(')'))
                                {
                                    string[] pointsAtStart = line.Split(':');

                                    p1PtsStart = int.Parse(pointsAtStart[1].Substring(0, 5).Trim(' '));
                                    p2PtsStart = int.Parse(pointsAtStart[2].Trim(' '));

                                    break;
                                }

                                // FULL MOVES
                                int turnNumber = 0;
                                string playerMove = "";

                                // NOTE - NEVER HAVE MORE THAN 2 ' ' BETWEEN ENTRIES - ' ' >= 2 IS THE SECOND PLAYER
                                // WORST CASE FORMATTING WOULD LEAVE NO SPACE - TEST FOR PLAYER 1 DICE AND 2 ' '
                                // NOTE - FORMATTING ALLOWS FOR 7 SPACES BEFORE 2 ' ' - I.E. DOUBLES / TAKES / CONCEDES / NO MOVE
                                int index = 0, blankCounter = 0;
                                char[] chars = line.ToArray();

                                string debugText = string.Empty;

                                foreach (char c in chars)
                                {
                                    debugText += c;

                                    if (index > minP1Length)
                                    {
                                        // TEST IF LINE DATA FOR P1 IS EXTREMELY LONG
                                        if (c == ':')
                                        {
                                            index -= 3;
                                            break;
                                        }
                                        else if (blankCounter >= 2) break;
                                    }

                                    if (c == ' ') blankCounter++;
                                    else blankCounter = 0;

                                    index++;
                                }

                                // SPLIT THE STRING FOR PLAYER1 AND PLAYER2 MOVES
                                string[] p1Moves = line.Substring(0, index).TrimStart(' ').TrimEnd(' ').Split(' ');
                                string[] p2Moves = line.Substring(index, line.Length - index).TrimStart(' ').TrimEnd(' ').Split(' ');

                                int dice1 = 0, dice2 = 0;

                                // ---------- PLAYER 1 MOVES ---------- 

                                foreach (string movePart in p1Moves)
                                {
                                    if (movePart != string.Empty)
                                    {
                                        // FIRST MOVE CONTAINS TURN NUMBER
                                        if (!movePart.Contains('(') && movePart.Contains(')'))
                                        {
                                            turnNumber = int.Parse(movePart.Trim(')'));
                                            continue;
                                        }

                                        // DICE ROLLS
                                        if (movePart.Contains(':'))
                                        {
                                            dice1 = int.Parse(movePart.Substring(0, 1));
                                            dice2 = int.Parse(movePart.Substring(1, 1));

                                            playerMove += movePart + " ";
                                            continue;
                                        }

                                        // MOVES
                                        if (movePart.Contains('/'))
                                        {
                                            string[] ft = movePart.Split('/', '(', ')');

                                            string from = ft[0], to = ft[1];
                                            int fromInt = 0, toInt = 0;
                                            bool blot = false;

                                            if (from.Contains("bar")) from = from.Replace("bar", "25");
                                            else if (from.Contains("Bar")) from = from.Replace("Bar", "25");

                                            if (to.Contains("*"))
                                            {
                                                to = to.Trim('*');
                                                blot = true;
                                            }
                                            else if (to.Contains("off")) to = to.Replace("off", "0");
                                            else if (to.Contains("Off")) to = to.Replace("Off", "0");

                                            fromInt = int.Parse(from);
                                            toInt = int.Parse(to);

                                            if (toInt != 0 && boardState[toInt] == -1 && !blot)
                                            {
                                                // BLOT HAS BEEN MISSED IN NOTATION
                                                blot = true;
                                            }

                                            if (blot)
                                            {
                                                // NOTE - PLAYER2 BAR IS [0] AND (-)
                                                boardState[toInt] = 0;
                                                boardState[p2Bar] -= 1;
                                            }

                                            // MULTIPLE REPEAT MOVES
                                            if (movePart.Contains('(') && movePart.Contains(')'))
                                            {
                                                int numberOfRepeats = int.Parse(ft[2]);

                                                playerMove += from + "/" + to + (blot ? "* " : " ");

                                                boardState[fromInt] -= 1;
                                                if (toInt > 0) boardState[toInt] += 1;
                                                else boardState[p1Home] += 1;

                                                for (int rm = 1; rm < numberOfRepeats; rm++)
                                                {
                                                    playerMove += from + "/" + to + " ";

                                                    boardState[fromInt] -= 1;
                                                    if (toInt > 0) boardState[toInt] += 1;
                                                    else boardState[p1Home] += 1;
                                                }
                                            }
                                            else
                                            {
                                                playerMove += from + "/" + to + (blot ? "* " : " ");

                                                boardState[fromInt] -= 1;
                                                if (toInt > 0) boardState[toInt] += 1;
                                                else boardState[p1Home] += 1;
                                            }

                                            // SANITY CHECK MOVE IS NOT USING BOTH DICE
                                            // WHAT ARE THE LEAGAL MOVES - OTHERWISE APP. WILL CRASH
                                            if ((fromInt - toInt) == (dice1 + dice2))
                                            {
                                                // NOTE - MAY ALRAEDY CONTAIN LEGAL MOVES - SPLIT STRING AND REMOVE LAST ELEMENT
                                                string[] moves = playerMove.Split(' ');
                                                int count = Regex.Matches(playerMove, "/").Count;
                                                playerMove = moves[0] + " ";

                                                // NOTE - ABOVE MULITPLE REPEAT MOVE WILL ADD INITIAL VALUE
                                                // REMOVE LAST TO ALLOW SPLIT
                                                bool doubleUsed = false;
                                                if (movePart.Contains('(') && movePart.Contains(')'))
                                                {
                                                    doubleUsed = true;
                                                    count -= 1;
                                                }

                                                for (int c = 1; c < count; c++)
                                                {
                                                    playerMove += moves[c] + " ";
                                                }

                                                // CHECK WHICH MOVE IS LEGAL AND SPLIT THE MOVES
                                                if (boardState[(fromInt - dice1)] >= 0)
                                                {
                                                    playerMove += from + "/" + (fromInt - dice1).ToString() + " ";
                                                    playerMove += (fromInt - dice1).ToString() + "/" + to + (blot ? "* " : " ");

                                                    if (doubleUsed)
                                                    {
                                                        playerMove += from + "/" + (fromInt - dice1).ToString() + " ";
                                                        playerMove += (fromInt - dice1).ToString() + "/" + to;
                                                    }
                                                }
                                                else if (boardState[(fromInt - dice2)] >= 0)
                                                {
                                                    playerMove += from + "/" + (fromInt - dice2).ToString() + " ";
                                                    playerMove += (fromInt - dice2).ToString() + "/" + to + (blot ? "* " : " ");

                                                    if (doubleUsed)
                                                    {
                                                        playerMove += from + "/" + (fromInt - dice2).ToString() + " ";
                                                        playerMove += (fromInt - dice2).ToString() + "/" + to;
                                                    }
                                                }
                                            }
                                            else if ((fromInt - toInt) == (3 * dice1) && (dice1 == dice2))
                                            {
                                                // SPECIAL CASE WHERE PLAYER HAS USED 3 DICE TO MOVE A SINGLE COUNTER
                                                string[] moves = playerMove.Split(' ');
                                                int count = Regex.Matches(playerMove, "/").Count;
                                                playerMove = moves[0] + " ";

                                                // CAPTURE ANY PREVIOUS VALID MOVES
                                                for (int c = 1; c < count; c++)
                                                {
                                                    playerMove += moves[c] + " ";
                                                }

                                                playerMove += from + "/" + (fromInt - dice1).ToString() + " ";
                                                playerMove += (fromInt - dice1).ToString() + "/" + (fromInt - dice1 * 2).ToString() + " ";
                                                playerMove += (fromInt - dice1 * 2).ToString() + "/" + to + (blot ? "* " : " ");
                                            }
                                            else if ((fromInt - toInt) == ((dice1 + dice2) * (dice1 == dice2 ? 2 : 1)))
                                            {
                                                // SPECIAL CASE WHERE PLAYER HAS USED DOUBLES TO MOVE ONE COUNTER 4
                                                // MOVE MUST BE LEGAL AS IS SHOWN IN NOTATION
                                                string[] moves = playerMove.Split(' ');
                                                playerMove = moves[0] + " ";
                                                playerMove += from + "/" + (fromInt - dice1).ToString() + " ";
                                                playerMove += (fromInt - dice1).ToString() + "/" + (fromInt - dice1 * 2).ToString() + " ";
                                                playerMove += (fromInt - dice1 * 2).ToString() + "/" + (fromInt - dice1 * 3).ToString() + " ";
                                                playerMove += (fromInt - dice1 * 3).ToString() + "/" + to + (blot ? "* " : " ");
                                            }
                                            else if ((dice1 == dice2) && toInt == 0 && fromInt > dice1)
                                            {
                                                // CASE WHERE DOUBLE ROLLED AND PLAYER IS BEARING OFF A DOUBLE MOVE
                                                // HOW MANY DICE WERE USED
                                                int noOfDice = 2;
                                                // ONLY 2(3) FROM POINT 5 CAN USE 3 DICE - OTHERWISE EXACT == WILL BE CAUGHT ABOVE
                                                if (dice1 == 2 && fromInt == 5) noOfDice = 3;

                                                string[] moves = playerMove.Split(' ');
                                                int count = Regex.Matches(playerMove, "/").Count;
                                                playerMove = moves[0] + " ";

                                                // MULTIPLE REPEAT MOVES
                                                int repeats = 1;
                                                if (movePart.Contains('(') && movePart.Contains(')'))
                                                {
                                                    count = 0;
                                                    repeats = 2;
                                                }

                                                // CAPTURE ANY PREVIOUS VALID MOVES
                                                for (int c = 1; c < count; c++)
                                                {
                                                    playerMove += moves[c] + " ";
                                                }

                                                for (int r = 0; r < repeats; r++)
                                                {
                                                    // '-1' AS FINAL MOVE WILL BE ADDED LATER
                                                    for (int n = 0; n < noOfDice - 1; n++)
                                                    {
                                                        playerMove += (fromInt - (dice1 * n)).ToString() + "/" + (fromInt - (dice1 * (n + 1))).ToString() + " ";
                                                    }

                                                    playerMove += (fromInt - (dice1 * (noOfDice - 1))).ToString() + "/0 ";
                                                }
                                            }
                                            else if (toInt == 0 && ((fromInt - toInt) > (dice1 > dice2 ? dice2 : dice1)) && p1Moves.Length == 3)
                                            {
                                                // CASE WHERE SINGLE MOVE IS MADE (p1Moves.Length == 3) BEARING OFF

                                                // ARE BOTH DICE VALID - IF YES BOTH MUST BE PLAYED - IF NOT THE ORIGINAL MOVE IS ALREADY VALID
                                                // BLOTS WOULD BE NOTED AS SEPARATE ENTRIES AND ALREADY HANDLED

                                                int dice1End = (fromInt - dice1) <= 0 ? p1Home : (fromInt - dice1);
                                                int dice2End = (fromInt - dice2) <= 0 ? p1Home : (fromInt - dice2);

                                                if (boardState[dice1End] >= 0 && boardState[dice2End] >= 0)
                                                {
                                                    // ONLY SINGLE MOVE PLAYED - GET DICE ROLLS
                                                    string[] moves = playerMove.Split(' ');
                                                    int count = Regex.Matches(playerMove, "/").Count;
                                                    playerMove = moves[0] + " ";

                                                    // SMALLEST SHOULD BE PLAYED FIRST
                                                    playerMove += from + "/" + (fromInt - (dice1 > dice2 ? dice2 : dice1)).ToString() + " ";
                                                    playerMove += (fromInt - (dice1 > dice2 ? dice2 : dice1)).ToString() + "/" + to + " ";
                                                }
                                            }
                                        }

                                        // DOUBLES / TAKES / CONCEDES
                                        if (movePart == "Doubles") { playerMove = movePart; crawfordInPlay = false; }
                                        if (movePart == "Takes") playerMove = movePart;
                                        if (movePart == "Drops") playerMove = movePart;
                                        if (movePart == "Concedes") playerMove = movePart;
                                        if (movePart.Contains("?")) playerMove = "Concedes";
                                    }
                                }

                                // THERE IS NO MOVE INFO - OTHER PLAYER GOES FIRST OR GAME HAS BEEN LOST
                                if (playerMove == string.Empty) playerMove = ":";

                                gameMovesList.Add(playerMove);

                                playerMove = "";

                                int counters = 0;
                                foreach (int point in boardState)
                                {
                                    //counters += point;
                                    counters += Math.Abs(point);
                                }

                                if (counters != 30)
                                    Debug.Log("COUNTERS1 " + counters + " " + turnNumber);

                                // ---------- PLAYER 2 MOVES ---------- 

                                foreach (string movePart in p2Moves)
                                {
                                    if (movePart != string.Empty)
                                    {
                                        // DICE ROLLS
                                        if (movePart.Contains(':'))
                                        {
                                            dice1 = int.Parse(movePart.Substring(0, 1));
                                            dice2 = int.Parse(movePart.Substring(1, 1));

                                            playerMove += movePart + " ";
                                            continue;
                                        }

                                        // MOVES
                                        if (movePart.Contains('/'))
                                        {
                                            string[] ft = movePart.Split('/', '(', ')');

                                            string from = ft[0], to = ft[1];
                                            int fromInt = 0, toInt = 0;
                                            bool blot = false;

                                            if (from.Contains("bar")) from = from.Replace("bar", "25");
                                            else if (from.Contains("Bar")) from = from.Replace("Bar", "25");

                                            if (to.Contains("*"))
                                            {
                                                to = to.Trim('*');
                                                blot = true;
                                            }
                                            else if (to.Contains("off")) to = to.Replace("off", "0");
                                            else if (to.Contains("Off")) to = to.Replace("Off", "0");

                                            // INVERT AS PLAYER2 IS PLAYING FROM 1 - 24 - (25 / [0] IS BAR)
                                            fromInt = 25 - int.Parse(from);
                                            toInt = 25 - int.Parse(to);

                                            if (toInt != 25 && boardState[toInt] == 1 && !blot)
                                            {
                                                // BLOT HAS BEEN MISSED IN NOTATION
                                                blot = true;
                                            }

                                            if (blot)
                                            {
                                                // NOTE - PLAYER1 BAR IS [25] AND (+)
                                                boardState[toInt] = 0;
                                                boardState[p1Bar] += 1;
                                            }

                                            // MULTIPLE REPEAT MOVES
                                            if (movePart.Contains('(') && movePart.Contains(')'))
                                            {
                                                int numberOfRepeats = int.Parse(ft[2]);

                                                playerMove += from + "/" + to + (blot ? "* " : " ");

                                                boardState[fromInt] += 1;
                                                if (toInt < 25) boardState[toInt] -= 1;
                                                else boardState[p2Home] -= 1;

                                                for (int rm = 1; rm < numberOfRepeats; rm++)
                                                {
                                                    playerMove += from + "/" + to + " ";

                                                    boardState[fromInt] += 1;
                                                    if (toInt < 25) boardState[toInt] -= 1;
                                                    else boardState[p2Home] -= 1;
                                                }
                                            }
                                            else
                                            {
                                                playerMove += from + "/" + to + (blot ? "* " : " ");

                                                boardState[fromInt] += 1;
                                                if (toInt < 25) boardState[toInt] -= 1;
                                                else boardState[p2Home] -= 1;
                                            }

                                            // SANITY CHECK MOVE IS NOT USING BOTH DICE
                                            // WHAT ARE THE LEAGAL MOVES - OTHERWISE APP. WILL CRASH
                                            if ((toInt - fromInt) == (dice1 + dice2))
                                            {
                                                // NOTE - MAY ALRAEDY CONTAIN LEGAL MOVES - SPLIT STRING AND REMOVE LAST ELEMENT
                                                string[] moves = playerMove.Split(' ');
                                                int count = Regex.Matches(playerMove, "/").Count;
                                                playerMove = moves[0] + " ";

                                                // NOTE - ABOVE MULITPLE REPEAT MOVE WILL ADD INITIAL VALUE
                                                // REMOVE LAST TO ALLOW SPLIT
                                                bool doubleUsed = false;
                                                if (movePart.Contains('(') && movePart.Contains(')'))
                                                {
                                                    doubleUsed = true;
                                                    count -= 1;
                                                }

                                                for (int c = 1; c < count; c++)
                                                {
                                                    playerMove += moves[c] + " ";
                                                }

                                                // CHECK WHICH MOVE IS LEGAL AND SPLIT THE MOVES
                                                if (boardState[(fromInt + dice1)] <= 0)
                                                {
                                                    playerMove += from + "/" + ((25 - fromInt) - dice1).ToString() + " ";
                                                    playerMove += ((25 - fromInt) - dice1).ToString() + "/" + to + (blot ? "* " : " ");

                                                    if (doubleUsed)
                                                    {
                                                        playerMove += from + "/" + ((25 - fromInt) - dice1).ToString() + " ";
                                                        playerMove += ((25 - fromInt) - dice1).ToString() + "/" + to;
                                                    }
                                                }
                                                else if (boardState[(fromInt + dice2)] <= 0)
                                                {
                                                    playerMove += from + "/" + ((25 - fromInt) - dice2).ToString() + " ";
                                                    playerMove += ((25 - fromInt) - dice2).ToString() + "/" + to + (blot ? "* " : " ");

                                                    if (doubleUsed)
                                                    {
                                                        playerMove += from + "/" + ((25 - fromInt) - dice2).ToString() + " ";
                                                        playerMove += ((25 - fromInt) - dice2).ToString() + "/" + to;
                                                    }
                                                }
                                            }
                                            else if ((toInt - fromInt) == (dice1 * 3) && (dice1 == dice2))
                                            {
                                                // SPECIAL CASE WHERE PLAYER HAS USED 3 / 4 DICE TO MOVE A SINGLE COUNTER
                                                string[] moves = playerMove.Split(' ');
                                                int count = Regex.Matches(playerMove, "/").Count;
                                                playerMove = moves[0] + " ";

                                                // CAPTURE ANY PREVIOUS VALID MOVES
                                                for (int c = 1; c < count; c++)
                                                {
                                                    playerMove += moves[c] + " ";
                                                }

                                                playerMove += from + "/" + ((25 - fromInt) - dice1).ToString() + " ";
                                                playerMove += ((25 - fromInt) - dice1).ToString() + "/" + ((25 - fromInt) - dice1 * 2).ToString() + " ";
                                                playerMove += ((25 - fromInt) - dice1 * 2).ToString() + "/" + to + (blot ? "* " : " ");
                                            }
                                            else if ((toInt - fromInt) == ((dice1 + dice2) * (dice1 == dice2 ? 2 : 1)))
                                            {
                                                // SPECIAL CASE WHERE PLAYER HAS USED DOUBLES TO MOVE ONE COUNTER 4
                                                // MOVE MUST BE LEGAL AS NOTED
                                                string[] moves = playerMove.Split(' ');
                                                playerMove = moves[0] + " ";
                                                playerMove += from + "/" + ((25 - fromInt) - dice1).ToString() + " ";
                                                playerMove += ((25 - fromInt) - dice1).ToString() + "/" + ((25 - fromInt) - dice1 * 2).ToString() + " ";
                                                playerMove += ((25 - fromInt) - dice1 * 2).ToString() + "/" + ((25 - fromInt) - dice1 * 3).ToString() + " ";
                                                playerMove += ((25 - fromInt) - dice1 * 3).ToString() + "/" + to + (blot ? "* " : " ");
                                            }
                                            else if ((dice1 == dice2) && toInt == 25 && (25 - fromInt) > dice1)
                                            {
                                                // CASE WHERE DOUBLE ROLLED AND PLAYER IS BEARING OFF A DOUBLE MOVE
                                                // HOW MANY DICE WERE USED
                                                int noOfDice = 2;
                                                // ONLY 2(3) FROM POINT 5 CAN USE 3 DICE - OTHERWISE EXACT == WILL BE CAUGHT ABOVE
                                                if (dice1 == 2 && (25 - fromInt) == 5) noOfDice = 3;

                                                string[] moves = playerMove.Split(' ');
                                                int count = Regex.Matches(playerMove, "/").Count;
                                                playerMove = moves[0] + " ";

                                                // MULTIPLE REPEAT MOVES
                                                int repeats = 1;
                                                if (movePart.Contains('(') && movePart.Contains(')'))
                                                {
                                                    count = 0;
                                                    repeats = 2;
                                                }

                                                // CAPTURE ANY PREVIOUS VALID MOVES
                                                for (int c = 1; c < count; c++)
                                                {
                                                    playerMove += moves[c] + " ";
                                                }

                                                for (int r = 0; r < repeats; r++)
                                                {
                                                    // '-1' AS FINAL MOVE WILL BE ADDED LATER
                                                    for (int n = 0; n < noOfDice - 1; n++)
                                                    {
                                                        playerMove += ((25 - fromInt) - (dice1 * n)).ToString() + "/" + ((25 - fromInt) - (dice1 * (n + 1))).ToString() + " ";
                                                    }

                                                    playerMove += ((25 - fromInt) - (dice1 * (noOfDice - 1))).ToString() + "/0 ";
                                                }
                                            }
                                            else if (toInt == 25 && ((toInt - fromInt) > (dice1 > dice2 ? dice2 : dice1)) && p2Moves.Length == 2)
                                            {
                                                // CASE WHERE SINGLE MOVE IS MADE (p2Moves.Length == 2) BEARING OFF

                                                // ARE BOTH DICE VALID - IF YES BOTH MUST BE PLAYED - IF NOT THE ORIGINAL MOVE IS ALREADY VALID
                                                // BLOTS WOULD BE NOTED AS SEPARATE ENTRIES AND ALREADY HANDLED

                                                int dice1End = (25 - fromInt - dice1) <= 0 ? p2Home : (fromInt - dice1);
                                                int dice2End = (25 - fromInt - dice2) <= 0 ? p2Home : (fromInt - dice2);

                                                // PLAYER 2 IS NEGATIVE
                                                if (boardState[dice1End] <= 0 && boardState[dice2End] <= 0)
                                                {
                                                    // ONLY SINGLE MOVE PLAYED - GET DICE ROLLS
                                                    string[] moves = playerMove.Split(' ');
                                                    int count = Regex.Matches(playerMove, "/").Count;
                                                    playerMove = moves[0] + " ";

                                                    // SMALLEST SHOULD BE PLAYED FIRST
                                                    playerMove += from + "/" + (25 - fromInt - (dice1 > dice2 ? dice2 : dice1)).ToString() + " ";
                                                    playerMove += (25 - fromInt - (dice1 > dice2 ? dice2 : dice1)).ToString() + "/" + to + " ";
                                                }
                                            }
                                        }

                                        // DOUBLES / TAKES / CONCEDES
                                        if (movePart == "Doubles") { playerMove = movePart; crawfordInPlay = false; }
                                        if (movePart == "Takes") playerMove = movePart;
                                        if (movePart == "Drops") playerMove = movePart;
                                        if (movePart == "Concedes") playerMove = movePart;
                                        if (movePart.Contains("?")) playerMove = "Concedes";
                                    }
                                }

                                // THERE IS NO MOVE INFO - GAME HAS BEEN LOST
                                // WILL BE IN LINE IF PLAYER 2 HAS BORNE OFF
                                if (playerMove == string.Empty) playerMove = ":";
                                gameMovesList.Add(playerMove);

                                counters = 0;
                                foreach (int point in boardState)
                                {
                                    //counters += point;
                                    counters += Math.Abs(point);
                                }

                                if (counters != 30)
                                    Debug.Log("COUNTERS2 " + counters + " " + turnNumber);
                            }
                            break;
                        case string n when n.Contains("Doubles"):
                            {
                                // SPECIFIC CASE WHEN DOUBLES / TAKES / DROPS IN SINGLE LINE
                                string[] doubles = line.Split(' ');

                                foreach (string d in doubles)
                                {
                                    if (d.Contains("Doubles")) gameMovesList.Add("Doubles");
                                    if (d.Contains("Takes")) gameMovesList.Add("Takes");
                                    if (d.Contains("Drops")) gameMovesList.Add("Drops");
                                }

                                crawfordInPlay = false;
                            }
                            break;
                        case string x when x.Contains("?"):
                            {
                                gameMovesList.Add("Concedes");
                            }
                            break;
                        case string z when z.Contains("Concedes"):
                            {
                                gameMovesList.Add("Concedes");
                            }
                            break;
                    }
                }

                // CREATE MATCH ASSET OBJ - IF ALRADY EXISTS WILL BE OVERWRITTEN
                // FOR WHEN MATCH DATA IS PROPPERLY FORMATTED AND CONTAINS EVENT AND ROUND INFO
                //matchData.MatchConstructor(round, round, points, player1Surname, player1, player2Surname, player2, gameDataList.ToArray());

                // TEST IF MATCH HAS AN ID NUMBER
                if (matchID == "123456" || matchID == "" || matchID == string.Empty)
                {
                    UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

                    for (int _id = fallBackID.Length; _id < 7; _id++)
                        fallBackID += Mathf.Ceil(UnityEngine.Random.Range(1.0f, 9.0f)).ToString();

                    Debug.Log("_ID " + matchID + " " + matchID.Length);

                    matchID = fallBackID;

                    Debug.Log("ID " + fallBackID);
                }

                // HANDLE CRAWFORD
                if (crawfordInPlay) crawford = "true";
                else crawford = "false";

                matchData.MatchConstructor(eventName + " " + player1 + " vs " + player2, matchID, eventName, round, points, player1Surname, player1, player2Surname, player2, crawford, gameDataList.ToArray());

                Debug.Log($"EVENT {eventName} MATCHID {matchID} ROUND {round} POINTS {points} P1 {player1} P2 {player2} CRAWFORD {crawford} GAME DATA {gameDataList.Count}");

                matchDataList.Add(matchData);

                if (matchData != null)
                {
                    AssetDatabase.CreateAsset(matchData, BuildFolder + matchName + ".asset");
                    AssetDatabase.Refresh();
                }
                else Debug.Log($"MATCH DATA NULL");

                matchData = null;

                Debug.Log($"MATCH CREATED");

                // END FOREACH
            }

            Debug.Log($"*** ALL MATCHES BUILT ***");

            //// CREATE NEW GAME OBJECT AND ADD COMPONENT MATCHREPLAY_DLC
            //PrefabUtility.CreatePrefab();

            //GameObject replayDLC = new GameObject();
            //replayDLC.gameObject.name = "MatchReplayDLC_" + matchID;
            //replayDLC.AddComponent<MatchReplayDLC>();
            //replayDLC.GetComponentInChildren<MatchReplayDLC>().ConfigureMatchReplayDLC(matchID, matchDataList.ToArray());

            //// ADD REPLAY_DLC TO ASSETBUNDLE ARRAY
            ////replayDLCList.Add(replayDLC);
#endif
        }

        private static void BoardState(string _x, int[] _boardState)
        {
            int valid = 0;
            string board = "";
            foreach (int b in _boardState)
            {
                valid += b;
                board += b + ", ";
            }

            if (valid != 0)
                Debug.Log(_x + " " + board + " *" + valid);
        }

        //// https://answers.unity.com/questions/1805368/save-unityobject-as-an-asset.html

        //private static void CreateMatchAsset(MatchReplayDLC historicMatch)
        //{
        //    MatchReplayDLCConstructor obj = MatchReplayDLCConstructor.NewInstance(historicMatch);
        //    AssetDatabase.CreateAsset(obj, "HistoricMatch.asset");
        //}

        //[System.Serializable]
        //private class MatchReplayDLCConstructor : ScriptableObject
        //{
        //    private MatchReplayDLC historicMatch;

        //    public static MatchReplayDLCConstructor NewInstance(MatchReplayDLC historicMatch)
        //    {
        //        MatchReplayDLCConstructor instance = ScriptableObject.CreateInstance<MatchReplayDLCConstructor>();
        //        instance.historicMatch = historicMatch;
        //        return instance;
        //    }
        //}

        public static void ConstructAssetBundle()
        {
            // CREATE REPLAY_DLC ASSETBUNDLE ARRAY
        }

        private enum DataAttribute
        {
            // MATCH LEVEL DATA
            Site,
            Match,
            ID,
            Event,
            Player,
            Surname,
            Point,

            // GAME LEVEL DATA
        }

        private static DataAttribute dataAttribute
        {
            get;
            set;
        }

        // ______________________________________________________ GETTERS && SETTERS ______________________________________________________

        public void Reset()
        {
            BuildingAssets = false;
            BuildConfigured = false;
            BuildComplete = false;
            BuildFailed = false;
        }

        public static string TextFolder
        {
            get => textFolder;
            set => textFolder = value;
        }

        public static string BuildFolder
        {
            get => buildFolder;
            set => buildFolder = value;
        }

        public static bool UsingWorldChampionship
        {
            get => usingWorldChampionship;
            set => usingWorldChampionship = value;
        }

        public bool BuildingAssets
        {
            get => buildingAssets;
            private set => buildingAssets = value;
        }

        public bool BuildConfigured
        {
            get => buildConfigured;
            private set => buildConfigured = value;
        }

        public bool BuildComplete
        {
            get => buildComplete;
            private set => buildComplete = value;
        }
        public bool BuildFailed
        {
            get => buildFailed;
            private set => buildFailed = value;
        }
    }
}