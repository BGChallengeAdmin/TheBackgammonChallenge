using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Backgammon
{
    public class MatchAISelectPoints : MonoBehaviour
    {
        [SerializeField] private Backgammon_Asset.MatchReplayDLC _aiMatch = null;
        [SerializeField] private AISettingsScriptableObject _aiSettingsSO = null;

        [Header("BUTTONS")]
        [SerializeField] Button _continueAIButton = null;

        // LANGUAGE FIELDS
        [Header("HEADERS")]
        [SerializeField] private Text _titleText = null;
        [SerializeField] private Text _descriptionText = null;
        [SerializeField] private Text _singleGameButtonText = null;
        [SerializeField] private Text _matchButtonText = null;
        [SerializeField] private Text _backButtonText = null;
        [SerializeField] private Text _continueButtonText = null;
        [SerializeField] private Text _commenceButtonText = null;

        [Header("MATCH PLAYED FOR")]
        [SerializeField] private TMP_Text _matchPlayedForText = null;

        [Header("BACKGROUND IMAGE")]
        [SerializeField] private Image _backgroundImage1;

        private LanguageScriptableObject languageSO = null;
        private bool _ifBack = false;
        private bool _ifCommence = false;
        private bool _ifContinue = false;

        private int _highestRankValue = 1;
        private int _lowestRankValue = 5;

        private int _points = 5;
        private int _ply = 1;
        private float _noise = 0.01f;

        private string _preset = string.Empty;

        protected void OnEnable()
        {
            IfBack = false;
            IfCommence = false;
            IfContinue = false;

            // SET VALUES TO PLAYER SAVED VALUES
            _highestRankValue = _aiSettingsSO.PlayerRankHighest;
            _lowestRankValue = _aiSettingsSO.PlayerRankLowest;
            _points = _aiSettingsSO.PlayerPoints;
            _ply = _aiSettingsSO.PlayerPly;
            _noise = _aiSettingsSO.PlayerNoise;
            _preset = _aiSettingsSO.PlayerPreset;

            SetPointsValue();

            // TEST IF A.I. GAME TO CONTINUE
            if (!Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame.IfMatchToContinue)
                _continueAIButton.interactable = false;
            else _continueAIButton.interactable = true;

            // CONFIGURE LANGUAGE BY REGION
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                //_descriptionText.text = languageSO.Using + " ";

                //if (Game.aiIfUsingHistoricDice)
                //{
                //    _descriptionText.text += languageSO.matchAIDiceTypeHistoric + " " + languageSO.matchAIDiceTypeDiceRolls;
                //    _descriptionText.text += "\n- " + languageSO.matchAISelectMatchPlayedForHistoric + " " + _points + " " + languageSO.points; ;
                //}
                //else if (Game.aiIfUsingManualDice)
                //{
                //    _descriptionText.text += languageSO.matchAIDiceTypeManual + " " + languageSO.matchAIDiceTypeDiceRolls;
                //}
                //else
                //{
                //    _descriptionText.text += languageSO.matchAIDiceTypeNormal + " " + languageSO.matchAIDiceTypeDiceRolls;
                //}

                _titleText.text = languageSO.matchAIPointsTitle;
                _descriptionText.text = languageSO.matchAIPointsDescription;

                _singleGameButtonText.text = languageSO.matchAIPointsSingleGame;
                _matchButtonText.text = languageSO.matchAIPointsMatch;

                _backButtonText.text = languageSO.Back;
                _commenceButtonText.text = languageSO.Continue;
            }
        }

        // ------------------------------------------------- A.I. SETTIGNS ----------------------------------------------

        public void OnClickedPointsButton(int upOrDown)
        {
            var increment = upOrDown == 0 ? -6 :
                            upOrDown == 1 ? -2 :
                            upOrDown == 2 ? 2 : 6;

            _points = Mathf.Clamp((_points + increment), _aiSettingsSO.PointsMin, _aiSettingsSO.PointsMax);

            SetPointsValue();
        }

        private void SetPointsValue()
        {
            _matchPlayedForText.text = _points.ToString();
        }

        // ------------------------------------------------- BUTTON CLICKS ----------------------------------------------

        public void OnClickSingleGame()
        {
            _points = 1;
            OnClickCommence();
        }

        public void OnClickPlayMatch()
        {
            _points = 5;
            OnClickCommence();
        }

        public void OnClickContinue()
        {
            IfContinue = true;
            OnClickCommence(false);
        }

        public void OnClickBack()
        {
            IfBack = true;
        }

        public void OnClickCommence(bool resetGame = true)
        {
            // SET ALL PLAYER VALUES TO AI SETTINGS
            _aiSettingsSO.PlayerRankHighest = _highestRankValue;
            _aiSettingsSO.PlayerRankLowest = _lowestRankValue;
            _aiSettingsSO.PlayerPoints = _points;
            _aiSettingsSO.PlayerPly = _ply;
            _aiSettingsSO.PlayerNoise = _noise;
            _aiSettingsSO.GamesPlayed = 0;

            // RESET ALL VALUES - POINTS AND GAME CAN BE SEPARATE
            Backgammon_Asset.MatchData match = _aiMatch.Match(0);
            Backgammon_Asset.GameData game = match.Game(0);

            // NOTE: SCRIPTABLE OBJECT - DATA PERSISTENCE - MUST BE '0'
            if (resetGame) game.GameConstructor(0, 0, game.Moves, 0, 0);

            match.GameDataArray[0] = game;

            _aiMatch.Match(0).MatchConstructor(
                match.Title, match.ID, match.Event, match.Round,
                _points,
                match.Player1Surname, match.Player1, match.Player2Surname, match.Player2, match.Crawford,
                match.GameDataArray);

            // RESET ENTRY IN PLAYER SCORE DATA
            PlayerScoreData scoreData = new PlayerScoreData();
            scoreData.matchKey = match.Title + " " + match.ID;
            Main.Instance.PlayerScoresObj.SetPlayerScoreData(scoreData);

            // SET GLOBAL VALUES FOR GAME
            MatchSelectUI.Match = _aiMatch.Match(0);
            GameListUI.IndexGame = 0;
            GameListUI.IndexTurn = 0;
            GameListUI.playingAs = PlayerId.Player1;

            // NOTE: SCRIPTABLE OBJECT - RECONFIGURE BOARD
            _aiSettingsSO.ContinueBoardState.ResetToNewGame();
            _aiSettingsSO.PlayerTurnID = Game2D.PlayingAs.PLAYER_1;

            Main.Instance.IfMatchedPlay = true;
            Main.Instance.IfPlayerVsAI = true;

            IfCommence = true;
        }
        public void EnableDefaultBackground(bool enable)
        {
            _backgroundImage1.enabled = enable;
        }

        // ----------------------------------------------- GETTERS && SETTERS -----------------------------------------------

        public bool IfBack
        {
            get => _ifBack;
            private set => _ifBack = value;
        }

        public bool IfCommence
        {
            get => _ifCommence;
            private set => _ifCommence = value;
        }

        public bool IfContinue
        {
            get => _ifContinue;
            private set => _ifContinue = value;
        }
    }
}
