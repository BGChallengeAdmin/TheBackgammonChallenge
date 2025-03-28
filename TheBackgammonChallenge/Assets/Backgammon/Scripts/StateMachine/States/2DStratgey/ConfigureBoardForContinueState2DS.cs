namespace Backgammon
{
    public class ConfigureBoardForContinueState2DS : GameState2DStrategy
    {
        public ConfigureBoardForContinueState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: CONFIGURE BOARD CONTINUE");

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnBegin;

            if (Context.TurnConfig.IndexTurn < Context.GameConfig.FastForwardTurnIndex || Context.GameConfig.IfAIGameToContinue)
            {
                // FADE SCREEN OUT FOR CONTINUE
                if (!Context.GameConfig.IfAIGameToContinue)
                {
                    Context.FadeInFadeOutBlack.SetActive(true);
                    Context.FadeInFadeOutBlack.SetFadeToBlackSeconds(.1f);
                }
                else ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnConfigure;

                // MATCH STATS
                var saveObject = Context.Strategy.GameToContinue;

                Context.GameStats.SaveObject = saveObject;

                Context.GameStats.TotalValidPlayerMovesThisGame = saveObject.MovesMade;
                Context.GameStats.TotalValidPlayerMatchesThisGame = saveObject.MovesMatched;
                Context.GameStats.TotalValidOpponentMovesThisGame = saveObject.OpponentMovesMade;

                Context.GameStats.PlayerTopRankedThisGame = saveObject.AITopMatched;
                Context.GameStats.ProTopRankedThisGame = saveObject.ProMovesMatched;
                Context.GameStats.OpponentTopRankedThisGame = saveObject.OpponentMovesMatched;

                Context.GameStats.PlayerSecondRankedThisGame = saveObject.PlayerSecondMatched;
                Context.GameStats.ProSecondRankedThisGame = saveObject.ProSecondMatched;
                Context.GameStats.OpponentSecondRankedThisGame = saveObject.OpponentSecondMatched;

                Context.GameStats.PlayerScoreThisGame = saveObject.PlayerTotalScore;
                Context.GameStats.ProScoreThisGame = saveObject.ProTotalScore;
                Context.GameStats.OpponentScoreThisGame = saveObject.OpponentTotalScore;

                Context.Strategy.ConfigureBoardContinueEnter(Context);

                // SET GAME UI
                Context.GameScreenUI.SetPlayerTopMatched(saveObject.AITopMatched);
                Context.GameScreenUI.SetProPlayerTopMatched(saveObject.ProMovesMatched);
                Context.GameScreenUI.SetOpponentTopMatched(saveObject.OpponentMovesMatched);
            }
            else if (Context.TurnConfig.IndexTurn == Context.GameConfig.FastForwardTurnIndex)
            {
                // FADE SCREEN IN FOR GAME
                Context.FadeInFadeOutBlack.SetFadeOutFromBlackSeconds(1f);
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnConfigure;
            }
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }
    }
}