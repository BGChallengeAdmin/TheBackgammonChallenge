using System;
using UnityEngine;

namespace Backgammon
{
    public class TurnBeginState2D : GameState2D
    {
        public TurnBeginState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: TURN BEGIN -> {(Context.IsPlayersTurn ? "PLAYER" : "AI")}");

            // COPY THE BOARD STATE
            Context.TurnBeginBoardState.SetStateFromContext(Context);

            // SET VALUES FOR CONTINUE - SAVE BOARD STATE
            #region SAVE_BOARD_FOR_CONTINUE
            if (!Context.IfFastForwarding)
            {
                SaveForContinue();
            }
            #endregion

            #region SAVE_PLAYER_SCORE
            if (!Context.IfFastForwarding)
            {
                UpdatePlayerScoreData();
            }
            #endregion

            // SET PLAYER AND A.I. MOVE DATA TO THE A.I. GAME
            if (Main.Instance.IfPlayerVsAI) SetAITurnMoveData();

            // DEBUG TESTING - NO FAST FORWARD IN A.I. GAME
            if (Main.Instance.IfPlayerVsAI && Context.IfFastForwarding)
            {
                Context.FastForwardTurnIndex = Context.IndexTurn + 2;
                SetAIDebugTurnMoveData();
            }

            // GET TURN DATA FOR NORMAL AND A.I. GAMEPLAY
            var turnData = Context.SelectedMatch.Game(Context.IndexGame).GetPlayerMove(Context.IndexTurn);

            #region TestWins
            if (turnData.Contains("Wins"))
            {
                UnityEngine.Debug.Log($"PLAYER WINS");
                Context.GameWon = true;
                Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.GameWon;
                ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                goto BailOut;
            }
            else if (turnData.Contains("Loses"))
            {
                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                goto BailOut;
            }
            #endregion

            string[] moveParts = turnData.Split(':');

            #region TestExceptions
            if (moveParts[0] == string.Empty)
            {
                // NOTE: NOTATION ALLOWS ":" TO SHOW A SKIPPED TURN
                Context.Debug_debugObject.DebugMessage($"EMPTY TURN!!! SKIP A TURN OR WIN -> NO DICE WERE ROLLED");

                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;

                if (Context.PlayerIsBlockedFromMovingFromBar)
                {
                    Context.PlayerIsUnableToMove = true;
                    Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerUnableToMove;
                    ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                }

                // NOTE: IF NEXT TURN IS "PLAYER WINS" AND NOT BORNE OFF - PLAYER HAS CONCEDED..
                if (Context.SelectedMatch.Game(Context.IndexGame).GetPlayerMove(Context.IndexTurn + 1).Contains("Wins") &&
                    !(Context.HomeManager.PlayerHome.Counters == 15 || Context.HomeManager.OpponentHome.Counters == 15))
                {
                    moveParts = new string[] { "Concedes" };
                    goto Concedes;
                }

                goto BailOut;
            }

            Concedes:;

            // TEST MOVES / DOUBLES
            if (moveParts.Length == 1)
            {
                var result = moveParts[0];

                if (result == "Doubles")
                {
                    Context.Debug_debugObject.DebugMessage($"{(Context.IsPlayersTurn ? "PLAYER" : "OPPONENT")} DOUBLES");

                    ActiveState = GameStateMachine2D.EGameState2D.DoublingData;
                    goto BailOut;
                }
                else if (result == "Takes")
                {
                    Context.AIIndexTurn -= 2;
                    Context.DoublingTakesOrDrops = true;
                    ActiveState = GameStateMachine2D.EGameState2D.DoublingTakesOrDrops;
                    Context.Debug_debugObject.DebugMessage($"{(Context.IsPlayersTurn ? "PLAYER" : "OPPONENT")} TAKES");
                    goto BailOut;
                }
                else if (result == "Drops")
                {
                    Context.DoublingTakesOrDrops = false;
                    ActiveState = GameStateMachine2D.EGameState2D.DoublingTakesOrDrops;
                    Context.Debug_debugObject.DebugMessage($"{(Context.IsPlayersTurn ? "PLAYER" : "OPPONENT")} DROPS");
                    Context.AIGameWasWon = true;
                    goto BailOut;
                }
                else if (result == "Concedes") 
                {
                    Context.PlayerConcedes = true;
                    Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerConcedes;
                    ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                    Context.Debug_debugObject.DebugMessage($"PLAYER CONCEDES");
                    Context.AIGameWasWon = true;
                    goto BailOut;
                }

                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                goto BailOut;
            }
            #endregion

            #region UpdateUI
            // UPDATE U.I.
            if (ActiveState != GameStateMachine2D.EGameState2D.TurnEnd)
            {
                Context.GameScreenUI.SetPlayerScore(Context.PlayerScoreThisGame);
                Context.GameScreenUI.SetProPlayerScore(Context.ProScoreThisGame);
                Context.GameScreenUI.SetOpponentScore(Context.OpponentScoreThisGame);

                Context.GameScreenUI.SetPlayerTopMatched(Context.PlayerTopRankedThisGame);
                Context.GameScreenUI.SetProPlayerTopMatched(Context.ProTopRankedThisGame);
                Context.GameScreenUI.SetOpponentTopMatched(Context.OpponentTopRankedThisGame);

                var indexTurn = Main.Instance.IfPlayerVsAI ? Context.AIIndexTurn : Context.IndexTurn;
                Context.GameScreenUI.SetGameTurn(indexTurn + (Context.IfPlayer1GoesFirst ? 1 : 0));

                Context.GameScreenUI.SetUndoButtonActive(false);

                if (Context.FastForwardTurnIndex <= Context.IndexTurn)
                    Context.IfFastForwarding = false;
            }
            #endregion

            #region CapturePlayerMove
            // CAPTURE DICE ROLLS
            string dice = moveParts[0];
            var dice1 = int.Parse(dice[0].ToString());
            var dice2 = int.Parse(dice[1].ToString());

            Context.Dice1 = dice1;
            Context.Dice2 = dice2;
            Context.DiceManager.SetDiceValues(dice1, dice2);

            //UnityEngine.Debug.Log($"DICE {dice[0]} {dice[1]} -> {Context.Dice1} {Context.Dice2}");

            // RESET AND CAPTURE MOVE DATA
            for (int playerM = 0; playerM < 4; playerM++) { Context.PlayerMovesInfo[playerM] = Context.PlayerMovesInfo[playerM].Reset(); }
            for (int proM = 0; proM < 4; proM++) { Context.RecordedMovesInfo[proM] = Context.RecordedMovesInfo[proM].Reset(); }
            //for (int aiM = 0; aiM < 4; aiM++) { Context.TopRankedMovesInfo[aiM] = Context.TopRankedMovesInfo[aiM].Reset(); }

            // ONLY RESET OPPONENT ON THEIR TURN - MAINTAIN FOR PLAYER ANLYSIS
            if (!Context.IsPlayersTurn) for (int oppM = 0; oppM < 4; oppM++) 
                    Context.OpponentMovesInfo[oppM] = Context.OpponentMovesInfo[oppM].Reset();

            string movesData = moveParts[1];
            string[] moves = movesData.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Context.TotalMovesThisTurn = moves.Length;
            Context.MovesAvailable = Context.TotalMovesThisTurn;

            // TEST WHEN THERE IS NO MOVE DATA
            if (moves.Length == 0)
            {
                UnityEngine.Debug.Log($"THERE IS NO MOVE DATA - PLAYER WAS BLOCKED FROM MOVING");

                Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerUnableToMove; 
                Context.PlayerIsUnableToMove = true;

                if (Context.IfFastForwarding)
                    ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                else
                    ActiveState = GameStateMachine2D.EGameState2D.RollDice;

                goto BailOut;
            }
            else
            {
                Context.IsPlayersMakingMoves = Context.IsPlayersTurn ? true : false;
                Context.PlayerMoveIndex = 0;
                Context.CountersToMoveIndex = 0;

                if (!Context.IsPlayersTurn) Context.CountersToMoveIndex = Context.TotalMovesThisTurn;

                if (Context.IfFastForwarding)
                {
                    Context.IsPlayersMakingMoves = false;
                    Context.CountersToMoveIndex = Context.TotalMovesThisTurn;
                }
            }

            // NOTE: MOVES WILL BE IN THE FORMAT X/Y* (* IF THERE IS A BLOT)
            for (int move = 0; move < moves.Length; move++)
            {
                var pointFrom = moves[move].Split('/')[0];
                var pointTo = moves[move].Split('/')[1];
                var ifBlot = pointTo[pointTo.Length - 1] == '*' ? true : false;

                if (ifBlot) { pointTo = pointTo.TrimEnd('*'); }

                Context.RecordedMovesInfo[move].pointFrom = pointFrom == "bar" ? 25 : int.Parse(pointFrom);
                Context.RecordedMovesInfo[move].pointTo = pointTo == "off" ? 0 : int.Parse(pointTo);
                Context.RecordedMovesInfo[move].ifBlot = ifBlot;

                // CAPTURE AND MAINTAIN OPPONENT DATA FOR PLAYER TURN ANALYSIS
                if (!Context.IsPlayersTurn)
                {
                    Context.OpponentMovesInfo[move].pointFrom = pointFrom == "bar" ? 25 : int.Parse(pointFrom);
                    Context.OpponentMovesInfo[move].pointTo = pointTo == "off" ? 0 : int.Parse(pointTo);
                    Context.OpponentMovesInfo[move].ifBlot = ifBlot;
                }
            }

            #endregion

            // CAPTURE A.I. DATA FOR THE TURN
            if (!Main.Instance.IfPlayerVsAI || (Main.Instance.IfPlayerVsAI && Context.IsPlayersTurn))
                ActiveState = GameStateMachine2D.EGameState2D.AIData;
            else ActiveState = GameStateMachine2D.EGameState2D.RollDice;

            if (Context.IfFastForwarding) ActiveState = GameStateMachine2D.EGameState2D.MoveCounters;

            BailOut:;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
        
        // ---------------------------------------- AI METHODS ----------------------------------------

        private void SetAITurnMoveData()
        {
            // RESET INDEX TO CURRENT MOVE
            Context.IndexTurn = 0;

            // RESTORE DEFAULT MOVES
            Context.AIGameData.Moves[Context.IndexTurn] = ":";
            Context.AIGameData.Moves[1] = "For:Doubles";
            Context.AIGameData.Moves[2] = "For:Takes";

            if (TestIfBlockedFromMovingFromBar(Context.IsPlayersTurn)) return;

            if (Context.IsPlayersTurn)
            {
                var dice = $"{Context.Dice1.ToString()}{Context.Dice2.ToString()}:";
                var move = " 0/0 0/0" + ((Context.Dice1 == Context.Dice2) ? " 0/0 0/0" : string.Empty);

                Context.AIGameData.Moves[Context.IndexTurn] = dice + (TestIfValidMoves() ? move : string.Empty);

                if (Context.AIOfferedDoubles)
                {
                    Debug.Log($"AI OFFERED DOUBLE -> PLAYER {Context.AIPlayerAcceptsDoubles}");

                    Context.AIOfferedDoubles = false;
                    Context.IndexTurn = 2;

                    if (Context.AIPlayerAcceptsDoubles) Context.AIGameData.Moves[Context.IndexTurn] = "Takes";
                    else Context.AIGameData.Moves[Context.IndexTurn] = "Drops";
                }
                else if (Context.HomeManager.OpponentHome.Counters == 15)
                {
                    Context.AIGameData.Moves[Context.IndexTurn] = "Loses";
                }
                else if (Context.HomeManager.PlayerHome.Counters == 15 || Context.AIGameWasWon)
                {
                    SetAIGameWonForPoints(true);
                }
                else if (Context.ConcedeTheGame)
                {
                    Context.AIGameData.Moves[Context.IndexTurn] = "Concedes";
                }
            }
            else if (!Context.IsPlayersTurn)
            {
                // IF NO MOVE DATA THEN DOUBLING INTERACTION
                if (Context.HomeManager.PlayerHome.Counters == 15)
                {
                    Context.AIGameData.Moves[Context.IndexTurn] = "Loses";
                }
                else if (Context.HomeManager.OpponentHome.Counters == 15 || Context.AIGameWasWon)
                {
                    SetAIGameWonForPoints(false);
                }
                else if (Context.CapturedAIMoveData)
                {
                    // BUILD THE MOVE AND SET TO THE TURN
                    var dice = $"{Context.Dice1.ToString()}{Context.Dice2.ToString()}:";
                    var move = string.Empty;

                    // WILL BE NULL IF "NO MOVE POSSIBLE" -> CAUSE THE A.I. TO SKIP A TURN
                    if (Context.AIRankedMoves[0] is not null)
                    {
                        var aiRank = GetAIRank(Context.AISettings.PlayerPreset);
                        var rank = aiRank <= Context.AIRankedMoves.Length ? aiRank : Context.AIRankedMoves.Length;

                        if (Context.AIIndexTurn < 2) rank = 1;

                        //rank = 2;

                        var aiRankedMove = Context.AIRankedMoves[rank - 1].movePart;
                        var blot = false;
                        var blottedToCounter = 0;

                        foreach (FromTo fromTo in aiRankedMove)
                        {
                            blot = TestIfBlot(fromTo.to);

                            for (int b = 0; b < blottedToCounter; b++)
                            {
                                FromTo ftb = aiRankedMove[b];
                                if (fromTo.to == ftb.to)
                                    blot = false;
                            }

                            blottedToCounter++;

                            move += " " + fromTo.from + "/" + fromTo.to + (blot ? "*" : string.Empty);
                        }
                    }
                    else 
                    {
                        // NOTE: REQUIRES RESET
                        Context.OpponentMatchedRankThisTurn = 0;
                    }

                    Context.AIGameData.Moves[Context.IndexTurn] = dice + move;
                }
                else if (Context.PlayerOfferedDoubles)
                {
                    Context.IndexTurn = 2;

                    if (Context.AIDoublingData.cubeDecision.other == "accept")
                        Context.AIGameData.Moves[Context.IndexTurn] = "Takes";
                    else if (Context.AIDoublingData.cubeDecision.other == "resign")
                        Context.AIGameData.Moves[Context.IndexTurn] = "Drops";
                }
                else if (Context.CapturedAIDoublingData)
                {
                    if (Context.AIDoublingData.cubeDecision.owner == "double" ||
                        Context.AIDoublingData.cubeDecision.owner == "redouble")
                    {
                        Context.AIOfferedDoubles = true;

                        Context.IndexTurn = 1;
                        Context.AIGameData.Moves[Context.IndexTurn] = "Doubles";
                    }
                }
            }
        }
        
        private void SetAIDebugTurnMoveData()
        {
            // RESET INDEX TO CURRENT MOVE
            Context.IndexTurn = 0;

            // RESTORE DEFAULT MOVES
            Context.AIGameData.Moves[Context.IndexTurn] = ":";
            Context.AIGameData.Moves[1] = "For:Doubles";
            Context.AIGameData.Moves[2] = "For:Takes";

            // BUILD THE MOVE AND SET TO THE TURN
            var dice = $"{Context.Dice1.ToString()}{Context.Dice2.ToString()}:";
            var move = string.Empty;

            // WILL BE NULL IF "NO MOVE POSSIBLE" -> CAUSE THE A.I. TO SKIP A TURN
            if (Context.AIRankedMoves[0] is not null)
            {
                var aiRankedMove = Context.AIRankedMoves[0].movePart;
                var blot = false;
                var blottedToCounter = 0;

                foreach (FromTo fromTo in aiRankedMove)
                {
                    blot = TestIfBlot(fromTo.to);

                    for (int b = 0; b < blottedToCounter; b++)
                    {
                        FromTo ftb = aiRankedMove[b];
                        if (fromTo.to == ftb.to)
                            blot = false;
                    }

                    blottedToCounter++;

                    move += " " + fromTo.from + "/" + fromTo.to + (blot ? "*" : string.Empty);
                }
            }
            else 
            {
                // TEST IF GAME WAS WON
                if (Context.HomeManager.PlayerHome.Counters == 15 ||
                    Context.HomeManager.OpponentHome.Counters == 15)
                {
                    UnityEngine.Debug.Log($"*** GAME HAS BEEN WON POWER MOVE***");
                    Context.Game.ConfigureContextAndInit();
                }
            }

            Context.AIGameData.Moves[Context.IndexTurn] = dice + move;
        }

        private bool TestIfBlockedFromMovingFromBar(bool playerTurn)
        {
            Context.PlayerIsBlockedFromMovingFromBar = false;

            if (playerTurn) 
            {
                if (Context.BarManager.PlayerBar.Counters == 0) return false;

                for (int p = 24; p > 18; p--)
                {
                    var point = Context.PointsManager.GetPlayerPointByID(p);

                    if (point.Owner == Context.PlayingAs) return false;
                    else if (point.Counters < 2) return false;
                }
            }
            else
            {
                if (Context.BarManager.OpponentBar.Counters == 0) return false;

                for (int p = 6; p > 0; p--)
                {
                    var point = Context.PointsManager.GetPlayerPointByID(p);

                    if (point.Owner != Context.PlayingAs) return false;
                    else if (point.Counters < 2) return false;
                }

                Context.OpponentMatchedRankThisTurn = 0;
            }

            Context.PlayerIsBlockedFromMovingFromBar = true;
            return true;
        }

        private bool TestIfValidMoves(bool ifPlayerTurn = true)
        {
            var valid = false;

            var opponentColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;
            var step1 = 0;
            var step2 = 0;

            // COUNTER ON BAR
            if (Context.BarManager.PlayerBar.Counters > 0)
            {
                step1 = 25 - Context.Dice1;
                step2 = 25 - Context.Dice2;

                valid = (TestIfCanMoveToPoint(step1, opponentColour) || 
                        (TestIfCanMoveToPoint(step2, opponentColour)) ? true : false);
            }
            // POINTS
            else
            {
                for (int p = 24; p > 0; p--)
                {
                    if (Context.PointsManager.GetPlayerPointByID(p).Owner != Context.PlayingAs) continue;
                    
                    step1 = p - Context.Dice1;
                    step2 = p - Context.Dice2;

                    var valid1 = TestIfCanMoveToPoint(step1, opponentColour);
                    var valid2 = TestIfCanMoveToPoint(step2, opponentColour);

                    if (valid1 || valid2)
                    {
                        valid = true;
                        break;
                    }
                }
            }

            return valid;
        }

        private bool TestIfCanMoveToPoint(int pointToIndex, Game2D.PlayerColour opponentColour)
        {
            var playerColour = Context.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            Context.IfBearingOff = Context.PointsManager.TestIfBearingOff(playerColour);

            if (!Game2D.Context.IfBearingOff && pointToIndex <= 0) return false;

            if (pointToIndex > 0)
            {
                var pointTo = Context.PointsManager.GetPlayerPointByID(pointToIndex);
                if (pointTo.Colour == opponentColour && pointTo.Counters > 1) return false;
            }

            return true;
        }

        private bool TestIfBlot(string pointTo)
        {
            if (pointTo == "off") return false;

            var valid = false;
            var to = int.Parse(pointTo);

            if (0 < to && to < 25)
            {
                var playerOn = Context.IsPlayersTurn ? Context.OpponentAs : Context.PlayingAs;
                var point = Context.IsPlayersTurn ? Context.PointsManager.GetPlayerPointByID(to) : 
                                                    Context.PointsManager.GetOpponentPointByID(to);
                if (point.Owner == playerOn && point.Counters == 1) valid = true;
            }

            return valid;
        }

        private int GetAIRank(string difficulty)
        {
            switch (difficulty)
            {
                case "PERFECT": return 1;
                case "HARD":
                    {
                        var h = UnityEngine.Random.Range(0, 101);
                        if (h < 25) return 2;
                        else return 1;
                    }
                case "MEDIUM":
                    {
                        var m = UnityEngine.Random.Range(0, 101);
                        if (m < 2) return 4;
                        else if (m < 10) return 3;
                        else if (m < 40) return 2;
                        else return 1;
                    }
                case "EASY":
                    {
                        var e = UnityEngine.Random.Range(0, 101);
                        if (e < 10) return 5;
                        else if (e < 35) return 4;
                        else if (e < 65) return 3;
                        else if (e < 85) return 2;
                        else return 1;
                    }
                default: return 1;
            }
        }

        private void SetAIGameWonForPoints(bool playerWon)
        {
            var multiplier = TestWonByGammonBackgammon(playerWon);
            var pointsWon = Context.DoublingManager.CubeValue * multiplier;

            Context.AIGameData.Moves[Context.IndexTurn] = "Wins " + pointsWon + " point";

            Context.AIGameData.GameConstructor(Context.AIGameData.Player1PointsAtStart,
                                               Context.AIGameData.Player2PointsAtStart,
                                               Context.AIGameData.Moves,
                                               Context.AIGameData.Player1PointsAtStart + (playerWon ? pointsWon : 0),
                                               Context.AIGameData.Player2PointsAtStart + (playerWon ? 0 : pointsWon));
        }

        private int TestWonByGammonBackgammon(bool playerWon)
        {
            var multiplier = 1;

            if ((playerWon ? Context.BarManager.OpponentBar.Counters :
                Context.BarManager.PlayerBar.Counters) > 0)
            {
                Context.GameWonByBackGammon = true;
                return multiplier = 3;
            }

            var from = playerWon ? 6 : 24;
            var to = playerWon ? 0 : 18;

            for (var p = from; p > to ; p--)
            {
                var point = Context.PointsManager.GetPlayerPointByID(p);

                if (point.Owner == (playerWon ? Context.OpponentAs : Context.PlayingAs))
                {
                    Context.GameWonByBackGammon = true;
                    return multiplier = 3;
                }
            }
                
            if ((playerWon ? Context.HomeManager.OpponentHome.Counters :
                Context.HomeManager.PlayerHome.Counters) == 0)
            {
                Context.GameWonByGammon = true;
                return multiplier = 2;
            }

            return multiplier;
        }

        // -------------------------------------- HELPER METHODS --------------------------------------

        private void SaveForContinue()
        {
            var saveObject = Main.Instance.IfPlayerVsAI ? Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame : 
                                                          Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame;

            saveObject.IfMatchToContinue = true;
            saveObject.BoardState = Context.TurnBeginBoardState.GetBoardStateString();

            // MATCH CONFIG

            saveObject.MatchTitle = Context.SelectedMatch.Title;
            saveObject.MatchID = Context.SelectedMatch.ID;
            saveObject.GameName = Context.SelectedGame.name;
            saveObject.PlayingAs = Context.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? 1 : 2;

            saveObject.MatchIndex = (Context.SelectedMatch.ID != "DEMO") ? int.Parse(Context.SelectedMatch.ID) : 0;
            saveObject.GameIndex = Context.IndexGame;
            saveObject.TurnIndex = Main.Instance.IfPlayerVsAI ? Context.AIIndexTurn : Context.IndexTurn;

            // MATCH STATS

            saveObject.MovesMade = Context.TotalValidPlayerMovesThisGame;
            saveObject.MovesMatched = Context.TotalValidPlayerMatchesThisGame;
            saveObject.OpponentMovesMade = Context.TotalValidOpponentMovesThisGame;

            saveObject.AITopMatched = Context.PlayerTopRankedThisGame;
            saveObject.ProMovesMatched = Context.ProTopRankedThisGame;
            saveObject.OpponentMovesMatched = Context.OpponentTopRankedThisGame;

            saveObject.PlayerSecondMatched = Context.PlayerSecondRankedThisGame;
            saveObject.ProSecondMatched = Context.ProSecondRankedThisGame;
            saveObject.OpponentSecondMatched = Context.OpponentSecondRankedThisGame;

            saveObject.PlayerTotalScore = Context.PlayerScoreThisGame;
            saveObject.ProTotalScore = Context.ProScoreThisGame;
            saveObject.OpponentTotalScore = Context.OpponentScoreThisGame;

            saveObject.IsPlayerTurn = Context.IsPlayersTurn;

            // NOTE: ONLY USED FOR A.I. GAME
            var playerHasCube = Context.DoublingManager.CubeOwner == Game2D.PlayingAs.PLAYER_1;
            saveObject.PlayerDoublingValue =  playerHasCube ? Context.DoublingManager.CubeValueIndex : 0;

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
            Backgammon_Asset.GameData game = Context.SelectedGame;
            string gameName = game.name;
            string matchKey = Context.SelectedMatch.Title + " " + Context.SelectedMatch.ID;

            PlayerScoreData scoreData = Main.Instance.PlayerScoresObj.GetPlayerScoreData(matchKey);
            MatchScoreData currentMatch = scoreData.matchScores.ID != string.Empty ? scoreData.matchScores : new MatchScoreData();
            GameScoreData currentGame = currentMatch.gameScoresDict.ContainsKey(gameName) ? currentMatch.gameScoresDict[gameName] : new GameScoreData();

            currentGame.name = gameName;

            if (Context.TotalValidPlayerMovesThisGame > 0)
            {
                // CUMULATIVE SCORES FOR THIS GAME
                currentGame.fullCumulativeMovesMade += 1;
                currentGame.fullCumulativeMovesMatched += (Context.PlayerMatchedProMove) ? 1 : 0;
                currentGame.fullCumulativeTopMatchedMoves += (Context.PlayerMatchedRankThisTurn == 1) ? 1 : 0;
                currentGame.fullCumulativeTopMatchedMoves += (Context.ProMatchedRankThisTurn == 1) ? 1 : 0;
            }

            //if (currentGame.player1PossibleCounterMoves < player1PossibleMovesThisGame)
            //    currentGame.player1PossibleCounterMoves = player1PossibleMovesThisGame;
            //if (currentGame.player2PossibleCounterMoves < player2PossibleMovesThisGame)
            //    currentGame.player2PossibleCounterMoves = player2PossibleMovesThisGame;
            //if (currentGame.totalCounterMovesSeenPlayed < (player1PossibleMovesThisGame + player2PossibleMovesThisGame))
            //    currentGame.totalCounterMovesSeenPlayed = (player1PossibleMovesThisGame + player2PossibleMovesThisGame);

            // SEPARATED SCORES THIS GAME / BEST
            if (Context.PlayingAs == Game2D.PlayingAs.PLAYER_1)
            {
                currentGame.player1IndexTurn = Context.IndexTurn;
                currentGame.player1MovesMade = Context.TotalValidPlayerMovesThisGame;
                currentGame.player1MovesMatched = Context.TotalValidPlayerMatchesThisGame;
                currentGame.player1TopMatched = Context.PlayerTopRankedThisGame;
                currentGame.player1ProTopMatched = Context.ProTopRankedThisGame;

                if (currentGame.player1BestIndexTurn <= Context.IndexTurn) 
                    currentGame.player1BestIndexTurn = Context.IndexTurn;
                if (currentGame.player1BestMovesMade <= Context.TotalValidPlayerMovesThisGame) 
                    currentGame.player1BestMovesMade = Context.TotalValidPlayerMovesThisGame;
                if (currentGame.player1BestMovesMatched <= Context.TotalValidPlayerMatchesThisGame) 
                    currentGame.player1BestMovesMatched = Context.TotalValidPlayerMatchesThisGame;
                if (currentGame.player1BestTopMatched <= Context.PlayerTopRankedThisGame) 
                    currentGame.player1BestTopMatched = Context.PlayerTopRankedThisGame;
                if (currentGame.player1BestProTopMatched <= Context.ProTopRankedThisGame) 
                    currentGame.player1BestProTopMatched = Context.ProTopRankedThisGame;
            }
            else if (Context.PlayingAs == Game2D.PlayingAs.PLAYER_2)
            {
                currentGame.player2IndexTurn = Context.IndexTurn;
                currentGame.player2MovesMade = Context.TotalValidPlayerMovesThisGame;
                currentGame.player2MovesMatched = Context.TotalValidPlayerMatchesThisGame;
                currentGame.player2TopMatched = Context.PlayerTopRankedThisGame;
                currentGame.player2ProTopMatched = Context.ProTopRankedThisGame;

                if (currentGame.player2BestIndexTurn <= Context.IndexTurn)
                    currentGame.player2BestIndexTurn = Context.IndexTurn;
                if (currentGame.player2BestMovesMade <= Context.TotalValidPlayerMovesThisGame)
                    currentGame.player2BestMovesMade = Context.TotalValidPlayerMovesThisGame;
                if (currentGame.player2BestMovesMatched <= Context.TotalValidPlayerMatchesThisGame)
                    currentGame.player2BestMovesMatched = Context.TotalValidPlayerMatchesThisGame;
                if (currentGame.player2BestTopMatched <= Context.PlayerTopRankedThisGame)
                    currentGame.player2BestTopMatched = Context.PlayerTopRankedThisGame;
                if (currentGame.player2BestProTopMatched <= Context.ProTopRankedThisGame)
                    currentGame.player2BestProTopMatched = Context.ProTopRankedThisGame;
            }

            currentGame.numberOfTurns = game.NumberOfMoves;
            currentGame.cumulativeIndexTurnsPlayed += Context.IndexTurn - currentGame.indexTurnPlayed;
            currentGame.indexTurnPlayed = Context.IndexTurn;
            //currentGame.activeTimePlayed = Mathf.Round(activeTimePlayedThisGame);

            // END OF GAME STATS FOR THIS GAME
            currentGame.playingAs = (Context.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? 1 : 2);
            currentGame.fullMovesMade = Context.TotalValidPlayerMovesThisGame;
            currentGame.fullMovesMatched = Context.TotalValidPlayerMatchesThisGame;
            currentGame.fullTopMatched = Context.PlayerTopRankedThisGame;
            currentGame.fullProTopMatched = Context.ProTopRankedThisGame;
            currentGame.fullOpponentTopMatched = Context.OpponentTopRankedThisGame;

            // MATCH CREDENTIALS
            currentMatch.name = Context.SelectedMatch.Title;
            currentMatch.ID = Context.SelectedMatch.ID;
            currentMatch.lastGamePlayed = Context.IndexGame + 1;
            
            if (Context.IndexTurn > currentMatch.totalNumberOfTurns - 2) currentMatch.lastGameCompleted = Context.IndexGame + 1;
            currentMatch.lastPlayedAs = (Context.PlayingAs == Game2D.PlayingAs.PLAYER_1 ? 1 : 2);
            
            if (Context.PlayingAs == Game2D.PlayingAs.PLAYER_1) 
            { if ((Context.IndexGame + 1) > currentMatch.lastHighestGamePlayedP1) 
                    currentMatch.lastHighestGamePlayedP1 = (Context.IndexGame + 1); };
            
            if (Context.PlayingAs == Game2D.PlayingAs.PLAYER_1) 
            { if ((Context.IndexGame + 1) > currentMatch.lastHighestGamePlayedP2) 
                    currentMatch.lastHighestGamePlayedP2 = (Context.IndexGame + 1); };

            if (currentMatch.gameScoresDict.ContainsKey(gameName))
                currentMatch.gameScoresDict[gameName] = currentGame;
            else currentMatch.gameScoresDict.Add(gameName, currentGame);

            if (currentMatch.games == null) currentMatch.games = new GameScoreData[Context.SelectedMatch.GameCount];
            currentMatch.games[Context.IndexGame] = currentGame;

            scoreData.matchKey = matchKey;
            scoreData.matchScores = currentMatch;

            Main.Instance.PlayerScoresObj.SetPlayerScoreData(scoreData);
        }

        private void UpdatePlayerStatsGraphData()
        {
            Backgammon_Asset.GameData game = Context.SelectedMatch.Game(Context.IndexGame);
            string gameName = game.name;
            string matchKey = Context.SelectedMatch.Title + " " + Context.SelectedMatch.ID;

            //var graphStatsGameData = new GraphStatsGameData();

            //graphStatsGameData.matchKey = matchKey;
            //graphStatsGameData.player1 = match.Player1;
            //graphStatsGameData.player2 = match.Player2;
            //graphStatsGameData.playingAs = playingAs == PlayerId.Player1 ? "player1" : "player2";
            //graphStatsGameData.gameName = gameName;
            //graphStatsGameData.movesMade = movesThisGame;
            //graphStatsGameData.movesMatched = matchesThisGame;
            //graphStatsGameData.topMatched = topMatchesThisGame;
            //graphStatsGameData.proTopMatched = proTopMatchesThisGame;

            //Main.Instance.PlayerScoresObj .SetGraphStatsData(graphStatsGameData);
        }
    }
}