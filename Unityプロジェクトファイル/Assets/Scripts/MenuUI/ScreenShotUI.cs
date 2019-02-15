using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenShotUI : UIBase
    {
        public ScrollRect scrollRect;
        public List<ScreenShotView> screenShotViewList = new List<ScreenShotView>();

        public override void Show()
        {
            base.Show();
            scrollRect.horizontalNormalizedPosition = 0f;
            screenShotViewList.ForEach(view => view.Clear());
        }

        public void SetSceenShotToImage(string nickName, Texture2D texture)
        {
            ScreenShotView ssv = screenShotViewList.FirstOrDefault(view => view.Empty == true);
            if(ssv != null)
            {
                ssv.SetScreenShot(nickName, texture);
            }
        }

        public override void Hide()
        {
            screenShotViewList.ForEach(view => view.Clear());
            base.Hide();
        }
    }
}