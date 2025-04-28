using UnityEngine;
using ThreePlusGamesCallBreak;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CallBreak_Socketmanager
{
    public class CallBreak_SocketEventManager : MonoBehaviour
    {
        public static JSONObject HEART_BEAT()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();
            OBJ.AddField("data", data);
            return OBJ;
        }

        //======================== Sign Up ====================================

        internal static JSONObject SignUp_New()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();
            data.AddField("isAlredyPlaying", CallBreak_GS.Inst.isAlredyPlaying);
            data.AddField("acessToken", MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken);
            data.AddField("minPlayer", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.minPlayer.ToString());
            data.AddField("noOfPlayer", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.noOfPlayer.ToString());
            data.AddField("lobbyId", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData._id);
            data.AddField("isUseBot", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.isUseBot);
            data.AddField("entryFee", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.entryFee.ToString());
            data.AddField("moneyMode", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.moneyMode);
            data.AddField("totalRound", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.noOfRounds);
            data.AddField("winningAmount", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.winningAmount.ToString());
            data.AddField("userName", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.displayName);
            data.AddField("userId", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.userID);
            data.AddField("profilePic", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.avatar);
            data.AddField("gameId", MGPSDK.MGPGameManager.instance.sdkConfig.data.gameData.gameId);
            data.AddField("isFTUE", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE);
            data.AddField("fromBack", CallBreak_GS.Inst.fromBack);
            data.AddField("deviceId", SystemInfo.deviceUniqueIdentifier);
            data.AddField("isPlay", MGPSDK.MGPGameManager.instance.sdkConfig.data.gameData.isPlay);
            data.AddField("latitude", MGPSDK.MGPGameManager.instance.sdkConfig.data.location.latitude.ToString());
            data.AddField("longitude", MGPSDK.MGPGameManager.instance.sdkConfig.data.location.longitude.ToString());
            data.AddField("gameModeId", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.gameModeId);
            OBJ.AddField("data", data);

            return OBJ;
        }

        public static T GetDeserializeData<T>(JObject dataObjectWithInsideData)
        {
            return JsonConvert.DeserializeObject<T>(dataObjectWithInsideData.ToString());
        }

        //======================== Sign Up End =================================

        internal static JSONObject ThrowCard(string throwCard)
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();
            data.AddField("seatIndex", CallBreak_GS.Inst.userinfo.Player_Seat);
            data.AddField("card", throwCard);
            OBJ.AddField("data", data);
            return OBJ;
        }
        internal static JSONObject USER_BID(int seatIndex, int amount)
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();
            data.AddField("seatIndex", seatIndex);
            data.AddField("bid", amount);

            OBJ.AddField("data", data);
            return OBJ;
        }

        internal static JSONObject SHOW_SCORE_BOARD()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            data.AddField("tableId", CallBreak_GS.Inst.gameinfo.tabelId);
            OBJ.AddField("data", data);
            return OBJ;
        }
        internal static JSONObject LEAVE_TABLE(bool isLeaveFromScoreBoard=false)
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            data.AddField("tableId", CallBreak_GS.Inst.gameinfo.tabelId);
            data.AddField("isLeaveFromScoreBoard", isLeaveFromScoreBoard);
            data.AddField("flag", "");
            OBJ.AddField("data", data);

            return OBJ;
        }

        internal static JSONObject BACK_IN_GAME_PLAYING()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            OBJ.AddField("data", data);
            return OBJ;
        }

        internal static JSONObject REJOIN_TABLE()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            data.AddField("userId", MGPSDK.MGPGameManager.instance.sdkConfig.data.playerData[0].userId);
            OBJ.AddField("data", data);
            return OBJ;
        }

        internal static JSONObject FTUE_MESSAGE()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            OBJ.AddField("data", data);
            return OBJ;
        }

        internal static JSONObject FTUE_CHANGE_STEP(string StepName)
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            data.AddField("step", StepName);
            OBJ.AddField("data", data);
            return OBJ;
        }

        internal static JSONObject TABLE_INFO()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            data.AddField("userId", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.userID);
            OBJ.AddField("data", data);

            return OBJ;

        }
        internal static JSONObject HELP()
        {
            JSONObject data = new JSONObject();
            JSONObject OBJ = new JSONObject();

            data.AddField("userId", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.userID);
            OBJ.AddField("data", data);

            return OBJ;

        }
    }
}
