using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class MatchWinnerIntroUI : MonoBehaviour
    {
        [SerializeField]
        private Text headerText = null;
        [SerializeField]
        private Text textLine1 = null;
        [SerializeField]
        private Text textLine2 = null;

        Backgammon_Asset.MatchData match;
        public GameObject game;
        private Game gameScript;

        private void Start()
        {
            gameScript = game.GetComponent<Game>();
        }

        void OnEnable()
        {
            match = MatchSelectUI.Match;

            if (match != null)
            {
                textLine1.text = "You are playing as " + (match.Winner() == 1? match.Player1 : match.Player2) + " the winner of the match.";
                textLine2.text = "Using their dice rolls aim to match their moves or better them to win the match.";
            }

            ifAccept = false;
            ifBack = false;
        }

        public void OnAccept()
        {
            GameListUI.playingAs =  match.Winner() == 1 ? PlayerId.Player1 : PlayerId.Player2;
            GameListUI._playingAs = match.Winner() == 1 ? Game.PlayingAs.PLAYER_1 : Game.PlayingAs.PLAYER_2;
            GameListUI._playingAs2D = match.Winner() == 1 ? Game2D.PlayingAs.PLAYER_1 : Game2D.PlayingAs.PLAYER_2;
            gameScript.playingAs = GameListUI.playingAs;

            if (MatchSelectUI.Match.Game(GameListUI.IndexGame).NumberOfMoves == 0)
            {
                return; // safeguard - ie. game data not yet setup
            }

            ifAccept = true;
        }

        public void OnBack()
        {
            MatchSelectUI.Match = null;

            ifBack = true;
        }

        public bool ifAccept;
        public bool ifBack;
    }
}
