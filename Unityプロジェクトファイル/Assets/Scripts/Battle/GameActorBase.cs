using System.Collections.Generic;
using TSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

/// <summary>
/// Player, Enemy, NPC Base
/// </summary>

namespace Battle
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class GameActorBase : MonoBehaviour, IDamageable, IAnimationEventReceivable, IPauseable
    {
        public Animator animator;
        protected NavMeshAgent navMeshAgent;
        public List<SkinnedMeshRenderer> bodyRendererList;
        protected AudioSource audioSource;
        public AudioMixerGroup mixerGroupSE;

        public OnTriggerEnterDetector hitArea;
        public Collider attackArea;

        public bool IsPaused { get; private set; }

        public virtual void Awake()
        {
            hitArea.callBack = OnAttackHit;

            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.outputAudioMixerGroup = mixerGroupSE;

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            navMeshAgent = GetComponent<NavMeshAgent>();

            if (bodyRendererList.Count == 0)
            {
                bodyRendererList.Add(GetComponentInChildren<SkinnedMeshRenderer>());
            }
        }

        public virtual void Start()
        {
        }

        public void EnableAttackArea()
        {
            attackArea.enabled = true;
        }

        public void DisableAttackArea()
        {
            attackArea.enabled = false;
        }

        public void PlaySE3D(string seName)
        {
            SoundManager.Instance.PlaySE3D(audioSource, seName, audioSource.volume);
        }

        public virtual void OnAnimationStateEnter(string name)
        {
        }

        public virtual void OnAnimationStateExit(string name)
        {
        }

        public virtual void OnAttackHit(Collider col)
        {
        }

        public virtual void OnEnable()
        {
            bodyRendererList.ForEach(b => b.enabled = true);
            hitArea.enabled = true;
            attackArea.enabled = true;

            navMeshAgent.enabled = true;
        }

        //完全に消える//
        public virtual void OnDisable()
        {
            bodyRendererList.ForEach(b => b.enabled = false);
            hitArea.enabled = false;
            attackArea.enabled = false;

            navMeshAgent.enabled = false;
        }

        //判定無効化＋死にモーション//
        public virtual void OnDead()
        {
            hitArea.enabled = false;
            attackArea.enabled = false;
        }

        public virtual void OnPause()
        {
            IsPaused = true;
            animator.speed = 0f;
            hitArea.enabled = false;
            audioSource.Pause();

            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.velocity = Vector3.zero;

#if UNITY_5_6_OR_NEWER
                navMeshAgent.isStopped = true;
#else
                navMeshAgent.Stop();
#endif
            }
        }

        public virtual void OnResume()
        {
            IsPaused = false;
            animator.speed = 1f;
            hitArea.enabled = true;
            audioSource.UnPause();

            if (navMeshAgent.isOnNavMesh)
            {
#if UNITY_5_6_OR_NEWER
                navMeshAgent.isStopped = false;
#else
                navMeshAgent.Resume();
#endif
            }
        }
    }
}