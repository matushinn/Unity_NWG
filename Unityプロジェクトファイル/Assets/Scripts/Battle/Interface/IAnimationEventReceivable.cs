/// <summary>
/// AnimationEventReceiverのイベントを受け取ることができる//
/// </summary>
namespace Battle
{
    public interface IAnimationEventReceivable
    {
        void EnableAttackArea();

        void DisableAttackArea();

        void PlaySE3D(string seName);

        void OnAnimationStateEnter(string name);

        void OnAnimationStateExit(string name);
    }
}