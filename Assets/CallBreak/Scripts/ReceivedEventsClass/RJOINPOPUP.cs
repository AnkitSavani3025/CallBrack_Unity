using System.Collections.Generic;

[System.Serializable]
public class RejoinResponseHandlerData
{
    public string title ;
    public string message ;
    public List<string> button_text ;
    public RejoinUserData rejoinUserData ;
}
[System.Serializable]
public class RejoinUserData
{
    public string acessToken ;
    public string minPlayer ;
    public string noOfPlayer;
    public string lobbyId ;
    public bool isUseBot ;
    public string entryFee ;
    public string moneyMode ;
    public int totalRound ;
    public string winningAmount ;
    public string userName ;
    public string userId ;
    public string profilePic ;
    public string gameId ;
    public bool isFTUE ;
    public bool fromBack ;
    public string deviceId ;
}
[System.Serializable]
public class RejoinResponseHandler
{
    public string en ;
    public RejoinResponseHandlerData data ;
}

