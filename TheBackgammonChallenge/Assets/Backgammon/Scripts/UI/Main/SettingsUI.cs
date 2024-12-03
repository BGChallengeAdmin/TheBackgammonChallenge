using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
	public class SettingsUI : MonoBehaviour
	{
        [Header("TEXT FIELDS")]
        [SerializeField] Text titleText;
        [SerializeField] Text playerColourText;
        [SerializeField] Text playingFromText;
        [SerializeField] Text replaySpeedText;

        [Header("BUTTONS")]
        [SerializeField] Button backButton;
        [SerializeField] Button resetButton;

        [Header("GAME OBJECTS")]
        [SerializeField] ButtonHighlight optionPlayerColorBlack = null;
        [SerializeField] ButtonHighlight optionPlayerColorWhite = null;
        [SerializeField] ButtonHighlight optionPlayingFromLHS = null;
        [SerializeField] ButtonHighlight optionPlayingFromRHS = null;
        [SerializeField] ButtonHighlight optionReplaySpeedX1 = null;
        [SerializeField] ButtonHighlight optionReplaySpeedX2 = null;
        [SerializeField] ButtonHighlight optionReplaySpeedX4 = null;
        [SerializeField] ButtonHighlight optionReplaySpeedX8 = null;

        protected void OnEnable()
        {
            backButton.onClick.AddListener(() => IfBack());
            resetButton.onClick.AddListener(() => IfReset());

            Back = false;
            OnOption();
        }

        protected void OnDisable()
        {
            backButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
        }

        // ------------------------------------- HELPER METHODS -----------------------------------------

        public void SetPlayerSettings(int _colour, int _LHS, int _speed)
        {
            playerColor = _colour == 0 ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            playingFrom = _LHS == 1 ? PlayingFrom.LHS : PlayingFrom.RHS;
            replaySpeed = _speed == 1 ? ReplaySpeed.Normal : _speed == 2 ? ReplaySpeed.Fast : _speed == 3 ? ReplaySpeed.VeryFast : ReplaySpeed.ExtremelyFast;

            if (playingFrom == PlayingFrom.LHS)
                playingFromPoint24Position = PointFlow.Point24Position.UpperLeft;
            else
                playingFromPoint24Position = PointFlow.Point24Position.UpperRight;
        }

        public int[] GetPlayerSettings()
        {
            int[] settings = new int[3];

            settings[0] = playerColor == Game2D.PlayerColour.BLACK ? 0 : 1;
            settings[1] = playingFrom == PlayingFrom.LHS ? 1 : 0;
            settings[2] = replaySpeed == ReplaySpeed.Normal ? 1 : replaySpeed == ReplaySpeed.Fast ? 2 : replaySpeed == ReplaySpeed.VeryFast ? 3 : 4;

            return settings;
        }

        private void ResetToDefaults()
        {
            playerColor = Game2D.PlayerColour.BLACK;
            playingFromPoint24Position = PointFlow.Point24Position.UpperLeft;
            playingFrom = PlayingFrom.LHS;
            playingAs = PlayingAs.Winner;
            replaySpeed = ReplaySpeed.Normal;
        }

        // ------------------------------------ SETTINGS -----------------------------------------
        
        public void SetPlayerColour(bool playingAsBlack)
        {
            playerColor = playingAsBlack ? Game2D.PlayerColour.BLACK : Game2D.PlayerColour.WHITE;
            OnOption();
        }

        public void SetPlayingFromSide(bool playingFromLeft)
        {
            playingFrom = playingFromLeft ? PlayingFrom.LHS : PlayingFrom.RHS;
            OnOption();
        }

        public void SetReplaySpeed(int option)
        {
            switch (option)
            {
                case 1:
                    replaySpeed = ReplaySpeed.Normal;
                    break;
                case 2:
                    replaySpeed = ReplaySpeed.Fast;
                    break;
                case 4:
                    replaySpeed = ReplaySpeed.VeryFast;
                    break;
                case 8:
                    replaySpeed = ReplaySpeed.ExtremelyFast;
                    break;
                default: replaySpeed = ReplaySpeed.Normal;
                    break;
            }

            OnOption();
        }

        void OnOption()
        {
            if (optionPlayerColorBlack != null)
            {
                optionPlayerColorBlack.Select(false);
                optionPlayerColorWhite.Select(false);
            }
            if (optionPlayingFromLHS != null)
            {
                optionPlayingFromLHS.Select(false);
                optionPlayingFromRHS.Select(false);
            }
            optionReplaySpeedX1.Select(false);
            optionReplaySpeedX2.Select(false);
            optionReplaySpeedX4.Select(false);
            optionReplaySpeedX8.Select(false);

            if (optionPlayerColorBlack != null)
            {
                switch (playerColor)
                {
                    case Game2D.PlayerColour.BLACK:
                        optionPlayerColorBlack.Select();
                        break;
                    case Game2D.PlayerColour.WHITE:
                        optionPlayerColorWhite.Select();
                        break;
                }
            }

            if (optionPlayingFromLHS != null)
            {
                switch (playingFrom)
                {
                    case PlayingFrom.LHS:
                        optionPlayingFromLHS.Select();
                        break;
                    case PlayingFrom.RHS:
                        optionPlayingFromRHS.Select();
                        break;
                }
            }

            switch (replaySpeed)
            {
                case ReplaySpeed.Normal:
                    optionReplaySpeedX1.Select();
                    break;
                case ReplaySpeed.Fast:
                    optionReplaySpeedX2.Select();
                    break;
                case ReplaySpeed.VeryFast:
                    optionReplaySpeedX4.Select();
                    break;
                case ReplaySpeed.ExtremelyFast:
                    optionReplaySpeedX8.Select();
                    break;
            }
        }

        // ----------------------------------- NAVIGATION -----------------------------------------

        private void IfBack() => Back = true;

        private void IfReset() => ResetToDefaults();

        // ------------------------------------- MEMBERS -----------------------------------------

        public enum PlayingFrom { LHS, RHS }

        public enum PlayingAs { Winner }

        public enum ReplaySpeed
        {
            Normal,
            Fast,
            VeryFast,
            ExtremelyFast
        }

        public static Game2D.PlayerColour playerColor = Game2D.PlayerColour.BLACK;
        public static PointFlow.Point24Position playingFromPoint24Position = PointFlow.Point24Position.UpperLeft;
        public static PlayingAs playingAs = PlayingAs.Winner;
        public static ReplaySpeed replaySpeed = ReplaySpeed.Fast;
        public static PlayingFrom playingFrom = PlayingFrom.LHS;

        public bool Back { get; private set; }
    }
}