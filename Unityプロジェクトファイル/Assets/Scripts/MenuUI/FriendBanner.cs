using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Menu
{
    public class FriendBanner : MonoBehaviour
    {
        public Button button;
        public Text nickName;
        public Text killCount;
        public Image card;

        public UnityAction yesButtonAction;

        public void Set(UnityAction yesButtonAction, string nickName, string killCount, Texture2D cardTexture = null)
        {
            this.gameObject.SetActive(true);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(yesButtonAction);

            this.nickName.text = nickName;
            this.killCount.text = killCount;

            if(cardTexture != null)
            {
                card.enabled = true;
                card.sprite = Sprite.Create(cardTexture, CardManager.CARDIMAGESIZE, Vector2.zero);
            }
            else
            {
                card.enabled = false;
            }

        }
    }
}