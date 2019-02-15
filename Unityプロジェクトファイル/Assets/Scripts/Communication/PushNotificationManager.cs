using NCMB;
using System.Collections.Generic;
using UnityEngine;

namespace Communication
{
    public class PushNotificationManager : SingletonMonoBehaviour<PushNotificationManager>
    {
        public void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        public void SendPushAddFriend(string installationObjectId, string nickName)
        {
            string title = "フレンド追加";
            string message = nickName + "さんにフレンド追加されました。";

            SendPushByInstallation(installationObjectId, title, message);
        }

        public void SendPushByInstallation(string installationObjectId, string title, string message)
        {
            Dictionary<string, object> searchConditionDict = new Dictionary<string, object>();
            searchConditionDict["objectId"] = installationObjectId;

            NCMBPush push = new NCMBPush();
            push.Message = message;
            push.Title = title;
            push.ImmediateDeliveryFlag = true;
            push.Dialog = true;
            push.PushToAndroid = true;
            push.PushToIOS = true;

            push.Add("searchCondition", searchConditionDict);
            push.Save();
        }
    }
}