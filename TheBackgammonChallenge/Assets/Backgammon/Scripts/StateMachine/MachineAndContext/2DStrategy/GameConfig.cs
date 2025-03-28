using Backgammon;
using System;

[Serializable]
public class GameConfig
{
    // MATCH CONFIG
    internal Backgammon_Asset.MatchData SelectedMatch = MatchSelectUI.Match;
    public int IndexGame = GameListUI.IndexGame;
    public Backgammon_Asset.GameData SelectedGame = GameListUI._game;

    // AI GAME CONFIG
    public Backgammon_Asset.MatchData AIMatchData = null;
    public Backgammon_Asset.GameData AIGameData = null;
    public AISettingsScriptableObject AISettings = null;

    // BOARD CONFIG
    public Game2D.PlayingAs PlayingAs = Game2D.PlayingAs.PLAYER_1;
    public Game2D.PlayingAs OpponentAs = Game2D.PlayingAs.PLAYER_2;
    public bool IfPlayingAsPlayer1 = true;
    public bool IfPlayFromLhs = true;
    public bool IfPlayerIsBlack = true;

    // FAST FORWARD / CONTINUE
    public bool IfGameToContinue = false;
    public bool IfAIGameToContinue = false;
    public bool IfFastForwarding = false;
    public int FastForwardTurnIndex = 0;

    // END GAME
    public bool GameWonByGammon = false;
    public bool GameWonByBackGammon = false;
    public bool AIGameWasWon = false;
    public bool IfSelectedAnotherGame = false;
    public bool IfPlayNextGame = false;
    public bool IfPlayAnotherMatch = false;

    //DEBUG
    public DebugPrefab Debug_reportState;
    public DebugPrefab Debug_debugObject;
}
