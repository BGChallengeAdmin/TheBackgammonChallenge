namespace Backgammon
{
    public class PlayoutProMovesState : GameState
    {
        public PlayoutProMovesState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.IsPlayersMakingMoves = false;
            Context.CounterMoveIndex = 0;
            Context.CountersToMoveIndex = Context.TotalMovesThisTurn;

            Context.DiceRollsUI.ResetImageSizes();

            ActiveState = GameStateMachine.EGameState.MoveCounters;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}