using UnityEngine;
using UnityEngine.UI;
namespace Menu
{
    public class RankingBanner : MonoBehaviour
    {
        public Text nickName;
        public Text killCount;
        public Text dateText;
        private Image thisImage;
        public Text rankNumber;

        private void Awake()
        {
            thisImage = GetComponent<Image>();
        }

        public void Set(string nickName, int killCount, string updateDate)
        {
            this.nickName.text = nickName;
            this.killCount.text = killCount.ToString() + " Kill";
            this.dateText.text = updateDate;

            rankNumber.enabled = true;
            thisImage.enabled = true;
        }

        public void Hide()
        {
            this.nickName.text = string.Empty;
            this.killCount.text = string.Empty;
            this.dateText.text = string.Empty;
            rankNumber.enabled = false;
            thisImage.enabled = false;
        }
    }
}