using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[System.Serializable]
public class SignUpResponseHandlerData
{
    public SIGNUP SIGNUP;
    public GAMETABLEINFO GAME_TABLE_INFO;
}
[System.Serializable]
public class GAMETABLEINFO
{
    public bool isRejoin;
    public bool isFTUE;
    public string bootValue;
    public int potValue;
    public int userTurnTimer;
    public List<int> winningScores;
    public string roundTableId;
    public string tableId;
    public int totalPlayers;
    public int totalRound;
    public int currentRound;
    public string winnningAmonut;
    public string noOfPlayer;
    public List<Seat> seats;
    public int seatIndex;
}
[System.Serializable]
public class SignUpResponseHandler
{
    public string en;
    public SignUpResponseHandlerData data;
    public string userId;
    public string tableId;
}
[System.Serializable]
public class Seat
{
    public string _id;
    public string userId;
    public string username;
    public string profilePicture;
    public int seatIndex;
}
[System.Serializable]
public class SIGNUP
{
    public string _id;
    public string userId;
    public string username;
    public float balance;
}

