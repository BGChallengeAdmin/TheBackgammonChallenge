using UnityEngine;

namespace Backgammon_Asset
{
    public class MatchReplayDLC : MonoBehaviour
    {
        [SerializeField]
        private string id = null;
        [SerializeField]
        private MatchData[] match = null;

        public void ConfigureMatchReplayDLC(string _ID, MatchData[] _dataArray)
        {
            ID = "DLC" + _ID;
            MatchData = _dataArray;
        }

        public string ID
        {
            get { return (id); }
            set => id = value;
        }

        public int MatchCount
        {
            get { return (match != null ? match.Length : 0); }
        }

        public MatchData[] MatchData
        {
            get => match;
            private set => match = value;
        }

        public MatchData Match(int index)
        {
            if (index < 0 || index >= match.Length)
                return (null); // invalid
            return (match[index]);
        }
    }
}