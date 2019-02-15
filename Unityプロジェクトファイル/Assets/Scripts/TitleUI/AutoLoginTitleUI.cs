using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Title {
    public class AutoLoginTitleUI : MonoBehaviour
    {
        public CanvasGroup mainCanvasGroup;
        public CanvasGroup dialougeCanvasGroup;
        public CanvasGroup nickNameCanvasGroup;
        public CanvasGroup termsOfUseCanvasGroup;
        public CanvasGroup creditsCanvasGroup;

        public InputField nickNameInputField;
        public Button nickNameOkButton;

        public Text buildNumberText;
        public Text userNameText;
        public Button startNewGameButton;
        public Button continueGameButton;

        public Button dataClearButton;
        public Button deviceCodeOutputButton;
        public Button deviceCodeInputButton;

        public Text dialogueMessage;
        public Button dialogueYesButton;

        public Text updateDate;

        public Text termsOfUseText;

        public void Start()
        {
            dialougeCanvasGroup.Hide();
            nickNameCanvasGroup.Hide();
            termsOfUseCanvasGroup.Hide();
            creditsCanvasGroup.Hide();
        }

        public void SetBuildNumber(string buildNumber)
        {
            buildNumberText.text = buildNumber;
        }

        public void ShowContinueGame(string userName)
        {
            userNameText.text = "ID: " + userName;

            continueGameButton.gameObject.SetActive(true);
            startNewGameButton.gameObject.SetActive(false);

            //データクリアボタンを有効化//
            dataClearButton.interactable = true;
            //引継ぎ発行ボタンを有効化//
            deviceCodeOutputButton.interactable = true;
        }

        public void SetGameSettingUpdateDate(string updateDateStr)
        {
            updateDate.text = updateDateStr;
        }

        public void ShowNewGame()
        {
            userNameText.text = string.Empty;

            continueGameButton.gameObject.SetActive(false);
            startNewGameButton.gameObject.SetActive(true);

            //データクリアボタンを無効化//
            dataClearButton.interactable = false;
            //引継ぎ発行ボタンを無効化//
            deviceCodeOutputButton.interactable = false;
        }

        public void ShowDialogueButton(string message, UnityAction chooseYes)
        {
            dialougeCanvasGroup.Show();

            dialogueMessage.text = message;

            chooseYes += () => HideDialogue();

            dialogueYesButton.onClick.RemoveAllListeners();
            dialogueYesButton.onClick.AddListener(chooseYes);

            //MainUIのボタンを無効化する//
            mainCanvasGroup.blocksRaycasts = false;
        }

        public void HideDialogue()
        {
            dialougeCanvasGroup.Hide();

            //MainUIのボタンを有効化する//
            mainCanvasGroup.blocksRaycasts = true;
        }

        public void ShowNickNameDialogue()
        {
            nickNameCanvasGroup.Show();
            //MainUIのボタンを無効化する//
            mainCanvasGroup.blocksRaycasts = false;
        }

        public void HideNickNameDialogue()
        {
            nickNameCanvasGroup.Hide();
            //MainUIのボタンを有効化する//
            mainCanvasGroup.blocksRaycasts = true;
        }

        public string GetInputFieldNickName()
        {
            return nickNameInputField.text;
        }

        public void ShowTermsOfUse(string text)
        {
            termsOfUseText.text = text;
            termsOfUseCanvasGroup.Show();
            //MainUIのボタンを無効化する//
            mainCanvasGroup.blocksRaycasts = false;
        }

        public void HideTermsOfUse()
        {
            termsOfUseCanvasGroup.Hide();
            //MainUIのボタンを有効化する//
            mainCanvasGroup.blocksRaycasts = true;
        }

        public void ShowCredits()
        {
            creditsCanvasGroup.Show();
            //MainUIのボタンを無効化する//
            mainCanvasGroup.blocksRaycasts = false;
        }

        public void HideCredits()
        {
            creditsCanvasGroup.Hide();
            //MainUIのボタンを有効化する//
            mainCanvasGroup.blocksRaycasts = true;
        }
    }
}