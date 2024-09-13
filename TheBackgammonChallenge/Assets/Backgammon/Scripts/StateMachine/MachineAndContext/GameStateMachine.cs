namespace Backgammon
{
    public class GameStateMachine : StateManager<GameStateMachine.EGameState>
    {
        private GameStateContext _context;

        internal void Init(GameStateContext context)
        {
            _context = context;

            InitializeStates();
            CurrentState.EnterState();
        }

        internal void Exit()
        {
            States[EGameState.ExitGame].EnterState();
        }

        private void InitializeStates()
        {
            ResetStates();

            States.Add(EGameState.ConfigureBoard, new ConfigureBoardState(_context, EGameState.ConfigureBoard));
            States.Add(EGameState.BeforeCommence, new BeforeCommenceState(_context, EGameState.BeforeCommence));
            States.Add(EGameState.TurnBegin, new TurnBeginState(_context, EGameState.TurnBegin));
            States.Add(EGameState.AIData, new AIDataState(_context, EGameState.AIData));
            States.Add(EGameState.DoublingData, new DoublingDataState(_context, EGameState.DoublingData));
            States.Add(EGameState.DoublingOffers, new DoublingOffersState(_context, EGameState.DoublingOffers));
            States.Add(EGameState.DoublingTakesOrDrops, new DoublingTakesOrDropsState(_context, EGameState.DoublingTakesOrDrops));
            States.Add(EGameState.DoublingInGame, new DoublingInGameState(_context, EGameState.DoublingInGame));
            States.Add(EGameState.RollDice, new DiceRollsState(_context, EGameState.RollDice));
            States.Add(EGameState.SelectPointFrom, new SelectPointFromState(_context, EGameState.SelectPointFrom));
            States.Add(EGameState.SelectPointTo, new SelectPointToState(_context, EGameState.SelectPointTo));
            States.Add(EGameState.MoveCounters, new MoveCountersState(_context, EGameState.MoveCounters));
            States.Add(EGameState.EvaluatePlayer, new EvaluatePlayerMoveState(_context, EGameState.EvaluatePlayer));
            States.Add(EGameState.EvaluateOpponent, new EvaluateOpponentMoveState(_context, EGameState.EvaluateOpponent));
            States.Add(EGameState.Analysis, new AnalysisState(_context, EGameState.Analysis));
            States.Add(EGameState.RestoreBoard, new RestoreBoardState(_context, EGameState.RestoreBoard));
            States.Add(EGameState.ObserveBoard, new ObserveBoardState(_context, EGameState.ObserveBoard));
            States.Add(EGameState.PlayoutProMoves, new PlayoutProMovesState(_context, EGameState.PlayoutProMoves));
            States.Add(EGameState.TurnEnd, new TurnEndState(_context, EGameState.TurnEnd));
            States.Add(EGameState.GameWon, new GameWonState(_context, EGameState.GameWon));
            States.Add(EGameState.GeneralInfo, new GeneralInfoState(_context, EGameState.GeneralInfo));
            States.Add(EGameState.ExitGame, new ExitGameState(_context, EGameState.ExitGame));

            CurrentState = States[EGameState.ConfigureBoard];
        }

        public enum EGameState
        {
            ConfigureBoard,
            BeforeCommence,
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
            EvaluatePlayer,
            EvaluateOpponent,
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