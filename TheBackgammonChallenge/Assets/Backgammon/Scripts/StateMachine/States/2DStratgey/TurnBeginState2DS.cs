using System;

namespace Backgammon
{
    public class TurnBeginState2DS : GameState2DStrategy
    {
        public TurnBeginState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: TURN BEGIN -> {(Context.TurnConfig.IsPlayersTurn ? "PLAYER" : "PRO")}");
            
            // COPY THE BOARD STATE
            Context.TurnConfig.TurnBeginBoardState.SetStateFromContext(Context);
            Context.GameTurnsList[Context.TurnConfig.IndexTurn].TurnBeginBoardState.SetStateFromContext(Context);

            // SET VALUES FOR CONTINUE - SAVE BOARD STATE
            #region SAVE_BOARD_FOR_CONTINUE
            if (!Context.GameConfig.IfFastForwarding)
            {
                SaveForContinue();
            }
            #endregion

            #region SAVE_PLAYER_SCORE
            if (!Context.GameConfig.IfFastForwarding)
            {
                UpdatePlayerScoreData();
                UpdatePlayerStatsGraphData();
            }
            #endregion

            var turnData = Context.Strategy.TurnBeginEnterGetMoveData(Context);

            #region TestWins
            if (turnData.Contains("Wins"))
            {
                UnityEngine.Debug.Log($"PLAYER WINS");
                Context.TurnConfig.GameWon = true;
                Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.GameWon;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                goto BailOut;
            }
            else if (turnData.Contains("Loses"))
            {
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                goto BailOut;
            }
            #endregion

            string[] moveParts = turnData.Split(':');

            #region TestExceptions

            // TODO / FIXME : TEST FOR POOR NOTATION (53:UnableToMove) != (61:CannotMove)
            // IN SOME CASES THE "CANNOT MOVE" NOTATION HAS BEEN USED TO COVER BOTH SCENARIOS
            // THIS IS / WILL BE UPDATED IN THE AutoBuilder
            // SHOULD BE CHECKED BUT IS HIGHLY UNLIKELY

            if (TestIfBlockedFromMovingFromBar(Context.TurnConfig.IsPlayersTurn))
            {
                moveParts[0] = string.Empty;
            }

            if (moveParts[0] == string.Empty)
            {
                // NOTE: NOTATION ALLOWS ":" TO SHOW A SKIPPED TURN
                Context.GameConfig.Debug_debugObject.DebugMessage($"EMPTY TURN!!! SKIP A TURN OR WIN -> NO DICE WERE ROLLED");

                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;

                if (Context.TurnConfig.PlayerIsBlockedFromMovingFromBar)
                {
                    Context.TurnConfig.PlayerIsUnableToMove = true;
                    Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerUnableToMove;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                }

                moveParts = Context.Strategy.TurnBeginEnterGetPlayerConcedes(Context);
                
                if (moveParts[0] == "Concedes") goto Concedes;

                goto BailOut;
            }

            Concedes:;

            // TEST MOVES / DOUBLES
            if (moveParts.Length == 1)
            {
                var result = moveParts[0];

                if (result == "Doubles")
                {
                    Context.GameConfig.Debug_debugObject.DebugMessage($"{(Context.TurnConfig.IsPlayersTurn ? "PLAYER" : "OPPONENT")} DOUBLES");

                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.DoublingData;
                    goto BailOut;
                }
                else if (result == "Takes")
                {
                    Context.TurnConfig.DoublingTakesOrDrops = true;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.DoublingTakesOrDrops;
                    Context.GameConfig.Debug_debugObject.DebugMessage($"{(Context.TurnConfig.IsPlayersTurn ? "PLAYER" : "OPPONENT")} TAKES");
                    goto BailOut;
                }
                else if (result == "Drops")
                {
                    Context.TurnConfig.DoublingTakesOrDrops = false;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.DoublingTakesOrDrops;
                    Context.GameConfig.Debug_debugObject.DebugMessage($"{(Context.TurnConfig.IsPlayersTurn ? "PLAYER" : "OPPONENT")} DROPS");
                    Context.GameConfig.AIGameWasWon = true;
                    goto BailOut;
                }
                else if (result == "Concedes")
                {
                    Context.TurnConfig.PlayerConcedes = true;
                    Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerConcedes;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                    Context.GameConfig.Debug_debugObject.DebugMessage($"PLAYER CONCEDES");
                    Context.GameConfig.AIGameWasWon = true;
                    goto BailOut;
                }

                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                goto BailOut;
            }
            #endregion

            #region UpdateUI
            // UPDATE U.I.
            if (ActiveState != GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd)
            {
                Context.GameScreenUI.SetPlayerScore(Context.GameStats.PlayerScoreThisGame);
                Context.GameScreenUI.SetProPlayerScore(Context.GameStats.ProScoreThisGame);
                Context.GameScreenUI.SetOpponentScore(Context.GameStats.OpponentScoreThisGame);

                Context.GameScreenUI.SetPlayerTopMatched(Context.GameStats.PlayerTopRankedThisGame);
                Context.GameScreenUI.SetProPlayerTopMatched(Context.GameStats.ProTopRankedThisGame);
                Context.GameScreenUI.SetOpponentTopMatched(Context.GameStats.OpponentTopRankedThisGame);

                Context.GameScreenUI.SetGameTurn(Context.TurnConfig.IndexTurn + (Context.TurnConfig.IfPlayer1GoesFirst ? 1 : 0));

                if (Context.GameConfig.FastForwardTurnIndex <= Context.TurnConfig.IndexTurn)
                    Context.GameConfig.IfFastForwarding = false;
            }
            #endregion

            #region CapturePlayerMove
            // CAPTURE DICE ROLLS
            string dice = moveParts[0];
            var dice1 = int.Parse(dice[0].ToString());
            var dice2 = int.Parse(dice[1].ToString());

            Context.TurnConfig.Dice1 = dice1;
            Context.TurnConfig.Dice2 = dice2;
            Context.DiceManager.SetDiceValues(dice1, dice2);

            //UnityEngine.Debug.Log($"DICE {dice[0]} {dice[1]} -> {Context.Dice1} {Context.Dice2}");

            // RESET AND CAPTURE MOVE DATA
            for (int playerM = 0; playerM < 4; playerM++) { Context.TurnConfig.PlayerMovesInfo[playerM] = Context.TurnConfig.PlayerMovesInfo[playerM].Reset(); }
            for (int proM = 0; proM < 4; proM++) { Context.TurnConfig.RecordedMovesInfo[proM] = Context.TurnConfig.RecordedMovesInfo[proM].Reset(); }
            //for (int aiM = 0; aiM < 4; aiM++) { Context.TopRankedMovesInfo[aiM] = Context.TopRankedMovesInfo[aiM].Reset(); }

            // ONLY RESET OPPONENT ON THEIR TURN - MAINTAIN FOR PLAYER ANLYSIS
            if (!Context.TurnConfig.IsPlayersTurn) for (int oppM = 0; oppM < 4; oppM++)
                    Context.TurnConfig.OpponentMovesInfo[oppM] = Context.TurnConfig.OpponentMovesInfo[oppM].Reset();

            string movesData = moveParts[1];
            string[] moves = movesData.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Context.TurnConfig.TotalMovesThisTurn = moves.Length;
            Context.TurnConfig.MovesAvailable = Context.TurnConfig.TotalMovesThisTurn;

            // TEST WHEN THERE IS NO MOVE DATA
            if (moves.Length == 0)
            {
                UnityEngine.Debug.Log($"THERE IS NO MOVE DATA - PLAYER WAS BLOCKED FROM MOVING");

                Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerUnableToMove;
                Context.TurnConfig.PlayerIsUnableToMove = true;

                if (Context.GameConfig.IfFastForwarding)
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                else
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RollDice;

                goto BailOut;
            }
            else
            {
                Context.TurnConfig.IsPlayersMakingMoves = Context.TurnConfig.IsPlayersTurn ? true : false;
                Context.TurnConfig.PlayerMoveIndex = 0;
                Context.TurnConfig.CountersToMoveIndex = 0;

                if (!Context.TurnConfig.IsPlayersTurn) Context.TurnConfig.CountersToMoveIndex = Context.TurnConfig.TotalMovesThisTurn;

                if (Context.GameConfig.IfFastForwarding)
                {
                    Context.TurnConfig.IsPlayersMakingMoves = false;
                    Context.TurnConfig.CountersToMoveIndex = Context.TurnConfig.TotalMovesThisTurn;
                }
            }

            // NOTE: MOVES WILL BE IN THE FORMAT X/Y* (* IF THERE IS A BLOT)
            for (int move = 0; move < moves.Length; move++)
            {
                var pointFrom = moves[move].Split('/')[0];
                var pointTo = moves[move].Split('/')[1];
                var ifBlot = pointTo[pointTo.Length - 1] == '*' ? true : false;

                if (ifBlot) { pointTo = pointTo.TrimEnd('*'); }

                Context.TurnConfig.RecordedMovesInfo[move].pointFrom = pointFrom == "bar" ? 25 : int.Parse(pointFrom);
                Context.TurnConfig.RecordedMovesInfo[move].pointTo = pointTo == "off" ? 0 : int.Parse(pointTo);
                Context.TurnConfig.RecordedMovesInfo[move].ifBlot = ifBlot;

                // CAPTURE AND MAINTAIN OPPONENT DATA FOR PLAYER TURN ANALYSIS
                if (!Context.TurnConfig.IsPlayersTurn)
                {
                    Context.TurnConfig.OpponentMovesInfo[move].pointFrom = pointFrom == "bar" ? 25 : int.Parse(pointFrom);
                    Context.TurnConfig.OpponentMovesInfo[move].pointTo = pointTo == "off" ? 0 : int.Parse(pointTo);
                    Context.TurnConfig.OpponentMovesInfo[move].ifBlot = ifBlot;
                }
            }

            #endregion

            // CAPTURE A.I. DATA FOR THE TURN
            ActiveState = Context.Strategy.TurnBeginEnterSelectState(Context);

            if (Context.GameConfig.IfFastForwarding)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.MoveCounters;

            BailOut:;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        // ---------------------------------- AI METHODS - MOVED TO AI STRATEGY --------------------------------------

        //private void SetAITurnMoveData()
        //{
        //    // RESTORE DEFAULT MOVES
        //    Context.GameConfig.AIGameData.Moves[0] = ":";
        //    Context.GameConfig.AIGameData.Moves[1] = "For:Doubles";
        //    Context.GameConfig.AIGameData.Moves[2] = "For:Takes";

        //    if (TestIfBlockedFromMovingFromBar(Context.TurnConfig.IsPlayersTurn)) return;

        //    if (Context.TurnConfig.IsPlayersTurn)
        //    {
        //        var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
        //        var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
        //        var dice = $"{dice1.ToString()}{dice2.ToString()}:";
        //        var move = " 0/0 0/0" + ((dice1 == dice2) ? " 0/0 0/0" : string.Empty);

        //        Context.GameConfig.AIGameData.Moves[0] = dice + (TestIfValidMoves() ? move : string.Empty);

        //        if (Context.TurnConfig.AIOfferedDoubles)
        //        {
        //            Debug.Log($"AI OFFERED DOUBLE -> PLAYER {Context.TurnConfig.AIPlayerAcceptsDoubles}");

        //            Context.TurnConfig.AIOfferedDoubles = false;

        //            if (Context.TurnConfig.AIPlayerAcceptsDoubles) Context.GameConfig.AIGameData.Moves[2] = "Takes";
        //            else Context.GameConfig.AIGameData.Moves[2] = "Drops";
        //        }
        //        else if (Context.HomeManager.OpponentHome.Counters == 15)
        //        {
        //            Context.GameConfig.AIGameData.Moves[2] = "Loses";
        //        }
        //        else if (Context.HomeManager.PlayerHome.Counters == 15 || Context.GameConfig.AIGameWasWon)
        //        {
        //            SetAIGameWonForPoints(true);
        //        }
        //        else if (Context.TurnConfig.ConcedeTheGame)
        //        {
        //            Context.GameConfig.AIGameData.Moves[0] = "Concedes";
        //        }
        //    }
        //    else if (!Context.TurnConfig.IsPlayersTurn)
        //    {
        //        // IF NO MOVE DATA THEN DOUBLING INTERACTION
        //        if (Context.HomeManager.PlayerHome.Counters == 15)
        //        {
        //            Context.GameConfig.AIGameData.Moves[0] = "Loses";
        //        }
        //        else if (Context.HomeManager.OpponentHome.Counters == 15 || Context.GameConfig.AIGameWasWon)
        //        {
        //            SetAIGameWonForPoints(false);
        //        }
        //        else if (Context.TurnConfig.CapturedAIMoveData)
        //        {
        //            // BUILD THE MOVE AND SET TO THE TURN
        //            var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
        //            var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
        //            var dice = $"{dice1.ToString()}{dice2.ToString()}:";
        //            var move = string.Empty;

        //            // WILL BE NULL IF "NO MOVE POSSIBLE" -> CAUSE THE A.I. TO SKIP A TURN
        //            if (Context.TurnConfig.AIRankedMoves[0] is not null)
        //            {
        //                var aiRank = GetAIRank(Context.GameConfig.AISettings.PlayerPreset);
        //                var rank = aiRank <= Context.TurnConfig.AIRankedMoves.Length ? aiRank : Context.TurnConfig.AIRankedMoves.Length;

        //                if (Context.TurnConfig.IndexTurn < 2) rank = 1;

        //                var aiRankedMove = Context.TurnConfig.AIRankedMoves[rank - 1].movePart;
        //                var blot = false;
        //                var blottedToCounter = 0;

        //                foreach (FromTo fromTo in aiRankedMove)
        //                {
        //                    blot = TestIfBlot(fromTo.to);

        //                    for (int b = 0; b < blottedToCounter; b++)
        //                    {
        //                        FromTo ftb = aiRankedMove[b];
        //                        if (fromTo.to == ftb.to)
        //                            blot = false;
        //                    }

        //                    blottedToCounter++;

        //                    move += " " + fromTo.from + "/" + fromTo.to + (blot ? "*" : string.Empty);
        //                }
        //            }
        //            else
        //            {
        //                // NOTE: REQUIRES RESET
        //                Context.TurnConfig.OpponentMatchedRankThisTurn = 0;
        //            }

        //            Context.GameConfig.AIGameData.Moves[Context.TurnConfig.IndexTurn] = dice + move;
        //        }
        //        else if (Context.TurnConfig.PlayerOfferedDoubles)
        //        {
        //            if (Context.TurnConfig.AIDoublingData.cubeDecision.other == "accept")
        //                Context.GameConfig.AIGameData.Moves[2] = "Takes";
        //            else if (Context.TurnConfig.AIDoublingData.cubeDecision.other == "resign")
        //                Context.GameConfig.AIGameData.Moves[2] = "Drops";
        //        }
        //        else if (Context.TurnConfig.CapturedAIDoublingData)
        //        {
        //            if (Context.TurnConfig.AIDoublingData.cubeDecision.owner == "double" ||
        //                Context.TurnConfig.AIDoublingData.cubeDecision.owner == "redouble")
        //            {
        //                Context.TurnConfig.AIOfferedDoubles = true;
        //                Context.GameConfig.AIGameData.Moves[1] = "Doubles";
        //            }
        //        }
        //    }
        //}

        //private void SetAIDebugTurnMoveData()
        //{
        //    // RESTORE DEFAULT MOVES
        //    Context.GameConfig.AIGameData.Moves[0] = ":";
        //    Context.GameConfig.AIGameData.Moves[1] = "For:Doubles";
        //    Context.GameConfig.AIGameData.Moves[2] = "For:Takes";

        //    // BUILD THE MOVE AND SET TO THE TURN
        //    var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
        //    var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
        //    var dice = $"{dice1.ToString()}{dice2.ToString()}:";
        //    var move = string.Empty;

        //    // WILL BE NULL IF "NO MOVE POSSIBLE" -> CAUSE THE A.I. TO SKIP A TURN
        //    if (Context.TurnConfig.AIRankedMoves[0] is not null)
        //    {
        //        var aiRankedMove = Context.TurnConfig.AIRankedMoves[0].movePart;
        //        var blot = false;
        //        var blottedToCounter = 0;

        //        foreach (FromTo fromTo in aiRankedMove)
        //        {
        //            blot = TestIfBlot(fromTo.to);

        //            for (int b = 0; b < blottedToCounter; b++)
        //            {
        //                FromTo ftb = aiRankedMove[b];
        //                if (fromTo.to == ftb.to)
        //                    blot = false;
        //            }

        //            blottedToCounter++;

        //            move += " " + fromTo.from + "/" + fromTo.to + (blot ? "*" : string.Empty);
        //        }
        //    }
        //    else
        //    {
        //        // TEST IF GAME WAS WON
        //        if (Context.HomeManager.PlayerHome.Counters == 15 ||
        //            Context.HomeManager.OpponentHome.Counters == 15)
        //        {
        //            UnityEngine.Debug.Log($"*** GAME HAS BEEN WON POWER MOVE***");
        //            Context.Game.ConfigureContextAndInit();
        //        }
        //    }

        //    Context.GameConfig.AIGameData.Moves[0] = dice + move;
        //}

        //private bool TestIfValidMoves(bool ifPlayerTurn = true)
        //{
        //    var valid = false;

        //    var opponentColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;
        //    var step1 = 0;
        //    var step2 = 0;

        //    var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
        //    var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

        //    // COUNTER ON BAR
        //    if (Context.BarManager.PlayerBar.Counters > 0)
        //    {
        //        step1 = 25 - dice1;
        //        step2 = 25 - dice2;

        //        valid = (TestIfCanMoveToPoint(step1, opponentColour) ||
        //                (TestIfCanMoveToPoint(step2, opponentColour)) ? true : false);
        //    }
        //    // POINTS
        //    else
        //    {
        //        for (int p = 24; p > 0; p--)
        //        {
        //            if (Context.PointsManager.GetPlayerPointByID(p).Owner != Context.GameConfig.PlayingAs) continue;

        //            step1 = p - dice1;
        //            step2 = p - dice2;

        //            var valid1 = TestIfCanMoveToPoint(step1, opponentColour);
        //            var valid2 = TestIfCanMoveToPoint(step2, opponentColour);

        //            if (valid1 || valid2)
        //            {
        //                valid = true;
        //                break;
        //            }
        //        }
        //    }

        //    return valid;
        //}

        //private bool TestIfCanMoveToPoint(int pointToIndex, Game2D.PlayerColour opponentColour)
        //{
        //    var playerColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
        //    Context.TurnConfig.IfBearingOff = Context.PointsManager.TestIfBearingOff(playerColour);

        //    if (!Context.TurnConfig.IfBearingOff && pointToIndex <= 0) return false;

        //    if (pointToIndex > 0)
        //    {
        //        var pointTo = Context.PointsManager.GetPlayerPointByID(pointToIndex);
        //        if (pointTo.Colour == opponentColour && pointTo.Counters > 1) return false;
        //    }

        //    return true;
        //}

        //private bool TestIfBlot(string pointTo)
        //{
        //    if (pointTo == "off") return false;

        //    var valid = false;
        //    var to = int.Parse(pointTo);

        //    if (0 < to && to < 25)
        //    {
        //        var playerOn = Context.TurnConfig.IsPlayersTurn ? Context.GameConfig.OpponentAs : Context.GameConfig.PlayingAs;
        //        var point = Context.TurnConfig.IsPlayersTurn ? Context.PointsManager.GetPlayerPointByID(to) :
        //                                            Context.PointsManager.GetOpponentPointByID(to);
        //        if (point.Owner == playerOn && point.Counters == 1) valid = true;
        //    }

        //    return valid;
        //}

        //private int GetAIRank(string difficulty)
        //{
        //    switch (difficulty)
        //    {
        //        case "PERFECT": return 1;
        //        case "HARD":
        //            {
        //                var h = UnityEngine.Random.Range(0, 101);
        //                if (h < 25) return 2;
        //                else return 1;
        //            }
        //        case "MEDIUM":
        //            {
        //                var m = UnityEngine.Random.Range(0, 101);
        //                if (m < 2) return 4;
        //                else if (m < 10) return 3;
        //                else if (m < 40) return 2;
        //                else return 1;
        //            }
        //        case "EASY":
        //            {
        //                var e = UnityEngine.Random.Range(0, 101);
        //                if (e < 10) return 5;
        //                else if (e < 35) return 4;
        //                else if (e < 65) return 3;
        //                else if (e < 85) return 2;
        //                else return 1;
        //            }
        //        default: return 1;
        //    }
        //}

        //private void SetAIGameWonForPoints(bool playerWon)
        //{
        //    var multiplier = TestWonByGammonBackgammon(playerWon);
        //    var pointsWon = Context.DoublingManager.CubeValue * multiplier;

        //    Context.GameConfig.AIGameData.Moves[0] = "Wins " + pointsWon + " point";

        //    Context.GameConfig.AIGameData.GameConstructor(Context.GameConfig.AIGameData.Player1PointsAtStart,
        //                                       Context.GameConfig.AIGameData.Player2PointsAtStart,
        //                                       Context.GameConfig.AIGameData.Moves,
        //                                       Context.GameConfig.AIGameData.Player1PointsAtStart + (playerWon ? pointsWon : 0),
        //                                       Context.GameConfig.AIGameData.Player2PointsAtStart + (playerWon ? 0 : pointsWon));
        //}

        //private int TestWonByGammonBackgammon(bool playerWon)
        //{
        //    var multiplier = 1;

        //    if ((playerWon ? Context.BarManager.OpponentBar.Counters :
        //        Context.BarManager.PlayerBar.Counters) > 0)
        //    {
        //        Context.GameConfig.GameWonByBackGammon = true;
        //        return multiplier = 3;
        //    }

        //    var from = playerWon ? 6 : 24;
        //    var to = playerWon ? 0 : 18;

        //    for (var p = from; p > to; p--)
        //    {
        //        var point = Context.PointsManager.GetPlayerPointByID(p);

        //        if (point.Owner == (playerWon ? Context.GameConfig.OpponentAs : Context.GameConfig.PlayingAs))
        //        {
        //            Context.GameConfig.GameWonByBackGammon = true;
        //            return multiplier = 3;
        //        }
        //    }

        //    if ((playerWon ? Context.HomeManager.OpponentHome.Counters :
        //        Context.HomeManager.PlayerHome.Counters) == 0)
        //    {
        //        Context.GameConfig.GameWonByGammon = true;
        //        return multiplier = 2;
        //    }

        //    return multiplier;
        //}

        // -------------------------------------- HELPER METHODS --------------------------------------

        private bool TestIfBlockedFromMovingFromBar(bool playerTurn)
        {
            Context.TurnConfig.PlayerIsBlockedFromMovingFromBar = false;

            if (playerTurn)
            {
                if (Context.BarManager.PlayerBar.Counters == 0) return false;

                for (int p = 24; p > 18; p--)
                {
                    var point = Context.PointsManager.GetPlayerPointByID(p);

                    if (point.Owner == Context.GameConfig.PlayingAs) return false;
                    else if (point.Counters < 2) return false;
                }
            }
            else
            {
                if (Context.BarManager.OpponentBar.Counters == 0) return false;

                for (int p = 6; p > 0; p--)
                {
                    var point = Context.PointsManager.GetPlayerPointByID(p);

                    if (point.Owner != Context.GameConfig.PlayingAs) return false;
                    else if (point.Counters < 2) return false;
                }

                Context.TurnConfig.OpponentMatchedRankThisTurn = 0;
            }

            Context.TurnConfig.PlayerIsBlockedFromMovingFromBar = true;
            return true;
        }

        private void SaveForContinue()
        {
            var saveObject = Main.Instance.IfPlayerVsAI ? Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame :
                                                          Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame;

            saveObject.IfMatchToContinue = true;
            saveObject.BoardState = Context.TurnConfig.TurnBeginBoardState.GetBoardStateString();

            // MATCH CONFIG

            saveObject.MatchTitle = Context.GameConfig.SelectedMatch.Title;
            saveObject.MatchID = Context.GameConfig.SelectedMatch.ID;
            saveObject.GameName = Context.GameConfig.SelectedGame.name;
            saveObject.PlayingAs = Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? 1 : 2;

            saveObject.MatchIndex = (Context.GameConfig.SelectedMatch.ID != "DEMO") ? int.Parse(Context.GameConfig.SelectedMatch.ID) : 0;
            saveObject.GameIndex = Context.GameConfig.IndexGame;
            saveObject.TurnIndex = Context.TurnConfig.IndexTurn;

            // MATCH STATS

            saveObject.MovesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
            saveObject.MovesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
            saveObject.OpponentMovesMade = Context.GameStats.TotalValidOpponentMovesThisGame;

            saveObject.AITopMatched = Context.GameStats.PlayerTopRankedThisGame;
            saveObject.ProMovesMatched = Context.GameStats.ProTopRankedThisGame;
            saveObject.OpponentMovesMatched = Context.GameStats.OpponentTopRankedThisGame;

            saveObject.PlayerSecondMatched = Context.GameStats.PlayerSecondRankedThisGame;
            saveObject.ProSecondMatched = Context.GameStats.ProSecondRankedThisGame;
            saveObject.OpponentSecondMatched = Context.GameStats.OpponentSecondRankedThisGame;

            saveObject.PlayerTotalScore = Context.GameStats.PlayerScoreThisGame;
            saveObject.ProTotalScore = Context.GameStats.ProScoreThisGame;
            saveObject.OpponentTotalScore = Context.GameStats.OpponentScoreThisGame;

            saveObject.IsPlayerTurn = Context.TurnConfig.IsPlayersTurn;

            // NOTE: ONLY USED FOR A.I. GAME
            var playerHasCube = Context.DoublingManager.CubeOwner == Game2D.PlayingAs.PLAYER_1;
            saveObject.PlayerDoublingValue = playerHasCube ? Context.DoublingManager.CubeValueIndex : 0;

            var opponentHasCube = Context.DoublingManager.CubeOwner == Game2D.PlayingAs.PLAYER_2;
            saveObject.ProDoublingValue = opponentHasCube ? Context.DoublingManager.CubeValueIndex : 0;

            if (Main.Instance.IfPlayerVsAI)
                Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame = saveObject;
            else
                Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame = saveObject;

            Main.Instance.PlayerPrefsObj.SaveAppData();
        }

        private void UpdatePlayerScoreData()
        {
            Backgammon_Asset.GameData game = Context.GameConfig.SelectedGame;
            string gameName = game.name;
            string matchKey = Context.GameConfig.SelectedMatch.Title + " " + Context.GameConfig.SelectedMatch.ID;

            PlayerScoreData scoreData = Main.Instance.PlayerScoresObj.GetPlayerScoreData(matchKey);
            MatchScoreData currentMatch = scoreData.matchScores.ID != string.Empty ? scoreData.matchScores : new MatchScoreData();
            GameScoreData currentGame = currentMatch.gameScoresDict.ContainsKey(gameName) ? currentMatch.gameScoresDict[gameName] : new GameScoreData();

            currentGame.name = gameName;

            if (Context.GameStats.TotalValidPlayerMovesThisGame > 0)
            {
                // CUMULATIVE SCORES FOR THIS GAME
                currentGame.fullCumulativeMovesMade += 1;
                currentGame.fullCumulativeMovesMatched += (Context.TurnConfig.PlayerMatchedProMove) ? 1 : 0;
                currentGame.fullCumulativeTopMatchedMoves += (Context.TurnConfig.PlayerMatchedRankThisTurn == 1) ? 1 : 0;
                currentGame.fullCumulativeTopMatchedMoves += (Context.TurnConfig.ProMatchedRankThisTurn == 1) ? 1 : 0;
            }

            //if (currentGame.player1PossibleCounterMoves < player1PossibleMovesThisGame)
            //    currentGame.player1PossibleCounterMoves = player1PossibleMovesThisGame;
            //if (currentGame.player2PossibleCounterMoves < player2PossibleMovesThisGame)
            //    currentGame.player2PossibleCounterMoves = player2PossibleMovesThisGame;
            //if (currentGame.totalCounterMovesSeenPlayed < (player1PossibleMovesThisGame + player2PossibleMovesThisGame))
            //    currentGame.totalCounterMovesSeenPlayed = (player1PossibleMovesThisGame + player2PossibleMovesThisGame);

            // SEPARATED SCORES THIS GAME / BEST
            if (Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1)
            {
                currentGame.player1IndexTurn = Context.TurnConfig.IndexTurn;
                currentGame.player1MovesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
                currentGame.player1MovesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
                currentGame.player1TopMatched = Context.GameStats.PlayerTopRankedThisGame;
                currentGame.player1ProTopMatched = Context.GameStats.ProTopRankedThisGame;

                if (currentGame.player1BestIndexTurn <= Context.TurnConfig.IndexTurn)
                    currentGame.player1BestIndexTurn = Context.TurnConfig.IndexTurn;
                if (currentGame.player1BestMovesMade <= Context.GameStats.TotalValidPlayerMovesThisGame)
                    currentGame.player1BestMovesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
                if (currentGame.player1BestMovesMatched <= Context.GameStats.TotalValidPlayerMatchesThisGame)
                    currentGame.player1BestMovesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
                if (currentGame.player1BestTopMatched <= Context.GameStats.PlayerTopRankedThisGame)
                    currentGame.player1BestTopMatched = Context.GameStats.PlayerTopRankedThisGame;
                if (currentGame.player1BestProTopMatched <= Context.GameStats.ProTopRankedThisGame)
                    currentGame.player1BestProTopMatched = Context.GameStats.ProTopRankedThisGame;
            }
            else if (Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_2)
            {
                currentGame.player2IndexTurn = Context.TurnConfig.IndexTurn;
                currentGame.player2MovesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
                currentGame.player2MovesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
                currentGame.player2TopMatched = Context.GameStats.PlayerTopRankedThisGame;
                currentGame.player2ProTopMatched = Context.GameStats.ProTopRankedThisGame;

                if (currentGame.player2BestIndexTurn <= Context.TurnConfig.IndexTurn)
                    currentGame.player2BestIndexTurn = Context.TurnConfig.IndexTurn;
                if (currentGame.player2BestMovesMade <= Context.GameStats.TotalValidPlayerMovesThisGame)
                    currentGame.player2BestMovesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
                if (currentGame.player2BestMovesMatched <= Context.GameStats.TotalValidPlayerMatchesThisGame)
                    currentGame.player2BestMovesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
                if (currentGame.player2BestTopMatched <= Context.GameStats.PlayerTopRankedThisGame)
                    currentGame.player2BestTopMatched = Context.GameStats.PlayerTopRankedThisGame;
                if (currentGame.player2BestProTopMatched <= Context.GameStats.ProTopRankedThisGame)
                    currentGame.player2BestProTopMatched = Context.GameStats.ProTopRankedThisGame;
            }

            currentGame.numberOfTurns = game.NumberOfMoves;
            currentGame.cumulativeIndexTurnsPlayed += Context.TurnConfig.IndexTurn - currentGame.indexTurnPlayed;
            currentGame.indexTurnPlayed = Context.TurnConfig.IndexTurn;
            //currentGame.activeTimePlayed = Mathf.Round(activeTimePlayedThisGame);

            // END OF GAME STATS FOR THIS GAME
            currentGame.playingAs = (Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? 1 : 2);
            currentGame.fullMovesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
            currentGame.fullMovesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
            currentGame.fullTopMatched = Context.GameStats.PlayerTopRankedThisGame;
            currentGame.fullProTopMatched = Context.GameStats.ProTopRankedThisGame;
            currentGame.fullOpponentTopMatched = Context.GameStats.OpponentTopRankedThisGame;

            // MATCH CREDENTIALS
            currentMatch.name = Context.GameConfig.SelectedMatch.Title;
            currentMatch.ID = Context.GameConfig.SelectedMatch.ID;
            currentMatch.lastGamePlayed = Context.GameConfig.IndexGame + 1;

            if (Context.TurnConfig.IndexTurn > currentMatch.totalNumberOfTurns - 2) currentMatch.lastGameCompleted = Context.GameConfig.IndexGame + 1;
            currentMatch.lastPlayedAs = (Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? 1 : 2);

            if (Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1)
            {
                if ((Context.GameConfig.IndexGame + 1) > currentMatch.lastHighestGamePlayedP1)
                    currentMatch.lastHighestGamePlayedP1 = (Context.GameConfig.IndexGame + 1);
            };

            if (Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1)
            {
                if ((Context.GameConfig.IndexGame + 1) > currentMatch.lastHighestGamePlayedP2)
                    currentMatch.lastHighestGamePlayedP2 = (Context.GameConfig.IndexGame + 1);
            };

            if (currentMatch.gameScoresDict.ContainsKey(gameName))
                currentMatch.gameScoresDict[gameName] = currentGame;
            else currentMatch.gameScoresDict.Add(gameName, currentGame);

            if (currentMatch.games == null) currentMatch.games = new GameScoreData[Context.GameConfig.SelectedMatch.GameCount];
            currentMatch.games[Context.GameConfig.IndexGame] = currentGame;

            scoreData.matchKey = matchKey;
            scoreData.matchScores = currentMatch;

            Main.Instance.PlayerScoresObj.SetPlayerScoreData(scoreData);
        }

        private void UpdatePlayerStatsGraphData()
        {
            Backgammon_Asset.GameData game = Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame);
            string gameName = game.name;
            string matchKey = Context.GameConfig.SelectedMatch.Title + " " + Context.GameConfig.SelectedMatch.ID;

            var graphStatsGameData = new GraphStatsGameData();

            graphStatsGameData.matchKey = matchKey;
            graphStatsGameData.player1 = Context.GameConfig.SelectedMatch.Player1;
            graphStatsGameData.player2 = Context.GameConfig.SelectedMatch.Player2;
            graphStatsGameData.playingAs = Context.GameConfig.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? "player1" : "player2";
            graphStatsGameData.gameName = gameName;
            graphStatsGameData.movesMade = Context.GameStats.TotalValidPlayerMovesThisGame;
            graphStatsGameData.movesMatched = Context.GameStats.TotalValidPlayerMatchesThisGame;
            graphStatsGameData.topMatched = Context.GameStats.PlayerTopRankedThisGame;
            graphStatsGameData.proTopMatched = Context.GameStats.ProTopRankedThisGame;

            Main.Instance.PlayerScoresObj.SetGraphStatsData(graphStatsGameData);
        }
    }
}