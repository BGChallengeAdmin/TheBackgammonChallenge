namespace Backgammon
{
    public class ConfigureBoardForContinueState2D : GameState2D
    {
        public ConfigureBoardForContinueState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: CONFIGURE BOARD CONTINUE");

            ActiveState = GameStateMachine2D.EGameState2D.TurnBegin;

            if (Context.IndexTurn < Context.FastForwardTurnIndex || Context.IfAIGameToContinue)
            {
                // FADE SCREEN OUT FOR CONTINUE
                if (!Context.IfAIGameToContinue)
                {
                    Context.FadeInFadeOutBlack.SetActive(true);
                    Context.FadeInFadeOutBlack.SetFadeToBlackSeconds(.1f);
                }
                else ActiveState = GameStateMachine2D.EGameState2D.TurnConfigure;

                // MATCH STATS
                var saveObject = Main.Instance.IfPlayerVsAI ? Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame :
                                                              Main.Instance.PlayerPrefsObj.ScoreInfoContinueProGame;

                Context.TotalValidPlayerMovesThisGame = saveObject.MovesMade;
                Context.TotalValidPlayerMatchesThisGame = saveObject.MovesMatched;
                Context.TotalValidOpponentMovesThisGame = saveObject.OpponentMovesMade;

                Context.PlayerTopRankedThisGame = saveObject.AITopMatched;
                Context.ProTopRankedThisGame = saveObject.ProMovesMatched;
                Context.OpponentTopRankedThisGame = saveObject.OpponentMovesMatched;

                Context.PlayerSecondRankedThisGame = saveObject.PlayerSecondMatched;
                Context.ProSecondRankedThisGame = saveObject.ProSecondMatched;
                Context.OpponentSecondRankedThisGame = saveObject.OpponentSecondMatched;

                Context.PlayerScoreThisGame = saveObject.PlayerTotalScore;
                Context.ProScoreThisGame = saveObject.ProTotalScore;
                Context.OpponentScoreThisGame = saveObject.OpponentTotalScore;

                // NOTE: ONLY USED FOR A.I. GAME
                if (Main.Instance.IfPlayerVsAI)
                {
                    var playerHasCube = saveObject.PlayerDoublingValue > 0 ? true : false;
                    var opponentHasCube = saveObject.ProDoublingValue > 0 ? true : false;

                    if (playerHasCube)
                    {
                        Context.DoublingManager.SetCubeOwner(Game2D.PlayingAs.PLAYER_1);
                        Context.DoublingManager.SetCubeValue(saveObject.PlayerDoublingValue);
                        Context.DoublingManager.SetCubeToMove(false);
                        Context.DoublingUI.SetCubeFace(saveObject.PlayerDoublingValue);
                    }
                    else if (opponentHasCube)
                    {
                        Context.DoublingManager.SetCubeOwner(Game2D.PlayingAs.PLAYER_2);
                        Context.DoublingManager.SetCubeValue(saveObject.ProDoublingValue);
                        Context.DoublingManager.SetCubeToMove(true);
                        Context.DoublingUI.SetCubeFace(saveObject.ProDoublingValue);
                    }
                }

                // SET GAME UI
                Context.GameScreenUI.SetPlayerTopMatched(saveObject.AITopMatched);
                Context.GameScreenUI.SetProPlayerTopMatched(saveObject.ProMovesMatched);
                Context.GameScreenUI.SetOpponentTopMatched(saveObject.OpponentMovesMatched);
            }
            else if (Context.IndexTurn == Context.FastForwardTurnIndex)
            {
                // FADE SCREEN IN FOR GAME
                Context.FadeInFadeOutBlack.SetFadeOutFromBlackSeconds(1f);
                ActiveState = GameStateMachine2D.EGameState2D.TurnConfigure;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }
    }
}