using UnityEngine;
using ThreePlusGamesCallBreak;
using UnityEngine.UI;
using CallBreak_Socketmanager;

public class CallBreak_BidSelectHandle : MonoBehaviour
{
    #region Variables
    public static CallBreak_BidSelectHandle Inst;
    public Image BidSelectTimer;
    [Header("User UI Interection Elements")]
    public Button[] bidBtn;
    public Sprite activte_Btn;
    public Sprite deactive_Btn;
    public Sprite select_Btn;
    private int timeForBidSelectionText;
    internal bool itsMyBidTurn = false;
    #endregion

    #region Unity callbacks

    private void Awake()
    {
        Inst = this;
    }

    private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

    private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;
    #endregion

    void ResetAllAfterNewGame()
    {

    }

    #region Bid Button Update

    internal void Bid_Button_Update(int maxBid, bool deActive)
    {
        Debug.Log(" Bid Button Update || Bid_Button_Update MaxBID  " + 13 + " DeActive " + deActive);
        if (deActive)
            for (int i = 0; i < maxBid; i++)
                bidBtn[i].GetComponent<Button>().interactable = false;
        else
            for (int i = 0; i < maxBid; i++)
                bidBtn[i].GetComponent<Button>().interactable = true;

    }

    void Deactivate_Btn(int i)
    {
        bidBtn[i].GetComponent<Button>().interactable = false;
        //bidBtn[i].GetComponent<Image>().sprite = deactive_Btn;
        //bidBtn[i].transform.GetChild(0).transform.GetComponent<Text>().color = Color.gray;
    }
    #endregion


    #region Bid Select Button click method
    /// <summary>
    /// on click any bid amount on selecct bid popup this method call
    /// in this method we will send User_Bid event to server with selected bid amount
    /// </summary>
    /// <param name="Value"></param>
    public void SelectBid(int Value)
    {
        Bid_Button_Update(13, true);
        CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.USER_BID(CallBreak_GS.Inst.userinfo.Player_Seat, Value),
            UserBidAcknowledgement, CallBreak_CustomEvents.USER_BID);
    }

    #endregion



    //public void StartBidselectionTextTimer(int _time)
    //{
    //    CancelInvoke(nameof(BidSelectionTextTimer));
    //    timeForBidSelectionText = _time;
    //    InvokeRepeating(nameof(BidSelectionTextTimer), 0, 1f);
    //}
    //internal void BidSelectionTextTimer(string timer)
    //{// "<color=#FEEDA8>Bid</color>"
    //    CallBreak_UIManager.Inst.bidSelectionText.text = "Select  your bid in <color=#00FF07>" + timer + " </color>seconds";//"Select Your Bid In " + timeForBidSelectionText + " Seconds";
    //    //if (timeForBidSelectionText == 0)
    //    //    CancelInvoke(nameof(BidSelectionTextTimer));
    //    //timeForBidSelectionText--;
    //}



    #region ACKNOWLEDGEMENT_CALLBACKS
    internal void UserBidAcknowledgement(string ackData)
    {
        Debug.Log("CallBreak_BidSelectHandle->UserBid Acknowledged || ackData : " + ackData);
        //CallBreak_UIManager.Inst.CommonPreloaderPanel.enabled = false;
    }

    #endregion
}
