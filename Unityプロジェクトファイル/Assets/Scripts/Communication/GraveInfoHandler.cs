using NCMB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Communication
{

    [RequireComponent(typeof(DataStoreCoroutine))]
    public class GraveInfoHandler : SingletonMonoBehaviour<GraveInfoHandler>
    {
        private DataStoreCoroutine dataStoreCoroutine;

        private void Awake()
        {
            dataStoreCoroutine = GetComponent<DataStoreCoroutine>();
            DontDestroyOnLoad(this.gameObject);
        }

        public IEnumerator SaveGraveInfoCoroutine(
            string nickName,
            string deathMessage,
            string curseTypeStr,
            Vector3 position)
        {
            NCMBObject ncmbObject
                = GraveInfo.CreateNCMBObject(nickName, deathMessage, curseTypeStr, position);

            IEnumerator coroutine
                = dataStoreCoroutine.SaveAsyncCoroutine(ncmbObject, Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            yield return coroutine.Current;
        }

        public IEnumerator FindGraveInfoListCoroutine(int maxGraveNum)
        {
            NCMBQuery<NCMBObject> query
                = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.GRAVEINFO_LIST);

            //日付の降順でデータを取得//
            query.OrderByDescending(NCMBDataStoreKey.CREATE_DATE);

            //取得するお墓の数の上限値//
            query.Limit = maxGraveNum;

            IEnumerator coroutine
                = dataStoreCoroutine.FindAsyncCoroutine(query, Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList
                    = (List<NCMBObject>)coroutine.Current;

                yield return ncmbObjectList.ConvertAll(ncmbObject => new GraveInfo(ncmbObject));
            }
            else
            {
                yield return null;
            }
        }

        public IEnumerator FetchGraveInfoByObjectRefCoroutine(NCMBObject ncmbObject)
        {
            GraveInfo graveInfo = null;
            bool isConnecting = true;

            ncmbObject.FetchAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    Main.Instance.ForceToTitle(e);
                }
                else
                {
                    graveInfo = new GraveInfo(ncmbObject);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return graveInfo;
        }

        public void CountUpGraveUsedCounter(string objectId)
        {
            NCMBObject ncmbObject = new NCMBObject(NCMBDataStoreClass.GRAVEINFO_LIST);
            ncmbObject.ObjectId = objectId;

            ncmbObject.FetchAsync((NCMBException e) =>
            {
                if (e == null)
                {
                    ncmbObject.Increment(NCMBDataStoreKey.CHECKCOUNTER);
                    ncmbObject.SaveAsync();
                }
            });
        }
    }
}