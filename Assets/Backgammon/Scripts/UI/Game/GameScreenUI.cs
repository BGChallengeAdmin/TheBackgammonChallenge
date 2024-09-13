using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace Backgammon
{
    public class GameScreenUI : MonoBehaviour
    {
        [Header("PREFABS")]
        [SerializeField] AnalysisAnimationPrefab _analysisAnimationPrefab;
        [SerializeField] GameWonAnimationPrefab _gameWonAnimationPrefab;

        [Header("PLAYER INFO")]
        [Header("PLAYER")]
        [SerializeField] TMP_Text _playerNameText;
        [SerializeField] TMP_Text _playerTopMatchedText;
        [SerializeField] TMP_Text _playerMatchPointsText;
        [SerializeField] Transform _playerTopMatchedHighlight;
        [SerializeField] Image _playerCounterImage;

        [Header("PRO PLAYER")]
        [SerializeField] TMP_Text _proPlayerNameText;
        [SerializeField] TMP_Text _proPlayerTopMatchedText;
        [SerializeField] TMP_Text _proPlayerMatchPointsText;
        [SerializeField] Transform _proPlayerTopMatchedHighlight;
        [SerializeField] Image _proPlayerCounterImage;

        [Header("OPPONENT")]
        [SerializeField] TMP_Text _opponentNameText;
        [SerializeField] TMP_Text _opponentTopMatchedText;
        [SerializeField] TMP_Text _opponentMatchPointsText;
        [SerializeField] Transform _opponentTopMatchedHighlight;
        [SerializeField] Image _opponentCounterImage;

        [Header("ANIMATION DEFAULT TRANSFORMS")]
        [SerializeField] Transform _playerAnimationDefaultTransform;
        [SerializeField] Transform _proPlayerAnimationDefaultTransform;
        [SerializeField] Transform _opponentAnimationDefaultTransform;

        [Header("GAME TURN")]
        [SerializeField] TMP_Text _gameTurnText;
        [SerializeField] TMP_Text _matchGameText;

        [Header("MENU")]
        [SerializeField] Button _menuButton;
        [SerializeField] Button _menuCloseButton;
        [SerializeField] Button _changeGameButton;
        [SerializeField] Button _undoMoveButton;
        [SerializeField] Button _concedeGameButton;
        [SerializeField] Button _concedeConfirmButton;
        [SerializeField] Button _concedeCancelButton;
        [SerializeField] Button _exitGameButton;
        [SerializeField] Button _exitConfirmButton;
        [SerializeField] Button _exitCancelButton;
        [SerializeField] Transform _menuOptionsContainer;
        [SerializeField] Transform _concedeOptionsContainer;
        [SerializeField] Transform _exitOptionsContainer;

        [Header("IN GAME STATS")]
        [SerializeField] Button _statisticsButton;
        [SerializeField] Button _statisticsCloseButton;
        [SerializeField] TMP_Text _ranksDescriptionText;
        [SerializeField] TMP_Text _opponentStatsText;
        [SerializeField] TMP_Text _proPlayerStatsText;
        [SerializeField] TMP_Text _playerStatsText;
        [SerializeField] Transform _inGameStatsContainer;
        [SerializeField] Transform _screenCentreTransform;

        [Header("AI LAYOUT TRANSFORMS")]
        [SerializeField] Transform _playerPanel;
        [SerializeField] Transform _playerStatsPanel;
        [SerializeField] Transform _proPlayerPanel;
        [SerializeField] Transform _proPlayerStatsPanel;
        [SerializeField] Transform _aiDataDelayPanel;

        [Header("SHOW RANK 1 BUTTON")]
        [SerializeField] Button _showOpponentRank1Button;
        [SerializeField] Transform _showOpponentRank1ButtonContainer;
        [SerializeField] Transform _opponentDiceContainer;

        private void Awake()
        {
            _inGameStatsContainer.transform.position = _screenCentreTransform.position;

            // MENU BUTTONS
            _menuButton.onClick.AddListener(() => OnClickMenu());
            _menuCloseButton.onClick.AddListener(() => OnClickMenu());
            // STATS BUTTONS
            _statisticsButton.onClick.AddListener(() => OnClickStats());
            _statisticsCloseButton.onClick.AddListener(() => OnClickStats());
            // GAME BUTTONS
            _changeGameButton.interactable = false;
            _undoMoveButton.onClick.AddListener(() => OnClickUndoMove());
            _undoMoveButton.interactable = false;
            // CONCEDES BUTTONS
            _concedeGameButton.onClick.AddListener(() => OnClickConcedeGame());
            _concedeConfirmButton.onClick.AddListener(() => OnClickConfirmConcede());
            _concedeCancelButton.onClick.AddListener(() => OnClickCancelConcede());
            // EXIT BUTTON
            _exitGameButton.onClick.AddListener(() => OnClickExit());
            _exitConfirmButton.onClick.AddListener(() => OnClickConfirmExit());
            _exitCancelButton.onClick.AddListener(() => OnClickCancelExit());

            // PLAYER
            _startPosition = _playerAnimationDefaultTransform.position + _offset;
            _endPosition = _playerTopMatchedText.gameObject.transform.position + _offset;

            _playerAnimation = Instantiate(_analysisAnimationPrefab, this.transform);
            _playerAnimation.Init(_startPosition, _endPosition, .75f, 1f);
            _playerAnimation.OnAnimationCompleteAction += PlayerScoreIncrementAction;

            // PRO
            _startPosition = _proPlayerAnimationDefaultTransform.position + _offset;
            _endPosition = _proPlayerTopMatchedText.gameObject.transform.position + _offset;

            _proPlayerAnimation = Instantiate(_analysisAnimationPrefab, this.transform);
            _proPlayerAnimation.Init(_startPosition, _endPosition, .75f, 1f);
            _proPlayerAnimation.OnAnimationCompleteAction += ProPlayerScoreIncrementAction;

            // OPPONENT
            _startPosition = _opponentAnimationDefaultTransform.position - _offset;
            _endPosition = _opponentTopMatchedText.gameObject.transform.position - _offset;

            _opponentAnimation = Instantiate(_analysisAnimationPrefab, this.transform);
            _opponentAnimation.Init(_startPosition, _endPosition, .75f, 1f);
            _opponentAnimation.OnAnimationCompleteAction += OpponentScoreIncrementAction;

            // SHOW PRO RANK 1 MOVE
            _showOpponentRank1ButtonContainer.gameObject.transform.localPosition = _opponentDiceContainer.localPosition;
            _showOpponentRank1Button.onClick.AddListener(() => ShowOpponentRank1Move());

            // GAME WON ANIMATIONS
            if (_gameWonAnimation1 is null) _gameWonAnimation1 = Instantiate(_gameWonAnimationPrefab, this.transform);
            if (_gameWonAnimation2 is null) _gameWonAnimation2 = Instantiate(_gameWonAnimationPrefab, this.transform);

            Reset();
        }

        private void OnEnable()
        {
            if (Main.Instance.IfPlayerVsAI) _concedeGameButton.interactable = true;
            else _concedeGameButton.interactable = false;

            SetShowOpponentRank1Active(false);
        }

        private void OnDestroy()
        {
            _playerAnimation.OnAnimationCompleteAction -= PlayerScoreIncrementAction;
            _proPlayerAnimation.OnAnimationCompleteAction -= ProPlayerScoreIncrementAction;
            _opponentAnimation.OnAnimationCompleteAction -= OpponentScoreIncrementAction;

            // MENU BUTTONS
            _menuButton.onClick.RemoveAllListeners();
            _menuCloseButton.onClick.RemoveAllListeners();
            // STATS BUTTONS
            _statisticsButton.onClick.RemoveAllListeners();
            _statisticsCloseButton.onClick.RemoveAllListeners();
            // GAME BUTTONS
            _undoMoveButton.onClick.RemoveAllListeners();
            // CONCEDE BUTTONS
            _concedeGameButton.onClick.RemoveAllListeners();
            _concedeConfirmButton.onClick.RemoveAllListeners();
            _concedeCancelButton.onClick.RemoveAllListeners();
            // EXIT BUTTONS
            _exitGameButton.onClick.RemoveAllListeners();
            _exitConfirmButton.onClick.RemoveAllListeners();
            _exitCancelButton.onClick.RemoveAllListeners();
            // SHOW OPPONENT RANK 1 MOVE
            _showOpponentRank1Button.onClick.RemoveAllListeners();
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);

            _displayMenu = false;
            _displayStats = false;

            _menuOptionsContainer.gameObject.SetActive(_displayMenu);
            _concedeOptionsContainer.gameObject.SetActive(_displayStats);
            _exitOptionsContainer.gameObject.SetActive(_displayStats);
            _inGameStatsContainer.gameObject.SetActive(_displayMenu);
        }

        internal void SetToAILayout(bool active)
        {
            // PLAYER SCORE PANEL
            var playerProPos = _proPlayerPanel.transform.localPosition;
            _playerPanel.transform.localPosition = new Vector3((active ? 0 : -1 * playerProPos.x), playerProPos.y, 0);
            _proPlayerPanel.gameObject.SetActive(!active);

            // PLAYER SCORE ANIMATION
            _startPosition = _playerAnimationDefaultTransform.position + _offset;
            _endPosition = _playerTopMatchedText.gameObject.transform.position + _offset;
            _playerAnimation.Init(_startPosition, _endPosition, .75f, 1f);

            // PLAYER STATS PANEL
            var proPlayerStatPos = _proPlayerStatsPanel.transform.localPosition;
            _playerStatsPanel.transform.localPosition = new Vector3((active ? 0 : -1 * proPlayerStatPos.x), proPlayerStatPos.y, 0);
            _proPlayerStatsPanel.gameObject.SetActive(!active);
        }

        internal void SetAIDataDelayActive(bool active)
        {
            _aiDataDelayPanel.gameObject.SetActive(active);
        }

        // SET PLAYER VALUES
        internal void SetPlayerName(string name)
        {
            _playerNameText.text = name;
        }

        internal void SetPlayerCounterIcon(Sprite counterSprite)
        {
            _playerCounterImage.sprite = counterSprite;
        }

        internal void SetPlayerScore(int score)
        {
            _playerScore = score;
        }

        internal void SetPlayerTopMatches(int matches)
        {
            _playerMatches = matches;
        }

        internal void SetPlayerTopMatched(int matched, bool highlight = false)
        {
            _playerTopMatchedText.text = matched.ToString();
            _playerTopMatchedHighlight.gameObject.SetActive(highlight);
        }

        internal void SetAndAnimatePlayerRank(string rank)
        {
            _playerAnimation.SetAndAnimateRankText(rank);
        }

        private void PlayerScoreIncrementAction(AnalysisAnimationPrefab animation)
        {
            var currentMatches = int.Parse(_playerTopMatchedText.text);
            SetPlayerTopMatched(_playerMatches, (_playerMatches > currentMatches));
        }

        // SET PRO PLAYER VALUES
        internal void SetProPlayerName(string name)
        {
            _proPlayerNameText.text = name;
        }

        internal void SetProPlayerCounterIcon(Sprite counterSprite)
        {
            _proPlayerCounterImage.sprite = counterSprite;
        }

        internal void SetProPlayerScore(int score)
        {
            _playerScore = score;
        }

        internal void SetProPlayerTopMatches(int matches)
        {
            _proPlayerMatches = matches;
        }

        internal void SetProPlayerTopMatched(int matched, bool highlight = false)
        {
            _proPlayerTopMatchedText.text = matched.ToString();
            _proPlayerTopMatchedHighlight.gameObject.SetActive(highlight);
        }

        internal void SetAndAnimateProPlayerRank(string rank)
        {
            _proPlayerAnimation.SetAndAnimateRankText(rank);
        }

        private void ProPlayerScoreIncrementAction(AnalysisAnimationPrefab animation)
        {
            var currentMatches = int.Parse(_proPlayerTopMatchedText.text);
            SetProPlayerTopMatched(_proPlayerMatches, (_proPlayerMatches > currentMatches));
        }

        // SET OPPONENT VALUES
        internal void SetOpponentName(string name)
        {
            _opponentNameText.text = name;
        }

        internal void SetOpponentCounterIcon(Sprite counterSprite)
        {
            _opponentCounterImage.sprite = counterSprite;
        }

        internal void SetOpponentScore(int score)
        {
            _opponentScore = score;
        }

        internal void SetOpponentTopMatches(int matches)
        {
            _opponentMatches = matches;
        }

        internal void SetOpponentTopMatched(int matched, bool highlight = false)
        {
            _opponentTopMatchedText.text = matched.ToString();
            _opponentTopMatchedHighlight.gameObject.SetActive(highlight);
        }

        internal void SetAndAnimateOpponentRank(string rank)
        {
            _opponentAnimation.SetAndAnimateRankText(rank);
        }

        private void OpponentScoreIncrementAction(AnalysisAnimationPrefab animation)
        {
            var currentMatches = int.Parse(_opponentTopMatchedText.text);
            SetOpponentTopMatched(_opponentMatches, (_opponentMatches > currentMatches));
        }

        // GAME TURN
        internal void SetMatchGameValues(int game, int gameOf, int playedFor)
        {
            _matchGameText.text = $"Game {game}/{gameOf} for {playedFor} Points";

            if (Main.Instance.IfPlayerVsAI)
            {
                _matchGameText.text = playedFor == 1 ? "Single Game" : $"Match for {playedFor} Points";
            }
        }

        internal void SetMatchPointsWon(int playerProPoints, int opponentPoints)
        {
            _playerMatchPointsText.text = playerProPoints.ToString();
            _proPlayerMatchPointsText.text = playerProPoints.ToString();
            _opponentMatchPointsText.text = opponentPoints.ToString();
        }

        internal void SetMatchPointsWonAnimation(bool playerWon, int pointsStart, int pointsEnd)
        {
            var points = pointsEnd - pointsStart;

            if (playerWon)
            {
                _gameWonAnimation1.SetPointsScoredText(_playerMatchPointsText.gameObject.transform.position + _offset, points, 1f);
                if (!Main.Instance.IfPlayerVsAI)
                    _gameWonAnimation2.SetPointsScoredText(_proPlayerMatchPointsText.gameObject.transform.position + _offset, points, 1f);
            }
            else _gameWonAnimation1.SetPointsScoredText(_opponentMatchPointsText.gameObject.transform.position - _offset, points, 1f);

            StartCoroutine(MatchPointsWonDelayCoroutine(playerWon, pointsEnd));
        }

        private IEnumerator MatchPointsWonDelayCoroutine(bool playerWon, int points)
        {
            yield return new WaitForSeconds(2f);

            if (playerWon)
            {
                _playerMatchPointsText.text = points.ToString();
                _proPlayerMatchPointsText.text = points.ToString();
            }
            else _opponentMatchPointsText.text = points.ToString();
        }

        internal void SetGameTurn(int gameturn)
        {
            _gameTurnText.text = "TURN " + gameturn.ToString();
        }

        internal void ResetTurnIndicators()
        {
            _playerTopMatchedHighlight.gameObject.SetActive(false);
            _proPlayerTopMatchedHighlight.gameObject.SetActive(false);
            _opponentTopMatchedHighlight.gameObject.SetActive(false);
        }

        internal void Reset()
        {
            SetPlayerScore(0);
            SetOpponentScore(0);

            SetPlayerTopMatches(0);
            SetProPlayerTopMatches(0);
            SetOpponentTopMatches(0);

            SetPlayerTopMatched(0);
            SetProPlayerTopMatched(0);
            SetOpponentTopMatched(0);

            ResetTurnIndicators();

            SetGameTurn(0);
            SetUndoButtonActive(false);

            _playerAnimation.Reset();
            _proPlayerAnimation.Reset();
            _opponentAnimation.Reset();

            _gameWonAnimation1.Reset();
            _gameWonAnimation2.Reset();
        }

        // MENU OPTIONS
        private void OnClickMenu()
        {
            _displayMenu = !_displayMenu;

            _menuOptionsContainer.gameObject.SetActive(_displayMenu);
            _concedeOptionsContainer.gameObject.SetActive(false);
        }

        internal void SetUndoButtonActive(bool active)
        {
            _undoMoveButton.interactable = active;
        }

        private void OnClickUndoMove()
        {
            // TOGGLE MENU OFF
            OnClickMenu();
            Game2D.Context.UndoPlayerMove = true;
        }

        private void OnClickConcedeGame()
        {
            // TOGGLE MENU OFF
            OnClickMenu();
            _concedeOptionsContainer.gameObject.SetActive(!_concedeOptionsContainer.gameObject.activeInHierarchy);
        }

        private void OnClickConfirmConcede()
        {
            _concedeOptionsContainer.gameObject.SetActive(false);
            Game2D.Context.ConcedeTheGame = true;
        }
        
        private void OnClickCancelConcede()
        {
            // TOGGLE MENU OFF
            OnClickMenu();
            _concedeOptionsContainer.gameObject.SetActive(false);
        }

        private void OnClickExit()
        {
            // TOGGLE MENU OFF
            OnClickMenu();
            _exitOptionsContainer.gameObject.SetActive(!_exitOptionsContainer.gameObject.activeInHierarchy);
        }

        private void OnClickConfirmExit()
        {
            _exitOptionsContainer.gameObject.SetActive(false);
            Game2D.Context.ExitFromStateMachine = true;
        }

        private void OnClickCancelExit()
        {
            // TOGGLE MENU OFF
            OnClickMenu();
            _exitOptionsContainer.gameObject.SetActive(false);
        }

        private void OnClickStats()
        {
            _displayStats = !_displayStats;

            _inGameStatsContainer.gameObject.SetActive(_displayStats);

            SetInGameStats();
        }

        private void SetInGameStats()
        {
            var opponent = Game2D.Context.IfPlayingAsPlayer1 ? Game2D.Context.SelectedMatch.Player2Surname :
                                                             Game2D.Context.SelectedMatch.Player1Surname;
            var proPlayer = Game2D.Context.IfPlayingAsPlayer1 ? Game2D.Context.SelectedMatch.Player1Surname :
                                                             Game2D.Context.SelectedMatch.Player2Surname;

            _ranksDescriptionText.text = "Each #Rank 1 move = 3pts\r\n#Rank 2 = 2 pts\r\nThis Game:";
            _opponentStatsText.text = $"{opponent}:\r\n" + 
                                      $"#Rank 1 moves made = {Game2D.Context.OpponentTopRankedThisGame}\r\n" +
                                      $"#Rank2 moves made = {Game2D.Context.OpponentSecondRankedThisGame}\r\n" +
                                      $"Total Score = {Game2D.Context.OpponentScoreThisGame} " +
                                      $"({(Game2D.Context.TotalValidOpponentMovesThisGame * 3)})";
            _proPlayerStatsText.text = $"{proPlayer}:\r\n" + 
                                       $"#Rank 1 moves made = {Game2D.Context.ProTopRankedThisGame}\r\n" +
                                       $"#Rank2 moves made = {Game2D.Context.ProSecondRankedThisGame}\r\n" +
                                       $"Total Score = {Game2D.Context.ProScoreThisGame} " +
                                       $"({(Game2D.Context.TotalValidPlayerMovesThisGame * 3)})";
            _playerStatsText.text = $"You:\r\n" + 
                                    $"#Rank 1 moves made = {Game2D.Context.PlayerTopRankedThisGame}\r\n" +
                                    $"#Rank2 moves made = {Game2D.Context.PlayerSecondRankedThisGame}\r\n" +
                                    $"Total Score = {Game2D.Context.PlayerScoreThisGame} " +
                                       $"({(Game2D.Context.TotalValidPlayerMovesThisGame * 3)})";
        }

        // SHOW OPPONENT RANK 1 MOVE
        internal void SetShowOpponentRank1Active(bool active)
        {
            _showOpponentRank1ButtonContainer.gameObject.SetActive(active);
        }

        private void ShowOpponentRank1Move()
        {
            Game2D.Context.ShowOpponentRank1Move = true;
        }

        // PRIVATE FIELDS

        private AnalysisAnimationPrefab _playerAnimation;
        private AnalysisAnimationPrefab _proPlayerAnimation;
        private AnalysisAnimationPrefab _opponentAnimation;

        private GameWonAnimationPrefab _gameWonAnimation1 = null;
        private GameWonAnimationPrefab _gameWonAnimation2 = null;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector3 _offset = new Vector3(0f, .75f, 0f);

        private int _playerScore = 0;
        private int _opponentScore = 0;

        private int _playerMatches = 0;
        private int _proPlayerMatches = 0;
        private int _opponentMatches = 0;

        private bool _displayMenu = false;
        private bool _displayStats = false;
    }
}