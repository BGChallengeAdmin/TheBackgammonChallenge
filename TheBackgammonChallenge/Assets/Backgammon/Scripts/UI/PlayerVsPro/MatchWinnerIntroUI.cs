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
        int gameIndex = GameListUI.IndexGame;
        int maxGameIndex = 1;

        protected void OnEnable()
        {
            match = MatchSelectUI.Match;

            if (match != null)
            {
                textLine1.text = "You are playing as " + (match.Winner() == 1? match.Player1 : match.Player2) + " the winner of the match.";
                textLine2.text = "Using " + (match.Winner() == 1? match.Player1 : match.Player2) + "'s dice rolls, aim to match or better his moves.";
            }

            ifAccept = false;
            ifBack = false;

            _gameDownButton.onClick.AddListener(() => ChangeGameIndex(false));
            _gameUpButton.onClick.AddListener(() => ChangeGameIndex(true));

            maxGameIndex = match.GameCount;

            SetGameIndex(GameListUI.IndexGame);
            TestIfPlayerCanChangeGameIndex();
        }

        protected void OnDisable()
        {
            _gameDownButton.onClick.RemoveAllListeners();
            _gameUpButton.onClick.RemoveAllListeners();
        }

        public void OnAccept()
        {
            GameListUI._playingAs3D = match.Winner() == 1 ? Game.PlayingAs.PLAYER_1 : Game.PlayingAs.PLAYER_2;
            GameListUI._playingAs2D = match.Winner() == 1 ? Game2D.PlayingAs.PLAYER_1 : Game2D.PlayingAs.PLAYER_2;
            GameListUI.IndexGame = gameIndex;

            if (MatchSelectUI.Match.Game(GameListUI.IndexGame).NumberOfMoves == 0)
            {
                return; // safeguard - ie. game data not yet setup
            }

            ifAccept = true;
        }

        private void TestIfPlayerCanChangeGameIndex()
        {
            _changeGameOptions.gameObject.SetActive(false);
            return;

            // WHICH GAME HAS THE PLAYER SEEN UP TO
            // NOTE: matchReference = matchScores.name + " " + matchScores.ID
            var matchKey = match.name + " " + match.ID;
            var playerMatchScore = Main.Instance.PlayerScoresObj.GetPlayerMatchScore(matchKey);

            if (playerMatchScore is null)
            {
                maxGameIndex = gameIndex;
            }
            else
            {
                foreach (var game in playerMatchScore.gameScoresDict.Values)
                {
                    if (game.indexTurnPlayed < game.numberOfTurns && game.number > 1)
                    {
                        _changeGameOptions.gameObject.SetActive(true);
                        maxGameIndex = game.number;
                        break;
                    }
                }
            }
        }

        private void ChangeGameIndex(bool increment)
        {
            gameIndex += (increment ? 1 : -1);

            if (gameIndex < 0) gameIndex = 0;
            else if (gameIndex >= maxGameIndex) gameIndex = maxGameIndex - 1;
            
            SetGameIndex(gameIndex);
        }

        private void SetGameIndex(int gameIndex)
        {
            _gameNumberSelectedText.text = (gameIndex + 1).ToString();
            GameListUI.IndexGame = gameIndex;
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
