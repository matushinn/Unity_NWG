using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Menu
{
    public class CardListUI : UIBase
    {
        public List<CardBanner> cardBannerList = new List<CardBanner>();

        public void Set(List<string> inventoryCardIdList, UnityAction<string> selectCardCallback)
        {
            foreach (string cardId in inventoryCardIdList)
            {
                CardBanner cardBanner = cardBannerList.First(cb => cb.Enabled == false);

                CardData cardData = CardManager.Instance.GetCardDataByCardId(cardId);
                cardBanner.Set(cardData, () => { selectCardCallback(cardId); });
            }
        }

        public override void Hide()
        {
            cardBannerList.ForEach(cb => cb.Clear());
            base.Hide();
        }
    }
}