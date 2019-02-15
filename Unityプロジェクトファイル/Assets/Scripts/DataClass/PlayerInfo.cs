using Communication;
using NCMB;
using System;

public class PlayerInfo
{
    public readonly string objectId = string.Empty;
    public readonly string userName = string.Empty;
    public readonly string nickName = string.Empty;
    public readonly int killCount = 0;
    public readonly string screenShotFileName = string.Empty;
    public readonly string equipCardId = string.Empty;
    public readonly string installationObjectId = string.Empty;
    public readonly DateTime lastLogInDateTime = new DateTime(0);

    public PlayerInfo(NCMBObject ncmbObject)
    {
        objectId = ncmbObject.ObjectId;
        userName = ncmbObject[NCMBDataStoreKey.USERNAME] as string;
        nickName = ncmbObject[NCMBDataStoreKey.NICKNAME] as string;
        killCount = (int)Convert.ToInt64(ncmbObject[NCMBDataStoreKey.KILLCOUNT]);
        screenShotFileName = ncmbObject[NCMBDataStoreKey.SCREENSHOT_FILENAME] as string;
        equipCardId = ncmbObject[NCMBDataStoreKey.EQUIPCARD_ID] as string;
        installationObjectId = ncmbObject[NCMBDataStoreKey.INSTALLSTION_OBJECTID] as string;
    }

    public static NCMBObject CreateNCMBObject(
        string nickName, 
        string userName)
    {
        NCMBObject ncmbObject
            = new NCMBObject(NCMBDataStoreClass.PLAYERINFO_LIST);

        ncmbObject[NCMBDataStoreKey.USERNAME] = userName;
        ncmbObject[NCMBDataStoreKey.NICKNAME] = nickName;
        ncmbObject[NCMBDataStoreKey.KILLCOUNT] = 0;
        ncmbObject[NCMBDataStoreKey.SCREENSHOT_FILENAME] = string.Empty;
        ncmbObject[NCMBDataStoreKey.EQUIPCARD_ID] = "Fast15";// string.Empty;
        ncmbObject[NCMBDataStoreKey.INSTALLSTION_OBJECTID] = string.Empty;

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        ncmbObject[NCMBDataStoreKey.INSTALLSTION_OBJECTID]
           = NCMBInstallation.getCurrentInstallation().ObjectId;
#endif
        return ncmbObject;
    }
}