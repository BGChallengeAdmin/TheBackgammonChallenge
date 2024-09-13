using UnityEngine;

namespace Backgammon
{
    public class GameWonState : GameState
    {
        public GameWonState(GameStateContext context, GameStateMachine.EGameState estate) : base(context, estate) { }

        public override void EnterState() 
        {
            UnityEngine.Debug.Log($"GAME WAS WON!!!");

            var winner = Context.SelectedMatch.Game(Context.IndexGame).Winner() == 1 ? Context.SelectedMatch.Player1 : Context.SelectedMatch.Player2;
            var text = "Game was Won by " + winner;

            Context.ChallengeProgressionUI.SetActive(true);
            Context.GameWonUI.SetGameWonText(text);
            Context.GameWonUI.SetActive(true);

            _delayTimer = _delayTime;
        }

        public override void UpdateState() 
        {
            if (_delayTimer > 0)
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer <= 0)
                    Context.ChallengeProgressionUI.SetSliderValue(1f);
            }

            if (!Context.GameWonUI.ClickedConfirm) return;

            ActiveState = GameStateMachine.EGameState.ExitGame;
        }

        public override void ExitState()
        {
            Context.GameWonUI.SetActive(false);
            Context.GameScreenUI.SetActive(false);
            Context.ChallengeProgressionUI.SetActive(false);

            Game.IfGameConcluded = true;
        }

        public override GameStateMachine.EGameState GetStateKey() { return StateKey; }

        public override GameStateMachine.EGameState GetActiveState() { return ActiveState; }

        private float _delayTime = 1f;
        private float _delayTimer = 1f;
    }
}