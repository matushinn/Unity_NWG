using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace Menu {
    public class GachaUI : UIBase
    {
        public Image cardImage;
        public Text cardText;
        public GameObject connectingImageObject;

        public Text gachaPriceText;
        public Text coinNumText;

    public void Show(int gachaPrice, int hasCoinNum)
        {
            base.Show();

            cardImage.sprite = null;
            cardImage.color = new Color(cardImage.color.r, cardImage.color.g, cardImage.color.b, 0.3f);
            cardText.text = "";

            gachaPriceText.text = "１回" + gachaPrice.ToString() + "コイン";
            SetCoinCount(hasCoinNum);
            connectingImageObject.SetActive(false);
        }


        public void ShowConnecting()
        {
            connectingImageObject.SetActive(true);
        }

        public void SetCoinCount(int hasCoinNum)
        {
            coinNumText.text = hasCoinNum.ToString();
        }

        public void ShowCard(CardData cardData)
        {
            connectingImageObject.SetActive(false);

            Texture2D texture = CardManager.Instance.GetTextureByCardId(cardData.id);
            cardImage.sprite = Sprite.Create(texture, CardManager.CARDIMAGESIZE, Vector2.zero);
            cardImage.color = new Color(cardImage.color.r, cardImage.color.g, cardImage.color.b, 1f);
            this.cardText.text = cardData.name + "\n x " + cardData.point;
        }

        public void ShowShortfall()
        {
            Main.Instance.ShowDialogueOK("コインが不足しています！");
        }
    }

}