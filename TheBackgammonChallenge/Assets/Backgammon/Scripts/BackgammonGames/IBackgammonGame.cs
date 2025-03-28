namespace Backgammon
{
    public interface IBackgammonGame
    {
        public void ConfigureContextAndInit(bool continueProOrAI = true);
        public bool ConfigureContextAndInitForContinue(bool continueProOrAI);
        //public IBackgammonContext Context { get; }
        public void ExitGame();
        public void SetUseDebugReportState(bool useDebugLogging);
        public void SetUseDebugGameObject(bool useDebugLogging);
    }
}