[System.Serializable]
public class JOIN_TABLEData
{
    public int totalPlayers ;
    public PlayarDetail playarDetail ;
}
[System.Serializable]
public class PlayarDetail
{
    public int seatIndex ;
    public string userId ;
    public string username ;
    public string profilePicture ;
}
[System.Serializable]
public class JOIN_TABLE
{
    public string en ;
    public JOIN_TABLEData data ;
}
