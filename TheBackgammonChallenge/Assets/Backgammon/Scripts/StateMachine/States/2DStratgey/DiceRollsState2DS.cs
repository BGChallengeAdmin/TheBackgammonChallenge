using UnityEngine;

namespace Backgammon
{
    public class DiceRollsState2DS : GameState2DStrategy
    {
        public DiceRollsState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: DICE ROLLS");

            // ENABLE DICE ROLLS
            Context.DiceRollsUI.SetActive(true);

            if (Context.TurnConfig.IsPlayersTurn)
            {
                var dontRollDiceAnimFirstTurn = ((Context.GameConfig.IfPlayingAsPlayer1 && Context.TurnConfig.IfPlayer1GoesFirst) ||
                                   (!Context.GameConfig.IfPlayingAsPlayer1 && !Context.TurnConfig.IfPlayer1GoesFirst)) &&
                                   Context.TurnConfig.IndexTurn < 2;

                if (Context.TurnConfig.IndexTurn > 1) Context.DiceRollsUI.SetPlayerTapToRoll();

                if (Context.DoublingManager.CubeOwner != Context.GameConfig.OpponentAs)
                    Context.DoublingManager.SetDoublingActive(true);

                Context.TurnConfig.PlayerOfferedDoubles = false;

                // NOTE: DISABLED REPLAY PRO MOVE IF NOT #RANK 1
                //if (Context.OpponentMatchedRankThisTurn != 1 && Context.OpponentMatchedRankThisTurn != 0)
                //    Context.GameScreenUI.SetShowOpponentRank1Active(true);
            }
            else
            {
                var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
                var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

                Context.DiceRollsUI.SetOpponentDiceRollsText((Context.GameConfig.IfPlayingAsPlayer1 ?
                                                                Context.GameConfig.SelectedMatch.Player2 : 
                                                                Context.GameConfig.SelectedMatch.Player1),
                                                                dice1, dice2, true);
            }
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;

            // TEST FOR CONCEDES
            if (Context.TurnConfig.IsPlayersTurn && Context.TurnConfig.ConcedeTheGame)
            {
                Context.TurnConfig.PlayerConcedes = true;
                Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerConcedes;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                Context.GameConfig.Debug_debugObject.DebugMessage($"PLAYER CONCEDES");
            }

            // TEST FOR DOUBLING
            if (Context.DoublingManager.DoublingWasClicked)
            {
                // DISCONNECT FROM CURRENT SERVER AND RESET
                Context.AIDataHandler.DisconnectFromTcpServer();
                Context.TurnConfig.AIDataAvailable = false;

                Context.DiceRollsUI.SetActive(false);
                Context.GameScreenUI.SetShowOpponentRank1Active(false);

                Context.TurnConfig.PlayerOfferedDoubles = true;
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.DoublingData;
            }

            // WAIT FOR PLAYER INTERACTION AND DICE ROLLS
            if (Context.TurnConfig.IsPlayersTurn)
            {
                // SHOW OPPONENT RANK 1 MOVE
                if (Context.TurnConfig.ShowOpponentRank1Move)
                {
                    // FALSE - RESET DICE ROLLS UI
                    Context.DiceRollsUI.SetActive(false);
                    Context.GameScreenUI.SetShowOpponentRank1Active(false);
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ObserveBoard;
                }

                // TEST FOR A.I. DATA
                if (!Context.TurnConfig.AIDataAvailable && !Context.GameConfig.IfFastForwarding)
                {
                    Context.TurnConfig.AIDataAvailable = Context.AIDataHandler.IfNewData();

                    if (Context.TurnConfig.AIDataAvailable)
                    {
                        Context.TurnConfig.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                        Context.TurnConfig.AITopRankedMove = Context.AIDataHandler.AIResponse();
                    }
                }

                var dontRollDiceAnimFirstTurn = ((Context.GameConfig.IfPlayingAsPlayer1 && Context.TurnConfig.IfPlayer1GoesFirst) ||
                                   (!Context.GameConfig.IfPlayingAsPlayer1 && !Context.TurnConfig.IfPlayer1GoesFirst)) &&
                                   Context.TurnConfig.IndexTurn < 2;

                if (!Context.DiceRollsUI.PlayerTapToRoll && Context.TurnConfig.IndexTurn > 1) return;

                Context.DoublingManager.SetDoublingActive(false);
                Context.GameScreenUI.SetShowOpponentRank1Active(false);

                if (!Context.DiceRollsUI.DiceRollAnimation && Context.TurnConfig.IndexTurn > 1) return;
            }

            // SET DICE FACE VALUES
            var blackDice = (Context.TurnConfig.IsPlayersTurn && Context.GameConfig.IfPlayerIsBlack) ||
                            (!Context.TurnConfig.IsPlayersTurn && !Context.GameConfig.IfPlayerIsBlack);

            var dice1 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1;
            var dice2 = Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2;

            Context.DiceRollsUI.SetActive(true);
            if (Context.TurnConfig.IsPlayersTurn) Context.DiceRollsUI.SetPlayerDiceFaceValues(blackDice, dice1, dice2);
            else Context.DiceRollsUI.SetOpponentDiceFaceValues(blackDice, dice1, dice2);

            if (Context.TurnConfig.IsPlayersTurn)
            {
                if (Context.TurnConfig.PlayerIsUnableToMove)
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                else
                {
                    Context.Strategy.DiceRollsDisplayUserHint(Context, dice1, dice2);

                    Context.GameStats.TotalValidPlayerMovesThisGame++;
                    ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.SelectPointFrom;
                }
            }
            else
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer <= 0)
                {
                    _delayTimer = _timeDelay;

                    Context.GameStats.TotalValidOpponentMovesThisGame++;

                    if (Context.TurnConfig.PlayerIsUnableToMove)
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                    else
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.MoveCounters;
                }
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private float _delayTimer = 2.0f;
        private float _timeDelay = 2.0f;
    }
}