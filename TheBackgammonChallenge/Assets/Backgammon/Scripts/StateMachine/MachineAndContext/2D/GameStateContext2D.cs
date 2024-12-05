using System;
using UnityEngine;
using static Backgammon.GeneralInfoState2D;

namespace Backgammon
{
    public class GameStateContext2D
    {
        public GameStateContext2D(Game2D game, BoardMaterialsManager2D boardMaterialsManager, BarManager2D barManager,
                                HomeManager2D homeManager, DoublingManager2D doublingManager, PointsManager2D pointsManager,
                                CountersManager2D countersManager, DiceManager2D diceManager,
                                ObserveAnalysisManager2D observeAnalysisManager)
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
            OpponentMovesInfo = new MoveInfo[4];
            TopRankedMovesInfo = new MoveInfo[4];
        }

        internal void SetDataHandlers(AIDataHandler aiDataHandler)
        {
            _aiDataHAndler = aiDataHandler;
        }

        internal void SetUI(Transform defaultBackground, GameScreenUI gameScreenUI, RollForGoesFirstUI rollForGoesFirstUI,
                            BeforeCommenceUI beforeCommenceUI, DoublingUI doublingUI, DiceRollsUI diceRollsUI,
                            AnalysisOrContinueUI analysisOrContinueUI, AnalysisUI analysisUI, ObserveBoardUI observeBoardUI,
                            TurnEndsUI turnEndsUI, GameWonUI gameWonUI, GeneralInfoUI generalInfoUI,
                            ChallengeProgressionUI challengeProgressionUI, ConfigureBoardManualUI configureBoardManual)
        {
            _defaultBackground = defaultBackground;
            _gameScreenUI = gameScreenUI;
            _rollForGoesFirstUI = rollForGoesFirstUI;
            _beforeCommenceUI = beforeCommenceUI;
            _doublingUI = doublingUI;
            _diceRollsUI = diceRollsUI;
            _analysisOrContinueUI = analysisOrContinueUI;
            _analysisUI = analysisUI;
            _observeBoardUI = observeBoardUI;
            _turnEndsUI = turnEndsUI;
            _gameWonUI = gameWonUI;
            _generalInfoUI = generalInfoUI;
            _challengeProgressionUI = challengeProgressionUI;
            _configureBoardManualUI = configureBoardManual;
        }

        // GAME
        private Game2D _game;
        public Game2D Game { get => _game; }

        public bool ExitFromStateMachine = false;

        // MANAGERS
        private BoardMaterialsManager2D _boardMaterialsManager;
        private BarManager2D _barManager;
        private HomeManager2D _homeManager;
        private DoublingManager2D _doublingManager;
        private PointsManager2D _pointsManager;
        private CountersManager2D _countersManager;
        private DiceManager2D _diceManager;
        private ObserveAnalysisManager2D _observeAnalysisManager;

        internal BoardMaterialsManager2D BoardMaterialsManager { get => _boardMaterialsManager; }
        internal BarManager2D BarManager { get => _barManager; }
        internal HomeManager2D HomeManager { get => _homeManager; }
        internal DoublingManager2D DoublingManager { get => _doublingManager; }
        internal PointsManager2D PointsManager { get => _pointsManager; }
        internal CountersManager2D CountersManager { get => _countersManager; }
        internal DiceManager2D DiceManager { get => _diceManager; }
        internal ObserveAnalysisManager2D ObserveAnalysisManager { get => _observeAnalysisManager; }

        // DATA HANDLERS
        private AIDataHandler _aiDataHAndler;

        public AIDataHandler AIDataHandler { get => _aiDataHAndler; }

        // UI
        private Transform _defaultBackground;
        private GameScreenUI _gameScreenUI;
        private RollForGoesFirstUI _rollForGoesFirstUI;
        private BeforeCommenceUI _beforeCommenceUI;
        private DoublingUI _doublingUI;
        private DiceRollsUI _diceRollsUI;
        private AnalysisOrContinueUI _analysisOrContinueUI;
        private AnalysisUI _analysisUI;
        private ObserveBoardUI _observeBoardUI;
        private TurnEndsUI _turnEndsUI;
        private GameWonUI _gameWonUI;
        private GeneralInfoUI _generalInfoUI;
        private ChallengeProgressionUI _challengeProgressionUI;
        private ConfigureBoardManualUI _configureBoardManualUI;

        internal Transform DefaultBackground { get => _defaultBackground; }
        internal GameScreenUI GameScreenUI { get => _gameScreenUI; }
        internal RollForGoesFirstUI RollForGoesFirstUI { get => _rollForGoesFirstUI; }
        internal BeforeCommenceUI BeforeCommenceUI { get => _beforeCommenceUI; }
        internal DoublingUI DoublingUI { get => _doublingUI; }
        internal DiceRollsUI DiceRollsUI { get => _diceRollsUI; }
        internal AnalysisOrContinueUI AnalysisOrContinueUI { get => _analysisOrContinueUI; }
        internal AnalysisUI AnalysisUI { get => _analysisUI; }
        internal ObserveBoardUI ObserveBoardUI { get => _observeBoardUI; }
        internal TurnEndsUI TurnEndsUI { get => _turnEndsUI; }
        internal GameWonUI GameWonUI { get => _gameWonUI; }
        internal GeneralInfoUI GeneralInfoUI { get => _generalInfoUI; }
        internal ChallengeProgressionUI ChallengeProgressionUI { get => _challengeProgressionUI; }
        internal ConfigureBoardManualUI ConfigureBoardManualUI { get => _configureBoardManualUI; }
        internal FadeInFadeOutImage FadeInFadeOutBlack { get => _defaultBackground.GetComponentInChildren<FadeInFadeOutImage>(); }

        // MATCH CONFIG
        internal Backgammon_Asset.MatchData SelectedMatch = MatchSelectUI.Match;
        public int IndexGame = GameListUI.IndexGame;
        public Backgammon_Asset.GameData SelectedGame = GameListUI._game;

        // BOARD CONFIG
        public Game2D.PlayingAs PlayingAs = Game2D.PlayingAs.PLAYER_1;
        public Game2D.PlayingAs OpponentAs = Game2D.PlayingAs.PLAYER_2;
        public bool IfPlayingAsPlayer1 = true;
        public bool IfPlayFromLhs = true;
        public bool IfPlayerIsBlack = true;

        // GAME CONFIG
        public int IndexTurn = 0;
        public int AIIndexTurn = 0;
        public bool AIDiceHaveBeenRolled = false;
        public bool IsPlayersTurn = true;
        public bool IsPlayersMakingMoves = true;
        public bool IfPlayer1Turn = true;
        public bool IfPlayer1GoesFirst = true;

        // TURN CONFIG
        public EGeneralInfoState2D GeneralInfoStateSwitch = EGeneralInfoState2D.None;
        public bool PlayerIsUnableToMove = false;
        public bool PlayerIsBlockedFromMovingFromBar = false;
        public bool PlayerOfferedDoubles = false;
        public bool AIOfferedDoubles = false;
        public bool AIPlayerAcceptsDoubles = false;
        public bool DoublingTakesOrDrops = false;

        public bool PlayerMatchedProMove = false;
        public bool ShowOpponentRank1Move = false;

        public bool UndoPlayerMove = false;
        public bool ReplayPlayerMove = false;
        public bool ConcedeTheGame = false;
        public bool PlayerConcedes = false;
        public bool GameWon = false;

        public int Dice1;
        public int Dice2;
        public int PreviousDice1 = 0;
        public int PreviousDice2 = 0;
        public int MovesAvailable = 0;
        public int TotalMovesThisTurn = 0;
        public MoveInfo[] PlayerMovesInfo;
        public MoveInfo[] RecordedMovesInfo;
        public MoveInfo[] OpponentMovesInfo;
        public MoveInfo[] TopRankedMovesInfo;

        public PlayablePosition2D PointFrom = null;
        public PlayablePosition2D PointTo = null;
        public int PlayerMoveIndex = 0;
        public int CounterMoveIndex = 0;
        public int CountersToMoveIndex = 0;

        public bool IfCounterOnBar = false;
        public bool IfBearingOff = false;

        // ANALYSIS
        public bool BoardHasBeenRestored = false;
        public bool ClickedPlayerMoveAnalysis = false;
        public bool ClickedProMoveAnalysis = false;
        public bool ClickedOpponentMoveAnalysis = false;
        public bool ClickedTopRankedMoveAnalysis = false;
        public bool PlayerHasClickedAnalyse = false;
        public bool PlayoutProMoves = false;

        public bool PlayerTurnWasAnalysed = false;
        public bool ProTurnWasAnalysed = false;
        public bool OpponentTurnWasAnalysed = false;
        public bool AITurnWasAnalysed = false;
        public float LostEquity = 0f;

        // STATS
        public int TotalValidPlayerMovesThisGame = 0;
        public int TotalValidPlayerMatchesThisGame = 0;
        public int TotalValidOpponentMovesThisGame = 0;

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
        public BoardState OpponentBoardState = new BoardState();

        public BoardState Rank1BoardState = new BoardState();
        public BoardState Rank2BoardState = new BoardState();
        public BoardState Rank3BoardState = new BoardState();
        public BoardState Rank4BoardState = new BoardState();
        public BoardState Rank5BoardState = new BoardState();

        // AI GAME CONFIG
        public Backgammon_Asset.MatchData AIMatchData = null;
        public Backgammon_Asset.GameData AIGameData = null;
        public AISettingsScriptableObject AISettings = null;

        // AI DATA
        public int AIDataRequestBailOutCounter = 0;
        public bool CapturedAIDoublingData = false;
        public bool CapturedAIMoveData = false;
        public bool AIDataAvailable = false;
        public Move[] AIRankedMoves = null;
        public Move AITopRankedMove = null;
        public Probabilities AIDoublingData = null;

        // FAST FORWARD / CONTINUE
        public bool IfGameToContinue = false;
        public bool IfAIGameToContinue = false;
        public bool IfFastForwarding = false;
        public int FastForwardTurnIndex = 0;

        // END GAME
        public bool GameWonByGammon = false;
        public bool GameWonByBackGammon = false;
        public bool AIGameWasWon = false;
        public bool IfPlayNextGame = false;
        public bool IfPlayAnotherMatch = false;

        //DEBUG
        public DebugPrefab Debug_reportState;
        public DebugPrefab Debug_debugObject;

        // ------------------------------------------- DATA STRUCTURES ------------------------------------------

        public struct GameTurn
        {
            public bool IfPlayer1Turn;
            public bool IsPlayersTurn;

            public bool PlayerIsUnableToMove;
            public bool PlayerOfferedDoubles;
            public bool PlayerAcceptsDoubles;

            public int Dice1;
            public int Dice2;
            public int MovesAvailable;
            public MoveInfo[] PlayerMovesInfo;
            public MoveInfo[] TopRankedMovesInfo;
            public BoardState TurnBeginBoardState;
            public BoardState PlayerBoardState;

            public bool CapturedAIDoublingData;
            public bool CapturedAIMoveData;
            public bool AIDataAvailable;
            public Move AITopRankedMove;
            public Move[] AIRankedMoves;
            public Probabilities AIDoublingData;

            public bool PlayerMoveEvaluated;
            public int TotalValidPlayerMovesThisGame;
            public int PlayerMatchedRankThisTurn;
            public int PlayerTopRankedThisGame;
            public int PlayerSecondRankedThisGame;
            public int PlayerScoreThisGame;

            public bool UndoPlayerMove;
            public bool ReplayPlayerMove;
            public bool PlayerConcedes;
            public bool GameWon;
        }

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
            public bool HasBeenSet;

            static char[] _conversionValues = new char[31] {'0',
                                                            'A', 'B', 'C', 'D', 'E',
                                                            'F', 'G', 'H', 'I', 'J',
                                                            'K', 'L', 'M', 'N', 'O',
                                                            'a', 'b', 'c', 'd', 'e',
                                                            'f', 'g', 'h', 'i', 'j',
                                                            'k', 'l', 'm', 'n', 'o'};

            // CONSTRUCTORS
            public void SetState(GameStateContext2D context, int playerBar, int playerHome,
                                            int opponentBar, int opponentHome, Point2DPrefab[] points)
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
                        (points[p].Colour == Game2D.PlayerColour.BLACK ? 1 : -1) :
                        (points[p].Colour == Game2D.PlayerColour.WHITE ? 1 : -1));
                }
            }

            public void SetStateFromContext(GameStateContext2D context)
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
                        (point.Colour == Game2D.PlayerColour.BLACK ? 1 : -1) :
                        (point.Colour == Game2D.PlayerColour.WHITE ? 1 : -1));
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

            public int GetPointCounterValue(int pointID)
            {
                return Points[pointID - 1];
            }

            public bool CompareBoardState(BoardState state, bool showDebug = false)
            {
                if (!state.HasBeenSet) return false;

                var matched = true;

                if (PlayerBarCount != state.PlayerBarCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: PLAYER BAR {PlayerBarCount} -> {state.PlayerBarCount}"); }
                if (OpponentBarCount != state.OpponentBarCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: OPPONENT BAR {OpponentBarCount} -> {state.OpponentBarCount}"); }
                if (PlayerHomeCount != state.PlayerHomeCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: PLAYER HOME {PlayerHomeCount} -> {state.PlayerHomeCount}"); }
                if (OpponentHomeCount != state.OpponentHomeCount)
                { matched = false; UnityEngine.Debug.Log($"NO MATCH: OPPONENT HOME {OpponentHomeCount} -> {state.OpponentHomeCount}"); }

                for (int p = 0; p < 24; p++)
                {
                    if (Points[p] != state.Points[p])
                    {
                        matched = false;

                        if (showDebug) Debug.Log($"POINT {(p + 1)}: {Points[p]} -> {state.Points[p]}");
                    }
                }

                return matched;
            }

            // SERIALIZE
            public string GetBoardStateString()
            {
                char[] boardStateArray = new char[28];

                boardStateArray[0] = ConvertToChar(PlayerBarCount);
                boardStateArray[1] = ConvertToChar(PlayerHomeCount);
                boardStateArray[2] = ConvertToChar(OpponentBarCount);
                boardStateArray[3] = ConvertToChar(OpponentHomeCount);

                for (int p = 0; p < 24; p++)
                {
                    boardStateArray[p + 4] = ConvertToChar(Points[p]);
                }

                return new string(boardStateArray);
            }

            public void SetBoardStateFromString(string boardStateString)
            {
                Reset();

                char[] boardStateArray = new char[28];
                boardStateArray = boardStateString.ToCharArray();

                PlayerBarCount = ConvertFromChar(boardStateArray[0]);
                PlayerHomeCount = ConvertFromChar(boardStateArray[1]);
                OpponentBarCount = ConvertFromChar(boardStateArray[2]);
                OpponentHomeCount = ConvertFromChar(boardStateArray[3]);

                for (int p = 0; p < 24; p++)
                {
                    Points[p] = ConvertFromChar(boardStateArray[p + 4]);
                }
            }

            private char ConvertToChar(int value)
            {
                return _conversionValues[(Math.Abs(value) + (value < 0 ? 15 : 0))];
            }

            private int ConvertFromChar(char character)
            {
                var index = Array.IndexOf(_conversionValues, character);
                return (index >= 15 ? -1 * (index - 15) : index);
            }

            // RESET
            public void Reset()
            {
                PlayerBarCount = 0;
                PlayerHomeCount = 0;
                OpponentBarCount = 0;
                OpponentHomeCount = 0;
                Points = new int[24];
                HasBeenSet = true;
            }

            public void ResetToNewGame()
            {
                Reset();

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