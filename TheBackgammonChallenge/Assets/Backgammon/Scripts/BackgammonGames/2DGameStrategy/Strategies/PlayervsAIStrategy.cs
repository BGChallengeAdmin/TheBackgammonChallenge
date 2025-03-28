using static Backgammon.GeneralInfoState2D;

namespace Backgammon
{
    public class PlayervsAIStrategy : IStrategy
    {
        private PlayervsAIStrategyTurnBeginGetMoveHelper turnBeginHelper = new();

        public GameScoreInfo.ContinueGame GameToContinue => Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame;

        public void ConfigureBoardContinueEnter(GameStateContext2DStrategy Context)
        {
            var playerHasCube = Context.GameStats.SaveObject.PlayerDoublingValue > 0 ? true : false;
            var opponentHasCube = Context.GameStats.SaveObject.ProDoublingValue > 0 ? true : false;

            if (playerHasCube)
            {
                Context.DoublingManager.SetCubeOwner(Game2D.PlayingAs.PLAYER_1);
                Context.DoublingManager.SetCubeValue(Context.GameStats.SaveObject.PlayerDoublingValue);
                Context.DoublingManager.SetCubeToMove(false);
                Context.DoublingUI.SetCubeFace(Context.GameStats.SaveObject.PlayerDoublingValue);
            }
            else if (opponentHasCube)
            {
                Context.DoublingManager.SetCubeOwner(Game2D.PlayingAs.PLAYER_2);
                Context.DoublingManager.SetCubeValue(Context.GameStats.SaveObject.ProDoublingValue);
                Context.DoublingManager.SetCubeToMove(true);
                Context.DoublingUI.SetCubeFace(Context.GameStats.SaveObject.ProDoublingValue);
            }
        }

        public void RollForGoesFirstEnter(GameStateContext2DStrategy Context, bool playerGoesFirst, ref int playerDice, ref int opponentDice)
        {
            playerDice = UnityEngine.Random.Range((playerGoesFirst ? 2 : 1), (playerGoesFirst ? 7 : 6));
            opponentDice = 0;

            if (playerGoesFirst)
            {
                opponentDice = UnityEngine.Random.Range(1, playerDice);
            }
            else
            {
                opponentDice = UnityEngine.Random.Range(playerDice + 1, 7);
            }

            Context.GameTurnsList[0].Dice1 = playerDice;
            Context.GameTurnsList[0].Dice2 = opponentDice;
        }

        public string BeforeCommenceEnter(GameStateContext2DStrategy Context)
        {
            return Context.TurnConfig.IfPlayer1GoesFirst ? (Context.GameConfig.SelectedMatch.Player1 + " go first.") :
                                                    (Context.GameConfig.SelectedMatch.Player2 + " goes first.");
        }

        public int[] TurnConfigureEnter(GameStateContext2DStrategy Context, int[] dice) { return dice; }

        public GameStateMachine2DStrategy.EGameState2DStrategy TurnConfigureEnterSelectState(GameStateContext2DStrategy Context)
        {
            if (!Context.TurnConfig.IsPlayersTurn)
            {
                if (Context.TurnConfig.PlayerOfferedDoubles)
                    return GameStateMachine2DStrategy.EGameState2DStrategy.TurnBegin;
                else if (!Context.TurnConfig.CapturedAIDoublingData &&
                        Context.DoublingManager.CubeOwner != Game2D.PlayingAs.PLAYER_1)
                    return GameStateMachine2DStrategy.EGameState2DStrategy.DoublingData;
                else if (!Context.TurnConfig.CapturedAIMoveData)
                {
                    Context.TurnConfig.AIDataRequestBailOutCounter = 0;
                    return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
                }
            }

            // DEBUG TESTING - NO FAST FORWARD IN A.I. GAME
            if (Context.GameConfig.IfFastForwarding)
                return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
            else
                return GameStateMachine2DStrategy.EGameState2DStrategy.TurnBegin;
        }

        public string TurnBeginEnterGetMoveData(GameStateContext2DStrategy Context)
        {
            turnBeginHelper.SetAITurnMoveData(Context);

            // DEBUG - NO FAST FORWARD IN A.I. GAME
            if (Context.GameConfig.IfFastForwarding)
            {
                Context.GameConfig.FastForwardTurnIndex = Context.TurnConfig.IndexTurn + 2;
                turnBeginHelper.SetAIDebugTurnMoveData(Context);
            }

            return Context.GameConfig.AIGameData.GetPlayerMove(0);
        }

        public string[] TurnBeginEnterGetPlayerConcedes(GameStateContext2DStrategy Context)
        {

            if (Context.TurnConfig.IsPlayersTurn && Context.TurnConfig.ConcedeTheGame)
            {
                return new string[] { "Concedes" };
            }
            else return new string[] { string.Empty };
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy TurnBeginEnterSelectState(GameStateContext2DStrategy Context)
        {
            var state = GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;

            if (Context.TurnConfig.IsPlayersTurn)
                state = GameStateMachine2DStrategy.EGameState2DStrategy.AIData;

            return state;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy AIDataEnter(GameStateContext2DStrategy Context)
        {
            if (Context.TurnConfig.IsPlayersTurn && !Context.GameConfig.IfFastForwarding)
                return GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;
            else
                return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
        }

        public AIData AIDataEnterConstructData(GameStateContext2DStrategy Context, AIData _aiDataToSend)
        {
            bool playingAgainstAITurn = !Context.TurnConfig.IsPlayersTurn;
            _aiDataToSend.noOfMoves = (playingAgainstAITurn ? Context.GameConfig.AISettings.PlayerRankHighest.ToString() : "5");
            _aiDataToSend.noise = playingAgainstAITurn ? Context.GameConfig.AISettings.PlayerNoise : 0;
            _aiDataToSend.ply = (playingAgainstAITurn ? Context.GameConfig.AISettings.PlayerPly : 2);
            _aiDataToSend.comment = $"{(Context.TurnConfig.IsPlayersTurn ? "PLAYER" : "A.I.")}";

            return _aiDataToSend;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy AIDataUpdate(GameStateContext2DStrategy Context)
        {
            if (!Context.TurnConfig.AIDataAvailable)
            {
                Context.TurnConfig.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.TurnConfig.AIDataAvailable)
                {
                    // RESPONSE DATA
                    Context.TurnConfig.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                    Context.TurnConfig.AITopRankedMove = Context.AIDataHandler.AIResponse();
                    Context.TurnConfig.CapturedAIMoveData = true;

                    return GameStateMachine2DStrategy.EGameState2DStrategy.TurnBegin;
                }
                else
                    return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
            }
            else
                return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy DoublingDataUpdate(GameStateContext2DStrategy Context, 
                                                               GameStateMachine2DStrategy.EGameState2DStrategy passThroughState)
        {
            if (!Context.TurnConfig.IsPlayersTurn && !Context.TurnConfig.CapturedAIDoublingData)
            {
                Context.TurnConfig.CapturedAIDoublingData = true;

                if (Context.TurnConfig.AIDoublingData.cubeDecision.owner == "double" ||
                    Context.TurnConfig.AIDoublingData.cubeDecision.owner == "redouble")
                    return GameStateMachine2DStrategy.EGameState2DStrategy.TurnBegin;
                else
                    return GameStateMachine2DStrategy.EGameState2DStrategy.TurnConfigure;
            }
            else
                return passThroughState;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy DoublingDataBailOut(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.AIData;
        }

        public void DoublingOffersEnter(GameStateContext2DStrategy Context)
        {
            var player = Context.GameConfig.SelectedMatch.Player2;
            var opponent = Context.GameConfig.SelectedMatch.Player1;

            Context.DoublingUI.SetOffersDoubleText(player, opponent);
        }

        public void DoublingOffersUpdate(GameStateContext2DStrategy Context)
        {
            if (Context.DoublingUI.IfClickedYes)
                Context.TurnConfig.AIPlayerAcceptsDoubles = true;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy DoublingInGameState(GameStateContext2DStrategy Context)
        {
            // PLAYER OFFERED A.I. DOUBLE
            if (Context.TurnConfig.IsPlayersTurn && Context.DoublingUI.IfClickedYes)
                return GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
            else
                return GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;
        }

        public void DiceRollsDisplayUserHint(GameStateContext2DStrategy Context, int dice1, int dice2)
        {
            Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, dice1, dice2, true);
        }

        public bool SelectPointFromEntry(GameStateContext2DStrategy Context, bool bar = false, bool bearingOff = false, bool normal = false)
        {
            var valid = false;

            var dice1_original = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
            var dice2_original = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
            var opponentColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;

            if (bar)
            {
                // A.I. GAME - SAFEGUARD AGAINST MULTIPLE COUNTERS ON BAR
                // ENSURE THERE IS A VALID MOVE

                if (Main.Instance.IfPlayerVsAI)
                {
                    var step1 = 25 - dice1_original;
                    var step2 = 25 - dice2_original;

                    var valid1 = Context.DiceManager.Dice1Played ? false : Context.PointsManager.TestIfCanMoveToPoint(step1, opponentColour);
                    var valid2 = Context.DiceManager.Dice2Played ? false : Context.PointsManager.TestIfCanMoveToPoint(step2, opponentColour);

                    valid = valid1 || valid2;
                }
            }

            if (bearingOff)
            {
                if (Context.HomeManager.PlayerHome.Counters < 15) valid = true;
            }

            if (normal)
            {
                // A.I. GAME - SAFEGUARD AGAINST INVALID MULTIPLE MOVES
                
                for (int p = 1; p < 25; p++)
                {
                    var point = Context.PointsManager.GetPlayerPointByID(p);
                    if (point.Owner != Game2D.PlayingAs.PLAYER_1 || point.Counters == 0) continue;

                    var step1 = p - dice1_original;
                    var step2 = p - dice2_original;

                    var valid1 = !Context.DiceManager.DoubleWasRolled && Context.DiceManager.Dice1Played ?
                                    false : Context.PointsManager.TestIfCanMoveToPoint(step1, opponentColour);
                    var valid2 = Context.DiceManager.Dice2Played ? false : Context.PointsManager.TestIfCanMoveToPoint(step2, opponentColour);

                    valid = valid1 || valid2;

                    if (valid) break;
                }
            }

            return valid;
        }

        public void EvaluatePlayerMoveEntry(GameStateContext2DStrategy Context)
        {
            // A.I. GAME - SET THE MOVE DATA TO THE PLAYER BEFORE ANALYSIS
            if (Context.TurnConfig.IsPlayersTurn)
            {
                Context.TurnConfig.RecordedMovesInfo = Context.TurnConfig.PlayerMovesInfo;
            }
        }

        public void PlayoutProMovesEntry(GameStateContext2DStrategy Context)
        {
            // A.I. GAME - PLAYER CAN USE LESS THAN MOVES AVAILABLE
            // NO HISTORIC DATA TO FALL BACK ON IN MOVE_COUNTERS

            Context.TurnConfig.CountersToMoveIndex = 0;

            foreach (var m in Context.TurnConfig.PlayerMovesInfo)
            {
                if (m.pointFrom == 0) break;
                else Context.TurnConfig.CountersToMoveIndex += 1;
            }
        }

        public void GameWonUpdateAIScore(GameStateContext2DStrategy Context)
        {
            Context.GameConfig.AIGameData.GameConstructor(Context.GameConfig.AIGameData.Player1PointsAtEnd,
                                                Context.GameConfig.AIGameData.Player2PointsAtEnd,
                                                Context.GameConfig.AIGameData.Moves,
                                                Context.GameConfig.AIGameData.Player1PointsAtEnd,
                                                Context.GameConfig.AIGameData.Player2PointsAtEnd);
        }

        public string GameWonUpdateScoreText(GameStateContext2DStrategy Context, string gameWonByText, string gameOrMatch, string winnersName)
        {
            gameWonByText += winnersName + " won the " + gameOrMatch;
            gameWonByText += Context.GameConfig.GameWonByGammon ? "\nBy Gammon!" : string.Empty;
            gameWonByText += Context.GameConfig.GameWonByBackGammon ? "\nBy BackGammon!!" : string.Empty;
            return gameWonByText;
        }

        public string GameWonUpdateStatsText(GameStateContext2DStrategy Context, string playerInfoText, bool setTextToStats = false)
        {
            if (setTextToStats)
            {
                var opponent = !Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;
                var points = (Context.GameStats.OpponentTopRankedThisGame * 3) + (Context.GameStats.OpponentSecondRankedThisGame * 2);
                var possible = (Context.GameStats.TotalValidOpponentMovesThisGame * 3);

                var opponentInfoText = $"{opponent} had \n\n{Context.GameStats.OpponentTopRankedThisGame} Rank #1 for {(Context.GameStats.OpponentTopRankedThisGame * 3)} Pts.";
                opponentInfoText += $"\n{Context.GameStats.OpponentSecondRankedThisGame} Rank#2 for {(Context.GameStats.OpponentSecondRankedThisGame * 2)} Pts.";
                opponentInfoText += $"\nFor a total of {points}/{possible} possible Pts.";

                Context.GameWonUI.SetGameWonLeftInfoText(opponentInfoText);
            }

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

                        // GRAMMAR - PLAYERS NAME IS "YOU" IN A.I. GAME
                        if (Context.TurnConfig.IsPlayersTurn)
                            Context.GeneralInfoUI.SetGeneralText(player + " are blocked from moving");
                    }
                    break;
                case EGeneralInfoState2D.EvaluateOpponentMoves:
                    {
                        returnString = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 :
                                                                          Context.GameConfig.SelectedMatch.Player2;
                        returnString = Context.GameConfig.SelectedMatch.Player2;
                    }
                    break;
                case EGeneralInfoState2D.PlayoutProMoves:
                    {
                        Context.GeneralInfoUI.SetGeneralText("REPLAYING YOUR MOVE");
                    }
                    break;
            }

            return returnString;
        }

        public GameStateMachine2DStrategy.EGameState2DStrategy GeneralInfoUpdate(GameStateContext2DStrategy Context)
        {
            return GameStateMachine2DStrategy.EGameState2DStrategy.AnalysisOrContinue;
        }
    }
}