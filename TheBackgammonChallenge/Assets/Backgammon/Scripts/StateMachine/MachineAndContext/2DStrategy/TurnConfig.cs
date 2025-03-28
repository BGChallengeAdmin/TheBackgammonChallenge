using System;
using static Backgammon.GameStateContext2D;
using static Backgammon.GeneralInfoState2D;

namespace Backgammon
{
    [Serializable]
    public class TurnConfig
    {
        // TURN CONFIG
        public int IndexTurn = 0;
        public EGeneralInfoState2D GeneralInfoStateSwitch = EGeneralInfoState2D.None;
        public bool IsPlayersTurn = true;
        public bool IfPlayer1Turn = true;
        public bool IfPlayer1GoesFirst = true;

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

        // MOVE STATE
        public bool IsPlayersMakingMoves = true;
        public bool PlayerIsUnableToMove = false;
        public bool IfCounterOnBar = false;
        public bool IfBearingOff = false;

        // MOVE
        public int Dice1;
        public int Dice2;
        public int PreviousDice1 = 0;
        public int PreviousDice2 = 0;
        public int MovesAvailable = 0;
        public int TotalMovesThisTurn = 0;

        public PlayablePosition2D PointFrom = null;
        public PlayablePosition2D PointTo = null;
        public int PlayerMoveIndex = 0;
        public int CounterMoveIndex = 0;
        public int CountersToMoveIndex = 0;

        public MoveInfo[] PlayerMovesInfo = new MoveInfo[4];
        public MoveInfo[] RecordedMovesInfo = new MoveInfo[4];
        public MoveInfo[] OpponentMovesInfo = new MoveInfo[4];
        public MoveInfo[] TopRankedMovesInfo = new MoveInfo[4];

        // DOUBLING
        public bool PlayerIsBlockedFromMovingFromBar = false;
        public bool PlayerOfferedDoubles = false;
        public bool AIOfferedDoubles = false;
        public bool AIPlayerAcceptsDoubles = false;
        public bool DoublingTakesOrDrops = false;

        // ANALYSIS
        public bool AITurnWasAnalysed = false;
        public bool PlayerHasClickedAnalyse = false;

        public bool PlayerTurnWasAnalysed = false;
        public bool ProTurnWasAnalysed = false;
        public bool OpponentTurnWasAnalysed = false;

        public bool ClickedPlayerMoveAnalysis = false;
        public bool ClickedProMoveAnalysis = false;
        public bool ClickedOpponentMoveAnalysis = false;
        public bool ClickedTopRankedMoveAnalysis = false;

        public bool PlayerMatchedProMove = false;
        public bool ShowOpponentRank1Move = false;
        public bool PlayoutProMoves = false;
        public bool BoardHasBeenRestored = false;
        
        public float LostEquity = 0f;

        // RANKING
        public int PlayerMatchedRankThisTurn = 0;
        public int ProMatchedRankThisTurn = 0;
        public int OpponentMatchedRankThisTurn = 0;

        // CONCEDES
        public bool UndoPlayerMove = false;
        public bool ReplayPlayerMove = false;
        public bool ConcedeTheGame = false;
        public bool PlayerConcedes = false;
        public bool GameWon = false;

        // AI DATA
        public int AIDataRequestBailOutCounter = 0;
        public bool CapturedAIDoublingData = false;
        public bool CapturedAIMoveData = false;
        public bool AIDataAvailable = false;
        public Move[] AIRankedMoves = null;
        public Move AITopRankedMove = null;
        public Probabilities AIDoublingData = null;
    }
}