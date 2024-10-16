using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

namespace Backgammon
{
    public class AIDataHandler : MonoBehaviour
    {
        [Header("DEBUGS")]
        [SerializeField] private DebugPrefab debug_dataHandler = null;
        [SerializeField] private DebugPrefab debug_dataSent = null;
        [SerializeField] private DebugPrefab debug_doublingResponse = null;
        [SerializeField] private DebugPrefab debug_dataResponse = null;
        [SerializeField] private DebugPrefab debug_pingData = null;

        #region private members 	
        private static TcpClient socketConnection;
        private static Thread clientReceiveThread;
        private Action serverTimeout;
        private Action resendAIMessage;
        private bool serverConnected = false;
        private string serverMessage = null;
        private string aiUserID = null;
        private string aiUserName = null;
        private bool connectionRequired = false;
        private bool isAlive = false;
        private bool newData = false;
        private bool resendData = false;

        private AIDataPositionHelper aiDataPositionHelper;
        private AIDataDiceHelper aiDataDiceHelper;
        private AIDataScoreHelper aiDataScoreHelper;
        private AIDataBoardHelper aiDataBoardHelper;
        private AIDataBarHelper aiDataBarHelper;
        private AIDataOffHelper aiDataOffHelper;

        private ServerConnection serverConnection;
        private List<ServerConnection> serverConnections;
        private int serverConnectionCounter = 0;

        private float serverPingHeartbeat = 60f;

        private List<OpenDNSConnections> dNSConnections;
        #endregion

        [Header("SERVER PORTS")]
        [SerializeField] bool _useLoadBalancing = false;
        [SerializeField] bool _use12344 = true;
        [SerializeField] bool _use12345 = false;

        private bool HAS_INTERNET_CONNECTION = false;
        private bool INTERNET_HEARTBEAT_REQUIRED = false;

        public AIData aiDataToSend;
        public static AIDataFromServer aiDataFromServer;
        private AIDataFromServer aiServerDataBuffer;

        private void Awake()
        {
            aiDataToSend = new AIData();
            aiDataPositionHelper = GetAIDataPositionHelper;
            aiDataDiceHelper = GetAIDataDiceHelper;
            aiDataScoreHelper = GetAIDataScoreHelper;
            aiDataBoardHelper = GetAIDataBoardHelper;
            aiDataBarHelper = GetAIDataBarHelper;
            aiDataOffHelper = GetAIDataOffHelper;
            aiDataFromServer = new AIDataFromServer();
            aiServerDataBuffer = new AIDataFromServer();

            // BGBLITZ SERVER CONNECTIONS
            serverConnections = new List<ServerConnection>();

            if (_useLoadBalancing || (_use12344 && _use12345))
            {
                _useLoadBalancing = true;
                _use12344 = true;
                _use12345 = true;

                serverConnections.Add(new ServerConnection("65.21.184.121", 12344));
                serverConnections.Add(new ServerConnection("65.21.184.121", 12345));
            }
            else if (_use12345)
            {
                _useLoadBalancing = false;
                _use12344 = false;

                serverConnections.Add(new ServerConnection("65.21.184.121", 12345));
            }
            else
            {
                _useLoadBalancing = false;
                _use12345 = false;
                _use12344 = true;

                serverConnections.Add(new ServerConnection("65.21.184.121", 12344));
            }

            // OPEN DNS CONNECTIONS FOR INTERNET CONNECTIVITY
            dNSConnections = new List<OpenDNSConnections>
            {
                new OpenDNSConnections("8.8.8.8"),          //8.8.8.8 - GOOGLE
                new OpenDNSConnections("1.1.1.1"),          //1.1.1.1 - CLOUDFLARE
                new OpenDNSConnections("1.0.0.1"),          //1.0.0.1 - CLOUDFLARE
                new OpenDNSConnections("4.2.2.2"),          //4.2.2.2
                new OpenDNSConnections("208.67.222.222"),   //208.67.222.222 - OPEN DNS
                new OpenDNSConnections("208.67.220.220")    //208.67.220.220 - OPEN DNS
            };
        }

        void Start()
        {
            //StartInternetConnectionHeartbeat();
            HAS_INTERNET_CONNECTION = true;

            serverTimeout += ReconnectToServer;
            resendAIMessage += SendAIMessage;

            StartCoroutine(ResendMessageOnFailedConnectionCoroutine(0.05f));
        }

        private void OnDestroy()
        {
            isAlive = false;
            DisconnectFromTcpServer();
            ServerConnected = false;

            //isAlive = false;
            //if (socketConnection != null) socketConnection.Close();
            //if (clientReceiveThread != null && clientReceiveThread.IsAlive) clientReceiveThread.Abort();
            //ServerConnected = false;
        }

        internal void SetUseDebugAIDataHandler(bool useDebugLogging)
        {
            debug_dataHandler.ShowMesssage = useDebugLogging;
        }

        internal void SetUseDebugObjects(bool dataSent, bool dataReceived, bool doublingDataReceived, bool pingServer)
        {
            debug_dataSent.ShowMesssage = dataSent;
            debug_dataResponse.ShowMesssage = dataReceived;
            debug_doublingResponse.ShowMesssage = doublingDataReceived;
            debug_pingData.ShowMesssage = pingServer;
        }

        // HANDLE SERVER CONNECTION

        private void ConnectToTcpServer()
        {
            try
            {
                debug_dataHandler.DebugMessage("CONNECTING TO SERVER...");
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
                serverMessage = null;
                isAlive = true;
                NewData = false;
            }
            catch (Exception e)
            {
                debug_dataHandler.DebugMessage("On client connect exception " + e);
            }
        }

        private void ReconnectToServer()
        {
            debug_dataHandler.DebugMessage($"RECONNECTING TO SERVER");
            isAlive = false;
            ConnectToTcpServer();
        }

        internal void DisconnectFromTcpServer()
        {
            connectionRequired = false;
            isAlive = false;

            debug_dataHandler.DebugMessage("BEGIN DISCONNECT...");

            if (socketConnection is not null && socketConnection.Connected)
            {
                try
                {
                    socketConnection.Client.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    socketConnection.Client.Close();
                    debug_dataHandler.DebugMessage("SOCKET CLOSED");
                }
            }

            if (clientReceiveThread is not null)
            {
                if (clientReceiveThread.IsAlive) clientReceiveThread.Abort();
                clientReceiveThread.Join();
            }

            ServerConnected = false;
        }

        public void ListenForData()
        {
            // SWITCH PORTS ON EACH CONNECT - LOAD BALANCE

            serverConnection = serverConnections.ToArray()[serverConnectionCounter++ % serverConnections.Count];

            debug_dataHandler.DebugMessage($"CONNECTION: {serverConnection.IPAddress} PORT:{serverConnection.port}");
            debug_dataSent.DebugMessage($"CONNECTION: {serverConnection.IPAddress} PORT:{serverConnection.port}");

            try
            {
                socketConnection = new TcpClient(serverConnection.IPAddress, serverConnection.port);
                Byte[] bytes = new Byte[2048];
                serverMessage = string.Empty;

                while (isAlive)
                {
                    debug_dataHandler.DebugMessage("IS ALIVE");

                    // Test socket connection
                    if (!socketConnection.Connected && connectionRequired)
                    {
                        debug_dataHandler.DebugMessage("DISCONNECTED LISTEN");
                        serverTimeout();
                        return;
                    }
                    else if (socketConnection.Connected)
                    {
                        ServerConnected = true;

                        debug_dataHandler.DebugMessage("TEST SOCKET " + socketConnection.Connected);

                        using (NetworkStream stream = socketConnection.GetStream())
                        {
                            int _length;

                            do
                            {
                                if (stream.DataAvailable)
                                {
                                    _length = stream.Read(bytes, 0, bytes.Length);
                                    var _incommingData = new byte[_length];
                                    Array.Copy(bytes, 0, _incommingData, 0, _length);
                                    serverMessage += Encoding.UTF8.GetString(_incommingData);
                                    debug_dataHandler.DebugMessage("server message received as: " + serverMessage);
                                }
                                else
                                {
                                    // ALLOW REFERENCES TO BE UPDATED
                                    Thread.Sleep(1);
                                }
                            }
                            while (connectionRequired);

                            stream.Close();
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                debug_dataHandler.DebugMessage("Socket exception: " + socketException);
                ReconnectToServer();
            }
            catch (ThreadAbortException threadException)
            {
                debug_dataHandler.DebugMessage("ThradException: " + threadException);
            }
            finally
            {
                debug_dataHandler.DebugMessage("END WHILE");
                ServerConnected = false;
            }
        }

        IEnumerator ResendMessageOnFailedConnectionCoroutine(float timeDelay)
        {
            while (true)
            {
                yield return new WaitForSeconds(timeDelay);

                if (resendData)
                {
                    resendAIMessage();
                    resendData = false;
                    yield break;
                }
            }
        }

        // TEST PING SERVERS - ESTABLISH INTERNET CONNECTION

        internal void StartInternetConnectionHeartbeat(float serverConnectionHeartbeat)
        {
            StopCoroutine(InternetConnectionHearbeatCoroutine());
            StopCoroutine(EstablishInterentConnectionCoroutine());

            INTERNET_HEARTBEAT_REQUIRED = true;

            debug_pingData.ShowMesssage = true;
            serverPingHeartbeat = serverConnectionHeartbeat;

            debug_dataHandler.DebugMessage($"STARTED INTERNET CONNECTION HEARTBEAT EVERY {serverPingHeartbeat} SECONDS");
            debug_pingData.DebugMessage($"STARTED INTERNET CONNECTION HEARTBEAT EVERY {serverPingHeartbeat} SECONDS");

            StartCoroutine(InternetConnectionHearbeatCoroutine());
        }

        internal void StopInternetConnectionHeartbeat()
        {
            INTERNET_HEARTBEAT_REQUIRED = false;

            StopCoroutine(InternetConnectionHearbeatCoroutine());
            StopCoroutine(EstablishInterentConnectionCoroutine());

            debug_dataHandler.DebugMessage($"STOPPED INTERNET CONNECTION HEARTBEAT");
            debug_pingData.DebugMessage($"STOPPED INTERNET CONNECTION HEARTBEAT");

            HAS_INTERNET_CONNECTION = true;
            debug_pingData.ShowMesssage = false;
        }

        private IEnumerator InternetConnectionHearbeatCoroutine()
        {
            yield return new WaitForSeconds(.05f);

            if (ServerConnected) yield return new WaitForSeconds(.5f);

            HAS_INTERNET_CONNECTION = false;

            EstablishInternetConnection();

            yield return new WaitForSeconds(serverPingHeartbeat);

            if (INTERNET_HEARTBEAT_REQUIRED)
                StartInternetConnectionHeartbeat(serverPingHeartbeat);
        }

        internal void EstablishInternetConnection()
        {
            StartCoroutine(EstablishInterentConnectionCoroutine());
        }

        private IEnumerator EstablishInterentConnectionCoroutine()
        {
            debug_dataHandler.DebugMessage($"ESTABLISHING INTERNET CONNECTION");
            debug_pingData.DebugMessage($"ESTABLISHING INTERNET CONNECTION");

            connectionRequired = true;
            ConnectToTcpServer();

            yield return AiPingCorountine(() => ServerConnected);

            while(!HAS_INTERNET_CONNECTION)
            {
                HAS_INTERNET_CONNECTION = IfServerPing();

                debug_dataHandler.DebugMessage($"NO PING RECEIVED");
                debug_pingData.DebugMessage($"NO PING RECEIVED");
                yield return new WaitForSeconds(.05f);
            }

            debug_dataHandler.DebugMessage($"PING RECEIVED ON: {serverConnection.IPAddress} : {serverConnection.port}!!!");
            debug_pingData.DebugMessage($"PING RECEIVED ON: {serverConnection.IPAddress} : {serverConnection.port}!!!");
        }

        internal void Ping()
        {
            StartCoroutine(AiPingCorountine(() => ServerConnected));
        }

        private IEnumerator AiPingCorountine(System.Func<bool> serverConnected)
        {
            yield return new WaitUntil(serverConnected);

            PingServer();
        }

        public void PingServer()
        {
            PingRequest ping = new PingRequest();

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();

                if (stream.CanWrite)
                {
                    string clientMessage = JsonUtility.ToJson(ping);
                    byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);                
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);

                    debug_dataHandler.DebugMessage($"PING SENT {clientMessage}");
                    debug_dataSent.DebugMessage($"PING SENT {clientMessage}");
                    debug_pingData.DebugMessage($"PING SENT {clientMessage}");

                    debug_dataHandler.DebugMessage("Client sent his message - should be received by server");
                    debug_pingData.DebugMessage("Client sent his message - should be received by server");
                }
            }
            catch (SocketException socketException)
            {
                debug_dataHandler.DebugMessage("Socket exception: " + socketException);
                debug_pingData.DebugMessage("Socket exception: " + socketException);
            }
        }

        public bool IfServerPing()
        {
            var dataBuffer = serverMessage;

            // TEST FORMATTING TO ENSURE FULL MESSAGE IS RECEIVED
            if (dataBuffer != null && dataBuffer != string.Empty)
            {
                debug_dataHandler.DebugMessage($"PING DATA BUFFER: {dataBuffer}");
                debug_pingData.DebugMessage($"PING DATA BUFFER: {dataBuffer}");
                
                aiServerDataBuffer.LoadString(dataBuffer);

                debug_dataHandler.DebugMessage($"PING SERVER MESSAGE: {aiServerDataBuffer}");
                debug_pingData.DebugMessage($"PING SERVER MESSAGE: {aiServerDataBuffer}");

                // TEST FOR SERVER PING
                if (aiServerDataBuffer.type != "Error" &&
                    aiServerDataBuffer.type == "Response" &&
                    aiServerDataBuffer.response.Contains("alive"))
                {
                    connectionRequired = false;
                    DisconnectFromTcpServer();
                    
                    debug_dataHandler.DebugMessage($"SERVER MESSAGE RECEIVED: {aiServerDataBuffer}");
                    debug_pingData.DebugMessage($"SERVER MESSAGE RECEIVED: {aiServerDataBuffer}");

                    serverMessage = null;

                    return true;
                }
            }

            return false;
        }

        // HANDLE DATA SEND AND RECEIVE
        // SET AND SEND A.I. TURN DATA
        public void SetAIData(AIData _aiData)
        {
            this.aiDataToSend = _aiData;

            if (AiUserID != null && AiUserID != "" && AiUserID != string.Empty)
            {
                this.aiDataToSend.id = AiUserID;
            }

            // ONCE DATA IS SET CONNECT TO SERVER
            if (HAS_INTERNET_CONNECTION)
            {
                connectionRequired = true;
                ConnectToTcpServer();
            }
        }

        internal void Send()
        {
            StartCoroutine(AiMessageSendCorountine(() => ServerConnected));
        }

        private IEnumerator AiMessageSendCorountine(System.Func<bool> serverConnected)
        {
            yield return new WaitUntil(serverConnected);

            SendAIMessage();
        }

        public void SendAIMessage()
        {
            debug_dataHandler.DebugMessage("SEND MESSAGE");

            if (socketConnection == null)
            {
                debug_dataHandler.DebugMessage("SOCKET NOT CONNECTED");
                return;
            }
            else if (!socketConnection.Connected)
            {
                debug_dataHandler.DebugMessage("DISCONNECTED SEND");
                resendData = true;
                return;
            }

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    //string clientMessage = JsonUtility.ToJson(aiDataToSend);
                    string clientMessage = JsonConvert.SerializeObject(aiDataToSend);
                    byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);                
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);

                    debug_dataHandler.DebugMessage($"MESSAGE SENT {clientMessage}");
                    debug_dataSent.DebugMessage($"MESSAGE SENT {clientMessage}");

                    debug_dataHandler.DebugMessage("Client sent his message - should be received by server");
                }
            }
            catch (SocketException socketException)
            {
                debug_dataHandler.DebugMessage("Socket exception: " + socketException);
            }
        }

        public bool IfNewData()
        {
            // DEFAULT THE DATA UNTIL THE SERVER MESSAGE HAS BEEN READ
            aiDataFromServer.type = "Error";

            var dataBuffer = serverMessage;

            // TEST FORMATTING TO ENSURE FULL MESSAGE IS RECEIVED
            if (dataBuffer != null && dataBuffer != string.Empty)
            {
                // TEST FULL FORMATTING OF MOVE OR PROBABILITY REQUESTS
                var dataBufferChars = dataBuffer.ToCharArray();
                var length = dataBufferChars.Length;
                var end = dataBufferChars[length - 3].ToString() + dataBufferChars[length - 2].ToString();

                // TEST CASE FOR NO A.I. MOVES - ENDS IN ""}"
                if (dataBufferChars[length - 3] == '"' &&
                    dataBufferChars[length - 2] == '}') TestIfNoMovesAvailable(dataBuffer);
                else if (end != "]}" && end != "}}") return false;
            }
            else return false;

            // DECODE SERVER MESSAGE
            if (serverMessage != null && !NewData) aiDataFromServer.LoadString(serverMessage);
            if (aiDataFromServer.type != "Error") NewData = true;

            // DISCONNECT FROM SERVER
            if (NewData)
            {
                connectionRequired = false;
                DisconnectFromTcpServer();
            }

            return NewData;
        }

        private bool TestIfNoMovesAvailable(string dataBuffer)
        {
            aiServerDataBuffer.LoadString(dataBuffer);

            if (aiServerDataBuffer.type != "Error" &&
               (aiServerDataBuffer.comment == " No move possible " ||
                aiServerDataBuffer.comment.Replace(" ", string.Empty) == "Nomovepossible"))
            {
                debug_dataResponse.DebugMessage($"NO MOVE POSSIBLE");

                // NOTE: ENSURE THERE IS NO PREVIOUS MOVE LOADED..
                if (aiServerDataBuffer.move[0] is null)
                {
                    debug_dataResponse.DebugMessage($"MOVE DATA WAS NULL");
                    return true;
                }
                else
                {
                    debug_dataResponse.DebugMessage($"***PREVIOUS MOVE DATA WAS NOT NULL***");
                    aiServerDataBuffer.move[0] = null;
                    return true;
                }
            }
            else return false;
        }

        // NEWTONSOFT JSON - DESERIALIZE AND VALIDATE
        private void ValidateJson(string srvMsg)
        {
            Debug.Log($"VALIDATE OBJECT");

            List<string> errors = new List<string>();
            var settings = new JsonSerializerSettings { Error = (se, ev) =>
            {
                errors.Add(ev.ErrorContext.Error.Message);
                ev.ErrorContext.Handled = true;
            } };
            AIDataFromServer srvBfr = JsonConvert.DeserializeObject<AIDataFromServer>(srvMsg, settings);

            foreach (var e in errors)
                Debug.Log(e);

            Debug.Log($"SVR_MSG: {srvMsg}");
            Debug.Log($"SVR_BFR: {srvBfr.move[0].probabilities.redWin}");

            DebugLogServerResponse(srvBfr);

            var valid = false;

            if (srvMsg != null && srvMsg != string.Empty)
            {
                // TEST CASE FOR NO A.I. MOVES
                if (srvBfr.type != "Error" &&
                   (srvBfr.comment == " No move possible " ||
                    srvBfr.comment.Replace(" ", string.Empty) == "Nomovepossible") &&
                    srvBfr.move[0] is null)
                    valid = true;

                // TEST FULL FORMATTING OF MOVE OR PROBABILITY REQUESTS
                var dataBufferChars = srvMsg.ToCharArray();
                var length = dataBufferChars.Length;
                var end = dataBufferChars[length - 3].ToString() + dataBufferChars[length - 2].ToString();

                if (end != "]}" && end != "}}") valid = false;
            }
            else valid = false;

            Debug.Log($"VALID -> {valid}");
        }

        private void DebugLogServerResponse(AIDataFromServer json)
        {
            Debug.Log($"");
            JToken jt = JToken.Parse(JsonConvert.SerializeObject(json));
            Debug.Log($"{jt.ToString()}");
            Debug.Log($"");
        }

        // END - NEWTONSOFT

        string[] response = new string[9];
        public Move AIResponse()
        {
            debug_dataHandler.DebugMessage("GET RESPONSE");
            debug_dataResponse.DebugMessage($"{serverMessage}");

            NewData = false;
            serverMessage = null;

            return aiDataFromServer.moveData;
        }

        public Probabilities AIDoublingResponse()
        {
            debug_dataHandler.DebugMessage($"GET DOUBLING");
            debug_doublingResponse.DebugMessage($"{serverMessage}");

            NewData = false;
            serverMessage = null;

            return aiDataFromServer.probabilities;
        }

        public string AIServerMessage()
        {
            if (serverMessage is null) return "NULL";
            else return serverMessage;
        }

        public Move[] AIResponseMoves()
        {
            return aiDataFromServer.move;
        }

        public int AIResponseMovesLength()
        {
            return aiDataFromServer.move.Length;
        }

        // DATA STRUCTURES

        public bool ServerConnected
        {
            get => serverConnected;
            private set => serverConnected = value;
        }

        public bool NewData
        {
            get { return newData; }
            private set { newData = value; }
        }

        public static AIData AIDataObject { get; } = new AIData();

        public static AIDataPositionHelper GetAIDataPositionHelper { get; } = new AIDataPositionHelper();

        public static AIDataDiceHelper GetAIDataDiceHelper { get; } = new AIDataDiceHelper();

        public static AIDataScoreHelper GetAIDataScoreHelper { get; } = new AIDataScoreHelper();

        public static AIDataBoardHelper GetAIDataBoardHelper { get; } = new AIDataBoardHelper();

        public static AIDataBarHelper GetAIDataBarHelper { get; } = new AIDataBarHelper();

        public static AIDataOffHelper GetAIDataOffHelper { get; } = new AIDataOffHelper();

        public string AiUserID { get => aiUserID; set => aiUserID = value; }

        public bool UseLoadBalancing { set => _useLoadBalancing = value; }

        private struct ServerConnection
        {
            public string IPAddress { get; private set; }
            public int port { get; private set; }

            public ServerConnection(string _IPAddress, int _port)
            {
                IPAddress = _IPAddress;
                port = _port;
            }
        }

        private struct OpenDNSConnections
        {
            public string IPAddress { get; private set; }

            public OpenDNSConnections(string dnsEndPoint)
            {
                IPAddress = dnsEndPoint;
            }
        } 
    }
}