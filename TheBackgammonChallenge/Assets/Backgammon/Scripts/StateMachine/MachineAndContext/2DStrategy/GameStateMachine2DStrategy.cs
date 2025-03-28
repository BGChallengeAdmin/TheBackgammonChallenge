namespace Backgammon
{
    public class GameStateMachine2DStrategy : StateManager<GameStateMachine2DStrategy.EGameState2DStrategy>
    {
        private GameStateContext2DStrategy _context;

        internal void Init(GameStateContext2DStrategy context)
        {
            _context = context;

            InitializeStates();
            CurrentState.EnterState();
        }

        internal void Exit()
        {
            TransitionToNextState(EGameState2DStrategy.ExitGame);
        }

        private void InitializeStates()
        {
            ResetStates();

            States.Add(EGameState2DStrategy.ConfigureBoard, new ConfigureBoardState2DS(_context, EGameState2DStrategy.ConfigureBoard));
            States.Add(EGameState2DStrategy.ConfigureBoardManual, new ConfigureBoardManualState2DS(_context, EGameState2DStrategy.ConfigureBoardManual));
            States.Add(EGameState2DStrategy.ConfigureBoardForContinue, new ConfigureBoardForContinueState2DS(_context, EGameState2DStrategy.ConfigureBoardForContinue));
            States.Add(EGameState2DStrategy.RollForGoesFirst, new RollForGoesFirstState2DS(_context, EGameState2DStrategy.RollForGoesFirst));
            States.Add(EGameState2DStrategy.BeforeCommence, new BeforeCommenceState2DS(_context, EGameState2DStrategy.BeforeCommence));
            States.Add(EGameState2DStrategy.TurnConfigure, new TurnConfigureState2DS(_context, EGameState2DStrategy.TurnConfigure));
            States.Add(EGameState2DStrategy.TurnBegin, new TurnBeginState2DS(_context, EGameState2DStrategy.TurnBegin));
            States.Add(EGameState2DStrategy.AIData, new AIDataState2DS(_context, EGameState2DStrategy.AIData));
            States.Add(EGameState2DStrategy.DoublingData, new DoublingDataState2DS(_context, EGameState2DStrategy.DoublingData));
            States.Add(EGameState2DStrategy.DoublingOffers, new DoublingOffersState2DS(_context, EGameState2DStrategy.DoublingOffers));
            States.Add(EGameState2DStrategy.DoublingTakesOrDrops, new DoublingTakesOrDropsState2DS(_context, EGameState2DStrategy.DoublingTakesOrDrops));
            States.Add(EGameState2DStrategy.DoublingInGame, new DoublingInGameState2DS(_context, EGameState2DStrategy.DoublingInGame));
            States.Add(EGameState2DStrategy.RollDice, new DiceRollsState2DS(_context, EGameState2DStrategy.RollDice));
            States.Add(EGameState2DStrategy.SelectPointFrom, new SelectPointFromState2DS(_context, EGameState2DStrategy.SelectPointFrom));
            States.Add(EGameState2DStrategy.SelectPointTo, new SelectPointToState2DS(_context, EGameState2DStrategy.SelectPointTo));
            States.Add(EGameState2DStrategy.MoveCounters, new MoveCountersState2DS(_context, EGameState2DStrategy.MoveCounters));
            States.Add(EGameState2DStrategy.EvaluateAIRanks, new EvaluateAIRankedMovesState2DS(_context, EGameState2DStrategy.EvaluateAIRanks));
            States.Add(EGameState2DStrategy.EvaluatePlayer, new EvaluatePlayerMoveState2DS(_context, EGameState2DStrategy.EvaluatePlayer));
            States.Add(EGameState2DStrategy.EvaluateOpponent, new EvaluateOpponentMoveState2DS(_context, EGameState2DStrategy.EvaluateOpponent));
            States.Add(EGameState2DStrategy.AnalysisOrContinue, new AnalysisOrContinueState2DS(_context, EGameState2DStrategy.AnalysisOrContinue));
            States.Add(EGameState2DStrategy.Analysis, new AnalysisState2DS(_context, EGameState2DStrategy.Analysis));
            States.Add(EGameState2DStrategy.RestoreBoard, new RestoreBoardState2DS(_context, EGameState2DStrategy.RestoreBoard));
            States.Add(EGameState2DStrategy.ObserveBoard, new ObserveBoardState2DS(_context, EGameState2DStrategy.ObserveBoard));
            States.Add(EGameState2DStrategy.PlayoutProMoves, new PlayoutProMovesState2DS(_context, EGameState2DStrategy.PlayoutProMoves));
            States.Add(EGameState2DStrategy.TurnEnd, new TurnEndState2DS(_context, EGameState2DStrategy.TurnEnd));
            States.Add(EGameState2DStrategy.GameWon, new GameWonState2DS(_context, EGameState2DStrategy.GameWon));
            States.Add(EGameState2DStrategy.GeneralInfo, new GeneralInfoState2DS(_context, EGameState2DStrategy.GeneralInfo));
            States.Add(EGameState2DStrategy.ExitGame, new ExitGameState2DS(_context, EGameState2DStrategy.ExitGame));

            CurrentState = States[EGameState2DStrategy.ConfigureBoard];
        }

        public enum EGameState2DStrategy
        {
            ConfigureBoard,
            ConfigureBoardManual,
            ConfigureBoardForContinue,
            RollForGoesFirst,
            BeforeCommence,
            TurnConfigure,
            TurnBegin,
            AIData,
            DoublingData,
            DoublingOffers,
            DoublingTakesOrDrops,
            DoublingInGame,
            RollDice,
            SelectPointFrom,
            SelectPointTo,
            MoveCounters,
            EvaluateAIRanks,
            EvaluatePlayer,
            EvaluateOpponent,
            AnalysisOrContinue,
            Analysis,
            RestoreBoard,
            ObserveBoard,
            PlayoutProMoves,
            TurnEnd,
            GameWon,
            GeneralInfo,
            ExitGame
        }
    }
}