namespace Backgammon
{
    public class ConfigureBoardManualState2D : GameState2D
    {
        public ConfigureBoardManualState2D(GameStateContext2D context, GameStateMachine2D.EGameState2D estate) : base(context, estate) { }

        public override void EnterState()
        {
            Context.Debug_reportState.DebugMessage($"ENTER STATE: CONFIGURE BOARD MANUAL");

            Context.ConfigureBoardManualUI.SetActive(true);

            ResetAllPointsAndCounters();

            // MAKES POINTS SELECTABLE
            Context.PointsManager.SetAllPointsActive(true);
        }

        public override void UpdateState()
        {
            // EXIT GAME / STATE MACHINE
            if (Context.ExitFromStateMachine)
                ActiveState = GameStateMachine2D.EGameState2D.ExitGame;

            if (Context.PointsManager.ManualSetupPointSelected)
            {
                Context.PointsManager.ManualSetupPointSelected = false;

                // TEST COUNTERS LEFT TO PLACE
                if (TestIfCountersLeftToPlace())
                {
                    var point = Context.PointsManager.GetPlayerPointByID(Context.PointsManager.SelectedPointFrom);

                    // TEST CAN PLACE COUNTERS HERE
                    if (Context.ConfigureBoardManualUI.PlacingPlayerCounters &&
                        point.Owner == Game2D.PlayingAs.PLAYER_2) return;
                    else if (!Context.ConfigureBoardManualUI.PlacingPlayerCounters &&
                        point.Owner == Game2D.PlayingAs.PLAYER_1) return;

                    // SELECT COUNTER AND MOVE TO POINT
                    if (Context.ConfigureBoardManualUI.PlacingPlayerCounters)
                    {
                        _counterToPlace = Context.HomeManager.PlayerHome.PopCounter();
                        _playerCounterIndex--;
                    }
                    else
                    {
                        _counterToPlace = Context.HomeManager.OpponentHome.PopCounter();
                        _opponentCounterIndex--;
                    }

                    Context.ConfigureBoardManualUI.IfLastPlacedPlayerCounter = Context.ConfigureBoardManualUI.PlacingPlayerCounters;
                    Context.ConfigureBoardManualUI.SetUndoInteractive(true);

                    _counterToPlace.SetCounterToMoveToPosition(Context.PointsManager.GetAdjustedPointPosition(point), Context.IfPlayerIsBlack);
                    Context.CountersManager.SetCounterSizeAndColour(_counterToPlace, true);
                    point.PushCounter(_counterToPlace);

                    // TEST IF NEED TO SWAP COUNTERS
                    TestIfNeedToSwapCountersPlaced();
                    TestIfReadyToCommence();
                }
                else
                {
                    UnityEngine.Debug.Log($"ALL COUNTERS HAVE BEEN PLACED");
                    TestIfReadyToCommence();
                }
            }

            if (Context.ConfigureBoardManualUI.IfClickedToBar)
            {
                Context.ConfigureBoardManualUI.IfClickedToBar = false;

                if (TestIfCountersLeftToPlace())
                {
                    if (Context.ConfigureBoardManualUI.PlacingPlayerCounters)
                    {
                        _counterToPlace = Context.HomeManager.PlayerHome.PopCounter();
                        _counterToPlace.SetCounterToMoveToBar(Context.BarManager.PlayerBar.GetCounterOffsetPosition());
                        Context.BarManager.PlayerBar.PushCounter(_counterToPlace);
                        _playerCounterIndex--;
                    }
                    else
                    {
                        _counterToPlace = Context.HomeManager.OpponentHome.PopCounter();
                        _counterToPlace.SetCounterToMoveToBar(Context.BarManager.OpponentBar.GetCounterOffsetPosition());
                        Context.BarManager.OpponentBar.PushCounter(_counterToPlace);
                        _opponentCounterIndex--;
                    }

                    Context.ConfigureBoardManualUI.IfLastPlacedPlayerCounter = Context.ConfigureBoardManualUI.PlacingPlayerCounters;
                    Context.CountersManager.SetCounterSizeAndColour(_counterToPlace, true);

                    // TEST IF NEED TO SWAP COUNTERS
                    TestIfNeedToSwapCountersPlaced();
                    TestIfReadyToCommence();
                }
                else
                {
                    UnityEngine.Debug.Log($"ALL COUNTERS HAVE BEEN PLACED");
                }
            }

            if (Context.ConfigureBoardManualUI.IfClickedUndo)
            {
                Context.ConfigureBoardManualUI.IfClickedUndo = false;
                Context.ConfigureBoardManualUI.SetUndoInteractive(false);

                var id = _counterToPlace.ParentID;

                if (id == 25)
                {
                    _counterToPlace = Context.ConfigureBoardManualUI.IfLastPlacedPlayerCounter ? 
                                        Context.BarManager.PlayerBar.PopCounter() :
                                        Context.BarManager.OpponentBar.PopCounter();
                }
                else if (id > 0)
                {
                    _counterToPlace = Context.PointsManager.GetPlayerPointByID(id).PopCounter();
                }
                else if (id == 0)
                {
                    _counterToPlace = Context.ConfigureBoardManualUI.IfLastPlacedPlayerCounter ? 
                                        Context.HomeManager.PlayerHome.PopCounter() :
                                        Context.HomeManager.OpponentHome.PopCounter();
                }

                if (Context.ConfigureBoardManualUI.IfLastPlacedPlayerCounter)
                {
                    _counterToPlace.SetCounterToMoveToHome(Context.HomeManager.PlayerHome.GetCounterOffsetPosition());
                    Context.HomeManager.PlayerHome.PushCounter(_counterToPlace);
                    _playerCounterIndex++;
                }
                else
                {
                    _counterToPlace.SetCounterToMoveToHome(Context.HomeManager.OpponentHome.GetCounterOffsetPosition());
                    Context.HomeManager.OpponentHome.PushCounter(_counterToPlace);
                    _opponentCounterIndex++;
                }

                Context.CountersManager.SetCounterSizeAndColour(_counterToPlace, false);
            }

            if (Context.ConfigureBoardManualUI.IfClickedReset)
            {
                Context.ConfigureBoardManualUI.IfClickedReset = false;
                ResetAllPointsAndCounters();
            }

            if (Context.ConfigureBoardManualUI.IfClickedExit)
            {
                Game2D.IfManualSetupBoard = false;
                Context.ConfigureBoardManualUI.IfClickedExit = false;
                Context.ExitFromStateMachine = true;
            }

            if (!Context.ConfigureBoardManualUI.IfClickedDone) return;

            Game2D.IfManualSetupBoard = false;
            Context.PointsManager.SetAllPointsActive(false);

            ActiveState = GameStateMachine2D.EGameState2D.RollForGoesFirst;
        }

        public override void ExitState()
        {
            Context.PointsManager.ResetFromManualPlacement();
            Context.ConfigureBoardManualUI.SetActive(false);

            ActiveState = StateKey;
        }

        private void TestIfNeedToSwapCountersPlaced()
        {
            if (_playerCounterIndex <= 0 && Context.ConfigureBoardManualUI.PlacingPlayerCounters)
                Context.ConfigureBoardManualUI.SwapCounterToPlaceType();
            else if (_opponentCounterIndex <= 0 && !Context.ConfigureBoardManualUI.PlacingPlayerCounters)
                Context.ConfigureBoardManualUI.SwapCounterToPlaceType();
        }

        private bool TestIfCountersLeftToPlace()
        {
            return (_playerCounterIndex > 0 || _opponentCounterIndex > 0);
        }

        private void TestIfReadyToCommence()
        {
            if (_playerCounterIndex <= 0 && _opponentCounterIndex<= 0)
            {
                Context.ConfigureBoardManualUI.SetReadyToCommence(true);
            }
        }

        private void ResetAllPointsAndCounters()
        {            
            // RESET ALL POINTS
            Context.PointsManager.ClearAllCountersFromPoints();
            Context.HomeManager.ResetPlayerHome();
            Context.HomeManager.ResetOpponentHome();
            Context.BarManager.PlayerBar.ResetCounters();
            Context.BarManager.OpponentBar.ResetCounters();

            // MOVE ALL COUNTERS TO HOME
            var playerHome = Context.HomeManager.PlayerHome;
            _playerCounters = Context.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                           Context.CountersManager.WhiteCounters;

            for (int p = 0; p < _playerCounters.Length; p++)
            {
                var counter = _playerCounters[p];
                counter.SetCounterToMoveToHomeInstant(playerHome.GetCounterOffsetPosition());
                Context.CountersManager.SetCounterSizeAndColour(counter, false);
                playerHome.PushCounter(counter);
            }

            var opponentHome = Context.HomeManager.OpponentHome;
            _opponentCounters = !Context.IfPlayerIsBlack ? Context.CountersManager.BlackCounters :
                                                              Context.CountersManager.WhiteCounters;

            for (int o = 0; o < 15; o++)
            {
                var counter = _opponentCounters[o];
                counter.SetCounterToMoveToHomeInstant(opponentHome.GetCounterOffsetPosition());
                Context.CountersManager.SetCounterSizeAndColour(counter, false);
                opponentHome.PushCounter(counter);
            }

            _playerCounterIndex = 15;
            _opponentCounterIndex = 15;
        }

        public override GameStateMachine2D.EGameState2D GetStateKey() { return StateKey; }

        public override GameStateMachine2D.EGameState2D GetActiveState() { return ActiveState; }

        Counter2DPrefab[] _playerCounters;
        Counter2DPrefab[] _opponentCounters;
        Counter2DPrefab _counterToPlace;

        int _playerCounterIndex = 15;
        int _opponentCounterIndex = 15;
    }
}
