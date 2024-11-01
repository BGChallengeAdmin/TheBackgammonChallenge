namespace Backgammon
{
    public class ConfigureBoardState : GameState
    {
        public ConfigureBoardState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate){ }

        public override void EnterState()
        {
            Context.BarManager.Init();

            Context.HomeManager.Init(Context.IfPlayFromLhs);

            Context.DoublingManager.Init(Context.IfPlayFromLhs);

            // NOTE: ACTS AS PARTIAL CONTROLLER FOR BAR / HOME VIA -> GAME.CONTEXT
            Context.PointsManager.InitPointsFrom24(Context.IfPlayFromLhs);

            Context.CountersManager.Init();
            Context.CountersManager.ZeroCounterPositions();

            Context.DiceManager.Init();

            Context.ObserveAnalysisManager.Init();

            SetCountersToPlayerStart();
            SetCountersToOpponentStart();
        }

        public override void UpdateState()
        {
            if (Context.CountersManager.TestIfAnyCountersMoving()) return;

            ActiveState = GameStateMachine.EGameState.BeforeCommence;

            UnityEngine.Debug.Log($"CONFIGURE BOARD -> {ActiveState.ToString()}");
        }

        public override void ExitState() 
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        private void SetCountersToPlayerStart()
        {
            Context.HomeManager.ResetPlayerHome();

            Context.PointsManager.SetCountersToStartingPoints(true,
                                                        Context.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                                           Context.CountersManager.WhiteCounters);
        }

        private void SetCountersToOpponentStart()
        {
            Context.HomeManager.ResetOpponentHome();

            Context.PointsManager.SetCountersToStartingPoints(false,
                                                        !Context.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                                            Context.CountersManager.WhiteCounters);
        }
    }
}