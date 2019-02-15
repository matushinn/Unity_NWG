using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Communication
{

    public class UserAuth : SingletonMonoBehaviour<UserAuth>
    {
        private void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        public DateTime GetUpdateTime()
        {
            DateTime date = (DateTime)NCMBUser.CurrentUser.UpdateDate;
            return Utility.UtcToLocal(date);
        }

        public IEnumerator LogInCoroutine(string userName, string password, UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;

            NCMBUser.LogInAsync(userName, password, (NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SignUpCoroutine(
            string userName,
            string mail,
            string password,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBUser user = new NCMBUser();
            user.UserName = userName;
            user.Email = mail == "" ? null : mail;
            user.Password = password;

            user.Add(NCMBUserKey.NICKNAME, userName);
            user.Add(NCMBUserKey.KILLCOUNT, 0);
            user.Add(NCMBUserKey.COINCOUNT, 0);
            user.Add(NCMBUserKey.CARDIDLIST, new List<string>());
            user.Add(NCMBUserKey.DAILYBONUSFLAG, false);

            bool isConnecting = true;

            user.SignUpAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator AutoSignUpCoroutine(
            string nickName,
            UnityAction<NCMBException> errorCallback)
        {
            NCMBUser user = new NCMBUser();

            user.Add(NCMBUserKey.NICKNAME, nickName);
            user.Add(NCMBUserKey.KILLCOUNT, 0);
            user.Add(NCMBUserKey.COINCOUNT, 0);
            user.Add(NCMBUserKey.CARDIDLIST, new List<string>());
            user.Add(NCMBUserKey.DAILYBONUSFLAG, false);

            string generatedPassword = Utility.GenerateRandomAlphanumeric(8, true);
            user.Password = generatedPassword;

            bool isSuccess = false;

            //ID重複の際に成功するまで繰り返す//
            while (!isSuccess)
            {
                string generatedUserName = Utility.GenerateRandomAlphanumeric(8, true);
                user.UserName = generatedUserName;

                bool isConnecting = true;

                user.SignUpAsync((NCMBException e) =>
                {
                    if (e != null)
                    {
                    //userNameが衝突した場合は処理を繰り返すため、エラー終了しない//
                    if (e.ErrorCode != NCMBException.DUPPLICATION_ERROR)
                        {
                            errorCallback(e);
                        }
                    }
                    else
                    {
                    //ログインに成功したら生成したID・パスをローカルに保存する//
                    SaveLocalUserNameAndPassword(generatedUserName, generatedPassword);
                        isSuccess = true;
                    }

                    isConnecting = false;
                });

                while (isConnecting) { yield return null; }
            }
        }

        public void SaveLocalUserNameAndPassword(string userName, string password)
        {
            PlayerPrefs.SetString(PlayerPrefsKey.NCMB_USERNAME, userName);
            PlayerPrefs.SetString(PlayerPrefsKey.NCMB_PASSWORD, password);
            PlayerPrefs.Save();
        }

        public string GetUserName()
        {
            if (NCMBUser.CurrentUser == null)
            {
                Main.Instance.ForceToTitle("ログインしていません");
                return string.Empty;
            }

            return NCMBUser.CurrentUser.UserName;
        }

        public string LocalSavedUserName
        {
            get
            {
                return PlayerPrefs.HasKey(PlayerPrefsKey.NCMB_USERNAME) ?
                    PlayerPrefs.GetString(PlayerPrefsKey.NCMB_USERNAME) : string.Empty;
            }
        }

        public string LocalSavedPassword
        {
            get
            {
                return PlayerPrefs.HasKey(PlayerPrefsKey.NCMB_PASSWORD) ?
                    PlayerPrefs.GetString(PlayerPrefsKey.NCMB_PASSWORD) : string.Empty;
            }
        }

        public void DeleteLocalUserNameAndPassword()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey.NCMB_USERNAME);
            PlayerPrefs.DeleteKey(PlayerPrefsKey.NCMB_PASSWORD);
        }

        public IEnumerator AutoLogInCoroutine(UnityAction<NCMBException> errorCallback)
        {
            yield return LogInCoroutine(LocalSavedUserName, LocalSavedPassword, errorCallback);
        }

        public IEnumerator DeleteUserAsyncCoroutine(UnityAction<NCMBException> errorCallback)
        {
            if (NCMBUser.CurrentUser == null)
            {
                errorCallback(new NCMBException("ログインしていません"));
                yield break;
            }

            bool isConnecting = true;
            //NCMBからユーザーを削除//
            NCMBUser.CurrentUser.DeleteAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator LogOutCoroutine()
        {
            bool isConnecting = true;

            NCMBUser.LogOutAsync((NCMBException e) =>
            {
                if (e != null)
                {
                //処理を繰り返す//
                StartCoroutine(LogOutCoroutine());
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

        //メインゲーム中、デイリーボーナスなどでも追加される//
        public void AddCoinCount(int num)
        {
            if (NCMBUser.CurrentUser != null && NCMBUser.CurrentUser.ContainsKey(NCMBUserKey.COINCOUNT))
            {
                NCMBUser.CurrentUser[NCMBUserKey.COINCOUNT] = GetCoinCount() + num;
            }
        }

        public int GetCoinCount()
        {
            if (NCMBUser.CurrentUser != null && NCMBUser.CurrentUser.ContainsKey(NCMBUserKey.COINCOUNT))
            {
                return Convert.ToInt32(NCMBUser.CurrentUser[NCMBUserKey.COINCOUNT]);
            }

            return 0;
        }

        public void AddKillCount(int num)
        {
            if (NCMBUser.CurrentUser.ContainsKey(NCMBUserKey.KILLCOUNT))
            {
                NCMBUser.CurrentUser[NCMBUserKey.KILLCOUNT] = GetKillCount() + num;
            }
            else
            {
                NCMBUser.CurrentUser.Add(NCMBUserKey.KILLCOUNT, num);
            }
        }

        public int GetKillCount()
        {
            if (NCMBUser.CurrentUser.ContainsKey(NCMBUserKey.KILLCOUNT))
            {
                return Convert.ToInt32(NCMBUser.CurrentUser[NCMBUserKey.KILLCOUNT]);
            }

            return 0;
        }

        public void SetOwnGraveObjectRef(NCMBObject ncmbObject)
        {
            NCMBUser.CurrentUser[NCMBUserKey.LASTGRAVEINFO] = ncmbObject;
        }

        public IEnumerator SaveAsyncCurrentUserCoroutine(UnityAction<NCMBException> errorCallback)
        {
            bool isConnecting = true;

            NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
        }

        public bool HasPlayerLastGraveObject
        {
            get { return NCMBUser.CurrentUser != null && NCMBUser.CurrentUser.ContainsKey(NCMBUserKey.LASTGRAVEINFO); }
        }

        public NCMBObject GetPlayerLastGraveObject()
        {
            if (NCMBUser.CurrentUser != null)
            {
                return (NCMBObject)NCMBUser.CurrentUser[NCMBUserKey.LASTGRAVEINFO];
            }
            else
            {
                return null;
            }
        }

        public string GetNickName()
        {
            if (NCMBUser.CurrentUser != null)
            {
                return NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string;
            }
            else
            {
                return "Unknown";
            }
        }

        public IEnumerator FetchLoginDateSpan(UnityAction<NCMBException> errorCallback)
        {
            //最終セーブ時刻と端末の日付が1日またいでいるか確認//
            if ((DateTime.Now.Date - GetUpdateTime().Date).Days == 0)
            {
                yield return 0;
                yield break;
            }

            //Now.Dateは信用できないのでNCMBの日付を更新して厳密にチェックする//

            //最後のupdate日付を取得//
            DateTime lastUpdateDate = GetUpdateTime().Date;

            //NCMBUserは保存をする必要が無い場合は通信を行わないため、DailyBonusフラグを変更する//
            bool value = (bool)NCMBUser.CurrentUser[NCMBUserKey.DAILYBONUSFLAG];
            NCMBUser.CurrentUser[NCMBUserKey.DAILYBONUSFLAG] = !value;

            //日付更新のためにセーブ実行//
            bool isConnecting = true;
            NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            //更新後のupdate日と比較して日数を返す//
            yield return (GetUpdateTime().Date - lastUpdateDate).Days;
        }

        public void AddFriend(string objectId)
        {
            List<string> friendList
                = GetStringListFromNCMBUserField(NCMBUserKey.FRIENDID_LIST);

            friendList.Add(objectId);

            NCMBUser.CurrentUser[NCMBUserKey.FRIENDID_LIST] = new ArrayList(friendList);

            NCMBUser.CurrentUser.SaveAsync();
        }

        public void RemoveFriend(string objectId)
        {
            List<string> friendList
                = GetStringListFromNCMBUserField(NCMBUserKey.FRIENDID_LIST);

            friendList.Remove(objectId);

            NCMBUser.CurrentUser[NCMBUserKey.FRIENDID_LIST] = new ArrayList(friendList);
            NCMBUser.CurrentUser.SaveAsync();
        }

        public void AddCardId(string cardId)
        {
            List<string> cardList = GetCardIdList();

            if (!cardList.Contains(cardId))
            {
                cardList.Add(cardId);
            }

            //Arrayに戻さないとローカルのCurrentUserからデータが取得できなくなる//
            NCMBUser.CurrentUser[NCMBUserKey.CARDIDLIST] = new ArrayList(cardList);
            NCMBUser.CurrentUser.SaveAsync();
        }

        public List<string> GetFriendIdList()
        {
            return GetStringListFromNCMBUserField(NCMBUserKey.FRIENDID_LIST);
        }

        public List<string> GetCardIdList()
        {
            return GetStringListFromNCMBUserField(NCMBUserKey.CARDIDLIST);
        }

        private List<string> GetStringListFromNCMBUserField(string key)
        {
            if (NCMBUser.CurrentUser.ContainsKey(key))
            {
                ArrayList arrayList = NCMBUser.CurrentUser[key] as ArrayList;

                if (arrayList != null)
                {
                    return arrayList.ToList<string>();
                }
            }

            return new List<string>();
        }
    }

}