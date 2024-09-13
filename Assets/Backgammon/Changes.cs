//Changes made within this version of the project

//Added a woodgrain texture to the whole of the board:
//  -Changed prefabs 'HomeUpper', 'HomeLower', 'NoHomeUpper', 'NoHomeLower'
//  -Changed gameobjects bar, upper and lower to reflect this texture change


//Edited 'game' script within the sections regarding the turns, marked these changed with a comment
//Edited 'game' script to force a player interaction when either side cannot move, instead of just if the player themselves cannot move

//Edited SettingsUI script to remove the option to switch between 'observer' and 'winner' and all code relating to this
//Edited the actual settings menu object to reflect the script changes.

/*
   TODO:

    *Get rid of the "BLOCK" on the "PAUSE MENU" button when in "FAST FORWARD"

     *Implement "SINGLE TURN" - POSSIBLE APPROACHES
         * "TitleMenuUI" - 158 (Test against previous 'n' matches / games / moves array - do not allow repeats)
         * Create "SINGLE TURN" at the point of the turn - "FAST FORWARD" is not effective
         * Once the turn has been played a new turn should be selected
         * (POSSIBLE ANSWER) GameListUI - Include an array of up to (100+) potnetial random moves - or find a way to calculate new move each turn
            * Store an array of all match lengths and turns - Use this to select new match
         * SOLVED - "Game" - 1971 - The move data is not always clean and can contain additional ( ' ' ) separator values
         

     *Implement "DONWLOAD GAMES"
        * "TitleMenuUI" - 14 (utilise - ResetMatchCount() - where new matches have been downloaded)
     
     
     *If "CONTINUE" is not an option the button should be hidden - the "START NEW MATCH" should then be full width - Visibility and scale button
        *Save game option needs to store the data and this needs to be recovered at "LOAD"
        *Starting a "RANDOM GAME" does not active the "CONTINUE" button - To do with how it is saved on EXIT and LOAD
     
    
     *"MatchSelectUI" - "TitleMenuUI" allow set for single move - "COMMENCE" or start move immediately
     *If you "CONTINUE" and "QUIT MATCH" before "COMMENCE" - "CONTINUE" no longer works
    
    
     *"STATS" consider scroing for "SRT" in a similar way to "Tournament" and "PlayerVsPro"


    CHANGES:
    
     *Change the "CONTINUE" button click screen - Remove "Commence" and options to change game - This chould continue or "GO BACK"
         * DEPRACTED "Main" - 140 (Set behaviour of "CONTINUE")
         

     *SINGLE RANDOM TURN
         *"MatchData"
            *66 (GETTER - GameData for match)
         *"TitleMenuUI"
            *25 (Manage "RST" bool in "Main")
            *78 (Wire up UI button - Random Single Turn)
            *149 - DEBUG - Specifgy the match / game / turn for the PlayerVsPro version
            *165 (funciton == SelectSingleRandomGameMove();)
            *220 - DEBUG - Specifgy the match / game / turn for the RST version
            *235 (function == ResetMatchCount(); - see TmUI 14)
         *"GameListUI"
            *57 (Set the indexTurn of the game)
            *87 (Made the indexTurn of a game public)
         *"Main"
            *150 (New state for "SINGLE RANDOM TURN")
            *344 (Configure Board)
            *354 (Configure board for "SINGLE RANDOM TURN")
            *401 (Handle Game_Out for SRT)
            *605 (SRTGame appStates)
            *637 (bool for "RST")
         *"Game" 
            *118 (Set the use of fastForward) 
            *367 (Handle timeFactor between normal and SRT)
            *380 (game.FastForwardReset();)
            *1918 (if RST - configure board fro RST after initially building the board layout)
            *1946 (function == SetSingleTurnGameState)


    ERRORS:
        * FIXED - "Play Random Game" - Throws a "Null Point" where a previous game has been played and SAVED
        * FIXED - "Play Random Game" - Pulls from the saved game, does not crete a new "RANDOM GAME"
        * FIXED - "AWAKE" vs "VISIBLE" on the borad does not wipe the previous game - (POSSIBLE ANSWER) Reset the board / refresh the FastForward values
        * FIXED - (POSSIBLE ANSWER) - ON "Play Random Game" Do not load from "SAVE"
        * Game 9 Turn 29 - Error - String was not in correct format
        * FIXED - Playing SRT and "QUIT" - "Stats" option on the Title Page throws an error
        * Game plays multiple repeat moves in SRT where there are doubles given and taken
        * "TitleMenuUI" 220
                    int DBDLC = 2;
                    int DBDLCmatch = 0;
                    int DBindexGame = 5;
                    int DBindexTurn = 36;
            * Historic Data is incorrect - duplication of moves from turn 36 - 38


    POINTERS:
        * Hi Jack, I believe the break in point for the "SELECT WHO TO PLAY AS" option, involving the two buttons in teh first image, would be;
        * Game.cs @ 365 - There would need to be a new state introduced here to present the dialog to the user. Thsi woudl allow for setting of the underlying;
        * Player1 / Player2 etc. The engine should be able to handle this change from what I undestand of it.
        * There will need to be a case - if(playerVsPro) / if(!Main.Instance.ifRandomSingleTurn)
        * PLEASE CHECK - "TitleMenuUI" 149 / 220 - as these are DEBUG feature to specify game / match / turn;
        * Commenting these out will resume normal Game Play.

 */



/*
 JACK'S CHANGES
 * Created a new UI element called 'PlayerSelectUI' and the relevant buttons

 * "PlayerSelectUI"
 * Created a new script named 'PlayerSelectUI' to handle the button presses for the Player Select menu
 * Shifted the code within 'Main' and moved it to this script where it is easier to control and manage (See 'main' changes for details on the code)

 * "Game"
 * 836 to 846 - Added conditional statements to check who the current player is. Displays a message to indicate which pro is moving first
 * 1953 to 1954 - Commented out these lines as this function has been moved to a different script

 * "Main"
 * 344 - Activates the PlayerSelectUI game object
 //MOVED TO 'PLAYERSELECTUI' * 347 to 353 - Conditional statements to check which button has been pressed on the 'PlayerSelectUI', set the player, disable the PlayerSelectUI object and enable the board object
 * 394 to 398 - Added a one-time conditional to prevent a weird glitch where the buttons needed to be pressed twice on the first game played... (not the cleanest solution, feel free to change it)

 */