namespace Backgammon
{
    public class RollForGoesFirstState2D : GameState2D
    {
        public RollForGoesFirstState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState() 
        {
            Context.RollForGoesFirstUI.SetActive(true);

            var playerGoesFirst = (Context.IfPlayer1GoesFirst && Context.IfPlayingAsPlayer1) ||
                            (!Context.IfPlayer1GoesFirst && !Context.IfPlayingAsPlayer1);

            var dice = Context.SelectedGame.GetPlayerMove((Context.IfPlayer1GoesFirst ? 0 : 1)).Split(":")[0];
            int playerDice = 0, opponentDice = 0;

            if (Main.Instance.IfPlayerVsAI)
            {
                playerDice = UnityEngine.Random.Range((playerGoesFirst ? 2 : 1), (playerGoesFirst ? 7 : 6));
                opponentDice = 0;

                if (playerGoesFirst)
                {
                    opponentDice = UnityEngine.Random.Range(1, playerDice);
                }
                else
                {
                    opponentDice = UnityEngine.Random.Range(playerDice + 1, 7);
                }

                Context.Dice1 = playerDice;
                Context.Dice2 = opponentDice;

                Context.AIDiceHaveBeenRolled = true;
            }
            else
            {
                var dice1 = int.Parse(dice[0].ToString());
                var dice2 = int.Parse(dice[1].ToString());

                var larger = dice1 > dice2 ? dice1 : dice2;
                var smaller = dice1 < dice2 ? dice1 : dice2;

                playerDice = playerGoesFirst ? larger : smaller;
                opponentDice = playerGoesFirst ? smaller : larger;
            }

            Context.RollForGoesFirstUI.RollOffForGoesFirst(playerGoesFirst, playerDice, opponentDice);
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