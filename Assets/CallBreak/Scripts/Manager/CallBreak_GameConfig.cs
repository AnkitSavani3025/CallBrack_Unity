using System;
using System.Collections.Generic;
using UnityEngine;
using CallBreak_Socketmanager;

[Serializable]
public class CallBreak_GameConfig : MonoBehaviour
{
    /******************************** Global Variables & Constants ********************************/

    public static CallBreak_GameConfig Inst;
    public bool isFTUEFinished=true;
    public bool NetworkIndicetor;
    public List<string> NetworkIndicatorColors;
    public float application_version = 1.1f;
    public Action ServerConfigFetched;
    private void Awake() => Inst = this;
    private void Start()
    {
        ServerConfigFetched?.Invoke();
        //        CallBreak_SocketConnection.intance.SocketConnectionStart(CallBreak_SocketConnection.intance.S3URL);
    }
}