using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class ScreenShotSaveUI : UIBase
    {
        public RawImage screenShotConfirm;
        public Text messageText;

        public Button yesButton;
        public Button noButton;

        public void Show(Texture2D tex)
        {
            base.Show();
            screenShotConfirm.texture = tex;

            yesButton.interactable = true;
            noButton.interactable = true;

            messageText.text = "このスクリーンショットを保存しますか？";
        }

        public void DisableAllButtons()
        {
            yesButton.interactable = false;
            noButton.interactable = false;

            messageText.text = "保存中...";
        }
    }
}