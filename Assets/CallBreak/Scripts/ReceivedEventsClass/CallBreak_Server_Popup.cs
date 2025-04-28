
using System.Collections.Generic;
namespace ThreePlusGamesCallBreak
{
    [System.Serializable]
    public class Server_PopupData
    {
        public bool isPopup;
        public string popupType;
        public string title;
        public string message;
        public int buttonCounts;
        public List<string> button_text;
        public List<string> button_color;
        public List<string> button_methods;
        public bool showLoader;
    }
    [System.Serializable]
    public class CallBreak_Server_Popup
    {
        public string en;
        public Server_PopupData data;
    }

}