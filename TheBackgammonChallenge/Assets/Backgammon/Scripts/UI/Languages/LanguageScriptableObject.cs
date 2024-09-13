using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace Backgammon
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Language", fileName = "Language")]
    public class LanguageScriptableObject : ScriptableObject
    {
        public string shortCode;
        public string longName;

        // SELECT LANGUAGE
        [Header("SELECT LANGUAGE")]
        public string selectLanguageTitle;
        public string selectLanguageText;

        // HOST INTRO UI
        [Header("HOST INTRO UI")]
        public string hostIntroTitle;
        public string hostIntroText1;
        public string hostIntroText2;
        public string hostIntroText3;
        public string hostIntroText4;
        public string hostIntroText5;
        public string hostIntroPrompt;

        // TITLE MENU
        [Header("TITLE MENU")]
        public string titleMenuTitle;
        public string titleLanguageSelectTitle;
        public string titleMenuDownload;
        public string titleMenuNewMatch;
        public string titleMenuRandomGame;
        public string titleMenuContinueMatch;
        public string titleMenuPlayDemo;
        public string titleMenuStatistics;
        public string titleMenuSettings;
        public string titleMenuExitApp;

        // MATCH SELECT INTRO
        [Header("MATCH SELECT INTRO")]
        public string matchSelectIntroTitle;
        public string matchSelectIntroDescription;
        public string matchSelectIntroPracticeButton;
        public string matchSelectIntroAIButton;

        // MATCH SELECT
        [Header("MATCH SELECT")]
        public string matchSelectTitle;
        public string matchSelectHeader;
        public string matchSelectSearch;
        public string matchSelectMatchStats;
        public string matchSelectPlayAI;

        // AI MATCH SELECT SPLASH
        [Header("AI MATCH SELECT SPLASH")]
        public string matchAISelectSplashTitle;
        public string matchAISelectSplashDescription;
        public string matchAISelectSplashText1;
        public string matchAISelectSplashText2;
        public string matchAISelectSplashText3;
        public string matchAISelectSplashText4;
        public string matchAISelectSplashText5;
        public string matchAISelectSplashText6;
        public string matchAISelectSplashText7;
        public string matchAISelectSplashChallengeText;

        // AI PLAYED FOR POINTS
        [Header("AI PLAYED FOR POINTS")]
        public string matchAIPointsTitle;
        public string matchAIPointsDescription;
        public string matchAIPointsSingleGame;
        public string matchAIPointsMatch;

        // AI DICE TYPE SELECT
        [Header("AI DICE TYPE SELECT")]
        public string matchAIDiceTypeTitle;
        public string matchAIDiceTypeHeader;
        public string matchAIDiceTypeNormal;
        public string matchAIDiceTypeNormalDescription;
        public string matchAIDiceTypeHistoric;
        public string matchAIDiceTypeHistoricDescription;
        public string matchAIDiceTypeManual;
        public string matchAIDiceTypeManualDescription;
        public string matchAIDiceTypeDiceRolls;

        // AI HISTORIC DICE
        [Header("AI HISTORIC DICE")]
        public string matchAIHistoricDiceTitle;
        public string matchAIHistoricDiceMatchDescription;
        public string matchAIHistoricDicePlayerDescription;
        public string matchAIHistoricDiceWinner;

        // AI MATCH SELECT
        [Header("AI DIFFICULTY SELECT")]
        public string matchAISelectTitle;
        public string matchAISelectDescription;
        public string matchAISelectCommence;
        public string matchAISelectEASY;
        public string matchAISelectMEDIUM;
        public string matchAISelectHARD;
        public string matchAISelectPERFECT;
        public string matchAISelectEASYDesc;
        public string matchAISelectMEDIUMDesc;
        public string matchAISelectHARDDesc;
        public string matchAISelectPERFECTDesc;
        public string matchAISelectRandomDesc;
        public string matchAISelectRankDescription;
        public string matchAISelectRandom;
        public string matchAISelectAdvancedOptions;
        public string matchAISelectPlyDescription;
        public string matchAISelectPlyDescriptionAdv;
        public string matchAISelectNoiseDescription;
        public string matchAISelectNoiseDescriptionAdv;
        public string matchAISelectMatchPlayedForDescription;
        public string matchAISelectMatchPlayedForHistoric;
        public string matchAISelectPlayASingleGame;
        public string matchAISelectPlayA5PointMatch;
        public string matchAISelectUsingHistoricDice;
        public string matchAISelectUsingRandomDice;

        // MATCH STATS
        [Header("MATCH STATS")]
        public string matchStatsTitle;
        public string matchStatsLastGamePlayed;
        public string matchStatsPlayingAs;
        public string matchStatsHighestGame;
        public string matchStatsOverallCompletion;
        public string matchStatsMovesMatched;
        public string matchStatsAIMovesMatched;

        // GAME LIST
        [Header("GAME LIST")]
        public string gameListTitleText;
        public string gameListMatchPlayedFor;
        public string gameListDemoText1Line1;
        public string gameListDemoText1Line2;
        public string gameListDemoText1Line3;
        public string gameListDemoText2Line1;
        public string gameListDemoText2Line2;
        public string gameListDemoText2Line3;
        public string gameListDemoText2Line4;
        public string gameListDemoText2Line5;

        // PLAYER SELECT
        [Header("PLAYER SELECT")]
        public string playerSelectSelectPlayer;

        // IN GAME PAUSE
        [Header("IN GAME PAUSE")]
        public string pauseChangeGame;
        public string pauseChangePlayer;
        public string pauseMatchData;
        public string pauseMainMenu;
        public string pauseDiceOptions;
        public string pauseAISettings;
        public string pauseDiceOptionsHeader;
        public string pauseDiceOptionsManual;
        public string pauseDiceOptionsManualText;
        public string pauseDiceOptionsReRoll;
        public string pauseDiceOptionsReRollText;
        public string pauseDiceOptionsBothText;
        public string pauseChangeGameTitle;
        public string pauseChangeGameContinueMatch;
        public string pauseChangeAISettings;
        public string pauseChangePlayerTitle;
        public string pauseChangePlayerRestartGame;
        public string pauseChangePlayerRestartMatch;

        // IN GAME REVIEW
        [Header("IN GAME REVIEW")]
        public string gameReviewIsUnableToMove;
        public string gameReviewTapToContinue;
        public string gameReviewAIAnlaysis;
        public string gameReviewYourMove;
        public string gameReviewBGBlitzMove;
        public string gameReviewProsMove;
        public string gameReviewTopRankedMove;
        public string gameReviewProsOriginalMove;
        public string gameReviewYouHaveA;

        // IN GAME STATISTICS
        [Header("IN GAME STATS")]
        public string gameStatsTitle;
        public string gameStatsHowYoureDoing;
        public string gameStatsMovesMadeThisMatch;
        public string gameStatsMovesMadeThisGame;
        public string gameStatsMovesMatched;
        public string gameStatsProMatched;

        // END GAME STATS
        [Header("END GAME STATS")]
        public string endGameWouldYouLikeAnotherMove;
        public string endGameWouldYouLikeAnotherGame;
        public string endGameWouldYouLikeAnotherMatch;
        public string endGameStatistics;
        public string endMatchStatistics;
        public string endMatchOfTheProMoves;
        public string endMatchAndProMatched;
        public string endMatchInATotalOf;
        public string endMatchYouHaveWon;
        public string endMatchYouHaveLost;
        public string endYouAreCurrentlyPlayingAt;
        public string endNoRankingGivenUnder10Moves;
        public string endUnranked;
        public string endBeginner;
        public string endIntermediate;
        public string endExceptional;
        public string endMaster;
        public string endInternational;

        // DICE ROLLS
        [Header("DICE ROLLS")]
        public string diceRollsTapToRoll;
        public string diceRollsNoMoreValidMoves;
        public string diceRollsDemoHeader;
        public string diceRollsDemoTapToRoll;
        public string diceRollsMakeYourMove;
        public string diceRollsIsBlockedFromMoving;

        // DOUBLING
        [Header("DOUBLING")]
        public string doublingOffer;
        public string doublingIsOffering;
        public string doublingWasThisAGoodDoubld;
        public string doublingShouldTheyAccept;
        public string doublingDouble;
        public string doublingOffersDouble;
        public string doublingTakesDouble;
        public string doublingDropsDouble;
        public string doublingNoAIData;
        public string doublingAIEvalShould;
        public string doublingAIEvalMatchEquity;
        public string doublingAIEvalMoreInfo;
        public string doublingAIEvalToWin;
        public string doublingAIEvalToWinGammon;
        public string doublingAIEvalToWinBackGammon;
        public string doublingCORRECT;
        public string doublingINCORRECT;
        public string doublingTOOGOOD;

        // TURN END REVIEW
        [Header("TURN END REVIEW")]
        public string turnEndReviewYourScore;
        public string turnEndReviewProScore;
        public string turnEndReviewMatched;
        public string turnEndReviewNoMatched;
        public string turnEndReviewRank1;
        public string turnEndReviewRank;
        public string turnEndReviewUnranked;
        public string turnEndReviewYourLastMoveRanked;

        // ANALYSIS
        [Header("ANALYSIS")]
        public string analysisTitle;
        public string analysisYourRank;
        public string analysisProRank;
        public string analysisShowYourMove;
        public string analysisShowProMove;
        public string analysisShowAIMove;
        public string analysisUnranked;

        // STATISTICS
        [Header("STATISTICS")]
        public string statsTitle;
        public string statsHeader;
        public string statsStatistics;
        public string statsResetData;
        public string statsTotalMovesPlayed;
        public string statsProMovesEquals;
        public string statsYouHave;
        public string statsAvailableMatches;

        // STATISTICS GRAPHS
        [Header("STATS GRAPH/TABLE")]
        public string graphButtonLabelText;
        public string tableButtonLabelText;
        public string movesMatchedLabelText;
        public string rank1AIMatchedLabelText;
        public string rank1ProAIMatchedLabelText;
        public string averageLabelText;
        public string movesMadeLabelText;
        public string percentageLabelText;

        public string playerNameLabelText;
        public string MovesMatchedLabelText;
        public string MovesMatchPerLabelText;
        public string AIMatchesLabelText;
        public string AIMatchPerLabelText;
        public string ProAIMatchesLabelText;
        public string ProAIMatchPerLabelText;
        public string YourRankingLabelText;

        // RESET APP DATA
        [Header("RESEAT APP DATA")]
        public string resetPartial;
        public string resetFull;
        public string resetLine1;
        public string resetLine2;
        public string resetLine3;
        public string resetLine4;
        public string resetConfirmText;

        // DOWNLOADS
        [Header("DOWNLOADS")]
        public string downloadsTitle;
        public string downloadsPurchase;
        public string downloadsRestorePurchases;
        public string downloadsInfoPopupTitle;

        // DEMO
        [Header("DEMO")]
        public string demoStart1;
        public string demoStart2;
        public string demoGoesFirst1;
        public string demoGoesFirst2;
        public string demoRollDice1;
        public string demoRollDice2;
        public string demoSelectFrom1;
        public string demoSelectFrom2;
        public string demoSelectFrom3;
        public string demoSelectFrom4;
        public string demoSelectFromOut1;
        public string demoSelectFromOut2;
        public string demoSelectFromOut3;
        public string demoReviewIn1;
        public string demoReviewIn2;
        public string demoReview;
        public string demoAnalysis;
        public string demoDoublingIn;
        public string demoDoublingOut;
        public string demoEnd1;
        public string demoEnd2;
        public string demoEnd3;
        public string demoEnd4;
        public string demoEnd5;

        // GENERAL
        [Header("GENERAL")]
        public string ChanceOfLosing;
        public string ByGammon;
        public string ByBackGammon;
        public string Menu;
        public string Back;
        public string Confirm;
        public string Start;
        public string CONTINUE;
        public string Continue;
        public string Concede;
        public string Yes;
        public string No;
        public string Close;
        public string Points;
        public string points;
        public string Using;
        public string Its;
        public string The;
        public string the;
        public string Details;
        public string should;
        public string and;
        public string You;
        public string Pro;
        public string WinsThe;
        public string LosesThe;
        public string ConcedesThe;
        public string For;
        public string pointMatch;
        public string Games;
        public string Game;
        public string Matches;
        public string Match;
        public string move;
        public string moves;
        public string level;
        public string MATCHED;
        public string DIDNTMATCH;
        public string SMove;
        public string ToWin;
        public string BLACK;
        public string WHITE;
        public string UNDO;
        public string RANK;
        public string a;
        public string an;
        public string AI;
        public string ComingSoon;
        public string CrafordRuleInPlay;
        public string CrafordRuleNotInPlay;
        public string movesFirst;
        public string YouHaveChosenToConcedeTheGame;
    }
}