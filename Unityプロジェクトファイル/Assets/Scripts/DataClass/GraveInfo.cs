using NCMB;
using System;
using System.Collections;
using UnityEngine;
using Communication;

public class GraveInfo
{
    public enum CurseType { None, Damage, Heal }

    public readonly string nickName;
    public readonly string deathMessage;
    public readonly string objectId;
    public readonly CurseType curseType;
    public readonly Vector3 position;
    public readonly int checkCounter;

    //コンストラクタ//
    public GraveInfo(NCMBObject ncmbObject)
    {
        nickName = ncmbObject[NCMBDataStoreKey.NICKNAME] as string;
        deathMessage = ncmbObject[NCMBDataStoreKey.MESSAGE] as string;
        objectId = ncmbObject.ObjectId;
        curseType = (CurseType)Enum.ToObject(typeof(CurseType), ncmbObject[NCMBDataStoreKey.CURSETYPE]);
        position = Utility.DoubleArrayListToVector3(ncmbObject[NCMBDataStoreKey.POSITION] as ArrayList);
        checkCounter = Convert.ToInt32(ncmbObject[NCMBDataStoreKey.CHECKCOUNTER]);
    }

    public static NCMBObject CreateNCMBObject(
        string nickName, 
        string deathMessage, 
        string curseTypeStr, 
        Vector3 position)
    {
        double[] positionDoubleArray
            = Utility.Vector3toDoubleArray(new Vector3(position.x, 0f, position.z));

        NCMBObject ncmbObject = new NCMBObject(NCMBDataStoreClass.GRAVEINFO_LIST);
        ncmbObject[NCMBDataStoreKey.NICKNAME] 
            = string.IsNullOrEmpty(nickName) ? "Unknown" : nickName;
        ncmbObject[NCMBDataStoreKey.POSITION] = positionDoubleArray;
        ncmbObject[NCMBDataStoreKey.MESSAGE] = deathMessage;
        ncmbObject[NCMBDataStoreKey.CURSETYPE] 
            = (int)Utility.TryParse<CurseType>(curseTypeStr);
        ncmbObject[NCMBDataStoreKey.CHECKCOUNTER] = 0;

        return ncmbObject;
    }
}