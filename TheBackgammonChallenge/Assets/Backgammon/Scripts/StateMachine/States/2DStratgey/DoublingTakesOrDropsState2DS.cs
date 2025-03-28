using UnityEngine;

namespace Backgammon
{
    public class DoublingTakesOrDropsState2DS : GameState2DStrategy
    {
        public DoublingTakesOrDropsState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING TAKES DROPS");

            Context.DoublingUI.SetActive(true);

            var player = Context.TurnConfig.IfPlayer1Turn ? Context.GameConfig.SelectedMatch.Player1 : 
                                                            Context.GameConfig.SelectedMatch.Player2;

            if (Context.TurnConfig.DoublingTakesOrDrops)
            {
                Context.DoublingUI.SetTakesDoubleText(player);
                Context.DoublingManager.TakesDoublingCube((Context.TurnConfig.IsPlayersTurn ? 
                    Context.GameConfig.PlayingAs : Context.GameConfig.OpponentAs));
                
                Context.DoublingUI.SetCubeFace(Context.DoublingManager.CubeValueIndex);
                Context.DoublingManager.SetDoublingActive(false);
            }
            else if (!Context.TurnConfig.DoublingTakesOrDrops) Context.DoublingUI.SetDropsDoubleText(player);
        }

        public override void UpdateState()
        {
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                _delayTimer = _timeDelay;

                Context.DoublingUI.SetActive(false);

                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
            }

            if (Context.GameConfig.IfFastForwarding)
            {
                Context.DoublingUI.SetActive(false);
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnEnd;
            }

            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;
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