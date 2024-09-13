using TMPro;
using UnityEngine;

public class DebugTextToUI : MonoBehaviour
{
    [SerializeField] TMP_Text debug_textToUI;

    private void Awake()
    {
        if (!display_debugMessages)
            this.gameObject.SetActive(false);
    }

    internal void AddDebugMessage(string message)
    {
        if (display_debugMessages && (messageCounter++ < max_messageCount))
        {
            var debug = debug_textToUI.text;
            debug_textToUI.text = debug + message + "\n";
        }
    }

    private int messageCounter = 0;
    public int max_messageCount = 20;
    public bool display_debugMessages = true;
}
