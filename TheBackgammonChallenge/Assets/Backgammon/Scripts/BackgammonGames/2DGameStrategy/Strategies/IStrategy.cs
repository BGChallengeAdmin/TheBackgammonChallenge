
using static Backgammon.GeneralInfoState2D;

namespace Backgammon
{
    public interface IStrategy 
    {
        public GameScoreInfo.ContinueGame GameToContinue { get; }

        public abstract void ConfigureBoardContinueEnter(GameStateContext2DStrategy Context);
        public abstract void RollForGoesFirstEnter(GameStateContext2DStrategy Context, bool playerGoesFirst, ref int playerDice, ref int opponentDice);
        public abstract string BeforeCommenceEnter(GameStateContext2DStrategy Context);
        public abstract int[] TurnConfigureEnter(GameStateContext2DStrategy Context, int[] dice);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy TurnConfigureEnterSelectState(GameStateContext2DStrategy Context);
        public abstract string TurnBeginEnterGetMoveData(GameStateContext2DStrategy Context);
        public abstract string[] TurnBeginEnterGetPlayerConcedes(GameStateContext2DStrategy Context);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy TurnBeginEnterSelectState(GameStateContext2DStrategy Context);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy AIDataEnter(GameStateContext2DStrategy Context);
        public abstract AIData AIDataEnterConstructData(GameStateContext2DStrategy Context, AIData _aiDataToSend);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy AIDataUpdate(GameStateContext2DStrategy Context);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy DoublingDataUpdate(GameStateContext2DStrategy Context,
                                                                        GameStateMachine2DStrategy.EGameState2DStrategy passThroughState);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy DoublingDataBailOut(GameStateContext2DStrategy Context);
        public abstract void DoublingOffersEnter(GameStateContext2DStrategy Context);
        public abstract void DoublingOffersUpdate(GameStateContext2DStrategy Context);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy DoublingInGameState(GameStateContext2DStrategy Context);
        public abstract void DiceRollsDisplayUserHint(GameStateContext2DStrategy Context, int dice1, int dice2);
        public abstract bool SelectPointFromEntry(GameStateContext2DStrategy Context, bool bar = false, bool bearingOff = false, bool normal = false);
        public abstract void EvaluatePlayerMoveEntry(GameStateContext2DStrategy Context);
        public abstract void PlayoutProMovesEntry(GameStateContext2DStrategy Context);
        public abstract void GameWonUpdateAIScore(GameStateContext2DStrategy Context);
        public abstract string GameWonUpdateScoreText(GameStateContext2DStrategy Context, string gameWonByText, string gameOrMatch, string winnersName);
        public abstract string GameWonUpdateStatsText(GameStateContext2DStrategy Context, string playerInfoText = "", bool setTextToStats = false);
        public abstract string GeneralInfoEnter(GameStateContext2DStrategy Context, EGeneralInfoState2D state);
        public abstract GameStateMachine2DStrategy.EGameState2DStrategy GeneralInfoUpdate(GameStateContext2DStrategy Context);
    }
}