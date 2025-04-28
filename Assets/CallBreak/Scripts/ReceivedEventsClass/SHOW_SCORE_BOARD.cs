using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHOW_SCORE_BOARD
{
    public List<RoundScoreData> data { get; set; }
}
public class RoundScore
{
    public string username { get; set; }
    public string profilePicture { get; set; }
    public bool isLeft { get; set; }
    public int seatIndex { get; set; }
    public int bid { get; set; }
    public int hands { get; set; }
    public int roundBags { get; set; }
    public int roundPoint { get; set; }
    public int totalBags { get; set; }
    public int BagsPenalty { get; set; }
    public int totalPoint { get; set; }
    public double userId { get; set; }
}

public class RoundScoreData
{
    public string title { get; set; }
    public List<int> winner { get; set; }
    public List<RoundScore> roundScore { get; set; }
}
