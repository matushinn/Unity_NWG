using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class DailyBonusUI : UIBase
    {
        public Text message;
        public Text touchScreenMessage;
        public Button hiddenButton;

        public void Show(int days)
        {
            message.text = days + "日ぶりのログイン！";

            hiddenButton.interactable = false;
            touchScreenMessage.enabled = false;

            StartCoroutine(ButtonEnableTimer(0.5f));

            base.Show();
        }

        private IEnumerator ButtonEnableTimer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            hiddenButton.interactable = true;
            touchScreenMessage.enabled = true;
        }

        public override void  Hide()
        {
            touchScreenMessage.enabled = false;
            base.Hide();
        }
    }
}