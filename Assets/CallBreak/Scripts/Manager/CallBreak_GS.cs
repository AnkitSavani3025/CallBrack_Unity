using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_GS : MonoBehaviour
    {
        #region variables
        public static CallBreak_GS Inst;

        internal UserData userinfo;
        internal GameData gameinfo;
        public BootData bootData;

        internal int HandCollectCountFTUE = 0;
        internal string roundTableId = "";

        [Tooltip("User turn time in seconds")]
        internal int userTurnTimer, RoundNo;

        public int MaxPlayer, ActivePlayer, MaxRound;

        internal int CurrentRound = 0, bv;
        public bool collectBootAnimation = false;
        public bool isSocketClosed = false, isMyTurn = false, NotAnyCardWithSuit = false, canDisplayRoundTimerinScoreBoard = false;
        internal int EntryFreeAmount, FinalPointsAmountNegetive, FinalPointsAmountPositive;
        internal bool isMyCardThrowResDone = false, isBootCollected = false;

        [Tooltip("scoreboard screen next and previous button click flag")]
        internal bool NextPanelClick = false, PreviousPanelClick = false;

        //[Tooltip("flags for check reconnection case")]
        //internal bool isDisconnected = false, isUserComeFromBG = false;

        [Tooltip("to check is player rejoin after app kill")]
        internal bool /*isAppKillRejoin = true,*/ fromBack = false, isAlredyPlaying = false, isRejoinOrNot = false;//isMinimize = false ,fromBack = false;

        public string isInernet = "wifi";

        internal bool canDisplayPreloader = true, StopBidTurnTimer = false, isWinnerDeclared = false, isSelfUserInactive = false;//HideReconnectionLoader = false

        internal bool isGameEnded = false;
        internal float userWalletBalance;
        internal CurrentTableState CurrentTableState;

        [Tooltip(" Downloaded Sprite List ")]
        public List<Sprite> downloadedSpriteList;


        #endregion

        #region Unity callbacks
        private void Awake()
        {
            Inst = this;
            Input.multiTouchEnabled = false;
        }

        private void Start()
        {
            userinfo = new UserData();
            gameinfo = new GameData();
            bootData = new BootData();

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    isInernet = "MobileData";
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    isInernet = "wifi";
                }
            }
        }
        private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

        private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;

        #endregion

        #region Disable User tap on cards
        internal void DisableAllMyCardTrigger()
        {
            //  Debug.Log("DisableAllMyCardTrigger");
            for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
            {
                CallBreak_PlayerController.Inst._myCardList[i].MyCardThrowd = true;
            }
        }
        #endregion
        #region Enable User tap on cards
        internal void EnableAllMyCardTrigger()
        {
            if (isMyTurn)
            {
                for (int i = 0; i < CallBreak_PlayerController.Inst._myCardList.Count; i++)
                {
                    CallBreak_PlayerController.Inst._myCardList[i].MyCardThrowd = false;
                }
            }
        }
        #endregion

        internal void ResetAllAfterNewGame()
        {
            userinfo = new UserData();
            gameinfo = new GameData();
            bootData = new BootData();
            isMyTurn = false;
            NotAnyCardWithSuit = false;
            collectBootAnimation = false;
            canDisplayRoundTimerinScoreBoard = false; /*isAppKillRejoin = true;*/  isRejoinOrNot = false; // isMinimize = false; 
            isMyCardThrowResDone = false; NextPanelClick = false; PreviousPanelClick = false;
            canDisplayPreloader = true; StopBidTurnTimer = false; isWinnerDeclared = false; isSelfUserInactive = false;
            //HideReconnectionLoader = false;

            MaxPlayer = ActivePlayer = EntryFreeAmount = FinalPointsAmountNegetive = FinalPointsAmountPositive = userTurnTimer = RoundNo = 0;
            CurrentRound = bv = CallBreak_AllPopupHandler.Instance.rtsTimer = 0;
            roundTableId = "";
        }
    }
    #region user data class that contain all data of user
    public class UserData
    {
        public string ID;
        public string session_id;
        public string Name;
        public int UserSeatIndex;
        public string UserType;
        public string PicUrl;
        public string PinCode;
        public string State;

        public string Current_Table_ID;
        public int Player_Seat;
        public int PlayerSeat_Index;

        public UserData()
        {
            ID = ("-" + 1.ToString());
            Name = "";
            UserSeatIndex = -1;
            Current_Table_ID = "";
            Player_Seat = -1;
            PlayerSeat_Index = 0;
            PinCode = "";
            State = "";
        }
    }
    #endregion

    #region game data class that contain all data of game
    public class GameData
    {
        public string gameId;
        public float potValue;
        public float WinAmount;
        public string tabelId;
        public string modeId;
        public string gameTypeId;
        public List<BootData> bootInfo;
        public GameData()
        {
            gameId = "";
            potValue = 0;
            tabelId = "";
            modeId = "";
            gameTypeId = "";
            bootInfo = new List<BootData>();
        }
    }
    #endregion

    #region boot data class contain game boot details
    public class BootData
    {
        public float bootValue;
        public string bootID;

        public BootData()
        {
            bootValue = 0;
            bootID = "";
        }
    }
    #endregion
    public enum CurrentTableState
    {
        Waiting,
        GameStarted,
        none,
    }
}
