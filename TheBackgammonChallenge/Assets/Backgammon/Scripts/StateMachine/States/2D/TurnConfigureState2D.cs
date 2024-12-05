using UnityEngine;

namespace Backgammon
{
    public class TurnConfigureState2D : GameState2D
    {
        public TurnConfigureState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: CONFIGURE TURN");

            // DEFAULT TO TURN BEGIN - NORMAL GAMEPLAY
            ActiveState = GameStateMachine2D.EGameState2D.TurnBegin;

            // PLAYER VS A.I.
            if (Main.Instance.IfPlayerVsAI)
            {
                // NEED TO SET DICE FOR PLAYER AND A.I. DATA
                if (!Context.AIDiceHaveBeenRolled)
                {
                    var dice = GetDiceInfoForTurn(Context.AIIndexTurn);

                    Context.Dice1 = dice[0];
                    Context.Dice2 = dice[1];

                    Context.AIDiceHaveBeenRolled = true;
                }

                if (!Context.IsPlayersTurn)
                {
                    if (Context.PlayerOfferedDoubles)
                        ActiveState = GameStateMachine2D.EGameState2D.TurnBegin;
                    else if (!Context.CapturedAIDoublingData && 
                            Context.DoublingManager.CubeOwner != Game2D.PlayingAs.PLAYER_1)
                                ActiveState = GameStateMachine2D.EGameState2D.DoublingData;
                    else if (!Context.CapturedAIMoveData)
                    {
                        Context.AIDataRequestBailOutCounter = 0;
                        ActiveState = GameStateMachine2D.EGameState2D.AIData;
                    }
                }

                // DEBUG TESTING - NO FAST FORWARD IN A.I. GAME
                if (Context.IfFastForwarding)
                    ActiveState = GameStateMachine2D.EGameState2D.AIData;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        int historicIndexTurn = 0;

        private int[] GetDiceInfoForTurn(int aiIndexTurn)
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

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}