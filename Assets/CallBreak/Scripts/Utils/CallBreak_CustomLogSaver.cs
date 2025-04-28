using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_CustomLogSaver : MonoBehaviour
    {

        string filename = "";
        private void Awake()
        {
            filename = Application.persistentDataPath + "/MYLOGS.text";
            if (File.Exists(filename))
                File.Delete(filename);
            Debug.Log(filename);
        }
        private void OnDisable()
        {

            Application.logMessageReceived -= Log;
        }
        private void OnEnable()
        {
            Application.logMessageReceived += Log;

        }

        public void Log(string LogMsg, string logStack, LogType log)
        {
            if (SystemInfo.deviceName == "Galaxy A13" || SystemInfo.deviceUniqueIdentifier == "5f33502fe67d4f97") return;

            TextWriter tw = new StreamWriter(filename, true);

            tw.WriteLine("[" + System.DateTime.Now + "]" + LogMsg);

            tw.Close();

        }
    }

}