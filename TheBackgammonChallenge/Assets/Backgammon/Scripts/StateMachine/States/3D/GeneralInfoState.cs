using UnityEngine;

namespace Backgammon
{
	public class GeneralInfoState : GameState
	{
        public GeneralInfoState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState()
        {
            if (!_postShownDisableInfoPanelForShortTimeDelay) Context.GeneralInfoUI.SetActive(true);

            if (Context.BeginTurnDialog)
            {
                var colour = Context.IsPlayersTurn ? (Context.IfPlayerIsBlack ? "BLACK" : "WHITE") :
                                                     (Context.IfPlayerIsBlack ? "WHITE" : "BLACK");

                Context.GeneralInfoUI.SetGeneralText(colour + " MOVES");
            }
            else if (Context.PlayerIsUnableToMove)
            {
                var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
                Context.GeneralInfoUI.SetGeneralText(player + " is blocked from moving");
            }
            else if (Context.PlayoutProMoves)
            {
                Context.GeneralInfoUI.SetGeneralText("PROS ORIGINAL MOVE");
            }
            else if (_postShownDisableInfoPanelForShortTimeDelay)
            {
                _delayTimer = _shortTimeDelay;
            }
            else if (!Context.PlayerMoveEvaluated)
            {
                var evaluation = Context.PlayerMatchedRankThisTurn == 1 ? "EXCELLENT" :
                                 Context.PlayerMatchedRankThisTurn == 2 ? "GOOD MOVE" :
                                 Context.PlayerMatchedRankThisTurn == 3 ? "NOT BAD" : string.Empty;

                Context.GeneralInfoUI.SetGeneralText($"{evaluation}" + "\n" + "#RANK " + $"{Context.PlayerMatchedRankThisTurn}");
                Context.GameScreenUI.SetPlayerTopMatches(Context.PlayerTopRankedThisGame);

                if (Context.PlayerMatchedRankThisTurn == 1)
                    Context.GameScreenUI.SetAndAnimatePlayerRank("#RANK " + Context.PlayerMatchedRankThisTurn.ToString());
            }
            else if (!Context.PlayerProMoveEvaluated)
            {
                Context.GeneralInfoUI.SetGeneralText((Context.PlayerMatchedProMove ? "MATCHED" : "DIDN'T MATCH") +
                                                     "\n" + "PRO #RANK " + $"{Context.ProMatchedRankThisTurn}");
                Context.GameScreenUI.SetProPlayerTopMatches(Context.ProTopRankedThisGame);

                if (Context.ProMatchedRankThisTurn == 1)
                    Context.GameScreenUI.SetAndAnimateProPlayerRank("#RANK " + Context.ProMatchedRankThisTurn.ToString());
            }
            else if (!Context.OpponentMoveEvaluated)
            {
                var opponent = Context.IfPlayer1Turn ? Context.SelectedMatch.Player2 : Context.SelectedMatch.Player1;

                Context.GeneralInfoUI.SetGeneralText($"{Context.SelectedMatch.Player1}" +
                                                        "\n" + "#RANK " + $"{Context.OpponentMatchedRankThisTurn}");
                Context.GameScreenUI.SetOpponentTopMatches(Context.OpponentTopRankedThisGame);

                if (Context.OpponentMatchedRankThisTurn == 1)
                    Context.GameScreenUI.SetAndAnimateOpponentRank("#RANK " + Context.OpponentMatchedRankThisTurn.ToString());

                _timeDelay = _extendedTimeDelay;
            }
        }

        public override void UpdateState()
        {
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                Context.GeneralInfoUI.SetActive(false);

                _delayTimer = _timeDelay;

                if (Context.BeginTurnDialog)
                {
                    Context.BeginTurnDialog = false;
                    ActiveState = GameStateMachine.EGameState.RollDice;
                }
                if (Context.PlayerIsUnableToMove)
                {
                    ActiveState = GameStateMachine.EGameState.TurnEnd;
                }
                else if (Context.PlayoutProMoves)
                {
                    Context.PlayoutProMoves = false;
                    ActiveState = GameStateMachine.EGameState.PlayoutProMoves;
                }
                else if (_postShownDisableInfoPanelForShortTimeDelay)
                {
                    _postShownDisableInfoPanelForShortTimeDelay = false;
                    EnterState();
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
                    ActiveState = GameStateMachine.EGameState.Analysis;
                }
                else if (!Context.OpponentMoveEvaluated)
                {
                    Context.OpponentMoveEvaluated = true;
                    ActiveState = GameStateMachine.EGameState.TurnEnd;
                }
            }

            if (Context.IfFastForwarding)
            {
                Context.GeneralInfoUI.SetActive(false);
                ActiveState = GameStateMachine.EGameState.TurnEnd;
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        private float _delayTimer = 2.0f;
        private float _timeDelay = 2.0f;
        private float _extendedTimeDelay = 4.0f;

        private float _shortTimeDelay = 1.0f; 
        private bool _postShownDisableInfoPanelForShortTimeDelay = false;
    }
}