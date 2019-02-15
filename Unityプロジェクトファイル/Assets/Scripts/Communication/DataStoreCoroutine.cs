using NCMB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// NCMBObject操作のコルーチンラッパークラス//
/// </summary>

namespace Communication
{

    public class DataStoreCoroutine : MonoBehaviour
    {
        public IEnumerator FindAsyncCoroutine(NCMBQuery<NCMBObject> query, UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;

            List<NCMBObject> ncmbObjectList = new List<NCMBObject>();

            query.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }
                else
                {
                    ncmbObjectList = _ncmbObjectList;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return ncmbObjectList;
        }

        public IEnumerator SaveAsyncCoroutine(NCMBObject ncmbObject, UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;

            ncmbObject.SaveAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return ncmbObject;
        }

        public IEnumerator CountCoroutine(string className, UnityAction<NCMBException> errorCallback)
        {
            int count = 0;

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(className);

            bool isConnecting = true;

            query.CountAsync((int _count, NCMBException e) =>
            {
                if (e != null)
                {
                //通信失敗の場合はタイトルに戻る//
                errorCallback(e);
                }
                else
                {
                    count = _count;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return count;
        }

        public IEnumerator FetchAsyncCoroutine(NCMBObject ncmbObject, UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;

            ncmbObject.FetchAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator DeleteAsyncCoroutine(NCMBObject ncmbObject, UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;

            ncmbObject.DeleteAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

    }
}