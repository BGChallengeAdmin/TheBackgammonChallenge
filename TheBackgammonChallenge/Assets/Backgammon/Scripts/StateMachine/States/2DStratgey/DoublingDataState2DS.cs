using UnityEngine;

namespace Backgammon
{
    public class DoublingDataState2DS : GameState2DStrategy
    {
        public DoublingDataState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING DATA");

            // CONSTRUCT AI DATA REQUEST
            aiDataToSend = AIDataHandler.AIDataObject;

            // RESET THE DOUBLING RESPONSE - NO GUARANTEE OF SERVER RESPONSE
            if (Context.TurnConfig.AIDoublingData != null)
            {
                Context.TurnConfig.AIDoublingData.cubeDecision.owner = "hold";
                Context.TurnConfig.AIDoublingData.cubeDecision.other = "accept";
            }

            ConstructDataToSend();

            // SEND DATA COROUTINE
            Context.AIDataHandler.SendAIData(aiDataToSend, true);

            _delayTimer = _delayTime;
        }

        public override void UpdateState()
        {
            // TEST FOR A.I. DATA
            if (!Context.TurnConfig.AIDataAvailable)
            {
                Context.TurnConfig.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.TurnConfig.AIDataAvailable)
                {
                    Context.TurnConfig.AIDoublingData = Context.AIDataHandler.AIDoublingResponse();

                    if (Context.TurnConfig.PlayerOfferedDoubles)
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.DoublingInGame;
                    else ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.DoublingOffers;

                    var state = Context.Strategy.DoublingDataUpdate(Context, ActiveState);

                    if (state != StateKey)
                        ActiveState = state;

                    Context.TurnConfig.AIDataAvailable = false;
                }
            }

            // BAIL OUT TIMER
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                ActiveState = Context.Strategy.DoublingDataBailOut(Context);
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void ConstructDataToSend()
        {
            // NOTE: "red" IS WHO'S ON - POSITIVE VALUES ON THE POINTS

            aiDataBoardHelper = AIDataHandler.GetAIDataBoardHelper;

            // CUBE
            aiDataBoardHelper.cube = Context.DoublingManager.CubeValue;
            aiDataBoardHelper.cubeowner = Context.DoublingManager.CubeOwner == Game2D.PlayingAs.NONE ? "none" :
                                          (Context.TurnConfig.IsPlayersTurn ?
                                          Context.DoublingManager.CubeOwner == Context.GameConfig.PlayingAs ? "red" : "green" :
                                          Context.DoublingManager.CubeOwner == Context.GameConfig.OpponentAs ? "red" : "green");

            // POINTS
            // NOTE: ACTIVE PLAYER IS ALWAYS '+' POINTS VALUES - FROM THEIR 24
            var points = new int[24];

            for (int p = 24; p > 0; p--)
            {
                var point = Context.TurnConfig.IsPlayersTurn ?
                                    Context.PointsManager.GetPlayerPointByID(p) :
                                    Context.PointsManager.GetOpponentPointByID(p);

                if (point.Counters == 0) continue;

                points[24 - p] = point.Counters * (Context.TurnConfig.IsPlayersTurn ?
                                                    (point.Owner == Context.GameConfig.PlayingAs ? 1 : -1) :
                                                    (point.Owner == Context.GameConfig.PlayingAs ? -1 : 1));
            }

            aiDataBoardHelper.points = points;

            // BAR
            aiDataBarHelper = AIDataHandler.GetAIDataBarHelper;
            aiDataBarHelper.red = Context.TurnConfig.IsPlayersTurn ? Context.BarManager.PlayerBar.Counters :
                                                          Context.BarManager.OpponentBar.Counters;
            aiDataBarHelper.green = Context.TurnConfig.IsPlayersTurn ? Context.BarManager.OpponentBar.Counters :
                                                          Context.BarManager.PlayerBar.Counters;
            aiDataBoardHelper.bar = aiDataBarHelper;

            // HOME
            aiDataOffHelper = AIDataHandler.GetAIDataOffHelper;
            aiDataOffHelper.red = Context.TurnConfig.IsPlayersTurn ? Context.HomeManager.PlayerHome.Counters :
                                                          Context.HomeManager.OpponentHome.Counters;
            aiDataOffHelper.green = Context.TurnConfig.IsPlayersTurn ? Context.HomeManager.OpponentHome.Counters :
                                                          Context.HomeManager.PlayerHome.Counters;
            aiDataBoardHelper.off = aiDataOffHelper;

            // DICE
            aiDataDiceHelper = AIDataHandler.GetAIDataDiceHelper;
            aiDataDiceHelper.red = 0;
            aiDataDiceHelper.green = 0;

            // GAME SCORE
            aiDataScoreHelper = AIDataHandler.GetAIDataScoreHelper;
            aiDataScoreHelper.red = Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player1PointsAtStart :
                                                                 Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player2PointsAtStart;
            aiDataScoreHelper.green = Context.GameConfig.IfPlayingAsPlayer1 ? Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player2PointsAtStart :
                                                                 Context.GameConfig.SelectedMatch.Game(Context.GameConfig.IndexGame).Player1PointsAtStart;

            // MATCH SCORE
            aiDataPositionHelper = AIDataHandler.GetAIDataPositionHelper;
            aiDataPositionHelper.matchLength = Context.GameConfig.SelectedMatch.Points.ToString();
            aiDataPositionHelper.whosOn = "red";
            aiDataPositionHelper.crawford = Context.GameConfig.SelectedMatch.Crawford == "true" ? true : false;

            aiDataPositionHelper.board = aiDataBoardHelper;
            aiDataPositionHelper.dice = aiDataDiceHelper;
            aiDataPositionHelper.score = aiDataScoreHelper;

            // BUILD DATA TO SEND
            aiDataToSend.position = aiDataPositionHelper;
        }

        private AIData aiDataToSend;
        private AIDataPositionHelper aiDataPositionHelper;
        private AIDataDiceHelper aiDataDiceHelper;
        private AIDataScoreHelper aiDataScoreHelper;
        private AIDataBoardHelper aiDataBoardHelper;
        private AIDataBarHelper aiDataBarHelper;
        private AIDataOffHelper aiDataOffHelper;

        private float _delayTime = 3f;
        private float _delayTimer = 3f;
    }
}