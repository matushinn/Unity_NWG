using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattleMainUI : UIBase
    {
        public List<GraveNameText> graveNameTextList = new List<GraveNameText>();

        public Text killCounterText;
        public Text coinCounterText;

        public InputField playerDeathMessageField;

        public Button pauseButton;
        public Button screenShotButton;
        public Button attackButton;
        public JoyStick joyStick;

        public TextFadeInOutAnimation cardEffect;

        public Image myCardImage;
        public Button myCardButton;
        public Image friendCardImage;
        public Button friendCardButton;
        public Text friendCardNickNameText;

        public override void Awake()
        {
            base.Awake();
            graveNameTextList.ForEach(graveNameText => graveNameText.enabled = false);
        }

        public void SetActiveVirtualPads(bool value)
        {
            joyStick.gameObject.SetActive(value);
            attackButton.gameObject.SetActive(value);
        }

        public void SetKillCounter(int count)
        {
            killCounterText.text = count.ToString();
        }

        public void SetCoinCounter(int count)
        {
            coinCounterText.text = count.ToString();
        }

        public void PauseGraveNameText()
        {
            graveNameTextList.ForEach(graveNameText => graveNameText.Pause());
        }

        public void ResumeGraveNameText()
        {
            graveNameTextList.ForEach(graveNameText => graveNameText.Resume());
        }

        public void ShowGraveNickNameInField(GraveObject graveObject)
        { 
            if (graveNameTextList.Any(graveNameText => graveNameText.ReratedGraveObject == graveObject) == false)
            {
                GraveNameText graveNameText = graveNameTextList.FirstOrDefault(t => t.enabled == false);

                if (graveNameText != null)
                {       
                    graveNameText.ShowGraveNickNameAndSetRelation(graveObject.graveMessagePoint, graveObject);
                }
            }
        }

        public void SetMyCardImage(Texture2D texture)
        {
            myCardImage.sprite = Sprite.Create(texture, CardManager.CARDIMAGESIZE, Vector2.zero);
            myCardImage.enabled = true;
        }

        public void SetFriendCardImageAndName(Texture2D texture, string nickName)
        {
            friendCardImage.sprite = Sprite.Create(texture, CardManager.CARDIMAGESIZE, Vector2.zero);
            friendCardImage.enabled = true;

            friendCardNickNameText.text = nickName;

            friendCardButton.enabled = true;
        }

        public void SetDisappearFriendCard()
        {
            friendCardButton.interactable = false;
            friendCardNickNameText.text = string.Empty;
        }

        public string GetPlayerDeathMessage()
        {
            return playerDeathMessageField.text;
        }

        public void DisableButtons()
        {
            graveNameTextList.ForEach(graveNameText => graveNameText.Clear());
            pauseButton.interactable = false;
            screenShotButton.interactable = false;
            attackButton.interactable = false;

            joyStick.enabled = false;
            myCardButton.interactable = false;
            friendCardButton.interactable = false;
        }

        public void CardTextEffect(string str)
        {
            cardEffect.Play(str);
        }

        public void SetDisableMyCard()
        {
            myCardButton.interactable = false;
        }

        public void SetDisableFriendCard()
        {
            friendCardButton.interactable = false;
        }
    }
}