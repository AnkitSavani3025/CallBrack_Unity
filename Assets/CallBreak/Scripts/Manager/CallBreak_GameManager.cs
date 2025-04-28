using DG.Tweening;
using ThreePlusGamesCallBreak;
using System.Collections.Generic;
using UnityEngine;
using CallBreak_Socketmanager;
using UnityEngine.UI;

public class CallBreak_GameManager : MonoBehaviour
{
    #region variables
    public static CallBreak_GameManager Inst;
    [Tooltip("all player list")]
    public List<CallBreak_Users> player_list;
    [Tooltip("active player list")]
    public List<CallBreak_Users> ActivePlayerList;
    public Text fpsText;
   
    #endregion Disconnection_issue



    #region Unity callbacks
    private void Awake()
    {
        Inst = this;
        InvokeRepeating(nameof(SetFpsText), 0.1f, 0.5f);        
    }

    private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

    private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;
    #endregion
    public float deltaTime, fps;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
    }
    void SetFpsText() => fpsText.text = Mathf.Ceil(fps).ToString();



    #region Get Player by using its seatIndex
    internal CallBreak_Users GetPlayerSeatIndex(int id)
    {
        CallBreak_Users p = null;
        for (var i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
        {
            if (id == CallBreak_PlayerController.Inst.playerList[i].SeatIndex)
            {
                p = CallBreak_PlayerController.Inst.playerList[i];
            }
        }
        return p;  //return player match with seatIndex pass in argument
    }
    #endregion

    #region Set self player seatIndex
    internal void SetMySeatIndex()
    {
        for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats.Count; i++)
        {
            if (CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats[i].userId != null)
            {
                string id = CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats[i].userId;

                if (id == CallBreak_GS.Inst.userinfo.ID) //seat self player at bottom 
                {
                    CallBreak_GS.Inst.userinfo.UserSeatIndex = CallBreak_SocketEventReceiver.Inst.signUpResponseHandler.data.GAME_TABLE_INFO.seats[i].seatIndex;
                }
            }
        }
    }
    #endregion

    #region Set self player seatIndex on appkill rejoin game
    internal void SetMyRejoinSeatIndex()
    {
        for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail.Count; i++)
        {

            if (CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].userId != null)
            {
                string id = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].userId.ToString();
                if (id == CallBreak_GS.Inst.userinfo.ID)//set seat index for self player to seat at bottom
                {
                    CallBreak_GS.Inst.userinfo.UserSeatIndex = CallBreak_SocketEventReceiver.Inst.getRejoinData.playarDetail[i].seatIndex;
                }
            }
        }
    }
    #endregion

    #region Set all player seatIndex
    internal void setseatindex(int _selfPlayerSi)
    {
        try
        {
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                int val = _selfPlayerSi + i; //set seatindex like (if self player seat index is 0 then set 0,1,2,3 or self player seat index is 2 then 3,0,1,2 like that)

                if (val >= CallBreak_PlayerController.Inst.playerList.Count)
                {
                    val = val - CallBreak_PlayerController.Inst.playerList.Count;
                }
                CallBreak_PlayerController.Inst.playerList[i].SeatIndex = val;
                Debug.Log("<color=Green>" + CallBreak_PlayerController.Inst.playerList[i].name + " SeatIndex:</color>" + val);
            }
        }
        catch (System.Exception e) { Debug.LogError(e); }
    }
    #endregion



    internal void StopTimerAndMakePlayerINWatingState(int playerCount)
    {
        Debug.Log("Making Player Again in Wating State");
        //CallBreak_UIManager.Inst.CancelInvoke(nameof(CallBreak_UIManager.Inst.RTS_Timer));

        //CallBreak_UIManager.Inst.OpenCenterMsgToast(" Game will start when all " + playerCount + " players  join ");
        CallBreak_AllPopupHandler.Instance.OpenCenterMsgToast(" Game will start when all " + playerCount + " players  join ");
    }

    #region Remove Player from player list and add new player
    public void RemovePlayerToPlayerList(int index)
    {
        //clear player list before we add player
        CallBreak_PlayerController.Inst.playerList.Clear();

        //add empty player list, we will store player
        for (int i = 0; i < 4; i++)
        {
            CallBreak_PlayerController.Inst.playerList.Add(CallBreak_UIManager.Inst.PlayerHolders[i].GetComponent<CallBreak_Users>());
        }

        if (index.Equals(2))
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == 1 || i == 2)
                {
                    CallBreak_PlayerController.Inst.playerList[i].transform.localScale = Vector3.zero;
                    CallBreak_PlayerController.Inst.playerList[i - 1].transform.localScale = Vector3.one;
                    CallBreak_PlayerController.Inst.playerList.RemoveAt(i);
                }
            }
        }
        else if (index.Equals(4))
            for (int i = 0; i < 4; i++)
                CallBreak_PlayerController.Inst.playerList[i].transform.localScale = Vector3.one;
    }
    #endregion

    #region Reset All Gameplay
    void ResetAllAfterNewGame()
    {
        player_list.Clear(); ActivePlayerList.Clear();
    }
    #endregion

   
}
