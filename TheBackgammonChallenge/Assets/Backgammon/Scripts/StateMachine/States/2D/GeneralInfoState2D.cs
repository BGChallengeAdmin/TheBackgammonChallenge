using UnityEngine;

namespace Backgammon
{
    public class GeneralInfoState2D : GameState2D
    {
        public GeneralInfoState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: GENERAL INFO");

            if (!_postShownDisableInfoPanelForShortTimeDelay) Context.GeneralInfoUI.SetActive(true);

            Context.GeneralInfoUI.SetPlayerReplayMoveOptionActive(false);

            if (_postShownDisableInfoPanelForShortTimeDelay)
            {
                _delayTimer = _shortTimeDelay;
                goto bailOut;
            }

            switch (Context.GeneralInfoStateSwitch)
            {
                case EGeneralInfoState2D.PlayerUnableToMove:
                    {
                        var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
                        Context.GeneralInfoUI.SetGeneralText(player + " is blocked from moving");

                        if (Main.Instance.IfPlayerVsAI && Context.IsPlayersTurn)
                            Context.GeneralInfoUI.SetGeneralText(player + " are blocked from moving");

                        // CLEAR UP U.I.
                        Context.DiceRollsUI.SetOpponentDiceRollsText(string.Empty, 0, 0, false);
                    }
                break;
                case EGeneralInfoState2D.EvaluatePlayerMovesNoAIData:
                    {
                        var matched = Context.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH";
                        var text = "YOU " + matched + " THE PRO MOVE";
                        text += "\n\nNO A.I. DATA AVAILABLE FOR RANK ANALYSIS";

                        _delayTimer = _extendedTimeDelay;

                        Context.GeneralInfoUI.SetGeneralText(text);
                    }
                    break;
                case EGeneralInfoState2D.EvaluatePlayerMoves:
                    {
                        // TEST FOR FORCED MOVES
                        var playerRank = Context.PlayerMatchedRankThisTurn;
                        var comingInFromBar = Context.PlayerMovesInfo.Length == 1 && Context.PlayerMovesInfo[0].pointFrom == 25;
                        var singleRankedMove = Context.AIRankedMoves.Length == 1;
                        var singlePossibleMove = Context.TotalMovesThisTurn == 1;
                        var forcedMove = false;

                        if ((comingInFromBar || singleRankedMove || singlePossibleMove) && !Context.IfBearingOff)
                        {
                            forcedMove = true;
                        }

                        // POPUPS IF NOT FORCED
                        if (!forcedMove)
                        {
                            var rank = Context.PlayerMatchedRankThisTurn > 0 ? (Context.PlayerMatchedRankThisTurn - 1) :
                                                                               (Context.AIRankedMoves.Length - 1);

                            var upperProb = Context.AIRankedMoves[0].probabilities.redWin;
                            var lowerProb = Context.AIRankedMoves[rank].probabilities.redWin;
                            Context.LostEquity = (upperProb - lowerProb) * 100f;

                            var replayMove = string.Empty;

                            var evaluation = playerRank == 1 ? "EXCELLENT\n" :
                                             playerRank == 2 ? "GOOD MOVE\n" :
                                             playerRank == 3 ? "NOT BAD\n" : string.Empty;

                            // OPTION TO REPLAY THE MOVE IF RANK 4< && EQUITY LOSS WAS >5%
                            if ((playerRank > 3 || playerRank == 0) && Context.LostEquity >= 5)
                            {
                                evaluation = "Your move was ";
                                replayMove = $"You dropped {Context.LostEquity:0.00}% chance to win.\n Would you like to replay the move?";

                                Context.GeneralInfoUI.SetPlayerReplayMoveOptionActive(true);
                            }

                            Context.GeneralInfoUI.SetGeneralText($"{evaluation}" +
                                                                 $"{(playerRank == 0 ? "UNRANKED" : "#RANK ")}" +
                                                                 $"{(playerRank == 0 ? string.Empty : playerRank)} " +
                                                                 $"{replayMove}");

                            if (playerRank == 1) Context.GameScreenUI.SetAndAnimatePlayerRank("#RANK " + playerRank.ToString());
                        }
                        else
                        {
                            Context.GeneralInfoUI.SetGeneralText("No more moves");
                            if (Context.AIRankedMoves.Length == 1)
                            {
                                _delayTimer = -1;
                                Context.GeneralInfoUI.SetActive(false);
                            }
                        }

                        Context.GameScreenUI.SetPlayerTopMatches(Context.PlayerTopRankedThisGame);
                    }
                    break;
                case EGeneralInfoState2D.EvaluateProMoves:
                    {
                        // TEST FOR FORCED MOVES
                        var playerRank = Context.PlayerMatchedRankThisTurn;
                        var comingInFromBar = Context.PlayerMovesInfo.Length == 1 && Context.PlayerMovesInfo[0].pointFrom == 25;
                        var singleRankedMove = Context.AIRankedMoves.Length == 1;
                        var singlePossibleMove = Context.TotalMovesThisTurn == 1;
                        var forcedMove = false;

                        if (((comingInFromBar || singleRankedMove || singlePossibleMove) && !Context.IfBearingOff) ||
                            (singleRankedMove && Context.IfBearingOff))
                        {
                            forcedMove = true;
                        }

                        // POPUPS IF NOT FORCED
                        if (!forcedMove)
                        {
                            var better = (playerRank < Context.ProMatchedRankThisTurn && playerRank != 0) ||
                                         (Context.ProMatchedRankThisTurn == 0 && playerRank != 0);
                            var matchedBetter = better ? "EXCEPTIONAL!!\n\n" : string.Empty;
                            var matchedText = better ? "BEAT" : (Context.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH");

                            Context.GeneralInfoUI.SetGeneralText(matchedBetter +
                                                                 "YOU " +
                                                                 matchedText +
                                                                 "\n" + "THE PROS ORIGINAL\n" +
                                                                 "#RANK " + $"{Context.ProMatchedRankThisTurn} MOVE.");

                            if (Context.ProMatchedRankThisTurn == 1)
                                Context.GameScreenUI.SetAndAnimateProPlayerRank("#RANK " + Context.ProMatchedRankThisTurn.ToString());
                        }
                        else
                        {
                            _delayTimer = -1;
                            Context.GeneralInfoUI.SetActive(false);
                        }

                        Context.GameScreenUI.SetProPlayerTopMatches(Context.ProTopRankedThisGame);
                    }
                    break;
                case EGeneralInfoState2D.EvaluateOpponentMoves:
                    {
                        var forcedMove = false;

                        // TEST FOR FORCED MOVES
                        if (Context.OpponentMovesInfo.Length == 1 && Context.OpponentMovesInfo[0].pointFrom == 25 ||
                            Context.AIRankedMoves.Length == 1 || Context.TotalMovesThisTurn == 1)
                        {
                            forcedMove = true;
                        }

                        // POPUPS IF NOT FORCED
                        if (!forcedMove)
                        {
                            var opponent = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
                            if (Main.Instance.IfPlayerVsAI) opponent = Context.SelectedMatch.Player2;

                            Context.GeneralInfoUI.SetGeneralText($"{opponent}" + "\n" + "#RANK " + $"{Context.OpponentMatchedRankThisTurn}");

                            if (Context.OpponentMatchedRankThisTurn == 1)
                                Context.GameScreenUI.SetAndAnimateOpponentRank("#RANK " + Context.OpponentMatchedRankThisTurn.ToString());

                            _delayTimer = _extendedTimeDelay;
                        }
                        else
                        {
                            _delayTimer = -1;
                            Context.GeneralInfoUI.SetActive(false);
                        }

                        Context.GameScreenUI.SetOpponentTopMatches(Context.OpponentTopRankedThisGame);
                    }
                    break;
                case EGeneralInfoState2D.PlayoutProMoves:
                    {
                        Context.GeneralInfoUI.SetGeneralText("THIS WAS THE ORIGINAL PRO MOVE");

                        if (Main.Instance.IfPlayerVsAI) Context.GeneralInfoUI.SetGeneralText("REPLAYING YOUR MOVE");
                    }
                    break;
                case EGeneralInfoState2D.PlayerConcedes:
                    {
                        var player = Context.ConcedeTheGame ? "You" : (Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2);
                        var concedes = Context.ConcedeTheGame ? "concede" : "concedes";
                        var gameOrMatch = Context.IndexGame + 1 <= Context.SelectedMatch.GameCount;

                        Context.GeneralInfoUI.SetGeneralText($"{player} {concedes} the {(gameOrMatch ? "Game" : "Match")}");

                        _delayTimer = _extendedTimeDelay;
                    }
                    break;
                case EGeneralInfoState2D.GameWon:
                    {
                        var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;

                        var points = Context.IfPlayer1Turn ? (Context.SelectedGame.Player1PointsAtEnd - Context.SelectedGame.Player1PointsAtStart) :
                                                            (Context.SelectedGame.Player2PointsAtEnd - Context.SelectedGame.Player2PointsAtStart);

                        var text = string.Empty;
                        text += Context.GameWonByBackGammon ? "BackGammon!!\n\n" : string.Empty;
                        text += Context.GameWonByGammon ? "Gammon!\n\n" : string.Empty;
                        text += $"Game won by {player} for {points} Pts.";

                        Context.GeneralInfoUI.SetGeneralText(text);
                    }
                    break;
                case EGeneralInfoState2D.AIDataRequestFailed:
                    {
                        var text = "Sorry.\nSomething has gone wrong.\n\n" +
                            "Your game has been saved, please try again later.";

                        _delayTimer = _extendedTimeDelay;

                        // DEBUG - A.I. DATA STATE TIMEOUT

                        text = $"A.I. Data response\n\n";
                        text += Context.AIDataHandler.AIServerMessage();
                        Context.GameScreenUI.SetActive(false);
                        Context.GeneralInfoUI.SetDebugContinueButtonActive(true);
                        _delayTimer = -1;

                        // END DEBUG

                        Context.GeneralInfoUI.SetGeneralText(text);
                    }
                    break;
                case EGeneralInfoState2D.None:
                    {
                        Context.GeneralInfoUI.SetGeneralText("ERROR\n\nGENERAL INFO - HAS NO STATE");
                    }
                    break;
            }

            bailOut:;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            _delayTimer -= Time.deltaTime;

            // IF PLAYER IS < RANK 4 - REPLAY THE MOVE INTERACTION
            if (Context.GeneralInfoUI.IfAwaitingPlayerInteraction)
            {
                if (Context.GeneralInfoUI.IfReplayMove)
                {
                    _delayTimer = _normalTimeDelay;

                    Context.ReplayPlayerMove = true;
                    ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;

                    Context.GeneralInfoUI.SetActive(false);
                }
                else if (Context.GeneralInfoUI.IfContinue)
                {
                    Context.GeneralInfoUI.SetActive(false);
                    Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluateProMoves;
                    _postShownDisableInfoPanelForShortTimeDelay = true;
                    _bailOutAndReEnterState = true;
                    goto bailOut;
                }
            }
            // NORMAL FUNCTION OF GENRAL INFO PANEL
            else if (_delayTimer <= 0)
            {
                _delayTimer = _normalTimeDelay;
                Context.GeneralInfoUI.SetActive(false);

                #region DEBUG
                //if (Context.AIDataRequestBailOutCounter > 0)
                //{
                //    //Context.ExitFromStateMachine = true;

                //    // DEBUG - A.I. DATA TIMEOUT
                //    Context.GeneralInfoUI.SetActive(true);
                //    _delayTimer = -1;
                //    if (!Context.GeneralInfoUI.IfDebugContinue) return;

                //    Context.GameScreenUI.SetActive(true);
                //    Context.GeneralInfoUI.SetActive(false);
                //    Context.GeneralInfoUI.SetDebugContinueButtonActive(false);
                //    Context.AIDataRequestBailOutCounter = 0;
                //    ActiveState = GameStateMachine2D.EGameState2D.AIData;

                //    // END DEBUG
                //}
                //else if (Context.BeginTurnDialog)
                //{
                //    Context.BeginTurnDialog = false;
                //    ActiveState = GameStateMachine2D.EGameState2D.RollDice;
                //}
                //else if (Context.GameWon)
                //{
                //    ActiveState = GameStateMachine2D.EGameState2D.GameWon;
                //}
                //else if (Context.PlayerIsUnableToMove || Context.PlayerConcedes)
                //{
                //    ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                //}
                //else if (Context.PlayoutProMoves)
                //{
                //    ActiveState = GameStateMachine2D.EGameState2D.PlayoutProMoves;
                //}
                //else if (!Context.AIDataAvailable)
                //{
                //    if (!Context.PlayerMatchedProMove)
                //        ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                //    else
                //        ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                //}
                //else if (_postShownDisableInfoPanelForShortTimeDelay)
                //{
                //    _postShownDisableInfoPanelForShortTimeDelay = false;
                //    Context.GeneralInfoStateSwitch = GeneralInfoState2D.GeneralInfoState2DEnum.EvaluateProMoves;

                //    if (!Main.Instance.IfPlayerVsAI) EnterState();
                //    else
                //    {
                //        Context.PlayerProMoveEvaluated = true;
                //        ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;
                //    }
                //}
                //else if (!Context.PlayerMoveEvaluated)
                //{
                //    Context.PlayerMoveEvaluated = true;
                //    _postShownDisableInfoPanelForShortTimeDelay = true;
                //    EnterState();
                //}
                //else if (!Context.PlayerProMoveEvaluated)
                //{
                //    Context.PlayerProMoveEvaluated = true;

                //    if (!Context.PlayerMatchedProMove)
                //        ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                //    else
                //        ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;
                //}
                //else if (!Context.OpponentMoveEvaluated)
                //{
                //    Context.OpponentMoveEvaluated = true;
                //    ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                //}

                #endregion

                // ADDS A SHORT TIME DELAY AFTER INFO PANEL SHOWN
                if (_postShownDisableInfoPanelForShortTimeDelay)
                {
                    Context.GeneralInfoUI.SetActive(false);
                    _postShownDisableInfoPanelForShortTimeDelay = false;

                    if (!Main.Instance.IfPlayerVsAI)
                    {
                        _bailOutAndReEnterState = true;
                        goto bailOut;
                    }
                    else
                    {
                        ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;
                    }
                }
                
                switch (Context.GeneralInfoStateSwitch)
                {
                    case EGeneralInfoState2D.PlayerUnableToMove:
                        {
                            ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluatePlayerMovesNoAIData:
                        {
                            if (!Context.PlayerMatchedProMove)
                                ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                            else
                                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluatePlayerMoves:
                        {
                            Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluateProMoves;
                            _postShownDisableInfoPanelForShortTimeDelay = true;
                            _bailOutAndReEnterState = true;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluateProMoves:
                        {
                            if (!Context.PlayerMatchedProMove)
                                ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                            else
                                ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluateOpponentMoves:
                        {
                            ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.PlayoutProMoves:
                        {
                            ActiveState = GameStateMachine2D.EGameState2D.PlayoutProMoves;
                        }
                        break;
                    case EGeneralInfoState2D.PlayerConcedes:
                        {
                            if (Context.ConcedeTheGame) Context.ExitFromStateMachine = true;
                            ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.GameWon:
                        {
                            ActiveState = GameStateMachine2D.EGameState2D.GameWon;
                        }
                        break;
                    case EGeneralInfoState2D.AIDataRequestFailed:
                        {
                            //Context.ExitFromStateMachine = true;

                            // DEBUG - A.I. DATA TIMEOUT
                            Context.GeneralInfoUI.SetActive(true);
                            _delayTimer = -1;
                            if (!Context.GeneralInfoUI.IfDebugContinue) return;

                            Context.GameScreenUI.SetActive(true);
                            Context.GeneralInfoUI.SetActive(false);
                            Context.GeneralInfoUI.SetDebugContinueButtonActive(false);
                            Context.AIDataRequestBailOutCounter = 0;
                            ActiveState = GameStateMachine2D.EGameState2D.AIData;

                            // END DEBUG
                        }
                        break;
                    case EGeneralInfoState2D.None:
                        {
                            //Context.ExitFromStateMachine = true;
                        }
                        break;
                }
            }

            bailOut:;

            if (_bailOutAndReEnterState)
            {
                _bailOutAndReEnterState = false;
                EnterState();
            }

            if (Context.IfFastForwarding)
            {
                Context.GeneralInfoUI.SetActive(false);
                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private static float _shortTimeDelay = 1.0f;
        private static float _normalTimeDelay = 3.0f;
        private static float _extendedTimeDelay = 4.0f;
        private static float _delayTimer = _normalTimeDelay;

        private bool _postShownDisableInfoPanelForShortTimeDelay = false;
        private bool _bailOutAndReEnterState = false;

        public enum EGeneralInfoState2D
        {
            None,
            PlayerUnableToMove,
            EvaluatePlayerMovesNoAIData,
            EvaluatePlayerMoves,
            EvaluateProMoves,
            EvaluateOpponentMoves,
            PlayoutProMoves,
            PlayerConcedes,
            GameWon,
            AIDataRequestFailed
        }
    }
}