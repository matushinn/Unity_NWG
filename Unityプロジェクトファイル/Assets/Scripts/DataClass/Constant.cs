namespace Communication
{
    //PlayerPrefsのキー名定義//
    public static class PlayerPrefsKey
    {
        public static readonly string NCMB_USERNAME = "NCMBUserName";
        public static readonly string NCMB_PASSWORD = "NCMBPassword";
        public static readonly string GAMESETTING_UPDATEDATE = "GameSettingUpdateDate";
    }

    //会員管理のフィールド名定義//
    public static class NCMBUserKey
    {
        public static readonly string USERNAME = "UserName";
        public static readonly string NICKNAME = "NickName";
        public static readonly string KILLCOUNT = "KillCount";
        public static readonly string COINCOUNT = "CoinCount";
        public static readonly string CARDIDLIST = "CardIdList";
        public static readonly string PLAYERINFO = "PlayerInfo";
        public static readonly string LASTGRAVEINFO = "LastGraveInfo";
        public static readonly string FRIENDID_LIST = "FriendIdList";
        public static readonly string INSTALLATION_OBJECTID = "InstallationObjectId";
        public static readonly string DAILYBONUSFLAG = "DailyBonusFlag";
    }

    //データストアのフィールド名定義//
    public static class NCMBDataStoreKey
    {
        //Default
        public static readonly string OBJECTID = "objectId";
        public static readonly string CREATE_DATE = "createDate";
        public static readonly string UPDATE_DATE = "updateDate";

        //General
        public static readonly string USERNAME = "UserName";
        public static readonly string NICKNAME = "NickName";
        public static readonly string KILLCOUNT = "KillCount";

        //GraveInfo
        public static readonly string CHECKCOUNTER = "CheckCounter";
        public static readonly string MESSAGE = "Message";
        public static readonly string CURSETYPE = "CurseType";
        public static readonly string POSITION = "Position";

        //PlayerInfo
        public static readonly string SCREENSHOT_FILENAME = "ScreenShotFileName";
        public static readonly string EQUIPCARD_ID = "EquipCardId";
        public static readonly string INSTALLSTION_OBJECTID = "InstallationObjectId";

        //GameSetting
        public static readonly string IS_SERVICE_ENABLE = "IsServiceEnable";
        public static readonly string BANNERFILE_NAME = "BannerFileName";
        public static readonly string ENEMY_DROPCOIN_NUM = "EnemyDropCoinNum";
        public static readonly string GACHAPRICE = "GachaPrice";
        public static readonly string TERMSOFUSE = "TermsOfUse";

        //Announcements
        public static readonly string TITLE = "Title";
        public static readonly string MAIN_TEXT = "MainText";
    }

    //データストアのクラス名定義//
    public static class NCMBDataStoreClass
    {
        public static readonly string GRAVEINFO_LIST = "GraveInfoList";
        public static readonly string KILLRANKING = "KillRanking";
        public static readonly string PLAYERINFO_LIST = "PlayerInfoList";
        public static readonly string GAMESETTING = "GameSetting";
        public static readonly string ANNOUNCEMENTS = "Announcements";
    }

    public static class PushNotificationKey
    {
    }
}