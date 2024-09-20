namespace Backgammon
{
    public class DoublingInGameState : GameState
    {
        public DoublingInGameState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            Context.DoublingUI.SetActive(true);
            Context.DoublingUI.IfOfferedDoubles();
        }

        public override void UpdateState() 
        {
            if (!Context.DoublingUI.IfClicked) return;

            Context.DoublingUI.SetOfferedDoublesText(Context.AIDoublingData);
            
            if (!Context.DoublingUI.IfClickedConfirm) return;
            
            ActiveState = GameStateMachine.EGameState.RollDice;
        }

        public override void ExitState()
        {
            Context.DoublingUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }
    }
}