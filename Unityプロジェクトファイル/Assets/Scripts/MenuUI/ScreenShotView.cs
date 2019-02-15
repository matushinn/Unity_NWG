using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class ScreenShotView : MonoBehaviour
    {
        private Image screenShotImage;
        private Color defaultColor;
        public Text nickNameText;
        public TextTransition loadingTextTransition;

        public bool Empty { get; private set; }

        private void Awake()
        {
            screenShotImage = GetComponent<Image>();
            defaultColor = screenShotImage.color;
        }

        public void SetScreenShot(string text, Texture2D texture)
        {
            nickNameText.text = text;

            //texture.Resizeは灰色のテクスチャが生成されるだけなので使用しない//
            screenShotImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            screenShotImage.color = Color.white;
            loadingTextTransition.enabled = false;
            Empty = false;
        }

        public void Clear()
        {
            nickNameText.text = string.Empty;
            screenShotImage.sprite = null;
            screenShotImage.color = defaultColor;
            loadingTextTransition.enabled = true;
            Empty = true;
        }
    }
}