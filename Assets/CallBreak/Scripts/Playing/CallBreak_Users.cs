using DG.Tweening;
using ThreePlusGamesCallBreak;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CallBreak_Users : MonoBehaviour
{
    #region Variables
    public static CallBreak_Users Inst;
    [Tooltip("User name")]
    public Text UserName, BidAndHandValue, MyScore;
    [Tooltip("User seated index and his/her bid value")]
    public int SeatIndex, MyBidValue, MyHandCollect;
    [Tooltip("Player Id")]
    public string PlayerID;
    public int playerSitedWithIndex, CardRotationOnDeal;
    public CallBreak_SeatStatus SeatStatus = CallBreak_SeatStatus.Empty;

    public bool isSitIndexProvided = false, MyBidSelected = false, isMyTurn;

    public float time;

    [Tooltip("User turn timer image refrence")]
    [SerializeField]
    internal Image TimerBG;
    [SerializeField] Text turnTimer;
    [SerializeField] Color turnTimer_TextColor;
    [SerializeField] Color turnEndTimer_TextColor;
    [SerializeField] Image greenDot;
    [SerializeField] RectTransform green_Dot_Holder;

    // Borders // dark image for text Better View 
    public Image goldBorder;
    public Image darkImageForBetterView;
    //==
    [Tooltip("User profile picture")]
    public CallBreak_IMGLoader PlayerProfile;
    [Tooltip("User Bid tooltip BG, Dealer position and dealer icon")]
    public GameObject BidBG, DealerPos, DealerIcon, ProfileGlow, Inactive;

    [Tooltip("contain user turn timer IEnumerator method")]
    internal IEnumerator timerCo, timerCountTextCo;

    #endregion

    #region Unity callbacks
    private void Awake()
    {
        Inst = this;
    }
    private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

    private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;


    #endregion



    #region call User turn timer
    /// <summary>
    /// start user turn animation on profile when we rechive user turn statred response from server
    /// </summary>
    /// <param name="seatIndex"></param>
    internal void StartTurnTimer(int seatIndex)
    {
        Debug.Log(" Start Turn Timer    ");
        if (this.timerCo != null)
            StopCoroutine(this.timerCo);
        this.timerCo = UserTurnFillAmount(CallBreak_GS.Inst.userTurnTimer, seatIndex);   //userTurnTimer
        darkImageForBetterView.enabled = true;
        StartCoroutine(this.timerCo);

        if (this.timerCountTextCo != null)
            StopCoroutine(this.timerCountTextCo);
        this.timerCountTextCo = UserTurnTimerText(int.Parse(CallBreak_GS.Inst.userTurnTimer.ToString()));   //userTurnTimer
        StartCoroutine(this.timerCountTextCo);


    }
    #endregion   


    #region User turn timer animation
    /// <summary>
    /// user turn animation on profile
    /// decrease image fill amount by time
    /// </summary>
    /// <param name="UserTurnTime"></param>
    /// <param name="seatIndex"></param>
    /// <returns></returns>
    IEnumerator UserTurnFillAmount(float UserTurnTime, int seatIndex)
    {
        Debug.Log(" UserTurnFillAmount");
        greenDot.sprite = CallBreak_UIManager.Inst.greenDot;
        TimerBG.sprite = CallBreak_UIManager.Inst.timerImage;
        TimerBG.transform.localScale = Vector3.one;
        goldBorder.transform.localScale = Vector3.zero;
        time = UserTurnTime + 1.0f;
        TimerBG.fillAmount = 1;
        isMyTurn = true;
        turnTimer.color = turnTimer_TextColor;
        //turnTimer.text = UserTurnTime.ToString();
        //CallBreak_UIManager.Inst.bidSelectionText.text = "Select  your bid in <color=#00FF07>" + UserTurnTime + " </color>seconds";//"Select Your Bid In " + timeForBidSelectionText + " Seconds";

        float fillamount = 0.1f / UserTurnTime;
        float BlockAmount = 1 / UserTurnTime;
        float last_0_5_Sec = BlockAmount / 2;
        // UserTurnTime += 1;

        while (TimerBG.fillAmount > 0)
        {
            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex)
                if (TimerBG.fillAmount <= last_0_5_Sec)
                    CallBreak_GS.Inst.DisableAllMyCardTrigger();

            if (TimerBG.fillAmount < 0.5f)
            {
                greenDot.sprite = CallBreak_UIManager.Inst.redDot;
                TimerBG.sprite = CallBreak_UIManager.Inst.redTimerImage;
                turnTimer.color = turnEndTimer_TextColor;
            }

            TimerBG.fillAmount -= fillamount;
            green_Dot_Holder.rotation = Quaternion.Euler(new Vector3(0, 0, TimerBG.fillAmount * 360));

            //UserTurnTime -= 0.1f;
            //string timer = "";
            //if (UserTurnTime > 10)
            //    timer = UserTurnTime.ToString().Substring(0, 2);
            //else
            //    timer = UserTurnTime.ToString().Substring(0, 1);


            //// if (CallBreak_BidSelectHandle.Inst.itsMyBidTurn)
            //CallBreak_UIManager.Inst.bidSelectionText.text = "Select  your bid in <color=#00FF07>" + timer + " </color>seconds";//"Select Your Bid In " + timeForBidSelectionText + " Seconds";
            //turnTimer.text = timer;


            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
    #region call User turn timer after rejoin
    /// <summary>
    /// user turn animation call from here when user rejoin 
    /// </summary>
    /// <param name="RemainingTime"></param>
    /// <param name="seatIndex"></param>
    internal void StartRemainingTurnTimer(float RemainingTime, int seatIndex)
    {
        //Debug.LogError(" Darkimage for Better view Are eneble true ");
        if (this.timerCo != null)
            StopCoroutine(this.timerCo);
        this.timerCo = UserTurnFillAmountRejoin(RemainingTime, seatIndex);   //userTurnTimer
        StartCoroutine(this.timerCo);

        if (this.timerCountTextCo != null)
            StopCoroutine(this.timerCountTextCo);
        this.timerCountTextCo = UserTurnTimerText(int.Parse(RemainingTime.ToString()));   //userTurnTimer
        StartCoroutine(this.timerCountTextCo);
    }
    #endregion

    #region User turn timer animation
    /// <summary>
    /// user turn animation on profile for rejoin
    /// calculate user remaning turn time and depends on it set image fill amount first
    /// decrease image fill amount by time
    /// </summary>
    /// <param name="UserTurnTime"></param>
    /// <param name="seatIndex"></param>
    /// <returns></returns>
    IEnumerator UserTurnFillAmountRejoin(float UserTurnTime, int seatIndex)
    {

        //calculate remaning turn time on rejoin
        darkImageForBetterView.enabled = true;
        greenDot.sprite = CallBreak_UIManager.Inst.greenDot;
        TimerBG.sprite = CallBreak_UIManager.Inst.timerImage;
        TimerBG.transform.localScale = Vector3.one;
        darkImageForBetterView.enabled = true;
        goldBorder.transform.localScale = Vector3.zero;
        float fillamount = 1f / CallBreak_GS.Inst.userTurnTimer;
        fillamount = fillamount * UserTurnTime;
        turnTimer.color = turnTimer_TextColor;

        TimerBG.fillAmount = fillamount;  //set remaining turn time
        float Fill = 0.1f / CallBreak_GS.Inst.userTurnTimer;
        float BlockAmount = 1 / CallBreak_GS.Inst.userTurnTimer;
        float last_0_5_Sec = BlockAmount / 2;
        //UserTurnTime += 1;
        while (TimerBG.fillAmount > 0)
        {
            if (CallBreak_GS.Inst.userinfo.Player_Seat == seatIndex)
                if (TimerBG.fillAmount <= last_0_5_Sec) CallBreak_GS.Inst.DisableAllMyCardTrigger();


            if (TimerBG.fillAmount < 0.5f)
            {
                greenDot.sprite = CallBreak_UIManager.Inst.redDot;
                TimerBG.sprite = CallBreak_UIManager.Inst.redTimerImage;
                turnTimer.color = turnEndTimer_TextColor;

            }

            TimerBG.fillAmount -= Fill;
            green_Dot_Holder.rotation = Quaternion.Euler(new Vector3(0, 0, TimerBG.fillAmount * 360));

            //UserTurnTime -= 0.1f;
            //string timer = "";
            //if (UserTurnTime > 10)
            //    timer = UserTurnTime.ToString().Substring(0, 2);
            //else
            //    timer = UserTurnTime.ToString().Substring(0, 1);


            //if (CallBreak_BidSelectHandle.Inst.itsMyBidTurn)
            //    CallBreak_UIManager.Inst.bidSelectionText.text = "Select  your bid in <color=#00FF07>" + timer + " </color>seconds";//"Select Your Bid In " + timeForBidSelectionText + " Seconds";
            //turnTimer.text = timer;


            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
    IEnumerator UserTurnTimerText(int _timer)
    {
        while (_timer > -1)
        {
            Debug.Log(" Timer  " + _timer);
            if (CallBreak_BidSelectHandle.Inst.itsMyBidTurn)
                CallBreak_UIManager.Inst.bidSelectionText.text = "Select your bid in <color=#00FF07>" + _timer + " </color>seconds";//"Select Your Bid In " + timeForBidSelectionText + " Seconds";
            turnTimer.text = _timer.ToString();

            _timer--;

            yield return new WaitForSeconds(1f);
        }

    }
    /// <summary>
    /// stop ongoing user turn timer
    /// </summary>
    internal void StopTurnTimer()
    {
        TimerBG.transform.localScale = Vector3.zero;
        darkImageForBetterView.enabled = false;
        isMyTurn = false;
        if (timerCo != null)
        {
            StopCoroutine(this.timerCo);
        }
        if (timerCountTextCo != null)
        {
            StopCoroutine(this.timerCountTextCo);
        }
    }

    #region make User InActive
    /// <summary>
    /// make user inactive
    /// set dark profile on user profile and set inactive tag on profile with specifice message
    /// </summary>
    /// <param name="msg"></param>
    internal void MakeInActive(string msg)
    {
        Inactive.transform.DOScale(1, 0);
        darkImageForBetterView.enabled = true;
        Inactive.transform.GetChild(0).gameObject.transform.GetComponent<Text>().text = msg;
        Color GlowColor = ProfileGlow.transform.GetComponent<Image>().color;
        GlowColor.a = 0.5f;
        ProfileGlow.transform.GetComponent<Image>().color = GlowColor;

    }
    #endregion

    #region Make User Active
    /// <summary>
    /// when user come back on game make him active again
    /// remove his dark profile and enable glow back to normal
    /// </summary>
    internal void MakeActive()
    {
        darkImageForBetterView.enabled = false;
        Inactive.transform.DOScale(0, 0);
        Color GlowColor = ProfileGlow.transform.GetComponent<Image>().color;
        GlowColor.a = 1f;
        ProfileGlow.transform.GetComponent<Image>().color = GlowColor;
    }
    #endregion

    #region User Complately Left the Game
    /// <summary>
    /// when user Complately Left from game and there is no any chances for back then remove that player and unload his profile pic
    /// Complately Left User not able to join back game
    /// </summary>
    internal void PlayerLeft()
    {
        //  MyDeatailHolder.transform.localScale = Vector3.zero;
        PlayerProfile.UnLoadIMG();
    }
    #endregion

    #region After User Left Make Seat Empty
    /// <summary>
    /// when user Complately Left from game make those seat empty and available for other
    /// </summary>
    /// <param name="SeatIndex"></param>
    internal void MakeSeatEmpty(int SeatIndex)
    {
        for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
        {
            if (CallBreak_PlayerController.Inst.playerList[i].playerSitedWithIndex.Equals(SeatIndex))
            {
                CallBreak_PlayerController.Inst.playerList[i].SeatStatus = CallBreak_SeatStatus.Empty;
            }
        }
    }
    #endregion

    public void ResetAllAfterNewGame()
    {
        DealerIcon.transform.localScale = Vector3.zero;
        UserName.text = "";
        BidAndHandValue.text = "0/0";
        MyScore.text = "0";
        playerSitedWithIndex = SeatIndex = -1;
        MyBidValue = MyHandCollect = 0;
        PlayerID = null;
        BidBG.transform.localScale = Vector3.zero;
        SeatStatus = CallBreak_SeatStatus.Empty;
        isSitIndexProvided = false; MyBidSelected = false;
        PlayerLeft();
        Inactive.transform.DOScale(0, 0);
        transform.DOScale(0, 0);
        PlayerProfile.UnLoadIMG();

    }
    /// <summary>
    /// IF player Leave table Before Lock in period Than Make Player Seat Empty Than Other Player In Waiting state 
    /// </summary>
    public void MakeEmptyUserdata()
    {
        DealerIcon.transform.localScale = Vector3.zero;
        UserName.text = "";
        BidAndHandValue.text = "0/0";
        MyScore.text = "0";
        PlayerProfile.UnLoadIMG();
    }
}

public enum CallBreak_SeatStatus
{
    Empty,
    Allowed
}
