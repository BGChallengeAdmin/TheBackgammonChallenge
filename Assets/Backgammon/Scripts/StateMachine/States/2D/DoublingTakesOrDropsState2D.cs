using UnityEngine;

namespace Backgammon
{
    public class DoublingTakesOrDropsState2D : GameState2D
    {
        public DoublingTakesOrDropsState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING TAKES DROPS");

            Context.DoublingUI.SetActive(true);

            var player = Context.IfPlayer1Turn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
            if (Main.Instance.IfPlayerVsAI)
                player = Context.IsPlayersTurn ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;

            if (Context.DoublingTakesOrDrops)
            {
                Context.DoublingUI.SetTakesDoubleText(player);
                Context.DoublingManager.TakesDoublingCube((Context.IsPlayersTurn ? Context.PlayingAs : Context.OpponentAs));
                Context.DoublingUI.SetCubeFace(Context.DoublingManager.CubeValueIndex);
                Context.DoublingManager.SetDoublingActive(false);
            }
            else if (!Context.DoublingTakesOrDrops) Context.DoublingUI.SetDropsDoubleText(player);
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                _delayTimer = _timeDelay;

                Context.DoublingUI.SetActive(false);

                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
            }

            if (Context.IfFastForwarding)
            {
                Context.DoublingUI.SetActive(false);
                ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
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