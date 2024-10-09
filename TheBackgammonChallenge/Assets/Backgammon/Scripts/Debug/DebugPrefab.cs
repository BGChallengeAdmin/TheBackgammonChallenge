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

        public void DebugMessage(string message)
        {
            if (!showMessage) return;

            Debug.Log($"{objName} - {message}");

            debugOutputLogFile.WriteToDebugLogFile(message);
        }

        public bool ShowMesssage { set => showMessage = value; }
    }
}