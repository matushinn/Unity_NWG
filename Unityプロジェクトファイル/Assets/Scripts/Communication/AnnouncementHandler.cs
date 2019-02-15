using NCMB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// お知らせ管理クラス
/// </summary>
/// 
namespace Communication
{ 
    [RequireComponent(typeof(DataStoreCoroutine))]
    public class AnnouncementHandler : SingletonMonoBehaviour<AnnouncementHandler>
    {
        private DataStoreCoroutine dataStoreCoroutine;

        private void Awake()
        {
            dataStoreCoroutine = GetComponent<DataStoreCoroutine>();
            DontDestroyOnLoad(this.gameObject);
        }

        public IEnumerator GetAnnoucementListCoroutine(UnityAction<NCMBException> errorCallback)
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.ANNOUNCEMENTS);

            //登録日が新しい順//
            query.OrderByDescending(NCMBDataStoreKey.CREATE_DATE);
            //取得上限数//
            query.Limit = 10;

            IEnumerator coroutine = dataStoreCoroutine.FindAsyncCoroutine(query, errorCallback);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                List<NCMBObject> ncmbObjectList = (List<NCMBObject>)coroutine.Current;
                yield return ncmbObjectList.ConvertAll(ncmbObject => new AnnouncementInfo(ncmbObject)).ToList();
            }
        }

    }

}