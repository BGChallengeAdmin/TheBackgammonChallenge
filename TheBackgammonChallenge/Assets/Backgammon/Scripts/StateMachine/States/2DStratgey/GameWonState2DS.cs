namespace Backgammon
{
    public class GameWonState2DS : GameState2DStrategy
    {
        public GameWonState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: GAME WON");

            SetEndOfGameMatchStats();
            Context.GameScreenUI.SetMenuAndStatsInteractible(false);

            var wonMatchByPoints = (Context.GameConfig.SelectedGame.Player1PointsAtEnd >= Context.GameConfig.SelectedMatch.Points) ||
                                  (Context.GameConfig.SelectedGame.Player2PointsAtEnd >= Context.GameConfig.SelectedMatch.Points);
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
                var wonMatchByPoints = (Context.GameConfig.SelectedGame.Player1PointsAtEnd >= Context.GameConfig.SelectedMatch.Points) ||
                                      (Context.GameConfig.SelectedGame.Player2PointsAtEnd >= Context.GameConfig.SelectedMatch.Points);

                Context.GameConfig.IfPlayNextGame = !wonMatchByPoints;
                Context.GameConfig.IfPlayAnotherMatch = wonMatchByPoints;

                // UPDATE A.I. GAME
                if (!wonMatchByPoints)
                    Context.Strategy.GameWonUpdateAIScore(Context);
            }

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;
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

            var wonMatchByPoints = (Context.GameConfig.SelectedGame.Player1PointsAtEnd >= Context.GameConfig.SelectedMatch.Points) ||
                                  (Context.GameConfig.SelectedGame.Player2PointsAtEnd >= Context.GameConfig.SelectedMatch.Points);
            var gameOrMatch = wonMatchByPoints ? "Match" : "Game";
            var winnerWasP1 = Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Winner() == 1;
            var winnersName = winnerWasP1 ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;

            var gameWonByText = wonMatchByPoints ? "Congratulations!!\n\n" : string.Empty;

            gameWonByText = Context.Strategy.GameWonUpdateScoreText(Context, gameWonByText, gameOrMatch, winnersName);

            Context.GameWonUI.SetGameWonText(gameWonByText);

            // MATCH AND GAME SCORES
            Backgammon_Asset.GameData game = Context.GameConfig.SelectedGame;
            string gameName = game.name;
            string matchKey = Context.GameConfig.SelectedMatch.Title + " " + Context.GameConfig.SelectedMatch.ID;

            PlayerScoreData scoreData = Main.Instance.PlayerScoresObj.GetPlayerScoreData(matchKey);
            MatchScoreData currentMatch = scoreData.matchScores.ID != string.Empty ? scoreData.matchScores : new MatchScoreData();
            GameScoreData currentGame = currentMatch.gameScoresDict.ContainsKey(gameName) ?
                                            currentMatch.gameScoresDict[gameName] : new GameScoreData();

            // PLAYER INFO
            SetPlayerStatsPanel();
            _ = Context.Strategy.GameWonUpdateStatsText(Context, setTextToStats: true);

            //SetPlayerProStatsPanel();
            //SetOpponentStatsPanel();

            // GAME POINTS WON ANIMATION
            var playerWon = (Context.GameConfig.IfPlayingAsPlayer1 && winnerWasP1) || (!Context.GameConfig.IfPlayingAsPlayer1 && !winnerWasP1);

            var pointsStart = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedGame.Player1PointsAtStart :
                                                      Context.GameConfig.SelectedGame.Player2PointsAtStart;

            var pointsEnd = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedGame.Player1PointsAtEnd :
                                                    Context.GameConfig.SelectedGame.Player2PointsAtEnd;

            Context.GameScreenUI.SetMatchPointsWonAnimation(playerWon, pointsStart, pointsEnd);

            Context.GameWonUI.SetActive(true);
        }

        private void SetPlayerStatsPanel()
        {
            var playerInfoText = Context.Strategy.GameWonUpdateStatsText(Context);

            playerInfoText += $"You had\n {Context.GameStats.PlayerTopRankedThisGame} Rank #1 for {(Context.GameStats.PlayerTopRankedThisGame * 3)} Pts.";
            playerInfoText += $"\n{Context.GameStats.PlayerSecondRankedThisGame} Rank #2 for {(Context.GameStats.PlayerSecondRankedThisGame * 2)} Pts.";
            playerInfoText += $"\nFor a total of {Context.GameStats.PlayerScoreThisGame} / {(Context.GameStats.TotalValidPlayerMovesThisGame * 3)} possible Pts.";

            Context.GameWonUI.SetGameWonRightInfoText(playerInfoText);
        }

        private void SetPlayerProStatsPanel()
        {
            var playerPro = Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;
            var playerProInfoText = $"{playerPro} had\n\n{Context.GameStats.ProTopRankedThisGame} Rank #1 for {(Context.GameStats.ProTopRankedThisGame * 3)} Pts.";
            playerProInfoText += $"\n{Context.GameStats.ProSecondRankedThisGame} Rank #2 for {(Context.GameStats.ProSecondRankedThisGame * 2)} Pts.";
            playerProInfoText += $"\nFor a total of {Context.GameStats.ProScoreThisGame} / {(Context.GameStats.TotalValidPlayerMovesThisGame * 3)} possible Pts.";

            if (!Main.Instance.IfPlayerVsAI)
                Context.GameWonUI.SetGameWonLeftInfoText(playerProInfoText);
        }

        private void SetOpponentStatsPanel()
        {
            var opponent = !Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;
            var points = (Context.GameStats.OpponentTopRankedThisGame * 3) + (Context.GameStats.OpponentSecondRankedThisGame * 2);
            var possible = (Context.GameStats.TotalValidOpponentMovesThisGame * 3);

            var opponentInfoText = $"{opponent} had \n\n{Context.GameStats.OpponentTopRankedThisGame} Rank #1 for {(Context.GameStats.OpponentTopRankedThisGame * 3)} Pts.";
            opponentInfoText += $"\n{Context.GameStats.OpponentSecondRankedThisGame} Rank#2 for {(Context.GameStats.OpponentSecondRankedThisGame * 2)} Pts.";
            opponentInfoText += $"\nFor a total of {points}/{possible} possible Pts.";

            if (Main.Instance.IfPlayerVsAI)
                Context.GameWonUI.SetGameWonLeftInfoText(opponentInfoText);
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private float _delayTime = 1f;
        private float _delayTimer = 1f;
    }
}