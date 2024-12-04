THE BACKGAMMON CHALLENGE


// IDENTIFYING LOGIN AND START-UP PROCESSES

There are a series of "DebugObjects" under the main hierarchy headers [WorldObjects, DataHandlers, GameAssetManager] with a bool switch
to Debug.Log() and DebugFileOutput() the Login process. Enablig these manually will print out the Login process.

Alternatively the "DEBUG_TOOLKIT" can be enabled and accessed from the "MatchTypeSelectIntro" UI while the app is running. This allows 
for the "DebugObjects" to be turned on and off and the app "Restarted" from the Main "Login" AppState.

Enter "ENABLE_DEBUG_TOOLKIT" / "DISABLE_DEBUG_TOOLKIT" in the search bar to enable the button on the main splash page.

NOTE: The DebugOutputFile() can be found at "C:\Users\[USERNAME]\AppData\LocalLow\EchoAlpha\TheBackgammonChallenge\BackgammonChallenge\DebugLog"
This file will be automatically deleted on start-up if it becomes over 2.5MB and a new file created. The setting for file size deletion can be 
found in DebugOutputFile.cs

The DEBUG_TOOLKIT also contains definitions for "In Game" Logging and internet connection HEARTBEAT functionality in DataHandler.cs.
NOTE: The Heartbeat Logging may not be currently configured but has been tested and mainly disconnected [BE CAREFUL WITH THESE OPTIONS].


// APP START-UP

Main.cs acts as a Singleton throughout the app. There are multiple bool values to configure the game and play style.

The Main.cs entry point of the app is built on a switch statement to handle all of the main UI page interactions and Game2D lifecycle.

Main is responsible for launching the 3D Background. This can be disbaled in the "APP STATE" options.


// UI AND GAME MANAGEMENT

The UI is handled by a single UI Canvas with nested panels for each UI screen. This handles the main entry point for the app as well as
entry to both the "Player vs Pro" and "A.I." versions. Within this is also the setup for the 2D board and UI for the game in progress.

The lifecycle of the 2D and 3D versions of the game are handled by separate StateMachines which are found under the "2DGame_Controller" and
"3DGame_Controller" gameobjects in the hierarchy respectively.

The 2D Game Controller handles the UI elements for the Game Board and management of StateChanges and User Interactions when either the 
"Pro" or "A.I." version of the game is running.

The 3D version is set as a passive background. This will handle its own lifecycle if enabled at Login, and will be enabled/disabled by the 
2D Game is started from the "Game" AppState in Main.
NOTE: Both 2D and 3D versions use the same Base State Machine [StateManager.cs]. If the DEBUG_TOOLKIT is enabled then both will write to 
the Console and DebugOutputFile(). There is no clear way to distinguish between these Logs.

The 3D version will handle its own lifecycle on completion of a "Background" game and reset to a new game indefinitely. The Game is fully playable
with commented code in each of the required GameStates handling similar user input and state management, but has been disconnected from the UI.
The major issue with using teh 3D version on older devices was graphical stuttering, there are multiple asset optimisations that have not been applied.


// 2D GAME LIFECYCLE

The 2D Game handles both the "Pro" and "A.I." versions of the app based on Main.Instance.ifPlayervsAI

The game Context is recreated with each fresh start. Settings are pulled from static members of ["MatchSelectUI", "GameListUI", "SettingsUI"]

"Continue Game" is handled from Main.cs AppStates and injects loaded data from PlayerPrefsHandler when the context is Initialized. This causes the Game2D
to load a ContinueGameState before the TurnConfigState.

The significant differences between the "Pro" and "A.I." versions is the DataCaptureState. The A.I. version requires a server response before beginning the turn,
this will determine if the A.I. is going to enter the DoublingState before playing out the turn normally.

Conversely the "Pro" version handles the recorded data in TurnBeginState to determine GameStateTransitions. DoublingData is only captured when the 
RecordedData indicates, or player offers Doubles, otherwise the DataCaptuesState sends the request in the background and the normal turn is played out.

Both versions of the game will auto-increment via the GameListUI.cs static gameNumber. This causes the Context and StateMachine to be reset.


// DATA HANDLER

The A.I. data handler uses a load balancing approach on a worker thread for both the Doubling and Move Data. A heartbeat funtion has been included 
to automatically test all connecitons and discard those that are slow to respond. This was required to prevent the thread becoming locked if there was an 
unexpected or unhandled response from the servers, allowing the app to handle server or internet conneciton loss gracefully.

The TIMEOUT_DELAY should be carefully considered in how long to test each connection compared to the player turn duration. Testing is not done post a Doubling
request as the TurnData request is likely to follow and has significantly longer before another request is expected.


// DATA STORAGE

General player score is recorded in the PlayerPrefs, along with the Continue game data.

There are two JSON serialized files to record the player score for each match and statistics for specific data points to be shown in the Statistics UI panel.
The Statisitcs information is not currently used and there is no information Read / Written to a file.

The Data Storage appraoch should likely be revisited and refactored to be more accessible / manageable.


// HISTORIC GAME ASSETS

These are the recorded "Pro" games and matches. These are built and formatted by the Backgammon_Asset -> AssetBuilder App [DISCUSSED LATER]

The Match Data is accessed via [MatchReplayDLC.cs && MatchData.cs] which are automatically bundled by the AssetBuilder App to be imported as a Unity Package.

Historic Matches must be added to a GameObject which is named "tournamentmatchbundleXXX" to conform with the naming scheme used in In App Purchasing. On App start up
GameAssetManager.cs will connect to the required store, verify receipts, and unloock the named GameObjects.




ASSET BUILDER

Simple to use converter for Backgammon Match Transcripts (.txt).

Main source: http://itikawa.com/kifdb/herodb.cgi?table=bg


Simply group the (.txt) trasncripts in to a folder under the main project filepath, specifiy the location in the uiHandler.cs, run the app and press "Build".
The APP will format the document in to a MatchReplayDLC and throw an error on any file which does is not correctly formatted or contains errors in the data.

The APP is not verbose in build failure but is accurate in error handling and correctly formatting the output GameObjects. If the APP does throw and error it
is usually best to scrap the (.txt) file as there can be incorrect notations and line formatting errors which make the file useless.