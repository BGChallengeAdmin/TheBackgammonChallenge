using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace Backgammon
{
    public class GameWonUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _gameWonText;
        [SerializeField] TMP_Text _playerGameWonInfoText;
        [SerializeField] TMP_Text _playerProGameWonInfoText;
        [SerializeField] TMP_Text _opponentGameWonInfoText;
        [SerializeField] TMP_Text _gameWonButtonText;
        [SerializeField] TMP_Text _cancelButtonText;

        [Header("BUTTONS")]
        [SerializeField] Button _gameWonButton;
        [SerializeField] Button _cancelButton;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _opponentScorePanelTransform;
        [SerializeField] Transform _playerScorePanelTransform;
        [SerializeField] Transform _playerProScorePanelTransform;
        [SerializeField] Transform _aiOffsetPositionTransform;

        private Vector3 _defaultPlayerScorePos = new Vector3(550, -50, 0);

        private void Awake()
        {
            _gameWonButton.onClick.AddListener(() => OnClickGameWon());
            _cancelButton.onClick.AddListener(() => OnClickCancel());

            SetGameWonButtonText(true);
            _cancelButtonText.text = "QUIT TO MENU";
        }

        private void OnEnable()
        {
            _ifClicked = false;
            _clickedConfirm = false;
            _clickedCancel = false;
        }

        private void OnDestroy()
        {
            _gameWonButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
        }

        // --------------------------------------- HELPER METHODS --------------------------------------

        internal void Init()
        {
            _defaultPlayerScorePos = _playerProScorePanelTransform.localPosition;
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        internal void SetToAILayout(bool active)
        {
            // OPPONENT PANEL
            _opponentScorePanelTransform.localPosition = new Vector3((active ? -1 * _aiOffsetPositionTransform.localPosition.x :
                                                                     -1 * _defaultPlayerScorePos.x), _defaultPlayerScorePos.y, 0);

            // PLAYER - IS NOT USED IN A.I. -> USE PLAYER PRO
            _playerScorePanelTransform.gameObject.SetActive(!active);

            // PLAYER PRO -> SHOWS A.I. PLAYER STATS
            _playerProScorePanelTransform.localPosition = new Vector3((active ? _aiOffsetPositionTransform.localPosition.x :
                                                                      _defaultPlayerScorePos.x), _defaultPlayerScorePos.y, 0);
        }

        internal void SetGameWonText(string text)
        {
            _gameWonText.text = text;
        }

        internal void SetPlayerGameWonInfoText(string text)
        {
            _playerGameWonInfoText.text = text;
        }

        internal void SetPlayerProGameWonInfoText(string text)
        {
            _playerProGameWonInfoText.text = text;
        }

        internal void SetOpponentGameWonInfoText(string text)
        {
            _opponentGameWonInfoText.text = text;
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