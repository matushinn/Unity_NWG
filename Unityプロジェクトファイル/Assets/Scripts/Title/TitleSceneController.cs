using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Communication;

namespace Title
{
    public class TitleSceneController : MonoBehaviour
    {
        private UserAuth userAuth;
        private PlayerInfoHandler playerInfoHandler;
        private GameSettingManager gameSettingManager;

        public InputField userNameInputField;
        public InputField passwordInputField;
        public InputField mailInputField;
        
        public LogInUI logInUI;
        public SignUpUI signUpUI;

        public Text buildNumberText;
        public Text settingFileDateText;

        private void Awake()
        {
            userAuth = UserAuth.Instance;
            playerInfoHandler = PlayerInfoHandler.Instance;
            gameSettingManager = GameSettingManager.Instance;
        }

        public void Start()
        {
            OnLogInMode();

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

            if (string.IsNullOrEmpty(gameSettingManager.GetUpdateDateString()))
            {
                settingFileDateText.text = "No Setting Data Found";
            }
            else
            {
                settingFileDateText.text = gameSettingManager.GetUpdateDateString();
            }

            if(gameSettingManager.IsServiceEnable() == false)
            {
                Main.Instance.ShowErrorDialogue("サーバーメンテナンス中です。");
            }

            Main.Instance.HideLoadingPanel();
        }

        public void OnLogInMode()
        {
            logInUI.Show();
            signUpUI.Hide();

            mailInputField.text = string.Empty;

            buildNumberText.text = Main.Instance.BuildNumber;
        }

        public void OnSignUpMode()
        {
            logInUI.Hide();
            signUpUI.Show();
        }

        public void OnButtonStartLogIn()
        {
            if (gameSettingManager.IsLoaded)
            {
                if (CheckGameValid() == false) return;

                StartCoroutine(LogInSequence());

            } else { 

                StartCoroutine(LoadGameSetting());
            }
        }

        private IEnumerator LogInSequence()
        {
            string userName = userNameInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password))
            {
                Main.Instance.ShowErrorDialogue("IDとパスワードを入力してください。");
                yield break;
            }

            yield return userAuth.LogInCoroutine(userName, password, Main.Instance.ForceToTitle);

            Main.Instance.OnMenu();
        }

        public void OnButtonStartSignUp()
        {
            if (gameSettingManager.IsLoaded)
            {
                if (CheckGameValid() == false) return;

                StartCoroutine(SignUpSequence());
            }
            else
            {
                StartCoroutine(LoadGameSetting());
            }
        }

        public IEnumerator SignUpSequence()
        {
            string userName = userNameInputField.text;
            string password = passwordInputField.text;
            string mail = mailInputField.text;

            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(mail))
            {
                Main.Instance.ShowErrorDialogue("IDとパスワード、メールアドレスを入力してください。");
                yield break;
            }

            yield return userAuth.SignUpCoroutine(userName, mail, password, Main.Instance.ForceToTitle);

            //オートログインモードでない場合はユーザーIDとニックネームは同一//
            yield return playerInfoHandler.CreateOwnDataCoroutine(userName, userName, Main.Instance.ForceToTitle);

            yield return userAuth.SaveAsyncCurrentUserCoroutine(Main.Instance.ForceToTitle);

            Main.Instance.OnMenu();
        }


        //ゲームが有効か調べ、NGの場合はダイアログを表示する//
        private bool CheckGameValid()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Main.Instance.ShowErrorDialogue("ネットワーク接続がありません。");
                return false;
            }

            //サービス中か確認//
            if (gameSettingManager.IsServiceEnable() == false)
            {
                Main.Instance.ShowErrorDialogue("サーバーメンテナンス中です。");
                return false;
            }

            return true;
        }
    }
}