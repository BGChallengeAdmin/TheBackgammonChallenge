using UnityEngine;

namespace Backgammon_Asset
{
    [CreateAssetMenu(fileName = "match.asset", menuName = "Data/Match")]
    public class MatchData : ScriptableObject
    {
        [SerializeField]
        private string title = null;
        [SerializeField]
        private string id = null;
        [SerializeField]
        private string eventName = null;
        [SerializeField]
        private string round = null;
        [SerializeField]
        private int points = 1;
        [SerializeField]
        private string player1Surname = null;
        [SerializeField]
        private string player1 = null;
        [SerializeField]
        private string player2Surname = null;
        [SerializeField]
        private string player2 = null;
        [SerializeField]
        private GameData[] games = null;
        [SerializeField]
        private string crawford = null;

        public void MatchConstructor(string _title, string _id, string eventName, string _round, int _points, string _p1S, string _p1, string _p2S, string _p2, string _crawford, GameData[] _games)
        {
            Title = _title;
            ID = _id;
            Event = eventName;
            Round = _round;
            Points = _points;
            Player1Surname = _p1S;
            Player1 = _p1;
            Player2Surname = _p2S;
            Player2 = _p2;
            Crawford = _crawford;
            GameDataArray = _games;
        }

        public string Title
        {
            get { return (title); }
            private set => title = value;
        }

        public string ID
        {
            get { return (id); }
            private set => id = value;
        }

        public string Event
        {
            get { return (eventName); }
            private set => eventName = value;
        }

        public string Round
        {
            get { return (round); }
            private set => round = value;
        }

        public int Points
        {
            get { return (points); }
            private set => points = value;
        }

        public string Player1Surname
        {
            get { return (player1Surname); }
            private set => player1Surname = value;
        }

        public string Player1
        {
            get { return (player1); }
            private set => player1 = value;
        }

        public string Player2Surname
        {
            get { return (player2Surname); }
            private set => player2Surname = value;
        }

        public string Player2
        {
            get { return (player2); }
            private set => player2 = value;
        }

        public int GameCount
        {
            get { return (games != null ? games.Length : 0); }
        }

        public GameData[] GameDataArray
        {
            get { return (games); }
            private set => games = value;
        }

        public string Crawford
        {
            get => crawford;
            private set => crawford = value;
        }

        public GameData Game(int index)
        {
            if (index < 0 || index >= games.Length)
                return (null); // invalid
            return (games[index]);
        }

        public int Winner()
        {
            GameData finalGame = games[games.Length - 1];
            bool player1Wins = finalGame.Player1PointsAtEnd > finalGame.Player2PointsAtEnd;
            bool player2Wins = finalGame.Player2PointsAtEnd > finalGame.Player1PointsAtEnd;
            return (player1Wins ? 1 : (player2Wins ? 2 : 0));
        }
    }
}