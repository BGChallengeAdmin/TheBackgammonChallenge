using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backgammon_Asset;

namespace Backgammon_Asset
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        private uiHandler userInterface = null;
        [SerializeField]
        private HistoricMatchHandler matchHandler = null;
        [SerializeField]
        private BuildComplete buildComplete = null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            appState = AppState.MAIN_userInterface;
        }

        private void Update()
        {
            switch (appState)
            {
                case AppState.MAIN_userInterface:
                    {
                        if (userInterface.clickedBuild)
                        {
                            userInterface.clickedBuild = false;

                            if (userInterface.ArePathsValid)
                            {
                                HistoricMatchHandler.TextFolder = userInterface.TextFolder;
                                HistoricMatchHandler.BuildFolder = userInterface.BuildFolder;

                                appState = AppState.IN_buildAsset;
                                break;
                            }
                            else
                            {
                                // HANDLE ERRORS IN FILE PATHS AND BUILDS

                                Debug.Log("ERROR - PATHS ARE NOT VALID");
                            }
                        }
                    }
                    break;
                case AppState.IN_buildAsset:
                    {
                        // HANDLE CONSTRUCTION OF FILE PATHS AND BUILDS
                        if (!matchHandler.BuildingAssets)
                        {
                            matchHandler.gameObject.SetActive(true);
                            HistoricMatchHandler.ConstructMatchAssets();
                            matchHandler.Building(true);
                        }
                        else if (matchHandler.BuildConfigured)
                        {
                            appState = AppState.UPDATE_historicMatchHandler;
                        }
                        else if (matchHandler.BuildFailed)
                        {
                            appState = AppState.OUT_buildFailed;
                            break;
                        }
                    }
                    break;
                case AppState.UPDATE_historicMatchHandler:
                    {
                        if (matchHandler.BuildFailed)
                        {
                            appState = AppState.OUT_buildFailed;
                            break;
                        }
                        else if (matchHandler.BuildingAssets && matchHandler.BuildConfigured)
                        {
                            HistoricMatchHandler.ConstructAssetBundle();
                        }
                        else if (matchHandler.BuildComplete)
                        {
                            Debug.Log("BUILD COMPLETED IN ...");

                            matchHandler.Reset();
                            matchHandler.gameObject.SetActive(false);

                            appState = AppState.OUT_buildComplete;
                            break;
                        }
                    }
                    break;
                case AppState.OUT_buildComplete:
                    {
                        // HANDLE BUILD COMPLETE

                        buildComplete.gameObject.SetActive(true);
                        buildComplete.SetHeaderText("COMPLETE");
                    }
                    break;
                case AppState.OUT_buildFailed:
                    {
                        // HANDLE BUILD FAILED ERRORS AND RETURN TO UI

                        Debug.Log("BUILD FAILED");

                        matchHandler.Reset();
                        matchHandler.gameObject.SetActive(false);

                        appState = AppState.MAIN_userInterface;
                    }
                    break;
            }
        }

        public enum AppState
        {
            MAIN_userInterface,
            IN_buildAsset,
            UPDATE_historicMatchHandler,
            OUT_buildComplete,
            OUT_buildFailed,
        }

        public AppState appState
        {
            get;
            private set;
        }

        // --- singleton-related -------------------------------------------------------------------------------------------------------------

        public static Main Instance
        {
            get
            {
                return (instance);
            }
        }

        private static Main instance = null;
    }
}