using UnityEngine;

namespace Battle
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventReceiver : MonoBehaviour
    {
        private IAnimationEventReceivable receiver;

        private void Awake()
        {
            receiver = GetComponentInParent<IAnimationEventReceivable>();
            DisableAttackArea();
        }

        public void EnableAttackArea()
        {
            receiver.EnableAttackArea();
        }

        public void DisableAttackArea()
        {
            receiver.DisableAttackArea();
        }

        public void PlaySE3D(string seName)
        {
            receiver.PlaySE3D(seName);
        }
    }
}