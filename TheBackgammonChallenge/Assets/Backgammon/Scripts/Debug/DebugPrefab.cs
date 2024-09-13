using UnityEngine;

namespace Backgammon
{
    public class DebugPrefab : MonoBehaviour
    {
        [SerializeField] private string objName;
        [SerializeField] private bool showMessage;

        public void DebugMessage(string message)
        {
            if (!showMessage) return;

            Debug.Log($"{objName} - {message}");
        }

        public bool ShowMesssage { set => showMessage = value; }
    }
}