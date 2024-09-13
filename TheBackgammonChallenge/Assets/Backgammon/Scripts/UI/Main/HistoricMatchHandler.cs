using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class HistoricMatchHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject historicMatchesFolder;
        [SerializeField]
        private GameObject restorePurchasesButton;

        [SerializeField] private Text _downloadsTitle;
        [SerializeField] private Text _backButtonText;
        [SerializeField] private Text _restoreButtonText;

        private void OnEnable()
        {
            ifBack = false;
            restorePurchases = false;

#if UNITY_IOS || UNITY_STANDALONE_OSX
            restorePurchasesButton.gameObject.SetActive(true);
#endif

            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null ) 
            {
                _downloadsTitle.text = languageSO.downloadsTitle;
                _backButtonText.text = languageSO.Back;
                _restoreButtonText.text = languageSO.downloadsRestorePurchases;
            }
        }

        public void OnClickedBack()
        {
            ifBack = true;
        }

        public void OnClickedRestorePurchases()
        {
            restorePurchases = true;
        }

        public GameObject HistoricMatchesFolder
        {
            get => historicMatchesFolder;
        }

        public bool ifBack = false;
        public bool restorePurchases = false;
    }
}
