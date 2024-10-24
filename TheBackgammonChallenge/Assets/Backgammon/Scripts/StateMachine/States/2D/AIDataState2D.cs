using System.Diagnostics;
using UnityEngine;

namespace Backgammon
{
    public class AIDataState2D : GameState2D
    {
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

        public AIDataState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: AI DATA");

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
                        
            // TODO: IF FAILS TO CONNECT - DISCONNECT
            // DOES FAILURE CAUSE APP TO HANG??
            //Context.AIDataHandler.Send();

            // NORMAL GAME PLAY - OR A.I. PLAYER TURN
            if (!Main.Instance.IfPlayerVsAI || 
                (Main.Instance.IfPlayerVsAI && Context.IsPlayersTurn && !Context.IfFastForwarding))
                    ActiveState = GameStateMachine2D.EGameState2D.RollDice;

            _delayTimer = _delayTime;
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            // NOTE: A.I. RESPONSE IS REQUIRED BEFORE A.I. TURN BEGIN
            if (Main.Instance.IfPlayerVsAI && !Context.AIDataAvailable)
            {
                Context.AIDataAvailable = Context.AIDataHandler.IfNewData();

                if (Context.AIDataAvailable)
                {
                    // BENCHMARKING
                    if (_benchmarking)
                    {
                        _stopwatch.Stop();
                        _connectionTimesArray[_connectionCounter++] = _stopwatch.ElapsedMilliseconds;
                        _connectionAverageTime += _stopwatch.ElapsedMilliseconds;
                    }

                    // RESPONSE DATA
                    Context.AIRankedMoves = Context.AIDataHandler.AIResponseMoves();
                    Context.AITopRankedMove = Context.AIDataHandler.AIResponse();
                    Context.CapturedAIMoveData = true;

                    // RANK DISRTIBUTION
                    _range = 0f;

                    if (Context.AIRankedMoves[0] is not null)
                    {
                        var upperProb = Context.AIRankedMoves[0].probabilities.redWin;
                        var lowerProb = Context.AIRankedMoves[Context.AIRankedMoves.Length - 1].probabilities.redWin;
                        _range = upperProb - lowerProb;
                    }

                    _averageRange += _range;
                    _rangeCounter++;

                    if (_range > _highestRange) _highestRange = _range;
                    if (_range < _lowestRange) _lowestRange = _range;

                    ActiveState = GameStateMachine2D.EGameState2D.TurnBegin;
                }

                // BAIL OUT TIMER
                _delayTimer -= Time.deltaTime;

                if (_delayTimer <= 0)
                {
                    Context.GameScreenUI.SetAIDataDelayActive(true);

                    _delayTimer = _delayTime;

                    if (Context.AIDataRequestBailOutCounter > 2)
                    {
                        UnityEngine.Debug.Log($"***DATA STATE BAIL OUT COUNTER***");

                        if (Context.IfFastForwarding)
                        {
                            Context.AIDataRequestBailOutCounter = 0;
                        }
                        else
                        {
                            Context.GeneralInfoStateSwitch = GeneralInfoState2D.EGeneralInfoState2D.AIDataRequestFailed;
                            ActiveState = GameStateMachine2D.EGameState2D.GeneralInfo;
                        }
                    }

                    Context.AIDataRequestBailOutCounter += 1;
                }
            }
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

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        private void ConstructDataToSend()
        {
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
            // NOTE: HIGHEST DICE FIRST -> UNLESS DOUBLES BOTH '0'
            aiDataDiceHelper = AIDataHandler.GetAIDataDiceHelper;
            aiDataDiceHelper.red = Context.Dice1 > Context.Dice2 ? Context.Dice1 : Context.Dice2;
            aiDataDiceHelper.green = Context.Dice1 > Context.Dice2 ? Context.Dice2 : Context.Dice1;

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
            _aiDataToSend.position = aiDataPositionHelper;

            if (Main.Instance.IfPlayerVsAI)
            {
                playingAgainstAITurn = !Context.IsPlayersTurn;
                _aiDataToSend.noOfMoves = (playingAgainstAITurn ? Context.AISettings.PlayerRankHighest.ToString() : "5");
                _aiDataToSend.noise = doublingInGame ? 0 : (playingAgainstAITurn ? Context.AISettings.PlayerNoise : 0);
                _aiDataToSend.ply = (playingAgainstAITurn ? Context.AISettings.PlayerPly : 2);
                _aiDataToSend.comment = $"{(Context.IsPlayersTurn ? "PLAYER" : "A.I.")}";
            }
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
                UnityEngine.Debug.Log($"POINT {p} -> {points[p - 1]} -> {point.Owner}");
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
            aiDataDiceHelper.red = Context.Dice1 > Context.Dice2 ? Context.Dice1 : Context.Dice2;
            aiDataDiceHelper.green = Context.Dice1 > Context.Dice2 ? Context.Dice2 : Context.Dice1;

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
            _aiDataToSend.position = aiDataPositionHelper;
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