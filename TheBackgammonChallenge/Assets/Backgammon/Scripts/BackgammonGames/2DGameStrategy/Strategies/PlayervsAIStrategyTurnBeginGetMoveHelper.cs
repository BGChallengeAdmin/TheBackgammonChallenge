using UnityEngine;

namespace Backgammon
{
    public class PlayervsAIStrategyTurnBeginGetMoveHelper
    {
        public void SetAITurnMoveData(GameStateContext2DStrategy Context)
        {
            // RESTORE DEFAULT MOVES
            Context.GameConfig.AIGameData.Moves[0] = ":";
            Context.GameConfig.AIGameData.Moves[1] = "For:Doubles";
            Context.GameConfig.AIGameData.Moves[2] = "For:Takes";

            if (TestIfBlockedFromMovingFromBar(Context)) return;

            if (Context.TurnConfig.IsPlayersTurn)
            {
                var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
                var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
                var dice = $"{dice1.ToString()}{dice2.ToString()}:";
                var move = " 0/0 0/0" + ((dice1 == dice2) ? " 0/0 0/0" : string.Empty);

                Context.GameConfig.AIGameData.Moves[0] = dice + (TestIfValidMoves(Context) ? move : string.Empty);

                if (Context.TurnConfig.AIOfferedDoubles)
                {
                    Debug.Log($"AI OFFERED DOUBLE -> PLAYER {Context.TurnConfig.AIPlayerAcceptsDoubles}");

                    Context.TurnConfig.AIOfferedDoubles = false;

                    if (Context.TurnConfig.AIPlayerAcceptsDoubles) Context.GameConfig.AIGameData.Moves[2] = "Takes";
                    else Context.GameConfig.AIGameData.Moves[2] = "Drops";
                }
                else if (Context.HomeManager.OpponentHome.Counters == 15)
                {
                    Context.GameConfig.AIGameData.Moves[2] = "Loses";
                }
                else if (Context.HomeManager.PlayerHome.Counters == 15 || Context.GameConfig.AIGameWasWon)
                {
                    SetAIGameWonForPoints(Context, true);
                }
                else if (Context.TurnConfig.ConcedeTheGame)
                {
                    Context.GameConfig.AIGameData.Moves[0] = "Concedes";
                }
            }
            else if (!Context.TurnConfig.IsPlayersTurn)
            {
                // IF NO MOVE DATA THEN DOUBLING INTERACTION
                if (Context.HomeManager.PlayerHome.Counters == 15)
                {
                    Context.GameConfig.AIGameData.Moves[0] = "Loses";
                }
                else if (Context.HomeManager.OpponentHome.Counters == 15 || Context.GameConfig.AIGameWasWon)
                {
                    SetAIGameWonForPoints(Context, false);
                }
                else if (Context.TurnConfig.CapturedAIMoveData)
                {
                    // BUILD THE MOVE AND SET TO THE TURN
                    var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
                    var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
                    var dice = $"{dice1.ToString()}{dice2.ToString()}:";
                    var move = string.Empty;

                    // WILL BE NULL IF "NO MOVE POSSIBLE" -> CAUSE THE A.I. TO SKIP A TURN
                    if (Context.TurnConfig.AIRankedMoves[0] is not null)
                    {
                        var aiRank = GetAIRank(Context.GameConfig.AISettings.PlayerPreset);
                        var rank = aiRank <= Context.TurnConfig.AIRankedMoves.Length ? aiRank : Context.TurnConfig.AIRankedMoves.Length;

                        if (Context.TurnConfig.IndexTurn < 2) rank = 1;

                        var aiRankedMove = Context.TurnConfig.AIRankedMoves[rank - 1].movePart;
                        var blot = false;
                        var blottedToCounter = 0;

                        foreach (FromTo fromTo in aiRankedMove)
                        {
                            blot = TestIfBlot(Context, fromTo.to);

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
                        Context.TurnConfig.OpponentMatchedRankThisTurn = 0;
                    }

                    Context.GameConfig.AIGameData.Moves[Context.TurnConfig.IndexTurn] = dice + move;
                }
                else if (Context.TurnConfig.PlayerOfferedDoubles)
                {
                    if (Context.TurnConfig.AIDoublingData.cubeDecision.other == "accept")
                        Context.GameConfig.AIGameData.Moves[2] = "Takes";
                    else if (Context.TurnConfig.AIDoublingData.cubeDecision.other == "resign")
                        Context.GameConfig.AIGameData.Moves[2] = "Drops";
                }
                else if (Context.TurnConfig.CapturedAIDoublingData)
                {
                    if (Context.TurnConfig.AIDoublingData.cubeDecision.owner == "double" ||
                        Context.TurnConfig.AIDoublingData.cubeDecision.owner == "redouble")
                    {
                        Context.TurnConfig.AIOfferedDoubles = true;
                        Context.GameConfig.AIGameData.Moves[1] = "Doubles";
                    }
                }
            }
        }

        public void SetAIDebugTurnMoveData(GameStateContext2DStrategy Context)
        {
            // RESTORE DEFAULT MOVES
            Context.GameConfig.AIGameData.Moves[0] = ":";
            Context.GameConfig.AIGameData.Moves[1] = "For:Doubles";
            Context.GameConfig.AIGameData.Moves[2] = "For:Takes";

            // BUILD THE MOVE AND SET TO THE TURN
            var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
            var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;
            var dice = $"{dice1.ToString()}{dice2.ToString()}:";
            var move = string.Empty;

            // WILL BE NULL IF "NO MOVE POSSIBLE" -> CAUSE THE A.I. TO SKIP A TURN
            if (Context.TurnConfig.AIRankedMoves[0] is not null)
            {
                var aiRankedMove = Context.TurnConfig.AIRankedMoves[0].movePart;
                var blot = false;
                var blottedToCounter = 0;

                foreach (FromTo fromTo in aiRankedMove)
                {
                    blot = TestIfBlot(Context, fromTo.to);

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

            Context.GameConfig.AIGameData.Moves[0] = dice + move;
        }

        private bool TestIfBlockedFromMovingFromBar(GameStateContext2DStrategy Context)
        {
            var playerTurn = Context.TurnConfig.IsPlayersTurn;
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

        private bool TestIfValidMoves(GameStateContext2DStrategy Context)
        {
            var valid = false;

            var opponentColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.WHITE : Game2D.PlayerColour.BLACK;
            var step1 = 0;
            var step2 = 0;

            var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
            var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

            // COUNTER ON BAR
            if (Context.BarManager.PlayerBar.Counters > 0)
            {
                step1 = 25 - dice1;
                step2 = 25 - dice2;

                valid = (TestIfCanMoveToPoint(Context, step1, opponentColour) ||
                        (TestIfCanMoveToPoint(Context, step2, opponentColour)) ? true : false);
            }
            // POINTS
            else
            {
                for (int p = 24; p > 0; p--)
                {
                    if (Context.PointsManager.GetPlayerPointByID(p).Owner != Context.GameConfig.PlayingAs) continue;

                    step1 = p - dice1;
                    step2 = p - dice2;

                    var valid1 = TestIfCanMoveToPoint(Context, step1, opponentColour);
                    var valid2 = TestIfCanMoveToPoint(Context, step2, opponentColour);

                    if (valid1 || valid2)
                    {
                        valid = true;
                        break;
                    }
                }
            }

            return valid;
        }

        private bool TestIfCanMoveToPoint(GameStateContext2DStrategy Context, int pointToIndex, Game2D.PlayerColour opponentColour)
        {
            var playerColour = Context.GameConfig.IfPlayerIsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            Context.TurnConfig.IfBearingOff = Context.PointsManager.TestIfBearingOff(playerColour);

            if (!Context.TurnConfig.IfBearingOff && pointToIndex <= 0) return false;

            if (pointToIndex > 0)
            {
                var pointTo = Context.PointsManager.GetPlayerPointByID(pointToIndex);
                if (pointTo.Colour == opponentColour && pointTo.Counters > 1) return false;
            }

            return true;
        }

        private bool TestIfBlot(GameStateContext2DStrategy Context, string pointTo)
        {
            if (pointTo == "off") return false;

            var valid = false;
            var to = int.Parse(pointTo);

            if (0 < to && to < 25)
            {
                var playerOn = Context.TurnConfig.IsPlayersTurn ? Context.GameConfig.OpponentAs : Context.GameConfig.PlayingAs;
                var point = Context.TurnConfig.IsPlayersTurn ? Context.PointsManager.GetPlayerPointByID(to) :
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

        private void SetAIGameWonForPoints(GameStateContext2DStrategy Context, bool playerWon)
        {
            var multiplier = TestWonByGammonBackgammon(Context, playerWon);
            var pointsWon = Context.DoublingManager.CubeValue * multiplier;

            Context.GameConfig.AIGameData.Moves[0] = "Wins " + pointsWon + " point";

            Context.GameConfig.AIGameData.GameConstructor(Context.GameConfig.AIGameData.Player1PointsAtStart,
                                               Context.GameConfig.AIGameData.Player2PointsAtStart,
                                               Context.GameConfig.AIGameData.Moves,
                                               Context.GameConfig.AIGameData.Player1PointsAtStart + (playerWon ? pointsWon : 0),
                                               Context.GameConfig.AIGameData.Player2PointsAtStart + (playerWon ? 0 : pointsWon));
        }

        private int TestWonByGammonBackgammon(GameStateContext2DStrategy Context, bool playerWon)
        {
            var multiplier = 1;

            if ((playerWon ? Context.BarManager.OpponentBar.Counters :
                Context.BarManager.PlayerBar.Counters) > 0)
            {
                Context.GameConfig.GameWonByBackGammon = true;
                return multiplier = 3;
            }

            var from = playerWon ? 6 : 24;
            var to = playerWon ? 0 : 18;

            for (var p = from; p > to; p--)
            {
                var point = Context.PointsManager.GetPlayerPointByID(p);

                if (point.Owner == (playerWon ? Context.GameConfig.OpponentAs : Context.GameConfig.PlayingAs))
                {
                    Context.GameConfig.GameWonByBackGammon = true;
                    return multiplier = 3;
                }
            }

            if ((playerWon ? Context.HomeManager.OpponentHome.Counters :
                Context.HomeManager.PlayerHome.Counters) == 0)
            {
                Context.GameConfig.GameWonByGammon = true;
                return multiplier = 2;
            }

            return multiplier;
        }

    }
}