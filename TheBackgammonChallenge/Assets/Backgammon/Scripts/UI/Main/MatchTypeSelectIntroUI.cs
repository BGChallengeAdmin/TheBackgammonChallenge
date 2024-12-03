using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class MatchTypeSelctIntroUI : MonoBehaviour
    {
        // LANGUAGE FIELDS
        [Header("LANGUAGE")]
        [SerializeField] private Text _titleText = null;
        [SerializeField] private Text _descriptionText = null;
        [SerializeField] private Text _challengeButtonText = null;
        [SerializeField] private Text _aiButtonText = null;
        [SerializeField] private Text _settingsText = null;
        [SerializeField] private Text _languageSelectText = null;
        [SerializeField] private Text _backButtonText = null;
        [SerializeField] private Text _exitButtonText = null;
        [SerializeField] private Text _versionNumberText = null;

        [Header("INACTIVE BUTTONS")]
        [SerializeField] private Button _aiButton = null;
        [SerializeField] private Button _backButton = null;

        [Header("UPDATE POPUP")]
        [SerializeField] private GameObject _updatePopupObj = null;
        [SerializeField] private Text _updatePopupText = null;

        [Header("BACKGROUNDS")]
        [SerializeField] Image _defaultBackground1 = null;
        [SerializeField] Image _defaultBackground2 = null;

        [Header("DEBUG")]
        [SerializeField] GameObject _debugToolkitButtonContainer = null;

        private LanguageScriptableObject languageSO = null;

        private void OnEnable()
        {
            IfClicked = false;
            IfPlayPro = false;
            IfPlayAI = false;
            IfSettings = false;
            IfConfigureBoard = false;
            IfChangeLanguage = false;
            IfBack = false;
            IfDebugToolkit = false;
            IfExit = false;

            UpdatedConfirmed = false;

            // DISABLE DEBUG U.I. IF NOT IN USE
            _debugToolkitButtonContainer.SetActive(Main.Instance.IfUsingDebugToolkit);

            // CONFIGURE LANGUAGE BY REGION
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                _titleText.text = languageSO.matchSelectIntroTitle;
                _descriptionText.text = languageSO.matchSelectIntroDescription;
                _challengeButtonText.text = languageSO.hostIntroText2;
                _aiButtonText.text = languageSO.hostIntroText4 + "\n" + "(" + languageSO.ComingSoon + ")";
                _settingsText.text = languageSO.titleMenuSettings;
                _languageSelectText.text = languageSO.titleLanguageSelectTitle;
                _backButtonText.text = languageSO.Back;
                _exitButtonText.text = languageSO.titleMenuExitApp;
            }

            _versionNumberText.text = "version " + Application.version;

            _backButton.interactable = false;
        }

        // APP VERSION UPDATE
        public void CheckNewVersionUpdate(bool enable, string CurrentVersionNumber)
        {
            _updatePopupObj.SetActive(enable);
            _updatePopupText.text = "App updated from " + CurrentVersionNumber + " to " + Application.version.ToString();
        }

        public void OnClickUpdatePopupOk()
        {
            UpdatedConfirmed = true;
            CheckNewVersionUpdate(false, string.Empty);
        }

        // BACKGROUND IMAGE
        internal void EnableDefaultBackground(bool enable)
        {
            _defaultBackground1.enabled = enable;
            _defaultBackground2.enabled = enable;
        }

        // BUTTON INTERACITONS
        public void OnClickedPlayPro()
        {
            IfPlayPro = true;
            IfClicked = true;
        }

        public void OnClickedAI()
        {
            IfPlayAI = true;
            IfClicked = true;
        }

        public void OnClickSettings()
        {
            IfSettings = true;
            IfClicked = true;
        }

        public void OnClickConfigureBoard()
        {
            IfConfigureBoard = true;
            IfClicked = true;
        }

        public void OnClickChangeLanguage()
        {
            IfChangeLanguage = true;
            IfClicked = true;
        }

        public void OnClickedBack()
        {
            IfBack = true;
            IfClicked = true;
        }

        public void OnClickedDebugToolkit()
        {
            IfDebugToolkit = true;
            IfClicked = true;
        }

        public void OnClickedExit()
        {
            IfExit = true;
            IfClicked = true;
        }

        private bool _ifPlayPro = false;
        private bool _ifPlayAI = false;
        private bool _ifConfigureBoard = false;
        private bool _ifSettings = false;
        private bool _ifChangeLanguage = false;
        private bool _ifBack = false;
        private bool _ifDebugToolkit = false;
        private bool _ifExit = false;
        private bool _ifClicked = false;

        public bool UpdatedConfirmed = false;

        public bool IfClicked
        {
            get => _ifClicked;
            private set => _ifClicked = value;
        }

        public bool IfPlayPro
        {
            get => _ifPlayPro;
            private set => _ifPlayPro = value;
        }

        public bool IfPlayAI
        {
            get => _ifPlayAI;
            private set => _ifPlayAI = value;
        }

        public bool IfSettings
        {
            get => _ifSettings;
            private set => _ifSettings = value;
        }

        public bool IfConfigureBoard
        {
            get => _ifConfigureBoard;
            private set => _ifConfigureBoard = value;
        }

        public bool IfChangeLanguage
        {
            get => _ifChangeLanguage;
            private set => _ifChangeLanguage = value;
        }

        public bool IfBack
        {
            get => _ifBack;
            private set => _ifBack = value;
        }

        public bool IfDebugToolkit
        {
            get => _ifDebugToolkit;
            private set => _ifDebugToolkit = value;
        }

        public bool IfExit
        {
            get => _ifExit;
            private set => _ifExit = value;
        }
    }
}