
using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Communication
{

    [RequireComponent(typeof(DataStoreCoroutine))]
    public class GameSettingManager : SingletonMonoBehaviour<GameSettingManager>
    {
        private DataStoreCoroutine dataStoreCoroutine;
        private FileStoreManager fileStoreManager;

        private GameSettingInfo currentGameSettingInfo;

        public bool IsLoaded { get { return currentGameSettingInfo != null; } }

        private void Awake()
        {
            dataStoreCoroutine = GetComponent<DataStoreCoroutine>();
            fileStoreManager = FileStoreManager.Instance;

            currentGameSettingInfo = new GameSettingInfo();

            DontDestroyOnLoad(this.gameObject);
        }

        public IEnumerator FetchGameSetting()
        {
            NCMBQuery<NCMBObject> query
                = new NCMBQuery<NCMBObject>(NCMBDataStoreClass.GAMESETTING);

            //この設定ファイルは1個しか存在しない//
            query.Limit = 1;

            IEnumerator coroutine
                = dataStoreCoroutine.FindAsyncCoroutine(query, Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            List<NCMBObject> ncmbObjectList = (List<NCMBObject>)coroutine.Current;

            if (!ncmbObjectList.Any())
            {
                //データスストア上にない場合はGameSettingInfoクラスの初期値で動作する//
                currentGameSettingInfo = new GameSettingInfo();
                yield break;
            }
            else
            {
                currentGameSettingInfo = new GameSettingInfo(ncmbObjectList.First());
            }

            //端末に更新履歴があるか
            if (PlayerPrefs.HasKey(PlayerPrefsKey.GAMESETTING_UPDATEDATE))
            {
                //その日付は変わっているか//
                string datetimeString
                    = PlayerPrefs.GetString(PlayerPrefsKey.GAMESETTING_UPDATEDATE);

                DateTime lastUpdateDateTime
                    = DateTime.FromBinary(Convert.ToInt64(datetimeString));

                //日付に変化がある場合に限りバナー画像を再取得//
                if (lastUpdateDateTime != currentGameSettingInfo.updateDate)
                {
                    yield return UpdateResourceFiles();
                }
            }
            else
            {
                yield return UpdateResourceFiles();
            }

            //端末に更新ファイルの作成日付を保存//
            PlayerPrefs.SetString(
                PlayerPrefsKey.GAMESETTING_UPDATEDATE,
                currentGameSettingInfo.updateDate.ToBinary().ToString()
                );

            PlayerPrefs.Save();
        }

        private IEnumerator UpdateResourceFiles()
        {
            //バナーファイルの再取得//
            if (string.IsNullOrEmpty(currentGameSettingInfo.bannerFileName))
            {
                fileStoreManager.DeleteBannerCacheFile();
                yield break;
            }

            yield return fileStoreManager.FetchBannerFileCoroutine(currentGameSettingInfo.bannerFileName, Main.Instance.ForceToTitle);
        }

        public int GetGachaPrice()
        {
            return currentGameSettingInfo.gachaPrice;
        }

        public int GetEnemyDropCoinNum()
        {
            return currentGameSettingInfo.enemyDropCoinNum;
        }

        public string GetUpdateDateString()
        {
            if (currentGameSettingInfo.updateDate == new DateTime())
            {
                return string.Empty;
            }

            return currentGameSettingInfo.updateDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string GetTermsOfUse()
        {
            return currentGameSettingInfo.termsOfUse;
        }

        public bool IsServiceEnable()
        {
            return currentGameSettingInfo.isServiceEnable;
        }
    }
}