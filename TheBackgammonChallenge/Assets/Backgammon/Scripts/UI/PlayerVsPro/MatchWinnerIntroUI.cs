using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class MatchWinnerIntroUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] private Text headerText = null;
        [SerializeField] private Text textLine1 = null;
        [SerializeField] private Text textLine2 = null;

        [Header("BACKGROUND IMAGE")]
        [SerializeField] private Image _backgroundImage1;
        [SerializeField] private Image _backgroundImage2;

        [Header("CHANGE GAME")]
        [SerializeField] Transform _changeGameOptions;
        [SerializeField] Button _gameDownButton;
        [SerializeField] Button _gameUpButton;
        [SerializeField] TextMeshProUGUI _gameNumberSelectedText;

        Backgammon_Asset.MatchData match;
        int gameNumber = GameListUI.IndexGame;

        protected void OnEnable()
        {
            match = MatchSelectUI.Match;

            if (match != null)
            {
                textLine1.text = "You are playing as " + (match.Winner() == 1? match.Player1 : match.Player2) + " the winner of the match.";
                textLine2.text = "Using their dice rolls aim to match their moves or better them to win the match.";
            }

            ifAccept = false;
            ifBack = false;

            _gameDownButton.onClick.AddListener(() => ChangeGameNumber(false));
            _gameUpButton.onClick.AddListener(() => ChangeGameNumber(true));

            SetGameNumber(GameListUI.IndexGame);
            TestIfPlayerCanChangeGameNumber();
        }

        public void OnAccept()
        {
            GameListUI.playingAs =  match.Winner() == 1 ? PlayerId.Player1 : PlayerId.Player2;
            GameListUI._playingAs = match.Winner() == 1 ? Game.PlayingAs.PLAYER_1 : Game.PlayingAs.PLAYER_2;
            GameListUI._playingAs2D = match.Winner() == 1 ? Game2D.PlayingAs.PLAYER_1 : Game2D.PlayingAs.PLAYER_2;
            GameListUI.IndexGame = gameNumber;

            if (MatchSelectUI.Match.Game(GameListUI.IndexGame).NumberOfMoves == 0)
            {
                return; // safeguard - ie. game data not yet setup
            }

            ifAccept = true;
        }

        private void TestIfPlayerCanChangeGameNumber()
        {
            // WHICH GAME HAS THE PLAYER SEEN UP TO

            _changeGameOptions.gameObject.SetActive(true);
        }

        private void ChangeGameNumber(bool increment)
        {
            gameNumber += (increment ? 1 : -1);

            if (gameNumber < 0) gameNumber = 0;
            else if (gameNumber >= match.GameCount) gameNumber = match.GameCount - 1;
            
            SetGameNumber(gameNumber);
        }

        private void SetGameNumber(int gameNumber)
        {
            _gameNumberSelectedText.text = (gameNumber + 1).ToString();
            GameListUI.IndexGame = gameNumber;
        }

        public void EnableDefaultBackground(bool enable)
        {
            _backgroundImage1.enabled = enable;
            _backgroundImage2.enabled = enable;
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
