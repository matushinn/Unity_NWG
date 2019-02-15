using NCMB;
using System.Collections.Generic;
using System.Linq;
using TSM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Main : SingletonMonoBehaviour<Main>
{
    private enum LoginMode { Manual, Auto };

    [SerializeField]
    private LoginMode loginMode = LoginMode.Manual;

    public bool IsAutoLoginMode { get { return loginMode == LoginMode.Auto; } }

    public PermanentUI permanentUI;
    public BadWordList badWordList;

    public bool HasBadWord(string message)
    {
        if(badWordList != null)
        {
            string[] badwords = badWordList.words;
            return badwords.Any(word => word.Contains(message));
        }
        return false;
    }

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        HideLoadingPanel();

#if UNITY_EDITOR

        if (SceneManager.GetSceneByName("Title").IsValid())
        {
        }
        else if (SceneManager.GetSceneByName("Battle").IsValid())
        {
            ShowLoadingPanel();
        }
        else if (SceneManager.GetSceneByName("Menu").IsValid())
        {
            ShowLoadingPanel();
        }
        else
        {
            OnTitle();
        }

#else
        OnTitle();
#endif
    }

    public void OnTitle()
    {
        SoundManager.Instance.ClearAudioListenerPos();

        if (loginMode == LoginMode.Auto)
        {
            SceneManager.LoadScene("AutoLogInTitle", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        }
    }

    public void OnBattle()
    {
        ShowLoadingPanel();
        SoundManager.Instance.ClearAudioListenerPos();
        SceneManager.LoadScene("Battle", LoadSceneMode.Single);
    }

    public void OnMenu()
    {
        SoundManager.Instance.ClearAudioListenerPos();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void ShowLoadingPanel()
    {
        permanentUI.ShowLoading();
    }

    public void HideLoadingPanel()
    {
        permanentUI.HideLoading();
    }

    public void ShowErrorDialogue(string message)
    {
        permanentUI.ShowOK(message);
    }

    public string BuildNumber
    {
        get
        {
            var buildManifest = CloudBuildManifest.Load();
            return buildManifest == null
                ? "Not Cloud Build"
                : "Build Number " + buildManifest.BuildNumber;
        }
    }

    public void ShowErrorDialogue(NCMBException exeption )
    {
        permanentUI.ShowOK(GetErrorMessageFromEroorCode(exeption));
    }

    public void ShowDialogueOK(string message, UnityAction callback = null)
    {
        permanentUI.ShowOK(message, callback);
    }

    public void ShowDialogueYN(string message, UnityAction callback)
    {
        permanentUI.ShowYN(message, callback);
    }

    public void ForceToTitle(NCMBException exeption = null)
    {
        if (exeption != null)
        {
            permanentUI.ShowOK(GetErrorMessageFromEroorCode(exeption));
        }

        OnTitle();
    }

    public void ForceToTitle(string message)
    {
        permanentUI.ShowOK(message);
        OnTitle();
    }

    public string GetErrorMessageFromEroorCode(NCMBException exeption)
    {
        if (exeption.ErrorCode == NCMBException.INCORRECT_PASSWORD)
        {
            return "IDとパスワードが一致しません。";
        }
        else if (exeption.ErrorCode == NCMBException.DUPPLICATION_ERROR)
        {
            return "そのIDは既に使われています。";
        }
        else if (exeption.ErrorCode == NCMBException.INVALID_FORMAT)
        {
            return "不正なメールアドレスフォーマットです。";
        }
        else if (exeption.ErrorCode == NCMBException.REQUEST_OVERLOAD)
        {
            //実際は月間利用上限数を超えている状態。//
            return "サーバーメンテナンス中です。";
        }
        else if (exeption.ErrorCode == "E503001")
        {
            //実際は503//
            return "503 サーバーメンテナンス中です。";
        }
        else
        {
            return "ネットワークエラーが発生しました。";
        }
    }

}