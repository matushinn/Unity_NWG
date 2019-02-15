using NCMB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Communication
{

    [RequireComponent(typeof(DataStoreCoroutine))]
    public class RankingHandler : SingletonMonoBehaviour<RankingHandler>
    {
        private DataStoreCoroutine dataStoreCoroutine;

        private void Awake()
        {
            dataStoreCoroutine = GetComponent<DataStoreCoroutine>();
            DontDestroyOnLoad(this.gameObject);
        }

        public IEnumerator FindKillRankingCoroutine(int num)
        {
            NCMBQuery<NCMBObject> query
                = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.KILLRANKING);

            query.OrderByDescending(NCMBDataStoreKey.KILLCOUNT);
            query.Limit = num;

            IEnumerator coroutine
                = dataStoreCoroutine.FindAsyncCoroutine(query, Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            List<NCMBObject> ncmbObjectList
                = (List<NCMBObject>)coroutine.Current;

            List<RankingInfo> ranking
                = ncmbObjectList.Select(obj => new RankingInfo(obj)).ToList();

            yield return ranking;
        }

        public IEnumerator SaveKillDataCoroutine(string nickName, int killCount)
        {
            NCMBObject ncmbObject = new NCMBObject(NCMBDataStoreClass.KILLRANKING);
            ncmbObject[NCMBDataStoreKey.NICKNAME] = nickName;
            ncmbObject[NCMBDataStoreKey.KILLCOUNT] = killCount;

            yield return dataStoreCoroutine.SaveAsyncCoroutine(ncmbObject, Main.Instance.ForceToTitle);
        }
    }
}