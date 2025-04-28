using System.Collections.Generic;
[System.Serializable]
public class USER_TURN_STARTEDData
{
    public int seatIndex ;
    public int time ;
    public string cardSequence ;
    public List<string> card ;
}
[System.Serializable]

public class USER_TURN_STARTED
{
    public string en ;
    public USER_TURN_STARTEDData data ;
}
