using System.Diagnostics;
using UnityEngine;

namespace Backgammon
{
    public class AIDataState2DS : GameState2DStrategy
    {
        public AIDataState2DS(GameStateContext2DStrategy context, GameStateMachine2DStrategy.EGameState2DStrategy estate) : base(context, estate) { }

        // BENCHMARKING
        private bool _benchmarking = false;
        private int _connectionCounter = 0;
        private Stopwatch _stopwatch = new Stopwatch();
        private long[] _connectionTimesArray = new long[100];
        private long _connectionAverageTime = 0;

        // RANK DISTRIBUTION
        private int _rangeCounter = 0;
        private float _range = 0;
        private float _averageRange = 0;
        private float _lowestRange = 100;
        private float _highestRange = -1;

        public override void EnterState()
        {
            Context.GameConfig.Debug_reportState.DebugMessage($"ENTER STATE: AI DATA");

            // CONSTRUCT AI DATA REQUEST
            _aiDataToSend = AIDataHandler.AIDataObject;

            ConstructDataToSend();

            if (_benchmarking)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
            }

            // SEND DATA COROUTINE ON SEND
            Context.AIDataHandler.SendAIData(_aiDataToSend);

            var state = Context.Strategy.AIDataEnter(Context);

            if (state != StateKey)
                ActiveState = state;

            _delayTimer = _delayTime;
        }

        public override void UpdateState()
        {
            // NOTE: A.I. RESPONSE IS REQUIRED BEFORE A.I. TURN BEGIN
            var state = Context.Strategy.AIDataUpdate(Context);

            if (state != StateKey)
            {
                _delayTimer = _delayTime;
                ActiveState = state;
            }

            // BAIL OUT TIMER
            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                Context.GameScreenUI.SetAIDataDelayActive(true);

                _delayTimer = _delayTime;

                if (Context.TurnConfig.AIDataRequestBailOutCounter > 2)
                {
                    UnityEngine.Debug.Log($"***DATA STATE BAIL OUT COUNTER***");

                    if (Context.GameConfig.IfFastForwarding)
                    {
                        Context.TurnConfig.AIDataRequestBailOutCounter = 0;
                    }
                    else
                    {
                        Context.TurnConfig.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.AIDataRequestFailed;
                        ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.GeneralInfo;
                    }
                }

                Context.TurnConfig.AIDataRequestBailOutCounter += 1;
            }

            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2DStrategy.EGameState2DStrategy.ExitGame;
        }

        public override void ExitState()
        {
            // BENCHMARKING
            if (_benchmarking)
            {
                UnityEngine.Debug.Log($"REQUEST: {_connectionCounter} : {_stopwatch.ElapsedMilliseconds}ms " +
                                      $"-> AVG. {(_connectionAverageTime / _connectionCounter)}ms");
            }

            // RANGE
            //UnityEngine.Debug.Log($"RANGE {_rangeCounter} - EQUITY {_range} - AVG. {(_averageRange / _rangeCounter) * 100f} " +
            //                      $"-> L/H {_lowestRange * 100f} / {_highestRange * 100f}");

            Context.GameScreenUI.SetAIDataDelayActive(false);

            ActiveState = StateKey;
        }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetStateKey() { return StateKey; }

        public override GameStateMachine2DStrategy.EGameState2DStrategy GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void ConstructDataToSend()
        {
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
            // NOTE: HIGHEST DICE FIRST -> UNLESS DOUBLES BOTH '0'
            aiDataDiceHelper = AIDataHandler.GetAIDataDiceHelper;
            aiDataDiceHelper.red = Context.TurnConfig.Dice1 > Context.TurnConfig.Dice2 ? Context.TurnConfig.Dice1 : Context.TurnConfig.Dice2;
            aiDataDiceHelper.green = Context.TurnConfig.Dice1 > Context.TurnConfig.Dice2 ? Context.TurnConfig.Dice2 : Context.TurnConfig.Dice1;

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
            _aiDataToSend.position = aiDataPositionHelper;

            _aiDataToSend = Context.Strategy.AIDataEnterConstructData(Context, _aiDataToSend);
        }

        private AIData _aiDataToSend;
        private AIDataPositionHelper aiDataPositionHelper;
        private AIDataDiceHelper aiDataDiceHelper;
        private AIDataScoreHelper aiDataScoreHelper;
        private AIDataBoardHelper aiDataBoardHelper;
        private AIDataBarHelper aiDataBarHelper;
        private AIDataOffHelper aiDataOffHelper;

        private bool playingAgainstAITurn;
        private object highestAILevel;
        private bool doublingInGame;
        private int aiNoise;
        private int aiPly;

        private float _delayTime = 3f;
        private float _delayTimer = 3f;
    }
}