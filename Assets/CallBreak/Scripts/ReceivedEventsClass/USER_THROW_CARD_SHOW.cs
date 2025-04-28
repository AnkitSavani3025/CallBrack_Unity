[System.Serializable]
public class USER_THROW_CARD_SHOWData
{
    public int seatIndex;
    public string card;
    public bool breakingSpades;
    public bool turnTimeout;
}
[System.Serializable]
public class USER_THROW_CARD_SHOW
{
    public string en;
    public USER_THROW_CARD_SHOWData data;
}

