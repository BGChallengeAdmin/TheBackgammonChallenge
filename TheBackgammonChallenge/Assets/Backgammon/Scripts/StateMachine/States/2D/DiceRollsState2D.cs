using UnityEngine;

namespace Backgammon
{
    public class DiceRollsState2D : GameState2D
    {
        public DiceRollsState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: DICE ROLLS");

            // ENABLE DICE ROLLS
            Context.DiceRollsUI.SetActive(true);

            if (Context.IsPlayersTurn)
            {
                var dontRollDiceAnimFirstTurn = ((Context.IfPlayingAsPlayer1 && Context.IfPlayer1GoesFirst) ||
                                   (!Context.IfPlayingAsPlayer1 && !Context.IfPlayer1GoesFirst)) &&
                                   Context.IndexTurn < 2;

                if (Context.IndexTurn > 1) Context.DiceRollsUI.SetPlayerTapToRoll();

                if (Context.DoublingManager.CubeOwner != Context.OpponentAs)
                    Context.DoublingManager.SetDoublingActive(true);

                Context.PlayerOfferedDoubles = false;

                // NOTE: DISABLED REPLAY PRO MOVE IF NOT #RANK 1
                //if (Context.OpponentMatchedRankThisTurn != 1 && Context.OpponentMatchedRankThisTurn != 0)
                //    Context.GameScreenUI.SetShowOpponentRank1Active(true);
            }
            else
            {
                Context.DiceRollsUI.SetOpponentDiceRollsText((Context.IfPlayingAsPlayer1 ?
                                                                    Context.SelectedMatch.Player2 : Context.SelectedMatch.Player1),
                                                                    Context.Dice1, Context.Dice2, true);
            }
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            // TEST FOR CONCEDES
            if (Context.IsPlayersTurn && Context.ConcedeTheGame)
            {
                Context.PlayerConcedes = true;
                Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.PlayerConcedes;
                ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                Context.Debug_debugObject.DebugMessage($"PLAYER CONCEDES");
            }

            // TEST FOR DOUBLING
            if (Context.DoublingManager.DoublingWasClicked)
            {
                // DISCONNECT FROM CURRENT SERVER AND RESET
                Context.AIDataHandler.DisconnectFromTcpServer();
                Context.AIDataAvailable = false;

                Context.DiceRollsUI.SetActive(false);
                Context.GameScreenUI.SetShowOpponentRank1Active(false);
                
                Context.PlayerOfferedDoubles = true;
                ActiveState = GameStateMachine2D.EGameState2D.DoublingData;
            }

            // WAIT FOR PLAYER INTERACTION AND DICE ROLLS
            if (Context.IsPlayersTurn)
            {
                // SHOW OPPONENT RANK 1 MOVE
                if (Context.ShowOpponentRank1Move)
                {
                    // FALSE - RESET DICE ROLLS UI
                    Context.DiceRollsUI.SetActive(false);
                    Context.GameScreenUI.SetShowOpponentRank1Active(false);
                    ActiveState = GameStateMachine2D.EGameState2D.ObserveBoard;
                }

                // TEST FOR A.I. DATA
                if (!Context.AIDataAvailable && !Context.IfFastForwarding)
                {
                    Context.AIDataAvailable = Context.AIDataHandler.IfNewData();

                    if (Context.AIDataAvailable)
                    {
                        Context.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                        Context.AITopRankedMove = Context.AIDataHandler.AIResponse();
                    }
                }
                
                var dontRollDiceAnimFirstTurn = ((Context.IfPlayingAsPlayer1 && Context.IfPlayer1GoesFirst) ||
                                   (!Context.IfPlayingAsPlayer1 && !Context.IfPlayer1GoesFirst)) &&
                                   Context.IndexTurn < 2;

                if (!Context.DiceRollsUI.PlayerTapToRoll && Context.IndexTurn > 1) return;

                Context.DoublingManager.SetDoublingActive(false);
                Context.GameScreenUI.SetShowOpponentRank1Active(false);

                if (!Context.DiceRollsUI.DiceRollAnimation && Context.IndexTurn > 1) return;
            }

            // SET DICE FACE VALUES
            var blackDice = (Context.IsPlayersTurn && Context.IfPlayerIsBlack) ||
                            (!Context.IsPlayersTurn && !Context.IfPlayerIsBlack);

            Context.DiceRollsUI.SetActive(true);
            if (Context.IsPlayersTurn) Context.DiceRollsUI.SetPlayerDiceFaceValues(blackDice, Context.Dice1, Context.Dice2);
            else Context.DiceRollsUI.SetOpponentDiceFaceValues(blackDice, Context.Dice1, Context.Dice2);

            if (Context.IsPlayersTurn)
            {
                if (Context.PlayerIsUnableToMove)
                    ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                else
                {
                    // DISPLAY DICE ROLLS HINT FOR 2-3 TURNS
                    if (Context.IndexTurn <= 5 && !Main.Instance.IfPlayerVsAI)
                    {
                        Context.DiceRollsUI.SetPlayerDiceRollsText((Context.IfPlayingAsPlayer1 ?
                                                                    Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2),
                                                                    Context.Dice1, Context.Dice2, true);
                    }
                    else Context.DiceRollsUI.SetPlayerDiceRollsText(string.Empty, Context.Dice1, Context.Dice2, true);

                    Context.TotalValidPlayerMovesThisGame++;
                    ActiveState = GameStateMachine2D.EGameState2D.SelectPointFrom;
                }
            }
            else
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer <= 0)
                {
                    _delayTimer = _timeDelay;

                    Context.TotalValidOpponentMovesThisGame++;

                    if (Context.PlayerIsUnableToMove) 
                        ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                    else 
                        ActiveState = GameStateMachine2D.EGameState2D.MoveCounters;
                }
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        private float _delayTimer = 2.0f;
        private float _timeDelay = 2.0f;
    }
}