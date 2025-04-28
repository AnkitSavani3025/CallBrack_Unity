using System.Collections.Generic;

[System.Serializable]
public class USER_BID_TURN_STARTEDData
{
    public List<int> seatIndexList;
    public int time;
}
[System.Serializable]
public class USER_BID_TURN_STARTED
{
    public string en;
    public USER_BID_TURN_STARTEDData data;
}