
using System.Collections.Generic;
[System.Serializable]
public class WINNER_DECLAREData
{
    public int timer ;
    public List<int> winner ;
    public RoundScoreHistory roundScoreHistory ;
    public List<WinningAmount> winningAmount ;
    public string roundTableId ;
    public int nextRound;
}
[System.Serializable]
public class WINNER_DECLARE
{
    public string en ;
    public WINNER_DECLAREData data ;
}
[System.Serializable]
public class RoundScoreHistory
{
    public List<Total> total ;
    public List<Score> scores ;
    public List<User> users ;
}
[System.Serializable]
public class Score
{
    public string title ;
    public List<Score2> score ;
}
[System.Serializable]
public class Score2
{
    public double roundPoint ;
    public int seatIndex ;
}
[System.Serializable]
public class Total
{
    public double totalPoint ;
    public int seatIndex ;
}
[System.Serializable]

public class User
{
    public string username ;
    public string profilePicture ;
    public int seatIndex ;
    public string userStatus;
}
[System.Serializable]

public class WinningAmount
{
    public int seatIndex ;
    public string userId ;
    public string winningAmount ;
}


