using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Communication
{
    public class Gacha : SingletonMonoBehaviour<Gacha>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        public IEnumerator GachaScriptCoroutine(string itemId, string userName, UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;
            string resultstr = "";

            NCMBScript script = new NCMBScript("gachaScript.js", NCMBScript.MethodType.GET);
            Dictionary<string, object> query = new Dictionary<string, object>() { { "item", itemId }, { "user", userName } };
            script.ExecuteAsync(null, null, query, (byte[] result, NCMBException e) =>
            {
                if (e != null)
                {
                    // 失敗//
                    //item=1&user=ncmbgacha
                }
                else
                {
                    // 成功//
                    resultstr = new string(Encoding.UTF8.GetChars(result, 0, 2));

                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return resultstr;
        }
    }
}