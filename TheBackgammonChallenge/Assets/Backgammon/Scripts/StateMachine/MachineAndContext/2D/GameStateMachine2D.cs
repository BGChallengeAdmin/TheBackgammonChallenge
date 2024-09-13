namespace Backgammon
{
    public class GameStateMachine2D : StateManager<GameStateMachine2D.EGameState2D>
    {
        private GameStateContext2D _context;

        internal void Init(GameStateContext2D context)
        {
            _context = context;

            InitializeStates();
            CurrentState.EnterState();
        }

        internal void Exit()
        {
            TransitionToNextState(EGameState2D.ExitGame);
        }

        private void InitializeStates()
        {
            ResetStates();

            States.Add(EGameState2D.ConfigureBoard, new ConfigureBoardState2D(_context, EGameState2D.ConfigureBoard));
            States.Add(EGameState2D.ConfigureBoardManual, new ConfigureBoardManualState2D(_context, EGameState2D.ConfigureBoardManual));
            States.Add(EGameState2D.ConfigureBoardForContinue, new ConfigureBoardForContinueState2D(_context, EGameState2D.ConfigureBoardForContinue));
            States.Add(EGameState2D.RollForGoesFirst, new RollForGoesFirstState2D(_context, EGameState2D.RollForGoesFirst));
            States.Add(EGameState2D.BeforeCommence, new BeforeCommenceState2D(_context, EGameState2D.BeforeCommence));
            States.Add(EGameState2D.TurnConfigure, new TurnConfigureState2D(_context, EGameState2D.TurnConfigure));
            States.Add(EGameState2D.TurnBegin, new TurnBeginState2D(_context, EGameState2D.TurnBegin));
            States.Add(EGameState2D.AIData, new AIDataState2D(_context, EGameState2D.AIData));
            States.Add(EGameState2D.DoublingData, new DoublingDataState2D(_context, EGameState2D.DoublingData));
            States.Add(EGameState2D.DoublingOffers, new DoublingOffersState2D(_context, EGameState2D.DoublingOffers));
            States.Add(EGameState2D.DoublingTakesOrDrops, new DoublingTakesOrDropsState2D(_context, EGameState2D.DoublingTakesOrDrops));
            States.Add(EGameState2D.DoublingInGame, new DoublingInGameState2D(_context, EGameState2D.DoublingInGame));
            States.Add(EGameState2D.RollDice, new DiceRollsState2D(_context, EGameState2D.RollDice));
            States.Add(EGameState2D.SelectPointFrom, new SelectPointFromState2D(_context, EGameState2D.SelectPointFrom));
            States.Add(EGameState2D.SelectPointTo, new SelectPointToState2D(_context, EGameState2D.SelectPointTo));
            States.Add(EGameState2D.MoveCounters, new MoveCountersState2D(_context, EGameState2D.MoveCounters));
            States.Add(EGameState2D.EvaluateAIRanks, new EvaluateAIRankedMovesState2D(_context, EGameState2D.EvaluateAIRanks));
            States.Add(EGameState2D.EvaluatePlayer, new EvaluatePlayerMoveState2D(_context, EGameState2D.EvaluatePlayer));
            States.Add(EGameState2D.EvaluateOpponent, new EvaluateOpponentMoveState2D(_context, EGameState2D.EvaluateOpponent));
            States.Add(EGameState2D.AnalysisOrContinue, new AnalysisOrContinueState2D(_context, EGameState2D.AnalysisOrContinue));
            States.Add(EGameState2D.Analysis, new AnalysisState2D(_context, EGameState2D.Analysis));
            States.Add(EGameState2D.RestoreBoard, new RestoreBoardState2D(_context, EGameState2D.RestoreBoard));
            States.Add(EGameState2D.ObserveBoard, new ObserveBoardState2D(_context, EGameState2D.ObserveBoard));
            States.Add(EGameState2D.PlayoutProMoves, new PlayoutProMovesState2D(_context, EGameState2D.PlayoutProMoves));
            States.Add(EGameState2D.TurnEnd, new TurnEndState2D(_context, EGameState2D.TurnEnd));
            States.Add(EGameState2D.GameWon, new GameWonState2D(_context, EGameState2D.GameWon));
            States.Add(EGameState2D.GeneralInfo, new GeneralInfoState2D(_context, EGameState2D.GeneralInfo));
            States.Add(EGameState2D.ExitGame, new ExitGameState2D(_context, EGameState2D.ExitGame));

            CurrentState = States[EGameState2D.ConfigureBoard];
        }

        public enum EGameState2D
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