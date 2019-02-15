//メインゲームのステート変更に影響するもの//
namespace Battle
{
    public interface IBattleSceneState
    {
        void OnBattleStart();
        void OnPlayerDead();
        void OnBattleEnd();
    }

}