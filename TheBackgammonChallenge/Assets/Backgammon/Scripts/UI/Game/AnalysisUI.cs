using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
    public class AnalysisUI : MonoBehaviour
    {
        [Header("TRANSFORMS")]
        [SerializeField] Transform _analysisPanelTransform;
        [SerializeField] Transform _leftPointsTransform;

        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _analysisText = null;
        [SerializeField] TMP_Text _playerRankText = null;
        [SerializeField] TMP_Text _proRankText = null;

        [Header("BUTTONS")]
        [SerializeField] Button _playerMoveButton = null;
        [SerializeField] Button _proMoveButton = null;
        [SerializeField] Button _opponentMoveButton = null;
        [SerializeField] Button _topRankedMoveButton = null;
        [SerializeField] Button _continueButton = null;

        [Header("A.I. LAYOUT")]
        [SerializeField] Transform _playerProRankTransform;
        [SerializeField] Transform _playerRankTransform;
        [SerializeField] Transform _aiRankTransform;

        TMP_Text _playerMoveButtonText;
        TMP_Text _proMoveButtonText;
        TMP_Text _opponentMoveButtonText;
        TMP_Text _topRankedMoveButtonText;
        TMP_Text _continueButtonText;

        private void Awake()
        {
            //_analysisPanelTransform.GetComponent<RectTransform>().sizeDelta =
            //    new Vector2((_analysisPanelScreenWidthPercent * Screen.width),
            //                (_analysisPanelScreenHeightPercent * Screen.height));

            _analysisPanelTransform.localPosition = _leftPointsTransform.localPosition;

            _playerMoveButton.onClick.AddListener(() => OnClickPlayerMove());
            _proMoveButton.onClick.AddListener(() => OnClickProMove());
            _opponentMoveButton.onClick.AddListener(() => OnClickOpponentMove());
            _topRankedMoveButton.onClick.AddListener(() => OnClickTopRankedMove());
            _continueButton.onClick.AddListener(() => OnClickedContinue());
        }

        private void OnEnable()
        {
            _clickedPlayerMove = false;
            _clickedProMove = false;
            _clickedOpponentMove = false;
            _clickedTopRankedMove = false;
            _clickedContinue = false;
            _ifClicked = false;

            var match = Game2D.Context.SelectedMatch;
            var player1 = Game2D.Context.IfPlayingAsPlayer1;
            var proPlayer = player1 ? match.Player1Surname : match.Player2Surname;
            var opponent = player1 ? match.Player2Surname : match.Player1Surname;

            _playerMoveButtonText.text = "Show your move";
            if (!Main.Instance.IfPlayerVsAI)
                _proMoveButtonText.text = "Show " + proPlayer + "s move";
            _opponentMoveButtonText.text = "Show " + opponent + "s move";
            _topRankedMoveButtonText.text = "Show Top Ranked move";
            _continueButtonText.text = "Continue";
        }

        private void OnDestroy()
        {
            _playerMoveButton.onClick.RemoveAllListeners();
            _proMoveButton.onClick.RemoveAllListeners();
            _opponentMoveButton.onClick.RemoveAllListeners();
            _topRankedMoveButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
        }

        internal void Init()
        {
            _playerMoveButtonText = _playerMoveButton.GetComponentInChildren<TMP_Text>();
            _proMoveButtonText = _proMoveButton.GetComponentInChildren<TMP_Text>();
            _opponentMoveButtonText = _opponentMoveButton.GetComponentInChildren<TMP_Text>();
            _topRankedMoveButtonText = _topRankedMoveButton.GetComponentInChildren<TMP_Text>();
            _continueButtonText = _continueButton.GetComponentInChildren<TMP_Text>();
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        internal void SetToAILayout(bool active)
        {
            // SET WHICH VALUES SHOWN
            _playerRankTransform.gameObject.SetActive(!active);
            _playerProRankTransform.gameObject.SetActive(!active);
            _aiRankTransform.gameObject.SetActive(active);

            // SET BUTTON TEXT
            var match = Game2D.Context.SelectedMatch;
            var proPlayer = Game2D.Context.IfPlayingAsPlayer1 ? match.Player1Surname : match.Player2Surname;
            _proMoveButtonText.text = active ? ("Your % to win.") : ("Show " + proPlayer + "s move");
        }

        // TEXT
        internal void SetAnalysisText(string text)
        {
            _analysisText.text = text;
            SetPlayerRankText(string.Empty);
            SetProRankText(string.Empty);
        }

        internal void SetPlayerRankText(string rank)
        {
            _playerRankText.text = "Your rank " + rank;
        }

        internal void SetProRankText(string rank)
        {
            _proRankText.text = "Pro rank " + rank;
        }

        string playerAIRankText;
        string playerAIPercentToWin;
        internal void SetAIRankText(string rank, float percentageToWin)
        {
            playerAIRankText = "Your rank " + rank;
            playerAIPercentToWin = "You have " + percentageToWin + "% to win.";

            _aiRankTransform.GetComponent<TMP_Text>().text = playerAIRankText;
            toggle = false;
        }

        // BUTTONS
        private void OnClickPlayerMove()
        {
            _clickedPlayerMove = true;
            _ifClicked = true;
        }

        bool toggle = false;
        private void OnClickProMove()
        {
            if (!Main.Instance.IfPlayerVsAI)
            {
                _clickedProMove = true;
                _ifClicked = true;
            }
            else
            {
                _aiRankTransform.GetComponent<TMP_Text>().text = toggle ? playerAIRankText : playerAIPercentToWin;
                toggle = !toggle;
            }
        }

        private void OnClickOpponentMove()
        {
            _clickedOpponentMove = true;
            _ifClicked = true;
        }

        private void OnClickTopRankedMove()
        {
            _clickedTopRankedMove = true;
            _ifClicked = true;
        }

        private void OnClickedContinue()
        {
            _clickedContinue = true;
            _ifClicked = true;
        }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private float _analysisPanelScreenWidthPercent = .3f;
        private float _analysisPanelScreenHeightPercent = .5f;

        private bool _clickedPlayerMove = false;
        private bool _clickedProMove = false;
        private bool _clickedOpponentMove = false;
        private bool _clickedTopRankedMove = false;
        private bool _clickedContinue = false;
        private bool _ifClicked = false;

        public bool ClickedPlayerMove { get => _clickedPlayerMove; }
        public bool ClickedProMove { get => _clickedProMove; }
        public bool ClickedOpponentMove { get => _clickedOpponentMove; }
        public bool ClickedTopRankedMove { get => _clickedTopRankedMove; }
        public bool ClickedContinue { get => _clickedContinue; }
        public bool IfClicked { get => _ifClicked; }
    }
}