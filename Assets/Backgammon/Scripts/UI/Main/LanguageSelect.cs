using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class LanguageSelect : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text confirmButtonText;

        private void OnEnable()
        {
            _languageSelected = false;
        }

        public void OnClickSelectLanguage(int buttonID)
        {
            var shortCode = string.Empty;

            switch (buttonID)
            {
                case 0: shortCode = "en_GB"; break;
                case 1: shortCode = "fr_FR"; break;
                case 2: shortCode = "es_ES"; break;
                case 3: shortCode = "de_DE"; break;
                case 4: shortCode = "ja_JP"; break;
            }

            Main.Instance.WorldRegionObj.TrySetLanguageSOFromShortCode(shortCode);
            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                titleText.text = languageSO.selectLanguageTitle;
                descriptionText.text = languageSO.selectLanguageText;
                confirmButtonText.text = languageSO.Confirm;
            }
        }

        public void OnOKClicked()
        {
            _languageSelected = true;
        }

        public bool LanguageSelected { get => _languageSelected; }

        bool _languageSelected;
    }
}