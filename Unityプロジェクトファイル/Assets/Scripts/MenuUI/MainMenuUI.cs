using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenuUI : UIBase
    {
        public Text userNameText;
        public Text nickNameText;
        public Image bannerImage;
        public Text coinCountText;

        public Text DebugText;

        public Image equipCardImage;

        public void SetUserName(string userName)
        {
            userNameText.text = "ID: " + userName;
        }

        public void SetNickName(string nickName)
        {
            nickNameText.text = nickName;
        }

        public void SetBannerImage(Texture2D texture)
        {
            bannerImage.sprite = Sprite.Create(texture, new Rect(0, 0, 256, 64), Vector2.zero);
            bannerImage.enabled = true;
        }

        public void SetEquipCardImage(CardData cardData)
        {
            Texture2D texture = CardManager.Instance.GetTextureByCarData(cardData);

            equipCardImage.sprite = Sprite.Create(texture, CardManager.CARDIMAGESIZE, Vector2.zero);
            equipCardImage.enabled = true;
        }

        public void SetCoinCount(int num)
        {
            coinCountText.text = num.ToString();
        }

        public void ClearBanner()
        {
            bannerImage.sprite = null;
            bannerImage.enabled = false;
        }
    }
}