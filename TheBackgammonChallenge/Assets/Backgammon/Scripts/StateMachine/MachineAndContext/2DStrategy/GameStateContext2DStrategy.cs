using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class GameStateContext2DStrategy : IBackgammonContext
    {
        public GameStateContext2DStrategy(Game2DStrategy game, BoardMaterialsManager2D boardMaterialsManager, BarManager2D barManager,
                                HomeManager2D homeManager, DoublingManager2D doublingManager, PointsManager2D pointsManager,
                                CountersManager2D countersManager, DiceManager2D diceManager,
                                ObserveAnalysisManager2D observeAnalysisManager)
        {
            this._game = game;
            _boardMaterialsManager = boardMaterialsManager;
            _barManager = barManager;
            _homeManager = homeManager;
            _doublingManager = doublingManager;
            _pointsManager = pointsManager;
            _countersManager = countersManager;
            _diceManager = diceManager;
            _observeAnalysisManager = observeAnalysisManager;
        }

        internal void SetDataHandlers(AIDataHandler aiDataHandler)
        {
            _aiDataHAndler = aiDataHandler;
        }

        internal void SetUI(Transform defaultBackground, GameScreenUI gameScreenUI, RollForGoesFirstUI rollForGoesFirstUI,
                            BeforeCommenceUI beforeCommenceUI, DoublingUI doublingUI, DiceRollsUI diceRollsUI,
                            AnalysisOrContinueUI analysisOrContinueUI, AnalysisUI analysisUI, ObserveBoardUI observeBoardUI,
                            TurnEndsUI turnEndsUI, GameWonUI gameWonUI, GeneralInfoUI generalInfoUI, DemoUI demoUI,
                            ChallengeProgressionUI challengeProgressionUI, ConfigureBoardManualUI configureBoardManual)
        {
            _defaultBackground = defaultBackground;
            _gameScreenUI = gameScreenUI;
            _rollForGoesFirstUI = rollForGoesFirstUI;
            _beforeCommenceUI = beforeCommenceUI;
            _doublingUI = doublingUI;
            _diceRollsUI = diceRollsUI;
            _analysisOrContinueUI = analysisOrContinueUI;
            _analysisUI = analysisUI;
            _observeBoardUI = observeBoardUI;
            _turnEndsUI = turnEndsUI;
            _gameWonUI = gameWonUI;
            _generalInfoUI = generalInfoUI;
            _demoUI = demoUI;
            _challengeProgressionUI = challengeProgressionUI;
            _configureBoardManualUI = configureBoardManual;
        }

        internal void SetGameStategy(IStrategy strategy) => _strategy = strategy;

        #region MANAGERS_UI_FIELDS

        // GAME
        private Game2DStrategy _game;
        public Game2DStrategy Game { get => _game; }

        // MANAGERS
        private BoardMaterialsManager2D _boardMaterialsManager;
        private BarManager2D _barManager;
        private HomeManager2D _homeManager;
        private DoublingManager2D _doublingManager;
        private PointsManager2D _pointsManager;
        private CountersManager2D _countersManager;
        private DiceManager2D _diceManager;
        private ObserveAnalysisManager2D _observeAnalysisManager;

        // DATA HANDLERS
        private AIDataHandler _aiDataHAndler;

        // UI
        private Transform _defaultBackground;
        private GameScreenUI _gameScreenUI;
        private RollForGoesFirstUI _rollForGoesFirstUI;
        private BeforeCommenceUI _beforeCommenceUI;
        private DoublingUI _doublingUI;
        private DiceRollsUI _diceRollsUI;
        private AnalysisOrContinueUI _analysisOrContinueUI;
        private AnalysisUI _analysisUI;
        private ObserveBoardUI _observeBoardUI;
        private TurnEndsUI _turnEndsUI;
        private GameWonUI _gameWonUI;
        private GeneralInfoUI _generalInfoUI;
        private DemoUI _demoUI;
        private ChallengeProgressionUI _challengeProgressionUI;
        private ConfigureBoardManualUI _configureBoardManualUI;

        #endregion

        #region MANAGERS_UI_GETTERS

        public AIDataHandler AIDataHandler { get => _aiDataHAndler; }

        internal BoardMaterialsManager2D BoardMaterialsManager { get => _boardMaterialsManager; }
        internal BarManager2D BarManager { get => _barManager; }
        internal HomeManager2D HomeManager { get => _homeManager; }
        internal DoublingManager2D DoublingManager { get => _doublingManager; }
        internal PointsManager2D PointsManager { get => _pointsManager; }
        internal CountersManager2D CountersManager { get => _countersManager; }
        internal DiceManager2D DiceManager { get => _diceManager; }
        internal ObserveAnalysisManager2D ObserveAnalysisManager { get => _observeAnalysisManager; }

        internal Transform DefaultBackground { get => _defaultBackground; }
        internal GameScreenUI GameScreenUI { get => _gameScreenUI; }
        internal RollForGoesFirstUI RollForGoesFirstUI { get => _rollForGoesFirstUI; }
        internal BeforeCommenceUI BeforeCommenceUI { get => _beforeCommenceUI; }
        internal DoublingUI DoublingUI { get => _doublingUI; }
        internal DiceRollsUI DiceRollsUI { get => _diceRollsUI; }
        internal AnalysisOrContinueUI AnalysisOrContinueUI { get => _analysisOrContinueUI; }
        internal AnalysisUI AnalysisUI { get => _analysisUI; }
        internal ObserveBoardUI ObserveBoardUI { get => _observeBoardUI; }
        internal TurnEndsUI TurnEndsUI { get => _turnEndsUI; }
        internal GameWonUI GameWonUI { get => _gameWonUI; }
        internal GeneralInfoUI GeneralInfoUI { get => _generalInfoUI; }
        internal DemoUI DemoUI { get => _demoUI; }
        internal ChallengeProgressionUI ChallengeProgressionUI { get => _challengeProgressionUI; }
        internal ConfigureBoardManualUI ConfigureBoardManualUI { get => _configureBoardManualUI; }
        internal FadeInFadeOutImage FadeInFadeOutBlack { get => _defaultBackground.GetComponentInChildren<FadeInFadeOutImage>(); }

        public bool ExitFromStateMachine = false;

        #endregion

        #region STRATEGY
        IStrategy _strategy;
        GameConfig _gameConfig = new GameConfig();
        TurnConfig _turnConfig = new TurnConfig();
        List<GameTurn> _gameTurnsList = new() { new GameTurn() };
        GameStats _gameStats = new GameStats();

        public IStrategy Strategy => _strategy;
        public GameConfig GameConfig => _gameConfig;
        public TurnConfig TurnConfig => _turnConfig;
        public List<GameTurn> GameTurnsList => _gameTurnsList; 
        public GameStats GameStats => _gameStats;

        #endregion region
    }
}