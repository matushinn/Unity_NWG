using UnityEngine;

namespace Battle
{
    public class CheckAnimationState : StateMachineBehaviour
    {
        /// <summary>
        /// IAnimationEventReceivableを継承するオブジェクトにアニメーションの状態を伝える//
        /// </summary>

        public string stateName;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.GetComponentInParent<IAnimationEventReceivable>().OnAnimationStateEnter(stateName);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.GetComponentInParent<IAnimationEventReceivable>().OnAnimationStateExit(stateName);
        }
    }
}