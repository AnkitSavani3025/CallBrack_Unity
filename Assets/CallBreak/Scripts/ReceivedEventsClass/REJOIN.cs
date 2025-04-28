using System.Collections.Generic;
[System.Serializable]
public class REJOIN
{
    public bool statusFlag ;
    public string message ;
    public string reason ;
    public string type ;
    public string title ;
    public string roundTableId ;
    public string userBalance ;

    public bool isRejoin ;
    public string bootValue ;
    public int userTurnTimer ;
    public List<int> winningScores ;
    public int seatIndex ;
    public string tableId ;
    public string tableState ;
    public int totalPlayers ;
    public int noOfPlayer;
    public int totalRound ;
    public int currentRound ;
    public string winnningAmonut ;
    public List<string> turnCurrentCards ;
    public string turnCardSequence ;
    public bool breakingCallBreak ;
    public int currentTurn ;
    public int dealerPlayer ;
    public bool isBidTurn ;
    public int currentTurnTimer ;
    public float potValue ;
    public int remaningRoundTimer;
    public List<PlayarRoundDetail> playarDetail ;
    public string massage;
}
[System.Serializable]

public class PlayarRoundDetail
{
    public string userId ;
    public string username ;
    public string profilePicture ;
    public int seatIndex ;
    public List<string> currentCards ;
    public int bid ;
    public bool bidTurn ;
    public int hands ;
    public float point ;
    public bool isAuto ;
    public bool isLeft ;
}





