using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
    public class GameWonUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _gameWonText;
        [SerializeField] TMP_Text _gameWonLeftInfoText;
        [SerializeField] TMP_Text _gameWonRightInfoText;
        [SerializeField] TMP_Text _gameWonButtonText;
        [SerializeField] TMP_Text _cancelButtonText;

        [Header("BUTTONS")]
        [SerializeField] Button _gameWonButton;
        [SerializeField] Button _cancelButton;

        protected void Awake()
        {
            _gameWonButton.onClick.AddListener(() => OnClickGameWon());
            _cancelButton.onClick.AddListener(() => OnClickCancel());

            SetGameWonButtonText(true);
            _cancelButtonText.text = "QUIT TO MENU";
        }

        protected void OnEnable()
        {
            _ifClicked = false;
            _clickedConfirm = false;
            _clickedCancel = false;
        }

        protected void OnDestroy()
        {
            _gameWonButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
        }

        // --------------------------------------- HELPER METHODS --------------------------------------

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        internal void SetGameWonText(string text)
        {
            _gameWonText.text = text;
        }

        internal void SetGameWonLeftInfoText(string text)
        {
            _gameWonLeftInfoText.text = text;
        }

        internal void SetGameWonRightInfoText(string text)
        {
            _gameWonRightInfoText.text = text;
        }

        internal void SetGameWonButtonText(bool gameOrMatch)
        {
            _gameWonButtonText.text = gameOrMatch ? "PLAY NEXT GAME" : "PLAY ANOTHER MATCH";
        }

        private void OnClickGameWon()
        {
            _ifClicked = true;
            _clickedConfirm = true;
        }

        private void OnClickCancel()
        {
            _ifClicked = true;
            _clickedCancel = true;
        }

        // -------------------------------------- GETTERS && SETTERS --------------------------------------

        private bool _ifClicked;
        private bool _clickedConfirm;
        private bool _clickedCancel;

        internal bool IfClicked { get => _ifClicked; }
        internal bool ClickedConfirm { get => _clickedConfirm; }
        internal bool ClickedCancel { get => _clickedCancel; }
    }
}