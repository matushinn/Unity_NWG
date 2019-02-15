using NCMB;
using System;
using Communication;

public class RankingInfo
{
    public readonly string nickName;
    public readonly int killCount;
    public readonly string updateDateStr;

    public RankingInfo(NCMBObject ncmbObject)
    {
        nickName = ncmbObject[NCMBDataStoreKey.NICKNAME] as string;
        killCount = (int)Convert.ToInt64(ncmbObject[NCMBDataStoreKey.KILLCOUNT]);

        DateTime _updateDate = ncmbObject.UpdateDate ?? new DateTime(0);
        _updateDate = Utility.UtcToLocal(_updateDate);
        updateDateStr = _updateDate.ToString("yyyy/MM/dd");
    }
}