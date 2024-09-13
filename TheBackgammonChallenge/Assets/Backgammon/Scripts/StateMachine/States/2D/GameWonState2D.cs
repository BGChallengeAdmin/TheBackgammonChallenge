namespace Backgammon
{
    public class GameWonState2D : GameState2D
    {
        public GameWonState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: GAME WON");

            SetEndOfGameMatchStats();

            var wonMatchByPoints = (Context.SelectedGame.Player1PointsAtEnd >= Context.SelectedMatch.Points) ||
                                  (Context.SelectedGame.Player2PointsAtEnd >= Context.SelectedMatch.Points);
            Context.GameWonUI.SetGameWonButtonText(!wonMatchByPoints);

            //Context.ChallengeProgressionUI.SetActive(true);

            _delayTimer = _delayTime;
        }

        public override void UpdateState()
        {
            //if (_delayTimer > 0)
            //{
            //    _delayTimer -= Time.deltaTime;

            //    if (_delayTimer <= 0)
            //        Context.ChallengeProgressionUI.SetSliderValue(1f);
            //}

            if (!Context.GameWonUI.IfClicked) return;

            if (Context.GameWonUI.ClickedConfirm)
            {
                var wonMatchByPoints = (Context.SelectedGame.Player1PointsAtEnd >= Context.SelectedMatch.Points) ||
                                      (Context.SelectedGame.Player2PointsAtEnd >= Context.SelectedMatch.Points);

                Context.IfPlayNextGame = !wonMatchByPoints;
                Context.IfPlayAnotherMatch = wonMatchByPoints;

                // UPDATE A.I. GAME
                if (Main.Instance.IfPlayerVsAI && !wonMatchByPoints)
                {
                    Context.AIGameData.GameConstructor(Context.AIGameData.Player1PointsAtEnd,
                                                       Context.AIGameData.Player2PointsAtEnd,
                                                       Context.AIGameData.Moves,
                                                       Context.AIGameData.Player1PointsAtEnd,
                                                       Context.AIGameData.Player2PointsAtEnd);
                }
            }

            ActiveState = GameStateMachine2D.EGameState2D.ExitGame;
        }

        public override void ExitState()
        {
            Context.GameWonUI.SetActive(false);
            Context.GameScreenUI.SetActive(false);
            Context.ChallengeProgressionUI.SetActive(false);

            Game.IfGameConcluded = true;
        }

        private void SetEndOfGameMatchStats()
        {
            // SET WINNER TEXT

            var wonMatchByPoints = (Context.SelectedGame.Player1PointsAtEnd >= Context.SelectedMatch.Points) ||
                                  (Context.SelectedGame.Player2PointsAtEnd >= Context.SelectedMatch.Points);
            var gameOrMatch = wonMatchByPoints ? "Match" : "Game";
            var winnerWasP1 = Context.SelectedMatch.Game(Context.IndexGame).Winner() == 1;
            var winnersName = winnerWasP1 ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;

            var gameWonByText = wonMatchByPoints ? "Congratulations!!\n\n" : string.Empty;

            if (Main.Instance.IfPlayerVsAI)
            {
                gameWonByText += winnersName + " won the " + gameOrMatch;
                gameWonByText += Context.GameWonByGammon ? "\nBy Gammon!" : string.Empty;
                gameWonByText += Context.GameWonByBackGammon ? "\nBy BackGammon!!" : string.Empty;
            }
            else gameWonByText += gameOrMatch + " was Won by\n" + winnersName;

            Context.GameWonUI.SetGameWonText(gameWonByText);

            // MATCH AND GAME SCORES
            Backgammon_Asset.GameData game = Context.SelectedGame;
            string gameName = game.name;
            string matchKey = Context.SelectedMatch.Title + " " + Context.SelectedMatch.ID;

            PlayerScoreData scoreData = Main.Instance.PlayerScoresObj.GetPlayerScoreData(matchKey);
            MatchScoreData currentMatch = scoreData.matchScores.ID != string.Empty ? scoreData.matchScores : new MatchScoreData();
            GameScoreData currentGame = currentMatch.gameScoresDict.ContainsKey(gameName) ?
                                            currentMatch.gameScoresDict[gameName] : new GameScoreData();

            // PLAYER INFO
            SetPlayerStatsPanel();
            SetPlayerProStatsPanel();
            SetOpponentStatsPanel();

            // GAME POINTS WON ANIMATION
            var playerWon = (Context.IfPlayingAsPlayer1 && winnerWasP1) || (!Context.IfPlayingAsPlayer1 && !winnerWasP1);

            var pointsStart = Context.IfPlayer1Turn ? Context.SelectedGame.Player1PointsAtStart :
                                                      Context.SelectedGame.Player2PointsAtStart;

            var pointsEnd = Context.IfPlayer1Turn ? Context.SelectedGame.Player1PointsAtEnd :
                                                    Context.SelectedGame.Player2PointsAtEnd;

            Context.GameScreenUI.SetMatchPointsWonAnimation(playerWon, pointsStart, pointsEnd);

            Context.GameWonUI.SetActive(true);
        }

        private void SetPlayerStatsPanel()
        {
            var playerInfoText = $"You matched {Context.TotalValidPlayerMatchesThisGame}/{Context.TotalValidPlayerMovesThisGame} moves";
            playerInfoText += $"\n";
            playerInfoText += $"You had\n {Context.PlayerTopRankedThisGame} Rank #1 for {(Context.PlayerTopRankedThisGame * 3)} Pts.";
            playerInfoText += $"\n{Context.PlayerSecondRankedThisGame} Rank #2 for {(Context.PlayerSecondRankedThisGame * 2)} Pts.";
            playerInfoText += $"\nFor a total of {Context.PlayerScoreThisGame} / {(Context.TotalValidPlayerMovesThisGame * 3)} possible Pts.";
            Context.GameWonUI.SetPlayerGameWonInfoText(playerInfoText);
        }

        private void SetPlayerProStatsPanel()
        {
            var playerPro = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
            var playerProInfoText = $"{playerPro} had\n\n{Context.ProTopRankedThisGame} Rank #1 for {(Context.ProTopRankedThisGame * 3)} Pts.";
            playerProInfoText += $"\n{Context.ProSecondRankedThisGame} Rank #2 for {(Context.ProSecondRankedThisGame * 2)} Pts.";
            playerProInfoText += $"\nFor a total of {Context.ProScoreThisGame} / {(Context.TotalValidPlayerMovesThisGame * 3)} possible Pts.";

            Context.GameWonUI.SetPlayerProGameWonInfoText(playerProInfoText);
        }

        private void SetOpponentStatsPanel()
        {
            var opponent = !Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
            var points = (Context.OpponentTopRankedThisGame * 3) + (Context.OpponentSecondRankedThisGame * 2);
            var possible = (Context.TotalValidOpponentMovesThisGame * 3);
            
            var opponentInfoText = $"{opponent} had \n\n{Context.OpponentTopRankedThisGame} Rank #1 for {(Context.OpponentTopRankedThisGame * 3)} Pts.";
            opponentInfoText += $"\n{Context.OpponentSecondRankedThisGame} Rank#2 for {(Context.OpponentSecondRankedThisGame * 2)} Pts.";
            opponentInfoText += $"\nFor a total of {points}/{possible} possible Pts.";
            Context.GameWonUI.SetOpponentGameWonInfoText(opponentInfoText);
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private float _delayTime = 1f;
        private float _delayTimer = 1f;
    }
}