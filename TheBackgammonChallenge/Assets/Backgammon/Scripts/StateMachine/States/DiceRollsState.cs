using UnityEngine;

namespace Backgammon
{
    public class DiceRollsState : GameState
    {
        public DiceRollsState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

        public override void EnterState()
        {
            // ENABLE DICE ROLLS
            Context.DiceRollsUI.SetActive(true);
            if (Context.IsPlayersTurn)
            {
                Context.DiceRollsUI.SetPlayerTapToRoll();

                //if ((Context.DoublingManager.CubeOwner == Context.PlayingAs) ||
                //    (Context.DoublingManager.CubeOwner == Game.PlayingAs.NONE))
                //    Context.DoublingManager.SetDoublingActive(true);

                Context.PlayerOfferedDoubles = false;
            }
        }

        public override void UpdateState()
        {
            // TEST FOR DOUBLING
            if (Context.DoublingManager.DoublingWasClicked)
            {
                Context.DiceRollsUI.SetActive(false);
                Context.PlayerOfferedDoubles = true;
                ActiveState = GameStateMachine.EGameState.DoublingData;
            }

            if (Context.IsPlayersTurn && !Context.DiceRollsUI.PlayerTapToRoll) return;

            if (Context.IsPlayersTurn) Context.DoublingManager.SetDoublingActive(false);

            if (Context.IsPlayersTurn && !Context.DiceRollsUI.DiceRollAnimation) return;
            
            var blackDice = (Context.IsPlayersTurn && Context.IfPlayerIsBlack) ||
                            (!Context.IsPlayersTurn && !Context.IfPlayerIsBlack);

            Context.DiceRollsUI.SetActive(true);
            if (Context.IsPlayersTurn) Context.DiceRollsUI.SetPlayerDiceFaceValues(blackDice, Context.Dice1, Context.Dice2);
            else Context.DiceRollsUI.SetOpponentDiceFaceValues(blackDice, Context.Dice1, Context.Dice2);

            // DEBUG - UNCOMMENT LINE -> REMOVE DELAY
            //ActiveState = GameStateMachine.EGameState.MoveCounters;
            if (Context.IsPlayersTurn)
            {
                if (Context.PlayerIsUnableToMove)
                    ActiveState = GameStateMachine.EGameState.GeneralInfo;
                else
                {
                    Context.TotalValidPlayerMovesThisGame++;
                    ActiveState = GameStateMachine.EGameState.SelectPointFrom;
                }
            }
            else
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer <= 0)
                {
                    _delayTimer = _timeDelay;

                    if (Context.PlayerIsUnableToMove)
                        ActiveState = GameStateMachine.EGameState.GeneralInfo;
                    else
                        ActiveState = GameStateMachine.EGameState.MoveCounters;
                }
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
    }
}