using UnityEngine;

namespace Backgammon
{
    public class TurnEndState : GameState
    {
        public TurnEndState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

        public override void EnterState()
        {
            ActiveState = StateKey;
        }

        public override void UpdateState()
        {
            // RESET ALL HANDLERS TO NEXT MOVE

            Context.DiceRollsUI.SetActive(false);

            if (Context.AIDataHandler.ServerConnected)
                Context.AIDataHandler.DisconnectFromTcpServer();

            // IF GAME WON / LOST



            // DELAY THE END OF THE TURN

            // SKIP IF FIRST TURN -> PLAYER 2
            if (Context.IndexTurn == 0 && !Context.IfPlayer1GoesFirst) _delayTimer = -1;

            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                _delayTimer = _timeDelay;

                // IF NEXT TURN
                ActiveState = GameStateMachine.EGameState.TurnBegin;
            }
        }

        public override void ExitState()
        {
            // IF NEXT TURN - INCREMENT GAME TURN
            Context.IndexTurn += 1;
            Context.IsPlayersTurn = !Context.IsPlayersTurn;
            Context.IfPlayer1Turn = !Context.IfPlayer1Turn;

            // TURN CONFIG
            Context.PlayerIsUnableToMove = false;
            Context.PlayerMatchedProMove = false;

            Context.PlayerMoveIndex = 0;
            Context.CounterMoveIndex = 0;
            Context.CountersToMoveIndex = 0;

            // ANALYSIS
            Context.BoardHasBeenRestored = false;
            Context.ClickedPlayerMoveAnalysis = false;
            Context.ClickedProMoveAnalysis = false;
            Context.ClickedTopRankedMoveAnalysis = false;
            Context.PlayerHasClickedAnalyse = false;

            Context.PlayerTurnWasAnalysed = false;
            Context.ProTurnWasAnalysed = false;
            Context.OpponentTurnWasAnalysed = false;
            Context.AITurnWasAnalysed = false;
            
            Context.PlayerMatchedRankThisTurn = 0;
            Context.ProMatchedRankThisTurn = 0;
            Context.OpponentMatchedRankThisTurn = 0;
        
            // AI DATA
            Context.CaptureAIMoveData = false;
            Context.CaptureAIDoublingData = false;
            Context.AIDataAvailable = false;

            // GAME SCREEN UI
            Context.GameScreenUI.ResetTurnIndicators();
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        private float _timeDelay = .5f;
        private float _delayTimer = .5f;
    }
}