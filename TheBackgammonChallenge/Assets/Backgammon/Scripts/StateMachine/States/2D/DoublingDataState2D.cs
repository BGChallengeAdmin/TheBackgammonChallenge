using UnityEngine;

namespace Backgammon
{
    public class DoublingDataState2D : GameState2D
    {
        public DoublingDataState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: DOUBLING DATA");

            // CONSTRUCT AI DATA REQUEST
            aiDataToSend = AIDataHandler.AIDataObject;

            // RESET THE DOUBLING RESPONSE - NO GUARANTEE OF SERVER RESPONSE
            if (Context.AIDoublingData is not null)
            {
                Context.AIDoublingData.cubeDecision.owner = "hold";
                Context.AIDoublingData.cubeDecision.other = "accept";
            }

            ConstructDataToSend();

            // SEND DATA COROUTINE
            Context.AIDataHandler.SendAIData(aiDataToSend, true);
            //Context.AIDataHandler.Send();

            _delayTimer = _delayTime;
        }

        public override void UpdateState()
        {
            // TEST FOR A.I. DATA
            if (!Context.AIDataAvailable)
            {
                Context.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.AIDataAvailable)
                {
                    Context.AIDoublingData = Context.AIDataHandler.AIDoublingResponse();

                    if (Context.PlayerOfferedDoubles)
                        ActiveState = GameStateMachine2D.EGameState2D.DoublingInGame;
                    else ActiveState = GameStateMachine2D.EGameState2D.DoublingOffers;

                    if (Main.Instance.IfPlayerVsAI && !Context.IsPlayersTurn && !Context.CapturedAIDoublingData)
                    {
                        Context.CapturedAIDoublingData = true;

                        if (Context.AIDoublingData.cubeDecision.owner == "double" ||
                            Context.AIDoublingData.cubeDecision.owner == "redouble")
                                ActiveState = GameStateMachine2D.EGameState2D.TurnBegin;
                        else
                            ActiveState = GameStateMachine2D.EGameState2D.TurnConfigure;
                    }

                    Context.AIDataAvailable = false;
                }
            }

            // BAIL OUT TIMER
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                if (Context.PlayerOfferedDoubles)
                    ActiveState = GameStateMachine2D.EGameState2D.DoublingInGame;
                else ActiveState = GameStateMachine2D.EGameState2D.DoublingOffers;

                if (Main.Instance.IfPlayerVsAI)
                    ActiveState = GameStateMachine2D.EGameState2D.AIData;
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void ConstructDataToSend()
        {
            // NOTE: "red" IS WHO'S ON - POSITIVE VALUES ON TEH POINTS

            aiDataBoardHelper = AIDataHandler.GetAIDataBoardHelper;

            // CUBE
            aiDataBoardHelper.cube = Context.DoublingManager.CubeValue;
            aiDataBoardHelper.cubeowner = Context.DoublingManager.CubeOwner == Game2D.PlayingAs.NONE ? "none" :
                                          (Context.IsPlayersTurn ?
                                          Context.DoublingManager.CubeOwner == Context.PlayingAs ? "red" : "green" :
                                          Context.DoublingManager.CubeOwner == Context.OpponentAs ? "red" : "green");

            // POINTS
            // NOTE: ACTIVE PLAYER IS ALWAYS '+' POINTS VALUES - FROM THEIR 24
            var points = new int[24];

            for (int p = 24; p > 0; p--)
            {
                var point = Context.IsPlayersTurn ?
                                    Context.PointsManager.GetPlayerPointByID(p) :
                                    Context.PointsManager.GetOpponentPointByID(p);

                if (point.Counters == 0) continue;

                points[24 - p] = point.Counters * (Context.IsPlayersTurn ?
                                                    (point.Owner == Context.PlayingAs ? 1 : -1) :
                                                    (point.Owner == Context.PlayingAs ? -1 : 1));
            }

            aiDataBoardHelper.points = points;

            // BAR
            aiDataBarHelper = AIDataHandler.GetAIDataBarHelper;
            aiDataBarHelper.red = Context.IsPlayersTurn ? Context.BarManager.PlayerBar.Counters :
                                                          Context.BarManager.OpponentBar.Counters;
            aiDataBarHelper.green = Context.IsPlayersTurn ? Context.BarManager.OpponentBar.Counters :
                                                          Context.BarManager.PlayerBar.Counters;
            aiDataBoardHelper.bar = aiDataBarHelper;

            // HOME
            aiDataOffHelper = AIDataHandler.GetAIDataOffHelper;
            aiDataOffHelper.red = Context.IsPlayersTurn ? Context.HomeManager.PlayerHome.Counters :
                                                          Context.HomeManager.OpponentHome.Counters;
            aiDataOffHelper.green = Context.IsPlayersTurn ? Context.HomeManager.OpponentHome.Counters :
                                                          Context.HomeManager.PlayerHome.Counters;
            aiDataBoardHelper.off = aiDataOffHelper;

            // DICE
            aiDataDiceHelper = AIDataHandler.GetAIDataDiceHelper;
            aiDataDiceHelper.red = 0;
            aiDataDiceHelper.green = 0;

            // GAME SCORE
            aiDataScoreHelper = AIDataHandler.GetAIDataScoreHelper;
            aiDataScoreHelper.red = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Game(Context.IndexGame).Player1PointsAtStart :
                                                                 Context.SelectedMatch.Game(Context.IndexGame).Player2PointsAtStart;
            aiDataScoreHelper.green = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Game(Context.IndexGame).Player2PointsAtStart :
                                                                 Context.SelectedMatch.Game(Context.IndexGame).Player1PointsAtStart;

            // MATCH SCORE
            aiDataPositionHelper = AIDataHandler.GetAIDataPositionHelper;
            aiDataPositionHelper.matchLength = Context.SelectedMatch.Points.ToString();
            aiDataPositionHelper.whosOn = "red";
            aiDataPositionHelper.crawford = Context.SelectedMatch.Crawford == "true" ? true : false;

            aiDataPositionHelper.board = aiDataBoardHelper;
            aiDataPositionHelper.dice = aiDataDiceHelper;
            aiDataPositionHelper.score = aiDataScoreHelper;

            // BUILD DATA TO SEND
            aiDataToSend.position = aiDataPositionHelper;
        }

        private void ConstructDataToSend1()
        {
            // NOTE: "red" IS WHO'S ON - POSITIVE VALUES ON THE POINTS

            aiDataBoardHelper = AIDataHandler.GetAIDataBoardHelper;

            // CUBE
            aiDataBoardHelper.cube = Context.DoublingManager.CubeValue;
            aiDataBoardHelper.cubeowner = Context.DoublingManager.CubeOwner == Game2D.PlayingAs.NONE ? "none" :
                                          (Context.DoublingManager.CubeOwner == Context.PlayingAs) ? "red" : "green";

            // POINTS
            // NOTE: ACTIVE PLAYER IS ALWAYS '+' POINTS VALUES - FROM THEIR 24
            var points = new int[24];

            for (int p = 24; p > 0; p--)
            {
                var point = Context.IsPlayersTurn ?
                                    Context.PointsManager.GetPlayerPointByID(p) :
                                    Context.PointsManager.GetOpponentPointByID(p);

                if (point.Counters == 0) continue;

                points[24 - p] = point.Counters * (point.Owner == Context.PlayingAs ? 1 : -1);
            }

            for (int p = 1; p <= 24; p++)
            {
                var point = Context.PointsManager.GetPlayerPointByID(p);

                if (point.Counters == 0) continue;

                points[p - 1] = point.Counters * ((point.Owner == Game2D.PlayingAs.PLAYER_1 && Context.IfPlayingAsPlayer1) ? 1 : -1);
            }

            aiDataBoardHelper.points = points;

            // BAR
            aiDataBarHelper = AIDataHandler.GetAIDataBarHelper;
            aiDataBarHelper.red = Context.BarManager.PlayerBar.Counters;
            aiDataBarHelper.green = Context.BarManager.OpponentBar.Counters;
            aiDataBoardHelper.bar = aiDataBarHelper;

            // HOME
            aiDataOffHelper = AIDataHandler.GetAIDataOffHelper;
            aiDataOffHelper.red = Context.HomeManager.PlayerHome.Counters;
            aiDataOffHelper.green = Context.HomeManager.OpponentHome.Counters;
            aiDataBoardHelper.off = aiDataOffHelper;

            // DICE
            aiDataDiceHelper = AIDataHandler.GetAIDataDiceHelper;
            aiDataDiceHelper.red = 0;
            aiDataDiceHelper.green = 0;

            // GAME SCORE
            aiDataScoreHelper = AIDataHandler.GetAIDataScoreHelper;
            aiDataScoreHelper.red = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Game(Context.IndexGame).Player1PointsAtStart :
                                                                 Context.SelectedMatch.Game(Context.IndexGame).Player2PointsAtStart;
            aiDataScoreHelper.green = Context.IfPlayingAsPlayer1 ? Context.SelectedMatch.Game(Context.IndexGame).Player2PointsAtStart :
                                                                 Context.SelectedMatch.Game(Context.IndexGame).Player1PointsAtStart;

            // MATCH SCORE
            aiDataPositionHelper = AIDataHandler.GetAIDataPositionHelper;
            aiDataPositionHelper.matchLength = Context.SelectedMatch.Points.ToString();
            aiDataPositionHelper.whosOn = Context.IsPlayersTurn ? "red" : "green";
            aiDataPositionHelper.crawford = Context.SelectedMatch.Crawford == "true" ? true : false;

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