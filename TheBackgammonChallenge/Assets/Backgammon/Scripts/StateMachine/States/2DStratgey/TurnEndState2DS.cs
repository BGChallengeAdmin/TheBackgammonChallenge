using UnityEngine;

namespace Backgammon
{
    public class TurnEndState2DS : GameState2DStrategy
    {
        public TurnEndState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: TURN END");

            _delayTimer = Context.TurnConfig.IsPlayersTurn ? _opponentTimeDelay : _playerTimeDelay;

            // SKIP IF FIRST TURN -> PLAYER 2
            if (Context.TurnConfig.IndexTurn == 0 && !Context.TurnConfig.IfPlayer1GoesFirst) _delayTimer = -1;

            if (Context.GameConfig.IfFastForwarding) _delayTimer = -1;

            ActiveState = StateKey;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
            {
                if (Context.TurnConfig.ConcedeTheGame)
                {
                    var wonMatchByPoints = (Context.GameConfig.SelectedGame.Player1PointsAtEnd >= Context.GameConfig.SelectedMatch.Points) ||
                                          (Context.GameConfig.SelectedGame.Player2PointsAtEnd >= Context.GameConfig.SelectedMatch.Points);

                    Context.GameConfig.IfPlayNextGame = !wonMatchByPoints;
                    Context.GameConfig.IfPlayAnotherMatch = wonMatchByPoints;
                }

                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;
            }

            Context.DiceRollsUI.SetActive(false);

            if (Context.AIDataHandler.ServerConnected)
                Context.AIDataHandler.DisconnectFromTcpServer();

            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                // IF NEXT TURN
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnConfigure;
            }

            // IF FAST FORWARD - FADE BLACK SCREEN OUT
            if (Context.GameConfig.IfFastForwarding)
                if (Context.TurnConfig.IndexTurn == Context.GameConfig.FastForwardTurnIndex - 1)
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ConfigureBoardForContinue;
        }

        public override void ExitState()
        {
            // IF NEXT TURN - INCREMENT GAME TURN
            Context.TurnConfig.IndexTurn += 1;
            Context.TurnConfig.IsPlayersTurn = !Context.TurnConfig.IsPlayersTurn;
            Context.TurnConfig.IfPlayer1Turn = !Context.TurnConfig.IfPlayer1Turn;

            Context.GameTurnsList.Add(new GameTurn());

            // TURN CONFIG
            Context.TurnConfig.PlayerIsUnableToMove = false;
            Context.TurnConfig.PlayerMatchedProMove = false;

            Context.TurnConfig.PlayerMoveIndex = 0;
            Context.TurnConfig.CounterMoveIndex = 0;
            Context.TurnConfig.CountersToMoveIndex = 0;

            Context.TurnConfig.PreviousDice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn - 1].Dice1;
            Context.TurnConfig.PreviousDice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn - 1].Dice2;

            // ANALYSIS
            Context.TurnConfig.BoardHasBeenRestored = false;
            Context.TurnConfig.ClickedPlayerMoveAnalysis = false;
            Context.TurnConfig.ClickedProMoveAnalysis = false;
            Context.TurnConfig.ClickedOpponentMoveAnalysis = false;
            Context.TurnConfig.ClickedTopRankedMoveAnalysis = false;
            Context.TurnConfig.PlayerHasClickedAnalyse = false;

            Context.TurnConfig.PlayerTurnWasAnalysed = false;
            Context.TurnConfig.ProTurnWasAnalysed = false;
            Context.TurnConfig.OpponentTurnWasAnalysed = false;
            Context.TurnConfig.ReplayPlayerMove = false;
            Context.TurnConfig.PlayoutProMoves = false;

            // AI DATA
            Context.TurnConfig.CapturedAIMoveData = false;
            Context.TurnConfig.CapturedAIDoublingData = false;
            Context.TurnConfig.AIDataAvailable = false;

            // GAME SCREEN UI
            Context.GameScreenUI.ResetTurnIndicators();
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private float _playerTimeDelay = .5f;
        private float _opponentTimeDelay = 1.5f;
        private float _delayTimer = .5f;
    }
}