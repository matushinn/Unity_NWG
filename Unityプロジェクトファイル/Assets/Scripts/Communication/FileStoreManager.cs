using NCMB;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Communication
{
    public class FileStoreManager : SingletonMonoBehaviour<FileStoreManager>
    {
        private string bannerFileCachePath;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            bannerFileCachePath = Application.persistentDataPath + "/" + "MainMenuBanner.png";

#if UNITY_iOS
        //iCloud バックアップ不要ファイルに設定する//
        UnityEngine.iOS.Device.SetNoBackupFlag(bannerFileCachePath);
#endif
        }

        public IEnumerator SaveTextureCoroutine(
            Texture2D texture,
            UnityAction<NCMBException> errorCallback)
        {
            if (NCMBUser.CurrentUser == null)
            {
                yield break;
            }

            byte[] file = texture.EncodeToJPG(95);

            string fileName = NCMBUser.CurrentUser.UserName
                + "_"
                + Utility.GenerateRandomAlphanumeric(18)
                + "_ScreenShot.jpg";

            bool isConnecting = true;
            NCMBFile ncmbFile = new NCMBFile(fileName, file);
            ncmbFile.SaveAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                    fileName = null;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return fileName;
        }

        public IEnumerator FetchTextureCoroutine(string fileName, UnityAction<NCMBException> errorCallback)
        {
            Texture2D texture = null;
            bool isConnecting = true;

            NCMBFile file = new NCMBFile(fileName);
            file.FetchAsync((byte[] fileData, NCMBException e) =>
            {
                if (e != null)
                {
                    errorCallback(e);
                }
                else
                {
                // 成功
                texture = new Texture2D(128, 128);
                    texture.LoadImage(fileData);
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            yield return texture;
        }

        public IEnumerator FetchBannerFileCoroutine(string bannerFileName, UnityAction<NCMBException> errorCallback)
        {
            NCMBFile file = new NCMBFile(bannerFileName);

            byte[] fileData = null;

            bool isConnecting = true;

            file.FetchAsync((byte[] _fileData, NCMBException e) =>
            {
                if (e != null)
                {
                // 失敗
                if (e.ErrorCode == "E404001")
                    {
                        Debug.LogWarning("指定されたファイル名はファイルストアに存在していません");
                        DeleteBannerCacheFile();
                    }
                    else
                    {
                        errorCallback(e);
                    }
                }
                else
                {
                // 成功
                fileData = _fileData;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            if (fileData != null)
            {
                SaveBannerCacheFile(fileData);
            }
        }

        private void SaveBannerCacheFile(byte[] data)
        {
            FileStream fileSave = new FileStream(bannerFileCachePath, FileMode.Create);
            BinaryWriter binary = new BinaryWriter(fileSave);

            binary.Write(data);
            fileSave.Close();
        }

        public void DeleteBannerCacheFile()
        {
            if (File.Exists(bannerFileCachePath))
            {
                File.Delete(bannerFileCachePath);
            }
        }

        public Texture2D GetBannerCacheFile()
        {
            if (File.Exists(bannerFileCachePath))
            {
                byte[] bytes = File.ReadAllBytes(bannerFileCachePath);

                Texture2D texture = new Texture2D(128, 128);
                texture.LoadImage(bytes);

                return texture;
            }

            return null;
        }
    }
}