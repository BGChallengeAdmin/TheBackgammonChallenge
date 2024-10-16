using UnityEngine;

namespace Backgammon
{
    public class DebugPrefab : MonoBehaviour
    {
        [SerializeField] private string objName;
        [SerializeField] private bool showMessage;

        private static DebugOutputLogFile debugOutputLogFile = null;

        public static void Init()
        {
            debugOutputLogFile = new DebugOutputLogFile();
            debugOutputLogFile.Init();
        }

        public static void DebugMessageStatic(string message)
        {
            debugOutputLogFile.WriteToDebugLogFile(message);
        }

        public static void DeleteOutputFile()
        {
            debugOutputLogFile.DeleteOutputFile();
        }

        public void DebugMessage(string message)
        {
            if (!showMessage) return;

            Debug.Log($"{objName} - {message}");

            debugOutputLogFile.WriteToDebugLogFile($"{objName} - {message}");
        }

        private void OnDestroy()
        {
            if (debugOutputLogFile is not null)
                debugOutputLogFile.OnDestroy();
        }

        public bool ShowMesssage { set => showMessage = value; }
    }
}