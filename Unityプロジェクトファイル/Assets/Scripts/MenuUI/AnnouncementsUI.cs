using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class AnnouncementsUI : UIBase
    {
        public ScrollRect scrollRect;
        public List<AnnounceBanner> bannerList = new List<AnnounceBanner>();

        public GameObject messagePanelObject;
        public Text messagePanelTitleText;
        public Text messagePanelMainText;

        public Text statusText;

        private void Start()
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        public void SetAnnounceList(List<AnnouncementInfo> infoList)
        {
            statusText.text = string.Empty;

            foreach (AnnouncementInfo info in infoList)
            {
                AnnounceBanner banner = bannerList.Find(b => b.gameObject.activeSelf == false);

                banner.gameObject.SetActive(true);
                banner.SetBanner(info, () =>
                {
                    messagePanelObject.SetActive(true);
                    messagePanelTitleText.text = info.title;
                    messagePanelMainText.text = info.mainText;
                });
            }
        }

        public void HideMessage()
        {
            messagePanelObject.SetActive(false);
        }

        public void NoAnnounce()
        {
            statusText.text = "お知らせはありません";
        }

        public override void Show()
        {
            base.Show();
            statusText.text = "接続中...";
            bannerList.ForEach(banner => banner.gameObject.SetActive(false));
        }

        public override void Hide()
        {
            scrollRect.verticalNormalizedPosition = 1f;
            messagePanelObject.SetActive(false);
            base.Hide();
        }
    }
}