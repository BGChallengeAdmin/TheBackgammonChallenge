namespace Backgammon
{
    public abstract class GameState : BaseState<GameStateMachine.EGameState>
    {
        public GameState(GameStateContext context, GameStateMachine.EGameState stateKey) : base(stateKey) { Context = context; }

        protected GameStateContext Context;
    }
}

// GAMESTATE TEMPLATE

//namespace Backgammon
//{
//    public class GeneralInfoState : GameState
//    {
//        public GeneralInfoState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

//        public override void EnterState() { }

//        public override void UpdateState() { }

//        public override void ExitState()
//        {
//            ActiveState = StateKey;
//        }

//        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

//        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
//    }
//}