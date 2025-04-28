

using System.Collections.Generic;
[System.Serializable]
public class RemainWalletBalance
{
    public string userId;
    public float balance;
}
[System.Serializable]
public class COLLECT_BOOT_VALUEData
{
    public string bootValue;
    public List<string> userIds;
    public List<RemainWalletBalance> balance;
}
[System.Serializable]
public class COLLECT_BOOT_VALUE
{
    public string en;
    public COLLECT_BOOT_VALUEData data;
}

