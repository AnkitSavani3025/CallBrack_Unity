using System;
using System.Collections.Generic;
using System.Linq;
using CallBreak_Socketmanager;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_UIManager : MonoBehaviour
    {

        #region Variables
        public static CallBreak_UIManager Inst;
        [Tooltip("main card prefab, our card parent object, card desk prefab")]
        public GameObject CardPrefab, CloseDesk, MyCardsParent, CardDeskPrefab, CardMain, RoundPrefab, ftueCardPrefab;
        [Tooltip("dealers and player positions")]
        public GameObject[] AllPlayerPosition, PlayerHolders, DiscardPositions;

        public GameObject P2DisGenPos, P3DisGenPos, P4DisGenPos, CardGeneratePoint, FaekCardHolder;//TableDetailBG

        [Tooltip("Side menu and UI screen panel ")]
        public GameObject SideMenu, sideMenuButton, infoPanel, helpInfoPanel, helpInfoScrollContent;
        public Toggle soundToggle, musicToggle, vibrateToggle;
        public Image soundSelectImage, musicSelectImage, vibrationSelectImage;
        public Sprite selectSprite, deselectSprite;

        [Tooltip("Scorebaord screen scrollview content object")]
        public GameObject ScoreBoardContent;

        [Header(" UI Section Sprites ")]
        public Sprite timerImage;
        public Sprite greenDot, redDot, redTimerImage, Menu, backMenu, EnableSideMenuBtn, DisableSideMenuBtn, emptyProfileImage;



        [Header("BID Selection  UI Section")]
        public GameObject BidSelectPopup;
        public GameObject BidSelectPopupContent, EntryFreeAmount, FinalPointsAmount, PotAmount, ReconnectionPanel;
        public Text bidSelectionText;

        [Tooltip("Button Referance ")]
        [SerializeField] internal Button leaveTableButton, ScoreBoardBtn, SideMenuBtn;

        [Tooltip("user card enable and disable color")]
        public Color DisableCardColor, EnableCardColor;

        [Tooltip("to check is the scoreboard is open or not")]
        internal bool isScoreBoardOpen = false, isTieBreakerRound = false;

        [Tooltip("FTUE")]
        public Canvas FTUEPanel;//, CommonPreloaderPanel;


        [Tooltip(" Booles value for card highlight And sound and vibrate ")]
        [SerializeField] internal bool isSpeacialCase, soundValueChangeOnAwake, vibrateActive;


        [Tooltip("Header Table Details")]
        [SerializeField] Text winAmount, bootAmount, Rounds, tableID;
        [SerializeField] internal GameObject headerMainParent;
        [SerializeField] internal Text walletAmount;

        [Tooltip("Table Info Panel  Section")]
        [SerializeField] Text entryFee, Rake, no_Of_Rounds, no_Of_Players, no_Of_Cards, tempText;
        #endregion


        private void Awake()
        {
            ScoreBoardBtn.gameObject.SetActive(false);
            Inst = this;
            //InvokeRepeating(nameof(TempVoid), 0, 1f);
        }
        //int counter = 0;
        //void TempVoid()
        //{
        //    Debug.Log("[" + System.DateTime.Now + "]"+ " Counter " + counter);
        //    tempText.text = counter.ToString();
        //    counter++;
        //}


        private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

        private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;

        #region HideAll BidBG
        internal void HideAllPlayerBidToolTip()
        {
            Debug.Log("  HideAllPlayerBidToolTip  OUT ");
            this.WaitforTime(0.5f, () =>
            {
                Debug.Log("  HideAllPlayerBidToolTip  In  ");

                for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
                    CallBreak_PlayerController.Inst.playerList[i].BidBG.transform.DOScale(0, 0);
            });
        }
        #endregion

        #region Set Playing Header
        internal void SetPlayingHeader(int currentRound)
        {
            ScoreBoardBtn.gameObject.SetActive(currentRound > 1);

            Debug.Log(" Set Plating Header Info --> winAmount || bootAmount || Rounds || tableID || walletAmount ");
            if (CallBreak_GS.Inst.bootData.bootValue != 0)
            {
                walletAmount.text = " ₹" + CallBreak_GS.Inst.userWalletBalance;
                winAmount.text = "₹" + CallBreak_GS.Inst.gameinfo.WinAmount.ToString();
                bootAmount.text = CallBreak_GS.Inst.bootData.bootValue.ToString();
                Rounds.text = "Round: " + currentRound + "/" + CallBreak_GS.Inst.MaxRound;
                tableID.text = "#" + CallBreak_GS.Inst.gameinfo.tabelId.Substring(CallBreak_GS.Inst.gameinfo.tabelId.Length - 8);
            }
            else
            {
                walletAmount.text = " ₹" + CallBreak_GS.Inst.userWalletBalance;
                winAmount.text = "₹" + "-";
                bootAmount.text = "-";
                Rounds.text = "Round: " + currentRound + "/" + CallBreak_GS.Inst.MaxRound;
                tableID.text = "#" + CallBreak_GS.Inst.gameinfo.tabelId.Substring(CallBreak_GS.Inst.gameinfo.tabelId.Length - 8);
            }
            if (isTieBreakerRound && CallBreak_GS.Inst.MaxRound < currentRound)
            {
                Rounds.text = " Tie Breaker ";
                Debug.Log(" Its a Tie Breaker Round from set playing header || Current round : " + currentRound + " Max Round : " + CallBreak_GS.Inst.MaxRound);
            }

        }
        #endregion



        #region Playing screen button's click methods
        public void OnClickBtn(string btnName)
        {
            if (!soundValueChangeOnAwake)
                CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.ButtonClick);
            else
            {
                soundValueChangeOnAwake = false;
                return;
            }

            switch (btnName)
            {
                case "SideMenu":
                    OpenSideMenu();  //Open side menu screen
                    break;
                case "CloseSideMenu":
                    CloseSideMenu();  //close side menu screen
                    break;
                case "ScoreBoard":
                    OpenScoreBoardInPlaying();   //open scoreboard
                    break;
                case "CloseScoreBoard":
                    CallBreak_Scoreboard.Inst.Close_Score_Board(0.5f); //close scoreboard
                    break;
                case "Exit":
                    CloseSideMenu();  //close side menu screen
                    CallBreak_AllPopupHandler.Instance.OpenExitPopup();  //open exit alert popup
                    break;
                case "ExitNo":
                    CallBreak_AllPopupHandler.Instance.CloseExitPopup();  //close exit alert popup
                    break;
                case "ExitYes":
                    ExitYesClick();  //on click exit screen yes button
                    break;
                case "SoundToggle":
                    UpdateSoundToggle();
                    break;
                case "MusicToggle":
                    UpdateMusicToggle();
                    break;
                case "VibrateToggle":
                    UpdateVibrateToggle();
                    break;
                case "OpenInfo":
                    OpenInfo();
                    break;
                case "CloseInfo":
                    CloseInfo();
                    break;
                case "OpenHelp":
                    SendHelpInfo();
                    break;
                case "CloseHelp":
                    CloseHelpInfo();
                    break;
                case "RejoinToOldGame":
                    SendRejoinToOldGame();
                    break;
                case "StartWithNewGame":
                    SendNewGameDataFromRejoinPopup();
                    break;
                case "NewGame":
                    CallBreak_AllPopupHandler.Instance.GameButton();
                    break;
                default:
                    Debug.LogError(" This type Of Button Click Not Define " + btnName);
                    break;

            }
        }
        #endregion

        #region Rejoin to Old Played Loby By Player
        void SendRejoinToOldGame()
        {
            Debug.Log(" Singup Old game data send ");
            CallBreak_GS.Inst.isAlredyPlaying = true;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.acessToken;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.minPlayer = int.Parse(CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.minPlayer);
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.noOfPlayer = int.Parse(CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.noOfPlayer);
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData._id = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.lobbyId;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.isUseBot = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.isUseBot;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.entryFee = float.Parse(CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.entryFee);
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.moneyMode = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.moneyMode;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.noOfRounds = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.totalRound;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.winningAmount = float.Parse(CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.winningAmount);
            MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.displayName = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.userName;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.userID = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.userId;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.avatar = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.profilePic;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.gameData.gameId = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.gameId;
            MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.isFTUE;
            CallBreak_GS.Inst.fromBack = CallBreak_SocketEventReceiver.Inst.rejoinResponseHandler.data.rejoinUserData.fromBack;
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.SignUp_New(), CallBreak_SocketConnection.intance.SignUpAcknowledgement, CallBreak_CustomEvents.REJOIN_OR_NEW_GAME);  //new method for send event with argument for Acknowledgement
            CallBreak_AllPopupHandler.Instance.CloseRejoinToOldGamePopup();
        }
        void SendNewGameDataFromRejoinPopup()
        {
            Debug.Log(" Singup new game  data send ");
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.SignUp_New(), CallBreak_SocketConnection.intance.SignUpAcknowledgement, CallBreak_CustomEvents.REJOIN_OR_NEW_GAME);  //new method for send event with argument for Acknowledgement
            CallBreak_AllPopupHandler.Instance.CloseRejoinToOldGamePopup();

        }
        #endregion

        #region OpenScoreBoard if player click in between playing
        void OpenScoreBoardInPlaying()
        {
            Debug.Log("Scoreboard button click");
            //if (CallBreak_FTUEHandler.isFTUE && !CallBreak_FTUEHandler.isFTUESelfPlay) //if FTUE is ongoing then don't perform this and no need to display scoreboard to user
            //    return;
            Debug.Log("ScoreBoardContent.transform.childCount::" + ScoreBoardContent.transform.childCount);

            if (ScoreBoardContent.transform.childCount == 0)
            {
                Debug.Log(" Sent data to server for scoreboard ");
                CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.SHOW_SCORE_BOARD(), ShowScoreboardAcknowledgement, CallBreak_CustomEvents.SHOW_SCORE_BOARD);
            }
            else
            {
                for (int i = 0; i < ScoreBoardContent.transform.childCount; i++)  //clear scoreboard data on close scoreboard
                {
                    DestroyImmediate(ScoreBoardContent.transform.GetChild(i).gameObject);
                    Debug.Log("Inside OpenScoreBoardInPlaying++++++++++" + ScoreBoardContent.transform.childCount);
                }
                CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.SHOW_SCORE_BOARD(), ShowScoreboardAcknowledgement, CallBreak_CustomEvents.SHOW_SCORE_BOARD);
            }

        }
        #endregion

        #region Open And Close side menu method
        internal void OpenSideMenu()
        {
            if (sideMenuButton.GetComponent<Image>().sprite.name == "menu") //Open Side Manu
            {
                SideMenu.GetComponent<Canvas>().enabled = true;
                SideMenu.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-240.5f, -280), 0.3f, true).OnComplete(() =>
                {
                    sideMenuButton.GetComponent<Image>().sprite = backMenu;
                });
            }
            else    //Close  Side Manu
            {
                SideMenu.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-240.5f, 260), 0.3f, true).OnComplete(() =>
                {
                    SideMenu.GetComponent<Canvas>().enabled = false;
                    sideMenuButton.GetComponent<Image>().sprite = Menu;
                });
            }
        }

        internal void OpenInfo()
        {
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.TABLE_INFO(), ShowTable_InfoAcknowledgement, CallBreak_CustomEvents.GTI_INFO);
        }
        internal void SendHelpInfo()
        {
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.HELP(), CallBreak_SocketConnection.intance.HelpAcknowledgement, CallBreak_CustomEvents.HELP);
        }

        internal void Show_TableInfoData()
        {
            entryFee.text = CallBreak_SocketEventReceiver.Inst.GetTABLE_INFO.data.data.entryFee.ToString();
            Rake.text = CallBreak_GS.Inst.gameinfo.tabelId;
            no_Of_Rounds.text = CallBreak_SocketEventReceiver.Inst.GetTABLE_INFO.data.data.nmberOfRounds.ToString();
            no_Of_Players.text = CallBreak_SocketEventReceiver.Inst.GetTABLE_INFO.data.data.numberOfPlayer.ToString();
            no_Of_Cards.text = CallBreak_SocketEventReceiver.Inst.GetTABLE_INFO.data.data.numberOfCard.ToString();
            infoPanel.SetActive(true);
            infoPanel.transform.DOMove(new Vector2(0, 0), 0.3f);
            CloseSideMenu();
        }
        internal void ShowHelpInfoData()
        {
            helpInfoPanel.SetActive(true);
            helpInfoPanel.transform.DOMove(new Vector2(0, 0), 0.3f);
            CloseSideMenu();
        }

        internal void CloseInfo()
        {
            infoPanel.transform.DOMove(new Vector2(2200, 0), 0.3f).OnComplete(() =>
            {
                infoPanel.SetActive(false);
            });
        }
        internal void CloseHelpInfo()
        {
            helpInfoPanel.transform.DOMove(new Vector2(2200, 0), 0.3f).OnComplete(() =>
            {
                helpInfoPanel.SetActive(false);
                helpInfoScrollContent.GetComponent<RectTransform>().DOMoveY(0, 0);
            });
        }
        internal void CloseSideMenu()
        {
            SideMenu.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-240, 260f), 0.3f, true).OnComplete(() =>
            {
                sideMenuButton.GetComponent<Image>().sprite = Menu;
                SideMenu.GetComponent<Canvas>().enabled = false;
            });
        }
        #endregion      

        #region Update Sound Toggle
        internal void UpdateSoundToggle()
        {
            if (PlayerPrefs.GetString("Sound") == "OFF")
            {
                soundToggle.isOn = true;
                PlayerPrefs.SetString("Sound", "ON");
                soundSelectImage.sprite = selectSprite;
            }
            else
            {
                soundSelectImage.sprite = deselectSprite;
                soundToggle.isOn = false;
                PlayerPrefs.SetString("Sound", "OFF");
            }
        }
        #endregion

        #region Update Sound Toggle
        internal void UpdateMusicToggle()
        {
            if (PlayerPrefs.GetString("Music") == "OFF")
            {
                musicToggle.isOn = true;
                PlayerPrefs.SetString("Music", "ON");
                musicSelectImage.sprite = selectSprite;
                CallBreak_SoundManager.Inst.backGroundAudio.clip = CallBreak_SoundManager.Inst.backClip;
                CallBreak_SoundManager.Inst.backGroundAudio.Play();
            }
            else
            {
                musicSelectImage.sprite = deselectSprite;
                musicToggle.isOn = false;
                PlayerPrefs.SetString("Music", "OFF");
                CallBreak_SoundManager.Inst.backGroundAudio.Stop();
            }
        }
        #endregion

        #region Update Vibrate Toggle
        internal void UpdateVibrateToggle()
        {
            if (PlayerPrefs.GetString("Vibrate") == "OFF")
            {
                vibrateToggle.isOn = true;
                vibrateActive = true;
                PlayerPrefs.SetString("Vibrate", "ON");
                vibrationSelectImage.sprite = selectSprite;
            }
            else
            {
                vibrateToggle.isOn = false;
                vibrateActive = false;
                PlayerPrefs.SetString("Vibrate", "OFF");
                vibrationSelectImage.sprite = deselectSprite;
            }
        }
        #endregion

        #region reset card position before and after throw card
        /// <summary>
        ///  this is for Reset card position if some card are upper after user turn started  and card throw  
        /// </summary>
        internal void ResetAllCardPosition()
        {
            for (int i = 0; i < MyCardsParent.transform.childCount; i++)
            {
                MyCardsParent.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPosY(0, 0);
            }
        }
        #endregion

        #region in WINNER_DECLARE event response if in scoreboard screen old data present reset
        /// <summary>
        /// 
        /// </summary>
        internal void CloseScoreBoardForWinner()
        {
            Debug.Log("CloseScoreBoardForWinner");
            isScoreBoardOpen = false;
            Invoke(nameof(SetDataForWinner), 0.4f);  //set new data for scoreboard
        }

        void SetDataForWinner()
        {
            CallBreak_GS.Inst.canDisplayRoundTimerinScoreBoard = true;  //timer display for next round start
            CallBreak_Scoreboard.Inst.SetRoundData("WinnerData", CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.timer);
        }
        #endregion
        internal void CloseAllUserTurnTimer()
        {
            for (int i = 0; i < CallBreak_GameManager.Inst.ActivePlayerList.Count; i++)
            {
                CallBreak_GameManager.Inst.ActivePlayerList[i].StopTurnTimer();
                Debug.Log(" Stop user" + CallBreak_GameManager.Inst.ActivePlayerList[i] + " Turn Timer  Active Player list Count " + CallBreak_GameManager.Inst.ActivePlayerList.Count);
            }
        }

        #region Highlight current suit group and disable other group
        public void HighlightCurrentSuitGroup(string Suit, int CardPower)
        {
            Debug.Log("<color=yellow>HighlightCurrentSuitGroup Suit " + Suit + " Power " + CardPower + "</color>");

            switch (Suit)
            {
                case "C":   //current turn suit
                    EnableOtherSuit("C", CardPower);    //enable suit card group pass in argument
                    if (!CallBreak_GS.Inst.NotAnyCardWithSuit)   //disable other card groups if any card with current suit found
                        FindSuitGroup("D", "H", "S");     //disable suit card groups pass in argument
                    break;
                case "D":
                    EnableOtherSuit("D", CardPower);
                    if (!CallBreak_GS.Inst.NotAnyCardWithSuit)
                        FindSuitGroup("C", "H", "S");
                    break;
                case "H":
                    EnableOtherSuit("H", CardPower);
                    if (!CallBreak_GS.Inst.NotAnyCardWithSuit)
                        FindSuitGroup("C", "D", "S");
                    break;
                case "S":
                    EnableOtherSuit("S", CardPower);
                    if (!CallBreak_GS.Inst.NotAnyCardWithSuit)
                        FindSuitGroup("C", "D", "H");
                    break;
                case "N":
                    EnableAllSuit();   //enable all card
                    break;

                default:
                    break;
            }
        }
        #endregion
        #region Method for finding current suit cards group and do work
        /// <summary>
        /// find current suit cards groups pass in arguments and disable those cards groups
        /// </summary>
        /// <param name="Suit1"></param>
        /// <param name="Suit2"></param>
        /// <param name="Suit3"></param>
        /// 
        void DisableSuit(string suit)
        {
            List<CallBreak_MyCard> SuitGroup = new List<CallBreak_MyCard>();
            SuitGroup = CallBreak_PlayerController.Inst._myCardList.FindAll(x => x.type == suit);
            for (int i = 0; i < SuitGroup.Count; i++)
            {
                SuitGroup[i].GetComponent<Image>().DOColor(DisableCardColor, 0);
            }
            SuitGroup.Clear();
        }
        void FindSuitGroup(string Suit1, string Suit2, string Suit3)
        {
            string[] suit = new string[3] { Suit1, Suit2, Suit3 };
            foreach (string type in suit)
            {
                DisableSuit(type);
            }
        }
        #endregion

        #region reseting groups when we going to highlight one group
        void EnableOtherSuit(string Suit, int CardPower)
        {

            List<GameObject> cardwithLowPower = new List<GameObject>();
            List<GameObject> cardwithHighPower = new List<GameObject>();

            List<GameObject> otherCard_List = new List<GameObject>();
            List<GameObject> spadeCard_List = new List<GameObject>();

            int thrownCard_Power = CardPower;
            Debug.LogError("Card Thrown Power" + thrownCard_Power);

            List<CallBreak_MyCard> SuitGroup = new List<CallBreak_MyCard>();
            SuitGroup = CallBreak_PlayerController.Inst._myCardList.FindAll(x => x.type == Suit);
            for (int i = 0; i < SuitGroup.Count; i++)
            {
                if (SuitGroup[i].cardValue > thrownCard_Power)
                {
                    cardwithHighPower.Add(SuitGroup[i].gameObject);
                    Debug.LogError("Card added to more power Card List:- " + SuitGroup[i].name + " ||  With card Power ===> " + SuitGroup[i].cardValue);
                }
                if (SuitGroup[i].cardValue < thrownCard_Power)
                {
                    cardwithLowPower.Add(SuitGroup[i].gameObject);
                    Debug.LogError("Card added to less power Card List:- " + SuitGroup[i].name + " ||  With card Power ===> " + SuitGroup[i].cardValue);
                }

            }
            if (isSpeacialCase && SuitGroup.Count != 0)
            {
                CallBreak_GS.Inst.NotAnyCardWithSuit = false;

                foreach (CallBreak_MyCard card in SuitGroup)
                {
                    Debug.Log("Speacial Case found and highlight same suit cards " + card.name + " Value " + card.cardValue);
                    card.GetComponent<Image>().DOColor(EnableCardColor, 0);
                }
            }
            if (!isSpeacialCase && SuitGroup.Count != 0)
            {
                Debug.Log(" Its  Normal case Highlight same suit high card Only ");
                Highlight_Same_Suit_Card(cardwithLowPower, cardwithHighPower, SuitGroup);
            }
            if (isSpeacialCase && SuitGroup.Count == 0)
            {
                CallBreak_GS.Inst.NotAnyCardWithSuit = true;
                cardwithLowPower = new List<GameObject>();
                cardwithHighPower = new List<GameObject>();
                SuitGroup = new List<CallBreak_MyCard>();

                SuitGroup = CallBreak_PlayerController.Inst._myCardList.FindAll(x => x.type == "S");
                for (int i = 0; i < SuitGroup.Count; i++)
                {
                    if (SuitGroup[i].cardValue > thrownCard_Power)
                    {
                        cardwithHighPower.Add(SuitGroup[i].gameObject);
                        Debug.LogError("Card added to more power Card List:- " + SuitGroup[i].name + " ||  With card Power ===> " + SuitGroup[i].cardValue);
                    }
                    if (SuitGroup[i].cardValue < thrownCard_Power)
                    {
                        cardwithLowPower.Add(SuitGroup[i].gameObject);
                        Debug.LogError("Card added to less power Card List:- " + SuitGroup[i].name + " ||  With card Power ===> " + SuitGroup[i].cardValue);
                    }
                    spadeCard_List.Add(SuitGroup[i].gameObject);

                }
                if (spadeCard_List.Count > 0)
                {
                    if (cardwithHighPower.Count < 1)
                    {
                        // Do nothing ================
                    }
                    else
                    {
                        if (cardwithLowPower.Count > 0)
                        {
                            foreach (GameObject card in cardwithLowPower)
                            {
                                Debug.LogError(card.GetComponent<CallBreak_MyCard>().cardValue);
                                card.GetComponent<Image>().DOColor(DisableCardColor, 0);
                            }
                        }
                        FindSuitGroup("C", "D", "H");

                    }
                }
                else
                {
                    EnableAllSuit();

                }
            }
            if (!isSpeacialCase && SuitGroup.Count == 0)
            {
                Debug.Log("Normal case " + isSpeacialCase + "not any card with Current Suit");
                CallBreak_GS.Inst.NotAnyCardWithSuit = true;

                for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
                {
                    if (CallBreak_PlayerController.Inst._myCardList[i].cardName.Substring(0, 1) == "S")
                    {
                        Debug.Log("My Card list has card Spade || Card Seq ==> " + CallBreak_PlayerController.Inst._myCardList[i].cardName.Substring(0, 1));
                        spadeCard_List.Add(CallBreak_PlayerController.Inst._myCardList[i].gameObject);
                    }
                    else
                    {
                        Debug.Log("My Card list has card other than spade || Card Seq ==> " + CallBreak_PlayerController.Inst._myCardList[i].cardName.Substring(0, 1));
                        otherCard_List.Add(CallBreak_PlayerController.Inst._myCardList[i].gameObject);
                    }
                }

                if (spadeCard_List.Count < 1)
                {
                    EnableAllSuit();
                }
                else
                {
                    foreach (GameObject card in otherCard_List)
                    {
                        card.GetComponent<Image>().DOColor(DisableCardColor, 0);
                    }
                }
            }
            else if (!isSpeacialCase)
            {
                CallBreak_GS.Inst.NotAnyCardWithSuit = false;
            }
            else
            {
                Debug.Log(" Speacial case Dose not change in  boolen");
            }
        }

        void Highlight_Same_Suit_Card(List<GameObject> cardwithLowPower, List<GameObject> cardwithHighPower, List<CallBreak_MyCard> suitGroup)
        {
            if (cardwithHighPower.Count < 1)
            {
                // Do nothing ================
            }
            else
            {
                if (cardwithLowPower.Count > 0)
                {
                    foreach (GameObject card in cardwithLowPower)
                    {
                        Debug.LogError(card.GetComponent<CallBreak_MyCard>().cardValue);
                        card.GetComponent<Image>().DOColor(DisableCardColor, 0);
                    }
                }
            }
        }
        #endregion


        #region Enable all cards
        void EnableAllSuit()
        {
            for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
            {
                try
                {
                    CallBreak_PlayerController.Inst._myCardList[i].GetComponent<Image>().DOColor(EnableCardColor, 0);
                }
                catch { }
            }
        }

        void EnableCallBreak(List<GameObject> spadeCard_List, List<GameObject> otherCard_List)
        {
            if (spadeCard_List.Count > 0)
            {
                foreach (GameObject card in otherCard_List)
                {
                    card.GetComponent<Image>().DOColor(DisableCardColor, 0);
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
                    {
                        CallBreak_PlayerController.Inst._myCardList[i].GetComponent<Image>().DOColor(EnableCardColor, 0);
                    }

                }
                catch { }
            }
        }
        #endregion

        #region In Exit Alert popup Yes button click
        internal void ExitYesClick()
        {
            Debug.Log(" Game Exite Called Application Kill completed ");
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.LEAVE_TABLE(), CallBreak_UIManager.Inst.LeaveTableAcknowledgement, CallBreak_CustomEvents.LEAVE_TABLE); //send leave table
            //MGPSDK.MGPGameManager.instance.OnClickQuite();
        }
        #endregion   


        #region Stop all timer for all user
        /// <summary>
        /// hide ongoing user turn timer
        /// </summary>
        internal void StopTurnTimer()
        {
            Debug.Log("<color=red> stop timer </color> ");
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].goldBorder.transform.localScale = Vector3.one;
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;
                CallBreak_PlayerController.Inst.playerList[i].StopTurnTimer();
            }
            BidSelectPopup.GetComponent<Canvas>().enabled = false;
        }
        #endregion

        public void Activate_Gold_Border()
        {
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].goldBorder.transform.localScale = Vector3.one;
            }
        }


        #region after round play game playing resetting
        /// <summary>
        /// reset game play
        /// reset flags, lists and user scores and hand counts
        /// </summary>
        internal void GamePlayReset(bool canHideDealerIcon)
        {
            CallBreak_GS.Inst.isMyTurn = false;
            CallBreak_GS.Inst.NotAnyCardWithSuit = false;
            CallBreak_GS.Inst.StopBidTurnTimer = false;

            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].MyHandCollect = 0;
                CallBreak_PlayerController.Inst.playerList[i].MyBidValue = 0;
                CallBreak_PlayerController.Inst.playerList[i].BidAndHandValue.text = "0/0";
                if (canHideDealerIcon)
                    CallBreak_PlayerController.Inst.playerList[i].DealerIcon.transform.DOScale(0, 0);
            }
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;
            }
        }

        #endregion



        #region On Rejoin game reset playing and set current playing data
        /// <summary>
        /// after rejoin game from background /foreground or app kill this method call on server response
        /// if its app kill rejoin first seat and set player on table
        /// then stop all ongoing timer and process
        /// set on table cards throwed by user and remaning card on self user hand
        /// if its bid turn then display select bid popup for self user and biding toltip for other
        /// make players active/inactive depends on received status from server
        /// </summary>

        internal void ReJoinGameReset()
        {
            bool isAnyCardInHand = false;
            try
            {
                //else CallBreak_GS.Inst.isBootCollected = true;

                Debug.Log("  Re join game reset || Table state Recived From backend  || " + CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState);

                if (CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "ROUND_TIMER_STARTED" || CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "LOCK_IN_PERIOD"
                    || CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "SCOREBOARD_DECLARED")
                {
                    Debug.Log("RTS Time::" + CallBreak_SocketEventReceiver.Inst.getRejoinData.remaningRoundTimer);
                    CallBreak_AllPopupHandler.Instance.StartRTSTimer(CallBreak_SocketEventReceiver.Inst.getRejoinData.remaningRoundTimer, " New Round Start in ");
                }
                if (CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "LOCK_IN_PERIOD")
                {
                    CallBreak_AllPopupHandler.Instance.OpenBigTopMsgToast(CallBreak_SocketEventReceiver.Inst.getRejoinData.massage);
                    leaveTableButton.interactable = false;
                }
                else
                {
                    CallBreak_AllPopupHandler.Instance.CloseBigTopMsgToast();
                    leaveTableButton.interactable = true;
                }



                SetPlayerDataAfterAppKillRejoin();
                SideMenuBtn.interactable = true;
                SideMenuBtn.GetComponent<Image>().sprite = EnableSideMenuBtn;
                Debug.Log("Active Player Count::" + CallBreak_GS.Inst.ActivePlayer);
                if (CallBreak_GS.Inst.ActivePlayer != 4)
                {
                    CallBreak_PlayerController.Inst.SeatPlayerOnRejoin();
                }


                for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
                {
                    CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                    CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;

                }
                CallBreak_GS.Inst.isMyTurn = false;

                for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
                {
                    CallBreak_PlayerController.Inst.playerList[i].DealerIcon.transform.DOScale(0, 0);
                }
                // for set dealer icon on profile
                if (CallBreak_SocketEventReceiver.Inst.getRejoinData.dealerPlayer != -1)
                {
                    Debug.Log(">>>>>>>>>>>>>::" + CallBreak_SocketEventReceiver.Inst.getRejoinData.dealerPlayer);
                    int DealerSeatIndex = CallBreak_SocketEventReceiver.Inst.getRejoinData.dealerPlayer;
                    CallBreak_Users P = CallBreak_GameManager.Inst.GetPlayerSeatIndex(DealerSeatIndex);
                    P.DealerIcon.transform.DOScale(1, 0);

                }
                if (!CallBreak_CardDeal.Inst.CardDealAnimOnGoing)
                {
                    CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
                }
                try
                {
                    GameObject Card = CardMain;

                    CallBreak_PlayerController.Inst._myCardList = new List<CallBreak_MyCard>();
                    CallBreak_PlayerController.Inst._myCardList.Clear();

                    for (int i = 0; i < CallBreak_CardDeal.Inst.DisCardedCardsList.Count; i++)
                    {
                        DestroyImmediate(CallBreak_CardDeal.Inst.DisCardedCardsList[i]);
                    }
                    Debug.Log(" Rejoin Event ");
                    CallBreak_CardDeal.Inst.DisCardedCardDestroyImmediate();

                    CallBreak_CardDeal.Inst.DisCardedCardsList.Clear();
                    CallBreak_SocketEventReceiver.Inst.CardOrder = 0;
                    for (int j = 0; j < CallBreak_SocketEventReceiver.Inst.getRejoinData.turnCurrentCards.Count; j++)
                    {
                        if (CallBreak_SocketEventReceiver.Inst.getRejoinData.turnCurrentCards[j] != "U-0")
                        {
                            CallBreak_SocketEventReceiver.Inst.CardOrder++;
                            if (CallBreak_GS.Inst.userinfo.Player_Seat == j)
                            {
                                CallBreak_CardDeal.Inst.Player1CardThrowRejoin(CallBreak_SocketEventReceiver.Inst.getRejoinData.turnCurrentCards[j]);
                            }
                            else
                            {
                                CallBreak_CardDeal.Inst.UserCardThrow(CallBreak_SocketEventReceiver.Inst.getRejoinData.turnCurrentCards[j], j, true);
                            }
                        }
                    }
                    CallBreak_CardDeal.Inst.discardedCardContainer.transform.DOScaleX(1, 0f);

                    for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; i++)
                    {
                        if (CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].userId != null)
                        {
                            bool isBidTurn = CallBreak_SocketEventReceiver.Inst.getRejoinData.isBidTurn;
                            int seatindex = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].seatIndex;
                            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatindex)
                            {
                                int childCount = MyCardsParent.transform.childCount;

                                for (int n = 0; n < childCount; n++)
                                {
                                    DestroyImmediate(MyCardsParent.transform.GetChild(0).gameObject);
                                }

                                GameObject _Card = CardMain;
                                for (int m = 0; m < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards.Count; m++)
                                {
                                    Instantiate(_Card, MyCardsParent.transform);
                                }

                                Debug.Log("Rejoin Game Reset _myCardList.Count::" + CallBreak_PlayerController.Inst._myCardList.Count);
                                if (CallBreak_PlayerController.Inst._myCardList.Count == 0 && CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards.Count != 0)
                                {
                                    for (int a = 0; a < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards.Count; a++)
                                    {
                                        CallBreak_PlayerController.Inst._myCardList.Add(MyCardsParent.transform.GetChild(a).GetComponent<CallBreak_MyCard>());
                                    }
                                }
                                for (int k = 0; k < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards.Count; k++)
                                {
                                    string nm = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards[k];
                                    List<Sprite> temp = new List<Sprite>();
                                    temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == nm);
                                    CallBreak_PlayerController.Inst._myCardList[k].GetComponent<Image>().sprite = temp[0];
                                    CallBreak_PlayerController.Inst._myCardList[k].cardName = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards[k];
                                    CallBreak_PlayerController.Inst._myCardList[k].cardValue = int.Parse(CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards[k].Substring(2));
                                    if (CallBreak_PlayerController.Inst._myCardList[k].cardValue == 1)
                                    {
                                        CallBreak_PlayerController.Inst._myCardList[k].cardValue = 14;
                                    }
                                    CallBreak_PlayerController.Inst._myCardList[k].type = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards[k].Substring(0, 1);
                                    CallBreak_PlayerController.Inst._myCardList[k].gameObject.name = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards[k];
                                    isAnyCardInHand = true;
                                }

                                Debug.Log("<color=yellow>ReJoin Card Count::" + CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards.Count + " MyCardsParent childCount::" + MyCardsParent.transform.childCount + "</color>");
                                for (int j = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].currentCards.Count; j < MyCardsParent.transform.childCount; j++)
                                {
                                    CallBreak_PlayerController.Inst._myCardList.Remove(MyCardsParent.transform.GetChild(j).GetComponent<CallBreak_MyCard>());
                                    DestroyImmediate(MyCardsParent.transform.GetChild(j).gameObject);
                                    Debug.Log("<color=yellow>Remove Extra Card::</color>");
                                }
                                Debug.Log("<color=yellow>After Reset In Hand Card Count::</color>" + MyCardsParent.transform.childCount);
                                MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = true;

                            }

                            int handCount = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].hands;
                            int value = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].bid;
                            bool bidTurn = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].bidTurn;
                            float points = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].point;

                            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatindex);
                            p.MyHandCollect = handCount;
                            p.MyBidValue = value;
                            p.BidAndHandValue.text = p.MyHandCollect + "/" + p.MyBidValue;
                            p.MyScore.text = points.ToString();    //set player score here
                            if (bidTurn && isBidTurn)
                            // if (bidTurn)
                            {
                                p.BidBG.transform.DOScale(1, 0);
                                p.BidBG.transform.GetChild(0).transform.GetComponent<Text>().text = "<color=#FEEDA8>Bid</color>" + Environment.NewLine + value;
                                p.MyBidSelected = true;

                            }
                            else
                            {
                                p.BidBG.transform.DOScale(0, 0);
                                p.MyBidSelected = false;
                            }

                            bool isAuto = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].isAuto;
                            bool isLeft = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].isLeft;
                            if (isAuto && isLeft)
                            {
                                p.MakeInActive("LEFT");
                            }
                            else if (isAuto)
                            {
                                p.MakeInActive("DISCONNECTED");
                            }//both true left auto play
                            if (!isAuto && !isLeft)
                            {
                                p.MakeActive();
                                Debug.Log("Make " + p.UserName.text + " User Active");
                            }
                        }
                    }
                }
                catch (System.Exception e) { Debug.LogError(e.ToString()); }

                try
                {
                    string cardSequence = CallBreak_SocketEventReceiver.Inst.getRejoinData.turnCardSequence;

                    if (CallBreak_SocketEventReceiver.Inst.getRejoinData.currentTurn != -1)
                    {
                        int seatIndex = CallBreak_SocketEventReceiver.Inst.getRejoinData.currentTurn;
                        int currentTurnTimer = CallBreak_SocketEventReceiver.Inst.getRejoinData.currentTurnTimer;
                        int userTurnTimer = CallBreak_SocketEventReceiver.Inst.getRejoinData.userTurnTimer;
                        bool isBidTurn = CallBreak_SocketEventReceiver.Inst.getRejoinData.isBidTurn;
                        CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex);
                        CallBreak_GS.Inst.userTurnTimer = userTurnTimer;
                        Debug.Log("<color=red>is Bid Turn?::" + isBidTurn + "</color>");

                        if (isBidTurn)
                        {
                            try
                            {
                                if (CallBreak_CardDeal.Inst.DealAnimEnumeator != null)  //if bid turn is ongoing then stop card deal animation
                                    StopCoroutine(CallBreak_CardDeal.Inst.DealAnimEnumeator);
                            }
                            catch
                            {
                                Debug.Log(" Error in Stop Card Deal Animation ");
                            }
                            Debug.Log("Self SeatIndeax:" + CallBreak_GS.Inst.userinfo.Player_Seat + " == " + seatIndex + " ?");



                            for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; i++)
                            {

                                if (!CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].bidTurn)
                                {
                                    CallBreak_PlayerController.Inst.playerList[i].StartRemainingTurnTimer(currentTurnTimer, CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].seatIndex);
                                }

                                if (CallBreak_GS.Inst.userinfo.Player_Seat == CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].seatIndex && !CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].bidTurn)
                                {

                                    CallBreak_BidSelectHandle.Inst.Bid_Button_Update(13, false);
                                    BidSelectPopupContent.GetComponent<RectTransform>().DOScale(0, 0f);
                                    BidSelectPopup.GetComponent<Canvas>().enabled = true;
                                    BidSelectPopupContent.GetComponent<RectTransform>().DOScale(1, 0.5f);
                                    Debug.Log(" Remaining Turn Time in Rejoin:: " + currentTurnTimer);
                                    CallBreak_BidSelectHandle.Inst.itsMyBidTurn = true;
                                }
                            }



                            //if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex)
                            //{
                            //    CallBreak_BidSelectHandle.Inst.Bid_Button_Update(13, false);

                            //    BidSelectPopupContent.GetComponent<RectTransform>().DOScale(0, 0f);
                            //    BidSelectPopup.GetComponent<Canvas>().enabled = true;
                            //    BidSelectPopupContent.GetComponent<RectTransform>().DOScale(1, 0.5f);
                            //    Debug.Log(" Remaining Turn Time in Rejoin:: " + currentTurnTimer);
                            //    CallBreak_BidSelectHandle.Inst.itsMyBidTurn = true;
                            //    p.StartRemainingTurnTimer(currentTurnTimer, seatIndex);
                            //    //CallBreak_BidSelectHandle.Inst.StartBidselectionTextTimer(currentTurnTimer);
                            //}
                            //else
                            //{
                            //    p.StartRemainingTurnTimer(currentTurnTimer, seatIndex);
                            //    CallBreak_AllPopupHandler.Instance.OpenCenterMsgToast(" Other player are still Bidding ");
                            //}
                        }
                        else
                        {
                            int powerCard = 0;
                            int CardPower = 0;

                            List<int> card_sequence_list = new List<int>();
                            List<int> spade_card_sequence_list = new List<int>();
                            List<string> thrown_card_ist = new List<string>();
                            foreach (string Scard in CallBreak_SocketEventReceiver.Inst.getRejoinData.turnCurrentCards)
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
                                Debug.Log("Card on table With Power ==>>>" + powerCard); Debug.Log("Card on table:- " + Scard + " With Power ==>>>" + powerCard);
                            }
                            if (cardSequence != "S" && thrown_card_ist.Contains("S"))
                            {
                                Debug.Log(" <color=yellow>Its a Speacial Case found</color>");
                                isSpeacialCase = true;

                                List<CallBreak_MyCard> SuitGroup = new List<CallBreak_MyCard>();
                                SuitGroup = CallBreak_PlayerController.Inst._myCardList.FindAll(x => x.type == cardSequence);
                                if (SuitGroup.Count == 0)
                                {
                                    powerCard = spade_card_sequence_list.Max();
                                    Debug.Log(" Same Suit card not Found High Card is S-" + powerCard);

                                }
                            }

                            Debug.Log("Card on table With Power ==>>>" + powerCard);
                            HideAllPlayerBidToolTip();
                            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex)
                            {
                                if (currentTurnTimer > 14)
                                    CallBreak_GS.Inst.Invoke(nameof(CallBreak_GS.Inst.EnableAllMyCardTrigger), 0.7f);
                                else
                                    CallBreak_GS.Inst.EnableAllMyCardTrigger();
                                CallBreak_GS.Inst.isMyTurn = true;
                                if (cardSequence != "S" && thrown_card_ist.Contains("S"))
                                {
                                    isSpeacialCase = true;
                                    Debug.Log(" <color=yellow>Its a Speacial Case </color>");
                                }
                                HighlightCurrentSuitGroup(cardSequence, powerCard); //======= Need to uncomment
                                if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex && CallBreak_GS.Inst.isMyTurn && MyCardsParent.transform.childCount == 1)
                                {
                                    CallBreak_SocketEventReceiver.Inst.Invoke(nameof(CallBreak_SocketEventReceiver.Inst.AutoThrowCard), 0.1f);
                                }
                            }
                            else
                            {
                                CallBreak_GS.Inst.isMyTurn = false;
                                HighlightCurrentSuitGroup("N", powerCard);  //======= Need to uncomment
                            }
                            if (CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState != "SCOREBOARD_DECLARED" &&
                                CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState != "START_DEALING_CARD")
                            {
                                p.StartRemainingTurnTimer(currentTurnTimer, seatIndex);
                            }
                        }
                    }
                    Debug.Log("<color=Blue>ReJoin Data Set Successfully</color>");
                }
                catch (System.Exception e) { Debug.LogError("Rejoin:" + e.ToString()); }
            }
            catch (System.Exception e) { Debug.LogError(e.ToString()); }
            if (CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "BID_ROUND_TIMER_STARTED" || CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "BID_ROUND_STARTED" ||
                CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "ROUND_STARTED" || CallBreak_SocketEventReceiver.Inst.getRejoinData.tableState == "START_DEALING_CARD")
            {
                Debug.Log("<color=Blue>Display User Hand Cards:</color>" + isAnyCardInHand);
                if (isAnyCardInHand)
                {
                    MyCardsParent.transform.DOScaleX(1, 0.2f);
                }
            }
            ReconnectionPanel.SetActive(false);

        }
        #endregion


        #region player seat and set player data on table after app kill rejoin
        internal void SetPlayerDataAfterAppKillRejoin()
        {

            Debug.Log("<color=Blue>Set App Kill Rejoin Player Data</color>");
            CallBreak_GS.Inst.gameinfo.tabelId = CallBreak_SocketEventReceiver.Inst.getRejoinData.tableId;
            Debug.Log("Table Id::" + CallBreak_GS.Inst.gameinfo.tabelId);
            CallBreak_GS.Inst.userinfo.Current_Table_ID = CallBreak_GS.Inst.gameinfo.tabelId;
            CallBreak_GS.Inst.MaxPlayer = CallBreak_SocketEventReceiver.Inst.getRejoinData.noOfPlayer;
            CallBreak_GS.Inst.ActivePlayer = CallBreak_SocketEventReceiver.Inst.getRejoinData.totalPlayers;
            CallBreak_GS.Inst.userinfo.Player_Seat = CallBreak_SocketEventReceiver.Inst.getRejoinData.seatIndex;
            CallBreak_GS.Inst.gameinfo.potValue = CallBreak_SocketEventReceiver.Inst.getRejoinData.potValue;
            CallBreak_GS.Inst.bootData.bootValue = float.Parse(CallBreak_SocketEventReceiver.Inst.getRejoinData.bootValue);
            CallBreak_GS.Inst.userTurnTimer = CallBreak_SocketEventReceiver.Inst.getRejoinData.userTurnTimer;


            CallBreak_GS.Inst.FinalPointsAmountNegetive = CallBreak_SocketEventReceiver.Inst.getRejoinData.winningScores[0];
            CallBreak_GS.Inst.FinalPointsAmountPositive = CallBreak_SocketEventReceiver.Inst.getRejoinData.winningScores[1];
            CallBreak_GameManager.Inst.RemovePlayerToPlayerList(CallBreak_GS.Inst.MaxPlayer);

            //JSONObject cardArrayList = null;
            for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; i++)
            {
                if (CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].userId != null)
                {
                    if (CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].userId.ToString() == CallBreak_GS.Inst.userinfo.ID)
                    {
                        int seatIndex = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].seatIndex;
                        CallBreak_GameManager.Inst.setseatindex(seatIndex);
                    }
                }
            }
            CallBreak_PlayerController.Inst.SitUserOnExistingTable();
            Debug.Log("<color=Blue>App Kill Rejoin Player Data Set Successfully</color>");
            CallBreak_GS.Inst.isRejoinOrNot = false;
        }
        #endregion

        #region ACKNOWLEDGEMENT_CALLBACKS
        internal void ShowScoreboardAcknowledgement(string ackData)
        {
            Debug.Log("CallBreak_UIManager->ShowScoreboard Acknowledged || ackData : " + ackData);
        }

        internal void LeaveTableAcknowledgement(string ackData)
        {
            Debug.Log("CallBreak_UIManager->LeaveTable Acknowledged || ackData : " + ackData);
        }

        internal void ShowTable_InfoAcknowledgement(string ackData)
        {
            Debug.Log("CallBreak_UIManager->Table_INFO Acknowledged || ackData : " + ackData);

            CallBreak_SocketEventReceiver.Inst.ReceiveDataFromServerEnd(ackData);

        }

        #endregion

        #region All Gameplay reset for New Game
        void ResetAllAfterNewGame()
        {
            isTieBreakerRound = false;
            //ScoreBoardBtn.interactable = false;

            CallBreak_Scoreboard.Inst.winAmountHolderObj.SetActive(false);
            CallBreak_Scoreboard.Inst.gameStausHolder.SetActive(false);
        }
        #endregion
    }
    public enum PreloaderState
    {
        Animate,
        Stop
    }
}

