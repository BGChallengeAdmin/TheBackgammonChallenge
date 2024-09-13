using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon_Asset
{
    public class uiHandler : MonoBehaviour
    {
        public Button build;
        public bool clickedBuild = false;

        private string textFolder = null;
        private string buildFolder = null;
        private bool arePathsValid = false;

        private bool bundled = true;

        private void Awake()
        {
            // SET THESE VALUES TO THE FOLDER CONTAINING THE TEXT FILE 0 - 4
            if (bundled)
            {
                textFolder = "Assets/HistoricMatches/TextFiles/_startingMatches/";
                textFolder = "Assets/HistoricMatches/TextFiles/TournamentMatchBundle003/";
                textFolder = "Assets/HistoricMatches/TextFiles/UKMatchBundle103/";
                textFolder = "Assets/HistoricMatches/TextFiles/USMatchBundle203/";
                //textFolder = "Assets/HistoricMatches/TextFiles/UKMatchBundles - NotBundled/";
                //textFolder = "Assets/HistoricMatches/TextFiles/UKMatchBundlesByPlayer/Dwek/";

                buildFolder = "Assets/HistoricMatches/_startingMatches/";
                buildFolder = "Assets/HistoricMatches/TournamentMatchBundle003/";
                buildFolder = "Assets/HistoricMatches/UKMatchBundle103/";
                buildFolder = "Assets/HistoricMatches/USMatchBundle203/";
                //buildFolder = "Assets/HistoricMatches/UKMatchBundles - NotBundled/";
                //buildFolder = "Assets/HistoricMatches/UKMatchBundlesByPlayer/Dwek/";
            }
            else
            {
                textFolder = "Assets/HistoricMatches/TextFiles/NotBundled/";
                buildFolder = "Assets/HistoricMatches/NotBundled/";
            }
        }

        public void OnClickedBuild()
        {
            clickedBuild = true;

            TestPathsAreValid();
        }

        private void TestPathsAreValid()
        {
            // SANITY CHECK LOCATIONS IN CASE BUILDING NEW ASSET FOLDER STRUCTURE

            if (Directory.Exists(TextFolder)) Debug.Log("TEXT FOLDER VALID");
            if (Directory.Exists(BuildFolder)) Debug.Log("BUILD FOLDER VALID");
            if (Directory.Exists(TextFolder) && Directory.Exists(BuildFolder)) ArePathsValid = true;
            else ArePathsValid = false;
        }

        // ______________________________________________________ GETTERS && SETTERS ______________________________________________________

        public string TextFolder
        {
            get => textFolder;
            private set => textFolder = value;
        }

        public string BuildFolder
        {
            get => buildFolder;
            private set => buildFolder = value;
        }

        public bool ArePathsValid
        {
            get => arePathsValid;
            private set => arePathsValid = value;
        }
    }
}