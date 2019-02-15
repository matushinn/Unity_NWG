using System.Collections;
using UnityEngine;
using Communication;

namespace Battle
{
    public class ScreenShotManager : MonoBehaviour
    {
        private string screenShotsPath;
        public MainCamera mainCamera;

        private Texture2D currentScreenShot;

        public BattleSceneController battleSceneController;
        public ScreenShotSaveUI screenShotSaveUI;
        private PlayerInfoHandler playerInfoHandler;
        private FileStoreManager fileStoreManager;

        private void Awake()
        {
            fileStoreManager = FileStoreManager.Instance;
            playerInfoHandler = PlayerInfoHandler.Instance;
        }

        private void Start()
        {
#if UNITY_iOS
            //iCloud バックアップ不要ファイルに設定する//
            UnityEngine.iOS.Device.SetNoBackupFlag(screenShotsPath);
#endif
        }

        public void CancelScreenShotSave()
        {
            screenShotSaveUI.Hide();
            battleSceneController.InnnerResume();
        }

        public void AcceptScreenShotSave()
        {
            StartCoroutine(SaveScreenShot(currentScreenShot));
        }

        private IEnumerator SaveScreenShot(Texture2D texture)
        {
            screenShotSaveUI.DisableAllButtons();

            IEnumerator coroutine
                = fileStoreManager.SaveTextureCoroutine(
                    texture, 
                    Main.Instance.ForceToTitle
                    );

            yield return StartCoroutine(coroutine);

            string fileName = (string)coroutine.Current;

            yield return playerInfoHandler.SaveScreenShotNameToOwnData(
                fileName, 
                Main.Instance.ForceToTitle
                );

            screenShotSaveUI.Hide();

            battleSceneController.InnnerResume();
        }

        //ボタンはここからスタート//
        public void ScreenShot()
        {
            mainCamera.TakeScreenShotOnNextFlag(ShowScreenshotSaveWindow);
            Debug.Log("Saved Screenshots");
        }

        //main camera からスクリーンショットを受け取って発火//
        public void ShowScreenshotSaveWindow(Texture2D texture)
        {
            currentScreenShot = texture;
            screenShotSaveUI.Show(currentScreenShot);
            battleSceneController.InnerPause();
        }
    }
}