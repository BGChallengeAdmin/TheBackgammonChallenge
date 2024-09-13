namespace Backgammon
{
    public class DoublingInGameState2D : GameState2D
    {
        public DoublingInGameState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING IN GAME");

            Context.DoublingUI.SetActive(true);
            Context.DoublingUI.IfOfferedDoubles();

            Context.AIPlayerAcceptsDoubles = false;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            if (!Context.DoublingUI.IfClicked) return;

            if (Context.DoublingUI.IfClickedYes)
                Context.DoublingUI.SetOfferedDoublesText(Context.AIDoublingData);

            if (!Context.DoublingUI.IfClickedConfirm) return;

            ActiveState = GameStateMachine2D.EGameState2D.RollDice;

            if (Main.Instance.IfPlayerVsAI)
            {
                if (Context.IsPlayersTurn) ActiveState = GameStateMachine2D.EGameState2D.TurnEnd;
            }
        }

        public override void ExitState()
        {
            Context.DoublingUI.SetActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}