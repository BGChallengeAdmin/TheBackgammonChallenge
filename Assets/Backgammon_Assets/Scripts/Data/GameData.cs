using UnityEngine;

namespace Backgammon_Asset
{
    [CreateAssetMenu(fileName = "game.asset", menuName = "Data/Game")]
    public class GameData : ScriptableObject
    {
        [SerializeField]
        private int player1PointsAtStart = 0;
        [SerializeField]
        private int player2PointsAtStart = 0;
        [SerializeField]
        private Format format = Format.Processed;
        [SerializeField]
        private string[] moves = null;
        [SerializeField]
        private int player1PointsAtEnd = 0;
        [SerializeField]
        private int player2PointsAtEnd = 0;

        public void GameConstructor(int _p1PtsStart, int _p2PtsStart, string[] _moves, int _p1PtsEnd, int _p2PtsEnd)
        {
            Player1PointsAtStart = _p1PtsStart;
            Player2PointsAtStart = _p2PtsStart;
            Moves = _moves;
            Player1PointsAtEnd = _p1PtsEnd;
            Player2PointsAtEnd = _p2PtsEnd;
        }

        public int Player1PointsAtStart
        {
            get { return player1PointsAtStart; }
            private set => player1PointsAtStart = value;
        }

        public int Player2PointsAtStart
        {
            get { return player2PointsAtStart; }
            private set => player2PointsAtStart = value;
        }

        public Format GetFormat
        {
            get { return (format); }
        }

        // JAMES - Renamed for readability
        public int NumberOfMoves
        {
            get { return moves.Length; }
        }

        //JAMES - Return array of moves
        public string[] Moves
        {
            get { return moves; }
            private set => moves = value;
        }

        public int Player1PointsAtEnd
        {
            get { return player1PointsAtEnd; }
            private set => player1PointsAtEnd = value;
        }

        public int Player2PointsAtEnd
        {
            get { return player2PointsAtEnd; }
            private set => player2PointsAtEnd = value;
        }

        public string GetPlayerMove(int indexMove)
        {
            if (format == Format.RawHacked)
            {
                if (indexMove == 0)
                {
                    char[] separators = { ' ' };
                    raw = moves[0].Split(separators);
                    idx = 1;
                }
                string move = string.Empty;
                string data = raw[idx];
                if (data != "Wins")
                {
                    while (!data.Contains(":") && !data.Contains("Wins"))
                    {
                        ++idx;
                        data = raw[idx];
                    }
                }
                if (data != "Wins")
                {
                    move += data;
                    ++idx;
                    data = raw[idx];
                    while (data[data.Length - 1] != ':' && data[data.Length - 1] != ')' && data != "Wins")
                    {
                        if (data.Contains("/"))
                        {
                            move += " " + data;
                        }
                        ++idx;
                        data = raw[idx];
                    }
                    if (data[data.Length - 1] == ')')
                    {
                        ++idx;
                    }
                }
                else // 'Wins'...
                {
                    move += data + " " + raw[idx + 1] + " " + raw[idx + 2];
                }
                return (move);
            }
            else // Format.Processed...
            {
                if (indexMove < 0 && indexMove >= moves.Length)
                    return (null); // invalid
                return (moves[indexMove]);
            }
        }

        public int Winner()
        {
            bool player1Wins = player1PointsAtEnd > Player1PointsAtStart;
            return (player1Wins ? 1 : 2);
        }

        public enum Format
        {
            RawHacked, Processed
        }

        static string[] raw = null;
        static int idx = 0;
    }
}