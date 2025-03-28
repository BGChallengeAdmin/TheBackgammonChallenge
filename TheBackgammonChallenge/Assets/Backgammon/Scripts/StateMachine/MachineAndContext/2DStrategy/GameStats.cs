using System;

[Serializable]
public class GameStats
{
    public GameScoreInfo.ContinueGame SaveObject;

    // STATS
    public int TotalValidPlayerMovesThisGame = 0;
    public int TotalValidPlayerMatchesThisGame = 0;
    public int TotalValidOpponentMovesThisGame = 0;

    public int PlayerTopRankedThisGame = 0;
    public int ProTopRankedThisGame = 0;
    public int OpponentTopRankedThisGame = 0;

    public int PlayerSecondRankedThisGame = 0;
    public int ProSecondRankedThisGame = 0;
    public int OpponentSecondRankedThisGame = 0;

    public int PlayerScoreThisGame = 0;
    public int ProScoreThisGame = 0;
    public int OpponentScoreThisGame = 0;
}
