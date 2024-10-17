using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Backgammon;

public class DebugToolkitUI : MonoBehaviour
{
    [Header("LEFT PANEL")]
    [SerializeField] Toggle ServerHeartbeatToggle;
    [SerializeField] Toggle MaintainServerHeartbeatToggle;
    [SerializeField] TMP_InputField ServerHearbeatInputField;

    [Header("RIGHT PANEL")]
    [Header("APP")]
    [SerializeField] Toggle DebugGameAssetManagerToggle;
    [SerializeField] Toggle DebugLoginToggle;
    [SerializeField] Toggle DebugPlayerPrefsToggle;
    [SerializeField] Toggle DebugPlayerDataToggle;
    [SerializeField] Toggle DebugReportStateToggle;
    [SerializeField] Toggle DebugGameObjectToggle;

    [Header("DATA")]
    [SerializeField] Toggle DebugGameAIDataHandlerToggle;
    [SerializeField] Toggle DebugGameAIDataSentToggle;
    [SerializeField] Toggle DebugGameAIDataReceivedToggle;
    [SerializeField] Toggle DebugGameAIDoublingDataReceivedToggle;
    [SerializeField] Toggle DebugGameAIPingServerToggle;

    [Header("BUTTONS")]
    [SerializeField] Button RestartButton;
    [SerializeField] Button DeleteOutputFileButton;
    [SerializeField] Button ContinueButton;

    [Header("DEBUG")]
    [SerializeField] DebugPrefab debug_debugToolkit = null;

    [Header("ACCESSORS")]
    //LEFT PANEL
    public bool ClickedServerHeartbeat = false;
    public bool ClickedMaintainServerHeartbeat = false;

    public bool UseServerHeartbeat = false;
    public bool UseMaintainServerHeartbeat = false;
    
    // RIGHT PANEL
    //APP
    public bool ClickedDebugGameAssetManager = false;
    public bool ClickedDebugLogin = false;
    public bool ClickedDebugPlayerPrefs = false;
    public bool ClickedDebugPlayerData = false;
    public bool ClickedDebugReportState = false;
    public bool ClickedDebugGameObject = false;

    public bool UseDebugGameAssetManager = false;
    public bool UseDebugLogin = false;
    public bool UseDebugPlayerPrefs = false;
    public bool UseDebugPlayerData = false;
    public bool UseDebugReportState = false;
    public bool UseDebugGameObject = false;

    //DATA
    public bool ClickedDebugAIDataHandler = false;
    public bool ClickedDebugAIDataSent = false;
    public bool ClickedDebugAIDataReceived = false;
    public bool ClickedDebugAIDoublingDataReceived = false;
    public bool ClickedDebugAIPingServer = false;

    public bool UseDebugAIDataHandler = false;
    public bool UseDebugAIDataSent = false;
    public bool UseDebugAIDataReceived = false;
    public bool UseDebugAIDoublingDataReceived = false;
    public bool UseDebugAIPingServer = false;

    // BUTTONS
    public bool ClickedRestartApp = false;
    public bool ClickedDeleteOutputFile = false;
    public bool ClickedContinue = false;

    private void Awake()
    {
        if (Main.Instance.IfUsingDebugToolkit)
        {
            debug_debugToolkit.ShowMesssage = true;

            // LEFT PANEL
            ServerHeartbeatToggle.onValueChanged.AddListener(delegate { OnClickedServerHeartbeat(); });
            MaintainServerHeartbeatToggle.onValueChanged.AddListener(delegate { OnClickedMaintainServerHeartbeat(); });

            // RIGHT PANEL
            //APP
            DebugGameAssetManagerToggle.onValueChanged.AddListener(delegate { OnClickedDebugGameAssetManager(); });
            DebugLoginToggle.onValueChanged.AddListener(delegate { OnClickedDebugLogin(); });
            DebugPlayerPrefsToggle.onValueChanged.AddListener(delegate { OnClickedDebugPlayerPrefs(); });
            DebugPlayerDataToggle.onValueChanged.AddListener(delegate { OnClickedDebugPlayerData(); });
            DebugReportStateToggle.onValueChanged.AddListener(delegate { OnClickedDebugReportState(); });
            DebugGameObjectToggle.onValueChanged.AddListener(delegate { OnClickedDebugGameObject(); });

            //DATA
            DebugGameAIDataHandlerToggle.onValueChanged.AddListener(delegate { OnClickedDebugAIDataHandler(); });
            DebugGameAIDataSentToggle.onValueChanged.AddListener(delegate { OnClickedDebugAIDataSent(); });
            DebugGameAIDataReceivedToggle.onValueChanged.AddListener(delegate { OnClickedDebugAIDataReceived(); });
            DebugGameAIDoublingDataReceivedToggle.onValueChanged.AddListener(delegate { OnClickedDebugAIDoublingDataReceived(); });
            DebugGameAIPingServerToggle.onValueChanged.AddListener(delegate { OnClickedDebugAIPingServer(); });

            // BUTTONS
            RestartButton.onClick.AddListener(() => OnClickedRestartApp());
            DeleteOutputFileButton.onClick.AddListener(() => OnClickedDeleteOutputFile());
            ContinueButton.onClick.AddListener(() => OnClickedContinue());
        }
    }

    private void OnDestroy()
    {
        if (Main.Instance.IfUsingDebugToolkit)
        {
            // LEFT PANEL
            ServerHeartbeatToggle.onValueChanged.RemoveAllListeners();
            MaintainServerHeartbeatToggle.onValueChanged.RemoveAllListeners();

            //RIGHT PANEL
            //APP
            DebugGameAssetManagerToggle.onValueChanged.RemoveAllListeners();
            DebugLoginToggle.onValueChanged.RemoveAllListeners();
            DebugPlayerPrefsToggle.onValueChanged.RemoveAllListeners();
            DebugPlayerDataToggle.onValueChanged.RemoveAllListeners();
            DebugReportStateToggle.onValueChanged.RemoveAllListeners();
            DebugGameObjectToggle.onValueChanged.RemoveAllListeners();

            //DATA
            DebugGameAIDataHandlerToggle.onValueChanged.RemoveAllListeners();
            DebugGameAIDataSentToggle.onValueChanged.RemoveAllListeners();
            DebugGameAIDataReceivedToggle.onValueChanged.RemoveAllListeners();
            DebugGameAIDoublingDataReceivedToggle.onValueChanged.RemoveAllListeners();
            DebugGameAIPingServerToggle.onValueChanged.RemoveAllListeners();

            // BUTTONS
            RestartButton.onClick.RemoveAllListeners();
            DeleteOutputFileButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.RemoveAllListeners();
        }
    }

    // U.I. INTERACTIONS 

    internal void SetActive(bool enable)
    {
        this.gameObject.SetActive(enable);

        // lEFT PANEL
        ClickedServerHeartbeat = false;
        ClickedMaintainServerHeartbeat = false;

        // RIGHT PANEL
        //APP
        ClickedDebugGameAssetManager = false;
        ClickedDebugLogin = false;
        ClickedDebugPlayerPrefs = false;
        ClickedDebugPlayerData = false;
        ClickedDebugReportState = false;
        ClickedDebugGameObject = false;

        //DATA
        ClickedDebugAIDataHandler = false;
        ClickedDebugAIDataSent = false;
        ClickedDebugAIDataReceived = false;
        ClickedDebugAIDoublingDataReceived = false;
        ClickedDebugAIPingServer = false;

        // BUTTONS
        ClickedRestartApp = false;
        ClickedDeleteOutputFile = false;
        ClickedContinue = false;
    }

    // LEFT PANEL

    private void OnClickedServerHeartbeat()
    {
        ClickedServerHeartbeat = true;
        UseServerHeartbeat = ServerHeartbeatToggle.isOn;
        
        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: HEARTBEAT - {(ServerHeartbeatToggle.isOn ? "ENABLED" : "DISABLED")}");
        
        DebugGameAIPingServerToggle.isOn = UseServerHeartbeat;
    }

    private void OnClickedMaintainServerHeartbeat()
    {
        ClickedMaintainServerHeartbeat = true;
        UseMaintainServerHeartbeat = MaintainServerHeartbeatToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: MAINTAIN HEARTBEAT - {(MaintainServerHeartbeatToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    public float GetServerHeartbeatRate()
    {
        float rate;
        var input = ServerHearbeatInputField.text;

        if (float.TryParse(input, out rate)){ }
        else
        {
            // NOT A FLOAT - DELETE CONTENT
            ServerHearbeatInputField.text = string.Empty;
            rate = 60f;
        }

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: HEARTBEAT RATE - {rate}");

        return rate;
    }

    // RIGHT PANEL
    #region APP
    private void OnClickedDebugGameAssetManager()
    {
        ClickedDebugGameAssetManager = true;
        UseDebugGameAssetManager = DebugGameAssetManagerToggle.isOn;
        
        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: GAME ASSET MANAGER - {(DebugGameAssetManagerToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugLogin()
    {
        ClickedDebugLogin = true;
        UseDebugLogin = DebugLoginToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: LOGIN - {(DebugLoginToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugPlayerPrefs()
    {
        ClickedDebugPlayerPrefs = true;
        UseDebugPlayerPrefs = DebugPlayerPrefsToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: PLAYER PREFS - {(DebugPlayerPrefsToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugPlayerData()
    {
        ClickedDebugPlayerData = true;
        UseDebugPlayerData = DebugPlayerDataToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: PLAYER DATA - {(DebugPlayerDataToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugReportState()
    {
        ClickedDebugReportState = true;
        UseDebugReportState = DebugReportStateToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: REPORT STATE - {(DebugReportStateToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugGameObject()
    {
        ClickedDebugGameObject = true;
        UseDebugGameObject = DebugGameObjectToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: GAME OBJECT - {(DebugGameObjectToggle.isOn ? "ENABLED" : "DISABLED")}");
    }
    #endregion

    #region DATA
    private void OnClickedDebugAIDataHandler()
    {
        ClickedDebugAIDataHandler = true;
        UseDebugAIDataHandler = DebugGameAIDataHandlerToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: AI DATA HANDLER - {(DebugGameAIDataHandlerToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugAIDataSent()
    {
        ClickedDebugAIDataSent = true;
        UseDebugAIDataSent = DebugGameAIDataSentToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: AI DATA SENT - {(DebugGameAIDataSentToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugAIDataReceived()
    {
        ClickedDebugAIDataReceived = true;
        UseDebugAIDataReceived = DebugGameAIDataReceivedToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: AI DATA RECEIVED - {(DebugGameAIDataReceivedToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugAIDoublingDataReceived()
    {
        ClickedDebugAIDoublingDataReceived = true;
        UseDebugAIDoublingDataReceived = DebugGameAIDoublingDataReceivedToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: AI DOUBLING DATA RECEIVED - {(DebugGameAIDoublingDataReceivedToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    private void OnClickedDebugAIPingServer()
    {
        ClickedDebugAIPingServer = true;
        UseDebugAIPingServer = DebugGameAIPingServerToggle.isOn;

        debug_debugToolkit.DebugMessage($"DEBUGGING TOOLKIT: PING SERVER - {(DebugGameAIPingServerToggle.isOn ? "ENABLED" : "DISABLED")}");
    }

    #endregion

    // BUTTONS

    private void OnClickedRestartApp() { ClickedRestartApp = true; }
    private void OnClickedDeleteOutputFile() { ClickedDeleteOutputFile = true; }
    private void OnClickedContinue() { ClickedContinue = true; }
}
