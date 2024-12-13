using UnityEngine;
using UnityEngine.Assertions;

namespace Backgammon
{
    public class Game : MonoBehaviour
    {
        [Header("STATE MACHINE")]
        [SerializeField] GameStateMachine _stateMachine = null;

        [Header("MANAGERS")]
        [SerializeField] BoardMaterialsManager _boardMaterialsManager = null;
        [SerializeField] BarManager _barManager = null;
        [SerializeField] HomeManager _homeManager = null;
        [SerializeField] DoublingManager _doublingManager = null; 
        [SerializeField] PointsManager _pointsManager = null;
        [SerializeField] CountersManager _countersManager = null;
        [SerializeField] DiceManager _diceManager = null;
        [SerializeField] ObserveAnalysisManager _observeAnalysisManager = null;

        [Header("DATA HANDLERS")]
        [SerializeField] AIDataHandler _aiDataHandler = null;

        [Header("UI")]
        [SerializeField] Transform _defaultBackground = null;
        [SerializeField] Transform _baseLayer = null;
        [SerializeField] GameScreenUI _gameScreenUI = null;
        [SerializeField] BeforeCommenceUI _beforeCommenceUI = null;
        [SerializeField] DoublingUI _doublingUI = null;
        [SerializeField] DiceRollsUI _diceRollsUI = null;
        [SerializeField] AnalysisUI _analysisUI = null;
        [SerializeField] ObserveBoardUI _observeBoardUI = null;
        [SerializeField] TurnEndsUI _turnEndsUI = null;
        [SerializeField] GameWonUI _gameWonUI = null;
        [SerializeField] GeneralInfoUI _generalInfoUI = null;
        [SerializeField] ChallengeProgressionUI _challengeProgressionUI = null;

        protected void Awake()
        {
            ValidateConstraints();
        }

        protected void Update()
        {
            //if (Input.GetKeyDown("r"))
            //{
            //    ResetBoardLayout();
            //}
            //if (Input.GetKeyDown("1"))
            //{
            //    SetCountersToPlayerStart();
            //}
            //if (Input.GetKeyDown("2"))
            //{
            //    SetCountersToOpponentStart();
            //}
            //if (Input.GetKeyDown("3"))
            //{
            //    SetCountersToPlayerHome();
            //}
            //if (Input.GetKeyDown("4"))
            //{
            //    SetCountersToOpponentHome();
            //}
            //if (Input.GetKeyDown("5"))
            //{
            //    SetCountersToPlayerBar();
            //    SetCountersToOpponentBar();
            //}
        }

        protected void OnDisable()
        {
            // ENSURE ALL UI IS CLOSED DOWN
            //if (Context.GameScreenUI is not null) Context.GameScreenUI.SetActive(false);
            //if (Context.BeforeCommenceUI is not null) Context.BeforeCommenceUI.SetActive(false);
            //if (Context.DoublingUI is not null) Context.DoublingUI.SetActive(false);
            //if (Context.DiceRollsUI is not null) Context.DiceRollsUI.SetActive(false);
            //if (Context.AnalysisUI is not null) Context.AnalysisUI.SetActive(false);
            //if (Context.ObserveBoardUI is not null) Context.ObserveBoardUI.SetActive(false);
            //if (Context.TurnEndsUI is not null) Context.TurnEndsUI.SetActive(false);
            //if (Context.GameWonUI is not null) Context.GameWonUI.SetActive(false);
            //if (Context.GeneralInfoUI is not null) Context.GeneralInfoUI.SetActive(false);
            //if (Context.ChallengeProgressionUI is not null) Context.ChallengeProgressionUI.SetActive(false);

            //if (_defaultBackground.gameObject.activeSelf) _defaultBackground.gameObject.SetActive(true);
        }

        // -------------------------------------------- CONFIGURE GAME ---------------------------------------------------
        #region GameContext
        public void ResetGameContext()
        {
            Game.IfGameConcluded = false;
            SetGameActive(true);

            // INTIALIZE CONTEXT
            _context = new GameStateContext(this, _boardMaterialsManager, _barManager, _homeManager, 
                                            _doublingManager,  _pointsManager, _countersManager, 
                                            _diceManager, _observeAnalysisManager);
            _context.SetDataHandlers(_aiDataHandler);
            _context.SetUI(_gameScreenUI, _beforeCommenceUI, _doublingUI, _diceRollsUI, 
                            _analysisUI, _observeBoardUI, _turnEndsUI, _gameWonUI, _generalInfoUI,
                            _challengeProgressionUI);

            // MATCH
            _context.SelectedGame = _context.SelectedMatch.Game(_context.IndexGame);

            // BOARD CONFIG
            _context.PlayingAs = GameListUI._playingAs;
            _context.IfPlayingAsPlayer1 = GameListUI._playingAs == Game.PlayingAs.PLAYER_1 ? true : false;
            _context.IfPlayFromLhs = true;
            _context.IfPlayerIsBlack = true;

            // GAME
            _context.IndexTurn = 0;

            _context.IfPlayer1Turn = true;
            _context.IsPlayersTurn = _context.IfPlayingAsPlayer1;
            _context.IfPlayer1GoesFirst = _context.SelectedMatch.Game(_context.IndexGame).GetPlayerMove(0) == ":" ? false : true;

            // UI
            // NOTE: USING AS BACKGROUND
            //_gameScreenUI.SetActive(true);
            //_gameScreenUI.Reset();
            //_gameScreenUI.SetPlayerName("YOU");
            //_gameScreenUI.SetProPlayerName(_context.IfPlayingAsPlayer1 ? 
            //                            _context.SelectedMatch.Player1Surname : _context.SelectedMatch.Player2Surname);
            //_gameScreenUI.SetOpponentName(_context.IfPlayingAsPlayer1 ? 
            //                            _context.SelectedMatch.Player2Surname : _context.SelectedMatch.Player1Surname);

            //_observeAnalysisManager.Init();
            //_challengeProgressionUI.Init();
            
            // INIT UI (ABOVE) BEFORE SHOWING SCREEN
            _defaultBackground.gameObject.SetActive(false);

            // FAST FORWARD
            _context.FastForwardTurnIndex = Context.SelectedGame.Moves.Length - 1; //22 - 41

            if (_context.FastForwardTurnIndex >= Context.SelectedGame.Moves.Length) _context.FastForwardTurnIndex -= 5;
            if (Context.FastForwardTurnIndex > Context.IndexTurn) Context.IfFastForwarding = true;

            _stateMachine.Init(_context);

            // BOARD MATERIALS - ENSURE CONTEXT IS INITIALIZED - ALL PREFABS CREATED
            //_boardMaterialsManager.Init(Main.Instance.BoardDesignSO);

            //ResetBoardLayout();
        }

        internal void ResetBoardLayout()
        {
            Game.IfGameConcluded = false;
            SetGameActive(true);

            // INTIALIZE CONTEXT
            _context = new GameStateContext(this, _boardMaterialsManager, _barManager, _homeManager, 
                                            _doublingManager, _pointsManager, _countersManager, 
                                            _diceManager, _observeAnalysisManager);

            IfPlayFromLhs = true;
            IfPlayerIsBlack = true;

            // INSTANTIATE BAR
            _barManager.Init();

            // TODO: INSTANTIATE CUBE
            _doublingManager.Init(IfPlayFromLhs);

            // INSTANTIATE HOME
            _homeManager.Init(IfPlayFromLhs);

            // INSTANTIATE POINTS - IS RE-DONE IN CASE PLAYER CHANGES PLAYING LEFT / RIGHT
            _pointsManager.InitPointsFrom24(IfPlayFromLhs);

            // INIT COUNTERS
            _countersManager.Init();
            _countersManager.ZeroCounterPositions();

            // SET PLAYER / OPPONENT COUNTERS TO NEW GAME

            SetCountersToPlayerStart();
            SetCountersToOpponentStart();

            //SetCountersToPlayerHome();
            //SetCountersToOpponentHome();

            _observeAnalysisManager.Init();

            // TODO: UPDATE DICE ROLLS MANAGER

            // TODO: REPLAY DIALOG UI / MENU BAR / PLAYER INFO
        }

        internal void SetGameActive(bool active)
        {
            //_baseLayer.gameObject.SetActive(active);
            _boardMaterialsManager.gameObject.SetActive(active);
            _barManager.gameObject.SetActive(active);
            _homeManager.gameObject.SetActive(active);
            _doublingManager.gameObject.SetActive(active);
            _pointsManager.gameObject.SetActive(active);
            _countersManager.gameObject.SetActive(active);
            _diceManager.gameObject.SetActive(active);
            _observeAnalysisManager.gameObject.SetActive(active);
        }

        internal void SetGameDisabled()
        {
            SetGameActive(false);
            IfGameConcluded = true;
            _context.ExitFromStateMachine = true;
        }

        public PlayerId playingAs = PlayerId.None;
        bool IfPlayFromLhs;
        bool IfPlayerIsBlack;

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

            Assert.IsNotNull(_defaultBackground, "MISSING: DEFAULT BACKGROUND");
            Assert.IsNotNull(_gameScreenUI, "MISSING: GAME SCREEN");
            Assert.IsNotNull(_beforeCommenceUI, "MISSING: BEFORE COMMENCE");
            Assert.IsNotNull(_doublingUI, "MISSING: DOUBLING UI");
            Assert.IsNotNull(_diceRollsUI, "MISSING: DICE UI");
            Assert.IsNotNull(_analysisUI, "MISSING: ANALYSIS UI");
            Assert.IsNotNull(_observeBoardUI, "MISSING: OBSERVE BOARD UI");
            Assert.IsNotNull(_turnEndsUI, "MISSING: TURN ENDS UI");
            Assert.IsNotNull(_gameWonUI, "MISSING: GAME WON UI");
            Assert.IsNotNull(_generalInfoUI, "MISSING: GENERAL INFO UI");
            Assert.IsNotNull(_challengeProgressionUI, "MISSING: CHALLENGE PROGRESSION UI");
        }

        #endregion

        // ------------------------------------------ HELPER METHODS -------------------------------------------

        #region HelperMethods
        // START
        private void SetCountersToPlayerStart()
        {
            _barManager.ResetPlayerBar();
            _homeManager.ResetPlayerHome();

            _pointsManager.SetCountersToStartingPoints(true,
                                                        IfPlayerIsBlack ? _countersManager.BlackCounters :
                                                                           _countersManager.WhiteCounters);
        }

        private void SetCountersToOpponentStart()
        {
            _barManager.ResetOpponentBar();
            _homeManager.ResetOpponentHome();

            _pointsManager.SetCountersToStartingPoints(false,
                                                        !IfPlayerIsBlack ? _countersManager.BlackCounters :
                                                                            _countersManager.WhiteCounters);
        }

        // BAR
        private void SetCountersToPlayerBar()
        {
            var counters = IfPlayerIsBlack ? _countersManager.BlackCounters : _countersManager.WhiteCounters;

            foreach (Counter counter in counters)
            {
                _ = _pointsManager.GetPlayerPointByID(counter.ParentID).PopCounter();
                counter.SetCounterToMoveToPosition(_barManager.PlayerBar.GetCounterOffsetPosition());
                _barManager.PlayerBar.PushCounter(counter);
            }
        }

        private void SetCountersToOpponentBar()
        {
            var counters = !IfPlayerIsBlack ? _countersManager.BlackCounters : _countersManager.WhiteCounters;

            foreach (Counter counter in counters)
            {
                _ = _pointsManager.GetPlayerPointByID(counter.ParentID).PopCounter();
                counter.SetCounterToMoveToPosition(_barManager.OpponentBar.GetCounterOffsetPosition());
                _barManager.OpponentBar.PushCounter(counter);
            }
        }

        // HOME
        private void SetCountersToPlayerHome()
        {
            var counters = IfPlayerIsBlack ? _countersManager.BlackCounters : _countersManager.WhiteCounters;

            foreach (Counter counter in counters)
            {
                _ = _pointsManager.GetPlayerPointByID(counter.ParentID).PopCounter();
                counter.SetCounterToMoveToHome(_homeManager.PlayerHome.GetCounterOffsetPosition());
                _homeManager.PlayerHome.PushCounter(counter);
            }
        }

        private void SetCountersToOpponentHome()
        {
            var counters = !IfPlayerIsBlack ? _countersManager.BlackCounters : _countersManager.WhiteCounters;

            foreach (Counter counter in counters)
            {
                _ = _pointsManager.GetPlayerPointByID(counter.ParentID).PopCounter();
                counter.SetCounterToMoveToHome(_homeManager.OpponentHome.GetCounterOffsetPosition());
                _homeManager.OpponentHome.PushCounter(counter);
            }
        }

        // EDGE
        private void SetAllCountersToEdge()
        {
            foreach (Counter counter in _countersManager.BlackCounters)
            {
                counter.SetCounterToMoveToEdge();
            }
            
            foreach (Counter counter in _countersManager.WhiteCounters)
            {
                counter.SetCounterToMoveToEdge();
            }
        }

        #endregion

        // -------------------------------------------- GAME STATE ----------------------------------------------
        #region GameState

        private static GameStateContext _context;
        public static GameStateContext Context { get => _context; }
        public GameStateMachine GameStateMachine { get => _stateMachine; }

        internal static bool IfGameInPlay = false;
        internal static bool IfUpdatingBoard;
        internal static bool IfGameConcluded = true;
        internal static bool IfExitAIGame = false;

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