using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class MatchAIConfigureSettingsMain : MonoBehaviour
    {
        [Header("AI SETTINGS")]
        [SerializeField] private Backgammon_Asset.MatchReplayDLC _aiMatch = null;
        [SerializeField] private AISettingsScriptableObject _aiSettingsSO = null;

        [Header("OPTIONS BUTTONS")]
        [SerializeField] Button _setPointsButton = null;
        [SerializeField] Button _setDiceButton = null;
        [SerializeField] Button _setDifficultyButton = null;

        [Header("SET POINTS")]
        [SerializeField] GameObject _setPointsObj = null;
        [SerializeField] Text _pointsTitle = null;
        [SerializeField] Button _pointsSingleButton = null;
        [SerializeField] Button _pointsMatchButton = null;

        [Header("SET DICE")]
        [SerializeField] GameObject _setDiceObj = null; 
        [SerializeField] Text _diceTitle = null;
        [SerializeField] Button _historicDiceButton = null;
        [SerializeField] Button _randomDiceButton = null;

        [Header("SET DIFFICULTY")]
        [SerializeField] GameObject _setDifficultyObj = null;
        [SerializeField] Text _difficultyTitle = null;
        [SerializeField] private Button _easyButton = null;
        [SerializeField] private Button _mediumButton = null;
        [SerializeField] private Button _hardButton = null;
        [SerializeField] private Button _perfectButton = null;
        [SerializeField] private Button _randomButton = null;
        [SerializeField] Text _difficultyDescription = null;

        [Header("PAGE OPTIONS")]
        [SerializeField] Button _startMatchButton = null;
        [SerializeField] Button _backButton = null;

        private void Awake()
        {
            _setPointsButtonText = _setPointsButton.gameObject.GetComponentInChildren<Text>();
            _setDiceButtonText = _setDiceButton.gameObject.GetComponentInChildren<Text>();
            _setDifficultyButtonText = _setDifficultyButton.gameObject.GetComponentInChildren<Text>();

            _pointsSingleButtonText = _pointsSingleButton.gameObject.GetComponentInChildren<Text>();
            _pointsMatchButtonText = _pointsMatchButton.gameObject.GetComponentInChildren<Text>();

            _diceHistoricButtonText = _historicDiceButton.gameObject.GetComponentInChildren<Text>();
            _diceRandomButtonText = _randomDiceButton.gameObject.GetComponentInChildren<Text>();

            _defaultButtonColour = _setPointsButton.GetComponentInChildren<Image>().color;

            _easyButtonHighlight = _easyButton.GetComponentInChildren<Image>();
            _mediumButtonHighlight = _mediumButton.GetComponentInChildren<Image>();
            _hardButtonHighlight = _hardButton.GetComponentInChildren<Image>();
            _perfectButtonHighlight = _perfectButton.GetComponentInChildren<Image>();
            _randomButtonHighlight = _randomButton.GetComponentInChildren<Image>();

            _easyButtonText = _easyButton.GetComponentInChildren<Text>();
            _mediumButtonText = _mediumButton.GetComponentInChildren<Text>();
            _hardButtonText = _hardButton.GetComponentInChildren<Text>();
            _perfectButtonText = _perfectButton.GetComponentInChildren<Text>();
            _randomButtonText = _randomButton.GetComponentInChildren<Text>();

            _startButtonText = _startMatchButton.GetComponentInChildren<Text>();
            _backButtonText = _backButton.GetComponentInChildren<Text>();

            // TODO: CONFIGURE LANGUAGE BY REGION
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                _startButtonText.text = languageSO.Start;

                _easyButtonText.text = languageSO.matchAISelectEASY;
                _mediumButtonText.text = languageSO.matchAISelectMEDIUM;
                _hardButtonText.text = languageSO.matchAISelectHARD;
                _perfectButtonText.text = languageSO.matchAISelectPERFECT;
                _randomButtonText.text = languageSO.matchAISelectRandom;

                _startButtonText.text = languageSO.Start;
                _backButtonText.text = languageSO.Back;
            }
        }

        private void OnEnable()
        {
            IfStart = false;
            IfBack = false;
            ManualSetupBoard = false;
            
            _resetFromGamePause = false;
            _toggleDiceOptionAutoProgress = false;
            _startButtonText.text = languageSO != null ? languageSO.Start : "Start";
            _difficultyDescription.text = string.Empty;

            // CONFIGURE DIFFICULTY BUTTONS
            _easyButton.onClick.AddListener(() => OnEasyButtonClicked());
            _mediumButton.onClick.AddListener(() => OnMediumButtonClicked());
            _hardButton.onClick.AddListener(() => OnHardButtonClicked());
            _perfectButton.onClick.AddListener(() => OnPerfectButtonClicked());
            _randomButton.onClick.AddListener(() => OnRandomButtonClicked());

            // SET VALUES TO PLAYER SAVED VALUES
            _highestRankValue = _aiSettingsSO.PlayerRankHighest;
            _lowestRankValue = _aiSettingsSO.PlayerRankLowest;
            _points = _aiSettingsSO.PlayerPoints;
            _ply = _aiSettingsSO.PlayerPly;
            _noise = _aiSettingsSO.PlayerNoise;
            _preset = _aiSettingsSO.PlayerPreset;

            // RE-LOAD EACH TIME IN CASE NEW BUNDLES DOWNLOADED
            LoadHistoricDiceRolls();

            // SET BUTTONS PRESSED BASED ON PREVIOUS SELECTION
            if (Game2D.AIIfUsingHistoricDice) OnClickSetDiceToHistoric();
            else OnClickSetDiceToRandom();

            //if (_aiSettingsSO.PlayerPoints == 5) OnClickSetPointsToMatch();
            //else OnClickSetPointsToGame();

            // DEFAULT TO POINTS OPTION
            //OnClickSetPointsButton();

            if (_preset == "EASY") { OnEasyButtonClicked(); }
            else if (_preset == "MEDIUM") { OnMediumButtonClicked(); }
            else if (_preset == "HARD") { OnHardButtonClicked(); }
            else if (_preset == "PERFECT") { OnPerfectButtonClicked(); }

            ConfigureAISettingsToGame();
            OnClickSetDiceButton();
        }

        private void OnDisable()
        {
            _easyButton.onClick.RemoveAllListeners();
            _mediumButton.onClick.RemoveAllListeners();
            _hardButton.onClick.RemoveAllListeners();
            _perfectButton.onClick.RemoveAllListeners();
            _randomButton.onClick.RemoveAllListeners();
        }

        // -------------------------------------- OPTIONS BUTTONS --------------------------------------

        public void OnChangeAISettingsFromGamePause()
        {
            _resetFromGamePause = true;
            _startButtonText.text = languageSO != null ? languageSO.Continue : "Continue";
        }

        public void OnClickSetPointsButton()
        {
            ResetOptionsButtons();

            _setPointsObj.gameObject.SetActive(true);
            _setPointsButton.GetComponentInChildren<Image>().color = Color.cyan;
        }

        public void OnClickSetDiceButton()
        {
            ResetOptionsButtons();

            _setDiceObj.gameObject.SetActive(true);
            _setDiceButton.GetComponentInChildren<Image>().color = Color.cyan;
        }

        public void OnClickSetDifficultyButton()
        {
            ResetOptionsButtons();

            _setDifficultyObj.gameObject.SetActive(true);
            _setDifficultyButton.GetComponentInChildren<Image>().color = Color.cyan;
        }

        private void ResetOptionsButtons()
        {
            _setPointsObj.gameObject.SetActive(false);
            _setDiceObj.gameObject.SetActive(false);
            _setDifficultyObj.gameObject.SetActive(false);

            _setPointsButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
            _setDiceButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
            _setDifficultyButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
        }

        public void OnClickStart() 
        {
            ConfigureAISettingsToGame();
            ConfigureAISettingsToMatch();
            IfStart = true;
        }

        public void OnClickBack() 
        {
            if (_setDifficultyObj.gameObject.activeInHierarchy)
                OnClickSetDiceButton();
            else IfBack = true; 
        }

        // ------------------------------------- SETTINGS BUTTONS --------------------------------------

        // SET POINTS

        public void OnClickSetPointsToGame()
        {
            DeselectPointsButtonHighlights();
            _pointsSingleButton.GetComponentInChildren<Image>().color = Color.cyan;

            _points = 1;

            ConfigureAISettingsToGame();
            OnClickSetDiceButton();
        }

        public void OnClickSetPointsToMatch()
        {
            DeselectPointsButtonHighlights();
            _pointsMatchButton.GetComponentInChildren<Image>().color = Color.cyan;
            _points = 5;

            ConfigureAISettingsToGame();
            OnClickSetDiceButton();
        }

        private void DeselectPointsButtonHighlights()
        {
            _pointsSingleButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
            _pointsMatchButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
        }

        // SET DICE

        public void OnClickSetDiceToHistoric()
        {
            DeselectDiceButtonHighlights();
            //_historicDiceButton.GetComponentInChildren<Image>().color = Color.cyan;

            SelectRandomHistoricDiceRolls();
            _ifHistoricDiceRolls = true;

            ConfigureAISettingsToGame();
            OnClickSetDifficultyButton();

            //if (!_toggleDiceOptionAutoProgress)
            //    OnClickSetDifficultyButton();

            //_toggleDiceOptionAutoProgress = true;
        }

        public void OnClickSetDiceToRandom()
        {
            DeselectDiceButtonHighlights();
            //_randomDiceButton.GetComponentInChildren<Image>().color = Color.cyan;

            _ifHistoricDiceRolls = false;

            ConfigureAISettingsToGame();
            OnClickSetDifficultyButton();

            //_toggleDiceOptionAutoProgress = true;
        }

        private void DeselectDiceButtonHighlights()
        {
            _historicDiceButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
            _randomDiceButton.GetComponentInChildren<Image>().color = _defaultButtonColour;
        }

        // SET DIFFICULTY

        private void OnEasyButtonClicked()
        {
            DeselectDifficultyButtonHighlights();
            _easyButtonHighlight.color = Color.cyan;
            _preset = "EASY";

            _difficultyDescription.text = languageSO != null ? languageSO.matchAISelectEASYDesc : "EASY";

            _highestRankValue = 5;
            _lowestRankValue = 1;
            _ply = 0;
            _noise = 0.5f;

            ConfigureAISettingsToGame();
        }

        private void OnMediumButtonClicked()
        {
            DeselectDifficultyButtonHighlights();
            _mediumButtonHighlight.color = Color.cyan;
            _preset = "MEDIUM";

            _difficultyDescription.text = languageSO != null ? languageSO.matchAISelectMEDIUMDesc : "MEDIUM";

            _highestRankValue = 4;
            _lowestRankValue = 1;
            _ply = 1;
            _noise = 0.3f;

            ConfigureAISettingsToGame();
        }

        private void OnHardButtonClicked()
        {
            DeselectDifficultyButtonHighlights();
            _hardButtonHighlight.color = Color.cyan;
            _preset = "HARD";

            _difficultyDescription.text = languageSO != null ? languageSO.matchAISelectHARDDesc : "HARD";

            _highestRankValue = 2;
            _lowestRankValue = 1;
            _ply = 2;
            _noise = 0.2f;

            ConfigureAISettingsToGame();
        }

        private void OnPerfectButtonClicked()
        {
            DeselectDifficultyButtonHighlights();
            _perfectButtonHighlight.color = Color.cyan;
            _preset = "PERFECT";

            _difficultyDescription.text = languageSO != null ? languageSO.matchAISelectPERFECTDesc : "PERFECT";

            _highestRankValue = 1;
            _lowestRankValue = 1;
            _ply = 3;
            _noise = 0.1f;

            ConfigureAISettingsToGame();
        }

        public void OnRandomButtonClicked()
        {
            DeselectDifficultyButtonHighlights();
            _randomButtonHighlight.color = Color.cyan;
            _preset = "RANDOM";

            _difficultyDescription.text = languageSO != null ? languageSO.matchAISelectRandomDesc : "RANDOM";

            // NOTE: (INT) RANDOM IS EXCLUSIVE +1
            _lowestRankValue = Random.Range(1, (5 + 1));

            var range = Random.Range(0, 5);
            _highestRankValue = _lowestRankValue + range;
            _highestRankValue = _highestRankValue > 5 ? 5 : _highestRankValue;

            //_points = Random.Range(_aiSettingsSO.PointsMin, (_aiSettingsSO.PointsMax + 1));
            //_points += _points % 2 == 0 ? -1 : 0;

            _ply = Random.Range(_aiSettingsSO.PlyMin, _aiSettingsSO.PlyMax);

            _noise = Random.Range(_aiSettingsSO.NoiseMin, _aiSettingsSO.NoiseMax);

            ConfigureAISettingsToGame();
        }

        private void DeselectDifficultyButtonHighlights()
        {
            _easyButtonHighlight.color = Color.white;
            _mediumButtonHighlight.color = Color.white;
            _hardButtonHighlight.color = Color.white;
            _perfectButtonHighlight.color = Color.white;
            _randomButtonHighlight.color = Color.white;
        }

        // HANDLE HISTORIC DICE ROLLS

        private void LoadHistoricDiceRolls()
        {
            // MATCH DLC'S

            matchReplayDLCList = new List<Backgammon_Asset.MatchReplayDLC>();
            matchReplayDLCs = FindObjectsOfType<Backgammon_Asset.MatchReplayDLC>();

            foreach (Backgammon_Asset.MatchReplayDLC match in matchReplayDLCs)
            {
                // DO NOT INCLUDE 'AI' MATCH IN OPTIONS
                if (match.ID == "AI" || match.ID == "DEMO") continue;
                matchReplayDLCList.Add(match);
            }

            matchReplayDLCs = matchReplayDLCList.ToArray();

            // MATCHES

            matchList = new List<Backgammon_Asset.MatchData>();

            for (int matchDLCListCounter = 0; matchDLCListCounter < matchReplayDLCs.Length; ++matchDLCListCounter)
            {
                Backgammon_Asset.MatchReplayDLC matchReplayDLC = matchReplayDLCs[matchDLCListCounter];
                for (int matchInDLCCounter = 0; matchInDLCCounter < matchReplayDLC.MatchCount; ++matchInDLCCounter)
                {
                    Backgammon_Asset.MatchData match = matchReplayDLC.Match(matchInDLCCounter);
                    matchList.Add(match);
                }
            }

            matchArray = matchList.ToArray();

            selectedMatchIndex = 0;
            selectedMatch = matchArray[selectedMatchIndex];

            // GAMES

            gamesList = new List<Backgammon_Asset.GameData>();

            foreach (Backgammon_Asset.GameData game in selectedMatch.GameDataArray)
            {
                gamesList.Add(game);
            }

            gamesArray = gamesList.ToArray();

            selectedGameIndex = 0;
            selectedGame = gamesArray[selectedGameIndex];

            SelectRandomHistoricDiceRolls();
        }

        public void SelectRandomHistoricDiceRolls()
        {
            var randomMatch = selectedMatchIndex;

            while (selectedMatchIndex == randomMatch) { randomMatch = Random.Range(0, matchArray.Length); }

            selectedMatchIndex = randomMatch;
            selectedMatch = matchArray[selectedMatchIndex];

            foreach (Backgammon_Asset.GameData game in selectedMatch.GameDataArray)
            {
                gamesList.Add(game);
            }

            gamesArray = gamesList.ToArray();

            var randomGame = selectedGameIndex;

            while (selectedGameIndex == randomGame) { randomGame = Random.Range(0, gamesArray.Length); }

            selectedGameIndex = randomGame;
            selectedGame = gamesArray[selectedGameIndex];

            ConfigureAISettingsToGame();
        }

        public void IncrementHistoricDiceRolls()
        {
            if ((selectedGameIndex + 1) < selectedMatch.GameCount)
                selectedGame = gamesArray[++selectedGameIndex];
            else SelectRandomHistoricDiceRolls();

            ConfigureAISettingsToGame();
        }

        // MANUAL SETUP

        public void OnClickManualSetupBoard()
        {
            _manualSetupBoard = true;
            ConfigureAISettingsToMatch();
        }

        // CONFIGURE A.I.

        private void ConfigureAISettingsToGame()
        {
            // SET ALL PLAYER VALUES TO AI SETTINGS
            _aiSettingsSO.PlayerRankHighest = _highestRankValue;
            _aiSettingsSO.PlayerRankLowest = _lowestRankValue;
            _aiSettingsSO.PlayerPoints = _points;
            _aiSettingsSO.PlayerPly = _ply;
            _aiSettingsSO.PlayerNoise = _noise;
            _aiSettingsSO.GamesPlayed = 0;

            Main.Instance.IfPlayerVsHistoricAI = _ifHistoricDiceRolls;
            
            Game2D.AIIfUsingHistoricDice = _ifHistoricDiceRolls;
            Game2D.AIHistoricPlayingAsPlayer1 = true;
            Game2D.AIHistoricReplayAsOpponent = false;

            //Game2D.AIHistoricMatch = selectedMatch == null ? selectedMatch = matchArray[0] : selectedMatch;
            //Game2D.AIHistoricGame = selectedGame == null ? selectedGame = gamesArray[0] : selectedGame;
            //Game2D.AIHistoricIndexGame = selectedGameIndex;
        }

        private void ConfigureAISettingsToMatch()
        {
            // SET ALL PLAYER VALUES TO AI SETTINGS
            _aiSettingsSO.PlayerRankHighest = _highestRankValue;
            _aiSettingsSO.PlayerRankLowest = _lowestRankValue;
            _aiSettingsSO.PlayerPoints = _points;
            _aiSettingsSO.PlayerPly = _ply;
            _aiSettingsSO.PlayerNoise = _noise;
            _aiSettingsSO.PlayerPreset = _preset;
            _aiSettingsSO.GamesPlayed = 0;

            if (!_resetFromGamePause)
            {
                // RESET ALL VALUES - POINTS AND GAME CAN BE SEPARATE
                Backgammon_Asset.MatchData match = _aiMatch.Match(0);
                Backgammon_Asset.GameData game = match.Game(0);

                // NOTE: SCRIPTABLE OBJECT - DATA PERSISTENCE - MUST BE '0'
                game.GameConstructor(0, 0, game.Moves, 0, 0);

                match.GameDataArray[0] = game;

                _aiMatch.Match(0).MatchConstructor(
                    match.Title, match.ID, match.Event, match.Round,
                    _points,
                    match.Player1Surname, match.Player1, match.Player2Surname, match.Player2, match.Crawford,
                    match.GameDataArray);

                // RESET ENTRY IN PLAYER SCORE DATA
                PlayerScoreData scoreData = new PlayerScoreData();
                scoreData.matchKey = match.Title + " " + match.ID;
                Main.Instance.PlayerScoresObj.SetPlayerScoreData(scoreData);

                // SET GLOBAL VALUES FOR GAME
                MatchSelectUI.Match = _aiMatch.Match(0);
                GameListUI.IndexGame = 0;
                GameListUI.IndexTurn = 0;
                GameListUI.playingAs = PlayerId.Player1;
                GameListUI._playingAs2D = Game2D.PlayingAs.PLAYER_1;

                // NOTE: SCRIPTABLE OBJECT - RECONFIGURE BOARD
                _aiSettingsSO.ContinueBoardState.ResetToNewGame();
                _aiSettingsSO.PlayerTurnID = Game2D.PlayingAs.PLAYER_1;
            }

            Main.Instance.IfPlayerVsHistoricAI = _ifHistoricDiceRolls;
            Main.Instance.IfMatchedPlay = true;
            Main.Instance.IfPlayerVsAI = true;
        }

        // ----------------------------------------- FIELDS -----------------------------------------

        private LanguageScriptableObject languageSO = null;

        // USER INTERFACE

        private Text _setPointsButtonText = null;
        private Text _setDiceButtonText = null;
        private Text _setDifficultyButtonText = null;
        private Text _pointsSingleButtonText = null;
        private Text _pointsMatchButtonText = null;
        private Text _diceHistoricButtonText = null;
        private Text _diceRandomButtonText = null;
        private Text _startButtonText = null;
        private Text _backButtonText = null;

        private Image _easyButtonHighlight;
        private Image _mediumButtonHighlight;
        private Image _hardButtonHighlight;
        private Image _perfectButtonHighlight;
        private Image _randomButtonHighlight;

        private Text _easyButtonText;
        private Text _mediumButtonText;
        private Text _hardButtonText;
        private Text _perfectButtonText;
        private Text _randomButtonText;

        private Color _defaultButtonColour = Color.white;

        // HISTORIC DICE ROLLS

        private bool _toggleDiceOptionAutoProgress;
        private bool _ifHistoricDiceRolls;

        private List<Backgammon_Asset.MatchReplayDLC> matchReplayDLCList = null;
        private Backgammon_Asset.MatchReplayDLC[] matchReplayDLCs = null;

        private List<Backgammon_Asset.MatchData> matchList = null;
        private Backgammon_Asset.MatchData[] matchArray = null;

        private List<Backgammon_Asset.GameData> gamesList = null;
        private Backgammon_Asset.GameData[] gamesArray = null;

        private Backgammon_Asset.MatchData selectedMatch;
        private Backgammon_Asset.GameData selectedGame;
        
        private int selectedMatchIndex = 0;
        private int selectedGameIndex = 0;

        // A.I. SETTIGNS

        private int _highestRankValue = 1;
        private int _lowestRankValue = 5;
        private int _points = 5;
        private int _ply = 1;
        private float _noise = 0.01f;
        private string _preset = string.Empty;

        private bool _resetFromGamePause = false;
        private bool _manualSetupBoard = false;

        // ------------------------------------- GETTERS && SETTERS -------------------------------------

        public bool IfStart { get; internal set; }
        public bool IfBack { get; internal set; }
        public bool ManualSetupBoard
        {
            get => _manualSetupBoard;
            private set => _manualSetupBoard = value;
        }
    }
}