using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Battle
{
    public class OthersGraveInfoUI : UIBase
    {
        public Text nickNameText;
        public Text deathMessageText;

        public void Show(string deathMessage, string userNameAndDate)
        {
            nickNameText.text = userNameAndDate;
            deathMessageText.text = deathMessage;

            base.Show();
        }

    }

}