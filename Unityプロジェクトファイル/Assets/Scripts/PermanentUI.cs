using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 全シーンをまたいで存在するUIの管理クラス。「...Loading」「ネットワークエラー」表示。//
/// </summary>

public class PermanentUI : MonoBehaviour
{
    public CanvasGroup loadingCanvasGroup;
    public TextTransition connectingAnimation;
    public GameObject retryButtonObject;
    private Button retryButton;

    public CanvasGroup dialogueCanvasGroup;
    public Text dialogueText;
    public Button yesButton;
    public Button noButton;
    public Button okButton;

    private void Awake()
    {
        retryButton = retryButtonObject.GetComponent<Button>();

        DontDestroyOnLoad(this.gameObject);
        loadingCanvasGroup.Hide();
        dialogueCanvasGroup.Hide();
    }

    public void ShowLoading()
    {
        retryButtonObject.SetActive(false);
        retryButton.onClick.RemoveAllListeners();

        loadingCanvasGroup.Show();

        connectingAnimation.enabled = true;
    }

    public void ShowBackToTitleButton()
    {
        retryButtonObject.SetActive(true);
        retryButton.onClick.RemoveAllListeners();
        //retryButton.onClick.AddListener(buttonEvent);
    }

    public void HideLoading()
    {
        loadingCanvasGroup.Hide();
        connectingAnimation.enabled = false;
    }

    public void ShowYN(string message, UnityAction yesCallback)
    {
        ClearAllButton();

        dialogueText.text = message;

        yesButton.gameObject.SetActive(true);
        yesButton.onClick.AddListener(yesCallback);
        noButton.gameObject.SetActive(true);

        dialogueCanvasGroup.Show();
    }

    public void ShowOK(string message, UnityAction agreeCallback = null)
    {
        ClearAllButton();

        dialogueText.text = message;

        okButton.gameObject.SetActive(true);

        if (agreeCallback != null)
        {
            okButton.onClick.AddListener(agreeCallback);
        }

        dialogueCanvasGroup.Show();
    }

    void ClearAllButton()
    { 
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(dialogueCanvasGroup.Hide);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(dialogueCanvasGroup.Hide);

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(dialogueCanvasGroup.Hide);

        okButton.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }
}