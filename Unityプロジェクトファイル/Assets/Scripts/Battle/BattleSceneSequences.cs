using Communication;
using System.Collections;
using System.Collections.Generic;
using TSM;
using UnityEngine;

/// <summary>
/// ゲーム中の非同期処理
/// </summary>
///
namespace Battle
{
    public class BattleSceneSequences : MonoBehaviour
    {
        private BattleSceneController battleSceneController;
        private UserAuth userAuth;
        private GraveInfoHandler graveInfoHandler;

        private RankingHandler rankingDataHandler;

        public GraveObjectsManager graveObjectManager;
        private PlayerInfoHandler playerInfoHandler;

        public BattleMainUI battleMainUI;
        public SetOwnGraveUI setOwnGraveUI;

        private void Awake()
        {
            battleSceneController = BattleSceneController.Instance;
            userAuth = UserAuth.Instance;
            graveInfoHandler = GraveInfoHandler.Instance;
            playerInfoHandler = PlayerInfoHandler.Instance;
            rankingDataHandler = RankingHandler.Instance;
        }

        public void GameStart()
        {
            StartCoroutine(GameStartSequence());
        }

        public IEnumerator GameStartSequence()
        {
            SoundManager.Instance.StopBGM();

            //最新10件までの墓情報を入手する..
            IEnumerator coroutineGrave = graveInfoHandler.FindGraveInfoListCoroutine(10);
            yield return StartCoroutine(coroutineGrave);

            //GraveInfoをもとにゲーム内に墓を配置する//
            List<GraveInfo> graveInfoList = (List<GraveInfo>)coroutineGrave.Current;
            graveObjectManager.InstallationGraveList(graveInfoList);

            //PlayerInfoHandlerから装備カードの情報を取得する//
            string myCardId = playerInfoHandler.GetEquipCardIdFromOwnData();

            if (!string.IsNullOrEmpty(myCardId))
            {
                battleSceneController.MyCardData = CardManager.Instance.GetCardDataByCardId(myCardId);
                Texture2D myCardTexture = CardManager.Instance.GetTextureByCardId(myCardId);
                battleMainUI.SetMyCardImage(myCardTexture);
            }
            else
            {
                battleMainUI.SetDisableMyCard();
            }

            //Friendリストからカード情報を取得する//
            List<string> friendIdList = userAuth.GetFriendIdList();

            if (friendIdList.Count != 0)
            {
                IEnumerator coroutineFriend = playerInfoHandler.FindByIdListCoroutine(10, friendIdList, Main.Instance.ForceToTitle);

                yield return StartCoroutine(coroutineFriend);

                List<PlayerInfo> friendList = (List<PlayerInfo>)coroutineFriend.Current;

                //データ削除などで取得できない場合がある//
                if (friendList.Count != 0)
                {

                    PlayerInfo randomFriend = friendList.GetRandom();

                    battleSceneController.FriendCardData = CardManager.Instance.GetCardDataByCardId(randomFriend.equipCardId);

                    Texture2D friendCardTexture = CardManager.Instance.GetTextureByCardId(battleSceneController.FriendCardData.id);

                    battleMainUI.SetFriendCardImageAndName(friendCardTexture, randomFriend.nickName);

                }else
                {
                    battleMainUI.SetDisappearFriendCard();
                }

            }
            else
            {
                battleMainUI.SetDisappearFriendCard();
            }

            //プレイヤーの墓データがある場合は、ゲーム開始前にダイヤログを表示する//
            if (userAuth.HasPlayerLastGraveObject)
            {
                IEnumerator coroutine2 = graveInfoHandler.FetchGraveInfoByObjectRefCoroutine(userAuth.GetPlayerLastGraveObject());

                yield return StartCoroutine(coroutine2);

                GraveInfo graveInfo = (GraveInfo)coroutine2.Current;

                battleSceneController.ShowOwnGraveInfoSettingUI(graveInfo);

                //ほかのプレイヤーに墓をチェックされた回数分、コインを追加//
                battleSceneController.AddCoin(graveInfo.checkCounter);
            }
            else
            {
                battleSceneController.OnBattleStart();
            }

            Main.Instance.HideLoadingPanel();
        }

        public IEnumerator PlayerDeathSequence(Vector3 playerDeathPosition)
        {
            playerDeathPosition.y = 0;

            SoundManager.Instance.StopBGMWithFade();

            //お墓情報のUIボタンを無効化する//
            battleSceneController.DisableBattleUIButtons();

            yield return new WaitForSeconds(2f);

            battleSceneController.OnPlayerDead();

            graveObjectManager.InstallationTempGrave(playerDeathPosition);
            setOwnGraveUI.Show();
        }

        public IEnumerator SaveOwnGraveInfoSequense(string curseTypeStr)
        {
            Main.Instance.ShowLoadingPanel();

            //墓の情報をデータストアに保存する
            string nickName = userAuth.GetNickName();

            string deathMessage = battleSceneController.GetPlayerDeathMessage();
            if (string.IsNullOrEmpty(deathMessage))
            {
                deathMessage = "何も刻まれていない";
            }

            Vector3 tempGravePosition = graveObjectManager.GetTempGravePosition();

            IEnumerator coroutine = graveInfoHandler.SaveGraveInfoCoroutine(nickName, deathMessage, curseTypeStr, tempGravePosition);

            yield return StartCoroutine(coroutine);

            userAuth.SetOwnGraveObjectRef((NCMB.NCMBObject)coroutine.Current);

            //プレイヤーの情報を保存する//
            //会員管理へのセーブ（killCount, CoinCountの加算)//
            userAuth.AddCoinCount(battleSceneController.ThisGameCoinNum);
            userAuth.AddKillCount(battleSceneController.ThisGameKillCount);
            yield return userAuth.SaveAsyncCurrentUserCoroutine(Main.Instance.ForceToTitle);

            //キルランキングに「このプレイ」のキル数を保存//
            yield return rankingDataHandler.SaveKillDataCoroutine(nickName, battleSceneController.ThisGameKillCount);

            //PlayerInfoへ累計キルの値を更新//
            int killCount = userAuth.GetKillCount();
            yield return playerInfoHandler.SaveKillCountToOwnData(killCount, Main.Instance.ForceToTitle);

            Main.Instance.OnMenu();
        }
    }
}