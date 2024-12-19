using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class GameListUI : MonoBehaviour
    {
        [Header("MAIN BODY")]
        [SerializeField] private Text titleText = null;
        [SerializeField] private Text player1Text = null;
        [SerializeField] private Text player2Text = null;
        [SerializeField] private Text matchName = null;
        [SerializeField] private Text pointsToWin = null;
        [SerializeField] private Text backButtonText = null;

        [Header("CONTINUE")]
        [SerializeField] GameObject continueMatchOptionContainer;
        [SerializeField] GameObject highlightPlayer1;
        [SerializeField] GameObject highlightPlayer2;

        [Header("DEMO")]
        [SerializeField] private GameObject demoPopUp = null;
        [SerializeField] private Text demoPopUpText = null;
        [SerializeField] private Text demoPopUpContinueButtonText = null;

        [Header("LOWER CONTAINER - NULL?")]
        [SerializeField] private Text gamePlayed = null;
        [SerializeField] private Text gamePercentage = null;

        // CONTINUE PLAYING AS OBJECTS
        MatchScoreData continueMatchScore;
        LanguageScriptableObject languageSO;

        private float timer = 0f;

        public GameObject game;
        private Game gameScript;

        protected void Start()
        {
            gameScript = game.GetComponent<Game>();
        }

        protected void OnEnable()
        {
            // CONFIGURE LANGUAGE

            languageSO = Main.Instance.WorldRegionObj.LanguageSO;
            Backgammon_Asset.MatchData match = MatchSelectUI.Match;

            if (match != null)
            {
                if (languageSO != null)
                {
                    titleText.text = languageSO.gameListTitleText;
                    player1Text.text = match.Player1;
                    player2Text.text = match.Player2;
                    matchName.text = match.Title + " " + match.Round;
                    pointsToWin.text = languageSO.gameListMatchPlayedFor + " " + match.Points + " " + languageSO.points;
                    backButtonText.text = languageSO.Back;
                }
                else
                {
                    player1Text.text = match.Player1;
                    player2Text.text = match.Player2;
                    matchName.text = match.Title + " " + match.Round;
                    pointsToWin.text = "Match played for " + match.Points + " points";
                }

                if (Main.Instance.IfDemoIsInPlay)
                {
                    pointsToWin.text = string.Empty;
                }
            }

            RefreshGameText();

            confirmDemo = false;
            ifCommence = false;
            ifQuit = false;

            continueMatchOptionContainer.gameObject.SetActive(false);

            if (!Main.Instance.IfDemoIsInPlay)
            {
                continueMatchScore = Main.Instance.PlayerScoresObj.GetPlayerMatchScore(match.Title + " " + match.ID);
                var lastGamePlayed = continueMatchScore.lastGamePlayed;

                if (0 < lastGamePlayed && lastGamePlayed < match.GameCount + 1)
                {
                    continueMatchOptionContainer.gameObject.SetActive(true);
                    var leftArrow = continueMatchOptionContainer.transform.Find("playerLeft");
                    var rightArrow = continueMatchOptionContainer.transform.Find("playerRight");

                    var lastPlayedAs = continueMatchScore.lastPlayedAs;
                    var xPos = Mathf.Abs(continueMatchOptionContainer.transform.position.x);
                    var yPos = continueMatchOptionContainer.transform.position.y;

                    if (lastPlayedAs == 1)
                    {
                        continueMatchOptionContainer.transform.position = new Vector3(-xPos, yPos);
                        leftArrow.gameObject.SetActive(false);
                        rightArrow.gameObject.SetActive(true);
                    }
                    else
                    {
                        continueMatchOptionContainer.transform.position = new Vector3(xPos, yPos);
                        leftArrow.gameObject.SetActive(true);
                        rightArrow.gameObject.SetActive(false);
                    }
                }
            }

            if (Main.Instance.IfDemoIsInPlay)
            {
                demoPopUp.gameObject.SetActive(true);

                var demoText = languageSO.gameListDemoText1Line1 + "\n\n";
                demoText += languageSO.gameListDemoText1Line2 + "\n\n";
                demoText += languageSO.gameListDemoText1Line3;

                demoPopUpText.text = demoText;

                demoPopUpContinueButtonText.text = languageSO.Continue;
                demoOUT = false;
            }

            highlightPlayer1.gameObject.SetActive(true);
            highlightPlayer2.gameObject.SetActive(true);

            timer = 1.0f;
        }

        protected void Update()
        {
            timer -= Time.deltaTime;

            if (timer < 0)
                timer = 1.0f;

            highlightPlayer1.gameObject.transform.localScale = new Vector3(1 + (timer / 30), 1 + (timer / 30), 1);
            highlightPlayer2.gameObject.transform.localScale = new Vector3(1 + (timer / 30), 1 + (timer / 30), 1);
        }

        public void OnPlayer1Clicked()
        {
            _playingAs3D = Game.PlayingAs.PLAYER_1;
            _playingAs2D = Game2D.PlayingAs.PLAYER_1;

            gameScript.playingAs = PlayerId.Player1;
            OnCommence();
        }

        public void OnPlayer2Clicked()
        {
            _playingAs3D = Game.PlayingAs.PLAYER_2;
            _playingAs2D = Game2D.PlayingAs.PLAYER_2;

            gameScript.playingAs = PlayerId.Player2;
            OnCommence();
        }

        public void OnClickContinueDemoPopup()
        {
            if (demoOUT)
            {
                demoPopUp.gameObject.SetActive(false);
                confirmDemo = true;
                return;
            }

            var demoText = languageSO.gameListDemoText2Line1 + "\n\n";
            demoText += languageSO.gameListDemoText2Line2 + "\n\n";
            demoText += languageSO.gameListDemoText2Line3 + "\n\n";
            demoText += languageSO.gameListDemoText2Line4 + " '" + languageSO.Continue + "' ";
            demoText += languageSO.gameListDemoText2Line5 + "\n\n";

            demoPopUpText.text = demoText;
            demoOUT = true;
        }

        public void OnClickedContinuePlayingAs()
        {
            var gameToContinue = continueMatchScore.lastGamePlayed - 1;

            if (continueMatchScore.lastGamePlayed == continueMatchScore.lastGameCompleted && continueMatchScore.lastGameCompleted < MatchSelectUI.Match.GameCount)
            {
                gameToContinue++;
            }

            ContinueGame(gameToContinue, 0, continueMatchScore.lastPlayedAs);
        }

        public void OnClickPrev()
        {
            if (IndexGame > 0)
            {
                --IndexGame;
                RefreshGameText();
            }
        }

        public void OnClickNext()
        {
            if (IndexGame < MatchSelectUI.Match.GameCount - 1)
            {
                ++IndexGame;
                RefreshGameText();
            }
        }

        public void OnCommence()
        {
            if (MatchSelectUI.Match.Game(IndexGame).NumberOfMoves == 0)
            {
                return; // safeguard - ie. game data not yet setup
            }

            ifCommence = true;
        }

        public void OnQuit()
        {
            IndexGame = 0;
            MatchSelectUI.Match = null;

            ifQuit = true;
        }

        private void RefreshGameText()
        {
            gamePlayed.text = "Game " + (IndexGame + 1) + " of " + MatchSelectUI.Match.GameCount;
        }

        public void ContinueGame(int continueGameIndex, int continueGameTurn, int playingAs)
        {
            gameScript = game.GetComponent<Game>();

            if (IndexGame <= MatchSelectUI.Match.GameCount)
            {
                IndexGame = continueGameIndex;

                if (IndexTurn <= MatchSelectUI.Match.GameDataArray[IndexGame].NumberOfMoves)
                    IndexTurn = continueGameTurn;
            }

            if (playingAs == 1)
            {
                _playingAs3D = Game.PlayingAs.PLAYER_1;
                _playingAs2D = Game2D.PlayingAs.PLAYER_1;

                gameScript.playingAs = PlayerId.Player1;
            }
            else
            {
                _playingAs3D = Game.PlayingAs.PLAYER_2;
                _playingAs2D = Game2D.PlayingAs.PLAYER_2;
                
                gameScript.playingAs = PlayerId.Player2;
            }

            Debug.Log($"GAME LIST UI: GAME -> {IndexGame} - TURN -> {IndexTurn}");

            OnCommence();
        }

        public static Backgammon_Asset.GameData _game = null;
        public static Game.PlayingAs _playingAs3D = Game.PlayingAs.NONE;
        public static Game2D.PlayingAs _playingAs2D = Game2D.PlayingAs.NONE;

        public static int IndexGame = 0;
        public static int IndexTurn = 0;

        private string demoText1 = "Welcome to The Backgammon Challenge.\n\n" +
            "A new way to make use of recorded backgammon matches has been invented for this game app.\n\n" +
            "It's a unique way to match your playing skills against the best players in the world.";
        private string demoText2 = "For this game we have reporoduced a game from a match between two great players, Joe Dwek and Phillip Martyn.\n\n" +
            "You'll be challenging each of these world class players in turn.\n\n" +
            "Which player woud you like to challenge first?\n\n" +
            "Click 'Continue' to select a player.";
        private bool demoOUT = false;

        public bool confirmDemo;
        public bool ifCommence;
        public bool ifQuit;
    }
}