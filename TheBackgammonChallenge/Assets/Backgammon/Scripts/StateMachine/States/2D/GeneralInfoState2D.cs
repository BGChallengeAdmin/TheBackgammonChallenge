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

            if (Context.AIDataRequestBailOutCounter > 0)
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
            else if (Context.BeginTurnDialog)
            {
                var colour = Context.IsPlayersTurn ? (Context.IfPlayerIsBlack ? "BLACK" : "WHITE") :
                                                     (Context.IfPlayerIsBlack ? "WHITE" : "BLACK");

                Context.GeneralInfoUI.SetGeneralText(colour + " MOVES");
            }
            else if (Context.GameWon)
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
            else if (Context.PlayerConcedes)
            {
                var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
                var concedes = Context.IsPlayersTurn ? "concede" : "concedes";
                var gameOrMatch = Context.IndexGame + 1 <= Context.SelectedMatch.GameCount;

                Context.GeneralInfoUI.SetGeneralText($"{player} {concedes} the {(gameOrMatch ? "Game" : "Match")}");

                _delayTimer = _extendedTimeDelay;
            }
            else if (Context.PlayerIsUnableToMove)
            {
                var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
                Context.GeneralInfoUI.SetGeneralText(player + " is blocked from moving");

                if (Main.Instance.IfPlayerVsAI && Context.IsPlayersTurn)
                    Context.GeneralInfoUI.SetGeneralText(player + " are blocked from moving");

                // CLEAR UP U.I.
                Context.DiceRollsUI.SetOpponentDiceRollsText(string.Empty, 0, 0, false);
            }
            else if (Context.PlayoutProMoves)
            {
                Context.GeneralInfoUI.SetGeneralText("THIS WAS THE ORIGINAL PRO MOVE");

                if (Main.Instance.IfPlayerVsAI) Context.GeneralInfoUI.SetGeneralText("REPLAYING YOUR MOVE");
            }
            else if (!Context.AIDataAvailable)
            {
                var matched = Context.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH";
                var text = "YOU " + matched + " THE PRO MOVE";
                text += "\n\nNO A.I. DATA AVAILABLE FOR RANK ANALYSIS";

                _delayTimer = _extendedTimeDelay;

                Context.GeneralInfoUI.SetGeneralText(text);
            }
            else if (_postShownDisableInfoPanelForShortTimeDelay)
            {
                _delayTimer = _shortTimeDelay;
            }
            else if (!Context.PlayerMoveEvaluated)
            {
                // TEST FOR FORCED MOVES
                var playerRank = Context.PlayerMatchedRankThisTurn;
                var forcedMove = false;

                if ((Context.PlayerMovesInfo.Length == 1 && Context.PlayerMovesInfo[0].pointFrom == 25 ||
                    Context.AIRankedMoves.Length == 1 || Context.TotalMovesThisTurn == 1) && 
                    !Context.IfBearingOff)
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
            else if (!Context.PlayerProMoveEvaluated && !Main.Instance.IfPlayerVsAI)
            {
                // TEST FOR FORCED MOVES
                var playerRank = Context.PlayerMatchedRankThisTurn;
                var forcedMove = false;

                if (Context.PlayerMovesInfo.Length == 1 && Context.PlayerMovesInfo[0].pointFrom == 25 ||
                    Context.AIRankedMoves.Length == 1 || Context.TotalMovesThisTurn == 1)
                {
                    forcedMove = true;
                }

                // POPUPS IF NOT FORCED
                if (!forcedMove)
                {
                    var better = (playerRank < Context.ProMatchedRankThisTurn && playerRank != 0);
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
            else if (!Context.OpponentMoveEvaluated)
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
                    var opponent = Context.IfPlayer1Turn ? Context.SelectedMatch.Player2 : Context.SelectedMatch.Player1;
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
                    Context.PlayerMoveEvaluated = true;
                    _postShownDisableInfoPanelForShortTimeDelay = true;
                    EnterState();

                    Context.GeneralInfoUI.SetActive(false);
                }
            }
            // NORMAL FUNCTION OF GENRAL INFO PANEL
            else if (_delayTimer <= 0)
            {
                Context.GeneralInfoUI.SetActive(false);

                _delayTimer = _normalTimeDelay;

                if (Context.AIDataRequestBailOutCounter > 0)
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
                else if (Context.BeginTurnDialog)
                {
                    Context.BeginTurnDialog = false;
                    ActiveState = GameStateMachine2D.EGameState2D.RollDice;
                }
                else if (Context.GameWon)
                {
                    ActiveState = GameStateMachine2D.EGameState2D.GameWon;
                }
                else if (Context.PlayerIsUnableToMove || Context.PlayerConcedes)
                {
                    ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                }
                else if (Context.PlayoutProMoves)
                {
                    ActiveState = GameStateMachine2D.EGameState2D.PlayoutProMoves;
                }
                else if (!Context.AIDataAvailable)
                {
                    if (!Context.PlayerMatchedProMove)
                        ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                    else
                        ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                }
                else if (_postShownDisableInfoPanelForShortTimeDelay)
                {
                    _postShownDisableInfoPanelForShortTimeDelay = false;

                    if (!Main.Instance.IfPlayerVsAI) EnterState();
                    else
                    {
                        Context.PlayerProMoveEvaluated = true;
                        ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;
                    }
                }
                else if (!Context.PlayerMoveEvaluated)
                {
                    Context.PlayerMoveEvaluated = true;
                    _postShownDisableInfoPanelForShortTimeDelay = true;
                    EnterState();
                }
                else if (!Context.PlayerProMoveEvaluated)
                {
                    Context.PlayerProMoveEvaluated = true;

                    if (!Context.PlayerMatchedProMove)
                        ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                    else
                        ActiveState = GameStateMachine2D.EGameState2D.AnalysisOrContinue;
                }
                else if (!Context.OpponentMoveEvaluated)
                {
                    Context.OpponentMoveEvaluated = true;
                    ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                }
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

        private float _delayTimer = 1.5f;

        private float _shortTimeDelay = 1.0f;
        private float _normalTimeDelay = 3.0f;
        private float _extendedTimeDelay = 4.0f;

        private bool _postShownDisableInfoPanelForShortTimeDelay = false;
    }
}