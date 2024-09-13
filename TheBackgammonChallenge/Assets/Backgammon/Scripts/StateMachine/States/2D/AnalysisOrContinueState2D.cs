using UnityEngine;

namespace Backgammon
{
    public class AnalysisOrContinueState2D : GameState2D
    {
        public AnalysisOrContinueState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: ANALYSIS OR CONTINUE");

            Context.AnalysisOrContinueUI.SetActive(true);

            // ADD MARKERS TO INDICATE RANK 1 
            //if (Context.PlayerMatchedRankThisTurn == 1)
            //    Context.AnalysisOrContinueUI.SetMarkersToIndicateRank1(true);
            //if (Context.ProMatchedRankThisTurn == 1)
            //    Context.AnalysisOrContinueUI.SetMarkersToIndicateRank1(false);
            //if (Context.OpponentMatchedRankThisTurn == 1)
            //    Context.AnalysisOrContinueUI.SetMarkersToIndicateRank1(false, true);

            _delayTimer = _delayTime;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            if (_delayTimer >= 0)
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer < 0)
                {
                    Context.AnalysisOrContinueUI.ResetLines();
                }
            }

            if (!Context.AnalysisOrContinueUI.IfClicked) return;

            if (Context.AnalysisOrContinueUI.IfAnalysis)
            {
                if (!Context.BoardHasBeenRestored)
                    ActiveState = GameStateMachine2D.EGameState2D.RestoreBoard;
                else
                    ActiveState = GameStateMachine2D.EGameState2D.Analysis;
            }

            if (Context.AnalysisOrContinueUI.IfContinue)
            {
                if (Context.PlayerMatchedProMove && !Context.PlayerHasClickedAnalyse)
                    ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
                else
                {
                    Context.PlayoutProMoves = true;
                    ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                }
            }
        }

        public override void ExitState()
        {
            Context.AnalysisOrContinueUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private float _delayTime = 2f;
        private float _delayTimer = 2f;
    }
}