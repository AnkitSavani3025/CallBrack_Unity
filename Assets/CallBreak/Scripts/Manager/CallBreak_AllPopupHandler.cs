using System;
using CallBreak_Socketmanager;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_AllPopupHandler : MonoBehaviour
    {

        #region Variables  
        public static CallBreak_AllPopupHandler Instance;
        [Space]
        [Header(" ALL SERVER POPUP HANDLER ")]
        [Space]

        [Header("-----Server Driven ScoreBoard Toast Popup-----")]
        [SerializeField] private GameObject serverScoreBoardPopUp;
        [SerializeField] private GameObject serverScoreBoardPopUpBG;
        [SerializeField] private Text scoreBoardPopUpText;
        [Space]
        [Header("-----Server Driven Top Toast Popup-----")]
        [SerializeField] private GameObject serverTopToast;
        [SerializeField] private Text serverTopToastText;
        [Space]
        [Header("-----Server Driven Center Toast Popup-----")]
        [SerializeField] private GameObject serverCenterToast;
        [SerializeField] private Text serverCenterToastText;
        [Space]
        [Header("-----Server Driven Common Popup-----")]
        [SerializeField] private GameObject serverCommonPopup;
        [SerializeField] private GameObject serverCommonPopupContent;
        [SerializeField] private GameObject serverCommonPopupLoader;[Space]
        [Header("-----Server Driven Rejoin  Popup-----")]
        [SerializeField] private GameObject rejoinCommonPopup;
        [SerializeField] private GameObject rejoinCommonPopupContent;
        [SerializeField] private Text[] rejoinCommonPopupButtonTextList;
        [SerializeField] private Text rejoinCommonPopupMsgText;
        [SerializeField] private Text rejoinCommonPopupTitleText;

        [SerializeField] private Text serverCommonPopupTitle;
        [SerializeField] private Text serverCommonPopupMessage;
        [SerializeField] private Button[] serverCommonPopupButtons;
        [SerializeField] private Text[] serverCommonPopupButtonsText;
        [SerializeField] private Sprite GreenBtn, RedBtn;

        #endregion




        #region Unity callbacks
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        private void OnEnable()
        {
            Debug.Log(" ON ENEBLe ");
            CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;
        }

        private void OnDisable()
        {
            Debug.Log(" ON DISEBLE ");
            CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;

        }
        #endregion

        #region Game Reset
        void ResetAllAfterNewGame()
        {
            HideAllServerPopups();
        }
        #endregion

        #region Show Server Popup switch case
        public void Show_Server_Popup(CallBreak_Server_Popup dataobject)
        {
            string popupType = dataobject.data.popupType;
            switch (popupType)
            {
                case "topToastPopup":
                    ShowServer_TopToast(dataobject);
                    break;
                case "toastPopup":
                    ShowServer_CenterToast(dataobject);
                    break;
                case "commonPopup":
                    ShowServer_CommonPopup(dataobject);
                    break;
                case "scoreBoardPopup":
                    ShowScoreBoardPopup(dataobject);
                    break;

                default:
                    Debug.LogError("this popup type does not exist on unity.....");
                    break;
            }
        }
        #region Rejoin Common PopupHandler
        public void ShowRejoinToOldGamePopup(RejoinResponseHandler rejoinResponseHandler)
        {
            rejoinCommonPopup.SetActive(true);
            rejoinCommonPopupContent.transform.DOScale(0, 0);
            rejoinCommonPopupContent.transform.DOScale(1, 0.4f).SetEase(Ease.OutBounce);
            rejoinCommonPopupMsgText.text = rejoinResponseHandler.data.message;
            rejoinCommonPopupTitleText.text = rejoinResponseHandler.data.title;
            for (int i = 0; i < rejoinResponseHandler.data.button_text.Count; i++)
            {
                Debug.Log("rejoinResponseHandler.data data Button Text " + i + rejoinResponseHandler.data.button_text[i]);
                rejoinCommonPopupButtonTextList[i].text = rejoinResponseHandler.data.button_text[i];
            }
            Debug.Log(" Rejoin Common Popup Is Open Now ");
        }
        public void CloseRejoinToOldGamePopup()
        {
            rejoinCommonPopupContent.transform.DOScale(0, 0.2f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                rejoinCommonPopup.SetActive(false);
            });


        }
        #endregion
        #endregion

        #region Hide All Server popups
        public void HideAllServerPopups()
        {
            HideServer_TopToast();
            HideServer_CenterToast();
            HideServer_CommonPopup();
        }
        #endregion

        #region Show Top of screen Toast
        private void ShowServer_TopToast(CallBreak_Server_Popup popupData, bool isAutoHide = true)
        {
            Debug.LogError("-----------Show Server Top Toast-------------");

            serverTopToastText.text = popupData.data.message;
            RectTransform rect = serverTopToast.GetComponent<RectTransform>();
            rect.localScale = Vector2.one;

            HideServer_TopToast();

            serverTopToast.SetActive(true);

            if (isAutoHide)
            {
                rect.DOAnchorPos(new Vector2(0, -59f), 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    this.WaitforTime(6f, () =>
                    {
                        HideServer_TopToast();
                    });
                });
            }
            else
            {
                rect.DOAnchorPos(new Vector2(0, -59f), 0.3f).SetEase(Ease.OutBack);
            }
        }
        #endregion

        #region Hide top of screen toast
        public void HideServer_TopToast()
        {
            Debug.Log("Hide Server Header Toast");
            RectTransform topToastRect = serverTopToast.GetComponent<RectTransform>();
            topToastRect.DOAnchorPosY(50, 0.3f).OnComplete(() => serverTopToast.SetActive(false));

        }
        #endregion

        #region show Center toast
        private void ShowServer_CenterToast(CallBreak_Server_Popup popupData, bool isAutoHide = true)
        {
            Debug.Log("-----------Show Server Center Toast-------------");
            if (popupData.data.message.Contains("current turn is not your turn"))
                return;
            serverCenterToastText.text = popupData.data.message;
            HideServer_CenterToast();

            serverCenterToast.SetActive(true);
            serverCenterToast.transform.DOScale(0, 0);
            if (isAutoHide)
            {
                serverCenterToast.transform.DOScale(1, .4f).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    this.WaitforTime(3f, () =>
                    {
                        HideServer_CenterToast();
                    });
                });
            }
            else
            {
                serverCenterToast.transform.DOScale(1f, .4f).SetEase(Ease.OutElastic);
            }
        }
        #endregion
        #region show Center toast
        private void ShowScoreBoardPopup(CallBreak_Server_Popup popupData, bool isAutoHide = true)
        {
            Debug.LogError("-----------Show ScoreBoard Popup -------------");
            RectTransform rect = serverScoreBoardPopUp.GetComponent<RectTransform>();


            scoreBoardPopUpText.text = popupData.data.message;
            HideScoreBoardToast();
            serverScoreBoardPopUpBG.SetActive(true);


            if (isAutoHide)
            {
                rect.DOScale(1, .4f).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    this.WaitforTime(1f, () =>
                    {
                        HideScoreBoardToast();
                    });
                });
            }
            else
            {
                rect.DOScale(1f, .4f).SetEase(Ease.OutElastic);
            }
        }
        #endregion

        #region hide center toast 
        public void HideServer_CenterToast()
        {
            Debug.Log("Hide Server Center Toast");
            RectTransform toastRect = serverCenterToast.GetComponent<RectTransform>();
            serverCenterToast.transform.DOScale(0, .2f);
            serverCenterToast.SetActive(false);
        }
        #endregion
        #region hide ScoreBoard toast 
        public void HideScoreBoardToast()
        {
            Debug.Log("Hide Server Center Toast");
            //RectTransform toastRect = serverScoreBoardPopUp.GetComponent<RectTransform>();
            serverScoreBoardPopUp.transform.DOScale(0, .1f);
            serverScoreBoardPopUpBG.SetActive(false);
        }
        #endregion

        #region show server common popup
        internal void ShowServer_CommonPopup(CallBreak_Server_Popup popupData)
        {
            try
            {
                CallBreak_SocketConnection.intance.isSignupRecived = true;
                string title = popupData.data.title;
                string message = popupData.data.message;

                serverCommonPopupTitle.text = title;
                serverCommonPopupMessage.text = message;

                for (int i = 0; i < serverCommonPopupButtons.Length; i++)
                {
                    serverCommonPopupButtons[i].onClick.RemoveAllListeners();
                    serverCommonPopupButtons[i].gameObject.SetActive(false);
                }

                int buttonCount = popupData.data.buttonCounts;
                Debug.Log("buttonCount:" + popupData.data.buttonCounts);
                for (int i = 0; i < popupData.data.buttonCounts; i++)
                {
                    serverCommonPopupButtons[i].gameObject.SetActive(true);
                    serverCommonPopupButtonsText[i].text = popupData.data.button_text[i];
                }

                if (buttonCount == 1)
                {
                    SetButtonSprite(serverCommonPopupButtons[0].transform.GetComponent<Image>(), popupData.data.button_color == null ? "red" : popupData.data.button_color[0]);
                    serverCommonPopupButtons[0].onClick.AddListener(() => PopupButtonClick(popupData.data.button_methods[0]));
                }
                if (buttonCount == 2)
                {
                    SetButtonSprite(serverCommonPopupButtons[0].transform.GetComponent<Image>(), popupData.data.button_color == null ? "red" : popupData.data.button_color[0]);
                    serverCommonPopupButtons[0].onClick.AddListener(() => PopupButtonClick(popupData.data.button_methods[0]));
                    SetButtonSprite(serverCommonPopupButtons[1].transform.GetComponent<Image>(), popupData.data.button_color == null ? "green" : popupData.data.button_color[1]);
                    serverCommonPopupButtons[1].onClick.AddListener(() => PopupButtonClick(popupData.data.button_methods[1]));
                }

                serverCommonPopup.SetActive(true);
                offlineCommomPopup.SetActive(false);
                serverCommonPopupContent.transform.DOScale(0, 0);
                serverCommonPopupContent.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    serverCommonPopupLoader.SetActive(popupData.data.showLoader);
                });
            }
            catch (System.Exception e) { Debug.LogError(e.ToString()); }
        }
        #endregion

        #region server common popup button click method
        private void PopupButtonClick(string method)
        {
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            HideServer_CommonPopup();
            Invoke(method, 0);
        }
        #endregion

        #region hide server common popup
        public void HideServer_CommonPopup()
        {
            RectTransform contentRect = serverCommonPopupContent.GetComponent<RectTransform>();
            serverCommonPopupLoader.SetActive(false);
            contentRect.DOScale(0f, 0f).SetEase(Ease.InBack).OnComplete(() =>
            {
                for (int i = 0; i < serverCommonPopupButtons.Length; i++)
                {
                    serverCommonPopupButtons[i].onClick.RemoveAllListeners();
                    serverCommonPopupButtons[i].gameObject.SetActive(false);
                }
                serverCommonPopupTitle.text = string.Empty;
                serverCommonPopupMessage.text = string.Empty;
                serverCommonPopup.SetActive(false);
            });
        }
        #endregion

        void SetButtonSprite(Image Button, string buttonSpriteColor)
        {
            if (buttonSpriteColor == "green")
            {
                Button.sprite = GreenBtn;
            }
            else if (buttonSpriteColor == "red")
            {
                Button.sprite = RedBtn;
            }
        }


        [Space]
        [Header(" ALL TYPE SERVER TOAST POPUP ")]
        [Space]

        [SerializeField] GameObject TopMsgBG;
        [SerializeField] GameObject CenterMsgBG, TopMsgBGBig;
        #region open top msg box
        internal void OpenTopMsgToast(string msg)
        {
            TopMsgBG.SetActive(true);
            TopMsgBG.transform.GetChild(0).transform.GetComponent<Text>().text = msg;
            TopMsgBG.transform.DOScale(1, 0.3f);
            Invoke(nameof(CloseTopMsgToast), 2.5f);
        }
        #endregion

        #region close top msg box
        internal void CloseTopMsgToast()
        {
            TopMsgBG.transform.DOScale(0, 0.3f).OnComplete(() =>
            {
                TopMsgBG.SetActive(false);
            });
        }
        #endregion

        #region open center msg box
        internal void OpenCenterMsgToast(string msg)
        {
            StopRtstimer();
            CenterMsgBG.transform.DOScale(0, 0);
            Debug.Log("  OpenCenterMsgToast  MSG=> " + msg);
            CenterMsgBG.SetActive(true);
            CenterMsgBG.transform.DOScale(1, 0).SetEase(Ease.OutBounce);
            CenterMsgBG.transform.GetChild(0).transform.GetComponent<Text>().text = msg;
        }
        #endregion

        #region close center msg box
        internal void CloseCenterMsgToast()
        {
            Debug.Log(" Close Center Msg Toast ");
            CenterMsgBG.transform.DOScale(0, 0);
            CenterMsgBG.SetActive(false);
        }
        #endregion


        #region New Game Start Timer
        internal string rtsTimerString;
        internal int rtsTimer;
        internal void StartRTSTimer(int _RTS_waitTime, string msg)
        {
            Debug.Log("Open  RTS  timer and popup  Time= " + _RTS_waitTime + " Msg => " + msg);

            CancelInvoke(nameof(RTS_Timer));
            rtsTimer = _RTS_waitTime;
            rtsTimerString = msg;
            CenterMsgBG.transform.DOScale(0, 0);
            CenterMsgBG.SetActive(true);
            CenterMsgBG.transform.DOScale(1, 0.2f).SetEase(Ease.OutBounce);
            InvokeRepeating(nameof(RTS_Timer), 0, 1f);
        }
        internal void StopRtstimer()
        {
            Debug.Log("close rts timer and popup ");
            CancelInvoke(nameof(RTS_Timer));
            CloseCenterMsgToast();
            rtsTimerString = "";
            rtsTimer = 0;
        }
        void RTS_Timer()
        {
            if (rtsTimer < 1) //if wait time==0 then cancelinvoke this method and hide msg from table
            {
                StopRtstimer();
            }
            else
            {
                CenterMsgBG.transform.GetChild(0).transform.GetComponent<Text>().text = rtsTimerString + rtsTimer.ToString() + " SECONDS";
                rtsTimer--;
            }
            CenterMsgBG.SetActive(true);
            CenterMsgBG.transform.localScale = Vector3.one;
            Debug.Log(rtsTimerString + rtsTimer.ToString() + " SECONDS");

        }
        #endregion




        #region open big top msg box
        internal void OpenBigTopMsgToast(string msg)
        {
            Debug.Log(" OPEN  CENTER MSG BIG TOAST  " + msg);
            TopMsgBGBig.transform.DOScale(0, 0);
            TopMsgBGBig.SetActive(true);
            TopMsgBGBig.transform.GetChild(0).transform.GetComponent<Text>().text = msg;
            TopMsgBGBig.transform.DOScale(1, 0.3f).SetEase(Ease.OutBounce);
        }
        #endregion

        #region close top msg box big
        internal void CloseBigTopMsgToast()
        {
            Debug.Log(" CLOSE CENTER MSG BIG TOAST ");

            TopMsgBGBig.transform.DOScale(0, 0f);
            TopMsgBGBig.SetActive(false);
        }
        #endregion





        //==================================================================================================================================================================

        [Space]
        [Header(" ALL TYPE OF OFFLINE ALERT POPUP ")]
        [Space]

        public GameObject offlineCommomPopup, noInternetImage;
        public Button ExitButton, NewGameButton, EmptyButton, retrylogicExitBtn;
        public Text titleText, MsgText;
        public static char[] trim_char_arry = new char[] { '"' };

        #region Open Common popup by name

        void OpenNoInternetPopUp()
        {
            CallBreak_Scoreboard.Inst.Close_Score_Board(0);
            titleText.text = "Alert";
            MsgText.text = "Please check your internet connectivity and try again!";
            CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = false;
            noInternetImage.SetActive(true);

        }

        internal void OpenOfflineCommonPopup(string name, JSONObject data = null)
        {
            int sortingOrder = 8;
            try
            {
                string tital = "";
                string msg = "";
                Debug.Log("Open " + name + " Popup");
                ExitButton.gameObject.SetActive(false);
                NewGameButton.gameObject.SetActive(false);
                retrylogicExitBtn.gameObject.SetActive(false);
                noInternetImage.SetActive(false);

                EmptyButton.gameObject.SetActive(false);
                ExitButton.onClick.RemoveAllListeners();
                retrylogicExitBtn.onClick.RemoveAllListeners();
                NewGameButton.onClick.RemoveAllListeners();
                EmptyButton.onClick.RemoveAllListeners();
                if (data != null)
                {
                    if (data.GetField("data").HasField("tital"))
                        tital = data.GetField("data").GetField("tital").ToString().Trim(trim_char_arry);

                    if (data.GetField("data").HasField("msg"))
                        msg = data.GetField("data").GetField("msg").ToString().Trim(trim_char_arry);

                    if (data.GetField("data").HasField("reason"))
                        msg = data.GetField("data").GetField("reason").ToString().Trim(trim_char_arry);
                }
                switch (name)
                {
                    case "NoInternet":  //for No Internet Connection
                        sortingOrder = 20;
                        OpenNoInternetPopUp();
                        EmptyButton.onClick.AddListener(Reconnect);
                        break;
                    case "NoStableInternet":  //for No Stable Internet
                        Debug.Log("Open Internet not stable popup");
                        sortingOrder = 10;
                        OpenNoInternetPopUp();
                        CallBreak_InternetHandler.isInternetPopupOpened = true;
                        CallBreak_InternetHandler.Inst.isInternetDisconnected = true;
                        break;

                    case "DisconnectOnWaitingState":  //Disconnect on Player Wating State
                        sortingOrder = 10;
                        ExitButton.gameObject.SetActive(true);
                        titleText.text = tital;
                        MsgText.text = msg;
                        CallBreak_GS.Inst.canDisplayPreloader = false;
                        ExitButton.onClick.AddListener(ExitBtnClick);
                        break;

                    case "LeaveTable":  //Leave Table
                        sortingOrder = 10;
                        ExitButton.gameObject.SetActive(true);
                        titleText.text = tital;
                        MsgText.text = msg;
                        ExitButton.onClick.AddListener(ExitBtnClick);
                        break;

                    case "IAmBack":  //Back in to playing
                        sortingOrder = 8;
                        EmptyButton.gameObject.SetActive(true);
                        EmptyButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "I Am Back";
                        titleText.text = tital;
                        MsgText.text = msg;
                        EmptyButton.GetComponent<Image>().sprite = CallBreak_AssetsReferences.Inst.btn_Common_enable;
                        EmptyButton.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
                        CallBreak_GS.Inst.isSelfUserInactive = true;
                        EmptyButton.onClick.AddListener(BackToPlaying);
                        break;

                    case "SocketError":  //Socket Error
                        sortingOrder = 10;
                        EmptyButton.gameObject.SetActive(true);
                        EmptyButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OK";
                        titleText.text = "Alert";
                        MsgText.text = " There is some issue in server connection , please try after some time ";
                        EmptyButton.GetComponent<Image>().sprite = CallBreak_AssetsReferences.Inst.btn_Common_enable;
                        EmptyButton.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
                        EmptyButton.onClick.AddListener(CloseScocketErrorPopup);
                        break;
                    case "Retry":
                        sortingOrder = 10;
                        //NewGameButton.gameObject.SetActive(true);
                        //retrylogicExitBtn.gameObject.SetActive(true);
                        Debug.Log(" retry Popup Opened Now ");
                        titleText.text = "Alert";
                        MsgText.text = " Oops! Something went wrong with this table, please exit and rejoin the lobby ";
                        ExitButton.gameObject.SetActive(true);
                        ExitButton.onClick.AddListener(ExitBtnClick);
                        //CallBreak_SocketConnection.intance.ForcefullyCloseSocket();
                        //NewGameButton.onClick.AddListener(RetryClick);
                        //retrylogicExitBtn.onClick.AddListener(ExitBtnClick);
                        break;

                    default:
                        Debug.LogError(" This Type Popup Not Found in unity " + name);
                        break;
                }
            }
            catch (Exception e) { Debug.Log(e.ToString()); }
            CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);
            offlineCommomPopup.SetActive(true);
            offlineCommomPopup.GetComponent<Canvas>().sortingOrder = sortingOrder;
            Debug.Log("Common Popup is Opened Now");
        }
        #endregion

        #region No internet Reconnect method
        /// <summary>
        /// when internet available and no internet popup is open then user call this method by clicked on reconnect from No internet popup
        /// </summary>
        internal void Reconnect()
        {
            Debug.Log("Going for Socket Connection from Reconnect");
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            CallBreak_SocketConnection.intance.StartConnection();
        }
        internal void GameButton()
        {
            Debug.Log("Going for Socket Connection From New Game Button  ");
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            CallBreak_GS.Inst.CurrentTableState = CurrentTableState.none;
            CallBreak_GS.Inst.fromBack = false;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.gameData.isPlay = true;
            CallBreak_SocketConnection.intance.StartConnection();
        }
        #endregion


        internal void ExitBtnClick()
        {
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            ExitButton.interactable = false;
            MGPSDK.MGPGameManager.instance.OnClickQuite();
        }

        internal void BackToPlaying()
        {
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.BACK_IN_GAME_PLAYING()
            , BackInGamePlayingAcknowledgement, CallBreak_CustomEvents.BACK_IN_GAME_PLAYING);
        }

        internal void CloseOfflineCommomPopup()
        {
            offlineCommomPopup.SetActive(false);
        }
        internal void CloseScocketErrorPopup()
        {
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            EmptyButton.interactable = false;
            MGPSDK.MGPGameManager.instance.OnClickQuite();
        }

        internal void RetryClick()
        {
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            offlineCommomPopup.SetActive(false);
            CallBreak_SocketConnection.intance.StartConnection();
        }

        #region ACKNOWLEDGEMENT_CALLBACKS

        internal void BackInGamePlayingAcknowledgement(string ackData)
        {
            Debug.Log("CallBreak_PopupHandler-> BackInGamePlaying Acknowledged || ackData : " + ackData);
        }
        #endregion



        //========================================================================================================================================================================================================



        [Space(50)]
        [Header(" LEAVE TABLE POPUP == >")]
        [SerializeField] GameObject ExitPopup;
        [SerializeField] GameObject ExitPopupPanelBg;
        [SerializeField] Button ExitPopupYesButton;



        #region Open Exit popup
        internal void OpenExitPopup()
        {
            ExitPopupPanelBg.SetActive(true);
            ExitPopup.transform.DOScale(Vector2.zero, 0);
            ExitPopup.transform.DOScale(Vector2.one, 0.2f);
        }
        #endregion

        #region close Exit popup
        internal void CloseExitPopup()
        {
            ExitPopup.transform.DOScale(Vector2.zero, 0.1f).OnComplete(() =>
            {
                ExitPopupPanelBg.SetActive(false);
                ExitPopupYesButton.interactable = true;
            });
        }
        #endregion
    }
}