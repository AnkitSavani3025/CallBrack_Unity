using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThreePlusGamesCallBreak;
using UnityEngine;
using UnityEngine.UI;

namespace CallBreak_Socketmanager
{
    public class CallBreak_SocketEventReceiver : MonoBehaviour
    {
        #region Variables
        public static CallBreak_SocketEventReceiver Inst;

        public SignUpResponseHandler signUpResponseHandler = new SignUpResponseHandler();

        COLLECT_BOOT_VALUE getCollectBootvalue = new COLLECT_BOOT_VALUE();
        internal USER_BID_TURN_STARTED getUserBidTurnStarted = new USER_BID_TURN_STARTED();
        public SHOW_SCORE_BOARD getShowScoreBoard = new SHOW_SCORE_BOARD();
        SHOW_MY_CARDS getShowMyCards = new SHOW_MY_CARDS();
        public REJOIN getRejoinData = new REJOIN();
        public WINNER_DECLARE getRoundWinnerData = new WINNER_DECLARE();
        public TABLE_INFO GetTABLE_INFO = new TABLE_INFO();
        public CallBreak_Server_Popup getShowPopupData = new CallBreak_Server_Popup();
        public RejoinResponseHandler rejoinResponseHandler = new RejoinResponseHandler();
        public static char[] trim_char_arry = new char[] { '"' };
        internal int CardOrder = 0;
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            if (Inst == null)
                Inst = this;
            else
                Destroy(transform.gameObject);
            CallBreak_GS.Inst.CurrentTableState = CurrentTableState.none;

        }
        #endregion

        #region Receive Data From Server End

        internal void ReceiveDataFromServerEnd(string receiveJSONOstring)
        {

            string en = "";
            JSONObject data = new JSONObject(receiveJSONOstring);

            if (data.HasField("en"))
            {
                en = data.GetField("en").ToString().Trim('"');
            }
            else
            {
                Debug.LogError(" Event Name not Found in json string => " + data);
                return;
            }


            switch (en)
            {
                case "DONE":
                    break;

                case "HEART_BEAT":
                    Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || HEART_BEAT Event </color>" + data);
                    CallBreak_InternetHandler.isPongReceived = true;
                    CallBreak_SocketConnection.intance.socketState = SocketState.Running;
                    break;

                case "SIGNUP":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || SIGNUP Event </color>" + data);
                        SetSignUpdata(data);

                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "JOIN_TABLE":
                    try
                    {

                        Debug.Log("<color=Green>Class :- SocketEventReceiver || Method : ReceiveData -> || JOIN_TABLE Event </color>" + data);
                        SetJoinTableData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "ROUND_TIMER_STARTED":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || ROUND_TIMER_STARTED Event </color>" + data);
                        SetRoundTimerStartData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "COLLECT_BOOT_VALUE":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || COLLECT_BOOT_VALUE Event </color>" + data);
                        SetCollectBootvalue(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "SHOW_MY_CARDS":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || SHOW_MY_CARDS Event </color>" + data);
                        SetShowMyCardsData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "USER_BID_TURN_STARTED":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || USER_BID_TURN_STARTED Event </color>" + data);

                        //CallBreak_GS.Inst.HideReconnectionLoader = true;
                        SetUserBidTurnStartedData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "USER_BID_SHOW":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || USER_BID_SHOW Event </color>" + data);

                        //CallBreak_GS.Inst.HideReconnectionLoader = true;
                        SetUserBidData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "USER_TURN_STARTED":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || USER_TURN_STARTED Event </color>" + data);

                        //CallBreak_GS.Inst.HideReconnectionLoader = true;
                        SetUserTurnStartedData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "USER_THROW_CARD_SHOW":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || USER_THROW_CARD_SHOW Event </color>" + data);
                        //CallBreak_GS.Inst.HideReconnectionLoader = true;
                        CardOrder++;
                        SetUserThrowCardShowData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "WIN_OF_ROUND":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || WIN_OF_ROUND Event </color>" + data);
                        CardOrder = 0;
                        SetUserHandCollectData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "WINNER_DECLARE":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || WINNER_DECLARE Event </color>" + data);
                        //CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(0, 0f); 
                        SetRoundWinnerData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "SHOW_SCORE_BOARD":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || SHOW_SCORE_BOARD Event </color>" + data);
                        SetShowScoreBoardData(data);   //data
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;
                case "LEAVE_TABLE":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || LEAVE_TABLE Event </color>" + data);
                        SetLeaveTableData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "TIME_OUT_LEAVE_TABLE_POPUP":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || TIME_OUT_LEAVE_TABLE_POPUP Event </color>" + data);
                        SetLeaveTableTimeOutData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "REJOIN":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || REJOIN Event </color>" + data);
                        SetReJoinData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "LOCK_IN_PERIOD":
                    try
                    {
                        Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || LOCK_IN_PERIOD Event </color>" + data);
                        SetLockINPeriodData(data);
                    }
                    catch (Exception ex) { Debug.Log(ex.ToString()); }
                    break;

                case "BACK_IN_GAME_PLAYING":
                    Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || BACK_IN_GAME_PLAYING Event </color>" + data);
                    SetBackInGamePlayingData(data);
                    break;

                case "SHOW_POPUP":
                    Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || SHOW_POPUP Event </color>" + data);
                    SHOW_POPUP(data);
                    break;

                case "GTI_INFO":
                    Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || GTI_INFO Event </color>" + data);
                    SetTableInfoData(data);
                    break;
                case "HELP":
                    CallBreak_UIManager.Inst.ShowHelpInfoData();
                    break;
                case "REJOIN_POPUP":
                    Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || REJOIN_POPUP Event </color>" + data);
                    SetRejoinPopupData(data);
                    break;
                default:
                    Debug.LogError("  THIS Type Event Not Define in Your Game " + data);
                    break;
            }

        }

        #endregion




        /// <summary>
        /// Game Table Info method use for get all player data available in table and seat and set player details on table
        /// That Game Table Data will Be GetWith Signup Ack
        /// </summary>
        /// <param name="data"></param>

        #region Sign Up data set method
        void SetSignUpdata(JSONObject data)
        {
            try
            {
                //Get SignUp data, Deserialize data and set to SignUp class

                signUpResponseHandler = GetDeserializeData<SignUpResponseHandler>(data);
                PlayerPrefs.SetString("UID", signUpResponseHandler.data.SIGNUP.userId);
                PlayerPrefs.SetString("UN", signUpResponseHandler.data.SIGNUP.username);
                CallBreak_GS.Inst.userWalletBalance = signUpResponseHandler.data.SIGNUP.balance;
                CallBreak_GS.Inst.userinfo.ID = signUpResponseHandler.data.SIGNUP.userId;
                Debug.Log("<color=Red>Self Player UserId in SignUp:" + CallBreak_GS.Inst.userinfo.ID + "</color>");


                if (signUpResponseHandler.data.GAME_TABLE_INFO.isRejoin)
                {
                    CallBreak_GS.Inst.isRejoinOrNot = true;

                    string json = JsonUtility.ToJson(signUpResponseHandler.data.GAME_TABLE_INFO);
                    CallBreak_GS.Inst.CurrentTableState = CurrentTableState.GameStarted;

                    SetReJoinData(data.GetField("data").GetField("GAME_TABLE_INFO"));
                }
                else
                {
                    CallBreak_GS.Inst.CurrentTableState = CurrentTableState.Waiting;

                    CallBreak_GS.Inst.isRejoinOrNot = false;

                    Debug.Log(" Fresh User join to table ");
                    CallBreak_GS.Inst.roundTableId = signUpResponseHandler.data.GAME_TABLE_INFO.roundTableId;
                    CallBreak_GS.Inst.gameinfo.tabelId = signUpResponseHandler.data.GAME_TABLE_INFO.tableId;
                    Debug.Log("Table Id::" + CallBreak_GS.Inst.gameinfo.tabelId);
                    CallBreak_GS.Inst.userinfo.Current_Table_ID = CallBreak_GS.Inst.gameinfo.tabelId;
                    CallBreak_GS.Inst.MaxPlayer = int.Parse(signUpResponseHandler.data.GAME_TABLE_INFO.noOfPlayer);
                    CallBreak_GS.Inst.ActivePlayer = signUpResponseHandler.data.GAME_TABLE_INFO.totalPlayers;
                    CallBreak_GS.Inst.userinfo.Player_Seat = signUpResponseHandler.data.GAME_TABLE_INFO.seatIndex;
                    PlayerPrefs.SetInt("CallBreakUserSeatIndex", signUpResponseHandler.data.GAME_TABLE_INFO.seatIndex);
                    CallBreak_GS.Inst.gameinfo.potValue = signUpResponseHandler.data.GAME_TABLE_INFO.potValue;
                    CallBreak_GS.Inst.userTurnTimer = signUpResponseHandler.data.GAME_TABLE_INFO.userTurnTimer;
                    CallBreak_GS.Inst.FinalPointsAmountNegetive = signUpResponseHandler.data.GAME_TABLE_INFO.winningScores[0];  //get and set negetive and positive win amount
                    CallBreak_GS.Inst.FinalPointsAmountPositive = signUpResponseHandler.data.GAME_TABLE_INFO.winningScores[1];
                    Debug.Log(" Total seats in Table" + signUpResponseHandler.data.GAME_TABLE_INFO.seats.Count);

                    CallBreak_GS.Inst.gameinfo.tabelId = signUpResponseHandler.data.GAME_TABLE_INFO.tableId;
                    CallBreak_GS.Inst.bootData.bootValue = float.Parse(signUpResponseHandler.data.GAME_TABLE_INFO.bootValue);
                    CallBreak_GS.Inst.MaxRound = signUpResponseHandler.data.GAME_TABLE_INFO.totalRound;
                    CallBreak_GS.Inst.gameinfo.WinAmount = float.Parse(signUpResponseHandler.data.GAME_TABLE_INFO.winnningAmonut);

                    CallBreak_UIManager.Inst.SetPlayingHeader(signUpResponseHandler.data.GAME_TABLE_INFO.currentRound);
                    CallBreak_GameManager.Inst.RemovePlayerToPlayerList(CallBreak_GS.Inst.MaxPlayer);
                    CallBreak_GameManager.Inst.StopTimerAndMakePlayerINWatingState(CallBreak_GS.Inst.MaxPlayer);

                    for (int i = 0; i < signUpResponseHandler.data.GAME_TABLE_INFO.seats.Count; i++)
                    {
                        if (signUpResponseHandler.data.GAME_TABLE_INFO.seats[i].userId != null)
                        {
                            if (signUpResponseHandler.data.GAME_TABLE_INFO.seats[i].userId == CallBreak_GS.Inst.userinfo.ID)
                            {
                                CallBreak_GameManager.Inst.setseatindex(signUpResponseHandler.data.GAME_TABLE_INFO.seats[i].seatIndex);
                            }
                        }
                    }
                    CallBreak_PlayerController.Inst.SitUserOnExistingTable();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        #endregion

        #region set player data when any join table
        /// <summary>
        /// Join Table method for player join data and seat that user on table
        /// </summary>
        /// <param name="data"></param>
        void SetJoinTableData(JSONObject data)
        {

            JOIN_TABLE getJoinTable = new JOIN_TABLE();
            getJoinTable = GetDeserializeData<JOIN_TABLE>(data);
            CallBreak_GS.Inst.ActivePlayer = getJoinTable.data.totalPlayers;
            Debug.Log("Join Table Data Set::" + CallBreak_GS.Inst.ActivePlayer);
            Debug.Log("Joined Player Details::" + getJoinTable.data.playarDetail);
            CallBreak_PlayerController.Inst.SetAndSitPlayerOnJoinTable(getJoinTable.data.playarDetail);
        }
        #endregion
        #region set round timer stared data
        /// <summary>
        /// Round Timer Started method for Start Game Start Timer display to user on table
        /// </summary>
        /// <param name="data"></param>
        void SetRoundTimerStartData(JSONObject data)
        {
            ROUND_TIMER_STARTED getRoundTimerStarted = new ROUND_TIMER_STARTED();
            getRoundTimerStarted = GetDeserializeData<ROUND_TIMER_STARTED>(data);
            CallBreak_GS.Inst.CurrentTableState = CurrentTableState.GameStarted;


            CallBreak_GS.Inst.isGameEnded = false;

            if (CallBreak_GS.Inst.ActivePlayer == CallBreak_GS.Inst.MaxPlayer)
                CallBreak_AllPopupHandler.Instance.StartRTSTimer(getRoundTimerStarted.data.timer, " New Round Start in ");
        }

        #endregion

        #region set boot collect value data and play animation
        /// <summary>
        /// Boot Amount Collect and Display Animation for All User 
        /// animation take place on user profile
        /// </summary>
        /// <param name="data"></param>
        void SetCollectBootvalue(JSONObject data)
        {
            getCollectBootvalue = GetDeserializeData<COLLECT_BOOT_VALUE>(data);
            for (int i = 0; i < getCollectBootvalue.data.balance.Count; i++)
            {
                if (getCollectBootvalue.data.balance[i].userId == PlayerPrefs.GetString("UID"))
                {
                    CallBreak_GS.Inst.userWalletBalance = getCollectBootvalue.data.balance[i].balance;
                    CallBreak_UIManager.Inst.walletAmount.text = " ₹ " + getCollectBootvalue.data.balance[i].balance.ToString();
                }
            }
            CallBreak_GS.Inst.bootData.bootValue = float.Parse(getCollectBootvalue.data.bootValue);
            CallBreak_GS.Inst.isBootCollected = true;
            CallBreak_CardDeal.Inst.BootCollectAnimation();
        }
        #endregion

        #region set my cards data and play card deal animation
        /// <summary>
        /// Stop ongoing game start timer
        /// call method for Reset game play like all flag, variables, card list, other list
        /// get self user hand cards and set card details
        /// play card deal animation
        /// </summary>
        /// <param name="data"></param>
        void SetShowMyCardsData(JSONObject data)
        {
            GC.Collect();
            Resources.UnloadUnusedAssets(); // USE FOR GARBAGE COLLECTER WHEN START NEW ROUND 

            getShowMyCards = GetDeserializeData<SHOW_MY_CARDS>(data);

            CallBreak_UIManager.Inst.SideMenuBtn.interactable = true;
            CallBreak_UIManager.Inst.leaveTableButton.interactable = true;

            CallBreak_UIManager.Inst.SideMenuBtn.GetComponent<Image>().sprite = CallBreak_UIManager.Inst.EnableSideMenuBtn;

            CallBreak_AllPopupHandler.Instance.StopRtstimer();
            CallBreak_AllPopupHandler.Instance.CloseExitPopup();
            CallBreak_Scoreboard.Inst.Close_Score_Board(0f);
            CallBreak_AllPopupHandler.Instance.CloseBigTopMsgToast();

            CallBreak_CardDeal.Inst.DealAnimEnumeator = CallBreak_CardDeal.Inst.StartCardDeal(getShowMyCards.data.dealer);

            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].DealerIcon.transform.DOScale(0, 0);
            }
            CallBreak_Users P = CallBreak_GameManager.Inst.GetPlayerSeatIndex(getShowMyCards.data.dealer);
            P.DealerIcon.transform.DOScale(1, 0);
            Debug.Log("Dealer Icon set::" + P.gameObject.name);
            Debug.Log(" GO FOR CARD ANIMATION AND GENERATION");
            StartCoroutine(CallBreak_CardDeal.Inst.DealAnimEnumeator);
            CallBreak_GS.Inst.CurrentRound = getShowMyCards.data.currentRound;
            CallBreak_UIManager.Inst.SetPlayingHeader(getShowMyCards.data.currentRound);
        }

        internal void SetUserHandCards()
        {
            if (CallBreak_GS.Inst.CurrentRound > 1)
            {
                CallBreak_UIManager.Inst.GamePlayReset(false);
            }

            CallBreak_PlayerController.Inst._myCardList = new List<CallBreak_MyCard>();
            CallBreak_PlayerController.Inst._myCardList.Clear();

            if (CallBreak_UIManager.Inst.MyCardsParent.transform.childCount == 0)
            {
                CallBreak_UIManager.Inst.GamePlayReset(false);
            }

            for (int i = 0; i < 13; i++)
            {
                CallBreak_PlayerController.Inst._myCardList.Add(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(i).GetComponent<CallBreak_MyCard>());
            }
            Debug.Log("Show My Cards _myCardList.count::" + CallBreak_PlayerController.Inst._myCardList.Count);
            for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
            {
                try
                {
                    string nm = getShowMyCards.data.cards[i];
                    List<Sprite> temp = new List<Sprite>();
                    temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == nm);
                    CallBreak_PlayerController.Inst._myCardList[i].GetComponent<Image>().sprite = temp[0];
                    CallBreak_PlayerController.Inst._myCardList[i].cardName = getShowMyCards.data.cards[i];
                    CallBreak_PlayerController.Inst._myCardList[i].cardValue = int.Parse(getShowMyCards.data.cards[i].Substring(2));

                    if (CallBreak_PlayerController.Inst._myCardList[i].cardValue == 1)
                        CallBreak_PlayerController.Inst._myCardList[i].cardValue = 14;

                    CallBreak_PlayerController.Inst._myCardList[i].type = getShowMyCards.data.cards[i].Substring(0, 1);
                    CallBreak_PlayerController.Inst._myCardList[i].gameObject.name = getShowMyCards.data.cards[i];
                }
                catch (Exception e) { Debug.LogError("error in card sprite set::" + e.ToString()); }
            }
            for (int j = 13; j < CallBreak_UIManager.Inst.MyCardsParent.transform.childCount; j++)
            {
                CallBreak_PlayerController.Inst._myCardList.Remove(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(j).GetComponent<CallBreak_MyCard>());
                Destroy(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(j).gameObject);
                Debug.Log("<color=yellow>Remove Extra Card::</color>");
            }
        }

        #endregion


        #region set User bid turn started data
        /// <summary>
        /// User Bid Turn Started 
        /// if bid turn for self player then open select bid popup
        /// if for other player start timer on profile and display biding... text on tool tip
        /// </summary>
        /// <param name="data"></param>
        //void SetUserBidTurnStartedData(JSONObject data)
        //{
        //    //  CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(1, 0);

        //    getUserBidTurnStarted = GetDeserializeData<USER_BID_TURN_STARTED>(data);
        //    CallBreak_UIManager.Inst.StopTurnTimer();

        //    //  CallBreak_UIManager.Inst.SetTableDetail();  //set table details
        //    CallBreak_GS.Inst.userTurnTimer = getUserBidTurnStarted.data.time;
        //    CallBreak_BidSelectHandle.Inst.itsMyBidTurn = false;
        //    if (CallBreak_GS.Inst.userinfo.Player_Seat == getUserBidTurnStarted.data.seatIndex)
        //    {
        //        CallBreak_BidSelectHandle.Inst.Bid_Button_Update(13, false);

        //        CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = true;
        //        CallBreak_UIManager.Inst.BidSelectPopupContent.GetComponent<RectTransform>().DOScale(1, 1.2f);

        //        CallBreak_GS.Inst.StopBidTurnTimer = false;
        //        CallBreak_BidSelectHandle.Inst.itsMyBidTurn = true;
        //        CallBreak_GameManager.Inst.GetPlayerSeatIndex(getUserBidTurnStarted.data.seatIndex).StartTurnTimer(getUserBidTurnStarted.data.seatIndex);
        //        //CallBreak_BidSelectHandle.Inst.StartBidselectionTextTimer(getUserBidTurnStarted.data.time);
        //        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        //        {
        //            if (CallBreak_UIManager.Inst.vibrateActive)
        //                Handheld.Vibrate();
        //        }
        //        CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.UserTurn);

        //        CallBreak_UIManager.Inst.CloseSideMenu();
        //        CallBreak_UIManager.Inst.CloseInfo();
        //        CallBreak_UIManager.Inst.CloseHelpInfo();
        //        CallBreak_AllPopupHandler.Instance.CloseExitPopup();
        //        CallBreak_Scoreboard.Inst.Close_Score_Board(0.5f);
        //    }
        //    else
        //    {
        //        CallBreak_GameManager.Inst.GetPlayerSeatIndex(getUserBidTurnStarted.data.seatIndex).StartTurnTimer(getUserBidTurnStarted.data.seatIndex);
        //        CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(getUserBidTurnStarted.data.seatIndex);
        //        if (!(p.SeatIndex == CallBreak_GS.Inst.userinfo.UserSeatIndex))
        //        {
        //            CallBreak_AllPopupHandler.Instance.OpenCenterMsgToast("Other player are still Bidding");
        //        }
        //    }
        //}
        void SetUserBidTurnStartedData(JSONObject data)
        {
            //  CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(1, 0);

            getUserBidTurnStarted = GetDeserializeData<USER_BID_TURN_STARTED>(data);
            CallBreak_UIManager.Inst.StopTurnTimer();

            //  CallBreak_UIManager.Inst.SetTableDetail();  //set table details
            CallBreak_GS.Inst.userTurnTimer = getUserBidTurnStarted.data.time;
            CallBreak_BidSelectHandle.Inst.itsMyBidTurn = false;




            CallBreak_BidSelectHandle.Inst.Bid_Button_Update(13, false);
            CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = true;
            CallBreak_UIManager.Inst.BidSelectPopupContent.GetComponent<RectTransform>().DOScale(1, 1.2f);
            CallBreak_GS.Inst.StopBidTurnTimer = false;
            CallBreak_BidSelectHandle.Inst.itsMyBidTurn = true;

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (CallBreak_UIManager.Inst.vibrateActive)
                    Handheld.Vibrate();
            }
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.UserTurn);
            CallBreak_UIManager.Inst.CloseSideMenu();
            CallBreak_UIManager.Inst.CloseInfo();
            CallBreak_UIManager.Inst.CloseHelpInfo();
            CallBreak_AllPopupHandler.Instance.CloseExitPopup();
            CallBreak_Scoreboard.Inst.Close_Score_Board(0.5f);

            for (int i = 0; i < getUserBidTurnStarted.data.seatIndexList.Count; i++)
            {
                CallBreak_GameManager.Inst.GetPlayerSeatIndex(getUserBidTurnStarted.data.seatIndexList[i]).StartTurnTimer(getUserBidTurnStarted.data.seatIndexList[i]);
            }

        }
        #endregion

        #region set after user select bid response data
        /// <summary>
        /// User Bid Data set rechive after user made bid
        /// set bid amount in tool tip and in player details on user profile bottom
        /// </summary>
        /// <param name="data"></param>
        void SetUserBidData(JSONObject data)
        {
            CallBreak_GS.Inst.StopBidTurnTimer = false;
            USER_BID_SHOW getUserBid = new USER_BID_SHOW();
            getUserBid = GetDeserializeData<USER_BID_SHOW>(data);

            if (CallBreak_GS.Inst.userinfo.Player_Seat == getUserBid.data.seatIndex)
            {
                CallBreak_UIManager.Inst.BidSelectPopupContent.GetComponent<RectTransform>().DOScale(0, 0.5f).OnComplete(() =>
                {
                    CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = false;
                });
            }
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                if (CallBreak_PlayerController.Inst.playerList[i].SeatIndex == getUserBid.data.seatIndex)
                {
                    CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                    CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;
                }
            }
            int seatindex = getUserBid.data.seatIndex;
            int value = getUserBid.data.bid;
            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatindex);
            p.BidBG.transform.DOScale(1, 0);
            p.BidBG.transform.GetChild(0).transform.GetComponent<Text>().text = "<color=#FEEDA8>Bid</color>" + Environment.NewLine + value;
            p.MyBidSelected = true;
            p.MyBidValue = value;
            p.BidAndHandValue.text = "0/" + value;
            CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
            Debug.Log("SetUserBidData || ----");
        }
        #endregion


        #region set user turn started data
        /// <summary>
        /// rechive when user turn stared
        /// first check that is self user turn or not
        /// if yes then enable card depends on card display condition and start turn animation
        /// if its not self user turn then just display other user turn on profile
        /// </summary>
        /// <param name="data"></param>
        internal bool ismyUserturn;
        void SetUserTurnStartedData(JSONObject data)
        {
            USER_TURN_STARTED getUserTurnStarted = new USER_TURN_STARTED();
            getUserTurnStarted = GetDeserializeData<USER_TURN_STARTED>(data);
            CallBreak_UIManager.Inst.isSpeacialCase = false;
            CallBreak_UIManager.Inst.ResetAllCardPosition();

            int powerCard = 0;
            int CardPower = 0;
            CallBreak_UIManager.Inst.StopTurnTimer();
            CallBreak_UIManager.Inst.HideAllPlayerBidToolTip();
            int seatindex = getUserTurnStarted.data.seatIndex;

            CallBreak_GS.Inst.userTurnTimer = getUserTurnStarted.data.time;
            string cardSequence = getUserTurnStarted.data.cardSequence;
            List<int> card_sequence_list = new List<int>();
            List<int> spade_card_sequence_list = new List<int>();
            List<string> thrown_card_ist = new List<string>();

            foreach (string Scard in getUserTurnStarted.data.card)
            {
                thrown_card_ist.Add(Scard.Substring(0, 1));

                if (Scard.Substring(0, 1) == "S")
                {
                    string[] cardsplit = Scard.Split('-');
                    CardPower = int.Parse(cardsplit[1]);
                    if (CardPower == 1)
                        CardPower = 14;
                    Debug.Log("Spade Card on table:- " + Scard + " With Power ==>>>" + CardPower);
                    spade_card_sequence_list.Add(CardPower);
                }

                Debug.Log(Scard.Substring(0, 1) + " == " + cardSequence);
                if (Scard.Substring(0, 1) == cardSequence)
                {
                    string[] cardsplit = Scard.Split('-');
                    CardPower = int.Parse(cardsplit[1]);
                    if (CardPower == 1)
                        CardPower = 14;
                    card_sequence_list.Add(CardPower);
                    card_sequence_list.Sort();
                    powerCard = card_sequence_list[card_sequence_list.Count - 1];
                    powerCard = card_sequence_list.Max();

                    Debug.Log("Card on table:- " + Scard + " With Power ==>>>" + powerCard);
                }
                Debug.Log("Card on table With Power ==>>>" + powerCard);
            }
            if (cardSequence != "S" && thrown_card_ist.Contains("S"))
            {
                Debug.Log(" <color=yellow> Its a Speacial Case found </color>");
                CallBreak_UIManager.Inst.isSpeacialCase = true;

                List<CallBreak_MyCard> SuitGroup = new List<CallBreak_MyCard>();
                SuitGroup = CallBreak_PlayerController.Inst._myCardList.FindAll(x => x.type == cardSequence);
                if (SuitGroup.Count == 0)
                {
                    powerCard = spade_card_sequence_list.Max();
                    Debug.Log(" Same Suit card not Found High Card is S->" + powerCard);

                }
            }

            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
            }
            CallBreak_BidSelectHandle.Inst.itsMyBidTurn = false;
            CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatindex).StartTurnTimer(seatindex);

            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatindex)
            {
                CallBreak_GS.Inst.isMyTurn = true;
                CallBreak_AllPopupHandler.Instance.CloseTopMsgToast();

                if (CallBreak_UIManager.Inst.MyCardsParent.transform.childCount != 1)//&& !CallBreak_CardDeal.Inst.cardCollectAnimation)
                {
                    CallBreak_GS.Inst.EnableAllMyCardTrigger();
                }
                else
                    Debug.Log("Card COllect Animation ongoing Eneble card trigger from collect card animation");

                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                {
                    if (CallBreak_UIManager.Inst.vibrateActive)
                        Handheld.Vibrate();
                }
                CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.UserTurn);
                CallBreak_UIManager.Inst.HighlightCurrentSuitGroup(cardSequence, powerCard); //======= Need to uncomment

                CallBreak_UIManager.Inst.CloseSideMenu();
                CallBreak_UIManager.Inst.CloseInfo();
                CallBreak_UIManager.Inst.CloseHelpInfo();
                CallBreak_AllPopupHandler.Instance.CloseExitPopup();
                CallBreak_Scoreboard.Inst.Close_Score_Board(0.5f);
            }
            else
            {
                CallBreak_GS.Inst.isMyTurn = false;
                CallBreak_UIManager.Inst.HighlightCurrentSuitGroup("N", powerCard);  //======= Need to uncomment
            }

            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatindex && CallBreak_GS.Inst.isMyTurn && CallBreak_UIManager.Inst.MyCardsParent.transform.childCount == 1)
            {
                if (!CallBreak_GS.Inst.isSelfUserInactive)
                {
                    CallBreak_GS.Inst.DisableAllMyCardTrigger();
                    Invoke(nameof(AutoThrowCard), 0.8f);
                }
            }
        }

        #endregion

        #region auto throw last card 
        /// <summary>
        /// if user has only one last card then we call this method
        /// </summary>
        internal void AutoThrowCard()
        {
            if (CallBreak_GS.Inst.isMyTurn)
            {
                Debug.Log("<color=Red>Auto Throw Last Card from Client</color>");
                CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.ThrowCard
                    (CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).gameObject.transform.GetComponent<Image>().sprite.name),
                    ThrowCardAcknowledgement, CallBreak_CustomEvents.USER_THROW_CARD);

            }
        }
        #endregion

        #region Set User card throw data
        /// <summary>
        /// set user throw card data and play animation of card throw
        /// check that is this spade broken then display spade broken animation
        /// </summary>
        /// <param name="data"></param>
        void SetUserThrowCardShowData(JSONObject data)
        {
            for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
            {
                if (CallBreak_PlayerController.Inst._myCardList[i].gameObject != null)
                {
                    CallBreak_PlayerController.Inst._myCardList[i].GetComponent<CallBreak_MyCard>().isCardThrowed = false;
                }
            }

            CallBreak_UIManager.Inst.Activate_Gold_Border();
            CallBreak_UIManager.Inst.ResetAllCardPosition();
            USER_THROW_CARD_SHOW getUserThrowCardShow = new USER_THROW_CARD_SHOW();

            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ThrowCard);

            getUserThrowCardShow = GetDeserializeData<USER_THROW_CARD_SHOW>(data);
            int seatIndex = getUserThrowCardShow.data.seatIndex;
            string cardName = getUserThrowCardShow.data.card;
            bool turnTimeout = getUserThrowCardShow.data.turnTimeout;
            CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = true;

            if (turnTimeout)
            {
                if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex)
                {
                    if (CallBreak_GS.Inst.isMyTurn)
                    {
                        CallBreak_GS.Inst.isMyTurn = false;
                        CallBreak_AllPopupHandler.Instance.OpenTopMsgToast("You have timed out!");
                        CallBreak_CardDeal.Inst.UserCardThrow(cardName, seatIndex, false);
                    }
                }
                else
                {
                    string name = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex).UserName.text;
                    CallBreak_AllPopupHandler.Instance.OpenTopMsgToast(name + " has timed out!");
                }
            }

            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex)
            {
                if (CallBreak_GS.Inst.isMyTurn)
                    CallBreak_CardDeal.Inst.UserCardThrow(cardName, seatIndex, false);
            }
            else
            {
                CallBreak_CardDeal.Inst.UserCardThrow(cardName, seatIndex, false);
            }
            CallBreak_GS.Inst.isMyTurn = false;
        }
        #endregion

        #region set user hand collect data and play animation of hand collect
        /// <summary>
        /// user hand collect animation take place here
        /// </summary>
        /// <param name="data"></param>
        void SetUserHandCollectData(JSONObject data)
        {

            WIN_OF_ROUND getUserHandCollect = new WIN_OF_ROUND();
            getUserHandCollect = GetDeserializeData<WIN_OF_ROUND>(data);

            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.HandCollect);

            Debug.Log(" Set User Hand CollectData ");
            int seatIndex = getUserHandCollect.data.seatIndex;
            int handCount = getUserHandCollect.data.handCount;
            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex);
            GameObject Player = null;
            if (p.gameObject.name == "Player1")
            {
                Debug.Log(" Player1 (Self Player ) Card throw ");
                Player = CallBreak_UIManager.Inst.AllPlayerPosition[0].gameObject;
                CallBreak_CardDeal.Inst.HandCollectAnim(p.gameObject, seatIndex);
                CallBreak_GS.Inst.HandCollectCountFTUE = handCount;
            }
            else
            {
                Debug.Log(" Player1 card not throw " + p.name);
                Player = p.gameObject;
                CallBreak_CardDeal.Inst.HandCollectAnim(Player, seatIndex);

            }
            p.MyHandCollect = handCount;
            p.BidAndHandValue.text = p.MyHandCollect + "/" + p.MyBidValue;
        }
        #endregion

        #region set round winner data
        /// <summary>
        /// on 13 turn over we rechive RoundWinner
        /// display scoreboard and set scoreboard data
        /// </summary>
        /// <param name="data"></param>
        void SetRoundWinnerData(JSONObject data)
        {
            CallBreak_GS.Inst.RoundNo++;
            getRoundWinnerData = GetDeserializeData<WINNER_DECLARE>(data);
            Debug.Log(" Round Winner Declare:: " + CallBreak_UIManager.Inst.ScoreBoardContent.transform.childCount);
            if (CallBreak_UIManager.Inst.ScoreBoardContent.transform.childCount != 0)
            {
                CallBreak_UIManager.Inst.CloseScoreBoardForWinner();
            }
            else
            {
                CallBreak_GS.Inst.canDisplayRoundTimerinScoreBoard = true;
                Debug.Log(" Thats ROund Winner Data ");
                CallBreak_Scoreboard.Inst.SetRoundData("WinnerData", getRoundWinnerData.data.timer);
            }
        }
        #endregion

        #region set scoreboard data if player click inbetween playing
        /// <summary>
        /// if user click on scoreboard button from playing screen then we set scoreboard data from here and display scoreboard
        /// </summary>
        /// <param name="data"></param>
        void SetShowScoreBoardData(JSONObject data)
        {
            getRoundWinnerData = new WINNER_DECLARE();
            getRoundWinnerData = GetDeserializeData<WINNER_DECLARE>(data);
            Debug.Log("getRoundWinnerData.roundScoreHistory.users.Count:" + getRoundWinnerData.data.roundTableId);
            Debug.Log("<color=Blue>inside SetShowScoreBoardData</color>");
            CallBreak_GS.Inst.canDisplayRoundTimerinScoreBoard = false;
            CallBreak_Scoreboard.Inst.SetRoundData("RoundData", getRoundWinnerData.data.timer);
        }
        #endregion

        #region Show Table Info (Menu Section)
        void SetTableInfoData(JSONObject data)
        {
            GetTABLE_INFO = new TABLE_INFO();
            GetTABLE_INFO = GetDeserializeData<TABLE_INFO>(data);

            Debug.Log("<color=Blue>inside SetTableInfoData</color>");
            CallBreak_UIManager.Inst.Show_TableInfoData();
        }

        #endregion

        #region set data for leave table 
        /// <summary>
        /// if user leave set that user status from here
        /// </summary>
        /// <param name="data"></param>
        void SetLeaveTableData(JSONObject data)
        {
            LEAVE_TABLE getLeaveTableData = new LEAVE_TABLE();
            getLeaveTableData = GetDeserializeData<LEAVE_TABLE>(data);
            int seatIndex = getLeaveTableData.data.seatIndex;
            bool playerLeave = getLeaveTableData.data.playerLeave;
            string msg = getLeaveTableData.data.msg;
            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex);
            if (playerLeave)  //&& lockInState //need to add 
            {
                Debug.Log("<color=red>Player Left</color>");
                p.PlayerLeft();
                p.MakeSeatEmpty(seatIndex);
                CallBreak_GameManager.Inst.ActivePlayerList.Remove(p);
                Debug.Log("Stop Timer And Make Player IN WatingState || Active list Count "
                    + CallBreak_GameManager.Inst.ActivePlayerList.Count + " ||  Max player " + CallBreak_GS.Inst.MaxPlayer
                    + "|| Table State " + CallBreak_GS.Inst.CurrentTableState + " || isBootCollected " + CallBreak_GS.Inst.isBootCollected);
                if (CallBreak_GameManager.Inst.ActivePlayerList.Count != CallBreak_GS.Inst.MaxPlayer)
                {
                    CallBreak_AllPopupHandler.Instance.CloseBigTopMsgToast();
                    CallBreak_GameManager.Inst.StopTimerAndMakePlayerINWatingState(CallBreak_GS.Inst.MaxPlayer);
                }
                p.StopTurnTimer();
                p.MakeEmptyUserdata();

            }
            else
            {
                Debug.Log("<color=red>Player Disconnected</color>");
                p.MakeInActive(msg);
            }

            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex && getLeaveTableData.data.msg.Contains("LEFT"))
            {
                Debug.Log("<color=red>Self Player Quit</color>");
                MGPSDK.MGPGameManager.instance.OnClickQuite();
            }
        }
        #endregion

        #region set data for leave table and display popup
        /// <summary>
        /// when user miss 3 consecutive turn we rechive time out leave table
        /// on this response we display alert popup to user with I Am Back button
        /// </summary>
        /// <param name="data"></param>
        void SetLeaveTableTimeOutData(JSONObject data)
        {
            //CallBreak_PopupHandler.Inst.OpenCommonPopup("IAmBack", data);
            CallBreak_AllPopupHandler.Instance.OpenOfflineCommonPopup("IAmBack", data);
        }
        #endregion

        #region set Rejoin table data
        /// <summary>
        /// get rejoin data and Deserialize
        /// </summary>
        /// <param name="data"></param>
        void SetReJoinData(JSONObject data)
        {
            Debug.Log("<color=Green>Class :-SocketEventReceiver || Method : ReceiveData -> || REJOIN Event </color>" + data);

            try
            {
                getRejoinData = new REJOIN();
                getRejoinData = GetDeserializeData<REJOIN>(data);
            }
            catch (System.Exception e) { Debug.LogError(e.ToString()); }


            CallBreak_GS.Inst.userinfo.ID = PlayerPrefs.GetString("UID");
            CallBreak_GS.Inst.userinfo.Player_Seat = getRejoinData.seatIndex;
            Debug.Log("<color=Red>Self Player UserId:" + CallBreak_GS.Inst.userinfo.ID + "</color>");
            CallBreak_GS.Inst.roundTableId = getRejoinData.roundTableId;
            CallBreak_GS.Inst.MaxPlayer = getRejoinData.noOfPlayer;
            CallBreak_GS.Inst.gameinfo.tabelId = getRejoinData.tableId;
            CallBreak_GS.Inst.bootData.bootValue = float.Parse(getRejoinData.bootValue);
            CallBreak_GS.Inst.MaxRound = getRejoinData.totalRound;
            CallBreak_GS.Inst.gameinfo.WinAmount = float.Parse(getRejoinData.winnningAmonut);
            CallBreak_GS.Inst.userWalletBalance = float.Parse(getRejoinData.userBalance);
            Debug.Log(" current round in rejoin data  found " + getRejoinData.currentRound);
            if (CallBreak_GS.Inst.MaxRound < getRejoinData.currentRound)
                CallBreak_UIManager.Inst.isTieBreakerRound = true;

            CallBreak_UIManager.Inst.SetPlayingHeader(getRejoinData.currentRound);
            if (getRejoinData.tableState == "WAITING_FOR_PLAYERS")
            {
                Debug.Log(" Its a WAITING_FOR_PLAYERS state open waiting popup ");
                CallBreak_GameManager.Inst.StopTimerAndMakePlayerINWatingState(CallBreak_GS.Inst.MaxPlayer);
            }

            if (getRejoinData.statusFlag)  //if status flag is true then display alert popup with message received from server
            {
                Debug.Log("<color=red> statusflag : True </color>");
                CallBreak_CardDeal.Inst.ResetPreviousThrowsCards();  //reset previous throws cards from table                    
                CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);
                CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
                CallBreak_AllPopupHandler.Instance.OpenOfflineCommonPopup("DisconnectOnWaitingState", data);

            }
            else
            {
                Debug.Log("<color=Blue>ReJoin, Set Player Data Here</color>");
                int MaxPlayer = getRejoinData.noOfPlayer;
                int ActivePlayer = getRejoinData.totalPlayers;
                Debug.Log("MaxPlayer::" + MaxPlayer + "  ActivePlayer::" + ActivePlayer);

                if (MaxPlayer != ActivePlayer)
                {
                    CallBreak_UIManager.Inst.SetPlayerDataAfterAppKillRejoin();
                    CallBreak_CardDeal.Inst.ResetPreviousThrowsCards();
                    Debug.Log("On Rejoin All Available Player Seated");
                    CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);
                    Debug.Log("Hide reconnection Loader");
                    return;
                }

                CallBreak_GS.Inst.isMyTurn = false;
                for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
                {
                    CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;  //stop all ongoing timer
                    CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;

                }

                Debug.Log("<color=Blue>Table State in Rejoin::" + getRejoinData.tableState + "</color>");
                Debug.Log("<color=Blue>Card Deal Animation is Ongoing or Not:</color>" + CallBreak_CardDeal.Inst.CardDealAnimOnGoing);
                if (CallBreak_CardDeal.Inst.CardDealAnimOnGoing)
                {
                    CallBreak_CardDeal.Inst.KillCardDealAnimation();  //kill ongoing card deal animation
                    CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();   //close message tool tip from center table 
                    CallBreak_CardDeal.Inst.CardDealAnimOnGoing = false;
                }
                CallBreak_UIManager.Inst.ReJoinGameReset();   //reset game play and set game play details received in rejoin response
            }




        }
        #endregion

        #region set data to display toast msg for user lock
        /// <summary>
        /// Deserialize Data received from server
        /// display tool tip message on header of screen with message received from server
        /// </summary>
        /// <param name="data"></param>
        void SetLockINPeriodData(JSONObject data)
        {
            LOCK_IN_PERIOD getLockINPeriod = new LOCK_IN_PERIOD();
            getLockINPeriod = GetDeserializeData<LOCK_IN_PERIOD>(data);
            string msg = getLockINPeriod.data.msg;
            CallBreak_UIManager.Inst.SideMenuBtn.GetComponent<Image>().sprite = CallBreak_UIManager.Inst.DisableSideMenuBtn;
            CallBreak_UIManager.Inst.leaveTableButton.interactable = false;
            CallBreak_UIManager.Inst.SideMenuBtn.interactable = false;
            CallBreak_UIManager.Inst.CloseSideMenu();

            CallBreak_AllPopupHandler.Instance.CloseExitPopup();
            CallBreak_AllPopupHandler.Instance.OpenBigTopMsgToast(msg);
        }
        #endregion


        #region Set Back In Game Playing Data and make player Active
        /// <summary>
        /// Back in game playing event response data Deserialize
        /// if its self user then hide i am back alert popup
        /// if its not self player then remove inactive tag from player profile
        /// </summary>
        /// <param name="data"></param>
        void SetBackInGamePlayingData(JSONObject data)
        {
            BACK_IN_GAME_PLAYING getBackInGamePlaying = new BACK_IN_GAME_PLAYING();
            getBackInGamePlaying = GetDeserializeData<BACK_IN_GAME_PLAYING>(data);

            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(getBackInGamePlaying.data.seatIndex);
            p.MakeActive();
            Debug.Log("Player_Seat:" + CallBreak_GS.Inst.userinfo.Player_Seat + " getBackInGamePlaying:" + getBackInGamePlaying.data.seatIndex);
            if (CallBreak_GS.Inst.userinfo.Player_Seat == getBackInGamePlaying.data.seatIndex)
            {
                CallBreak_GS.Inst.isSelfUserInactive = false;
                CallBreak_AllPopupHandler.Instance.CloseOfflineCommomPopup();//hide i am back popup

            }
        }
        #endregion


        #region Server response for show Header Tooltip
        void SHOW_POPUP(JSONObject data)
        {
            getShowPopupData = GetDeserializeData<CallBreak_Server_Popup>(data);
            if (CallBreak_GS.Inst.isGameEnded)
            {
                if (getShowPopupData.data.popupType != "scoreBoardPopup")
                    CallBreak_CardDeal.Inst.ResetPreviousThrowsCards();  //reset previous throws cards from table


                CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);
                CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
                try
                {
                    string reason = "";
                    if (data.HasField("reason"))
                        reason = data.GetField("reason").ToString().Trim(trim_char_arry);
                }
                catch { }
            }
            //CallBreak_GenericErrorPopup.Inst.Show_Server_Popup(getShowPopupData);
            CallBreak_AllPopupHandler.Instance.Show_Server_Popup(getShowPopupData);
        }
        #endregion

        #region ACKNOWLEDGEMENT_CALLBACKS
        internal void ThrowCardAcknowledgement(string ackData)
        {
            Debug.Log("CallBreak_SocketEventReceiver->ThrowCard Acknowledged || ackData : " + ackData);
        }

        #endregion
        #region Server response for Rejoin Old Loby Popup
        void SetRejoinPopupData(JSONObject data)
        {
            rejoinResponseHandler = GetDeserializeData<RejoinResponseHandler>(data);
            Debug.Log(" Player already Playing Onther game  Show  Popup ");
            CallBreak_SocketConnection.intance.isSignupRecived = true;
            CallBreak_UIManager.Inst.ReconnectionPanel.SetActive(false);
            CallBreak_AllPopupHandler.Instance.ShowRejoinToOldGamePopup(rejoinResponseHandler);

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataObjectWithInsideData"></param>
        /// <returns></returns>
        public static T GetDeserializeData<T>(JSONObject dataObjectWithInsideData)
        {
            return JsonConvert.DeserializeObject<T>(dataObjectWithInsideData.ToString());
        }
    }
}
