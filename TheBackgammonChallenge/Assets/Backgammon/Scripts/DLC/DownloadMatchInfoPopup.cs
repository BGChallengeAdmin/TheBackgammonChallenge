using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class DownloadMatchInfoPopup : MonoBehaviour
    {
        [SerializeField]
        private DownloadMatchInfoContainer[] _matchInfoContainers = null;

        [SerializeField] private Text _infoPopupTitle;
        [SerializeField] private Text _backButtonText;

        private void Awake()
        {
            // DEFAULT SET ALL VALUES TO ""
            foreach(var info in _matchInfoContainers)
            {
                info.SetMatchInfo();
            }
        }

        private void OnEnable()
        {
            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO; 
            
            if (languageSO != null) 
            {
                _infoPopupTitle.text = languageSO.downloadsInfoPopupTitle;
                _backButtonText.text = languageSO.Back;
            }
        }

        public void SetMatchInfo(int matchNo, string matchTitle, string player1, string player2, string games)
        {
            _matchInfoContainers[matchNo].SetMatchInfo(matchTitle, player1, player2, games);
        }
    }
}