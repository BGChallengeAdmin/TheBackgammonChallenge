using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class DownloadMatchInfoContainer : MonoBehaviour
    {
        [SerializeField]
        private Text _matchTitle = null;
        [SerializeField]
        private Text _player1Name = null;
        [SerializeField]
        private Text _player2Name = null;
        [SerializeField]
        private Image _player1Flag = null;
        [SerializeField]
        private Image _player2Flag = null;

        private Color _defaultColour = new Color(0.7490196f, 0.7490196f, 0.7490196f, 1.0f);
        private Color _whiteBackground = Color.white;

        public void SetMatchInfo(string matchTitle = "", string player1 = "", string player2 = "", string games = "")
        {
            var player1Info = Main.Instance.WorldInfoObj.GetPlayerInfoByName(player1);
            var player2Info = Main.Instance.WorldInfoObj.GetPlayerInfoByName(player2);

            var player1Ranking = player1Info.worldRanking;
            var player2Ranking = player2Info.worldRanking;

            _matchTitle.text = matchTitle + "   " + games;
            _player1Name.text = player1 + ((player1Ranking != string.Empty && player1Ranking != null) ? (" [" + player1Ranking + "]") : player1Ranking);
            _player2Name.text = player2 + ((player2Ranking != string.Empty && player2Ranking != null) ? (" [" + player2Ranking + "]") : player2Ranking);

            SetPlayerFlag(true, Main.Instance.WorldRegionObj.GetFlagByCountryName(player1Info.nationality));
            SetPlayerFlag(false, Main.Instance.WorldRegionObj.GetFlagByCountryName(player2Info.nationality));
        }

        private void SetPlayerFlag(bool player1or2, Sprite flag)
        {
            if (flag != null)
            {
                if (player1or2)
                {
                    _player1Flag.color = _whiteBackground;
                    _player1Flag.sprite = flag;
                }
                else
                {
                    _player2Flag.color = _whiteBackground;
                    _player2Flag.sprite = flag;
                }
            }
            else
            {
                if (player1or2)
                    _player1Flag.color = _defaultColour;
                else
                    _player2Flag.color = _defaultColour;
            }
        }
    }
}