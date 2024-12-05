using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

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

        [Header("SERVER PORTS")]
        [SerializeField] bool _useLoadBalancing = false;
        [SerializeField] bool _use12344 = true;
        [SerializeField] bool _use12345 = false;

        public AIData aiDataToSend;
        public static AIDataFromServer aiDataFromServer;
        private AIDataFromServer aiServerDataBuffer;
        
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

        private List<ServerConnection> serverConnections;
        private ServerConnection serverConnection;
        private int serverConnectionCounter = 0;

        private float serverPingHeartbeat = 60f;

        private bool ESTABLISHING_INTERNET_CONNECTION = false;
        private bool HAS_INTERNET_CONNECTION = false;

        private int TEST_INTERNET_CONNECTION_COUNTER = 0;
        private int RESET_INTERNET_CONNECTION_COUNTER = 0;
        private int RESET_INTERNET_CONNECTION_COUNTER_ATTEMPTS = 10;

        private bool INTERNET_HEARTBEAT_REQUIRED = false;
        private bool MAINTAIN_HEARTBEAT = false;
        #endregion

        protected void Awake()
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
            SetupServerConnecitons();
        }

        protected void Start()
        {
            serverTimeout += ReconnectToServer;
            resendAIMessage += SendAIMessage;

            StartCoroutine(ResendMessageOnFailedConnectionCoroutine(0.05f));

            //StartInternetConnectionHeartbeat();
            //HAS_INTERNET_CONNECTION = true;

            TestServerConnections();
        }

        protected void OnDestroy()
        {
            StopAllCoroutines();

            isAlive = false;
            StopInternetConnectionHeartbeat();
            DisconnectFromTcpServer();
            ServerConnected = false;

            //isAlive = false;
            //if (socketConnection != null) socketConnection.Close();
            //if (clientReceiveThread != null && clientReceiveThread.IsAlive) clientReceiveThread.Abort();
            //ServerConnected = false;
        }

        private void SetDefaultServerConnections()
        {
            serverConnections.Clear();
            serverConnections.Add(new ServerConnection("65.109.236.16", 12344));
            serverConnections.Add(new ServerConnection("65.109.236.16", 12345));
        }

        // ESTABLISH SERVER CONNECTIONS
        #region SETUP_SERVER_CONNECTIONS

        private void TestServerConnections()
        {
            StartCoroutine(SetupServerConnecitonsCoroutine());
        }

        private IEnumerator SetupServerConnecitonsCoroutine()
        {
            ESTABLISHING_INTERNET_CONNECTION = true;

            debug_dataHandler.DebugMessage($"SETUP SERVER CONNECTIONS");

            // INITIALIZED TO USE BOTH - ESTABLISH IF BOTH ARE ACTIVE
            SetDefaultServerConnections();

            _useLoadBalancing = false;
            _use12344 = false;
            _use12345 = false;

            HAS_INTERNET_CONNECTION = false;
            var bailoutTimer = 10f;

            // TEST FIRST CONNECTION
            EstablishInternetConnection();
            while (!HAS_INTERNET_CONNECTION && bailoutTimer > 0)
            {
                bailoutTimer -= .05f;
                yield return new WaitForSeconds(.05f);
            }

            if (HAS_INTERNET_CONNECTION)
            {
                if (serverConnection.port == 12344) _use12344 = true;
                else if (serverConnection.port == 12345) _use12345 = true;
            }

            HAS_INTERNET_CONNECTION = false;
            bailoutTimer = 10f;

            // LOAD BALANCING SECOND
            EstablishInternetConnection();
            while (!HAS_INTERNET_CONNECTION && bailoutTimer > 0)
            {
                bailoutTimer -= .05f;
                yield return new WaitForSeconds(.05f);
            }

            if (HAS_INTERNET_CONNECTION)
            {
                if (serverConnection.port == 12344) _use12344 = true;
                else if (serverConnection.port == 12345) _use12345 = true;
            }

            // SHUT DOWN CONNECTION
            serverMessage = null;
            connectionRequired = false;
            DisconnectFromTcpServer();

            // SETUP LIST AGAIN WITH TESTED SERVERS
            SetupServerConnecitons();
        }

        private void SetupServerConnecitons()
        {
            serverConnections = new List<ServerConnection>();

            if (_useLoadBalancing || (_use12344 && _use12345))
            {
                _useLoadBalancing = true;
                _use12344 = true;
                _use12345 = true;

                SetDefaultServerConnections();
            }
            else if (_use12345)
            {
                _useLoadBalancing = false;
                _use12344 = false;

                serverConnections.Add(new ServerConnection("65.21.184.121", 12345));
            }
            else if (_use12344)
            {
                _useLoadBalancing = false;
                _use12345 = false;
                _use12344 = true;

                serverConnections.Add(new ServerConnection("65.21.184.121", 12344));
            }

            var connections = _useLoadBalancing ? "LOAD BALANCING" : 
                                     (_use12344 ? "12344" : 
                                     (_use12345 ? "12345" : 
                                                  "NONE!!!"));

            debug_dataHandler.DebugMessage($"USING CONNECTIONS: {connections}");

            if (serverConnections.Count > 0)
            {
                HAS_INTERNET_CONNECTION = true;
                RESET_INTERNET_CONNECTION_COUNTER = 0;
            }

            ESTABLISHING_INTERNET_CONNECTION = false;
        }

        #endregion

        // TEST PING SERVERS - ESTABLISH INTERNET CONNECTION
        #region ESTABLISH_SERVER_CONNECTED

        // DEPRACATED

        internal void StartInternetConnectionHeartbeat(float serverConnectionHeartbeat)
        {
            StopCoroutine(InternetConnectionHearbeatCoroutine(null));
            StopCoroutine(EstablishInterentConnectionCoroutine());

            INTERNET_HEARTBEAT_REQUIRED = true;

            debug_pingData.ShowMesssage = true;
            serverPingHeartbeat = serverConnectionHeartbeat;

            debug_dataHandler.DebugMessage($"STARTED INTERNET CONNECTION HEARTBEAT EVERY {serverPingHeartbeat} SECONDS");
            debug_pingData.DebugMessage($"STARTED INTERNET CONNECTION HEARTBEAT EVERY {serverPingHeartbeat} SECONDS");

            StartCoroutine(InternetConnectionHearbeatCoroutine(() => !ServerConnected));
        }

        internal void StopInternetConnectionHeartbeat()
        {
            INTERNET_HEARTBEAT_REQUIRED = false;

            StopCoroutine(InternetConnectionHearbeatCoroutine(null));
            StopCoroutine(EstablishInterentConnectionCoroutine());

            debug_dataHandler.DebugMessage($"STOPPED INTERNET CONNECTION HEARTBEAT");
            debug_pingData.DebugMessage($"STOPPED INTERNET CONNECTION HEARTBEAT");

            HAS_INTERNET_CONNECTION = true;
            debug_pingData.ShowMesssage = false;
        }

        private IEnumerator InternetConnectionHearbeatCoroutine(System.Func<bool> serverDisconnected)
        {
            yield return new WaitUntil(serverDisconnected);

            HAS_INTERNET_CONNECTION = false;

            EstablishInternetConnection();

            yield return new WaitForSeconds(serverPingHeartbeat);

            if (INTERNET_HEARTBEAT_REQUIRED || MAINTAIN_HEARTBEAT)
                StartInternetConnectionHeartbeat(serverPingHeartbeat);
        }

        // END DEPRACATED

        internal void EstablishInternetConnection()
        {
            StartCoroutine(EstablishInterentConnectionCoroutine());
        }

        private IEnumerator EstablishInterentConnectionCoroutine()
        {
            connectionRequired = true;
            ConnectToTcpServer();

            var bailoutTimer = 9f;
            yield return AiPingCorountine(() => ServerConnected);

            while (!HAS_INTERNET_CONNECTION && bailoutTimer > 0)
            {
                debug_dataHandler.DebugMessage($"NO PING RECEIVED");
                debug_pingData.DebugMessage($"NO PING RECEIVED");

                bailoutTimer -= .05f;
                yield return new WaitForSeconds(.05f);

                HAS_INTERNET_CONNECTION = IfServerPing();
            }

            if (HAS_INTERNET_CONNECTION)
            {
                debug_dataHandler.DebugMessage($"PING RECEIVED ON: {serverConnection.IPAddress} : {serverConnection.port}!!!");
                debug_pingData.DebugMessage($"PING RECEIVED ON: {serverConnection.IPAddress} : {serverConnection.port}!!!");
            }
            else
            {
                debug_dataHandler.DebugMessage($"STOPPING TEST FOR CONNECTION ON: {serverConnection.port}");
                debug_pingData.DebugMessage($"STOPPING TEST FOR CONNECTION ON: {serverConnection.port}");
            }
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

        private IEnumerator ReconnectToServers(System.Func<bool> establishedConnection)
        {           
            if (RESET_INTERNET_CONNECTION_COUNTER < RESET_INTERNET_CONNECTION_COUNTER_ATTEMPTS)
            {
                debug_dataHandler.DebugMessage($"RECONNECTING TO SERVERS: ATTEMPT #{RESET_INTERNET_CONNECTION_COUNTER}");

                RESET_INTERNET_CONNECTION_COUNTER += 1;

                TestServerConnections();

                yield return new WaitUntil(establishedConnection);

                Send();
            }
            else
            {
                debug_dataHandler.DebugMessage($"TOO MANY RECONNECT ATTEMPTS!!!");
            }
        }

        #endregion

        // HANDLE SERVER CONNECTION
        #region SERVER_CONNECTIONS

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

            debug_dataHandler.DebugMessage($"ESTABLISHING INTERNET CONNECTION ON: {serverConnection.IPAddress} : {serverConnection.port}");
            debug_pingData.DebugMessage($"ESTABLISHING INTERNET CONNECTION ON: {serverConnection.IPAddress} : {serverConnection.port}");

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
                    debug_dataHandler.DebugMessage($"FAILED CONNECTION - RESENDING A.I. MESSAGE");
                    resendAIMessage();
                    resendData = false;
                    yield break;
                }
            }
        }

        #endregion

        // HANDLE DATA SEND AND RECEIVE
        // SET AND SEND A.I. TURN DATA
        #region SEND_RECEIVE_DATA

        public void SendAIData(AIData _aiData, bool isDoublingTurn = false)
        {
            // ENSURE SERVERS ARE NOT TESTED AFTER DOUBLING FOR A.I. GAME
            if (!isDoublingTurn) TEST_INTERNET_CONNECTION_COUNTER += 1;

            this.aiDataToSend = _aiData;

            if (AiUserID != null && AiUserID != "" && AiUserID != string.Empty)
            {
                this.aiDataToSend.id = AiUserID;
            }

            // ONCE DATA IS SET CONNECT TO SERVER
            if (ESTABLISHING_INTERNET_CONNECTION)
            {
                StartCoroutine(DelayedConnectAndSendCoroutine(() => !ESTABLISHING_INTERNET_CONNECTION));
            }
            else
            {
                Send();
            }
        }

        private IEnumerator DelayedConnectAndSendCoroutine(System.Func<bool> connectionsEstablished)
        {
            yield return new WaitUntil(connectionsEstablished);
            Send();
        }

        internal void Send()
        {
            if (HAS_INTERNET_CONNECTION)
            {
                connectionRequired = true;
                ConnectToTcpServer();

                StartCoroutine(AiMessageSendCorountine(() => ServerConnected));
            }
            else
            {
                StartCoroutine(ReconnectToServers(() => !ESTABLISHING_INTERNET_CONNECTION));
            }
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
                    string clientMessage = JsonUtility.ToJson(aiDataToSend);
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
            
            debug_dataHandler.DebugMessage($"TESTING IF DATA RECEIVED: {dataBuffer}");
            debug_dataResponse.DebugMessage($"TESTING IF DATA RECEIVED: {dataBuffer}");

            // TEST FORMATTING TO ENSURE FULL MESSAGE IS RECEIVED
            if (dataBuffer != null && dataBuffer != string.Empty)
            {
                // TEST FULL FORMATTING OF MOVE OR PROBABILITY REQUESTS
                var dataBufferChars = dataBuffer.ToCharArray();
                var length = dataBufferChars.Length;
                var end = dataBufferChars[length - 3].ToString() + dataBufferChars[length - 2].ToString();

                // TEST CASE FOR NO A.I. MOVES - ENDS IN ""}"
                if (dataBufferChars[length - 3] == '"' &&
                    dataBufferChars[length - 2] == '}')
                        return TestIfNoMovesAvailable(dataBuffer);
                else if (end != "]}" && end != "}}") return false;
            }
            else return false;
            
            debug_dataHandler.DebugMessage($"DATA RECEIVED WAS CORRECTLY FORMATTED");
            debug_dataResponse.DebugMessage($"DATA RECEIVED WAS CORRECTLY FORMATTED");

            // DECODE SERVER MESSAGE
            if (serverMessage != null && !NewData) aiDataFromServer.LoadString(serverMessage);
            if (aiDataFromServer.type != "Error") NewData = true;

            // DISCONNECT FROM SERVER
            if (NewData)
            {
                debug_dataHandler.DebugMessage($"NEW DATA - DISCONNECTING FROM SERVER");
                debug_dataResponse.DebugMessage($"NEW DATA - DISCONNECTING FROM SERVER");

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

        string[] response = new string[9];
        public Move AIResponse()
        {
            debug_dataHandler.DebugMessage($"GET RESPONSE: {serverMessage}");
            debug_dataResponse.DebugMessage($"GET RESPONSE: {serverMessage}");

            NewData = false;
            serverMessage = null;

            // RE-TEST THE INTERNET CONNECTIONS PERIODICALLY
            if (TEST_INTERNET_CONNECTION_COUNTER > (2 * RESET_INTERNET_CONNECTION_COUNTER_ATTEMPTS))
            {
                TEST_INTERNET_CONNECTION_COUNTER = 0;
                TestServerConnections();
            }

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

        #endregion

        // DEBUG
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

        internal void SetUseDebugResetConnecitons(int reconnectAttempts)
        {
            RESET_INTERNET_CONNECTION_COUNTER_ATTEMPTS = reconnectAttempts;
        }

        // DATA STRUCTURES
        #region DATA_STRUCTURES

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

        [System.Serializable]
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
    }

    #endregion
}