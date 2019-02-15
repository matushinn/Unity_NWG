using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class FriendsUI : UIBase
    {
        public Text titleText;
        public Text messageText;

        public GameObject friendScrollViewObject;
        public List<FriendBanner> friendListBannerList = new List<FriendBanner>();

        public GameObject otherPlayerScrollViewObject;
        public List<FriendBanner> otherPlayerListBannerList = new List<FriendBanner>();

        public InputField searchOtherPlayerInputField;

        public ScrollRect friendScrollRect;
        public ScrollRect otherPlayerScrollRect;

        public void ShowFriendListConnecting()
        {
            friendScrollViewObject.SetActive(true);
            otherPlayerScrollViewObject.SetActive(false);

            titleText.text = "フレンドリスト";

            friendScrollRect.verticalNormalizedPosition = 1f;
            friendListBannerList.ForEach(friendBanner => friendBanner.gameObject.SetActive(false));

            messageText.text = "接続中...";
            base.Show();
        }

        public void ShowFriendList(List<PlayerInfo> playerInfoList, UnityAction<string> callbackRemoveFromfriendButton)
        {
            if (playerInfoList.Count == 0)
            {
                messageText.text = "フレンドがいません。";
                return;
            }
            else
            {
                messageText.text = string.Empty;
            }

            int i = 0;

            foreach (PlayerInfo playerInfo in playerInfoList)
            {
                UnityAction buttonAction = (() =>
                {
                    Main.Instance.ShowDialogueYN(playerInfo.nickName + "をフレンドから削除しますか？", ()=> callbackRemoveFromfriendButton(playerInfo.objectId));
                });

                string killCount = playerInfo.killCount + " Kill";

                if (!string.IsNullOrEmpty(playerInfo.equipCardId))
                {
                    Texture2D cardTexture = CardManager.Instance.GetTextureByCardId(playerInfo.equipCardId);
                    friendListBannerList[i].Set(buttonAction, playerInfo.nickName, killCount, cardTexture);
                }
                else
                {
                    friendListBannerList[i].Set(buttonAction, playerInfo.nickName, killCount);
                }

                i++;

                if (i > 4)
                    break;
            }
        }

        public void ShowOtherPlayertConnecting()
        {
            friendScrollViewObject.SetActive(false);
            otherPlayerScrollViewObject.SetActive(true);

            searchOtherPlayerInputField.text = string.Empty;
            titleText.text = "他のプレイヤー一覧";

            otherPlayerScrollRect.verticalNormalizedPosition = 1f;
            otherPlayerListBannerList.ForEach(friendBanner => friendBanner.gameObject.SetActive(false));

            messageText.text = "接続中...";
            base.Show();
        }

        public void ShowOtherPlayerList(List<PlayerInfo> playerInfoList, UnityAction<PlayerInfo> callbackAddFriendButton)
        {
            if (playerInfoList.Count == 0)
            {
                messageText.text = "対象プレイヤーがいません。";
                return;
            }
            else
            {
                messageText.text = string.Empty;
            }

            int i = 0;

            Debug.Log("PlayerInfoList　" + playerInfoList.Count);
            foreach (PlayerInfo playerInfo in playerInfoList)
            {
                UnityAction buttonAction = (() =>
                {
                    Main.Instance.ShowDialogueYN(playerInfo.nickName + "をフレンドに加えますか？", ()=> callbackAddFriendButton(playerInfo));
                });

                string killCount = playerInfo.killCount + " Kill";

                if(!string.IsNullOrEmpty(playerInfo.equipCardId))
                {
                    Texture2D cardTexture = CardManager.Instance.GetTextureByCardId(playerInfo.equipCardId);
                    otherPlayerListBannerList[i].Set(buttonAction, playerInfo.nickName, killCount, cardTexture);
                }
                else
                {
                    otherPlayerListBannerList[i].Set(buttonAction, playerInfo.nickName, killCount);
                }

                i++;

                if (i > 4)
                    break;
            }
        }

        public string GetSearchUserName()
        {
            return searchOtherPlayerInputField.text;
        }

        public void ShowNoFriend()
        {
            messageText.text = "フレンドがいません。\n「他のフレンド」ボタンから\n追加しましょう。";
        }
    }
}