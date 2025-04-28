[System.Serializable]
public class TABLE_INFOData
{
    public bool success ;
    public object error ;
    public Data data ;    
}
[System.Serializable]
public class Data
{
    public string entryFee ;
    public string rake ;
    public int nmberOfRounds ;
    public string numberOfPlayer ;
    public int numberOfCard ;
}
[System.Serializable]
public class TABLE_INFO
{
    public string en ;
    public TABLE_INFOData data ;
    public string userId ;
    public string tableId ;
}

