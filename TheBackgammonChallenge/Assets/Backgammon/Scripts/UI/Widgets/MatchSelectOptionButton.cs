using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class MatchSelectOptionButton : MonoBehaviour
    {
        [SerializeField]
        private Text _matchTitle = null;
        [SerializeField]
        private GameObject _vsText = null;
        [SerializeField]
        private Text _player1Name = null;
        [SerializeField]
        private Text _player2Name = null;
        [SerializeField]
        private Image _player1Flag = null;
        [SerializeField]
        private Image _player2Flag = null;
        [SerializeField]
        private GameObject _matchStatsButtonContainer = null;
        [SerializeField]
        private Text _matchStatsButtonText = null;

        private Color _defaultColour = new Color(0.7490196f, 0.7490196f, 0.7490196f, 1.0f);
        private Color _whiteBackground = Color.white;

        public string GetMatchTitle()
        {
            return _matchTitle.text;
        }

        public void SetMatchTitle(string matchTitle)
        {
            _matchTitle.text = matchTitle;

            if (matchTitle == "") _vsText.gameObject.SetActive(false);
            else _vsText.gameObject.SetActive(true);
        }

        public void SetPlayer1NameAndRanking(string playerName, bool winner = false)
        {
            var ranking = Main.Instance.WorldInfoObj.GetPlayerInfoByName(playerName).worldRanking;
            _player1Name.text = playerName + (((ranking != string.Empty) && ranking != null) ? (" [" + ranking + "]") : ranking);

            _player1Name.fontStyle = winner ? FontStyle.Bold : FontStyle.Normal;
            _player1Name.fontSize = winner ? 26 : 22;
        }

        public void SetPlayer2NameAndRanking(string playerName, bool winner = false)
        {
            var ranking = Main.Instance.WorldInfoObj.GetPlayerInfoByName(playerName).worldRanking;
            _player2Name.text = playerName + (((ranking != string.Empty) && ranking != null) ? (" [" + ranking + "]") : ranking);

            _player2Name.fontStyle = winner ? FontStyle.Bold : FontStyle.Normal;
            _player2Name.fontSize = winner ? 26 : 22;
        }

        public void SetPlayer1Flag(Sprite flag)
        {
            if (flag != null)
            {
                _player1Flag.color = _whiteBackground;
                _player1Flag.sprite = flag;
            }
        }

        public void SetPlayer2Flag(Sprite flag)
        {
            if (flag != null)
            {
                _player2Flag.color = _whiteBackground;
                _player2Flag.sprite = flag;
            }
        }

        public void DefaultFlagToNone()
        {
            _player1Flag.color = _defaultColour;
            _player1Flag.sprite = null;

            _player2Flag.color = _defaultColour;
            _player2Flag.sprite = null;
        }

        public void EnableMatchStatsButton(bool enable)
        {
            _matchStatsButtonContainer.gameObject.SetActive(enable);
        }

        public void SetMatchStatsButtonText(string text)
        {
            _matchStatsButtonText.text = text;
        }
    }
}