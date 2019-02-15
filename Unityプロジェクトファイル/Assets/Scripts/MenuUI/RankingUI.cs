using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Menu
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class RankingUI : UIBase
    {
        public Text messageText;

        public List<RankingBanner> rankingBannerList = new List<RankingBanner>();

        public override void Show()
        {
            base.Show();

            messageText.text = "接続中...";
            rankingBannerList.ForEach(banner => banner.Hide());
        }

        public void SetRankingData(List<RankingInfo> rankList)
        {
            if (rankList == null || rankList.Count == 0)
            {
                messageText.text = "ランキングデータがありません";
                return;
            }

            int i = 0;

            foreach (RankingInfo rankingInfo in rankList)
            {
                rankingBannerList[i].Set(rankingInfo.nickName, rankingInfo.killCount, rankingInfo.updateDateStr);
                i++;
            }

            messageText.text = string.Empty;
        }
    }

}