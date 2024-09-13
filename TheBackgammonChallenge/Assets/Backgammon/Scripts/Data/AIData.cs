using System;
using System.Linq;
using UnityEngine;

namespace Backgammon
{
    //------------------------------------ SEND DATA HELPERS -------------------------------------------

    [Serializable]
    public class AIData
    {
        public string type = "TutorRequest";
        public string id = "abc12";
        public string comment = "";
        public double noise = 0;
        public bool cubeful = true;
        public string gammons = "on";
        public string backgammons = "on";
        public string noOfMoves = "5";
        public int ply = 2;
        public AIDataPositionHelper position = null;
    }

    [Serializable]
    public class AIDataPositionHelper
    {
        public string matchLength = "0";
        public string whosOn = "red";
        public bool usecube = true;
        public bool crawford = true;
        public bool jacoby = false;
        public AIDataDiceHelper dice = null;
        public AIDataScoreHelper score = null;
        public AIDataBoardHelper board = null;
    }

    [Serializable]
    public class AIDataDiceHelper
    {
        public int red = 0;
        public int green = 0;
    }

    [Serializable]
    public class AIDataScoreHelper
    {
        public int red = 0;
        public int green = 0;
    }

    [Serializable]
    public class AIDataBoardHelper
    {
        public int cube = 1;
        public string cubeowner = "none";
        public int[] points = { 2, 0, 0, 0, 0, -5, 0, -4, 0, 0, 0, 5, -4, 0, 0, 0, 3, 0, 5, -1, 0, 0, 0, -1 };
        public AIDataBarHelper bar = null;
        public AIDataOffHelper off = null;
    }

    [Serializable]
    public class AIDataBarHelper
    {
        public int red = 0;
        public int green = 0;
    }

    [Serializable]
    public class AIDataOffHelper
    {
        public int red = 0;
        public int green = 0;
    }

    //---------------------------------------- RETURNED DATA HANDLERS --------------------------------------------------

    [Serializable]
    public class AIDataFromServer
    {
        public string type = "Error";
        public string response = string.Empty;
        public string id;
        public Move[] move;
        public Move moveData;
        public Probabilities probabilities;
        public CubeDecision cubeDecision = null;
        public string comment = string.Empty;

        public AIDataFromServer()
        {
            move = new Move[5];
            moveData = new Move();
            probabilities = new Probabilities();
            cubeDecision = new CubeDecision();
            comment = string.Empty;
        }

        public void LoadString(string serverMessage)
        {
            move = new Move[5];
            probabilities = new Probabilities();
            comment = string.Empty;

            JsonUtility.FromJsonOverwrite(serverMessage, this);

            if (type == "Error")
            {
                Debug.Log($"ERROR LOADING SERVER MESSAGE: {serverMessage}");
                return;
            }

            foreach (Move m in move)
            {
                moveData.LoadString(JsonUtility.ToJson(m));
            }

            if (move[0] != null) moveData = move.OrderBy(x => x.rank).FirstOrDefault();

            if (cubeDecision != null) probabilities.cubeDecision.LoadString(JsonUtility.ToJson(cubeDecision));
        }
    }

    [Serializable]
    public class Move
    {
        public int rank = -1;
        public FromTo[] movePart;
        public Probabilities probabilities;
        public MoneyEquity moneyEquity;
        public MatchEquity matchEquity;
        private int movesReturned;

        public Move()
        {
            movePart = new FromTo[5];
            probabilities = new Probabilities();
            moneyEquity = new MoneyEquity();
            matchEquity = new MatchEquity();
            MovesReturned = 0;
        }

        public void LoadString(string moves)
        {
            JsonUtility.FromJsonOverwrite(moves, this);
            MovesReturned = movePart.Length;
        }

        public int MovesReturned
        {
            get { return movesReturned; }
            private set { movesReturned = value; }
        }
    }

    /*
    { 
    "rank":1,
    "movePart":[
            { "from":"13","to":"7"},
            { "from":"8","to":"7"}
            ],
    "probabilities":
            { "greenWin":0.508,"greenWinG":0.144,"greenWinBG":0.007,"redWin":0.492,"redWinG":0.132,"redWinBG":0.006},
    "moneyEquity":
            { "green":0.229,"red":-0.229} 
    }
    */

    [Serializable]
    public class FromTo
    {
        public string from;
        public string to;

        public FromTo()
        {
            from = string.Empty;
            to = string.Empty;
        }
    }

    [Serializable]
    public class Probabilities
    {
        public float greenWin;
        public float greenWinG;
        public float greenWinBG;
        public float redWin;
        public float redWinG;
        public float redWinBG;
        public CubeDecision cubeDecision = new CubeDecision();
    }

    [Serializable]
    public class CubeDecision
    {
        public string owner;
        public string other;

        public void LoadString(string cubeDecision)
        {
            JsonUtility.FromJsonOverwrite(cubeDecision, this);
        }
    }

    [Serializable]
    public class MoneyEquity
    {
        public float green;
        public float red;
    }

    [Serializable]
    public class MatchEquity
    {
        public float green;
        public float red;
    }

    [Serializable]
    public class PingRequest
    {
        public string type = "PingRequest";
        public string id = "abc12";
    }
}

// BGBlitz Server Ping Request JSON
// {"type":"PingRequest","id":"abc12"}

/*
 * TUTOR REQUEST - RETURNS No of MOVES
 * {
"type":"TutorRequest",
"id":"yz234",
"comment":"A comment describing the request, just for debugging<",
"noise":0.05,
"cubeful":true,  
"gammons":"on",
"backgammons":"on",
"noOfMoves":"3",
"ply":2,
  "position": {
    "whosOn":"red",
    "usecube":true,
    "crawford":true,
    "jacoby":true,    
    "dice":  { "red":1, "green":2},    
    "score": { "red":0, "green":0},        
    "board" : {
      "cube":2, 
      "cubeowner":"green",      
      "points": [2,0,0,0,0,-5,0,-4,0,0,0,5,-4,0,0,0,3,0,5,-1,0,0,0,-1]
    }
  }
}
 * 
 * EVALUATION REQUEST - RETURNS THE LIKELY OUTCOME
 * 
{ 
"type":"EvalRequest",
"id":"1234ab",
"comment":"A comment describing the request, just for debugging",
"basicBet":1,
"cubeful":true,
"ply":2,
"limit":5,
"MoveNumber":1,
"gammons":"on",
"backgammons":"off",
"position": {
  "matchLength":0,
  "whosOn":"green",
  "usecube":true,
  "jacoby":true,
  "crawford":false,
  "dice":  { "red":0, "green":0 },  
  "score": { "red":0, "green":0 },  
  "board" : {
    "cube":2, 
    "cubeowner":"red",
    "points": [-2,-2,-2,-2,-3,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,2,0],
    "bar": { "red":1, "green":0 }
    "off": { "red":9, "green":-2 }
  }
}}
*/


// Requests can be either "eval" or "tutor"
// This may be valuable down the line to separate out the two functions
// This can be evaluated with a "statistics" request
