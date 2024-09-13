using System;

namespace Backgammon
{
	public class TurnBeginState : GameState
	{
		public TurnBeginState(GameStateContext context, GameStateMachine.EGameState estate) : base (context, estate) { }

		public override void EnterState()
        {
            UnityEngine.Debug.Log($"");
            UnityEngine.Debug.Log($"{(Context.IsPlayersTurn ? "PLAYER" : "PRO")} TURN");
            UnityEngine.Debug.Log($"");

            // IF PLAYING A.I. CAPTURE DATA AND RETURN

            // COPY THE BOARD STATE
            Context.TurnBeginBoardState.SetStateFromContext(Context);

            // GET PLAYER DICE && MOVES FROM MOVE DATA
            var turnData = Context.SelectedMatch.Game(Context.IndexGame).GetPlayerMove(Context.IndexTurn);

            // TODO: TEST IF WINS
            #region TestWins
            if (turnData.Contains("Wins"))
            {
                UnityEngine.Debug.Log($"PLAYER WINS");
                ActiveState = GameStateMachine.EGameState.GameWon;
				goto BailOut;
            }
            #endregion

            string[] moveParts = turnData.Split(':');

            //UnityEngine.Debug.Log($"********");
            //UnityEngine.Debug.Log($"{(Context.IsPlayersTurn ? "PLAYER" : "OPP")} TURN {Context.IndexTurn} -> {turnData}");

            #region TestExemptions
            if (moveParts[0] == string.Empty)
            {
				// NOTE: NOTATION ALLOWS ":" TO SHOW A SKIPPED TURN
                UnityEngine.Debug.Log($"EMPTY TURN!!! SKIP A TURN FOR WIN -> NO DICE WERE ROLLED");

                ActiveState = GameStateMachine.EGameState.TurnEnd; 
				goto BailOut;
            }

            // TODO: TEST MOVES / DOUBLES
            if (moveParts.Length == 1)
			{
				var result = moveParts[0];

				if (result == "Doubles")
				{
					UnityEngine.Debug.Log($"PLAYER DOUBLES");
   
                    ActiveState = GameStateMachine.EGameState.DoublingData;
                    goto BailOut;
                }
                else if (result == "Takes") 
				{
					Context.DoublingTakesOrDrops = true;
					ActiveState = GameStateMachine.EGameState.DoublingTakesOrDrops;
					UnityEngine.Debug.Log($"PLAYER TAKES");
                    goto BailOut;
                }
                else if (result == "Drops")
                {
                    Context.DoublingTakesOrDrops = false;
                    ActiveState = GameStateMachine.EGameState.DoublingTakesOrDrops;
                    UnityEngine.Debug.Log($"PLAYER DROPS");
                    goto BailOut;
                }
                else if (result == "Concedes") { UnityEngine.Debug.Log($"PLAYER CONCEDES"); }

                ActiveState = GameStateMachine.EGameState.TurnEnd;
				goto BailOut;
            }
            #endregion

            #region UpdateUI
            // UPDATE U.I.
            if (ActiveState != GameStateMachine.EGameState.TurnEnd)
            {
                Context.GameScreenUI.SetPlayerScore(Context.PlayerScoreThisGame);
                Context.GameScreenUI.SetProPlayerScore(Context.ProScoreThisGame);
                Context.GameScreenUI.SetOpponentScore(Context.OpponentScoreThisGame);

                Context.GameScreenUI.SetPlayerTopMatched(Context.PlayerTopRankedThisGame);
                Context.GameScreenUI.SetProPlayerTopMatched(Context.ProTopRankedThisGame);
                Context.GameScreenUI.SetOpponentTopMatched(Context.OpponentTopRankedThisGame);

                Context.GameScreenUI.SetGameTurn(Context.IndexTurn + (Context.IfPlayer1GoesFirst ? 1 : 0));

                if (Context.FastForwardTurnIndex <= Context.IndexTurn)
                    Context.IfFastForwarding = false;
            }
            #endregion

            #region CapturePlayerMove
            // CAPTURE DICE ROLLS
            string dice = moveParts[0];
            var dice1 = int.Parse(dice[0].ToString());
            var dice2 = int.Parse(dice[1].ToString());

            Context.Dice1 = dice1;
            Context.Dice2 = dice2;
            Context.DiceManager.SetDiceValues(dice1, dice2);

            //UnityEngine.Debug.Log($"DICE {dice[0]} {dice[1]} -> {Context.Dice1} {Context.Dice2}");

            // RESET AND CAPTURE MOVE DATA
            for (int playerM = 0; playerM < 4; playerM++) { Context.PlayerMovesInfo[playerM] = Context.PlayerMovesInfo[playerM].Reset(); }
            for (int proM = 0; proM < 4; proM++) { Context.RecordedMovesInfo[proM] = Context.RecordedMovesInfo[proM].Reset(); }
            for (int aiM = 0; aiM < 4; aiM++) { Context.TopRankedMovesInfo[aiM] = Context.TopRankedMovesInfo[aiM].Reset(); }

            string movesData = moveParts[1];
            string[] moves = movesData.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Context.TotalMovesThisTurn = moves.Length;
            Context.MovesAvailable = Context.TotalMovesThisTurn;

            // TEST WHEN THERE IS NO MOVE DATA
            if (moves.Length == 0)
            {
                UnityEngine.Debug.Log($"THERE IS NO MOVE DATA - PLAYER WAS BLOCKED FROM MOVING");
                Context.PlayerIsUnableToMove = true;
                ActiveState = GameStateMachine.EGameState.RollDice;

                goto BailOut;
            }
            else
            {
                Context.IsPlayersMakingMoves = Context.IsPlayersTurn ? true : false;
                Context.PlayerMoveIndex = 0;
                Context.CountersToMoveIndex = 0;

                if (!Context.IsPlayersTurn) Context.CountersToMoveIndex = Context.TotalMovesThisTurn;

                if (Context.IfFastForwarding)
                {
                    Context.IsPlayersMakingMoves = false;
                    Context.CountersToMoveIndex = Context.TotalMovesThisTurn;
                }
            }

            // NOTE: MOVES WILL BE IN THE FORMAT X/Y* (* IF THERE IS A BLOT)
            for (int move = 0; move < moves.Length; move++)
			{
				var pointFrom = moves[move].Split('/')[0];
				var pointTo = moves[move].Split('/')[1];
				var ifBlot = pointTo[pointTo.Length - 1] == '*' ? true : false;

				//UnityEngine.Debug.Log($"{moves[move]} -> {pointTo} -> {pointTo[pointTo.Length - 1]} -> {ifBlot}");

				if (ifBlot) { pointTo = pointTo.TrimEnd('*'); }

                Context.RecordedMovesInfo[move].pointFrom = pointFrom == "bar" ? 25 : int.Parse(pointFrom);
				Context.RecordedMovesInfo[move].pointTo = pointTo == "off" ? 0 : int.Parse(pointTo);
				Context.RecordedMovesInfo[move].ifBlot = ifBlot;
            }
            #endregion

            // TODO: CAPTURE A.I. DOUBLING DATA FOR CHALLENGE A.I.

            // CAPTURE A.I. DATA FOR THE TURN
            ActiveState = GameStateMachine.EGameState.AIData;

            if (Context.IfFastForwarding) ActiveState = GameStateMachine.EGameState.MoveCounters;

            BailOut:;
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