using UnityEngine;

namespace Backgammon
{
    public class DoublingDataState : GameState
    {
        public DoublingDataState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            // CONSTRUCT AI DATA REQUEST
            aiDataToSend = AIDataHandler.AIDataObject;

            ConstructDataToSend();

            // SEND DATA COROUTINE
            Context.AIDataHandler.SendAIData(aiDataToSend);
            Context.AIDataHandler.Send();

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
                        ActiveState = GameStateMachine.EGameState.DoublingInGame;
                    else ActiveState = GameStateMachine.EGameState.DoublingOffers;

                    Context.AIDataAvailable = false;
                }
            }

            // BAIL OUT TIMER
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                if (Context.PlayerOfferedDoubles)
                    ActiveState = GameStateMachine.EGameState.DoublingInGame;
                else ActiveState = GameStateMachine.EGameState.DoublingOffers;
            }
        }

        public override void ExitState()
        {
            ActiveState = StateKey;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void ConstructDataToSend()
        {
            aiDataBoardHelper = AIDataHandler.GetAIDataBoardHelper;

            // CUBE
            aiDataBoardHelper.cube = Context.DoublingManager.CubeValue;
            aiDataBoardHelper.cubeowner = Context.DoublingManager.CubeOwner == Game.PlayingAs.NONE ? "none" :
                                          Context.IsPlayersTurn ?
                                          ((Context.DoublingManager.CubeOwner == Context.PlayingAs) ? "red" : "green") :
                                          ((Context.DoublingManager.CubeOwner == Context.PlayingAs) ? "green" : "red");

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