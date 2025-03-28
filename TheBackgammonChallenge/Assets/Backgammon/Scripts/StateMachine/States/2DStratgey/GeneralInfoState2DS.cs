using UnityEngine;
using static Backgammon.GeneralInfoState2D;

namespace Backgammon
{
    public class GeneralInfoState2DS : GameState2DStrategy
    {
        public GeneralInfoState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        private static float _shortTimeDelay = 1.0f;
        private static float _normalTimeDelay = 3.0f;
        private static float _extendedTimeDelay = 4.0f;
        private static float _delayTimer = _normalTimeDelay;

        private bool _postShownDisableInfoPanelForShortTimeDelay = false;
        private bool _bailOutAndReEnterState = false;
        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: GENERAL INFO");

            if (!_postShownDisableInfoPanelForShortTimeDelay) Context.GeneralInfoUI.SetActive(true);

            Context.GeneralInfoUI.SetPlayerReplayMoveOptionActive(false);

            if (_postShownDisableInfoPanelForShortTimeDelay)
            {
                _delayTimer = _shortTimeDelay;
                goto bailOut;
            }

            switch (Context.TurnConfig.GeneralInfoStateSwitch)
            {
                case EGeneralInfoState2D.PlayerUnableToMove:
                    {
                        _ = Context.Strategy.GeneralInfoEnter(Context, EGeneralInfoState2D.PlayerUnableToMove);
                        
                        // CLEAR UP U.I.
                        Context.DiceRollsUI.SetOpponentDiceRollsText(string.Empty, 0, 0, false);
                    }
                    break;
                case EGeneralInfoState2D.EvaluatePlayerMovesNoAIData:
                    {
                        var matched = Context.TurnConfig.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH";
                        var text = "YOU " + matched + " THE PRO MOVE";
                        text += "\n\nNO A.I. DATA AVAILABLE FOR RANK ANALYSIS";

                        _delayTimer = _extendedTimeDelay;

                        Context.GeneralInfoUI.SetGeneralText(text);
                    }
                    break;
                case EGeneralInfoState2D.EvaluatePlayerMoves:
                    {
                        // TEST FOR FORCED MOVES
                        var playerRank = Context.TurnConfig.PlayerMatchedRankThisTurn;
                        var comingInFromBar = Context.TurnConfig.PlayerMovesInfo.Length == 1 && Context.TurnConfig.PlayerMovesInfo[0].pointFrom == 25;
                        var singleRankedMove = Context.TurnConfig.AIRankedMoves.Length == 1;
                        var singlePossibleMove = Context.TurnConfig.TotalMovesThisTurn == 1;
                        var forcedMove = false;

                        if ((comingInFromBar || singleRankedMove || singlePossibleMove) && !Context.TurnConfig.IfBearingOff)
                        {
                            forcedMove = true;
                        }

                        // POPUPS IF NOT FORCED
                        if (!forcedMove)
                        {
                            var rank = Context.TurnConfig.PlayerMatchedRankThisTurn > 0 ? (Context.TurnConfig.PlayerMatchedRankThisTurn - 1) :
                                                                               (Context.TurnConfig.AIRankedMoves.Length - 1);

                            var upperProb = Context.TurnConfig.AIRankedMoves[0].probabilities.redWin;
                            var lowerProb = Context.TurnConfig.AIRankedMoves[rank].probabilities.redWin;
                            Context.TurnConfig.LostEquity = (upperProb - lowerProb) * 100f;

                            var replayMove = string.Empty;

                            var evaluation = playerRank == 1 ? "EXCELLENT\n" :
                                             playerRank == 2 ? "GOOD MOVE\n" :
                                             playerRank == 3 ? "NOT BAD\n" : string.Empty;

                            // OPTION TO REPLAY THE MOVE IF RANK 4< && EQUITY LOSS WAS >5%
                            if ((playerRank > 3 || playerRank == 0) && Context.TurnConfig.LostEquity >= 5)
                            {
                                evaluation = "Your move was ";
                                replayMove = $"You dropped {Context.TurnConfig.LostEquity:0.00}% chance to win.\n Would you like to replay the move?";

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
                            if (Context.TurnConfig.AIRankedMoves.Length == 1)
                            {
                                _delayTimer = -1;
                                Context.GeneralInfoUI.SetActive(false);
                            }
                        }

                        Context.GameScreenUI.SetPlayerTopMatches(Context.GameStats.PlayerTopRankedThisGame);
                    }
                    break;
                case EGeneralInfoState2D.EvaluateProMoves:
                    {
                        // TEST FOR FORCED MOVES
                        var playerRank = Context.TurnConfig.PlayerMatchedRankThisTurn;
                        var comingInFromBar = Context.TurnConfig.PlayerMovesInfo.Length == 1 && Context.TurnConfig.PlayerMovesInfo[0].pointFrom == 25;
                        var singleRankedMove = Context.TurnConfig.AIRankedMoves.Length == 1;
                        var singlePossibleMove = Context.TurnConfig.TotalMovesThisTurn == 1;
                        var forcedMove = false;

                        if (((comingInFromBar || singleRankedMove || singlePossibleMove) && !Context.TurnConfig.IfBearingOff) ||
                            (singleRankedMove && Context.TurnConfig.IfBearingOff))
                        {
                            forcedMove = true;
                        }

                        // POPUPS IF NOT FORCED
                        if (!forcedMove)
                        {
                            var better = (playerRank < Context.TurnConfig.ProMatchedRankThisTurn && playerRank != 0) ||
                                         (Context.TurnConfig.ProMatchedRankThisTurn == 0 && playerRank != 0);
                            var matchedBetter = better ? "EXCEPTIONAL!!\n\n" : string.Empty;
                            var matchedText = better ? "BEAT" : (Context.TurnConfig.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH");

                            Context.GeneralInfoUI.SetGeneralText(matchedBetter +
                                                                 "YOU " +
                                                                 matchedText +
                                                                 "\n" + "THE PROS ORIGINAL\n" +
                                                                 "#RANK " + $"{Context.TurnConfig.ProMatchedRankThisTurn} MOVE.");

                            if (Context.TurnConfig.ProMatchedRankThisTurn == 1)
                                Context.GameScreenUI.SetAndAnimateProPlayerRank("#RANK " + Context.TurnConfig.ProMatchedRankThisTurn.ToString());
                        }
                        else
                        {
                            _delayTimer = -1;
                            Context.GeneralInfoUI.SetActive(false);
                        }

                        Context.GameScreenUI.SetProPlayerTopMatches(Context.GameStats.ProTopRankedThisGame);
                    }
                    break;
                case EGeneralInfoState2D.EvaluateOpponentMoves:
                    {
                        var forcedMove = false;

                        // TEST FOR FORCED MOVES
                        if (Context.TurnConfig.OpponentMovesInfo.Length == 1 && Context.TurnConfig.OpponentMovesInfo[0].pointFrom == 25 ||
                            Context.TurnConfig.AIRankedMoves.Length == 1 || Context.TurnConfig.TotalMovesThisTurn == 1)
                        {
                            forcedMove = true;
                        }

                        // POPUPS IF NOT FORCED
                        if (!forcedMove)
                        {
                            var opponent = Context.Strategy.GeneralInfoEnter(Context, EGeneralInfoState2D.EvaluateOpponentMoves);

                            Context.GeneralInfoUI.SetGeneralText($"{opponent}" + "\n" + "#RANK " + $"{Context.TurnConfig.OpponentMatchedRankThisTurn}");

                            if (Context.TurnConfig.OpponentMatchedRankThisTurn == 1)
                                Context.GameScreenUI.SetAndAnimateOpponentRank("#RANK " + Context.TurnConfig.OpponentMatchedRankThisTurn.ToString());

                            _delayTimer = _extendedTimeDelay;
                        }
                        else
                        {
                            _delayTimer = -1;
                            Context.GeneralInfoUI.SetActive(false);
                        }

                        Context.GameScreenUI.SetOpponentTopMatches(Context.GameStats.OpponentTopRankedThisGame);
                    }
                    break;
                case EGeneralInfoState2D.PlayoutProMoves:
                    {
                        _ = Context.Strategy.GeneralInfoEnter(Context, EGeneralInfoState2D.PlayoutProMoves);
                    }
                    break;
                case EGeneralInfoState2D.PlayerConcedes:
                    {
                        var player = Context.TurnConfig.ConcedeTheGame ? "You" : (Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2);
                        var concedes = Context.TurnConfig.ConcedeTheGame ? "concede" : "concedes";
                        var gameOrMatch = Context.GameConfig.IndexGame + 1 <= Context.GameConfig.SelectedMatch.GameCount;

                        Context.GeneralInfoUI.SetGeneralText($"{player} {concedes} the {(gameOrMatch ? "Game" : "Match")}");

                        _delayTimer = _extendedTimeDelay;
                    }
                    break;
                case EGeneralInfoState2D.GameWon:
                    {
                        var player = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 : Context.GameConfig.SelectedMatch.Player2;

                        var points = Context.TurnConfig.IfPlayer1Turn ? (Context.GameConfig.SelectedGame.Player1PointsAtEnd - Context.GameConfig.SelectedGame.Player1PointsAtStart) :
                                                            (Context.GameConfig.SelectedGame.Player2PointsAtEnd - Context.GameConfig.SelectedGame.Player2PointsAtStart);

                        var text = string.Empty;
                        text += Context.GameConfig.GameWonByBackGammon ? "BackGammon!!\n\n" : string.Empty;
                        text += Context.GameConfig.GameWonByGammon ? "Gammon!\n\n" : string.Empty;
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
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            _delayTimer -= Time.deltaTime;

            // IF PLAYER IS < RANK 4 - REPLAY THE MOVE INTERACTION
            if (Context.GeneralInfoUI.IfAwaitingPlayerInteraction)
            {
                if (Context.GeneralInfoUI.IfReplayMove)
                {
                    _delayTimer = _normalTimeDelay;

                    Context.TurnConfig.ReplayPlayerMove = true;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RestoreBoard;

                    Context.GeneralInfoUI.SetActive(false);
                }
                else if (Context.GeneralInfoUI.IfContinue)
                {
                    Context.GeneralInfoUI.SetActive(false);
                    Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluateProMoves;
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

                    var state = Context.Strategy.GeneralInfoUpdate(Context);

                    if (state == StateKey)
                    {
                        _bailOutAndReEnterState = true;
                        goto bailOut;
                    }
                    else
                    {
                        ActiveState = state;
                    }
                }

                switch (Context.TurnConfig.GeneralInfoStateSwitch)
                {
                    case EGeneralInfoState2D.PlayerUnableToMove:
                        {
                            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluatePlayerMovesNoAIData:
                        {
                            if (!Context.TurnConfig.PlayerMatchedProMove)
                                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RestoreBoard;
                            else
                                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluatePlayerMoves:
                        {
                            Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.EvaluateProMoves;
                            _postShownDisableInfoPanelForShortTimeDelay = true;
                            _bailOutAndReEnterState = true;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluateProMoves:
                        {
                            if (!Context.TurnConfig.PlayerMatchedProMove)
                                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RestoreBoard;
                            else
                                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.AnalysisOrContinue;
                        }
                        break;
                    case EGeneralInfoState2D.EvaluateOpponentMoves:
                        {
                            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.PlayoutProMoves:
                        {
                            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.PlayoutProMoves;
                        }
                        break;
                    case EGeneralInfoState2D.PlayerConcedes:
                        {
                            if (Context.TurnConfig.ConcedeTheGame) Context.ExitFromStateMachine = true;
                            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                        }
                        break;
                    case EGeneralInfoState2D.GameWon:
                        {
                            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GameWon;
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
                            Context.TurnConfig.AIDataRequestBailOutCounter = 0;
                            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.AIData;

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

            if (Context.GameConfig.IfFastForwarding)
            {
                Context.GeneralInfoUI.SetActive(false);
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}