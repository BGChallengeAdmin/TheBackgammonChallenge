using UnityEngine;

namespace Backgammon
{
    public class TurnEndState2D : GameState2D
    {
        public TurnEndState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: TURN END");

            _delayTimer = Context.IsPlayersTurn ? _opponentTimeDelay : _playerTimeDelay;

            // SKIP IF FIRST TURN -> PLAYER 2
            if (Context.IndexTurn == 0 && !Context.IfPlayer1GoesFirst) _delayTimer = -1;

            if (Context.IfFastForwarding) _delayTimer = -1;

            ActiveState = StateKey;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            Context.DiceRollsUI.SetActive(false);

            if (Context.AIDataHandler.ServerConnected)
                Context.AIDataHandler.DisconnectFromTcpServer();

            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                // IF NEXT TURN
                ActiveState = GameStateMachine2D.EGameState2D.TurnConfigure;
            }

            // IF FAST FORWARD - FADE BLACK SCREEN OUT
            if (Context.IfFastForwarding)
                if (Context.IndexTurn == Context.FastForwardTurnIndex - 1)
                    ActiveState = GameStateMachine2D.EGameState2D.ConfigureBoardForContinue;
        }

        public override void ExitState()
        {
            // IF NEXT TURN - INCREMENT GAME TURN
            Context.IndexTurn += 1;
            Context.AIIndexTurn += 1;
            Context.IsPlayersTurn = !Context.IsPlayersTurn;
            Context.IfPlayer1Turn = !Context.IfPlayer1Turn;

            // TURN CONFIG
            Context.PlayerIsUnableToMove = false;
            Context.PlayerMatchedProMove = false;

            Context.PlayerMoveIndex = 0;
            Context.CounterMoveIndex = 0;
            Context.CountersToMoveIndex = 0;

            Context.PreviousDice1 = Context.Dice1;
            Context.PreviousDice2 = Context.Dice2;

            // ANALYSIS
            Context.BoardHasBeenRestored = false;
            Context.ClickedPlayerMoveAnalysis = false;
            Context.ClickedProMoveAnalysis = false;
            Context.ClickedOpponentMoveAnalysis = false;
            Context.ClickedTopRankedMoveAnalysis = false;
            Context.PlayerHasClickedAnalyse = false;

            Context.PlayerTurnWasAnalysed = false;
            Context.ProTurnWasAnalysed = false;
            Context.OpponentTurnWasAnalysed = false;
            Context.ReplayPlayerMove = false;
            Context.PlayoutProMoves = false;

            // AI DATA
            Context.CapturedAIMoveData = false;
            Context.CapturedAIDoublingData = false;
            Context.AIDataAvailable = false;

            // GAME SCREEN UI
            Context.GameScreenUI.ResetTurnIndicators();
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private float _playerTimeDelay = .5f;
        private float _opponentTimeDelay = 1.5f;
        private float _delayTimer = .5f;
    }
}