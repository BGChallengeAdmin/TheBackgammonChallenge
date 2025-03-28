using UnityEngine;

namespace Backgammon
{
    public class TurnConfigureState2DS : GameState2DStrategy
    {
        public TurnConfigureState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: CONFIGURE TURN");

            if (Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1 == 0 &&
                Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2 == 0)
            {
                var dice = GetRandomDiceInfo();

                dice = Context.Strategy.TurnConfigureEnter(Context, dice);

                Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice1 = dice[0];
                Context.GameTurnsList[Context.TurnConfig.IndexTurn].Dice2 = dice[1];
            }

            ActiveState = Context.Strategy.TurnConfigureEnterSelectState(Context);
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        int historicIndexTurn = 0;

        private int[] GetRandomDiceInfo()
        {
            // HISTORIC DICE ARE HANDLED BY MatchAIConfigureSettingsMain.cs

            var diceInfo1 = ConvertDice(Random.Range(0, 601));
            var diceInfo2 = ConvertDice(Random.Range(0, 601));

            if (Game2D.AIIfUsingHistoricDice)
            {
                var game = Game2D.AIHistoricGame;
                string diceString = string.Empty;

                do
                {
                    if (historicIndexTurn >= game.NumberOfMoves - 2)
                    {
                        Main.Instance.IncrementHistoricDiceRolls();
                        game = Game2D.AIHistoricGame;
                        historicIndexTurn = 0;
                    }

                    string[] moveParts = game.Moves[++historicIndexTurn].Split(':');

                    if (moveParts.Length >= 2)
                    {
                        diceString = moveParts[0];

                        if (diceString.Length > 0)
                        {
                            diceInfo1 = int.Parse(diceString[0].ToString());
                            diceInfo2 = int.Parse(diceString[1].ToString());
                        }
                    }
                } while (diceString == string.Empty);
            }

            return new int[] { diceInfo1, diceInfo2 };
        }

        private int ConvertDice(int random)
        {
            if (random <= 100) return 1;
            else if (random <= 200) return 2;
            else if (random <= 300) return 3;
            else if (random <= 400) return 4;
            else if (random <= 500) return 5;
            else if (random <= 600) return 6;

            return 1;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}