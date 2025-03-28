namespace Backgammon
{
    public abstract class GameState2DStrategy : BaseState<GameStateMachine2DStrategy.EGameState2DStrategy>
    {
        public GameState2DStrategy(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy stateKey) : base(stateKey) { Context = context; }

        protected GameStateContext2DStrategy Context;
    }
}

//namespace Backgammon
//{
//    public class GeneralInfoState2D : GameState2D
//    {
//        public GeneralInfoState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

//        public override void EnterState() { }

//        public override void UpdateState() { }

//        public override void ExitState()
//        {
//            ActiveState = StateKey;
//        }

//        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

//        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
//    }
//}