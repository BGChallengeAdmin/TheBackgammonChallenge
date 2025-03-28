using UnityEngine;

namespace Backgammon
{
    public class AnalysisOrContinueState2DS : GameState2DStrategy
    {
        public AnalysisOrContinueState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: ANALYSIS OR CONTINUE");

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
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

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
                if (!Context.TurnConfig.BoardHasBeenRestored)
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RestoreBoard;
                else
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.Analysis;
            }

            if (Context.AnalysisOrContinueUI.IfContinue)
            {
                if (Context.TurnConfig.PlayerMatchedProMove && !Context.TurnConfig.PlayerHasClickedAnalyse)
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
                else
                {
                    Context.TurnConfig.PlayoutProMoves = true;
                    Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayoutProMoves;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                }
            }
        }

        public override void ExitState()
        {
            Context.AnalysisOrContinueUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private float _delayTime = 2f;
        private float _delayTimer = 2f;
    }
}