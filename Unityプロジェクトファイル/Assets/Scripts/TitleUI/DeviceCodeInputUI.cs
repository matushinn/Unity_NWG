using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class DeviceCodeInputUI : UIBase
    {
        public InputField inputId;
        public InputField inputPass;

        public Button inputOkButton;
        public Text inputIdIsWrongMessage;

        public override void Show()
        {
            inputOkButton.enabled = true;
            base.Show();
        }

        public void DisableInputOKButton()
        {
            inputOkButton.enabled = false;
        }

        public string GetInputUserNameText()
        {
            return inputId.text;
        }

        public string GetInputPasswordText()
        {
            return inputPass.text;
        }

        public override void Hide()
        {
            base.Hide();

            inputId.text = string.Empty;
            inputPass.text = string.Empty;
            inputIdIsWrongMessage.text = string.Empty;
        }

        public void RetryInput()
        {
            inputOkButton.enabled = true;
            inputIdIsWrongMessage.text = "もう一度入力してください";
        }
    }
}