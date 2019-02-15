using System.Collections;
using UnityEngine;
using Communication;


namespace Title
{
    public class AutoLogInTitleSceneController : MonoBehaviour
    {
        private UserAuth userAuth;
        private PlayerInfoHandler playerInfoHandler;
        private GameSettingManager gameSettingManager;

        public AutoLoginTitleUI autoLoginTitleUI;

        public DeviceCodeOutputUI deviceCodeOutputUI;
        public DeviceCodeInputUI deviceCodeInputUI;

        private void Awake()
        {
            userAuth = UserAuth.Instance;
            playerInfoHandler = PlayerInfoHandler.Instance;
            gameSettingManager = GameSettingManager.Instance;

            autoLoginTitleUI.SetBuildNumber(Main.Instance.BuildNumber);
        }

        public void Start()
        {
            CheckSavedUserNameExist();

            deviceCodeOutputUI.Hide();
            deviceCodeInputUI.Hide();

            StartCoroutine(LoadGameSetting());
        }

        //データストアからゲーム設定をロードする//
        public IEnumerator LoadGameSetting()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Main.Instance.ShowErrorDialogue("ネットワーク接続がありません。");
                yield break;
            }

            Main.Instance.ShowLoadingPanel();

            yield return gameSettingManager.FetchGameSetting();

            string updateDateStr = gameSettingManager.GetUpdateDateString();

            if (string.IsNullOrEmpty(updateDateStr))
            {
                autoLoginTitleUI.SetGameSettingUpdateDate("ゲーム設定ファイルが見つかりません。");
            }
            else
            {
                autoLoginTitleUI.SetGameSettingUpdateDate("update: " + updateDateStr);
            }

            //サービス中か確認//
            if (gameSettingManager.IsServiceEnable() == false)
            {
                Main.Instance.ShowErrorDialogue("サーバーメンテナンス中です。");
            }

            Main.Instance.HideLoadingPanel();
        }

        private void CheckSavedUserNameExist()
        {
            //ローカルにオートログイン用のIDが保存されているか調べる//
            if (string.IsNullOrEmpty(userAuth.LocalSavedUserName))
            {
                //初めからボタンを表示//
                autoLoginTitleUI.ShowNewGame();
            }
            else
            {
                //続きからボタンを表示//
                autoLoginTitleUI.ShowContinueGame(userAuth.LocalSavedUserName);
            }
        }

        public void ShowNickNameInputDialogue()
        {
            if (CheckGameValid() == false) return;

            if (gameSettingManager.IsLoaded)
            {
                autoLoginTitleUI.ShowNickNameDialogue();
            }
            else
            {
                autoLoginTitleUI.HideNickNameDialogue();
                StartCoroutine(LoadGameSetting());
            }
        }

        public void OnNickNameCheck()
        {
            string nickName = autoLoginTitleUI.GetInputFieldNickName();

            if (!string.IsNullOrEmpty(nickName))
            {
                //禁止ワードチェック//
                if (Main.Instance.HasBadWord(nickName))
                {
                    Main.Instance.ShowErrorDialogue("ニックネームにできない文字が含まれています");
                    return;
                }

                autoLoginTitleUI.HideNickNameDialogue();
                StartCoroutine(AutoSignUpSequence(nickName));
            }
        }

        private IEnumerator AutoSignUpSequence(string nickName)
        {
            Main.Instance.ShowLoadingPanel();

            yield return userAuth.AutoSignUpCoroutine(nickName, Main.Instance.ForceToTitle);

            yield return playerInfoHandler.CreateOwnDataCoroutine(nickName: nickName, userName: userAuth.LocalSavedUserName, errorCallback:Main.Instance.ForceToTitle);

            yield return userAuth.SaveAsyncCurrentUserCoroutine(Main.Instance.ForceToTitle);

            Main.Instance.OnMenu();
        }

        //続きからボタンから呼ばれる//
        public void AutoLogin()
        {
            if (gameSettingManager.IsLoaded)
            {
                if (CheckGameValid() == false) return;

                StartCoroutine(AutoLoginSequence());
            }
            else
            {
                StartCoroutine(LoadGameSetting());
            }
        }

        private IEnumerator AutoLoginSequence()
        {
            Main.Instance.ShowLoadingPanel();

            yield return userAuth.AutoLogInCoroutine(Main.Instance.ForceToTitle);

            Main.Instance.OnMenu();
        }

        private void DeleteAutoLoginData()
        {
            if (CheckGameValid() == false) return;

            StartCoroutine(DeleteAutoLoginDataSequence());
        }

        private IEnumerator DeleteAutoLoginDataSequence()
        {
            Main.Instance.ShowLoadingPanel();

            //一度ログインする//
            yield return userAuth.AutoLogInCoroutine(Main.Instance.ShowErrorDialogue);

            //自分のPlayerInfoを削除する//
            yield return playerInfoHandler.DeleteOwnDataCoroutine(Main.Instance.ShowErrorDialogue);

            //会員管理から削除する//
            yield return userAuth.DeleteUserAsyncCoroutine(Main.Instance.ShowErrorDialogue);

            //端末のID・パスワードを削除する//
            userAuth.DeleteLocalUserNameAndPassword();

            //初めからモードにする//
            autoLoginTitleUI.ShowNewGame();

            Main.Instance.HideLoadingPanel();
        }

        //ボタンから呼ばれる//
        public void ShowDeleteAccount()
        {
            if (CheckGameValid() == false) return;
 
            autoLoginTitleUI.ShowDialogueButton("ゲームサーバーからプレイヤーデータを完全に削除します。\n よろしいですか？", DeleteAutoLoginData);
        }

        public void ShowDeviceCodeOutputWarningDialogue()
        {
            autoLoginTitleUI.ShowDialogueButton("引継ぎコードを発行すると、本端末の情報は削除されます。\nよろしいですか？", ShowDeviceCodeOutput);
        }

        public void ShowDeviceCodeInputWarningDialogue()
        {
            if(CheckGameValid() == false) return;

            //端末に保存されたユーザーIDがあるか確認//
            if (string.IsNullOrEmpty(userAuth.LocalSavedUserName))
            {
                //無ければすぐ入力画面に遷移//
                ShowDeviceCodeInput();
            }
            else
            {
                autoLoginTitleUI.ShowDialogueButton("引継ぎコードを入力すると、本端末の情報は上書きされます。\nよろしいですか？", ShowDeviceCodeInput);
            }
        }

        public void HideDialogue()
        {
            autoLoginTitleUI.HideDialogue();
        }

        private void ShowDeviceCodeOutput()
        {
            string userName = userAuth.LocalSavedUserName;
            string password = userAuth.LocalSavedPassword;
            deviceCodeOutputUI.Show(userName, password);

            userAuth.DeleteLocalUserNameAndPassword();
        }

        public void HideDeveiceCodeOutput()
        {
            deviceCodeOutputUI.Hide();
            CheckSavedUserNameExist();
        }

        private void ShowDeviceCodeInput()
        {
            deviceCodeInputUI.Show();
        }

        public void CheckDeviceCodeInput()
        {
            if (CheckGameValid() == false) return;

            StartCoroutine(CheckDevieCodeSequence());
        }

        private IEnumerator CheckDevieCodeSequence()
        {
            deviceCodeInputUI.DisableInputOKButton();

            string userName = deviceCodeInputUI.GetInputUserNameText();
            string password = deviceCodeInputUI.GetInputPasswordText();

            yield return userAuth.LogInCoroutine(userName, password, Main.Instance.ForceToTitle);

            //成功したらローカルに書き込む//
            userAuth.SaveLocalUserNameAndPassword(userName, password);

            //プレイヤー自身のPlayerInfoを取得する
            yield return playerInfoHandler.FetchOwnDataCoroutine(Main.Instance.ForceToTitle);

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            //Installationを再設定する//
            yield return playerInfoHandler.SaveInstallationToOwnData(Main.Instance.ForceToTitle);
#endif

            Main.Instance.OnMenu();
        }

        public void HideDeveiceCodeInput()
        {
            deviceCodeInputUI.Hide();
        }

        public void OnButtonShowTermsOfUse()
        {
            string text = gameSettingManager.GetTermsOfUse();
            autoLoginTitleUI.ShowTermsOfUse(text);
        }

        public void OnButtonHideTermsOfUse()
        {
            autoLoginTitleUI.HideTermsOfUse();
        }

        public void OnButtonShowCredits()
        {
            autoLoginTitleUI.ShowCredits();
        }

        public void OnButtonHideCredits()
        {
            autoLoginTitleUI.HideCredits();
        }

        //ゲームが有効か調べ、NGの場合はダイアログを表示する//
        private bool CheckGameValid()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Main.Instance.ForceToTitle("ネットワーク接続がありません。");
                return false;
            }

            //サービス中か確認//
            if (gameSettingManager.IsServiceEnable() == false)
            {
                Main.Instance.ForceToTitle("サーバーメンテナンス中です。");
                return false;
            }

            return true;
        }

    }
}