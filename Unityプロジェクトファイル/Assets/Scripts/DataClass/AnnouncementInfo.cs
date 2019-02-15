using Communication;
using NCMB;
using System;

public class AnnouncementInfo
{ 
    public readonly string objectId = string.Empty;
    public readonly string title = string.Empty;
    public readonly string mainText = string.Empty;
    public readonly DateTime date = new DateTime(0);

    public AnnouncementInfo(NCMBObject ncmbObject)
    {
        objectId = ncmbObject.ObjectId;

        if(ncmbObject.ContainsKey(NCMBDataStoreKey.TITLE))
        {
            title = ncmbObject[NCMBDataStoreKey.TITLE] as string;
        }

        if (ncmbObject.ContainsKey(NCMBDataStoreKey.MAIN_TEXT))
        {
            mainText = ncmbObject[NCMBDataStoreKey.MAIN_TEXT] as string;
        }

        date = (DateTime)ncmbObject.CreateDate;
    }
}