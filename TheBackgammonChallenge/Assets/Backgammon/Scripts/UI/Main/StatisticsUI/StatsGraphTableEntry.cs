using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class StatsGraphTableEntry : MonoBehaviour
    {
        public Text playerName;
        public Text movesPlayed;
        public Text topMatches;
        public Text topMatchPercent;
        public Text aiMatches;
        public Text aiMatchPercent;
        public Text proAIMatches;
        public Text proAiMatchePercent;
        public Text ranking;

        public void SetEntryStats(string _name, string _moves, string _movesMatched, string _matchPercent,
            string _aiMatched, string _aiMatchPercent, string _proAIMatch, string _proAIMatchPercent, string _ranking)
        {
            playerName.text = _name;
            movesPlayed.text = _moves;
            topMatches.text = _movesMatched;
            topMatchPercent.text = _matchPercent;
            aiMatches.text = _aiMatched;
            aiMatchPercent.text = _aiMatchPercent;
            proAIMatches.text = _proAIMatch;
            proAiMatchePercent.text = _proAIMatchPercent;
            ranking.text = _ranking;
        }
    }
}