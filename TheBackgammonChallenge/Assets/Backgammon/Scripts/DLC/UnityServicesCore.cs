using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace Backgammon
{
    public class UnityServicesCore : MonoBehaviour
    {
        public string environment = "production";

        async void Awake()
        {
            Debug.Log($"UNITY CORE INITIALIZE");

            try
            {
                var options = new InitializationOptions().SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);
            }
            catch (Exception exception)
            {
                Debug.Log($"UNITY CORE: {exception}");
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeMethodLoad()
        {
            UnityEngine.Analytics.Analytics.initializeOnStartup = false;
            UnityEngine.Analytics.Analytics.deviceStatsEnabled = false;
            UnityEngine.Analytics.Analytics.limitUserTracking = true;
            UnityEngine.Analytics.Analytics.enabled = false;
            UnityEngine.Analytics.PerformanceReporting.enabled = false;
        }

        // --------------------------------------------------- SINGLETON -------------------------------------------------

        private static UnityServicesCore instance = null;

        public static UnityServicesCore Instance
        {
            get { return instance ?? (instance = new UnityServicesCore()); }
        }

        private UnityServicesCore() { }
    }
}