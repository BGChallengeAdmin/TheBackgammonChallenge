namespace Backgammon
{
    public class GameStateContext
    {
        public GameStateContext(Game game, BoardMaterialsManager boardMaterialsManager, BarManager barManager, 
                                HomeManager homeManager, DoublingManager doublingManager, PointsManager pointsManager, 
                                CountersManager countersManager, DiceManager diceManager, 
                                ObserveAnalysisManager observeAnalysisManager)
        {
            this._game = game;
            _boardMaterialsManager = boardMaterialsManager;
            _barManager = barManager;
            _homeManager = homeManager;
            _doublingManager = doublingManager;
            _pointsManager = pointsManager;
            _countersManager = countersManager;
            _diceManager = diceManager;
            _observeAnalysisManager = observeAnalysisManager;

            PlayerMovesInfo = new MoveInfo[4];
            RecordedMovesInfo = new MoveInfo[4];
            TopRankedMovesInfo = new MoveInfo[4];
        }

        internal void SetDataHandlers(AIDataHandler aiDataHandler)
        {
            _aiDataHAndler = aiDataHandler;
        }

        internal void SetUI(GameScreenUI gameScreenUI, BeforeCommenceUI beforeCommenceUI, DoublingUI doublingUI,
                            DiceRollsUI diceRollsUI, AnalysisUI analysisUI, ObserveBoardUI observeBoardUI,
                            TurnEndsUI turnEndsUI, GameWonUI gameWonUI, GeneralInfoUI generalInfoUI,
                            ChallengeProgressionUI challengeProgressionUI)
        {
            _gameScreenUI = gameScreenUI;
            _beforeCommenceUI = beforeCommenceUI;
            _doublingUI = doublingUI;
            _diceRollsUI = diceRollsUI;
            _analysisUI = analysisUI;
            _observeBoardUI = observeBoardUI;
            _turnEndsUI = turnEndsUI;
            _gameWonUI = gameWonUI;
            _generalInfoUI = generalInfoUI;
            _challengeProgressionUI = challengeProgressionUI;
        }

        // GAME
        private Game _game;
        public Game Game { get => _game; }

        // MANAGERS
        private BoardMaterialsManager _boardMaterialsManager;
        private BarManager _barManager;
        private HomeManager _homeManager;
        private DoublingManager _doublingManager;
        private PointsManager _pointsManager;
        private CountersManager _countersManager;
        private DiceManager _diceManager;
        private ObserveAnalysisManager _observeAnalysisManager;

        internal BoardMaterialsManager BoardMaterialsManager { get => _boardMaterialsManager; }
        internal BarManager BarManager { get => _barManager; }
        internal HomeManager HomeManager { get => _homeManager; }
        internal DoublingManager DoublingManager { get => _doublingManager; }
        internal PointsManager PointsManager { get => _pointsManager; }
        internal CountersManager CountersManager { get => _countersManager; }
        internal DiceManager DiceManager { get => _diceManager; }
        internal ObserveAnalysisManager ObserveAnalysisManager { get => _observeAnalysisManager; }

        // DATA HANDLERS
        private AIDataHandler _aiDataHAndler;

        public AIDataHandler AIDataHandler { get => _aiDataHAndler; }

        // UI
        private GameScreenUI _gameScreenUI = null;
        private BeforeCommenceUI _beforeCommenceUI;
        private DoublingUI _doublingUI;
        private DiceRollsUI _diceRollsUI;
        private AnalysisUI _analysisUI;
        private ObserveBoardUI _observeBoardUI;
        private TurnEndsUI _turnEndsUI;
        private GameWonUI _gameWonUI;
        private GeneralInfoUI _generalInfoUI;
        private ChallengeProgressionUI _challengeProgressionUI;

        internal GameScreenUI GameScreenUI { get => _gameScreenUI; }
        internal BeforeCommenceUI BeforeCommenceUI { get => _beforeCommenceUI; }
        internal DoublingUI DoublingUI { get => _doublingUI; }
        internal DiceRollsUI DiceRollsUI { get => _diceRollsUI; }
        internal AnalysisUI AnalysisUI { get => _analysisUI; }
        internal ObserveBoardUI ObserveBoardUI { get => _observeBoardUI; }
        internal TurnEndsUI TurnEndsUI { get => _turnEndsUI; }
        internal GameWonUI GameWonUI { get => _gameWonUI; }
        internal GeneralInfoUI GeneralInfoUI { get => _generalInfoUI; }
        internal ChallengeProgressionUI ChallengeProgressionUI {  get => _challengeProgressionUI; }

        // MATCH CONFIG
        internal Backgammon_Asset.MatchData SelectedMatch = MatchSelectUI.Match;
        public int IndexGame = GameListUI.IndexGame;
        public Backgammon_Asset.GameData SelectedGame = GameListUI._game;

        // BOARD CONFIG
        public Game.PlayingAs PlayingAs = Game.PlayingAs.PLAYER_1;
        public bool IfPlayingAsPlayer1 = true;
        public bool IfPlayFromLhs = true;
        public bool IfPlayerIsBlack = true;

        // GAME CONFIG
        public int IndexTurn = 0;
        public bool IsPlayersTurn = true;
        public bool IsPlayersMakingMoves = true;
        public bool IfPlayer1Turn = true;
        public bool IfPlayer1GoesFirst = true;

        // TURN CONFIG
        public bool BeginTurnDialog = false;
        public bool PlayerIsUnableToMove = false;
        public bool PlayerOfferedDoubles = false;
        public bool DoublingTakesOrDrops = false;
        public bool PlayerProMoveEvaluated = true;
        public bool PlayerMoveEvaluated = true;
        public bool OpponentMoveEvaluated = true;
        public bool PlayerMatchedProMove = false;
        public int Dice1;
        public int Dice2;
        public int TotalMovesThisTurn = 0;
        public int MovesAvailable = 0;
        public MoveInfo[] PlayerMovesInfo;
        public MoveInfo[] RecordedMovesInfo;
        public MoveInfo[] TopRankedMovesInfo;

        public PlayablePosition PointFrom = null;
        public PlayablePosition PointTo = null;
        public int PlayerMoveIndex = 0;
        public int CounterMoveIndex = 0;
        public int CountersToMoveIndex = 0;

        public bool IfCounterOnBar = false;
        public bool IfBearingOff = false;
        
        // ANALYSIS
        public bool BoardHasBeenRestored = false;
        public bool ClickedPlayerMoveAnalysis = false;
        public bool ClickedProMoveAnalysis = false;
        public bool ClickedTopRankedMoveAnalysis = false;
        public bool PlayerHasClickedAnalyse = false;
        public bool PlayoutProMoves = false;

        public bool PlayerTurnWasAnalysed = false;
        public bool ProTurnWasAnalysed = false;
        public bool OpponentTurnWasAnalysed = false;
        public bool AITurnWasAnalysed = false;

        public int TotalValidPlayerMovesThisGame = 0;
        public int PlayerMatchedRankThisTurn = 0;
        public int ProMatchedRankThisTurn = 0;
        public int OpponentMatchedRankThisTurn = 0;
        
        public int PlayerTopRankedThisGame = 0;
        public int ProTopRankedThisGame = 0;
        public int OpponentTopRankedThisGame = 0;

        public int PlayerSecondRankedThisGame = 0;
        public int ProSecondRankedThisGame = 0;
        public int OpponentSecondRankedThisGame = 0;

        public int PlayerScoreThisGame = 0;
        public int ProScoreThisGame = 0;
        public int OpponentScoreThisGame = 0;
        
        // BOARD STATE
        public BoardState TurnBeginBoardState = new BoardState();
        public BoardState PlayerBoardState = new BoardState();
        public BoardState ProBoardState = new BoardState();

        // AI DATA
        public bool CaptureAIMoveData = false;
        public bool CaptureAIDoublingData = false;
        public bool AIDataAvailable = false;
        public Move[] AIRankedMoves = null;
        public Move AITopRankedMove = null;
        public Probabilities AIDoublingData = null;

        // FAST FORWARD
        public bool IfFastForwarding = false;
        public int FastForwardTurnIndex = 0;

        // EXIT
        public bool ExitFromStateMachine = false;

        // ------------------------------------------- DATA STRUCTURES ------------------------------------------

        public struct MoveInfo
        {
            public int pointFrom;
            public int pointTo;
            public bool ifBlot;

            public MoveInfo Reset()
            {
                pointFrom = 0;
                pointTo = 0;
                ifBlot = false;

                return this;
            }
        }

        public struct BoardState
        {
            public int PlayerBarCount;
            public int PlayerHomeCount;
            public int OpponentBarCount;
            public int OpponentHomeCount;
            public int[] Points;

            // CONSTRUCTORS
            public void SetState(GameStateContext context, int playerBar, int playerHome,
                                            int opponentBar, int opponentHome, Point[] points)
            {
                Reset();

                PlayerBarCount = playerBar;
                PlayerHomeCount = playerHome;
                OpponentBarCount = opponentBar;
                OpponentHomeCount = opponentHome;

                for (int p = 0; p < 24; p++)
                {
                    Points[p] = points[p].Counters *
                        (context.IfPlayerIsBlack ?
                        (points[p].Colour == Game.PlayerColour.BLACK ? 1 : -1) :
                        (points[p].Colour == Game.PlayerColour.WHITE ? 1 : -1));
                }
            }

            public void SetStateFromContext(GameStateContext context)
            {
                Reset();

                PlayerBarCount = context.BarManager.PlayerBar.Counters;
                OpponentBarCount = context.BarManager.OpponentBar.Counters;
                PlayerHomeCount = context.HomeManager.PlayerHome.Counters;
                OpponentHomeCount = context.HomeManager.OpponentHome.Counters;

                for (int p = 0; p < 24; p++)
                {
                    var point = context.PointsManager.Points[p];

                    Points[p] = point.Counters *
                        (context.IfPlayerIsBlack ?
                        (point.Colour == Game.PlayerColour.BLACK ? 1 : -1) :
                        (point.Colour == Game.PlayerColour.WHITE ? 1 : -1));
                }
            }

            public void SetStateFromState(BoardState state)
            {
                Reset();

                PlayerBarCount = state.PlayerBarCount;
                OpponentBarCount = state.OpponentBarCount;
                PlayerHomeCount = state.PlayerHomeCount;
                OpponentHomeCount = state.OpponentHomeCount;

                for (int p = 0; p < 24; p++)
                {
                    Points[p] = state.Points[p];
                }
            }

            // HELPERS
            public void PopPushCounter(int pointFrom, int pointTo, bool ifBlot, bool ifPlayerTurn = true)
            {
                if (pointFrom == 25)
                {
                    if (ifPlayerTurn) PlayerBarCount -= 1;
                    else OpponentBarCount -= 1;
                }
                else 
                {
                    if (ifPlayerTurn) Points[pointFrom - 1] -= 1;
                    else Points[24 - pointFrom] += 1;
                }

                if (pointTo == 25)
                {
                    if (ifPlayerTurn) PlayerBarCount += 1;
                    else OpponentBarCount += 1;
                }
                else if (pointTo == 0)
                {
                    if (ifPlayerTurn) PlayerHomeCount += 1;
                    else OpponentHomeCount += 1;
                }
                else
                {
                    if (ifPlayerTurn) Points[pointTo - 1] += 1;
                    else Points[24 - pointTo] -= 1;

                    if (ifBlot)
                    {
                        if (ifPlayerTurn)
                        {
                            Points[pointTo - 1] += 1;
                            OpponentBarCount += 1;
                        }
                        else
                        {
                            Points[24 - pointTo] -= 1;
                            PlayerBarCount += 1;
                        }
                    }
                }
            }

            public bool CompareBoardState(BoardState state)
            {
                UnityEngine.Debug.Log($"** EVALUATE PLAYER BOARD STATE **");

                var matched = true;

                if (PlayerBarCount != state.PlayerBarCount) 
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: PLAYER BAR"); }
                if (OpponentBarCount != state.OpponentBarCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: OPPONENT BAR"); }
                if (PlayerHomeCount != state.PlayerHomeCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: PLAYER HOME"); }
                if (OpponentHomeCount != state.OpponentHomeCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: OPPONENT HOME"); }

                for (int p = 0; p < 24; p++)
                {
                    if (Points[p] != state.Points[p])
                    { matched = false; UnityEngine.Debug.Log($"NO MATCH: POINT {(p + 1)} -> {Points[p]} - {state.Points[p]}"); }
                }

                UnityEngine.Debug.Log($"**COMPARE STATES COMPLETE**{(matched ? "MATCHED" : "DIDN'T MATCH")}");
                return matched;
            }

            // RESET
            public void Reset()
            {
                PlayerBarCount = 0;
                PlayerHomeCount = 0;
                OpponentBarCount = 0;
                OpponentHomeCount = 0;
                Points = new int[24];
            }

            public void ResetToNewGame()
            {
                PlayerBarCount = 0;
                PlayerHomeCount = 0;
                OpponentBarCount = 0;
                OpponentHomeCount = 0;
                Points = new int[24];

                // NOTE: FROM POINT 1 - PLAYER IS POSITIVE COUNTER VALUES
                Points[0] = -2;
                Points[5] = 5;
                Points[7] = 3;
                Points[11] = -5;
                Points[12] = 5;
                Points[16] = -3;
                Points[18] = -5;
                Points[23] = 2;
            }
        }
    }
}