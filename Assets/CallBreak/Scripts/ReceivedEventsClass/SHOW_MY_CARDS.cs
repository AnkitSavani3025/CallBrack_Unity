using System.Collections.Generic;

[System.Serializable]
public class SHOW_MY_CARDSData
{
    public List<string> cards ;
    public int dealer ;
    public int currentRound ;
}
[System.Serializable]
public class SHOW_MY_CARDS
{
    public string en ;
    public SHOW_MY_CARDSData data ;
}
