using Backgammon_Asset;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class Main : MonoBehaviour
	{
        [Header("DATA HANDLERS")]
        [SerializeField] WorldRegionHandler _worldRegionHandler = null;
        [SerializeField] WorldPlayerInfoHandler _worldPlayerInfoHandler = null;
        [SerializeField] PlayerPrefsHandler _playerPrefsHandler = null;
        [SerializeField] PlayerScoresHandler _playerScoresHandler = null;
        [SerializeField] AIDataHandler _aiDataHandler = null;

        [Header("UI HANDLERS")]
        [SerializeField] LogoSplashUI logoSplashUI = null;
        [SerializeField] SettingsUI settingsUI = null;
        [SerializeField] LanguageSelect languageSelect = null;
        [SerializeField] MatchTypeSelctIntroUI matchTypeSelectIntroUI = null;
        [SerializeField] TitleMenuUI titleMenuUI = null;
        [SerializeField] DownloadNewMatches downloadNewMatchesUI = null;
        [SerializeField] MatchSelectUI matchSelectUI = null;
        [SerializeField] MatchWinnerIntroUI matchWinnerIntroUI = null;
        [SerializeField] GameListUI gameListUI = null;
        [SerializeField] BoardDesignerUI boardDesignerUI = null;
        [SerializeField] MatchAISelectPoints _aiSetPoints = null;
        [SerializeField] MatchAIConfigureSettingsMain _aiConfigMainSettings = null;

        [Header("DEFAULT UI")]
        [SerializeField] Transform _defaultBackground = null;

        [Header("GAME ASSETS")]
        [SerializeField] GameAssetManager gameAssetManager;

        [Header("GAME")]
        [SerializeField] Game2D _2DGame = null;
        [SerializeField] Game _3DGame = null;
        [SerializeField] Backgammon_Asset.MatchReplayDLC _defaultMatchData = null;
        [SerializeField] BoardDesignSO[] _boardDesignsSOArray = null;

        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_showLogin = null;
        [SerializeField] DebugPrefab debug_3DBackground = null;
        [SerializeField] DebugTextToUI debug_textToUI = null;
        [SerializeField] DebugToolkitUI _debugToolkitUI = null;
        [SerializeField] bool _usingDebugToolkit = false;

        [Header("APP STATE")]
        [SerializeField] bool USING_3D_BACKGROUND = true;

        public string CurrentVersionNumber = "0.0.1";
        public bool IfFirstPlaythrough = true;
        public string LanguageShortCode = string.Empty;
        public string AIUserID = null;

        internal bool IfUsingDebugToolkit = false;

        internal bool IfDemoIsInPlay;
        internal bool IfMatchedPlay;
        internal bool IfPlayerVsPro = false;
        internal bool IfPlayerVsAI = false;
        internal bool IfAIDataConnectionAvailable = false;

        internal bool IfClickedContinuePro;
        internal bool IfClickedContinueAI;

        internal bool IfPlayerVsHistoricAI;
        internal bool IfRandomSingleTurn;
        internal bool IfSelectSpecificMove;

        internal bool IfGamePaused;
        internal bool ifDebugging2D = false;

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            IfUsingDebugToolkit = _usingDebugToolkit;

            _defaultBackground.gameObject.SetActive(true);

            // JAMES - TURN SPLASH ON / OFF
            appState = AppState.LogoSplash_In;
            //appState = AppState.Login_In;
            //appState = AppState.MatchTypeSelectIntro_In;
            //appState = AppState.Game_In;

            DebugPrefab.Init();

            #region TODOforApple
            // HIDE HOME BUTTON (WHITE BAR) ON iOS
            //if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //    PlayerSettings.iOS.hideHomeButton = true;
            //}
            #endregion
        }

        protected void Start()
        {
            // NOTE: INITIALIZE ON START UP -
            // REINITIALIZED ON LOGIN SCREEN FOR DEBUG TOOLKIT
            _playerPrefsHandler.Init();

            // CONFIGURE LOCALIZATION AND LANGUAGE
            _worldRegionHandler.TrySetLanguageSOFromShortCode(LanguageShortCode);
        }

        protected void Update()
        {
            switch (appState)
            {
                // ----------------------------------------------- LOGO SPLASH --------------------------------------------
                //
                #region LogoSplash
                case AppState.LogoSplash_In:
                    {
                        logoSplashUI.gameObject.SetActive(true);

                        _playerPrefsHandler.Init();

                        gameAssetManager.Init();
                        gameAssetManager.LoadAllUnlockedContent();

                        timer = timeSplash;

                        appState = AppState.LogoSplash;
                    }
                    break;
                case AppState.LogoSplash:
                    {
                        timer -= Time.deltaTime;
                        if (timer > 0)
                            break;

                        appState = AppState.LogoSplash_Out;
                    }
                    break;
                case AppState.LogoSplash_Out:
                    {
                        logoSplashUI.gameObject.SetActive(false);

                        appState = AppState.Login_In;
                    }
                    break;
                #endregion
                // -------------------------------------------------- LOGIN -----------------------------------------------
                //
                #region Login
                case AppState.Login_In:
                    {
                        timer = 1.0f;

                        if (IfFirstPlaythrough)
                        {
                            _playerPrefsHandler.ResetAppData();
                            _playerScoresHandler.ResetPlayerScoreData();
                        }

                        // ESTABLISH INTERNET CONNECTIVITY
                        //_aiDataHandler.EstablishInternetConnection();

                        // LOAD THE PLAYER SCORE FILE
                        if (!_playerScoresHandler.ScoreFileLoaded && !IfFirstPlaythrough)
                        {
                            debug_showLogin.DebugMessage("LOADED SCORES SUCCESSFULLY " + _playerScoresHandler.AttemptToLoadPlayerData());
                        }

                        // GENRATE A NEW AI_USER_ID - NEW GAME NO VALUE OR DEFAULT IS BEING USED
                        if (AIUserID == null || AIUserID == string.Empty || AIUserID == "" || AIUserID == _aiDataHandler.aiDataToSend.id)
                        {
                            var random = new System.Random();
                            this.AIUserID = String.Format("#{0:X8}", random.Next(0x1000000));

                            //string randomUserId = string.Empty;
                            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                            //for (int _id = 0; _id < 14; _id++)
                            //    randomUserId += chars[(int)UnityEngine.Random.Range(0f, (chars.Length - 1))];

                            //Debug.Log("RANDOM ID " + randomUserId);

                            _playerPrefsHandler.SaveAppData();
                        }

                        appState = AppState.Login;
                    }
                    break;
                case AppState.Login:
                    {
                        // TODO: IF ONLINE SERVICE AND NO USERNAME DISPLAY LOGIN PAGE
                        timer -= Time.deltaTime;
                        if (timer > 0)
                            break;

                        // ONCE LOGIN AND USERNAME SET VALUES
                        _aiDataHandler.AiUserID = AIUserID;

                        if (IfFirstPlaythrough || LanguageShortCode == string.Empty)
                        {
                            appState = AppState.LanguageSelect_In;
                            break;
                        }

                        appState = AppState.Login_Out;
                    }
                    break;
                case AppState.Login_Out:
                    {
                        debug_showLogin.DebugMessage("LOGIN COMPLETE");

                        appState = AppState.HostIntro_In;
                        appState = AppState.MatchTypeSelectIntro_In;
                    }
                    break;
                #endregion
                // ----------------------------------------------- LANGUAGE SELECT ----------------------------------------
                //
                #region LanguageSelect
                case AppState.LanguageSelect_In:
                    {
                        languageSelect.gameObject.SetActive(true);

                        appState = AppState.LanguageSelect;
                    }
                    break;
                case AppState.LanguageSelect:
                    {
                        if (!languageSelect.LanguageSelected)
                            break;

                        // SET MAIN VALUE OF LANGUAGE AND SAVE
                        LanguageShortCode = _worldRegionHandler.LanguageShortCode;
                        _playerPrefsHandler.SaveAppData();

                        appState = AppState.LanguageSelect_Out;
                    }
                    break;
                case AppState.LanguageSelect_Out:
                    {
                        languageSelect.gameObject.SetActive(false);
                        appState = AppState.Login_Out;
                    }
                    break;
                #endregion
                // --------------------------------------------- MATCH SELECT INTRO ---------------------------------------
                //
                #region MatchTypeSelectIntro
                case AppState.MatchTypeSelectIntro_In:
                    {
                        matchTypeSelectIntroUI.gameObject.SetActive(true);

                        if (CurrentVersionNumber != Application.version.ToString())
                        {
                            matchTypeSelectIntroUI.CheckNewVersionUpdate(true, CurrentVersionNumber);
                        }

                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        appState = AppState.MatchTypeSelectIntro;
                    }
                    break;
                case AppState.MatchTypeSelectIntro:
                    {
                        if (CurrentVersionNumber != Application.version.ToString())
                        {
                            if (!matchTypeSelectIntroUI.UpdatedConfirmed)
                                break;

                            matchTypeSelectIntroUI.UpdatedConfirmed = true;
                            CurrentVersionNumber = Application.version.ToString();
                            _playerPrefsHandler.SaveAppData();
                        }

                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        if (!matchTypeSelectIntroUI.IfClicked)
                            break;

                        appState = AppState.MatchTypeSelectIntro_Out;
                    }
                    break;
                case AppState.MatchTypeSelectIntro_Out:
                    {
                        matchTypeSelectIntroUI.gameObject.SetActive(false);

                        if (matchTypeSelectIntroUI.IfBack)
                        {
                            appState = AppState.HostIntro_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfExit)
                        {
                            appState = AppState.ExitApp_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfPlayPro)
                        {
                            appState = AppState.TitleMenu_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfPlayAI)
                        {
                            appState = AppState.MatchAISelectPoints_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfSettings)
                        {
                            appState = AppState.Settings_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfConfigureBoard)
                        {
                            appState = AppState.ConfigureBoard_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfChangeLanguage)
                        {
                            appState = AppState.LanguageSelect_In;
                            break;
                        }

                        if (matchTypeSelectIntroUI.IfDebugToolkit)
                        {
                            appState = AppState.DebugToolkitUI_In;
                            break;
                        }

                        appState = AppState.TitleMenu_In;
                    }
                    break;
                #endregion
                // -------------------------------------------------- SETTINGS ---------------------------------------------
                //
                #region Settings
                case AppState.Settings_In:
                    {
                        settingsUI.gameObject.SetActive(true);
                        appState = AppState.Settings;
                    }
                    break;
                case AppState.Settings:
                    {
                        if (!settingsUI.Back)
                            break;

                        appState = AppState.Settings_Out;
                    }
                    break;
                case AppState.Settings_Out:
                    {
                        settingsUI.gameObject.SetActive(false);
                        _playerPrefsHandler.SaveAppData();
                        appState = AppState.TitleMenu_In;
                    }
                    break;
                #endregion
                // -------------------------------------------- DEBUG TOOLKIT UI ------------------------------------------
                //
                #region DEBUG_TOOLKIT_UI
                case AppState.DebugToolkitUI_In:
                    {
                        _debugToolkitUI.SetActive(true);

                        appState = AppState.DebugToolkitUI;
                    }
                    break;
                    case AppState.DebugToolkitUI:
                    {
                        // LEFT PANEL

                        //if (_debugToolkitUI.ClickedServerHeartbeat)
                        //{
                        //    if (_debugToolkitUI.UseServerHeartbeat) 
                        //        _aiDataHandler.StartInternetConnectionHeartbeat(_debugToolkitUI.GetServerHeartbeatRate());
                        //    else _aiDataHandler.StopInternetConnectionHeartbeat();

                        //    _debugToolkitUI.ClickedServerHeartbeat = false;
                        //}

                        if (_debugToolkitUI.ClickedMaintainServerHeartbeat)
                        {
                            _aiDataHandler.SetUseDebugResetConnecitons(_debugToolkitUI.GetServerHeartbeatRate());
                            _debugToolkitUI.ClickedMaintainServerHeartbeat = false;
                        }

                        // RIGHT PANEL
                        //APP

                        if (_debugToolkitUI.ClickedDebugGameAssetManager)
                        {
                            gameAssetManager.SetUseDebugGameAssetManager(_debugToolkitUI.UseDebugGameAssetManager);
                            _debugToolkitUI.ClickedDebugGameAssetManager = false;
                        }

                        if (_debugToolkitUI.ClickedDebugLogin)
                        {
                            if (_debugToolkitUI.UseDebugLogin) debug_showLogin.ShowMesssage = true;
                            else debug_showLogin.ShowMesssage = false;

                            _debugToolkitUI.ClickedDebugLogin = false;
                        }

                        if (_debugToolkitUI.ClickedDebugPlayerPrefs)
                        {
                            _playerPrefsHandler.SetUseDebugPlayerPrefs(_debugToolkitUI.UseDebugPlayerPrefs);
                            _debugToolkitUI.ClickedDebugPlayerPrefs = false;
                        }

                        if (_debugToolkitUI.ClickedDebugPlayerData)
                        {
                            _playerScoresHandler.ScoreFileLoaded = false;
                            _playerScoresHandler.SetUseDebugPlayerData(_debugToolkitUI.UseDebugPlayerData);
                            _debugToolkitUI.ClickedDebugPlayerData = false;
                        }

                        if (_debugToolkitUI.ClickedDebugReportState)
                        {
                            _2DGame.SetUseDebugReportState(_debugToolkitUI.UseDebugReportState);
                            _debugToolkitUI.ClickedDebugReportState = false;
                        }

                        if (_debugToolkitUI.ClickedDebugGameObject)
                        {
                            _2DGame.SetUseDebugGameObject(_debugToolkitUI.UseDebugGameObject);
                            _debugToolkitUI.ClickedDebugGameObject = false;
                        }

                        //DATA
                        if (_debugToolkitUI.ClickedDebugAIDataHandler)
                        {
                            _aiDataHandler.SetUseDebugAIDataHandler(_debugToolkitUI.UseDebugAIDataHandler);
                            _debugToolkitUI.ClickedDebugAIDataHandler = false;
                        }

                        if (_debugToolkitUI.ClickedDebugAIDataSent ||
                            _debugToolkitUI.ClickedDebugAIDataReceived ||
                            _debugToolkitUI.ClickedDebugAIDoublingDataReceived ||
                            _debugToolkitUI.ClickedDebugAIPingServer
                            )
                        {
                            _aiDataHandler.SetUseDebugObjects(
                                _debugToolkitUI.UseDebugAIDataSent,
                                _debugToolkitUI.UseDebugAIDataReceived,
                                _debugToolkitUI.UseDebugAIDoublingDataReceived,
                                _debugToolkitUI.UseDebugAIPingServer
                                );

                            _debugToolkitUI.ClickedDebugAIDataSent = false;
                            _debugToolkitUI.ClickedDebugAIDataReceived = false;
                            _debugToolkitUI.ClickedDebugAIDoublingDataReceived = false;
                            _debugToolkitUI.ClickedDebugAIPingServer = false;
                        }

                        // BUTTONS

                        if (_debugToolkitUI.ClickedRestartApp)
                        {
                            _debugToolkitUI.ClickedRestartApp = false;
                            appState = AppState.LogoSplash_In;
                            _debugToolkitUI.SetActive(false);
                            break;
                        }

                        if (_debugToolkitUI.ClickedDeleteOutputFile)
                        {
                            _debugToolkitUI.ClickedDeleteOutputFile = false;
                            DebugPrefab.DeleteOutputFile();
                        }

                        if (!_debugToolkitUI.ClickedContinue) return;

                        appState = AppState.DebugToolkitUI_Out;
                    } 
                    break;
                    case AppState.DebugToolkitUI_Out:
                    {
                        _debugToolkitUI.SetActive(false);

                        appState = AppState.MatchTypeSelectIntro_In;
                    } 
                    break;
                    #endregion
                // ---------------------------------------------- BOARD DESIGNER ------------------------------------------
                //
                #region 3DBoardDesigner
                case AppState.ConfigureBoard_In:
                    {
                        _defaultBackground.gameObject.SetActive(false);
                        boardDesignerUI.gameObject.SetActive(true);

                        // INITIALIZE CONTEXT TO ENABLE CONFIGURE BOARD
                        _3DGame.ResetBoardLayout();

                        appState = AppState.ConfigureBoard;
                    }
                    break;
                case AppState.ConfigureBoard:
                    {
                        if (!boardDesignerUI.IfClickedExit) return;

                        appState = AppState.ConfigureBoard_Out;
                    }
                    break;
                case AppState.ConfigureBoard_Out:
                    {
                        _3DGame.SetGameActive(false);
                        _defaultBackground.gameObject.SetActive(true);
                        boardDesignerUI.gameObject.SetActive(false);

                        appState = AppState.MatchTypeSelectIntro_In;
                    }
                    break;
                #endregion
                // --------------------------------------------------------------------------------------------------------
                // ----------------------------------------------- PLAYER VS PRO ------------------------------------------
                // ------------------------------------------------- TITLE MENU -------------------------------------------
                //
                #region TitleMenu
                case AppState.TitleMenu_In:
                    {
                        _playerPrefsHandler.LoadAppData();

                        // DEBUG - REMOVES REQUIREMENT FOR DEMO
                        IfFirstPlaythrough = false;

                        titleMenuUI.gameObject.SetActive(true);

                        IfClickedContinuePro = true;
                        IfClickedContinueAI = false;

                        //if (gameAssetManager.LoadAllUnlockedContent())
                        //    Debug.Log("RE-LOADED SCORES SUCCESSFULLY " + PlayerScoresHandler.AttemptToLoadPlayerData());

                        appState = AppState.TitleMenu;
                    }
                    break;
                case AppState.TitleMenu:
                    {
                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        if (!titleMenuUI.ifClicked)
                            break;

                        appState = AppState.TitleMenu_Out;
                    }
                    break;
                case AppState.TitleMenu_Out:
                    {
                        titleMenuUI.gameObject.SetActive(false);

                        if (titleMenuUI.ifSelectNewMatch)
                        {
                            appState = AppState.MatchSelect_In;
                            break;
                        }
                        else if (titleMenuUI.ifContinueMatch)
                        {
                            appState = AppState.Continue_In;
                            break;
                        }
                        else if (titleMenuUI.ifPlayDemo)
                        {
                            appState = AppState.Demo_In;
                            break;
                        }
                        else if (titleMenuUI.ifPlayerVsPro)
                        {
                            appState = AppState.PlayerSelect_In;
                            break;
                        }
                        else if (titleMenuUI.ifDailyChallenges)
                        {
                            appState = AppState.DailyChallenges_In;
                            break;
                        }
                        else if (titleMenuUI.ifRandomGame)
                        {
                            appState = AppState.Game_In;
                            break;
                        }
                        else if (titleMenuUI.ifSelectSpecificMove)
                        {
                            appState = AppState.MatchSelect_In;
                            break;
                        }
                        else if (titleMenuUI.ifViewStatistics)
                        {
                            appState = AppState.Statistics_In;
                            break;
                        }
                        else if (titleMenuUI.ifDownloadMatches)
                        {
                            appState = AppState.DownloadMatches_In;
                            break;
                        }
                        else if (titleMenuUI.ifExitApp)
                        {
                            appState = AppState.MatchTypeSelectIntro_In;
                            break;
                        }

                        appState = AppState.TitleMenu_In;
                    }
                    break;
                #endregion
                // --------------------------------------------- DOWNLOAD MATCHES -----------------------------------------
                //
                #region Download New Matches
                case AppState.DownloadMatches_In:
                    {
                        downloadNewMatchesUI.gameObject.SetActive(true);

                        appState = AppState.DownloadMatches;
                    }
                    break;
                case AppState.DownloadMatches:
                    {
                        if (!downloadNewMatchesUI.ifBack)
                            break;

                        appState = AppState.DownloadMatches_Out;
                    }
                    break;
                case AppState.DownloadMatches_Out:
                    {
                        downloadNewMatchesUI.gameObject.SetActive(false);

                        appState = AppState.TitleMenu_In;
                    }
                    break;
                #endregion
                // ------------------------------------------------ MATCH SELECT ------------------------------------------
                //
                #region MatchSelect
                case AppState.MatchSelect_In:
                    {
                        matchSelectUI.gameObject.SetActive(true);

                        appState = AppState.MatchSelect;
                    }
                    break;
                case AppState.MatchSelect:
                    {
                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        if (!(matchSelectUI.ifBack || MatchSelectUI.Match != null))
                            break;

                        appState = AppState.MatchSelect_Out;
                    }
                    break;
                case AppState.MatchSelect_Out:
                    {
                        matchSelectUI.gameObject.SetActive(false);

                        if (matchSelectUI.ifBack)
                        {
                            appState = AppState.TitleMenu_In;
                            break;
                        }

                        // NOTE: GAME LIST NO LONGER USED -> MATCH INFO
                        //appState = AppState.GameList_In;
                        appState = AppState.MatchIntro_In;
                    }
                    break;
                #endregion
                // ------------------------------------------------- MATCH INTRO ------------------------------------------
                //
                #region MatchIntro
                case AppState.MatchIntro_In:
                    {
                        matchWinnerIntroUI.gameObject.SetActive(true);

                        appState = AppState.MatchIntro;
                    }
                    break;
                case AppState.MatchIntro:
                    {
                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        if (!(matchWinnerIntroUI.ifAccept || matchWinnerIntroUI.ifBack))
                            break;

                        appState = AppState.MatchIntro_Out;
                    }
                    break;
                case AppState.MatchIntro_Out:
                    {
                        matchWinnerIntroUI.gameObject.SetActive(false);

                        if (matchWinnerIntroUI.ifBack)
                        {
                            appState = AppState.MatchSelect_In;
                            break;
                        }

                        // JUMP STRAIGHT TO GAME_LIST_OUT CHECK BEFORE GAME_IN
                        //appState = AppState.GameList_In;
                        appState = AppState.GameList_Out;
                    }
                    break;
                #endregion
                // ------------------------------------------------- GAME LIST --------------------------------------------
                //
                #region GameSelect
                case AppState.GameList_In:
                    {
                        gameListUI.gameObject.SetActive(true);

                        if (GameListUI.IndexGame >= MatchSelectUI.Match.GameCount)
                        {
                            appState = AppState.GameList_Out;
                            break;
                        }

                        appState = AppState.GameList;
                    }
                    break;
                case AppState.GameList:
                    {
                        if (!(gameListUI.ifCommence || gameListUI.ifQuit))
                            break;

                        appState = AppState.GameList_Out;
                    }
                    break;
                case AppState.GameList_Out:
                    {
                        gameListUI.gameObject.SetActive(false);

                        if (MatchSelectUI.Match == null || gameListUI.ifQuit) // quit match
                        {
                            appState = AppState.MatchSelect_In;
                            break;
                        }

                        if (GameListUI.IndexGame >= MatchSelectUI.Match.GameCount)
                        {
                            appState = AppState.TitleMenu_In;
                            break;
                        }

                        if (IfMatchedPlay)
                            appState = AppState.Game_In;
                        else
                            appState = AppState.PlayerSelect_In;
                    }
                    break;
                #endregion
                // -------------------------------------------------- CONTINUE --------------------------------------------
                //
                #region Continue
                case AppState.Continue_In:
                    {
                        SetGameTimeFactor(SettingsUI.replaySpeed);

                        signalCommenceGame = true;
                        timer = timeGameOver;
                        
                        // CONFIGURE GAME - IF FAIL - BAIL OUT TO TITLE MENU
                        if (!_2DGame.ConfigureContextAndInitForContinue(true))
                        {   
                            appState = AppState.TitleMenu_In;
                            Debug.Log($"BAIL OUT");
                        }
                        else
                        {
                            if (USING_3D_BACKGROUND) Enable3DBackground(false);
                            appState = AppState.Game;
                        }
                    }
                    break;
                case AppState.Continue_AI_In:
                    {
                        SetGameTimeFactor(SettingsUI.replaySpeed);

                        signalCommenceGame = true;
                        timer = timeGameOver;

                        // CONFIGURE GAME - IF FAIL - BAIL OUT TO TITLE MENU
                        if (!_2DGame.ConfigureContextAndInitForContinue(false))
                        {
                            appState = AppState.MatchAISelectPoints_In;
                            Debug.Log($"BAIL OUT");
                        }
                        else
                        {
                            if (USING_3D_BACKGROUND) Enable3DBackground(false);
                            appState = AppState.Game;
                        }
                    }
                    break;
                #endregion
                // --------------------------------------------------------------------------------------------------------
                // --------------------------------------------------------------------------------------------------------
                // ----------------------------------------------- A.I. CHALLENGE -----------------------------------------
                // ------------------------------------------------- SET POINTS -------------------------------------------
                //
                #region Set A.I. points
                case AppState.MatchAISelectPoints_In:
                    {
                        _aiSetPoints.gameObject.SetActive(true);

                        appState = AppState.MatchAISelectPoints;
                    }
                    break;
                case AppState.MatchAISelectPoints:
                    {
                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        if (!_aiSetPoints.IfCommence && !_aiSetPoints.IfBack) return;
                        
                        appState = AppState.MatchAISelectPoints_Out;
                    }
                    break;
                case AppState.MatchAISelectPoints_Out:
                    {
                        _aiSetPoints.gameObject.SetActive(false);

                        if (_aiSetPoints.IfCommence)
                            appState = AppState.MatchAIConfigureSettingsMain_In;

                        if (_aiSetPoints.IfContinue)
                        {
                            appState = AppState.Continue_AI_In;
                        }

                        if (_aiSetPoints.IfBack)
                            appState = AppState.MatchTypeSelectIntro_In;                      
                    }
                    break;
                #endregion
                // ------------------------------------------------ CONFGIURE GAME ----------------------------------------
                //
                #region Configure A.I. game
                case AppState.MatchAIConfigureSettingsMain_In:
                    {
                        _aiConfigMainSettings.gameObject.SetActive(true);

                        appState = AppState.MatchAIConfigureSettingsMain;
                    }
                    break;
                case AppState.MatchAIConfigureSettingsMain:
                    {
                        if (USING_3D_BACKGROUND && Game.IfGameConcluded)
                        {
                            Enable3DBackground(USING_3D_BACKGROUND);
                        }

                        if (!_aiConfigMainSettings.IfStart && !_aiConfigMainSettings.IfBack && 
                            !_aiConfigMainSettings.ManualSetupBoard) return;

                        appState = AppState.MatchAIConfigureSettingsMain_Out;
                    }
                    break;
                case AppState.MatchAIConfigureSettingsMain_Out:
                    {
                        _aiConfigMainSettings.gameObject.SetActive(false);

                        if (_aiConfigMainSettings.IfStart)
                            appState = AppState.Game_In;

                        if (_aiConfigMainSettings.ManualSetupBoard)
                        {
                            Game2D.IfManualSetupBoard = true;
                            appState = AppState.Game_In;
                        }

                        if (_aiConfigMainSettings.IfBack)
                            appState = AppState.MatchAISelectPoints_In;
                    } 
                    break;
                #endregion
                // --------------------------------------------------------------------------------------------------------
                // --------------------------------------------------------------------------------------------------------
                // ---------------------------------------------------- GAME ----------------------------------------------
                //
                #region Game
                case AppState.Game_In:
                    {
                        if (USING_3D_BACKGROUND) Enable3DBackground(false);

                        SetGameTimeFactor(SettingsUI.replaySpeed);

                        _2DGame.ConfigureContextAndInit();

                        signalCommenceGame = true;
                        timer = timeGameOver;

                        appState = AppState.Game;
                    }
                    break;
                case AppState.Game:
                    {
                        // NOTE: NOT USED

                        //if (playerSelectUI.firstGame == false)
                        //{
                        //    playerSelectUI.gameObject.SetActive(false);
                        //    playerSelectUI.firstGame = true;
                        //}

                        if (!Game2D.IfGameConcluded)
                        {
                            // NOTE: CHECK IF USED

                            //if (Game.aiHistoricDiceGetNewRandomDice)
                            //{
                            //    Game.aiHistoricDiceGetNewRandomDice = false;
                            //    matchAISelectDiceUI.SelectRandomGame();
                            //}

                            if (IfGamePaused)
                            {
                                if (Game2D.IfUpdatingBoard)
                                    break;
                                appState = AppState.PauseMenu_In;
                            }
                            break;
                        }

                        timer -= Time.deltaTime;
                        if (timer > 0)
                            break;

                        appState = AppState.Game_Out;
                    }
                    break;
                case AppState.Game_Out:
                    {
                        _2DGame.ExitGame();
                        _playerPrefsHandler.SaveAppData();

                        if (Game2D.Context.IfSelectedAnotherGame)
                        {
                            GameListUI.IndexTurn = 0;
                            appState = AppState.Game_In;
                            break;
                        }
                        else if (Game2D.Context.IfPlayNextGame)
                        {
                            // PRO AND A.I. MATCHES WERE NOT COMPLETED ON POINTS

                            // PRO MATCH HAS GAMES LEFT - CONTINUE TO NEXT GAME
                            if ((GameListUI.IndexGame + 1) < MatchSelectUI.Match.GameCount)
                            {
                                GameListUI.IndexGame++;
                                GameListUI.IndexTurn = 0;
                            }

                            appState = AppState.Game_In;
                            break;
                        }
                        else if (Game2D.Context.IfPlayAnotherMatch)
                        {
                            if (Main.instance.IfPlayerVsAI)
                            {
                                // GO TO MATCH SELECT
                                appState = AppState.MatchAISelectPoints_In;
                            }
                            else
                            {
                                // PRO MATCH WAS ENDED - GO TO PRO MATCH SELECT
                                appState = AppState.MatchSelect_In;
                            }
                            break;
                        }

                        Game2D.AIIfUsingHistoricDice = false;

                        // DEFAULT TO MAIN MENU
                        appState = AppState.MatchTypeSelectIntro_In;
                        break;

                        // JAMES - IF PLAYING A TOURNAMENT - AUTO INCREMENT TO THE NEXT GAME
                        if (IfMatchedPlay)
                        {
                            //JAMES - Handle exit on ifRandomSingleTurn
                            if (IfRandomSingleTurn)
                            {
                                titleMenuUI.SelectRandomSingleGameMove();

                                appState = AppState.PlayerSelect_In;
                                break;
                            }
                            else if (IfPlayerVsAI && IfPlayerVsHistoricAI && !_3DGame.AIMatchWon)
                            {
                                // NOTE: REFACTOR METHODS

                                //matchAISelectDiceUI.RestoreSelectedMatch();

                                //if (matchAISelectDiceUI.MatchHasNotEnded())
                                //    matchAISelectDiceUI.IncrementHistoricGame();
                                //else
                                {
                                    appState = AppState.MatchAISelectDifficulty_In;
                                    break;
                                }

                                appState = AppState.Game_In;
                                break;
                            }
                            else if (IfPlayerVsAI && IfPlayerVsHistoricAI && _3DGame.AIMatchWon)
                            {
                                // NOTE: REFACTOR METHODS

                                //matchAISelectDiceUI.RestoreSelectedMatch();

                                //// PLAY AS OPPONENT
                                //if (Game.aiHistoricReplayAsOpponent)
                                //{
                                //    matchAISelectDiceUI.ReplayAsOpponent();

                                //    // GOING TO AI SETTINGS RESETS THE AI GAME
                                //    //appState = AppState.Game_In;
                                //    appState = AppState.MatchAISelectDifficulty_In;
                                //    break;
                                //}

                                // AI MATCH WAS CONCLUDED - RETURN TO AI SETUP
                                Main.Instance.IfPlayerVsAI = false;
                                appState = AppState.MatchAISelectDifficulty_In;
                                break;
                            }
                            else if (IfPlayerVsAI && !_3DGame.AIMatchWon)
                            {
                                // AI MATCH IS CONFIGURED AT END OF LAST GAME - START NEW GAME
                                appState = AppState.Game_In;
                                break;
                            }
                            else if (IfPlayerVsAI && _3DGame.AIMatchWon)
                            {
                                // AI MATCH WAS CONCLUDED - RETURN TO AI SETUP
                                Main.Instance.IfPlayerVsAI = false;
                                appState = AppState.MatchAISelectDifficulty_In;
                                break;
                            }
                            else if ((GameListUI.IndexGame + 1) < MatchSelectUI.Match.GameCount)
                            {
                                // PRO MATCH HAS GAMES LEFT - CONTINUE TO NEXT GAME
                                GameListUI.IndexGame++;
                                GameListUI.IndexTurn = 0;

                                appState = AppState.Game_In;
                                break;
                            }
                            else
                            {
                                // PRO MATCH WAS ENDED - GO TO PRO MATCH SELECT
                                appState = AppState.MatchSelect_In;
                                break;
                            }
                        }
                        else if (Game.IfExitAIGame)
                        {
                            appState = AppState.MatchAISelectDifficulty_In;
                            break;
                        }

                        appState = AppState.MatchTypeSelectIntro_In;
                    }
                    break;
                #endregion
                // ---------------------------------------------------- EXIT ----------------------------------------------
                //  
                #region ExitApp
                case AppState.ExitApp_In:
                    {
                        appState = AppState.ExitApp_Out;
                    }
                    break;
                case AppState.ExitApp_Out:
                    {
                        //SaveAppData();

                        DebugPrefab.CloseLogFile();

#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif

                        Application.Quit();
                    }
                    break;
                #endregion
            }
        }

        // ------------------------------------------- GETTERS && SETTERS -------------------------------------------------
        // DATA HANDLERS
        public WorldRegionHandler WorldRegionObj { get => _worldRegionHandler; }
        public WorldPlayerInfoHandler WorldInfoObj { get => _worldPlayerInfoHandler; }
        public PlayerPrefsHandler PlayerPrefsObj { get => _playerPrefsHandler; }
        public PlayerScoresHandler PlayerScoresObj { get => _playerScoresHandler; }
        
        // UI HANDLERS
        public SettingsUI SettingsUI { get => settingsUI; }

        // BOARD DESIGNS
        public static int BoardDesignSOIndex = 0;
        public BoardDesignSO BoardDesignSO 
        {
            get
            {
                if (BoardDesignSOIndex >= _boardDesignsSOArray.Length) return _boardDesignsSOArray[0];
                else return _boardDesignsSOArray[BoardDesignSOIndex];
            }
        }

        public int MaxNumberOfBoardSO { get => _boardDesignsSOArray.Length; }

        // DEBUG
        public DebugTextToUI DebugTextToUI { get => debug_textToUI; }

        // --------------------------------------------- 3D BACKGROUND ----------------------------------------------------

        private void Enable3DBackground(bool enable)
        {
            var match = _defaultMatchData.Match(0);

            // IF DISABLE BOARD
            if (!enable)
            {
                _3DGame.SetGameDisabled();
            }

            // CONFIGURE MATCH CONTEXT
            if (enable)
            {
                debug_3DBackground.DebugMessage($"CONFIGURE MATCH CONTEXT");

                var matchList = FindObjectsByType<Backgammon_Asset.MatchReplayDLC>(sortMode: FindObjectsSortMode.None);
                var matchDataList = new List<MatchData>();

                foreach(var matchReplay in matchList)
                {
                    foreach (var matchToAdd in matchReplay.MatchData)
                        if (matchToAdd.Title == "AI" || matchToAdd.Title == "DEMO") continue;
                        else matchDataList.Add(matchToAdd);
                }
                    
                if (matchDataList.Count > 0)
                {
                    debug_3DBackground.DebugMessage($"FOUND {matchDataList.Count} MATCHES");
                    match = matchDataList[UnityEngine.Random.Range(0, matchDataList.Count)];
                    debug_3DBackground.DebugMessage($"USING: {match.name} {match.ID} {match.Title}");
                }

                if (match != null) MatchSelectUI.SetMatch(match);
                else
                {
                    USING_3D_BACKGROUND = false;
                    return;
                }
            }

            // CONFIGURE GAME CONTEXT
            if (enable)
            {
                debug_3DBackground.DebugMessage($"CONFIGURE GAME CONTEXT");
                GameListUI.IndexGame = UnityEngine.Random.Range(0, match.GameCount - 1);
                GameListUI._game = match.Game(GameListUI.IndexGame);
                GameListUI._playingAs = Game.PlayingAs.PLAYER_1;
            }

            // SET ACTIVE AND ENABLE CONTEXT
            if (enable)
            {
                debug_3DBackground.DebugMessage($"SET 3D BACKGROUND ACTIVE");
                _3DGame.ResetGameContext(); // START THE GAME
                Game.Context.BoardMaterialsManager.Init(Main.Instance.BoardDesignSO);
                _3DGame.gameObject.SetActive(enable);
            }

            if (enable) debug_3DBackground.DebugMessage($"DISABLE BACKGROUNDS");
            
            _defaultBackground.gameObject.SetActive(!enable);

            matchTypeSelectIntroUI.EnableDefaultBackground(!enable);
            titleMenuUI.EnableDefaultBackground(!enable);
            matchSelectUI.EnableDefaultBackground(!enable);
            matchWinnerIntroUI.EnableDefaultBackground(!enable);

            _aiSetPoints.EnableDefaultBackground(!enable);
            _aiConfigMainSettings.EnableDefaultBackground(!enable);

            if (!enable) debug_3DBackground.DebugMessage($"3D BACKGROUND DISABLED");
        }


        // ------------------------------------------------- HELPER -------------------------------------------------------
        
        internal void IncrementHistoricDiceRolls()
        {
            _aiConfigMainSettings.IncrementHistoricDiceRolls();
        }
        
        // ----------------------------------------------- APP STATE ------------------------------------------------------

        public enum AppState
        {
            //
            // MAIN
            //
            LogoSplash_In,
            LogoSplash_Out,
            LogoSplash,
            //
            //Splash_In,
            //Splash_Out,
            //Splash,
            //
            Login_In,
            Login_Out,
            Login,
            //
            LanguageSelect_In,
            LanguageSelect_Out,
            LanguageSelect,
            //
            HostIntro_In,
            //HostIntro_Out,
            //HostIntro,
            //
            MatchTypeSelectIntro_In,
            MatchTypeSelectIntro_Out,
            MatchTypeSelectIntro,
            //
            // TITLE MENU
            //
            TitleMenu_In,
            TitleMenu_Out,
            TitleMenu,
            //
            MatchSelect_In,
            MatchSelect_Out,
            MatchSelect,
            //
            Continue_In,
            Continue_AI_In,
            //Continue_Out,
            //Continue,
            //
            Demo_In,
            //Demo_Out,
            //Demo,
            //
            PlayerSelect_In,
            //PlayerSelect_Out,
            //PlayerSelect,
            //
            DailyChallenges_In,
            //DailyChallenges_Out,
            //DailyChallenges,
            //
            Statistics_In,
            //Statistics_Out,
            //Statistics,
            //Statistics_Reset,
            //
            DownloadMatches_In,
            DownloadMatches_Out,
            DownloadMatches,
            //
            // GAME
            //
            MatchIntro_In,
            MatchIntro_Out,
            MatchIntro,
            //
            GameList_In,
            GameList_Out,
            GameList,
            //
            Game_In,
            Game_Out,
            Game,
            //
            PauseMenu_In,
            //PauseMenu_Out,
            //PauseMenu,
            //
            // GENERAL
            //
            Settings_In,
            Settings_Out,
            Settings,
            //
            ConfigureBoard_In,
            ConfigureBoard_Out,
            ConfigureBoard,
            //
            MatchAISelectPoints_In,
            MatchAISelectPoints_Out,
            MatchAISelectPoints,
            //
            MatchAIConfigureSettingsMain_In,
            MatchAIConfigureSettingsMain_Out,
            MatchAIConfigureSettingsMain,
            ////
            //MatchAISplash_In,
            //MatchAISplash_Out,
            //MatchAISplash,
            ////
            //MatchAISelectDiceType_In,
            //MatchAISelectDiceType_Out,
            //MatchAISelectDiceType,
            ////
            //MatchAISelectManualDice_In,
            //MatchAISelectManualDice_Out,
            //MatchAISelectManualDice,
            ////
            //MatchAISelectHistoricDice_In,
            //MatchAISelectHistoricDice_Out,
            //MatchAISelectHistoricDice,
            ////
            MatchAISelectDifficulty_In,
            //MatchAISelectDifficulty_Out,
            //MatchAISelectDifficulty,
            ////
            //PlaySettings_In,
            //PlaySettings_Out,
            //PlaySettings,
            ////
            //PlayStatistics_In,
            //PlayStatistics_Out,
            //PlayStatistics,
            //
            DebugToolkitUI_In,
            DebugToolkitUI,
            DebugToolkitUI_Out,
            //
            ExitApp_In,
            ExitApp_Out
        }

        public AppState appState
        {
            get;
            private set;
        }
        
        float timer = 0;
        const float timeSplash = 3.0f;
        const float timeHostIntro = 3.0f;
        const float timeGameOver = 0.2f;

        // ----------------------------------------------- GAME STATE ------------------------------------------------------
        internal static bool signalCommenceGame = false;

        private void SetGameTimeFactor(SettingsUI.ReplaySpeed replaySpeed)
        {
            switch (replaySpeed)
            {
                case SettingsUI.ReplaySpeed.Normal:
                    Game.TimeFactor = 1.0f;
                    break;
                case SettingsUI.ReplaySpeed.Fast:
                    Game.TimeFactor = 0.5f;
                    break;
                case SettingsUI.ReplaySpeed.VeryFast:
                    Game.TimeFactor = 0.25f;
                    break;
                case SettingsUI.ReplaySpeed.ExtremelyFast:
                    Game.TimeFactor = 0.1f;
                    break;
            }
        }

        // ---------------------------------------------- HELPER METHODS -----------------------------------------------------


        // ------------------------------------------------ SINGLETON ------------------------------------------------------

        private static Main instance = null;
        public static Main Instance { get { return (instance); } }
    }
}