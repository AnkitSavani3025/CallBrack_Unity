using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThreePlusGamesCallBreak;
using CallBreak_Socketmanager;

public class CallBreak_Scoreboard : MonoBehaviour
{
    public static CallBreak_Scoreboard Inst;
    [Header("Player Profile Refrences")]
    [SerializeField] CallBreak_IMGLoader[] players_Profile;
    //[SerializeField] List<Image> ProfilePics;

    [SerializeField] GameObject[] AllPlayer, scoreRecord_Mask, playerStatusList;
    [SerializeField] Text[] AllPlayerTotalScore;
    [SerializeField] Text[] WinningAmount;

    [SerializeField] Text scoreBoardGameIdTxt, statusTxt, statusTitletxt;


    [Header("Score Board UI Refrences")]
    [SerializeField] internal GameObject playerProfileHolder, totalAmountHolderObj, winAmountHolderObj, maskHolderObj, gameStausHolder;

    [SerializeField] internal GameObject scoreBoardCloseBtn;//, scoreBoardExitBtn;
    [SerializeField] internal Transform scoreboardContent, scoreBoardScrolView;



    [Header("Scoreboard Round Content")]
    [SerializeField] GameObject round_Data_Prefab;

    [SerializeField] Color WinPlayerTextColor;
    [SerializeField] Color MyPlayerTextColor;

    [SerializeField] GameObject localUser;

    [SerializeField] internal int roundStatusTime, totalScorePanelHeight, winAmountHeight, scoreboardContentint, playerProfileHolderInt;

    private void Awake()
    {
        Inst = this;
        scoreboardContent.GetComponent<LayoutElement>().minHeight = scoreboardContent.parent.parent.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < round_Data_Prefab.transform.childCount; i++)
            round_Data_Prefab.transform.GetChild(i).gameObject.SetActive(true);
    }
    private void Start()
    {
        for (int i = 0; i < CallBreak_GS.Inst.MaxPlayer; i++)
            AllPlayer[i].transform.DOScale(1, 0);
    }
    private void OnEnable()
    {
        CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;
    }
    private void OnDisable()
    {
        CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;
    }
    internal void FirstSetupScoreBoard()
    {
        scoreboardContent.GetComponent<LayoutElement>().minHeight = scoreboardContent.parent.parent.GetComponent<RectTransform>().rect.height;
        if (CallBreak_GS.Inst.MaxPlayer == 2)
        {
            for (int i = round_Data_Prefab.transform.childCount - 1; i >= 3; i--)
            {
                winAmountHolderObj.transform.GetChild(i).gameObject.SetActive(false);
                totalAmountHolderObj.transform.GetChild(i).gameObject.SetActive(false);
                round_Data_Prefab.transform.GetChild(i).gameObject.SetActive(false);
                maskHolderObj.transform.GetChild(i).gameObject.SetActive(false);
            }
            playerProfileHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(930, playerProfileHolderInt);//165
            scoreBoardScrolView.GetComponent<RectTransform>().sizeDelta = new Vector2(930, 0);
            scoreboardContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(930, scoreboardContentint);//60
            winAmountHolderObj.GetComponent<RectTransform>().sizeDelta = new Vector2(930, winAmountHeight);//60
            totalAmountHolderObj.GetComponent<RectTransform>().sizeDelta = new Vector2(930, totalScorePanelHeight);//60
        }
        CancelInvoke(nameof(ScoreBoardStatusTimer));
    }
    internal void StartScoreBoradTimer(int timer, string msgText)
    {
        roundStatusTime = timer;
        gameStausHolder.SetActive(true);
        statusTitletxt.text = msgText;
        CancelInvoke(nameof(ScoreBoardStatusTimer));
        InvokeRepeating(nameof(ScoreBoardStatusTimer), 0, 1);

    }
    void ScoreBoardStatusTimer()
    {
        statusTxt.text = roundStatusTime.ToString();
        roundStatusTime--;
        Debug.Log(" ScoreBoard Timer => " + roundStatusTime + "Active self " + gameObject.activeSelf);
        if (roundStatusTime == 0)
        {
            if (CallBreak_GS.Inst.isGameEnded)
                ScorebaordNewGame();
            else
            {
                gameStausHolder.SetActive(false);
                CancelInvoke(nameof(ScoreBoardStatusTimer));
                Close_Score_Board(0);
            }
        }
    }
    bool WinnerFound = false;
    internal void SetRoundData(string resone, int timer)
    {
        scoreBoardGameIdTxt.text = "" + CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundTableId.Substring(CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundTableId.Length - 8);
        int localUser_SeatIndex = localUser.GetComponent<CallBreak_Users>().SeatIndex;
        List<GameObject> roundTotal_List = new List<GameObject>();
        FirstSetupScoreBoard();
        ResetScoreboard();

        Debug.Log("<color=yellow> REASON " + resone + "Current round " + CallBreak_GS.Inst.CurrentRound + "Max Round " + CallBreak_GS.Inst.MaxRound + "</color>");
        Debug.Log(" Time in ScoreBoard " + timer);

        this.transform.DOKill();
        CallBreak_UIManager.Inst.SetPlayingHeader(CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.nextRound);
        for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.users.Count; i++)
        {
            AllPlayer[i].transform.GetChild(2).GetComponent<Text>().text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.users[i].username;
            AllPlayer[i].SetActive(true);
            Debug.Log(" SetRoundData User Name " + CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.users[i].username + "Player  Namee " + AllPlayer[i].name);
            players_Profile[i].LoadIMG(CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.users[i].profilePicture, false);
            if (CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.users[i].userStatus != "playing")
            {
                playerStatusList[i].SetActive(true);
                playerStatusList[i].GetComponentInChildren<Text>().text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.users[i].userStatus;
            }

        }

        for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores.Count; i++)
        {
            GameObject Round = Instantiate(round_Data_Prefab, scoreboardContent.transform);
            roundTotal_List.Add(Round);
            Round.gameObject.name = "ROUND " + (i + 1);
            Round.GetComponent<CallBreak_RoundHandler>().roundTxt.text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores[i].title;

            for (int j = 0; j < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores[i].score.Count; j++)
            {
                double tScore = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores[i].score[j].roundPoint;
                Round.GetComponent<CallBreak_RoundHandler>().player_Round_Total[j].text = tScore.ToString();
            }
        }

        List<int> winner = new List<int>();
        WinnerFound = false;
        for (int q = 0; q < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.winner.Count; q++)
        {
            Debug.Log(" Winner Found From Backend " + CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.winner[q]);
            winner.Add(CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.winner[q]);  //add winner players in winner list
        }


        for (int j = 0; j < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores.Count; j++)
        {
            for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores[j].score.Count; i++)
            {

                int seatIndex = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.scores[j].score[i].seatIndex;


                for (int p = 0; p < winner.Count; p++)
                {
                    if (winner[p] == seatIndex)
                    {
                        Debug.Log("Winner of round " + winner[p] + " Winner Count " + winner.Count);
                        if (winner.Count == 1)
                        {
                            WinnerFound = true;
                            AllPlayer[seatIndex].transform.GetChild(1).transform.DOScale(1, 0);
                            AllPlayerTotalScore[seatIndex].GetComponent<Text>().DOColor(WinPlayerTextColor, 0);
                            scoreRecord_Mask[seatIndex].SetActive(true);


                            for (int k = 0; k < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.winningAmount.Count; k++)
                            {
                                if (k != seatIndex)
                                {
                                    WinningAmount[k].text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.winningAmount[k].winningAmount;
                                }
                                else
                                {
                                    WinningAmount[k].text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.winningAmount[k].winningAmount;
                                    // WinningAmount[k].GetComponent<Text>().DOColor(WinPlayerTextColor, 0);
                                }
                            }
                        }
                        else
                        {
                            CallBreak_UIManager.Inst.isTieBreakerRound = true;
                        }
                    }
                }
            }
            Debug.Log("roundScoreHistory Score Settted ");
            for (int i = 0; i < CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.total.Count; i++)
            {
                CallBreak_Users P = CallBreak_GameManager.Inst.GetPlayerSeatIndex(i);
                P.MyScore.text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.total[i].totalPoint.ToString();
                AllPlayerTotalScore[i].GetComponent<Text>().text = CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.total[i].totalPoint.ToString();
                Debug.Log("roundScoreHistory Total Score Settted " + CallBreak_SocketEventReceiver.Inst.getRoundWinnerData.data.roundScoreHistory.total[i].totalPoint);

            }
        }

        if (winner.Count == 0 && !CallBreak_UIManager.Inst.isTieBreakerRound && resone.Equals("WinnerData"))
        {
            Debug.Log("ROund time STart " + timer);
            if (timer != 0)
            {
                StartScoreBoradTimer(timer, " Next round start in ");
                CallBreak_AllPopupHandler.Instance.StartRTSTimer(timer, " Next round start in  ");
            }
        }
        else if (CallBreak_UIManager.Inst.isTieBreakerRound && resone.Equals("WinnerData"))
        {
            if (timer != 0)
            {
                StartScoreBoradTimer(timer, " Next tie breaker round start In ");
                CallBreak_AllPopupHandler.Instance.StartRTSTimer(timer, " Next tie breaker round start In ");
            }
        }

        if (winner.Count > 1)
        {
            for (int i = 0; i < roundTotal_List.Count; i++)
            {
                roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[0]].GetComponent<Text>().DOColor(WinPlayerTextColor, 0);
                roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[1]].GetComponent<Text>().DOColor(WinPlayerTextColor, 0);
                roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[0]].GetComponent<Text>().fontSize = 32;
                roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[0]].GetComponent<Text>().fontStyle = FontStyle.Bold;
                roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[1]].GetComponent<Text>().fontSize = 32;
                roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[1]].GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            for (int i = 0; i < roundTotal_List.Count; i++)
                if (winner.Count > 0)
                {
                    Debug.Log("  roundTotal_List[i]" + roundTotal_List[i]);
                    roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[0]].GetComponent<Text>().DOColor(WinPlayerTextColor, 0);
                    roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[0]].GetComponent<Text>().fontSize = 32;
                    roundTotal_List[i].GetComponent<CallBreak_RoundHandler>().player_Round_Total[winner[0]].GetComponent<Text>().fontStyle = FontStyle.Bold;
                }

        }

        if (WinnerFound)
        {
            Debug.Log(" Winner Found " + WinnerFound);
            winAmountHolderObj.SetActive(true);
            scoreBoardCloseBtn.transform.DOScale(0, 0);

            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(CallBreak_GS.Inst.userinfo.Player_Seat);
            p.MakeActive();
            CallBreak_GS.Inst.isGameEnded = true;
            CallBreak_GS.Inst.canDisplayPreloader = false;
            CallBreak_GS.Inst.isWinnerDeclared = true;
            CallBreak_UIManager.Inst.StopTurnTimer();

            if (timer != 0)
            {
                StartScoreBoradTimer(timer, " New game start in ");
                CallBreak_GS.Inst.fromBack = false;
            }
            CallBreak_UIManager.Inst.isTieBreakerRound = false;
            scoreBoardCloseBtn.transform.DOScale(0, 0.2f);

        }
        else if (!resone.Equals("RoundData"))
        {
            Debug.Log(" ScoreBoard Not  Open RoundData");
            winAmountHolderObj.SetActive(false);
            scoreBoardCloseBtn.transform.DOScale(1, 0);
        }
        else
        {
            Debug.Log(" ScoreBoard Are Open ");
            scoreBoardCloseBtn.transform.DOScale(1, 0);
        }

        if (resone.Equals("WinnerData"))
        {
            CallBreak_UIManager.Inst.GamePlayReset(true);  //reset game play
        }
        Debug.Log(" Now ScoreBoard Are Open ");
        this.transform.GetComponent<Canvas>().enabled = true;
        this.transform.DOMove(Vector2.zero, 0.3f).SetEase(Ease.InOutFlash);
    }

    #region Open scorebaord for WINNER_DECLARE event response
    internal void OpenScoreBoardForWinner()
    {
        CallBreak_UIManager.Inst.isScoreBoardOpen = true;
        this.transform.GetComponent<Canvas>().enabled = true;
        this.transform.DOMove(Vector2.zero, 0.3f).SetEase(Ease.InOutFlash).OnComplete(() =>
        {
            Debug.Log("Winner Screen:" + CallBreak_GS.Inst.canDisplayRoundTimerinScoreBoard);
        });
    }
    #endregion


    public void Close_Score_Board(float closeTime)
    {
        this.transform.DOKill();
        this.transform.DOMove(new Vector2(25f, 0f), closeTime).SetEase(Ease.InOutFlash).OnComplete(() =>
        {
            this.transform.GetComponent<Canvas>().enabled = true;
        });
        ResetScoreboard();
    }

    #region Scoreboard screen exit game 
    public void ScorebaordExitGame()
    {
        Debug.Log("LEAVE FROM SCOREBOARD "+ WinnerFound);
        if (WinnerFound)
        {
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.LEAVE_TABLE(WinnerFound), LeaveTableAcknowledgement, CallBreak_CustomEvents.LEAVE_TABLE);
            MGPSDK.MGPGameManager.instance.OnClickQuite();
        }
        else
        {
            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.LEAVE_TABLE(WinnerFound), LeaveTableAcknowledgement, CallBreak_CustomEvents.LEAVE_TABLE);
        }
    }
    #endregion

    #region Scoreboard screen new game 
    public void ScorebaordNewGame()
    {
        ResetAllGame();
        CallBreak_UIManager.Inst.FTUEPanel.enabled = false;
        MGPSDK.MGPGameManager.instance.sdkConfig.data.gameData.isPlay = true;
        CallBreak_SocketConnection.intance.ConnectionSignUp();
    }

    #endregion

    #region ACKNOWLEDGEMENT_CALLBACKS

    internal void LeaveTableAcknowledgement(string ackData)
    {
        Debug.Log("CallBreak_ScoreBoard || LeaveTable Acknowledged || ackData : " + ackData);
    }

    #endregion

    #region ResetScoreboard
    void ResetAllAfterNewGame()
    {
        Close_Score_Board(0);
    }
    internal void ResetScoreboard()
    {
        int child = scoreboardContent.transform.childCount;
        for (int i = 0; i < child; i++)
            DestroyImmediate(scoreboardContent.transform.GetChild(0).gameObject);
        for (int i = 0; i < 4; i++)
        {
            AllPlayer[i].SetActive(false);
            playerStatusList[i].SetActive(false);

            AllPlayer[i].transform.GetChild(1).transform.DOScale(0, 0);
            AllPlayer[i].transform.GetChild(2).GetComponent<Text>().DOColor(Color.white, 0);
            AllPlayerTotalScore[i].GetComponent<Text>().DOColor(Color.white, 0);
            scoreRecord_Mask[i].SetActive(false);
            players_Profile[i].UnLoadIMG();
        }
        scoreBoardCloseBtn.transform.DOScale(1, 0);
        gameStausHolder.SetActive(false);
        CancelInvoke(nameof(ScoreBoardStatusTimer));
    }
    #endregion
    #region ResetAll Game When Auto Start New Game
    public void ResetAllGame()
    {
        int x = CallBreak_UIManager.Inst.MyCardsParent.transform.childCount;
        for (int i = 0; i < x; i++)
        {
            Debug.Log(" Destroy Card  name when Scoreboard New Game " + CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).name);
            DestroyImmediate(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
        {
            CallBreak_PlayerController.Inst.playerList[i].ResetAllAfterNewGame();
        }
        Debug.Log(" Called From ScoreBoard ");
        CallBreak_UIManager.Inst.CloseHelpInfo();
        CallBreak_UIManager.Inst.CloseInfo();
        CallBreak_UIManager.Inst.CloseSideMenu();
        CallBreak_CardDeal.Inst.DisCardedCardDestroyImmediate();
        CallBreak_UIManager.Inst.BidSelectPopupContent.GetComponent<RectTransform>().DOScale(0, 0);
        CallBreak_UIManager.Inst.BidSelectPopup.GetComponent<Canvas>().enabled = false;
        CancelInvoke();
        ResetScoreboard();
        Close_Score_Board(0f);

        CallBreak_AllPopupHandler.Instance.StopRtstimer();
        CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
        CallBreak_AllPopupHandler.Instance.CloseExitPopup();
        CallBreak_AllPopupHandler.Instance.CloseTopMsgToast();
    }
    #endregion
}

