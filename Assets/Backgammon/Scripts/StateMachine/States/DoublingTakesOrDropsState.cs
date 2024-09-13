using UnityEngine;

namespace Backgammon
{
    public class DoublingTakesOrDropsState : GameState
    {
        public DoublingTakesOrDropsState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

        public override void EnterState()
        {
            Context.DoublingUI.SetActive(true);

            var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;

            Debug.Log($"TAKES OR DROPS");

            if (Context.DoublingTakesOrDrops)
            {
                Context.DoublingUI.SetTakesDoubleText(player);
                Context.DoublingManager.SetCubeToMove(!Context.IsPlayersTurn);
            }
            else if (!Context.DoublingTakesOrDrops) Context.DoublingUI.SetDropsDoubleText(player);
        }

        public override void UpdateState()
        {
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                _delayTimer = _timeDelay;

                Context.DoublingUI.SetActive(false);

                ActiveState = GameStateMachine.EGameState.TurnEnd;
            }

            if (Context.IfFastForwarding)
            {
                Context.DoublingUI.SetActive(false);
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
    }
}