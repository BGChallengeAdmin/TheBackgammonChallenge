using UnityEngine;

namespace Backgammon
{
	public class SettingsUI : MonoBehaviour
	{
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

        private PlayingFrom playingFrom = PlayingFrom.LHS;
    }
}