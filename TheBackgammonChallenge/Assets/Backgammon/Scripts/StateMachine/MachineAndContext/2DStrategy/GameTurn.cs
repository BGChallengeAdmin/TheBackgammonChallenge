using static Backgammon.GameStateContext2D;

namespace Backgammon
{
    public class GameTurn
    {
        public int GameTurnIndex = 0;
        public bool IsPlayersTurn = true;

        // BOARD STATE
        public BoardState TurnBeginBoardState = new BoardState();
        public BoardState PlayerBoardState = new BoardState();
        public BoardState ProBoardState = new BoardState();
        public BoardState OpponentBoardState = new BoardState();

        // MOVES
        public int Dice1;
        public int Dice2;

        // AI DATA
        public Move[] AIRankedMoves = null;
        public Move AITopRankedMove = null;
        public Probabilities AIDoublingData = null;
    }
}