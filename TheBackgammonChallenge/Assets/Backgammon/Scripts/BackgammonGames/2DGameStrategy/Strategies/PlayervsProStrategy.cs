using static Backgammon.GeneralInfoState2D;

namespace Backgammon
{
    public class PlayervsProStrategy : IStrategy
    {
        public GameScoreInfo.ContinueGame GameToContinue => Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame;

        public void ConfigureBoardContinueEnter(GameStateContext2DStrategy Context) { return; }

        public void RollForGoesFirstEnter(GameStateContext2DStrategy Context, bool playerGoesFirst, ref int playerDice, ref int opponentDice)
        {
            var dice = Context.GameConfig.SelectedGame.GetPlayerMove((Context.TurnConfig.IfPlayer1GoesFirst ? 0 : 1)).Split(":")[0];

            var dice1 = int.Parse(dice[0].ToString());
            var dice2 = int.Parse(dice[1].ToString());

            var larger = dice1 > dice2 ? dice1 : dice2;
            var smaller = dice1 < dice2 ? dice1 : dice2;

            playerDice = playerGoesFirst ? larger : smaller;
            opponentDice = playerGoesFirst ? smaller : larger;

            Context.GameTurnsList[0].Dice1 = playerDice;
            Context.GameTurnsList[0].Dice2 = opponentDice;
        }

        public string BeforeCommenceEnter(GameStateContext2DStrategy Context)
        {
            return (Context.TurnConfig.IfPlayer1GoesFirst ? 
                        Context.GameConfig.SelectedMatch.Player1 : 
                        Context.GameConfig.SelectedMatch.Player2) + " goes first.";
        }

        public int[] TurnConfigureEnter(GameStateContext2DStrategy Context, int[] dice)
        {
            var turnData = Context.GameConfig.SelectedGame.GetPlayerMove(Context.TurnConfig.IndexTurn);

            if (turnData.Contains("Wins") || turnData.Contains("Loses") || turnData.Contains("Doubles") ||
                turnData.Contains("Takes") || turnData.Contains("Drops") || turnData.Contains("Concedes"))
                return dice;            

            var diceString = turnData.Split(":")[0];
            
            if (diceString == string.Empty) return dice;

            var dice1 = int.Parse(diceString[0].ToString());
            var dice2 = int.Parse(diceString[1].ToString());
            
            Context.TurnConfig.Dice1 = dice1;
            Context.TurnConfig.Dice2 = dice2;

            dice[0] = dice1;
            dice[1] = dice2;

            return dice;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy TurnConfigureEnterSelectState(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.TurnBegin;
        }

        public string TurnBeginEnterGetMoveData(GameStateContext2DStrategy Context)
        {
            return Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).GetPlayerMove(Context.TurnConfig.IndexTurn);
        }

        public string[] TurnBeginEnterGetPlayerConcedes(GameStateContext2DStrategy Context)
        {
            if (Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).GetPlayerMove(Context.TurnConfig.IndexTurn + 1).Contains("Wins") &&
                    !(Context.HomeManager.PlayerHome.Counters == 15 || Context.HomeManager.OpponentHome.Counters == 15))
            {
                return new string[] { "Concedes" };
            }
            else return new string[] { string.Empty };
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy TurnBeginEnterSelectState(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy AIDataEnter(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;
        }

        public AIData AIDataEnterConstructData(GameStateContext2DStrategy Context, AIData _aiDataToSend)
        {
            return _aiDataToSend;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy AIDataUpdate(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy DoublingDataUpdate(GameStateContext2DStrategy Context,
                                                               GameStateMachine2DStrategy.EGameState2DStrategy passThroughState)
        {
            return passThroughState;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy DoublingDataBailOut(GameStateContext2DStrategy Context)
        {
            if (Context.TurnConfig.PlayerOfferedDoubles)
                return GameStateMachine2DStrategy.EGameState2DStrategy.DoublingInGame;
            else 
                return GameStateMachine2DStrategy.EGameState2DStrategy.DoublingOffers;
        }

        public void DoublingOffersEnter(GameStateContext2DStrategy Context)
        {
            var player = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;
            var opponent = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player2 : Context.GameConfig.SelectedMatch.Player1;

            Context.DoublingUI.SetOffersDoubleText(player, opponent);
        }

        public void DoublingOffersUpdate(GameStateContext2DStrategy Context) { return; }

        public GameStateMachine2DStrategy.EGameState2DStrategy DoublingInGameState(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;
        }

        public void DiceRollsDisplayUserHint(GameStateContext2DStrategy Context, int dice1, int dice2)
        {
            if (Context.TurnConfig.IndexTurn <= 5)
            {
                Context.DiceRollsUI.SetPlayerDiceRollsText((Context.GameConfig.IfPlayingAsPlayer1 ?
                                                            Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2),
                                                            dice1, dice2, true);
            }
            else Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, dice1, dice2, true);
        }

        public bool SelectPointFromEntry(GameStateContext2DStrategy Context, bool bar = false, bool bearingOff = false, bool normal = false) 
        {
            if (Context.HomeManager.PlayerHome.Counters < 15) return true;
            else return false;
        }

        public void EvaluatePlayerMoveEntry(GameStateContext2DStrategy Context) { return; }

        public void PlayoutProMovesEntry(GameStateContext2DStrategy Context) { return; }

        public void GameWonUpdateAIScore(GameStateContext2DStrategy Context) { return; }

        public string GameWonUpdateScoreText(GameStateContext2DStrategy Context, string gameWonByText, string gameOrMatch, string winnersName)
        {
            gameWonByText += gameOrMatch + " was Won by\n" + winnersName;
            return gameWonByText;
        }

        public string GameWonUpdateStatsText(GameStateContext2DStrategy Context, string playerInfoText, bool setTextToStats = false)
        {
            if (setTextToStats)
            {
                var playerPro = Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;
                var playerProInfoText = $"{playerPro} had\n\n{Context.GameStats.ProTopRankedThisGame} Rank #1 for {(Context.GameStats.ProTopRankedThisGame * 3)} Pts.";
                playerProInfoText += $"\n{Context.GameStats.ProSecondRankedThisGame} Rank #2 for {(Context.GameStats.ProSecondRankedThisGame * 2)} Pts.";
                playerProInfoText += $"\nFor a total of {Context.GameStats.ProScoreThisGame} / {(Context.GameStats.TotalValidPlayerMovesThisGame * 3)} possible Pts.";

                Context.GameWonUI.SetGameWonLeftInfoText(playerProInfoText);
            }

            playerInfoText += $"You matched {Context.GameStats.TotalValidPlayerMatchesThisGame}/{Context.GameStats.TotalValidPlayerMovesThisGame} moves";
            playerInfoText += $"\n";
            return playerInfoText;
        }

        public string GeneralInfoEnter(GameStateContext2DStrategy Context, EGeneralInfoState2D state)
        {
            var returnString = string.Empty;

            switch (state)
            {
                case EGeneralInfoState2D.PlayerUnableToMove:
                    {
                        var player = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 : 
                                                                        Context.GameConfig.SelectedMatch.Player2;
                        Context.GeneralInfoUI.SetGeneralText(player + " is blocked from moving");
                    }
                    break;
                case EGeneralInfoState2D.EvaluateOpponentMoves:
                    {
                        returnString = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 : 
                                                                          Context.GameConfig.SelectedMatch.Player2;
                    }
                    break;
                case EGeneralInfoState2D.PlayoutProMoves:
                    {
                        Context.GeneralInfoUI.SetGeneralText("THIS WAS THE ORIGINAL PRO MOVE");
                    }
                    break;
            }

            return returnString;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy GeneralInfoUpdate(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
        }
    }
}