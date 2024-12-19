using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class StatisticsGraphs : MonoBehaviour
    {
        [Header("LANGUAGE OBJECTS")]
        [SerializeField] private Text _graphButtonLabelText;
        [SerializeField] private Text _tableButtonLabelText;
        [SerializeField] private Text _backLabelText;
        [SerializeField] private Text _movesMatchedLabelText;
        [SerializeField] private Text _rank1AIMatchedLabelText;
        [SerializeField] private Text _rank1ProAIMatchedLabelText;
        [SerializeField] private Text _averageLabelText;
        [SerializeField] private Text _movesMadeLabelText;
        [SerializeField] private Text _percentageLabelText;

        [SerializeField] private Text _playerNameLabelText;
        [SerializeField] private Text _MovesMadeLabelText;
        [SerializeField] private Text _MovesMatchedLabelText;
        [SerializeField] private Text _MovesMatchPerLabelText;
        [SerializeField] private Text _AIMatchesLabelText;
        [SerializeField] private Text _AIMatchPerLabelText;
        [SerializeField] private Text _ProAIMatchesLabelText;
        [SerializeField] private Text _ProAIMatchPerLabelText;
        [SerializeField] private Text _YourRankingLabelText;

        [Header("GAME OBJECTS")]
        public GameObject statsGraphContainerObj;

        public GameObject lineRendererPrefab;
        public GameObject graphContentObj;
        public GameObject graphAxisLeft;
        public GameObject graphAxisRight;
        public GameObject graphAxisBase;
        public GameObject graphArea;
        public GameObject graphAxisPrefab;

        public GameObject tableEntryPrefab;
        public GameObject tableContentObj;
        public GameObject tableListContent;
        
        public Button selectGraphButton;
        public Button selectTableButton;

        private bool graphDataValid = false;
        private bool displayGraphOrTable;
        private ColorBlock reversedButtonColours;

        Dictionary<string, float> playerStatisticsDict;
        List<GraphStatsData> playerGraphStatsDataList;
        GameObject matchLine;
        GameObject topMatchLine;
        GameObject proMatchLine;
        GameObject avgMatchLine;
        GameObject avgTopMatchLine;
        GameObject avgProMatchLine;
        Vector3[] matchedMoves;
        Vector3[] topMatches;
        Vector3[] proTopMatches;
        Vector3[] avgMatchedMoves;
        Vector3[] avgTopMatches;
        Vector3[] avgProTopMatches;

        Dictionary<string, PlayerScoreData> playerScoresDict;
        Dictionary<string, PlayerScoreData> playerScoresTableDict;
        List<PlayerScoreData> playerScoresTableList;

        PlayerScoreData player1;
        PlayerScoreData player2;

        protected void Awake()
        {
            playerStatisticsDict = new Dictionary<string, float>();
            playerScoresTableDict = new Dictionary<string, PlayerScoreData>();
            playerScoresTableList = new List<PlayerScoreData>();

            matchLine = Instantiate(lineRendererPrefab) as GameObject;
            topMatchLine = Instantiate(lineRendererPrefab) as GameObject;
            proMatchLine = Instantiate(lineRendererPrefab) as GameObject;

            avgMatchLine = Instantiate(lineRendererPrefab) as GameObject;
            avgTopMatchLine = Instantiate(lineRendererPrefab) as GameObject;
            avgProMatchLine = Instantiate(lineRendererPrefab) as GameObject;

            matchLine.name = "MatchLine";
            topMatchLine.name = "TopMatchLine";
            proMatchLine.name = "ProMatchLine";

            avgMatchLine.name = "avgMatchLine";
            avgTopMatchLine.name = "avgTopMatchLine";
            avgProMatchLine.name = "avgProMatchLine";

            ConfigureLines();
        }

        protected void OnEnable()
        {
            // CONFIGURE LANGUAGE
            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                _graphButtonLabelText.text = languageSO.graphButtonLabelText;
                _tableButtonLabelText.text = languageSO.tableButtonLabelText;
                _backLabelText.text = languageSO.Back;
                _movesMatchedLabelText.text = languageSO.movesMadeLabelText;
                _rank1AIMatchedLabelText.text = languageSO.rank1AIMatchedLabelText;
                _rank1ProAIMatchedLabelText.text = languageSO.rank1ProAIMatchedLabelText;
                _averageLabelText.text = languageSO.averageLabelText;
                _movesMadeLabelText.text = languageSO.movesMadeLabelText;
                _percentageLabelText.text = languageSO.percentageLabelText;

                _playerNameLabelText.text = languageSO.playerNameLabelText;
                _MovesMadeLabelText.text = languageSO.movesMadeLabelText;
                _MovesMatchedLabelText.text = languageSO.MovesMatchedLabelText;
                _MovesMatchPerLabelText.text = languageSO.MovesMatchPerLabelText;
                _AIMatchesLabelText.text = languageSO.AIMatchesLabelText;
                _AIMatchPerLabelText.text = languageSO.AIMatchPerLabelText;
                _ProAIMatchesLabelText.text = languageSO.ProAIMatchesLabelText;
                _ProAIMatchPerLabelText.text = languageSO.ProAIMatchPerLabelText;
                _YourRankingLabelText.text = languageSO.YourRankingLabelText;
            }

            // CAPTURE DATA FOR GRAPH
            playerStatisticsDict = Main.Instance.PlayerScoresObj.GetPlayerStatisticsDict();
            playerGraphStatsDataList = new List<GraphStatsData>(Main.Instance.PlayerScoresObj.GetPlayerGraphStatsData());

            PopulateGraphData();

            // CAPTURE DATA FOR TABLE
            playerScoresDict = new Dictionary<string, PlayerScoreData>(Main.Instance.PlayerScoresObj.GetPlayerScoresDict());

            playerScoresTableDict.Clear();
            playerScoresTableList.Clear();

            PopulateTableData();

            // DEFAULT TO GRAPH
            selectGraphButton.interactable = false;
            selectTableButton.interactable = true;
            displayGraphOrTable = true;

            // SET TABLE BUTTON TO "OFF" APPEARANCE
            reversedButtonColours = selectTableButton.colors;
            reversedButtonColours.normalColor = selectTableButton.colors.pressedColor;
            reversedButtonColours.highlightedColor = selectTableButton.colors.normalColor;
            reversedButtonColours.selectedColor = selectTableButton.colors.normalColor;
            reversedButtonColours.pressedColor = selectTableButton.colors.normalColor;
            reversedButtonColours.disabledColor = selectTableButton.colors.normalColor;

            selectTableButton.colors = reversedButtonColours;

            UpdateUI();
        }

        private void UpdateUI()
        {
            graphContentObj.gameObject.SetActive(displayGraphOrTable);
            tableContentObj.gameObject.SetActive(!displayGraphOrTable);

            if (displayGraphOrTable)
            {
                // GRAPH IS DISPLAYED

                if (graphDataValid)
                {
                    // DRAW DATA
                    matchLine.GetComponent<LineRenderer>().SetPositions(matchedMoves);
                    topMatchLine.GetComponent<LineRenderer>().SetPositions(topMatches);
                    proMatchLine.GetComponent<LineRenderer>().SetPositions(proTopMatches);

                    // DRAW AVG LINES
                    avgMatchLine.GetComponent<LineRenderer>().SetPositions(avgMatchedMoves);
                    avgTopMatchLine.GetComponent<LineRenderer>().SetPositions(avgTopMatches);
                    avgProMatchLine.GetComponent<LineRenderer>().SetPositions(avgProTopMatches);
                }
            }
            else
            {
                // TABLE IS DISPLAYED

                // SET CONTENT LIST TO TOP
                tableListContent.gameObject.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1;
            }
        }

        public void OnClickGraph()
        {
            selectGraphButton.interactable = false;
            selectTableButton.interactable = true;

            displayGraphOrTable = true;
            UpdateUI();
        }

        public void OnClickTable()
        {
            selectGraphButton.colors = reversedButtonColours;
            selectGraphButton.interactable = true;
            selectTableButton.interactable = false;

            displayGraphOrTable = false;
            UpdateUI();
        }

        public void OnClickBackButton()
        {
            statsGraphContainerObj.gameObject.SetActive(false);
        }

        private void PopulateGraphData()
        {
            var dataPoints = 0;
            float totalMovesMade = 0f;

            foreach (var dataPoint in playerGraphStatsDataList)
            {
                dataPoints += dataPoint.gameDataList.Count;
                totalMovesMade += dataPoint.totalMovesMade;
            }

            graphDataValid = totalMovesMade >= 8 ? true : false;

            // GET MARGINS AND RELATIVE DATA POINTS TO GRAPH AREA
            var graphWidth = graphArea.GetComponent<RectTransform>().rect.width;
            var graphXZero = -graphWidth / 2;
            var widthDistributionPerMove = -graphXZero / totalMovesMade * 2;
            var graphYZero = -graphArea.GetComponent<RectTransform>().rect.height / 2;
            var graphDistributionHeight = -graphYZero / 50f;
            var graphAreaOffsetY = graphArea.transform.position.y;
            var runningMovesTotal = 0f;
            var counter = 0;

            // TODO: DATA POINTS NEED TO BE CAPPED AND SCALED TO THE WIDTH OF THE GRAPH AREA ~480 PXLS

            matchedMoves = new Vector3[dataPoints];
            topMatches = new Vector3[dataPoints];
            proTopMatches = new Vector3[dataPoints];

            // POPULATE ARRAYS OF DATA POINTS
            foreach (var dataPoint in playerGraphStatsDataList)
            {
                Debug.Log($"STATS DATA {dataPoint.matchKey}");

                foreach (var gameData in dataPoint.gameDataList)
                {
                    Debug.Log($"MOVES {gameData.movesMade} {runningMovesTotal}");

                    var movesMade = gameData.movesMade;
                    var percentageMatch = 0f;

                    percentageMatch = movesMade == 0 ? 0 : Mathf.Round((gameData.movesMatched / movesMade) * 100f);
                    percentageMatch = float.IsNaN(percentageMatch) ? 0 : percentageMatch;

                    matchedMoves[counter] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, (runningMovesTotal + movesMade), percentageMatch);

                    percentageMatch = movesMade == 0 ? 0 : Mathf.Round((gameData.topMatched / movesMade) * 100f);
                    percentageMatch = float.IsNaN(percentageMatch) ? 0 : percentageMatch;

                    topMatches[counter] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, (runningMovesTotal + movesMade), percentageMatch);

                    percentageMatch = movesMade == 0 ? 0 : Mathf.Round((gameData.proTopMatched / movesMade) * 100f);
                    percentageMatch = float.IsNaN(percentageMatch) ? 0 : percentageMatch;

                    proTopMatches[counter] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, (runningMovesTotal + movesMade), percentageMatch);

                    runningMovesTotal += gameData.movesMade;
                    counter++;
                }
            }

            // SET LINE VERTICES
            matchLine.GetComponent<LineRenderer>().positionCount = dataPoints;
            topMatchLine.GetComponent<LineRenderer>().positionCount = dataPoints;
            proMatchLine.GetComponent<LineRenderer>().positionCount = dataPoints;

            // RECALCULATE BASE AXIS
            var baseAxisChildContainer = graphAxisBase.transform.Find("AxisChildContainer").transform;

            foreach (Transform child in baseAxisChildContainer)
                Destroy(child.gameObject);

            var noOfGraphSectionsToAdd = 8;
            float axisSectionXScale = graphAxisBase.GetComponent<RectTransform>().rect.width / (noOfGraphSectionsToAdd * 100f);
            var movesPerSection = totalMovesMade / noOfGraphSectionsToAdd;

            for (int b_axis = 0; b_axis < noOfGraphSectionsToAdd; b_axis++)
            {
                var axisSection = Instantiate(graphAxisPrefab) as GameObject;
                axisSection.transform.SetParent(baseAxisChildContainer);
                axisSection.GetComponent<RectTransform>().localPosition = new Vector3(graphXZero + (axisSection.GetComponent<RectTransform>().rect.width * (b_axis + 0.5f) * axisSectionXScale), 0f);
                axisSection.GetComponent<RectTransform>().localScale = new Vector3(axisSectionXScale, 1.0f, 1.0f);

                axisSection.GetComponent<StatsGraphAxis>().SetGraphAxisText(-b_axis, (movesPerSection * (b_axis + 1)));
            }

            // AVG MATCH LINES

            avgMatchedMoves = new Vector3[2];
            avgTopMatches = new Vector3[2];
            avgProTopMatches = new Vector3[2];

            float totalPlayerMovesMade = playerStatisticsDict["totalPlayerMovesMade"];
            float proMatches = playerStatisticsDict["proMatches"];
            float totalAIMatchesMade = playerStatisticsDict["totalAIMatchesMade"];
            float totalProAIMatchesMade = playerStatisticsDict["totalProAIMatchesMade"];

            Debug.Log("MOVES " + totalPlayerMovesMade + " MATCHES " + proMatches + " AI MATCHES " + totalAIMatchesMade + " PRO " + totalProAIMatchesMade);

            float avgPercentageMatched = totalPlayerMovesMade == 0 ? 0 : Mathf.Round((proMatches / totalPlayerMovesMade) * 100f);
            avgPercentageMatched = float.IsNaN(avgPercentageMatched) ? 0 : avgPercentageMatched;

            float avgAIPercentageMatched = totalPlayerMovesMade == 0 ? 0 : Mathf.Round((totalAIMatchesMade / totalPlayerMovesMade) * 100f);
            avgAIPercentageMatched = float.IsNaN(avgAIPercentageMatched) ? 0 : avgAIPercentageMatched;

            float avgProAIPercentageMatched = totalPlayerMovesMade == 0 ? 0 : Mathf.Round((totalProAIMatchesMade / totalPlayerMovesMade) * 100f);
            avgProAIPercentageMatched = float.IsNaN(avgProAIPercentageMatched) ? 0 : avgProAIPercentageMatched;

            Debug.Log("MOVES " + totalPlayerMovesMade 
                + " MATCHES " + Mathf.Round((proMatches / totalPlayerMovesMade) * 100f) 
                + " AI MATCHES " + Mathf.Round((totalAIMatchesMade / totalPlayerMovesMade) * 100f) 
                + " PRO " + Mathf.Round((totalProAIMatchesMade / totalPlayerMovesMade) * 100f));

            avgMatchedMoves[0] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, 0, avgPercentageMatched);
            avgMatchedMoves[1] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, runningMovesTotal, avgPercentageMatched);

            avgTopMatches[0] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, 0, avgAIPercentageMatched);
            avgTopMatches[1] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, runningMovesTotal, avgAIPercentageMatched);

            avgProTopMatches[0] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, 0, avgProAIPercentageMatched);
            avgProTopMatches[1] = ScalePointPosition(graphXZero, widthDistributionPerMove, graphYZero, graphDistributionHeight,
                                                                    graphAreaOffsetY, runningMovesTotal, avgProAIPercentageMatched);
        }

        private void PopulateTableData()
        {
            // CONSTRUCT PLAYERSCOREDATA FOR EACH PRO PLAYER

            foreach (var match in playerScoresDict.Values)
            {
                var matchScore = match.matchScores;

                player1 = new PlayerScoreData();
                player2 = new PlayerScoreData();

                var p1Name = match.player1Name;
                var p2Name = match.player2Name;
                
                player1.player1Name = p1Name;
                player1.matchScores.movesMade = matchScore.player1BestMovesMade;
                player1.matchScores.movesMatched = matchScore.player1BestMovesMatched;
                player1.matchScores.topMatched = matchScore.player1BestTopMatched;
                player1.matchScores.proTopMatched = matchScore.player1BestProTopMatched;

                player2.player1Name = p2Name;
                player2.matchScores.movesMade = matchScore.player2BestMovesMade;
                player2.matchScores.movesMatched = matchScore.player2BestMovesMatched;
                player2.matchScores.topMatched = matchScore.player2BestTopMatched;
                player2.matchScores.proTopMatched = matchScore.player2BestProTopMatched;

                if (playerScoresTableDict.ContainsKey(p1Name))
                {
                    // TRANSFER INFO TO PLAYER SCORE OBJ
                    var _player1 = playerScoresTableDict[p1Name];

                    _player1.matchScores.movesMade += player1.matchScores.movesMade;
                    _player1.matchScores.movesMatched += player1.matchScores.movesMatched;
                    _player1.matchScores.percentageMatched = _player1.matchScores.movesMade == 0 ?
                        0 : ((_player1.matchScores.movesMatched / _player1.matchScores.movesMade) * 100f);
                    _player1.matchScores.topMatched += player1.matchScores.topMatched;
                    _player1.matchScores.percentageTopMatched = _player1.matchScores.movesMade == 0 ?
                        0 : ((_player1.matchScores.topMatched / _player1.matchScores.movesMade) * 100f);
                    _player1.matchScores.proTopMatched += player1.matchScores.proTopMatched;
                    _player1.matchScores.percentageProTopMatched = _player1.matchScores.movesMade == 0 ?
                        0 : ((_player1.matchScores.proTopMatched / _player1.matchScores.movesMade) * 100f);

                    playerScoresTableDict[p1Name] = _player1;
                }
                else
                {
                    // ADD NEW OBJ
                    player1.matchScores.percentageMatched = player1.matchScores.movesMade == 0 ?
                        0 : ((player1.matchScores.movesMatched / player1.matchScores.movesMade) * 100f);
                    player1.matchScores.percentageTopMatched = player1.matchScores.movesMade == 0 ?
                        0 : ((player1.matchScores.topMatched / player1.matchScores.movesMade) * 100f);
                    player1.matchScores.percentageProTopMatched = player1.matchScores.movesMade == 0 ?
                        0 : ((player1.matchScores.proTopMatched / player1.matchScores.movesMade) * 100f);
                    
                    playerScoresTableDict.Add(p1Name, player1);
                }

                if (playerScoresTableDict.ContainsKey(p2Name))
                {
                    // TRANSFER INFO TO PLAYER SCORE OBJ
                    var _player2 = playerScoresTableDict[p2Name];

                    _player2.matchScores.movesMade += player2.matchScores.movesMade;
                    _player2.matchScores.movesMatched += player2.matchScores.movesMatched;
                    _player2.matchScores.percentageMatched = _player2.matchScores.movesMade == 0 ?
                        0 : ((_player2.matchScores.movesMatched / _player2.matchScores.movesMade) * 100f);
                    _player2.matchScores.topMatched += player2.matchScores.topMatched;
                    _player2.matchScores.percentageTopMatched = _player2.matchScores.movesMade == 0 ?
                        0 : ((_player2.matchScores.topMatched / _player2.matchScores.movesMade) * 100f);
                    _player2.matchScores.proTopMatched += player2.matchScores.proTopMatched;
                    _player2.matchScores.percentageProTopMatched = _player2.matchScores.movesMade == 0 ?
                        0 : ((_player2.matchScores.proTopMatched / _player2.matchScores.movesMade) * 100f);

                    playerScoresTableDict[p2Name] = _player2;
                }
                else
                {
                    // ADD NEW OBJ
                    player2.matchScores.percentageMatched = player2.matchScores.movesMade == 0 ?
                        0 : ((player2.matchScores.movesMatched / player2.matchScores.movesMade) * 100f);
                    player2.matchScores.percentageTopMatched = player2.matchScores.movesMade == 0 ?
                        0 : ((player2.matchScores.topMatched / player2.matchScores.movesMade) * 100f);
                    player2.matchScores.percentageProTopMatched = player2.matchScores.movesMade == 0 ?
                        0 : ((player2.matchScores.proTopMatched / player2.matchScores.movesMade) * 100f);

                    playerScoresTableDict.Add(p2Name, player2);
                }
            }

            foreach (var pro in playerScoresTableDict.Values)
                playerScoresTableList.Add(pro);

            // CLEAR AND SORT TABLE
            foreach (Transform child in tableListContent.transform)
                Destroy(child.gameObject);

            playerScoresTableList = playerScoresTableList.OrderByDescending(x => x.matchScores.percentageTopMatched)
                .ThenByDescending(y => y.matchScores.movesMade).ThenByDescending(z => z.matchScores.movesMatched).ToList();

            // POPULATE TABLE FROM SORTED LIST
            foreach (var pro in playerScoresTableList)
            {
                GameObject tableEntry = Instantiate(tableEntryPrefab) as GameObject;
                StatsGraphTableEntry entryValues = tableEntry.GetComponent<StatsGraphTableEntry>();

                var _name = pro.player1Name;
                var _movesMade = pro.matchScores.movesMade.ToString();
                var _movesMatched = pro.matchScores.movesMatched.ToString();
                var _matchPercent = pro.matchScores.percentageMatched.ToString("F1") + "%";
                var _topMatched = pro.matchScores.topMatched.ToString();
                var _topMacthPercent = pro.matchScores.percentageTopMatched.ToString("F1") + "%";
                var _proTopMatched = pro.matchScores.proTopMatched.ToString();
                var _proTopMatchPercent = pro.matchScores.percentageProTopMatched.ToString("F1") + "%";
                var _rating = Main.Instance.PlayerScoresObj.GetPlayerRanking(pro.matchScores.percentageMatched);

                entryValues.SetEntryStats(_name, _movesMade, _movesMatched, _matchPercent, _topMatched, _topMacthPercent, _proTopMatched, _proTopMatchPercent, _rating);

                tableEntry.transform.SetParent(tableListContent.transform, false);
                tableEntry.transform.localScale = Vector3.one;
            }
        }

        private void ConfigureLines()
        {
            var lineThickness = 6f;
            var avgLineThickness = 4f;

            // NOTE - COLOURS TAKEN FROM STATS GRAPH - GRAPH CONTENT - GRAPH HEADER
            var matchedColour = new Color32(238, 250, 25, 255);
            var topMacthColour = new Color32(68, 231, 19, 255);
            var proMatchColour = new Color32(57, 103, 238, 255);
            var avgMatchedColour = new Color32(238, 250, 25, 165);
            var avgTopMacthColour = new Color32(68, 231, 19, 165);
            var avgProMatchColour = new Color32(57, 103, 238, 165);

            // CONFIGURE GRAPH LINE
            ConfigureGraphLine(matchLine, matchedColour, lineThickness);
            ConfigureGraphLine(topMatchLine, topMacthColour, lineThickness);
            ConfigureGraphLine(proMatchLine, proMatchColour, lineThickness);

            // CONFIGURE AVG MATCH LINE
            ConfigureGraphLine(avgMatchLine, avgMatchedColour, avgLineThickness);
            ConfigureGraphLine(avgTopMatchLine, avgTopMacthColour, avgLineThickness);
            ConfigureGraphLine(avgProMatchLine, avgProMatchColour, avgLineThickness);

            // CONFIGURE SIDE AXIS
            var graphYZero = -graphAxisLeft.GetComponent<RectTransform>().rect.height / 2;
            float axisSectionYScale = graphAxisLeft.GetComponent<RectTransform>().rect.height / 400f;

            for (int s_axis = 0; s_axis < 4; s_axis++)
            {
                var l_axis = Instantiate(graphAxisPrefab) as GameObject;
                l_axis.transform.SetParent(graphAxisLeft.transform);
                l_axis.GetComponent<RectTransform>().localPosition = new Vector3(0, (graphYZero + (l_axis.GetComponent<RectTransform>().rect.width * (s_axis + 0.5f) * axisSectionYScale)), 0f);
                l_axis.GetComponent<RectTransform>().localScale = new Vector3(axisSectionYScale, 1.0f, 1.0f);
                l_axis.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);

                l_axis.GetComponent<StatsGraphAxis>().SetGraphAxisText(-s_axis, (25 * (s_axis + 1)), true, 90f);

                var r_axis = Instantiate(graphAxisPrefab) as GameObject;
                r_axis.transform.SetParent(graphAxisRight.transform);
                r_axis.GetComponent<RectTransform>().localPosition = new Vector3(0, (graphYZero + (l_axis.GetComponent<RectTransform>().rect.width * (s_axis + 0.5f) * axisSectionYScale)), 0f);
                r_axis.GetComponent<RectTransform>().localScale = new Vector3(axisSectionYScale, 1.0f, 1.0f);
                r_axis.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

                r_axis.GetComponent<StatsGraphAxis>().SetGraphAxisText(-s_axis, (25 * (s_axis + 1)), true, -90f);
            }
        }

        private void ConfigureGraphLine(GameObject graphLine, Color colour, float lineThickness)
        {
            graphLine.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphArea.GetComponent<RectTransform>().rect.width);
            graphLine.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, graphArea.GetComponent<RectTransform>().rect.height);
            graphLine.transform.SetParent(graphArea.transform);
            graphLine.transform.localPosition = new Vector3();

            var _graphLineRenderer = graphLine.GetComponent<LineRenderer>();
            _graphLineRenderer.startColor = colour;
            _graphLineRenderer.endColor = colour;
            _graphLineRenderer.startWidth = lineThickness;
            _graphLineRenderer.endWidth = lineThickness;
        }

        private Vector3 ScalePointPosition(float xZero, float xDistribution, float yZero, float yDistribution, float graphAreaOffsetY, float movesMade, float yToScale)
        {
            return new Vector3((xZero + (xDistribution * movesMade)), (yZero + graphAreaOffsetY + (yDistribution * yToScale)));
        }

        private void WriteDebugValuesToStatsGraph(int noOfValues)
        {
            for (var match = 0; match < noOfValues; match++)
            {
                GraphStatsData statsData = new GraphStatsData();
                statsData.matchKey = match.ToString();
                statsData.totalMovesMade = match;

                var randomGames = UnityEngine.Random.Range(0, noOfValues);
                var randomResult = UnityEngine.Random.Range(25, 50);
                var randomeMatches = UnityEngine.Random.Range(60 + (match * 0.5f), 80 + match) / 100f;
                var randomTop = UnityEngine.Random.Range(50 + (match * 0.5f), 70 + match) / 100f;
                var randomPro = UnityEngine.Random.Range(85, 95) / 100f;
                var noise = 1 + (UnityEngine.Random.Range(-5, 5) / 100f);

                for (int games = 0; games < randomGames; games++)
                {
                    GraphStatsGameData game = new GraphStatsGameData();
                    game.movesMade = randomResult;
                    game.movesMatched = randomResult * randomeMatches * noise;
                    game.topMatched = randomResult * randomTop * 1 + noise;
                    game.proTopMatched = randomResult * randomPro * 1 + noise;

                    statsData.totalMovesMade += game.movesMade;
                    statsData.gameDataList.Add(game);
                }

                playerGraphStatsDataList.Add(statsData);
            }
        }
    }
}