using Communication;
using NCMB;
using System;

public class GameSettingInfo
{
    public readonly DateTime updateDate;
    public readonly bool isServiceEnable = true;
    public readonly string bannerFileName = string.Empty;
    public readonly int enemyDropCoinNum = 1;
    public readonly int gachaPrice = 10;
    public readonly string termsOfUse = "規約データがありません。";

    //データストア上にフィールドが無い場合はデフォルト値を利用する//
    public GameSettingInfo(NCMBObject ncmbObject)
    {
        updateDate = Utility.UtcToLocal((DateTime)ncmbObject.UpdateDate);

        if (ncmbObject.ContainsKey(NCMBDataStoreKey.IS_SERVICE_ENABLE))
        {
            isServiceEnable = (bool)ncmbObject[NCMBDataStoreKey.IS_SERVICE_ENABLE];
        }

        if (ncmbObject.ContainsKey(NCMBDataStoreKey.BANNERFILE_NAME))
        {
            bannerFileName = ncmbObject[NCMBDataStoreKey.BANNERFILE_NAME] as string;
        }

        if (ncmbObject.ContainsKey(NCMBDataStoreKey.ENEMY_DROPCOIN_NUM))
        {
            enemyDropCoinNum = Convert.ToInt32(ncmbObject[NCMBDataStoreKey.ENEMY_DROPCOIN_NUM]);
        }

        if (ncmbObject.ContainsKey(NCMBDataStoreKey.GACHAPRICE))
        {
            gachaPrice = Convert.ToInt32(ncmbObject[NCMBDataStoreKey.GACHAPRICE]);
        }

        if (ncmbObject.ContainsKey(NCMBDataStoreKey.TERMSOFUSE))
        {
            termsOfUse = ncmbObject[NCMBDataStoreKey.TERMSOFUSE] as string;
        }
    }

    public GameSettingInfo()
    {
        updateDate = new DateTime(0);
    }
}