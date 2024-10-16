using UnityEngine;
using UnityEngine.Assertions;

namespace Backgammon
{
    public class Game2D : MonoBehaviour
    {
        [Header("STATE MACHINE")]
        [SerializeField] GameStateMachine2D _stateMachine = null;

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
        [SerializeField] ChallengeProgressionUI _challengeProgressionUI = null;
        [SerializeField] ConfigureBoardManualUI _configureBoardManualUI = null;

        [Header("DEBUG")]
        [SerializeField] bool _debugSetFirstOrSecond = false;
        [SerializeField] bool _debugSetPlayerGoesFirst = false;
        [SerializeField] bool _resetDebugDice = false;
        [SerializeField] int _debugDice1 = 0;
        [SerializeField] int _debugDice2 = 0;
        [SerializeField] bool _debugAIPlaysItself = false;
        [SerializeField] int _debugGameIndex = 0;
        [SerializeField] int _debugFastForwardIndex = 0;
        [SerializeField] DebugPrefab _debugReportState = null;
        [SerializeField] DebugPrefab _debugObject = null;

        private void Awake()
        {
            ValidateConstraints();
        }

        private void OnEnable()
        {
            _context = new GameStateContext2D(this, _boardMaterialsManager, _barManager, _homeManager,
                                            _doublingManager, _pointsManager, _countersManager,
                                            _diceManager, _observeAnalysisManager);
        }

        private void Update()
        {
            if (_resetDebugDice)
            {
                _debugDice1 = 0;
                _debugDice2 = 0;
                _resetDebugDice = false;
                Context.IfFastForwarding = _debugAIPlaysItself;
            }

            if (_debugDice1 != 0) _context.Dice1 = _debugDice1;
            if (_debugDice2 != 0) _context.Dice2 = _debugDice2;
        }

        private void OnDisable()
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
                if (Context.ChallengeProgressionUI is not null) Context.ChallengeProgressionUI.SetActive(false);
            }
        }

        // -------------------------------------------- CONFIGURE GAME ---------------------------------------------------

        #region ConfigureContext
        internal void ConfigureContextAndInit(bool continueProOrAI = true)
        {
            this.gameObject.SetActive(true);
            SetGameActive(true);

            Game2D.IfGameConcluded = false;

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
            _context = new GameStateContext2D(this, _boardMaterialsManager, _barManager, _homeManager,
                                            _doublingManager, _pointsManager, _countersManager,
                                            _diceManager, _observeAnalysisManager);
            _context.SetDataHandlers(_aiDataHandler);
            _context.SetUI(_fadeInFadeOutBackground, _gameScreenUI, _rollForGoesFirstUI, _beforeCommenceUI,
                            _doublingUI, _diceRollsUI, _analysisOrContinueUI, _analysisUI, _observeBoardUI,
                            _turnEndsUI, _gameWonUI, _generalInfoUI, _challengeProgressionUI, _configureBoardManualUI);

            // MATCH
            _context.SelectedGame = _context.SelectedMatch.Game(_context.IndexGame);
            _context.IfGameToContinue = IfGameToContinue;
            _context.IfAIGameToContinue = IfAIGameToContinue;

            // AI MATCH
            _context.AIMatchData = Main.Instance.IfPlayerVsAI ? _aiMatchData : null;
            _context.AIGameData = Main.Instance.IfPlayerVsAI ? _aiGameData : null;
            _context.AISettings = Main.Instance.IfPlayerVsAI ? _aiSettings : null;

            // BOARD CONFIG
            _context.PlayingAs = GameListUI._playingAs2D;
            _context.OpponentAs = GameListUI._playingAs2D == PlayingAs.PLAYER_1 ? PlayingAs.PLAYER_2 : PlayingAs.PLAYER_1;
            _context.IfPlayingAsPlayer1 = GameListUI._playingAs2D == PlayingAs.PLAYER_1 ? true : false;
            _context.IfPlayFromLhs = true;
            _context.IfPlayerIsBlack = true;

            // GAME
            _context.IndexTurn = 0;

            _context.IfPlayer1Turn = true;
            _context.IsPlayersTurn = _context.IfPlayingAsPlayer1;
            _context.IfPlayer1GoesFirst = _context.SelectedMatch.Game(_context.IndexGame).GetPlayerMove(0) == ":" ? false : true;

            // AI GAME
            if (Main.Instance.IfPlayerVsAI)
            {
                // 50 / 50 THAT PLAYER GOES FIRST -> SET VALUE TO PREVENT SKIPPING FIRST TURN
                _context.AIGameData.Moves[0] = Random.Range(0, 2) == 1 ? "11: 1/1 1/1" : ":";
                if (_debugSetFirstOrSecond) _context.AIGameData.Moves[0] = _debugSetPlayerGoesFirst ? "11: 1/1 1/1" : ":";
                _context.IfPlayer1GoesFirst = _context.AIGameData.Moves[0] == ":" ? false : true;
                _context.IfPlayer1Turn = _context.IfPlayer1GoesFirst;
                _context.IsPlayersTurn = _context.IfPlayer1GoesFirst;

                if (IfAIGameToContinue)
                {
                    _context.AIGameData.Moves[0] = "11: 1/1 1/1";
                    _context.IfPlayer1GoesFirst = saveObject.IsPlayerTurn ? true : false;
                    _context.IfPlayer1Turn = _context.IfPlayer1GoesFirst;
                    _context.IsPlayersTurn = _context.IfPlayer1GoesFirst;
                    _context.AIIndexTurn = saveObject.TurnIndex;
                }
            }

            // UI
            _gameScreenUI.SetActive(true);
            _gameScreenUI.SetToAILayout(Main.Instance.IfPlayerVsAI);
            _gameScreenUI.Reset();
            _gameScreenUI.SetPlayerName("YOU");
            _gameScreenUI.SetPlayerCounterIcon(_context.IfPlayerIsBlack ? _context.CountersManager.DefaultBlackCounterImage :
                                                                          _context.CountersManager.DefaultWhiteCounterImage);
            _gameScreenUI.SetProPlayerName(_context.IfPlayingAsPlayer1 ?
                                        _context.SelectedMatch.Player1Surname : _context.SelectedMatch.Player2Surname);
            _gameScreenUI.SetProPlayerCounterIcon(_context.IfPlayerIsBlack ? _context.CountersManager.DefaultBlackCounterImage :
                                                                          _context.CountersManager.DefaultWhiteCounterImage);
            _gameScreenUI.SetOpponentName(_context.IfPlayingAsPlayer1 ?
                                        _context.SelectedMatch.Player2Surname : _context.SelectedMatch.Player1Surname);
            _gameScreenUI.SetOpponentCounterIcon(!_context.IfPlayerIsBlack ? _context.CountersManager.DefaultBlackCounterImage :
                                                                          _context.CountersManager.DefaultWhiteCounterImage);
            _gameScreenUI.SetMatchGameValues(_context.IndexGame + 1, _context.SelectedMatch.GameCount, _context.SelectedMatch.Points);

            var playerPoints = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Game(Context.IndexGame).Player1PointsAtStart :
                                                        Context.SelectedMatch.Game(Context.IndexGame).Player2PointsAtStart;

            var opponentPoints = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Game(Context.IndexGame).Player2PointsAtStart :
                                                        Context.SelectedMatch.Game(Context.IndexGame).Player1PointsAtStart;

            _gameScreenUI.SetMatchPointsWon(playerPoints, opponentPoints);

            _analysisUI.Init();
            _analysisUI.SetToAILayout(Main.Instance.IfPlayerVsAI);

            _gameWonUI.Init();
            _gameWonUI.SetToAILayout(Main.Instance.IfPlayerVsAI);

            _challengeProgressionUI.Init();

            // INIT UI (ABOVE) BEFORE SHOWING SCREEN
            _fadeInFadeOutBackground.gameObject.SetActive(false);

            // FAST FORWARD
            _context.FastForwardTurnIndex = IfGameToContinue ? saveObject.TurnIndex : 0;
            if (_debugFastForwardIndex != 0) _context.FastForwardTurnIndex = _debugFastForwardIndex;

            if (_context.FastForwardTurnIndex >= Context.SelectedGame.Moves.Length)
                _context.FastForwardTurnIndex = Context.SelectedGame.Moves.Length - 3;
            if (Context.FastForwardTurnIndex > Context.IndexTurn)
                Context.IfFastForwarding = true;

            // DEBUG
            _context.Debug_reportState = _debugReportState;
            _context.Debug_debugObject = _debugObject;

            // A.I. DEBUG - A.I. PLAYS AGAINST ITSELF
            if (_debugAIPlaysItself) Context.IfFastForwarding = _debugAIPlaysItself;
            //

            IfGameToContinue = false;
            IfAIGameToContinue = false;

            _stateMachine.Init(_context);

            // BOARD MATERIALS - ENSURE CONTEXT IS INITIALIZED - ALL PREFABS CREATED
            //_boardMaterialsManager.Init(Main.Instance.BoardDesignSO);
        }

        internal bool ConfigureContextAndInitForContinue(bool continueProOrAI)
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

        internal void ExitGame()
        {
            _stateMachine.Exit();
            SetGameActive(false);
            this.gameObject.SetActive(false);
        }

        internal void SetUseDebugReportState(bool useDebugLogging)
        {
            _debugReportState.ShowMesssage = useDebugLogging;
        }

        internal void SetUseDebugGameObject(bool useDebugLogging)
        {
            _debugObject.ShowMesssage = useDebugLogging;
        }

        #endregion

        // -------------------------------------------- GAME STATE ----------------------------------------------

        #region GameState

        private static GameStateContext2D _context;
        public static GameStateContext2D Context { get => _context; }
        public GameStateMachine2D GameStateMachine { get => _stateMachine; }

        internal static bool IfGameInPlay = false;
        internal static bool IfUpdatingBoard;
        internal static bool IfGameConcluded = false;
        internal static bool IfExitAIGame = false;
        internal static bool IfManualSetupBoard = false;
        internal bool IfGameToContinue = false;
        internal bool IfAIGameToContinue = false;

        internal bool AIMatchWon;
        internal static bool AIIfUsingHistoricDice;
        internal static bool AIHistoricPlayingAsPlayer1;
        internal static bool AIHistoricReplayAsOpponent;

        internal static float TimeFactor;
        internal static int IndexTurn;

        #endregion

        // ------------------------------------------- DATA STRUCTURES ------------------------------------------

        public enum PlayerColour
        {
            BLACK,
            WHITE,
            NONE
        }

        public enum PlayingAs
        {
            PLAYER_1,
            PLAYER_2,
            NONE
        }
    }
}