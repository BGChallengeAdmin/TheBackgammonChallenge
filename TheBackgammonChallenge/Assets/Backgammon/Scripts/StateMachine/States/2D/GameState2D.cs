namespace Backgammon
{
    public abstract class GameState2D : BaseState<GameStateMachine2D.EGameState2D>
    {
        public GameState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D stateKey) : base(stateKey) { Context = context; }

        protected GameStateContext2D Context;
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