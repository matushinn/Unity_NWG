using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Menu
{

    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class CardBanner : MonoBehaviour
    {
        public Button button;
        public Image image;
        public Text cardName;
        private string cardId;

        public bool Enabled { get { return button.interactable; } }

        private void Awake()
        {
            button.interactable = false;
            cardName.text = "????";
        }

        public void Set(CardData cardData, UnityAction onButton)
        {
            this.cardId = cardData.id;

            Texture2D texture = CardManager.Instance.GetTextureByCardId(cardId);
            image.sprite = Sprite.Create(texture, CardManager.CARDIMAGESIZE, Vector2.zero);
            button.interactable = true;
            this.cardName.text = cardData.name + "\n x "+ cardData.point ;

            button.onClick.AddListener(onButton);
        }
        
        public void Disable()
        {
            button.interactable = false;
        }

        public void Clear()
        {
            image.sprite = null;
            button.interactable = false;
            cardName.text = "????";

            button.onClick.RemoveAllListeners();
        }
    }
}
