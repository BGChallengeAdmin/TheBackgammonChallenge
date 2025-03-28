namespace Backgammon
{
    public class ConfigureBoardState2DS : GameState2DStrategy
    {
        public ConfigureBoardState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: CONFIGURE BOARD");

            //Context.BoardMaterialsManager.ConfigureBoardDimsToScreenSize();
            //Context.PointsManager.ConfigurePointsDimsToScreenSize();

            Context.HomeManager.Init(Context.GameConfig.IfPlayFromLhs);

            // NOTE: ACTS AS PARTIAL CONTROLLER FOR BAR / HOME VIA -> GAME.CONTEXT
            Context.PointsManager.InitPointsFrom24(Context.GameConfig.IfPlayFromLhs);

            var scaledCounterSize = Context.PointsManager.DefinePointCounterScaling();

            Context.CountersManager.Init(scaledCounterSize);

            var scaledCounterSizeToHome = scaledCounterSize * Context.CountersManager.ConfigureCountersDimsToHome();

            Context.HomeManager.SetScaledCounterSize(scaledCounterSizeToHome);

            Context.BarManager.Init();

            Context.DoublingManager.Init(Context.GameConfig.IfPlayFromLhs);

            Context.DoublingUI.SetCubeImage(Context.DoublingManager.GetCubeImage());
            Context.DoublingUI.SetCubeFace(Context.DoublingManager.CubeValueIndex);

            Context.DiceManager.Init(scaledCounterSize);

            Context.RollForGoesFirstUI.Init(scaledCounterSize);

            Context.AnalysisOrContinueUI.Init();

            Context.ObserveAnalysisManager.Init(scaledCounterSize);

            if (Context.GameConfig.IfAIGameToContinue)
            {
                var boardStateString = Main.Instance.PlayerPrefsObj.ScoreInfoContinueAIGame.BoardState;
                Context.TurnConfig.TurnBeginBoardState.SetBoardStateFromString(boardStateString);
                SetCountersToBoardState(Context.TurnConfig.TurnBeginBoardState);
            }
            else if (!Game2DStrategy.IfManualSetupBoard)
            {
                SetCountersToPlayerStart();
                SetCountersToOpponentStart();

                // DEBUG - SET BOARD FROM BOARD STATE
                Point2DPrefab[] points = Context.PointsManager.Points;
                GameStateContext2D.BoardState debugBoard = new GameStateContext2D.BoardState();

                // EITHER SET FROM 'BOARD STATE' - I.E. CONTINUE
                // - OR DEFINE FOR DEBUG USE
                int[] pointsArray = new int[]
                                    { 1, 3, 2, 2, 2, 2,
                                      0, 0, 0, 0, 0, 0,
                                      0, 0, 0, 0, 0, 0,
                                      -2, -2, -2, -2, -2, 0
                                    };

                debugBoard.Points = pointsArray;
                debugBoard.PlayerBarCount = 1;
                debugBoard.OpponentBarCount = 2;
                debugBoard.PlayerHomeCount = 2;
                debugBoard.OpponentHomeCount = 3;

                //SetCountersToBoardState(debugBoard);
                // END DEBUG
            }

            Context.FadeInFadeOutBlack.SetActive(false);
        }

        public override void UpdateState()
        {
            if (Context.CountersManager.TestIfAnyCountersMoving()) return;

            ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.RollForGoesFirst;

            if (Context.GameConfig.IfFastForwarding && !Main.Instance.IfPlayerVsAI)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ConfigureBoardForContinue;
            else if (Main.Instance.IfPlayerVsAI && Context.GameConfig.IfAIGameToContinue)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ConfigureBoardForContinue;
            else if (Game2D.IfManualSetupBoard)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ConfigureBoardManual;

            // NOTE: FAST FORWARD A.I. GAME IS FOR DEBUG ONLY
            else if (Context.GameConfig.IfFastForwarding && Main.Instance.IfPlayerVsAI)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.TurnConfigure;
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        private void SetCountersToPlayerStart()
        {
            Context.HomeManager.ResetPlayerHome();

            Context.PointsManager.SetCountersToStartingPoints(true,
                                                        Context.GameConfig.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                                           Context.CountersManager.WhiteCounters);
        }

        private void SetCountersToOpponentStart()
        {
            Context.HomeManager.ResetOpponentHome();

            Context.PointsManager.SetCountersToStartingPoints(false,
                                                        !Context.GameConfig.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                                            Context.CountersManager.WhiteCounters);
        }

        private void SetCountersToBoardState(GameStateContext2D.BoardState boardState)
        {
            Context.HomeManager.ResetPlayerHome();
            Context.HomeManager.ResetOpponentHome();
            Context.PointsManager.ClearAllCountersFromPoints();

            var playerCounters = Context.GameConfig.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                           Context.CountersManager.WhiteCounters;
            var opponentCounters = !Context.GameConfig.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                              Context.CountersManager.WhiteCounters;

            // CONFIGURE COUNTERS TO POINTS
            Context.PointsManager.SetCountersFromArray(boardState.Points, playerCounters, opponentCounters);

            // COUNTERS TO BAR
            var playerBar = Context.BarManager.PlayerBar;
            for (int pb = boardState.PlayerBarCount; pb > 0; pb--)
            {
                var counter = playerCounters[(15 - boardState.PlayerHomeCount - pb)];
                counter.SetCounterToMoveToBarInstant(playerBar.GetCounterOffsetPosition());
                playerBar.PushCounter(counter);
            }

            var opponentBar = Context.BarManager.OpponentBar;
            for (int ob = boardState.OpponentBarCount; ob > 0; ob--)
            {
                var counter = opponentCounters[(15 - boardState.OpponentHomeCount - ob)];
                counter.SetCounterToMoveToBarInstant(opponentBar.GetCounterOffsetPosition());
                opponentBar.PushCounter(counter);
            }

            // COUNTERS TO HOME
            var playerHome = Context.HomeManager.PlayerHome;
            for (int ph = boardState.PlayerHomeCount; ph > 0; ph--)
            {
                var counter = playerCounters[(15 - ph)];
                counter.SetCounterToMoveToHomeInstant(playerHome.GetCounterOffsetPosition());
                Context.CountersManager.SetCounterSizeAndColour(counter, false);
                playerHome.PushCounter(counter);
            }

            var opponentHome = Context.HomeManager.OpponentHome;
            for (int oh = boardState.OpponentHomeCount; oh > 0; oh--)
            {
                var counter = opponentCounters[(15 - oh)];
                counter.SetCounterToMoveToHomeInstant(opponentHome.GetCounterOffsetPosition());
                Context.CountersManager.SetCounterSizeAndColour(counter, false);
                opponentHome.PushCounter(counter);
            }
        }
    }
}