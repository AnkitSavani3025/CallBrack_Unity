using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using CallBreak_Socketmanager;
using System;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_InternetHandler : MonoBehaviour
    {
        #region Variables
        public static CallBreak_InternetHandler Inst;
        [Tooltip("flag for if socket disconnected due to no internet")]
        internal bool isInternetDisconnected = false;
        [Tooltip("flag if no internet popup is already open or not")]
        public static bool isInternetPopupOpened;
        public static bool isPongReceived = false;
        public bool checkInternetWithAPI, checkInternetWithHeartBeat, needToCheckInternet;

        [SerializeField] internal Image networkIcone;
        [Tooltip(" NetWork IndiCator Sprite ")]
        [SerializeField] internal Sprite goodSprite, normal1Sprite, normal2Sprite, lowSprite, badSprite, noInternetSprite;
        [SerializeField] string pingUrlForInternet = "https://ping.canbeuseful.com/ping.txt?uid=jksdf";


        int badDelayCounter, errorCounter;

        public long pingTime, requestTime, timeDelay, pongTime;

        public int PING_INTERVAL = 1, firstRequestDelay = 2;
        public int totalErrorCounter = 6, totalbadDelayCounter = 6;


        private Coroutine pingPongWithAPICorotine, pingPongWithHeartBeatCorotine;
        public List<int> PongTimer;

        List<long> timeDelayList = new List<long>();

        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            Debug.Log(" Internet Connection Ping Pong Url " + pingUrlForInternet);
            Inst = this;
        }
        #endregion

        #region Show No Internet Alert Popup
        public void Show_Nointernetpopup()
        {
            needToCheckInternet = false;
            if (!isInternetPopupOpened)
            {
                Debug.LogError(" NO INTERNET ");
                badDelayCounter = errorCounter = 0;
                networkIcone.sprite = noInternetSprite;
                StopCheckInternet();
                CallBreak_SocketConnection.intance.ForcefullyCloseSocket();
                isInternetPopupOpened = true;
                isInternetDisconnected = true;
                CallBreak_Scoreboard.Inst.Close_Score_Board(0);
                CallBreak_AllPopupHandler.Instance.OpenOfflineCommonPopup("NoInternet");
                CallBreak_SocketConnection.intance.ResetAllGameDataCloseSocket();
                CallBreak_SocketConnection.intance.CheckSocketConnectedAlltime();
            }
        }
        #endregion

        #region Hide No Internet Alert Popup
        public void Close_Nointernetpopup()
        {
            needToCheckInternet = true;
            Debug.Log("Close No Internet Popup");
            isInternetPopupOpened = false;
            isInternetDisconnected = false;
            CallBreak_AllPopupHandler.Instance.CloseOfflineCommomPopup();
        }
        #endregion
        #region check is Internet Available or not
        bool IsInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        #endregion



        public void CheckInternetWithPINGPONG()
        {
            badDelayCounter = errorCounter = 0;
            needToCheckInternet = true;
            StopCheckInternet();

            if (checkInternetWithAPI)
                pingPongWithAPICorotine = StartCoroutine(GetRequestWithAPI(pingUrlForInternet));
            else if (checkInternetWithHeartBeat)
                pingPongWithHeartBeatCorotine = StartCoroutine(GetRequestWithHeartBeat());
            else
                Debug.LogError(" Please Select Any METHOD for Internet Checking");

        }
        public void StopCheckInternet()
        {
            if (pingPongWithAPICorotine != null)
                StopCoroutine(pingPongWithAPICorotine);
            if (pingPongWithHeartBeatCorotine != null)
                StopCoroutine(pingPongWithHeartBeatCorotine);
        }

        IEnumerator GetRequestWithAPI(string uri)
        {
            yield return new WaitForSecondsRealtime(firstRequestDelay);
        SB:
            if (needToCheckInternet)
            {
                if (IsInternetConnection())
                {
                    if (badDelayCounter > totalbadDelayCounter || errorCounter > totalErrorCounter)
                    {
                        Debug.Log(" Auto Reconnect badDelayCounter || " + badDelayCounter + "|| errorCounter  " + errorCounter);
                        StopCheckInternet();
                        CallBreak_SocketConnection.intance.ReconnectOnSlowInternet();
                    }
                    else
                    {
                        requestTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

                        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                        {
                            webRequest.timeout = PING_INTERVAL;

                            yield return webRequest.SendWebRequest();

                            string[] pages = uri.Split('/');
                            int page = pages.Length - 1;

                            switch (webRequest.result)
                            {
                                case UnityWebRequest.Result.ConnectionError:
                                    Debug.LogError(": Connection Error Error : " + webRequest.error);
                                    errorCounter++;
                                    SetNetWorkIndiCatorOnError(errorCounter);
                                    Debug.Log(" <color=RED> NO Internt Connection 1  </color> Time Delay  XXXX  ");
                                    break;
                                case UnityWebRequest.Result.DataProcessingError:
                                    Debug.LogError(": DataProcessingError: " + webRequest.error);
                                    Debug.Log(" <color=RED> NO Internt Connection  2 </color> Time Delay  XXXX  ");
                                    break;
                                case UnityWebRequest.Result.ProtocolError:
                                    Debug.LogError(": HTTP Error: " + webRequest.error);
                                    errorCounter++;
                                    break;
                                case UnityWebRequest.Result.Success:

                                    errorCounter = (errorCounter > 3) ? errorCounter-- : 0;
                                    timeDelay = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - requestTime;
                                    timeDelayList.Add(timeDelay);
                                    break;
                            }
                            InternetstatusShow();
                        }
                        yield return new WaitForSecondsRealtime(PING_INTERVAL);

                        goto SB;
                    }
                }
                else
                    Show_Nointernetpopup();
            }
            else
            {
                Debug.LogError(" NO Need to Check Connection || needToCheckInternet => " + needToCheckInternet);
            }

        }







        #region Check Ping and TimeOut
        IEnumerator GetRequestWithHeartBeat()
        {
            yield return new WaitForSecondsRealtime(firstRequestDelay);
        SB:
            if (needToCheckInternet)
            {
                //Debug.Log(" Start Check ping Pong ");

                if (IsInternetConnection())
                {
                    SendPing();

                    yield return new WaitForSecondsRealtime(PING_INTERVAL);
                    if (badDelayCounter > totalbadDelayCounter || errorCounter > totalErrorCounter)
                    {
                        Debug.Log(" Auto Reconnect badDelayCounter || " + badDelayCounter + "|| errorCounter  " + errorCounter);
                        StopCheckInternet();
                        CallBreak_SocketConnection.intance.ReconnectOnSlowInternet();
                    }
                    else
                    {
                        if (isPongReceived)
                        {
                            errorCounter = (errorCounter > 3) ? errorCounter-- : 0;
                        }
                        else
                        {
                            errorCounter++;
                            Debug.Log("<Color=red>Ping server call missing:: </Color>" + errorCounter + "SocketState:: " + CallBreak_SocketConnection.intance.socketState + " IsInternet Available: " + IsInternetConnection());
                            SetNetWorkIndiCatorOnError(errorCounter);
                        }
                        goto SB;
                    }
                }
                else
                {
                    Show_Nointernetpopup();
                }
            }
            else
            {
                Debug.LogError(" NO Need to Check Connection || needToCheckInternet => " + needToCheckInternet);
            }

        }
        #endregion


        internal void SendPing()
        {
            pingTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            isPongReceived = false;
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.HEART_BEAT(), CallBreak_SocketConnection.intance.HeartBeatAcknowledgement, CallBreak_CustomEvents.HEART_BEAT);
        }

        #region Receive HEART_BEAT
        internal void OnReceiveHB()
        {
            isPongReceived = true;
            pongTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            timeDelay = pongTime - pingTime;
            timeDelayList.Add(timeDelay);
            InternetstatusShow();
        }
        #endregion


        void InternetstatusShow()
        {
            int delayListCount = timeDelayList.Count;

            if (delayListCount == 0 || errorCounter > 0 && errorCounter < 5) return;
            if (delayListCount > 2) timeDelayList.RemoveAt(0);

            SetInternetStatus(timeDelayList[timeDelayList.Count - 1]);
        }




        void SetInternetStatus(long Timeduration)
        {
            //Debug.Log(" Latency TIME " + Timeduration);
            if (Timeduration >= PongTimer[0] && Timeduration < PongTimer[1])
            {
                networkIcone.sprite = goodSprite;
                badDelayCounter = 0;
                //Debug.Log(" Fast Network ");
            }
            else if (Timeduration >= PongTimer[1] && Timeduration < PongTimer[2])
            {
                networkIcone.sprite = normal1Sprite;
                badDelayCounter = (badDelayCounter > 0) ? errorCounter-- : 0;
                //Debug.Log(" Avarage Network ");
            }
            else if (Timeduration >= PongTimer[2] && Timeduration < PongTimer[3])
            {
                networkIcone.sprite = normal2Sprite;
                badDelayCounter = (badDelayCounter > 0) ? errorCounter-- : 0;

                Debug.Log(" very Avarage Network ");
            }
            else if (Timeduration >= PongTimer[3] && Timeduration < PongTimer[4])
            {
                networkIcone.sprite = lowSprite;
                badDelayCounter++;
                Debug.Log("  Low  Network ");
            }
            else if (Timeduration > PongTimer[4])
            {
                networkIcone.sprite = badSprite;
                badDelayCounter++;
                Debug.Log("  very Low  Network ");
            }
        }
        void SetNetWorkIndiCatorOnError(int _errorCounter)
        {
            switch (_errorCounter)
            {
                case 1:
                    networkIcone.sprite = normal1Sprite;
                    break;

                case 2:
                    networkIcone.sprite = normal2Sprite;
                    break;

                case 3:
                    networkIcone.sprite = badSprite;
                    break;

                case 4:
                    networkIcone.sprite = badSprite;
                    break;
                default:
                    Debug.Log(" Internet Error Counter " + _errorCounter);
                    break;

            }
        }
    }
}
