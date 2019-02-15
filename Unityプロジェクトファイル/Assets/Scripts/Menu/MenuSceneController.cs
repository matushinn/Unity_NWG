using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Communication;
using System;
using TSM;

namespace Menu
{
    public class MenuSceneController : MonoBehaviour
    {
        private UserAuth userAuth;
        private PlayerInfoHandler playerInfoHandler;

        private RankingHandler rankingDataHandler;

        private FileStoreManager fileStoreManager;
        private PushNotificationManager pushNotificationManager;

        public MainMenuUI mainMenuUI;
        public DailyBonusUI dailyBonusUI;
        public RankingUI rankingUI;
        public FriendsUI friendsUI;
        public ScreenShotUI screenShotUI;
        public CardListUI cardListUI;
        public GachaUI gachaUI;
        public AnnouncementsUI announcementsUI;
        public OptionsUI optionsUI;

        public bool isDebugDailyBonus = false;

        private IEnumerator currentCoroutine;

        private CardData currentEquipmentCard;

        private void Awake()
        {
            userAuth = UserAuth.Instance;
            playerInfoHandler = PlayerInfoHandler.Instance;

            rankingDataHandler = RankingHandler.Instance;

            fileStoreManager = FileStoreManager.Instance;
            pushNotificationManager = PushNotificationManager.Instance;
        }

        public void Start()
        {
            OnButtonMainPanel();

            StartCoroutine(OnMenuSequence());
        }

        //メニュー画面開始シーケンス
        private IEnumerator OnMenuSequence()
        {
            Main.Instance.ShowLoadingPanel();

            //バナー画像の更新//
            mainMenuUI.ClearBanner();

            Texture2D bannerTexture2d = fileStoreManager.GetBannerCacheFile();
            if (bannerTexture2d != null)
            {
                mainMenuUI.SetBannerImage(bannerTexture2d);
            }

            //会員管理からユーザーネームを取得//
            mainMenuUI.SetUserName(userAuth.GetUserName());

            //会員管理からニックネーム取得//
            mainMenuUI.SetNickName(userAuth.GetNickName());

            //会員管理からコイン枚数を取得//
            mainMenuUI.SetCoinCount(userAuth.GetCoinCount());

            //会員管理から所持カードリストの確認//
            if(userAuth.GetCardIdList().Count == 0)
            {
                //なければデフォルトカードを付与//
                userAuth.AddCardId("Fast15");
            }

            //PlayerInfoから装備カードを取得//
            yield return playerInfoHandler.FetchOwnDataCoroutine(Main.Instance.ForceToTitle);

            string cardId = playerInfoHandler.GetEquipCardIdFromOwnData();

            if (!string.IsNullOrEmpty(cardId))
            {
                currentEquipmentCard = CardManager.Instance.GetCardDataByCardId(cardId);
                mainMenuUI.SetEquipCardImage(currentEquipmentCard);
            }

            //デイリーボーナスチェック//
            //本日の日付と会員管理の最終更新日付を比較//
            IEnumerator coroutine = userAuth.FetchLoginDateSpan(Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            int day = (int)coroutine.Current;

            if (day == 0)
            {
                mainMenuUI.Show();
            }
            else
            {
                //デイリーボーナス付与ダイヤログ表示//
                dailyBonusUI.Show(day);
            }

            Main.Instance.HideLoadingPanel();
        }

        public void OnButtonMainPanel()
        {
            mainMenuUI.Show();

            friendsUI.Hide();
            rankingUI.Hide();
            screenShotUI.Hide();
            cardListUI.Hide();
            gachaUI.Hide();
            announcementsUI.Hide();
            optionsUI.Hide();
        }

        //デイリーボーナスOK押したときの挙動//
        public void OnButtonGetDailyBonus()
        {
            StartCoroutine(GetDailyBonusCoroutine());
        }

        public IEnumerator GetDailyBonusCoroutine()
        {
            //コインを一枚付与//
            userAuth.AddCoinCount(1);

            //表示を更新//
            mainMenuUI.SetCoinCount(userAuth.GetCoinCount());

            //すぐに保存する//
            yield return userAuth.SaveAsyncCurrentUserCoroutine(Main.Instance.ForceToTitle);

            dailyBonusUI.Hide();

            mainMenuUI.Show();
        }

        public void OnButtonBattle()
        {
            Main.Instance.OnBattle();
        }

        public void OnButtonRankingPanel()
        {
            StartCoroutine(OnRankingPanelCoroutine());
        }

        public IEnumerator OnRankingPanelCoroutine()
        {
            mainMenuUI.Hide();

            rankingUI.Show();

            IEnumerator coroutine = rankingDataHandler.FindKillRankingCoroutine(10);

            yield return StartCoroutine(coroutine);

            rankingUI.SetRankingData((List<RankingInfo>)coroutine.Current);
        }

        public void OnButtonFriendPanel()
        {
            mainMenuUI.Hide();

            ShowFridendList();
        }

        public void ShowFridendList()
        {
            friendsUI.ShowFriendListConnecting();

            StartCoroutine(FetchFriendList());
        }

        private IEnumerator FetchFriendList()
        {
            List<string> friendIdList = userAuth.GetFriendIdList();

            if (friendIdList.Count != 0)
            {
                IEnumerator coroutine = playerInfoHandler.FindByIdListCoroutine(10, friendIdList, Main.Instance.ForceToTitle);

                yield return StartCoroutine(coroutine);

                if (coroutine.Current != null)
                {
                    friendsUI.ShowFriendList((List<PlayerInfo>)coroutine.Current, RemoveFriend);
                }
            }
            else
            {
                friendsUI.ShowNoFriend();
            }

        }

        //フレンドリスト上で「フレンドから削除」をクリックされた場合の処理//
        private void RemoveFriend(string objectId)
        {
            userAuth.RemoveFriend(objectId);
            ShowFridendList();//リフレッシュ//
        }

        public void ShowOtherPlayerList()
        {
            friendsUI.ShowOtherPlayertConnecting();

            StartCoroutine(FetchOtherPlayerList());
        }

        private IEnumerator FetchOtherPlayerList()
        {
            //自分、フレンド以外のユーザーを表示する//
            List<string> ignoreList = userAuth.GetFriendIdList();
            IEnumerator coroutine = playerInfoHandler.FindWithIgnoreIdListCoroutine(10, ignoreList, Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            if (coroutine != null)
            {
                friendsUI.ShowOtherPlayerList((List<PlayerInfo>)coroutine.Current, AddFriend);
            }
        }

        //プレイヤーリスト上で「フレンドに追加」をクリックした場合の処理//
        private void AddFriend(PlayerInfo playerInfo)
        {
            userAuth.AddFriend(playerInfo.objectId);

            //installationがあるユーザーにはフレンドを登録した相手にプッシュ通知//
            if (!string.IsNullOrEmpty(playerInfo.installationObjectId))
            {
                pushNotificationManager.SendPushAddFriend(playerInfo.installationObjectId, userAuth.GetNickName());
            }

            //リストのリフレッシュ//
            ShowOtherPlayerList();
        }

        private IEnumerator SearchPlayerByNickNameCoroutine(string nickname)
        {
            //自分、フレンド以外のプレイヤーを検索する//
            List<string> ignoreList = userAuth.GetFriendIdList();

            IEnumerator coroutine = playerInfoHandler.FindByNickNameCoroutine(nickname, ignoreList, Main.Instance.ForceToTitle);
            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                friendsUI.ShowOtherPlayerList((List<PlayerInfo>)coroutine.Current, AddFriend);
            }
        }

        public void OnButtonSearchPlayerByUserName()
        {
            string userName = friendsUI.GetSearchUserName();

            if (!string.IsNullOrEmpty(userName))
            {
                friendsUI.ShowOtherPlayertConnecting();
                StartCoroutine(SearchPlayerByUserNameCoroutine(userName));
            }
        }

        private IEnumerator SearchPlayerByUserNameCoroutine(string userName)
        {
            //自分、フレンド以外のプレイヤーを検索する//
            List<string> ignoreList = userAuth.GetFriendIdList();

            IEnumerator coroutine = playerInfoHandler.FindByUserNameCoroutine(userName, ignoreList, Main.Instance.ForceToTitle);
            yield return StartCoroutine(coroutine);

            if (coroutine.Current != null)
            {
                friendsUI.ShowOtherPlayerList((List<PlayerInfo>)coroutine.Current, AddFriend);
            }
        }

        public void OnButtonScreenShot()
        {
            //自分以外の全ユーザーのPlayerInfoを取得//
            mainMenuUI.Hide();
            screenShotUI.Show();

            currentCoroutine = FindScreenShot();
            StartCoroutine(currentCoroutine);
        }

        private void StopFetchScreenShot()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        private IEnumerator FindScreenShot()
        {
            IEnumerator coroutine =
                playerInfoHandler.FindWithIgnoreIdListHasScreenShotCoroutine(
                5,
                new List<string>(),
                Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            if (coroutine.Current == null)
            {
                yield break;
            }

            List<PlayerInfo> list = (List<PlayerInfo>)coroutine.Current;

            foreach (PlayerInfo playerInfo in (List<PlayerInfo>)coroutine.Current)
            {
                if (!string.IsNullOrEmpty(playerInfo.screenShotFileName))
                {
                    IEnumerator fileFetchCoroutine
                        = fileStoreManager.FetchTextureCoroutine(
                            playerInfo.screenShotFileName,
                            Main.Instance.ShowErrorDialogue
                            );

                    yield return StartCoroutine(fileFetchCoroutine);

                    Texture2D texture2d
                        = (Texture2D)fileFetchCoroutine.Current;

                    if (texture2d != null)
                    {
                        screenShotUI.SetSceenShotToImage(
                            playerInfo.nickName,
                            texture2d
                            );
                    }
                }
            }
        }

        public void OnButtonBackToTitle()
        {
            StartCoroutine(BackToTitleSequence());
        }

        private IEnumerator BackToTitleSequence()
        {
            Main.Instance.ShowLoadingPanel();

            StopFetchScreenShot();

            yield return userAuth.SaveAsyncCurrentUserCoroutine(Main.Instance.ForceToTitle);

            yield return userAuth.LogOutCoroutine();

            Main.Instance.OnTitle();
        }

        public void OnButtonCardList()
        {
            cardListUI.Show();
            mainMenuUI.Hide();

            List<string> inventoryCardList = userAuth.GetCardIdList();
            cardListUI.Set(inventoryCardList, (string cardid) => StartCoroutine(SelectCardFromInventory(cardid)));
        }

        private IEnumerator SelectCardFromInventory(string cardId)
        {
            currentEquipmentCard = CardManager.Instance.GetCardDataByCardId(cardId);
            mainMenuUI.SetEquipCardImage(currentEquipmentCard);
            OnButtonMainPanel();

            yield return playerInfoHandler.SaveEquipCardIdToOwnData(cardId, Main.Instance.ForceToTitle);
        }

        public void OnButtonGacha()
        {
            gachaUI.Show(GameSettingManager.Instance.GetGachaPrice(), userAuth.GetCoinCount());
            mainMenuUI.Hide();
        }

        public void OnButtonGachaStart()
        {
            if (userAuth.GetCoinCount() >= GameSettingManager.Instance.GetGachaPrice())
            {
                gachaUI.ShowConnecting();
                StartCoroutine(GachaSequence());
            }
            else
            {
                gachaUI.ShowShortfall();
            }
        }

        private IEnumerator GachaSequence()
        {
            string gachaListNumber = "1";

            IEnumerator coroutine = Gacha.Instance.GachaScriptCoroutine(gachaListNumber, userAuth.LocalSavedUserName, Main.Instance.ForceToTitle);

            yield return coroutine;

            string rarelityStr = (string)coroutine.Current;
            //  = rarelityObj.ToString();

            if (!string.IsNullOrEmpty(rarelityStr))
            {
                Debug.Log("rarelityStr " + rarelityStr);
              
                CardData cardData = CardManager.Instance.GetCardDataFromRarelity(gachaListNumber, rarelityStr);

                gachaUI.ShowCard(cardData);
                userAuth.AddCardId(cardData.id);

                userAuth.AddCoinCount(-GameSettingManager.Instance.GetGachaPrice());
                mainMenuUI.SetCoinCount(userAuth.GetCoinCount());
                gachaUI.SetCoinCount(userAuth.GetCoinCount());
                yield return userAuth.SaveAsyncCurrentUserCoroutine(Main.Instance.ForceToTitle);
            }
        }

        public void OnButtonAnnouncements()
        {
            announcementsUI.Show();
            mainMenuUI.Hide();

            StartCoroutine(AnnouncementsSequence());
        }

        IEnumerator AnnouncementsSequence()
        {
            List<AnnouncementInfo> announcementInfoList = new List<AnnouncementInfo>();

            IEnumerator coroutine = AnnouncementHandler.Instance.GetAnnoucementListCoroutine(Main.Instance.ForceToTitle);

            yield return StartCoroutine(coroutine);

            announcementInfoList = (List<AnnouncementInfo>)coroutine.Current;

            if(announcementInfoList.Count != 0)
            {
                announcementsUI.SetAnnounceList(announcementInfoList);
            }
            else
            {
                announcementsUI.NoAnnounce();
            }
        }

        public void OnButtonHideAnnouncementMessage()
        {
            announcementsUI.HideMessage();
        }

        public void OnButtonOptions()
        {
            optionsUI.Show();
            mainMenuUI.Hide();
        }

        public void StopBGM()
        {
            SoundManager.Instance.StopBGM();
        }
        
        public void OnButtonUnderConstruciton()
        {
            Main.Instance.ShowErrorDialogue("現在開発中です。");
        }
    }
}