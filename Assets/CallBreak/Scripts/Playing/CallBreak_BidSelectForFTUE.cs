using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreePlusGamesCallBreak;
using DG.Tweening;
using UnityEngine.UI;
using CallBreak_Socketmanager;
using System;

public class CallBreak_BidSelectForFTUE : MonoBehaviour
{
    #region Variables
    public static CallBreak_BidSelectForFTUE Inst;
    public Image BidSelectTimer;
    public GameObject[] bidButtons;
    public Sprite GreenBidButton, NormalBidButton;
    #endregion
    #region Unity callbacks

    private void Awake()
    {
        Inst = this;
        HighlightAnyBidButton(6);
    }


    private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

    private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;
    #endregion

    void ResetAllAfterNewGame()
    {
        for (int i = 0; i < bidButtons.Length; i++)
        {
            bidButtons[i].GetComponent<Image>().sprite = NormalBidButton;
        }
    }

    #region Bid Select Button click method
    /// <summary>
    /// on click any bid amount on selecct bid popup this method call
    /// in this method we will send User_Bid event to server with selected bid amount
    /// </summary>
    /// <param name="Value"></param>
    public void SelectBid(int Value)
    {
        CallBreak_UIManager.Inst.BidSelectPopupContent.GetComponent<RectTransform>().DOScale(0, 0.5f).OnComplete(() =>
        {
            CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = false;
        });

        CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.USER_BID(CallBreak_GS.Inst.userinfo.Player_Seat, Value),
            UserBidAcknowledgement, CallBreak_CustomEvents.USER_BID);
        
    }
    #endregion

    internal void HighlightAnyBidButton(int index)
    {
        for (int i = 0; i < bidButtons.Length; i++)
        {
            if (i == index)
                bidButtons[i].GetComponent<Image>().sprite = GreenBidButton;
            else
                bidButtons[i].GetComponent<Image>().sprite = NormalBidButton;
        }
    }

 
   
    #region ACKNOWLEDGEMENT_CALLBACKS
    internal void UserBidAcknowledgement(string ackData)
    {
        Debug.Log("CallBreak_BidSelectHandle->UserBid Acknowledged || ackData : " + ackData);
    }

    #endregion
}
