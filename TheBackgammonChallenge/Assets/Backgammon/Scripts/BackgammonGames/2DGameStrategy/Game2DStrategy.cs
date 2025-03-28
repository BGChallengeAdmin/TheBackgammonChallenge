using Backgammon_Asset;
using UnityEngine;
using UnityEngine.Assertions;
using static Backgammon.Game2D;

namespace Backgammon
{
    public class Game2DStrategy : MonoBehaviour, IBackgammonGame
    {
        [Header("STATE MACHINE")]
        [SerializeField] GameStateMachine2DStrategy _stateMachine = null;

        [Header("MANAGERS")]
        [SerializeField] Transform _boardContainer = null;
        [SerializeField] BoardMaterialsManager2D _boardMaterialsManager = null;
        [SerializeField] BarManager2D _barManager = null;
        [SerializeField] HomeManager2D _homeManager = null;
        [SerializeField] DoublingManager2D _doublingManager = null;
        [SerializeField] PointsManager2D _pointsManager = null;
        [SerializeField] CountersManager2D _countersManager = null;
        [SerializeField] DiceManager2D _diceManager = null;
        [SerializeField] ObserveAnalysisManager2D _observeAnalysisManager = null;

        [Header("DATA HANDLERS")]
        [SerializeField] AIDataHandler _aiDataHandler = null;

        [Header("A.I.")]
        [SerializeField] Backgammon_Asset.MatchData _aiMatchData = null;
        [SerializeField] Backgammon_Asset.GameData _aiGameData = null;
        [SerializeField] AISettingsScriptableObject _aiSettings = null;

        [Header("UI")]
        [SerializeField] Transform _fadeInFadeOutBackground = null;
        [SerializeField] GameScreenUI _gameScreenUI = null;
        [SerializeField] RollForGoesFirstUI _rollForGoesFirstUI = null;
        [SerializeField] BeforeCommenceUI _beforeCommenceUI = null;
        [SerializeField] DoublingUI _doublingUI = null;
        [SerializeField] DiceRollsUI _diceRollsUI = null;
        [SerializeField] AnalysisOrContinueUI _analysisOrContinueUI = null;
        [SerializeField] AnalysisUI _analysisUI = null;
        [SerializeField] ObserveBoardUI _observeBoardUI = null;
        [SerializeField] TurnEndsUI _turnEndsUI = null;
        [SerializeField] GameWonUI _gameWonUI = null;
        [SerializeField] GeneralInfoUI _generalInfoUI = null;
        [SerializeField] DemoUI _demoUI = null;
        [SerializeField] ChallengeProgressionUI _challengeProgressionUI = null;
        [SerializeField] ConfigureBoardManualUI _configureBoardManualUI = null;

        [Header("DEBUG PRO")]
        [SerializeField] int _debugGameIndex = 0;
        [SerializeField] int _debugFastForwardIndex = 0;

        [Header("DEBUG AI")]
        [SerializeField] bool _debugSetWhoGoesFirstOrSecondBool = false;
        [SerializeField] bool _debugSetPlayerGoesFirst = false;
        [SerializeField] bool _setDebugDice = false;
        [SerializeField] int _debugDice1 = 0;
        [SerializeField] int _debugDice2 = 0;
        [SerializeField] bool _debugAIPlaysItself = false;

        [Header("DEBUG OBJECTS")]
        [SerializeField] DebugPrefab _debugReportState = null;
        [SerializeField] DebugPrefab _debugObject = null;

        #region GameState

        private static GameStateContext2DStrategy _context;
        public static GameStateContext2DStrategy Context { get => _context; }
        
        //public IBackgammonContext Context { get => _context; }

        public GameStateMachine2DStrategy GameStateMachine { get => _stateMachine; }

        internal static bool IfGameInPlay = false;
        internal static bool IfUpdatingBoard;
        internal static bool IfGameConcluded = false;
        internal static bool IfExitAIGame = false;
        internal static bool IfManualSetupBoard = false;
        internal bool IfGameToContinue = false;
        internal bool IfAIGameToContinue = false;

        internal bool AIMatchWon;
        internal static bool AIIfUsingHistoricDice;
        internal static GameData AIHistoricGame;
        internal static int AIHistoricIndexTurn;
        internal static bool AIHistoricPlayingAsPlayer1;
        internal static bool AIHistoricReplayAsOpponent;

        internal static float TimeFactor;
        internal static int IndexTurn;

        #endregion

        protected void Awake()
        {
            ValidateConstraints();
        }

        protected void OnEnable()
        {
            _context = new GameStateContext2DStrategy(this, _boardMaterialsManager, _barManager, _homeManager,
                                            _doublingManager, _pointsManager, _countersManager,
                                            _diceManager, _observeAnalysisManager);
        }

        protected void Update()
        {
            if (_setDebugDice)
            {
                var gameTurn = _context.GameTurnsList.ToArray()[_context.TurnConfig.IndexTurn];

                if (_debugDice1 != 0) gameTurn.Dice1 = _debugDice1;
                if (_debugDice2 != 0) gameTurn.Dice2 = _debugDice2;

                _context.GameTurnsList.ToArray()[_context.TurnConfig.IndexTurn] = gameTurn;

                _debugDice1 = 0;
                _debugDice2 = 0;
                _setDebugDice = false;
                Context.GameConfig.IfFastForwarding = _debugAIPlaysItself;
            }
        }

        protected void OnDisable()
        {
            // ENSURE ALL UI AND DATA IS CLOSED DOWN
            if (Context is not null)
            {
                if (Context.GameScreenUI is not null) Context.GameScreenUI.SetActive(false);
                if (Context.RollForGoesFirstUI is not null) Context.RollForGoesFirstUI.SetActive(false);
                if (Context.BeforeCommenceUI is not null) Context.BeforeCommenceUI.SetActive(false);
                if (Context.DoublingUI is not null) Context.DoublingUI.SetActive(false);
                if (Context.DiceRollsUI is not null) Context.DiceRollsUI.SetActive(false);
                if (Context.AnalysisOrContinueUI is not null) Context.AnalysisOrContinueUI.SetActive(false);
                if (Context.AnalysisUI is not null) Context.AnalysisUI.SetActive(false);
                if (Context.ObserveBoardUI is not null) Context.ObserveBoardUI.SetActive(false);
                if (Context.TurnEndsUI is not null) Context.TurnEndsUI.SetActive(false);
                if (Context.GameWonUI is not null) Context.GameWonUI.SetActive(false);
                if (Context.GeneralInfoUI is not null) Context.GeneralInfoUI.SetActive(false);
                if (Context.GeneralInfoUI is not null) Context.GeneralInfoUI.SetActive(false);
                if (Context.ChallengeProgressionUI is not null) Context.ChallengeProgressionUI.SetActive(false);
            }
        }

        // -------------------------------------------- CONFIGURE GAME ---------------------------------------------------

        #region ConfigureContext
        public void ConfigureContextAndInit(bool continueProOrAI = true)
        {
            this.gameObject.SetActive(true);
            SetGameActive(true);

            Game2DStrategy.IfGameConcluded = false;

            var saveObject = continueProOrAI ? Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame
                                                : Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame;

            // SET CONTINUE MATCH
            if (IfGameToContinue)
            {
                MatchSelectUI.SetContinueMatchByID(saveObject.MatchID);
                GameListUI.IndexGame = saveObject.GameIndex;
                GameListUI._game = MatchSelectUI.Match.Game(saveObject.GameIndex);
                GameListUI._playingAs2D = saveObject.PlayingAs == 1 ? PlayingAs.PLAYER_1 : PlayingAs.PLAYER_2;
            }

            // DEBUG - SET INDEX GAME
            if (_debugGameIndex != 0)
            {
                var valid = _debugGameIndex < MatchSelectUI.Match.GameCount;
                GameListUI.IndexGame = valid ? _debugGameIndex : MatchSelectUI.Match.GameCount - 1;
            }

            // INTIALIZE NEW CONTEXT
            _context = new GameStateContext2DStrategy(this, _boardMaterialsManager, _barManager, _homeManager,
                                            _doublingManager, _pointsManager, _countersManager,
                                            _diceManager, _observeAnalysisManager);
            _context.SetDataHandlers(_aiDataHandler);
            _context.SetUI(_fadeInFadeOutBackground, _gameScreenUI, _rollForGoesFirstUI, _beforeCommenceUI,
                            _doublingUI, _diceRollsUI, _analysisOrContinueUI, _analysisUI, _observeBoardUI,
                            _turnEndsUI, _gameWonUI, _generalInfoUI, _demoUI, _challengeProgressionUI, _configureBoardManualUI);

            // MATCH
            _context.GameConfig.SelectedGame = _context.GameConfig.SelectedMatch.Game(_context.GameConfig.IndexGame);
            _context.GameConfig.IfGameToContinue = IfGameToContinue;
            _context.GameConfig.IfAIGameToContinue = IfAIGameToContinue;

            // AI MATCH
            _context.GameConfig.AIMatchData = Main.Instance.IfPlayerVsAI ? _aiMatchData : null;
            _context.GameConfig.AIGameData = Main.Instance.IfPlayerVsAI ? _aiGameData : null;
            _context.GameConfig.AISettings = Main.Instance.IfPlayerVsAI ? _aiSettings : null;

            // BOARD CONFIG
            _context.GameConfig.PlayingAs = GameListUI._playingAs2D;
            _context.GameConfig.OpponentAs = GameListUI._playingAs2D == PlayingAs.PLAYER_1 ? PlayingAs.PLAYER_2 : PlayingAs.PLAYER_1;
            _context.GameConfig.IfPlayingAsPlayer1 = GameListUI._playingAs2D == PlayingAs.PLAYER_1 ? true : false;
            _context.GameConfig.IfPlayFromLhs = SettingsUI.playingFrom == SettingsUI.PlayingFrom.LHS;
            _context.GameConfig.IfPlayerIsBlack = SettingsUI.playerColor == PlayerColour.BLACK;

            // GAME
            _context.TurnConfig.IndexTurn = 0;

            _context.TurnConfig.IfPlayer1Turn = true;
            _context.TurnConfig.IsPlayersTurn = _context.GameConfig.IfPlayingAsPlayer1;
            _context.TurnConfig.IfPlayer1GoesFirst = _context.GameConfig.SelectedMatch.Game(_context.GameConfig.IndexGame).GetPlayerMove(0) == ":" ? false : true;

            // AI GAME START
            if (Main.Instance.IfPlayerVsAI)
            {
                SetStartingPlayerToRandom();
                if (_debugSetWhoGoesFirstOrSecondBool) SetStartingPlayerWhoGoesFirst(_debugSetPlayerGoesFirst);

                if (IfAIGameToContinue)
                {
                    _context.GameConfig.AIGameData.Moves[0] = "11: 1/1 1/1";
                    _context.TurnConfig.IfPlayer1GoesFirst = saveObject.IsPlayerTurn ? true : false;
                    _context.TurnConfig.IfPlayer1Turn = _context.TurnConfig.IfPlayer1GoesFirst;
                    _context.TurnConfig.IsPlayersTurn = _context.TurnConfig.IfPlayer1GoesFirst;
                    _context.TurnConfig.IndexTurn = saveObject.TurnIndex;
                }
            }

            // UI
            _gameScreenUI.SetActive(true);
            _gameScreenUI.SetToAILayout(Main.Instance.IfPlayerVsAI);

            _gameScreenUI.Reset();
            _gameScreenUI.SetPlayerName("YOU");
            _gameScreenUI.SetPlayerCounterIcon(_context.GameConfig.IfPlayerIsBlack ? _context.CountersManager.DefaultBlackCounterImage :
                                                                          _context.CountersManager.DefaultWhiteCounterImage);
            _gameScreenUI.SetProPlayerName(_context.GameConfig.IfPlayingAsPlayer1 ?
                                        _context.GameConfig.SelectedMatch.Player1Surname : _context.GameConfig.SelectedMatch.Player2Surname);
            _gameScreenUI.SetProPlayerCounterIcon(_context.GameConfig.IfPlayerIsBlack ? _context.CountersManager.DefaultBlackCounterImage :
                                                                        _context.CountersManager.DefaultWhiteCounterImage);
            _gameScreenUI.SetOpponentName(_context.GameConfig.IfPlayingAsPlayer1 ?
                                        _context.GameConfig.SelectedMatch.Player2Surname : _context.GameConfig.SelectedMatch.Player1Surname);
            _gameScreenUI.SetOpponentCounterIcon(!_context.GameConfig.IfPlayerIsBlack ? _context.CountersManager.DefaultBlackCounterImage :
                                                                            _context.CountersManager.DefaultWhiteCounterImage);
            _gameScreenUI.SetMatchGameValues(_context.GameConfig.IndexGame + 1, _context.GameConfig.SelectedMatch.GameCount, _context.GameConfig.SelectedMatch.Points);

            var playerPoints = Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player1PointsAtStart :
                                                        Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player2PointsAtStart;

            var opponentPoints = Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player2PointsAtStart :
                                                        Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player1PointsAtStart;

            _gameScreenUI.SetMatchPointsWon(playerPoints, opponentPoints);

            _analysisUI.Init();
            _analysisUI.SetToAILayout(Main.Instance.IfPlayerVsAI, Context.GameConfig.SelectedMatch, Context.GameConfig.IfPlayingAsPlayer1);

            _challengeProgressionUI.Init();

            // INIT UI (ABOVE) BEFORE SHOWING SCREEN
            _fadeInFadeOutBackground.gameObject.SetActive(false);

            // FAST FORWARD
            _context.GameConfig.FastForwardTurnIndex = IfGameToContinue ? saveObject.TurnIndex : 0;
            if (_debugFastForwardIndex != 0) _context.GameConfig.FastForwardTurnIndex = _debugFastForwardIndex;

            if (_context.GameConfig.FastForwardTurnIndex >= Context.GameConfig.SelectedGame.Moves.Length)
                _context.GameConfig.FastForwardTurnIndex = Context.GameConfig.SelectedGame.Moves.Length - 4;
            if (Context.GameConfig.FastForwardTurnIndex > Context.TurnConfig.IndexTurn)
                Context.GameConfig.IfFastForwarding = true;

            // DEBUG
            _context.GameConfig.Debug_reportState = _debugReportState;
            _context.GameConfig.Debug_debugObject = _debugObject;

            // A.I. DEBUG - A.I. PLAYS AGAINST ITSELF - DEFAULT SHOULD BENO FAST FORWARD IN A.I.
            if (Main.Instance.IfPlayerVsAI) Context.GameConfig.IfFastForwarding = false;
            if (_debugAIPlaysItself) Context.GameConfig.IfFastForwarding = _debugAIPlaysItself;

            IfGameToContinue = false;
            IfAIGameToContinue = false;

            // TODO: SWITCH BODY EXPRESSION - SELECT GAME STRATEGY
            _context.SetGameStategy(SelectGameStrategy());

            _stateMachine.Init(_context);

            // BOARD MATERIALS - ENSURE CONTEXT IS INITIALIZED - ALL PREFABS CREATED
            //_boardMaterialsManager.Init(Main.Instance.BoardDesignSO);
        }

        private IStrategy SelectGameStrategy()
        {
            if (Main.Instance.IfPlayerVsPro) return new PlayervsProStrategy();
            else if (Main.Instance.IfPlayerVsAI) return new PlayervsAIStrategy();
            else return new PlayervsProStrategy();
        }

        public bool ConfigureContextAndInitForContinue(bool continueProOrAI)
        {
            var saveObject = continueProOrAI ? Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame
                                                : Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame;

            if (saveObject.IfMatchToContinue)
            {
                IfGameToContinue = true;
                IfAIGameToContinue = !continueProOrAI;
                ConfigureContextAndInit(continueProOrAI);
                return true;
            }
            else return false;
        }

        private void ValidateConstraints()
        {
            Assert.IsNotNull(_stateMachine, "MISSING: STATE MACHINE");

            Assert.IsNotNull(_boardMaterialsManager, "MISSING: BOARD MATERIALS MANAGER");
            Assert.IsNotNull(_barManager, "MISSING: BAR MANAGER");
            Assert.IsNotNull(_homeManager, "MISSING: HOME MANAGER");
            Assert.IsNotNull(_doublingManager, "MISSING: DOUBLING MANAGER");
            Assert.IsNotNull(_pointsManager, "MISSING: POINTS MANAGER");
            Assert.IsNotNull(_countersManager, "MISSING: COUNTERS MANAGER");
            Assert.IsNotNull(_diceManager, "MISSING: DICE MANAGER");
            Assert.IsNotNull(_observeAnalysisManager, "MISSING: OBSERVE ANALYSIS MANAGER");

            Assert.IsNotNull(_aiDataHandler, "MISSING: AI DATA HANDLER");

            Assert.IsNotNull(_fadeInFadeOutBackground, "MISSING: DEFAULT BACKGROUND");
            Assert.IsNotNull(_gameScreenUI, "MISSING: GAME SCREEN");
            Assert.IsNotNull(_rollForGoesFirstUI, "MISSING: ROLL FOR GOES FIRST SCREEN");
            Assert.IsNotNull(_beforeCommenceUI, "MISSING: BEFORE COMMENCE");
            Assert.IsNotNull(_doublingUI, "MISSING: DOUBLING UI");
            Assert.IsNotNull(_diceRollsUI, "MISSING: DICE UI");
            Assert.IsNotNull(_analysisOrContinueUI, "MISSING: ANALYSIS OR CONTINUE UI");
            Assert.IsNotNull(_analysisUI, "MISSING: ANALYSIS UI");
            Assert.IsNotNull(_observeBoardUI, "MISSING: OBSERVE BOARD UI");
            Assert.IsNotNull(_turnEndsUI, "MISSING: TURN ENDS UI");
            Assert.IsNotNull(_gameWonUI, "MISSING: GAME WON UI");
            Assert.IsNotNull(_generalInfoUI, "MISSING: GENERAL INFO UI");
            Assert.IsNotNull(_demoUI, "MISSING: DEMO UI");
            Assert.IsNotNull(_challengeProgressionUI, "MISSING: CHALLENGE PROGRESSION UI");
        }

        private void SetGameActive(bool active)
        {
            _boardContainer.gameObject.SetActive(active);
            _boardMaterialsManager.gameObject.SetActive(active);
            _barManager.gameObject.SetActive(active);
            _homeManager.gameObject.SetActive(active);
            _doublingManager.gameObject.SetActive(active);
            _pointsManager.gameObject.SetActive(active);
            _countersManager.gameObject.SetActive(active);
            _diceManager.gameObject.SetActive(active);
            _observeAnalysisManager.gameObject.SetActive(active);
        }

        public void ExitGame()
        {
            _stateMachine.Exit();
            SetGameActive(false);
            this.gameObject.SetActive(false);
        }

        public void SetUseDebugReportState(bool useDebugLogging)
        {
            _debugReportState.ShowMesssage = useDebugLogging;
        }

        public void SetUseDebugGameObject(bool useDebugLogging)
        {
            _debugObject.ShowMesssage = useDebugLogging;
        }

        public static void SetStartingPlayerToRandom()
        {
            _context.GameConfig.AIGameData.Moves[0] = Random.Range(0, 2) == 1 ? "11: 1/1 1/1" : ":";
            _context.TurnConfig.IfPlayer1GoesFirst = _context.GameConfig.AIGameData.Moves[0] == ":" ? false : true;
            _context.TurnConfig.IfPlayer1Turn = _context.TurnConfig.IfPlayer1GoesFirst;
            _context.TurnConfig.IsPlayersTurn = _context.TurnConfig.IfPlayer1GoesFirst;
        }

        public static void SetStartingPlayerWhoGoesFirst(bool playerOrPro)
        {
            _context.GameConfig.AIGameData.Moves[0] = playerOrPro ? "11: 1/1 1/1" : ":";
            _context.TurnConfig.IfPlayer1GoesFirst = _context.GameConfig.AIGameData.Moves[0] == ":" ? false : true;
            _context.TurnConfig.IfPlayer1Turn = _context.TurnConfig.IfPlayer1GoesFirst;
            _context.TurnConfig.IsPlayersTurn = _context.TurnConfig.IfPlayer1GoesFirst;
        }

        #endregion
    }
}