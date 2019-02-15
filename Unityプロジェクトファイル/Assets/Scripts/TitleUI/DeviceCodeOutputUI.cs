using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class DeviceCodeOutputUI : UIBase
    {
        public Text showIdText;
        public Text showPassText;

        public void Show(string showId, string showPass)
        {
            showIdText.text = showId;
            showPassText.text = showPass;

            base.Show();
        }

        public override void Hide()
        {
            base.Hide();

            showIdText.text = string.Empty;
            showPassText.text = string.Empty;
        }
    }
}