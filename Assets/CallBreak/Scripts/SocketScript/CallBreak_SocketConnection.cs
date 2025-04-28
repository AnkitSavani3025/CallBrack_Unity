using System;
using System.Collections;
using BestHTTP.SocketIO3;
using DG.Tweening;
using ThreePlusGamesCallBreak;
using UnityEngine;

namespace CallBreak_Socketmanager
{
    public class CallBreak_SocketConnection : MonoBehaviour
    {
        #region Variables
        public static CallBreak_SocketConnection intance;
        private SocketManager manager;
        public SocketState socketState;
        bool isFirstBackground;
        internal bool isSignupRecived;
        int socketErrorCounter;
        [SerializeField] int firstCheckConnectionDelay = 3;
        [SerializeField] float singUpRetryTime = 1.5f;

        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = false;
            Application.targetFrameRate = 60;

            if (Application.isMobilePlatform)
                QualitySettings.vSyncCount = 0;

            if (intance == null)
                intance = this;
        }

        #endregion
        private void Start()
        {
            if (MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE)
                CallBreak_GameConfig.Inst.isFTUEFinished = false;
            if (!isFirstBackground)
                StartConnection();
            else Debug.Log(" Socket Connetion Start From OnApplicationPause Funcation No Need from start ");
        }
        public void StartConnection()
        {
            if (!IsInternetConnection())
            {
                Debug.LogError(" No Internet Found In Your Device ");
                CallBreak_InternetHandler.Inst.Show_Nointernetpopup();  // Show No internet alert popup
            }
            else if (CallBreak_GameConfig.Inst.isFTUEFinished)
            {
                CallBreak_InternetHandler.Inst.Close_Nointernetpopup();  // Close internet alert popup If Internet available 

                string hostURL;
                if (int.Parse(MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.portNumber) != 0) hostURL = MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.hostURL + ":" + MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.portNumber;
                else// No Need to Assign port number if directly code run on server
                    hostURL = MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.hostURL;// + ":" + MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.portNumber;

                Debug.Log(" This is Not first Time user interaction start connection to URL : " + hostURL);
                CancelInvoke(nameof(StartConnection));
                SocketConnectionStart(hostURL);
            }
            else
            {
                Debug.Log(" This is a FTUE Show first Time user interaction ");
                CallBreak_FTUEManager.instance.StartFUTE();
            }
            CheckSocketConnectedAlltime();
        }

        #region SocketConnection
        public void SocketConnectionStart(string socketURL)
        {
            if (socketState == SocketState.Open)
            {
                Debug.Log("Socket already connected Retrun from here ");
                return;
            }
            socketURL = socketURL + "/socket.io/";

            Debug.Log(" <color=green> Socket Connection Start At URL -> </color>" + socketURL);
            CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(true);

            if (manager != null)
            {
                Debug.Log(" Socket Alredy Conneted First Dissconect it ");
                manager.Socket.Disconnect();
                manager = null;
            }

            manager = new SocketManager(new Uri(socketURL), Overrided_SocketOptions());

            manager.Socket.On(SocketIOEventTypes.Connect, SocketConnected);

            manager.Socket.On<CallBreakCustomError>(SocketIOEventTypes.Error, SocketError);

            manager.Socket.On(SocketIOEventTypes.Disconnect, SocketDisconnect);

            manager.Socket.On("HEART_BEAT", SocketHeartBeat);

            RegisterCustomEvents();

            CallBreak_InternetHandler.Inst.CheckInternetWithPINGPONG();

        }

        #endregion



        #region On Socket Methods For Communication 
        void SocketConnected()
        {
            Debug.Log("<color=green> Socket is now Connected   </color>");
            CallBreak_InternetHandler.Inst.Close_Nointernetpopup();
            socketState = SocketState.Open;
            socketErrorCounter = 0;
            ConnectionSignUp();
        }

        public void SocketDisconnect()
        {
            socketState = SocketState.Disconnect;
            CallBreak_GS.Inst.isSocketClosed = true;
            manager.Socket.Disconnect();
            CallBreak_InternetHandler.Inst.needToCheckInternet = false;
            Debug.LogError("<color=red> Socket Disconnect</color>");
        }

        public void SocketError(CallBreakCustomError CustomError)
        {
            Debug.LogError(" Socket Error : " + CustomError.message);
            socketState = SocketState.Error;
            ForcefullyCloseSocket();

            if (socketErrorCounter > 4)
            {
                if (checkSocketCorotine != null)
                    StopCoroutine(checkSocketCorotine);
                CallBreak_AllPopupHandler.Instance.OpenOfflineCommonPopup("SocketError");
            }
            socketErrorCounter++;

        }

        private void SocketHeartBeat()
        {
            CallBreak_InternetHandler.isPongReceived = true;
            socketState = SocketState.Running;
            Debug.Log("HEART_BEAT Recieved in Socket: ");
        }

        #endregion


        #region Send Sign Up
        public void ConnectionSignUp()
        {
            try
            {

                CallBreak_GS.Inst.isBootCollected = isSignupRecived = false;

                if (CallBreak_GS.Inst.CurrentTableState.Equals(CurrentTableState.Waiting) || CallBreak_GS.Inst.CurrentTableState.Equals(CurrentTableState.none))
                    CallBreak_GS.Inst.fromBack = false;
                CallBreak_AllPopupHandler.Instance.HideServer_CommonPopup();
                SendData(CallBreak_SocketEventManager.SignUp_New(), SignUpAcknowledgement, CallBreak_CustomEvents.SIGNUP);
                CancelInvoke(nameof(CheckSignupAckRecivedOrNot));
                Invoke(nameof(CheckSignupAckRecivedOrNot), singUpRetryTime);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                throw;
            }
        }
        public void CheckSignupAckRecivedOrNot()
        {
            if (isSignupRecived)
                return;
            else
            {
                Debug.Log(" Close And reConnect Socket SingUp Ack Not Recived in " + singUpRetryTime);
                ReconnectOnSlowInternet();
            }
        }
        #endregion


        #region Forcefully Close Socket
        internal void ForcefullyCloseSocket()
        {
            Debug.Log("<color=red> ForcefullyCloseSocket </color>");
            CallBreak_GS.Inst.isSocketClosed = true;
            socketState = SocketState.Disconnect;
            if (manager != null)
                manager.Socket.Disconnect();
            CallBreak_UIManager.Inst.CloseAllUserTurnTimer();

        }
        #endregion

        //new send data method
        public void SendData(JSONObject jsonData, Action<string> onComplete, CallBreak_CustomEvents eventName)
        {
            string jsonDataToString = jsonData.ToString();
            if (eventName != CallBreak_CustomEvents.HEART_BEAT)
                Debug.Log("<color=red>Send Data To Server :- " + jsonDataToString + "</color>" + "<color=green>, event Name: " + eventName + "</color>");

            try
            {
                manager.Socket.ExpectAcknowledgement<string>(onComplete).Volatile().Emit(eventName.ToString(), jsonDataToString.ToString());

            }
            catch (Exception ex)
            {
                Debug.LogError("Exception SendData Methods -> " + ex.ToString());
            }
        }

        private void OnDestroy()
        {
            if (manager != null)
                manager.Socket.Disconnect();
        }

        #region OnApplicationPause
        private void OnApplicationPause(bool pause)
        {
            Debug.Log(" OnApplicationPause => " + pause);
            if (MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE)
                return;
            if (pause)
            {
                GC.Collect();
                Resources.UnloadUnusedAssets();

                Debug.Log(" Application Run In BackGround ");

                ResetAllGameDataCloseSocket();
            }
            else
            {
                Debug.Log("  Application Come In ForGround  Socket State " + socketState);
                isFirstBackground = true;
                StartConnection();
            }
        }
        #endregion




        #region Reconnect Game On Slow InternetConnection
        internal void ReconnectOnSlowInternet()
        {
            Debug.Log(" CallBreak_SocketConnection || Reconnection Due to Slow Internet  ");
            ResetAllGameDataCloseSocket();
            isFirstBackground = true;
            StartConnection();

        }
        #endregion



        #region ACKNOWLEDGEMENT_CALLBACKS

        internal void SignUpAcknowledgement(string signUpAcknowlegementData)
        {
            Debug.Log(" SignUp Acknowledgement Payload " + signUpAcknowlegementData);
            isSignupRecived = true;
            CallBreak_GS.Inst.fromBack = true;
            JSONObject PayloadObject = new JSONObject();
            try
            {
                PayloadObject = new JSONObject(signUpAcknowlegementData.ToString());
                CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception || EX " + ex.ToString());
            }
            JSONObject data = PayloadObject.GetField("data");

            if (data.HasField("success") && !bool.Parse(data.GetField("success").ToString()))
            {
                Debug.LogError("SignUpAcknowledgement success false");

                string errorMessage = data.GetField("error").GetField("errorMessage").ToString();
                int errrCode = int.Parse(data.GetField("error").GetField("errorCode").ToString());

                if (errrCode == 500)
                {
                    string en = PayloadObject.GetField("en").ToString().Trim('"');
                    CallBreak_AllPopupHandler.Instance.OpenOfflineCommonPopup("Retry");
                    CallBreak_UIManager.Inst.CloseAllUserTurnTimer();
                }
            }
            else
            {
                Debug.Log("SignUpAcknowledgement success");
                CallBreak_SocketEventReceiver.Inst.ReceiveDataFromServerEnd(signUpAcknowlegementData);
            }
        }

        internal void FTUEMsgAcknowledgement(string ackData)
        {
            Debug.Log("FTUEMsgAcknowledgement");
            JSONObject PayloadObject = new JSONObject(ackData.ToString());
            Debug.Log(PayloadObject.ToString());

        }
        internal void RejoinAcknowledgement(string ackData)
        {
            Debug.Log("Rejoin || ackData : " + ackData);
            JSONObject PayloadObject = new JSONObject(ackData.ToString());
            JSONObject rejoinData = null;
            if (PayloadObject.HasField("data"))
            {
                rejoinData = PayloadObject.GetField("data").GetField("REJOIN_TABLE_INFO");
                bool isRejoin = bool.Parse(rejoinData.GetField("isRejoin").ToString().Trim('"'));
                if (isRejoin)
                {
                    CallBreak_SocketEventReceiver.Inst.ReceiveDataFromServerEnd(ackData);
                }
            }
        }
        internal void HelpAcknowledgement(string ackData)
        {
            Debug.Log(" HELP || HelpAcknowledgement : " + ackData);
            JSONObject PayloadObject = new JSONObject(ackData.ToString());
            if (PayloadObject.HasField("data"))
            {
                Debug.Log(" Data recived From Help " + PayloadObject);
                CallBreak_SocketEventReceiver.Inst.ReceiveDataFromServerEnd(ackData);



            }
        }

        internal void HeartBeatAcknowledgement(string ackData)
        {
            socketState = SocketState.Running;
            CallBreak_InternetHandler.Inst.OnReceiveHB();
        }

        #endregion


        #region CUSTOM_EVENTS
        public void RegisterCustomEvents()
        {
            var events = Enum.GetValues(typeof(CallBreak_CustomEvents)) as CallBreak_CustomEvents[];
            for (int i = 0; i < events.Length; i++)
            {
                string en = events[i].ToString();

                manager.Socket.On<Socket, CallBreakResponseData>(en, (socket, res) =>
                {
                    var data = res.data;

                    if (data == null) return;

                    socketState = SocketState.Running;

                    JSONObject receiveJsonObject = new JSONObject(data.ToString());

                    JSONObject decryptedJsonObject = new JSONObject(receiveJsonObject.ToString().Trim('"'));

                    CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);

                    if (decryptedJsonObject != null) //send data to our switch case
                    {
                        Debug.Log("<color=yellow> Data Recived From Server  </color>" + data);
                        CallBreak_SocketEventReceiver.Inst.ReceiveDataFromServerEnd(data.ToString());
                    }
                });
            }
        }
        #endregion


        #region check is Internet Available or not
        public bool IsInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        #endregion

        #region Overrided Socket Options
        private SocketOptions Overrided_SocketOptions()
        {
            SocketOptions socketOptions = new SocketOptions();
            socketOptions.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;
            socketOptions.Reconnection = true;
            socketOptions.ReconnectionAttempts = int.MaxValue;
            socketOptions.ReconnectionDelay = TimeSpan.FromMilliseconds(1000);
            socketOptions.ReconnectionDelayMax = TimeSpan.FromMilliseconds(5000);
            socketOptions.RandomizationFactor = 0.5f;
            int socketTimeout = (20) * 1000;
            socketOptions.Timeout = TimeSpan.FromMilliseconds(socketTimeout);
            socketOptions.AutoConnect = true;
            socketOptions.QueryParamsOnlyForHandshake = true;
            Debug.Log("Auth Token--" + MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken);
            socketOptions.Auth = (manager, socket) => new { token = MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken };
            return socketOptions;
        }
        #endregion

        #region ResetAllGameData On Any Type OF Reconnection
        internal void ResetAllGameDataCloseSocket()
        {
            Debug.Log(" Reset Game Data Start ");

            ForcefullyCloseSocket();
            OnlyResetGame();

        }
        void OnlyResetGame()
        {
            CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = false;
            CallBreak_Scoreboard.Inst.Close_Score_Board(0f);
            CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(0, 0);
            CallBreak_CardDeal.Inst.discardedCardContainer.transform.DOScaleX(0, 0f);

            CallBreak_AllPopupHandler.Instance.StopRtstimer();
            CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
            CallBreak_AllPopupHandler.Instance.CloseBigTopMsgToast();

            if (CallBreak_GS.Inst.collectBootAnimation)
                CallBreak_CardDeal.Inst.KillBootValueAnimation();
            CallBreak_PlayerController.Inst.ResetAllUserDetailsWhenAppBackground();
            CancelInvoke(nameof(CheckSignupAckRecivedOrNot));


            Debug.Log(" Reset Game Data Complete ");

        }
        #endregion
        Coroutine checkSocketCorotine;
        internal void CheckSocketConnectedAlltime()
        {
            if (checkSocketCorotine != null)
                StopCoroutine(checkSocketCorotine);

            checkSocketCorotine = StartCoroutine(CheckSocket());
        }
        IEnumerator CheckSocket()
        {
            yield return new WaitForSeconds(firstCheckConnectionDelay);

        CheckAgain:
            if (socketState.Equals(SocketState.Disconnect) || socketState.Equals(SocketState.Error))
            {
                Debug.Log(" Socket Connection Start From || CheckSocketConnectedAlltime ");
                StartConnection();
            }
            yield return new WaitForSeconds(1f);
            if (checkSocketCorotine != null)
                goto CheckAgain;
        }


    }

    #region ServerTypes
    public enum ServerType
    {
        Live = 0,
        Dev = 1,
        Staging = 2,
        LocalKishan = 3,
        LocalVaibhav = 4
    }
    #endregion

    #region SocketType
    public enum SocketState
    {
        None,
        Close,
        Connect,
        Open,
        Running,
        Error,
        Disconnect,
        Connecting
    }
    #endregion

    #region Response Data
    [System.Serializable]
    public class CallBreakResponseData
    {
        public string data;

        public CallBreakResponseData()
        {
            data = "";
        }
        public override string ToString()
        {
            return data;
        }
    }
    #endregion

    #region Custom Error Class
    [System.Serializable]
    public class CallBreakCustomError : Error
    {
        public CallBreakErrorData data;
        public override string ToString()
        {
            return $"[CustomError {message}, {data?.code}, {data?.content}]";
        }
    }

    public class CallBreakErrorData
    {
        public int code;
        public string content;
    }
    #endregion
}
