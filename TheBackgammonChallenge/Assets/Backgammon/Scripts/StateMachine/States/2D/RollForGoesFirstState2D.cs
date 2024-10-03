namespace Backgammon
{
    public class RollForGoesFirstState2D : GameState2D
    {
        public RollForGoesFirstState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState() 
        {
            Context.RollForGoesFirstUI.SetActive(true);

            var goesFirst = (Context.IfPlayer1GoesFirst && Context.IfPlayingAsPlayer1) ||
                            (!Context.IfPlayer1GoesFirst && !Context.IfPlayingAsPlayer1);

            Context.RollForGoesFirstUI.RollOffForGoesFirst(goesFirst);
        }

        public override void UpdateState() 
        {
            if (Context.RollForGoesFirstUI.Animating) return;

            ActiveState = GameStateMachine2D.EGameState2D.BeforeCommence;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}