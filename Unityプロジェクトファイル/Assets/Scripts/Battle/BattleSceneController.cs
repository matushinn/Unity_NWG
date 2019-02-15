using System.Collections.Generic;
using TSM;
using UnityEngine;
using Communication;

namespace Battle
{
    public class BattleSceneController : SingletonMonoBehaviour<BattleSceneController>
    {
        public bool IsGamePaused { get; private set; }

        public BattleMainUI battleMainUI;
        public PauseUI pauseUI;
        public OwnGraveInfoUI ownGraveInfoUI;
        public OthersGraveInfoUI othersGraveInfoUI;
        public LifePointBar lifePointBar;

        private List<IBattleSceneState> battleSceneStateObjects = new List<IBattleSceneState>();
        private List<IPauseable> pausableList = new List<IPauseable>();

        public Player player;

        //ゲーム中でタップされたGraveObject//
        public GraveObject selectedGraveObject;

        private GraveNameText selectedGraveNameText;

        public ItemSpawner itemManager;

        private BattleSceneSequences battleSceneSequences;
        private GameSettingManager gameSettingManager;
        private GraveInfoHandler graveInfoHandler;

        public int ThisGameCoinNum { get; private set; }
        public int ThisGameKillCount { get; private set; }

        private int currentEnemuDropCoinNum = 1;
        public CardData MyCardData;
        public CardData FriendCardData;

        public void AddGameStateChangableList(IBattleSceneState bss)
        {
            battleSceneStateObjects.Add(bss);
        }

        public void AddPausable(IPauseable obj)
        {
            pausableList.Add(obj);
        }

        private void Awake()
        {
            battleSceneSequences = GetComponent<BattleSceneSequences>();
            graveInfoHandler = GraveInfoHandler.Instance;
            gameSettingManager = GameSettingManager.Instance;
        }

        public void Start()
        {

            //バーチャルパッドを有効化//
#if UNITY_IOS || UNITY_ANDROID
            battleMainUI.SetActiveVirtualPads(true);
#else
            battleMainUI.SetActiveVirtualPads(false);
#endif
            //ゲーム設定ファイルから設定読み込み//
            if (gameSettingManager.IsLoaded)
            {
                currentEnemuDropCoinNum = gameSettingManager.GetEnemyDropCoinNum();
            }

            StartCoroutine(battleSceneSequences.GameStartSequence());
        }

        public void HideOwnGraveStatus()
        {
            ownGraveInfoUI.Hide();
            OnBattleStart();
        }

        public void SetLifeBar(int point)
        {
            lifePointBar.SetLifeBar(point);
        }

        public void StartDeath(Vector3 position)
        {
            StartCoroutine(battleSceneSequences.PlayerDeathSequence(position));
        }

        private void Update()
        {
            if (IsGamePaused)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    OnGameResume();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    OnGamePause();
                }
            }
        }

        public void OnBattleStart()
        {
            SoundManager.Instance.PlayBGM("Dysipe_1_loop");

            battleSceneStateObjects.ForEach(st => st.OnBattleStart());
            battleMainUI.Show();
        }

        public void OnPlayerDead()
        {
            battleSceneStateObjects.ForEach(sc => sc.OnPlayerDead());
            battleMainUI.Hide();
        }

        public string GetPlayerDeathMessage()
        {
            return battleMainUI.GetPlayerDeathMessage();
        }

        public void OnGamePause()
        {
            battleMainUI.Hide();
            pauseUI.Show();

            InnerPause();

            IsGamePaused = true;
        }

        public void InnerPause()
        {
            pausableList.ForEach(p => p.OnPause());
            battleMainUI.PauseGraveNameText();
        }

        public void OnGameResume()
        {
            battleMainUI.Show();
            pauseUI.Hide();

            InnnerResume();

            IsGamePaused = false;
        }

        public void ShowOwnGraveInfoSettingUI(GraveInfo graveInfo)
        {
            //ダイヤログ表示//
            ownGraveInfoUI.Show(graveInfo);
        }

        public void HideOwnGraveInfoSettingUI()
        {
            ownGraveInfoUI.Hide();
        }

        public void InnnerResume()
        {
            pausableList.ForEach(p => p.OnResume());
            battleMainUI.ResumeGraveNameText();
        }

        //ゲーム開始時の墓調べ数ボーナスと、Coin.csから呼ばれる//
        public void AddCoin(int num)
        {
            //総数を合計//
            ThisGameCoinNum += num;

            //今回プレイの数を表示//
            battleMainUI.SetCoinCounter(ThisGameCoinNum);
        }

        public void EnemyDead(Transform enemyDeadPosition)
        {
            //殺害総数をカウントアップ//
            ThisGameKillCount++;

            //今回プレイの殺害数表示の更新//
            battleMainUI.SetKillCounter(ThisGameKillCount);

            //currentEnemuDropCoinNumの回数分コインを輩出//
            for (int i = 0; i < currentEnemuDropCoinNum; i++)
            {
                itemManager.SpawnCoin(enemyDeadPosition.position);
            }
        }

        public void DisableBattleUIButtons()
        {
            battleMainUI.DisableButtons();
        }

        public void ShowSelectedGraveObjectsInfo(GraveNameText selectedGraveNameText)
        {
            selectedGraveObject = selectedGraveNameText.ReratedGraveObject;
            this.selectedGraveNameText = selectedGraveNameText;

            string deathMessage = selectedGraveObject.graveInfo.deathMessage;
            string nickName = " - " + selectedGraveObject.graveInfo.nickName;

            othersGraveInfoUI.Show(deathMessage, nickName);

            InnerPause();
        }
        

        public void AcceptGraveCurse()
        {
            switch (selectedGraveObject.graveInfo.curseType)
            {
                case GraveInfo.CurseType.None:
                    break;

                case GraveInfo.CurseType.Damage:
                    player.OnCurse();
                    break;

                case GraveInfo.CurseType.Heal:
                    player.OnHeal();

                    break;

                default:
                    break;
            }

            graveInfoHandler.CountUpGraveUsedCounter(selectedGraveObject.graveInfo.objectId);

            selectedGraveNameText.Clear();//対象のGraveTextをクリアする//

            selectedGraveObject.enabled = false;//Update処理を止める//
            selectedGraveObject = null;

            CancelGraveCurse();
        }

        public void CancelGraveCurse()
        {
            othersGraveInfoUI.Hide();
            InnnerResume();
        }

        public void OnButtonReturnToMenu()
        {
            //ギブアップの場合はセーブせずにメニューに戻る///
            Main.Instance.ShowLoadingPanel();

            SoundManager.Instance.StopBGMWithFade(1f);

            Main.Instance.OnMenu();
        }

        public void OnButtonSaveOwnGraveInfo(string curseTypeStr)
        {
           string message = battleMainUI.GetPlayerDeathMessage();

            //禁止ワードチェック//
            if(Main.Instance.HasBadWord(message))
            {
                Main.Instance.ShowErrorDialogue("投稿できない文字が含まれています");
                return;
            }

            StartCoroutine(battleSceneSequences.SaveOwnGraveInfoSequense(curseTypeStr));
        }

        public void OnButtonUseMyCard()
        {
            if(player.IsUsingCard)
            {

            }else
            {
                string effectName = MyCardData.name + " x " + MyCardData.point;
                battleMainUI.CardTextEffect(effectName);

                player.SetCardEffect(MyCardData.type, MyCardData.point);

                battleMainUI.SetDisableMyCard();
            }
        }

        public void OnButtonUseFriendCard()
        {
            if (player.IsUsingCard)
            {

            }
            else
            {
                string effectName = FriendCardData.name + " x " + FriendCardData.point;
                battleMainUI.CardTextEffect(effectName);

                player.SetCardEffect(FriendCardData.type, FriendCardData.point);

                battleMainUI.SetDisableFriendCard();
            }
        }
    }
}