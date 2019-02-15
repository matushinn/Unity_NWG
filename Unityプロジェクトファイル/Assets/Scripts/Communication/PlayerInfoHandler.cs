using NCMB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// データストア管理クラス
/// </summary>
/// 
namespace Communication
{

    [RequireComponent(typeof(DataStoreCoroutine))]
    public class PlayerInfoHandler : SingletonMonoBehaviour<PlayerInfoHandler>
    {
        private DataStoreCoroutine dataStoreCoroutine;

        private void Awake()
        {
            dataStoreCoroutine = GetComponent<DataStoreCoroutine>();
            DontDestroyOnLoad(this.gameObject);
        }

        public IEnumerator CreateOwnDataCoroutine(
            string nickName,
            string userName,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBObject ncmbObject
                = PlayerInfo.CreateNCMBObject(nickName: nickName, userName: userName);

            IEnumerator coroutine
                = dataStoreCoroutine.SaveAsyncCoroutine(ncmbObject, errorCallback);

            yield return StartCoroutine(coroutine);

            NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO]
                = (NCMBObject)coroutine.Current;
        }

        public IEnumerator FetchOwnDataCoroutine(UnityAction<NCMBException> errorCallback)
        {
            if (NCMBUser.CurrentUser == null)
            {
                errorCallback(new NCMBException());
                yield break;
            }

            yield return dataStoreCoroutine.FetchAsyncCoroutine((NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO], errorCallback);
        }

        public IEnumerator DeleteOwnDataCoroutine(UnityAction<NCMBException> errorCallback)
        {
            if (NCMBUser.CurrentUser != null)
            {
                NCMBObject playerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];

                if (playerInfo != null)
                {
                    yield return dataStoreCoroutine.DeleteAsyncCoroutine(playerInfo, errorCallback);
                }
            }
        }

        public IEnumerator SaveScreenShotNameToOwnData(
            string ncmbFileName,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBObject ownPlayerInfo
                = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];

            ownPlayerInfo[NCMBDataStoreKey.SCREENSHOT_FILENAME]
                = ncmbFileName as string;

            yield return dataStoreCoroutine.SaveAsyncCoroutine(ownPlayerInfo, errorCallback);
        }

        public IEnumerator SaveKillCountToOwnData(
            int killCount,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];
            ownPlayerInfo[NCMBDataStoreKey.KILLCOUNT] = killCount;
            yield return dataStoreCoroutine.SaveAsyncCoroutine(ownPlayerInfo, errorCallback);
        }

        public IEnumerator SaveInstallationToOwnData(UnityAction<NCMBException> errorCallback)
        {
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];
            ownPlayerInfo[NCMBDataStoreKey.INSTALLSTION_OBJECTID] = NCMBInstallation.getCurrentInstallation().ObjectId;
            yield return dataStoreCoroutine.SaveAsyncCoroutine(ownPlayerInfo, errorCallback);
        }

        public IEnumerator SaveEquipCardIdToOwnData(string equipCardId, UnityAction<NCMBException> errorCallback)
        {
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];
            ownPlayerInfo[NCMBDataStoreKey.EQUIPCARD_ID] = equipCardId;
            yield return dataStoreCoroutine.SaveAsyncCoroutine(ownPlayerInfo, errorCallback);
        }

        public string GetEquipCardIdFromOwnData()
        {
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];

            if (ownPlayerInfo.ContainsKey(NCMBDataStoreKey.EQUIPCARD_ID) == false)
            {
                return string.Empty;
            }else
            {
                return ownPlayerInfo[NCMBDataStoreKey.EQUIPCARD_ID] as string;
            }
        }

        public IEnumerator FindByIdListCoroutine(
            int num,
            List<string> objectIdList,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBQuery<NCMBObject> query
                = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.PLAYERINFO_LIST);

            //このObjectIDリストのPlayerInfoを取得//
            if (objectIdList != null)
            {
                query.WhereContainedIn(NCMBDataStoreKey.OBJECTID, objectIdList);
            }

            //取得上限数//
            query.Limit = num;

            IEnumerator coroutine = dataStoreCoroutine.FindAsyncCoroutine(query, errorCallback);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList = (List<NCMBObject>)coroutine.Current;
                yield return ncmbObjectList.ConvertAll(ncmbObject => new PlayerInfo(ncmbObject)).ToList();
            }
        }

        public IEnumerator FindWithIgnoreIdListCoroutine(
            int num,
            List<string> ignoreObjectIdList,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.PLAYERINFO_LIST);

            if (ignoreObjectIdList == null) ignoreObjectIdList = new List<string>();

            //自分のデータを除く//
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];
            ignoreObjectIdList.Add(ownPlayerInfo.ObjectId);

            //このリストと一致しないもの//
            query.WhereNotContainedIn(NCMBDataStoreKey.OBJECTID, ignoreObjectIdList);
            //登録日が新しい順//
            query.OrderByDescending(NCMBDataStoreKey.CREATE_DATE);
            //取得上限数//
            query.Limit = num;

            IEnumerator coroutine = dataStoreCoroutine.FindAsyncCoroutine(query, errorCallback);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList = (List<NCMBObject>)coroutine.Current;
                yield return ncmbObjectList.ConvertAll(ncmbObject => new PlayerInfo(ncmbObject)).ToList();
            }
        }

        public IEnumerator FindWithIgnoreIdListHasScreenShotCoroutine(
        int num,
        List<string> ignoreObjectIdList,
        UnityAction<NCMBException> errorCallback)
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.PLAYERINFO_LIST);

            if (ignoreObjectIdList == null) ignoreObjectIdList = new List<string>();

            //自分のデータを除く//
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];
            ignoreObjectIdList.Add(ownPlayerInfo.ObjectId);

            //スクリーンショットファイルがnullでないもの/
            query.WhereNotEqualTo(NCMBDataStoreKey.SCREENSHOT_FILENAME, string.Empty);
            //このリストと一致しないもの//
            query.WhereNotContainedIn(NCMBDataStoreKey.OBJECTID, ignoreObjectIdList);
            //登録日が新しい順//
            query.OrderByDescending(NCMBDataStoreKey.CREATE_DATE);
            //取得上限数//
            query.Limit = num;

            IEnumerator coroutine = dataStoreCoroutine.FindAsyncCoroutine(query, errorCallback);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList = (List<NCMBObject>)coroutine.Current;
                yield return ncmbObjectList.ConvertAll(ncmbObject => new PlayerInfo(ncmbObject)).ToList();
            }
        }

        public IEnumerator FindByNickNameCoroutine(string nickName, List<string> ignoreObjectIdList, UnityAction<NCMBException> errorCallback)
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.PLAYERINFO_LIST);

            if (ignoreObjectIdList == null) ignoreObjectIdList = new List<string>();

            //自分のデータを除く//
            NCMBObject ownPlayerInfo = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];
            ignoreObjectIdList.Add(ownPlayerInfo.ObjectId);

            query.WhereNotContainedIn(NCMBDataStoreKey.OBJECTID, ignoreObjectIdList); //このリストと一致しないもの//

            query.WhereEqualTo(NCMBDataStoreKey.NICKNAME, nickName); //このニックネームと一致するもの//

            query.OrderByDescending(NCMBDataStoreKey.CREATE_DATE);//登録日が新しい順//

            IEnumerator coroutine = dataStoreCoroutine.FindAsyncCoroutine(query, errorCallback);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList = (List<NCMBObject>)coroutine.Current;
                yield return ncmbObjectList.ConvertAll(ncmbObject => new PlayerInfo(ncmbObject)).ToList();
            }
        }

        public IEnumerator FindByUserNameCoroutine(
            string userName, List<string> ignoreObjectIdList,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBQuery<NCMBObject> query
                = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.PLAYERINFO_LIST);

            //自分のデータを除く//
            NCMBObject ownPlayerInfo
                = (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.PLAYERINFO];

            if (ignoreObjectIdList == null)
            {
                ignoreObjectIdList = new List<string>();
            }
            ignoreObjectIdList.Add(ownPlayerInfo.ObjectId);

            //このリストと一致しないもの//
            query.WhereNotContainedIn(NCMBDataStoreKey.OBJECTID, ignoreObjectIdList);

            //このユーザーネームと一致するもの//
            query.WhereEqualTo(NCMBDataStoreKey.USERNAME, userName);

            IEnumerator coroutine
                = dataStoreCoroutine.FindAsyncCoroutine(query, errorCallback);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList
                    = (List<NCMBObject>)coroutine.Current;

                yield return ncmbObjectList.ConvertAll(ncmbObject => new PlayerInfo(ncmbObject)).ToList();
            }
        }
    }
}