using ThreePlusGamesCallBreak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallBreak_Socketmanager;
using DG.Tweening;
public class CallBreak_PlayerController : MonoBehaviour
{
    #region Variables
    public static CallBreak_PlayerController Inst;

    [Tooltip("playerList contain all player")]
    public List<CallBreak_Users> playerList;
    [Tooltip("_myCardList contain all cards of this player")]
    internal List<CallBreak_MyCard> _myCardList;// = new List<CallBreak_MyCard>();
    public static char[] trim_char_arry = new char[] { '"' };
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
        _myCardList = new List<CallBreak_MyCard>();
    }

    #region methods for player's sit on table  
    /// <summary>
    /// first reset all player seat index 
    /// then set self user seat index
    /// and depends on it set other player seat index
    /// all those methods call from here
    /// </summary>
    internal void SitUserOnExistingTable()
    {
        try
        {
            ResetSitOnExistingTable();
            if (CallBreak_GS.Inst.isRejoinOrNot)
            {
                CallBreak_GameManager.Inst.SetMyRejoinSeatIndex(); // just Find Self Player SI and store in GS 
            }
            else
            {
                CallBreak_GameManager.Inst.SetMySeatIndex();// just Find Self Player SI and store in GS 
            }
            SetOtherPlayerSitPositionBasedOnMySeatIndex(); // Assign other player SI like Backend 
        }
        catch (System.Exception ex)
        {
            Debug.Log("Sit on existing Table Error>>>>>>" + ex.ToString());
        }
    }
    #endregion

    #region Make Seat Empty
    internal void ResetSitOnExistingTable()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].isSitIndexProvided = false;
            playerList[i].SeatStatus = CallBreak_SeatStatus.Empty;
        }
    }
    #endregion

    #region Set Other Player Seat Position Based On Self player SeatIndex
    /// <summary>
    /// depends on self user seat index we assign all other player seat index for our use 
    /// </summary>
    internal void SetOtherPlayerSitPositionBasedOnMySeatIndex()
    {
        int sitToSkip = playerList.Count - CallBreak_GS.Inst.userinfo.UserSeatIndex;

        int sit = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (i >= sitToSkip)
            {
                playerList[i].playerSitedWithIndex = sit;
                playerList[i].isSitIndexProvided = true;
                sit++;
            }
        }
        int newCount = playerList.Count - sit;

        for (int j = 0; j < newCount; j++)
        {
            if (!playerList[j].isSitIndexProvided)
            {
                playerList[j].playerSitedWithIndex = sit;
                playerList[j].isSitIndexProvided = true;
                sit++;
            }

        }



        if (CallBreak_GS.Inst.isRejoinOrNot)
        {
            for (int k = 0; k < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; k++)
            {
                if (CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[k].userId != null)
                {
                    SetAndSitPlayerOnRejoinTable(CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[k]);
                }
            }
        }
        else
        {
            for (int k = 0; k < CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats.Count; k++)
            {
                if (CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats[k].userId != null)
                {
                    SetAndSitPlayerOnTable(CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats[k]);
                }
            }
        }
    }
    #endregion

    #region set player data and sit on table
    /// <summary>
    /// depends on assign seat index seat player on table and assign player details
    /// </summary>
    /// <param name="Users"></param>
    internal void SetAndSitPlayerOnTable(Seat Users)
    {
        Debug.Log("SetAndSitPlayerOnTable" + Users.seatIndex);
        int seatIndex = Users.seatIndex;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerSitedWithIndex.Equals(seatIndex) && playerList[i].SeatStatus.Equals(CallBreak_SeatStatus.Empty))
            {

                try
                {
                    playerList[i].ProfileGlow.transform.localScale = Vector3.one;
                    playerList[i].PlayerID = Users.userId.ToString();
                    Debug.Log(playerList[i].PlayerID);
                    playerList[i].UserName.text = Users.username;
                    string ImgUrl = Users.profilePicture;
                    playerList[i].PlayerProfile.LoadIMG(ImgUrl, false);
                    playerList[i].MakeActive();
                    playerList[i].SeatStatus = CallBreak_SeatStatus.Allowed;

                    CallBreak_GameManager.Inst.ActivePlayerList.Add(playerList[i]);
                }
                catch (System.Exception eop)
                {
                    Debug.Log("JT Error>>>>>>>>>" + eop);
                }
            }

        }

    }
    #endregion

    #region set player data and sit on table for Rejoin
    /// <summary>
    /// Set And Sit Player On Table and set its details
    /// on rejoin response seat all available plaer on table
    /// </summary>
    /// <param name="Users"></param>
    internal void SetAndSitPlayerOnJoinTable(PlayarDetail Users)
    {
        int seatIndex = Users.seatIndex;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerSitedWithIndex.Equals(seatIndex) && playerList[i].SeatStatus.Equals(CallBreak_SeatStatus.Empty))
            {
                try
                {
                    Debug.Log("Set Player Data Here");
                    playerList[i].ProfileGlow.transform.localScale = Vector3.one;
                    playerList[i].PlayerID = Users.userId.ToString();
                    Debug.Log(playerList[i].PlayerID);
                    Debug.Log("ID:" + playerList[i].PlayerID + " Name:" + Users.username);
                    playerList[i].UserName.text = Users.username;
                    string ImgUrl = Users.profilePicture;
                    playerList[i].PlayerProfile.LoadIMG(ImgUrl, false);
                    playerList[i].MakeActive();
                    playerList[i].SeatStatus = CallBreak_SeatStatus.Allowed;
                    CallBreak_GameManager.Inst.ActivePlayerList.Add(playerList[i]);
                }
                catch (System.Exception eop)
                {
                    Debug.Log("JT Error>>>>>>>>>" + eop);
                }
            }

        }
        //if (CallBreak_FTUEHandler.isFTUE && !CallBreak_FTUEHandler.isFTUESelfPlay)
        //{
        //    if (CallBreak_GameManager.Inst.ActivePlayerList.Count == 4)
        //    {
        //        Debug.Log("1212");
        //        CallBreak_UIManager.Inst.CloseCenterMsgToast();
        //        CallBreak_FTUEHandler.Inst.DisplayStepToolTip();
        //        for (int i = 0; i < CallBreak_UIManager.Inst.PlayerHolders.Length; i++)
        //        {
        //            CallBreak_UIManager.Inst.PlayerHolders[i].transform.localScale = Vector3.one;
        //        }
        //    }
        //}
    }
    #endregion

    #region seat player on rejoin
    /// <summary>
    /// if user goes on backgraound when less then 4 user on table at a same time any user join then on rejoin seat 4th user on table and set player detail
    /// </summary>
    internal void SeatPlayerOnRejoin()
    {
        for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; i++)
        {
            for (int j = 0; j < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; j++)
            {
                if (playerList[i].playerSitedWithIndex.Equals(CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[j].seatIndex) &&
                    playerList[i].SeatStatus.Equals(CallBreak_SeatStatus.Empty))
                {
                    try
                    {
                        playerList[i].ProfileGlow.transform.localScale = Vector3.one;
                        playerList[i].PlayerID = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[j].userId.ToString();
                        Debug.Log(playerList[i].PlayerID);
                        Debug.Log("ID:" + playerList[i].PlayerID + " Name:" + CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[j].username);
                        playerList[i].UserName.text = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[j].username;
                        string ImgUrl = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[j].profilePicture;
                        playerList[i].PlayerProfile.LoadIMG(ImgUrl, false);
                        playerList[i].MakeActive();
                        playerList[i].SeatStatus = CallBreak_SeatStatus.Allowed;
                        CallBreak_GameManager.Inst.ActivePlayerList.Add(playerList[i]);
                        CallBreak_GS.Inst.ActivePlayer = CallBreak_GameManager.Inst.ActivePlayerList.Count;
                    }
                    catch (System.Exception eop)
                    {
                        Debug.Log("Seat Player on bg/fb rejoin error::>>>>>>>>>" + eop);
                    }
                }
                else
                {

                }
            }
        }
        List<int> AvailableUser = new List<int>();
        for (int k = 0; k < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; k++)
        {
            if (CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[k].userId != null)
            {
                AvailableUser.Add(CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[k].seatIndex);
            }
        }
        bool LeftUser = false;
        for (int j = 0; j < CallBreak_SocketEventReceiver.Inst.getRejoinData.noOfPlayer; j++)
        {
            for (int l = 0; l < AvailableUser.Count; l++)
            {
                Debug.Log(playerList[j].playerSitedWithIndex + " " + AvailableUser[l]);
                if (playerList[j].playerSitedWithIndex == AvailableUser[l])
                {
                    LeftUser = false;
                    break;
                }
                else
                {
                    LeftUser = true;
                }
            }
            if (LeftUser)
            {
                LeftUser = false;
                CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(playerList[j].playerSitedWithIndex);
                p.PlayerLeft();
                p.MakeSeatEmpty(playerList[j].playerSitedWithIndex);
            }
        }
        AvailableUser.Clear();

    }
    #endregion

    #region seat player on available(Empty) seat like join table for rejoin
    internal void SetAndSitPlayerOnRejoinTable(PlayarRoundDetail Users)
    {
        int seatIndex = Users.seatIndex;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerSitedWithIndex.Equals(seatIndex) && playerList[i].SeatStatus.Equals(CallBreak_SeatStatus.Empty))
            {
                try
                {
                    playerList[i].ProfileGlow.transform.localScale = Vector3.one;
                    playerList[i].PlayerID = Users.userId.ToString();
                    Debug.Log(playerList[i].PlayerID);
                    playerList[i].UserName.text = Users.username;
                    string ImgUrl = Users.profilePicture;
                    playerList[i].PlayerProfile.LoadIMG(ImgUrl, false);
                    playerList[i].MakeActive();
                    playerList[i].SeatStatus = CallBreak_SeatStatus.Allowed;
                    CallBreak_GameManager.Inst.ActivePlayerList.Add(playerList[i]);
                }
                catch (System.Exception eop)
                {
                    Debug.Log("JT Error>>>>>>>>>" + eop);
                }
            }
        }
    }
    #endregion
    internal void ResetAllUserDetailsWhenAppBackground()
    {
        for (int i = 0; i < playerList.Count; i++) playerList[i].ResetAllAfterNewGame();
        playerList.Clear(); CallBreak_GS.Inst.ResetAllAfterNewGame(); CallBreak_GameManager.Inst.ActivePlayerList.Clear();
        CallBreak_CardDeal.Inst.DisCardedCardDestroyImmediate();
    }
}