using TSM;
using UnityEngine;

namespace Battle
{
    public class Player : GameActorBase, IBattleSceneState, IMagicAcceptable
    {
        public const int DEFAULT_LIFEPOINT = 100;
        public int currentLifePoint = 0;

        public float moveSpeed = 10f;
        private float currentMoveSpeed;
        public float rotateSpeed = 5f;

        private float currentShieldPoint = 1f;

        public ParticleSystem healParticle;
        public ParticleSystem damageParticle;
        public ParticleSystem cardEffectParticle;

        private enum State { Wait, Walk, Dead, Attack }

        [SerializeField]
        private State currentState = State.Wait;

        public bool isAttackAnimation = false;

        public bool debug_NoDeath = false;

        public BattleSceneController battleSceneController;
        public bool IsUsingCard { get; private set; }
        private float cardEffectiTimer;
        private float cardEffectTime = 20f;

        public override void Awake()
        {
            base.Awake();
#if !UNITY_EDITOR
        debug_NoDeath = false;
#endif
            battleSceneController.AddGameStateChangableList(this);
            battleSceneController.AddPausable(this);
        }

        public override void Start()
        {
            base.Start();

            currentLifePoint = DEFAULT_LIFEPOINT;

            currentMoveSpeed = moveSpeed;
        }

        public void OnBattleStart()
        {
            OnWalk();
        }

        public void OnPlayerDead()
        {
            enabled = false;
        }

        public void OnBattleEnd()
        {
        }

        public override void OnAttackHit(Collider col)
        {
            if (debug_NoDeath)
                return;

            if (col.tag == "Enemy")
            {
                float attackPoint = col.gameObject.GetComponentInParent<Enemy>().AttackPoint;

                attackPoint = (int)(attackPoint * currentShieldPoint);
                Debug.Log("attackPoint" + attackPoint);
                AddLifePoint(-(int)attackPoint);
            }
        }

        public void AddLifePoint(int point)
        {
            currentLifePoint += point;

            if (currentLifePoint > 100)
            {
                currentLifePoint = 100;
            }
            else if (currentLifePoint <= 0)
            {
                currentLifePoint = 0;//UI表示のため0をセットする/
                OnDeath();
            }

            battleSceneController.SetLifeBar(currentLifePoint);
        }

        public void OnDeath()
        {
            currentState = State.Dead;

            navMeshAgent.velocity = Vector3.zero;

            hitArea.enabled = false;
            animator.SetTrigger("Death");

            battleSceneController.StartDeath(transform.localPosition);

            if (healParticle.isPlaying) { healParticle.Stop(); }
            if (damageParticle.isPlaying) { damageParticle.Stop(); }
            if (cardEffectParticle.isPlaying) { cardEffectParticle.Stop(); }
        }

        private void OnWalk()
        {
            currentState = State.Walk;
        }

        public void UpdateWalk()
        {
#if UNITY_IOS || UNITY_ANDROID
            float h = JoyStick.GetAxisHorizontal();
            float v = JoyStick.GetAxisVertical();
#else
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Fire1"))
            {
                OnButtonAttack();
            }
#endif
            //移動//
            Vector3 moveDirection = h * Vector3.right + v * Vector3.forward;
            moveDirection = currentMoveSpeed * moveDirection.normalized;
            navMeshAgent.velocity = moveDirection;

            //回転//
            if (moveDirection.sqrMagnitude > 0.001)
            {
                float step = rotateSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDirection, step, 0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }

            //アニメーション更新//
            float walkAmount = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
            animator.SetFloat("Walk", walkAmount);
        }

        //攻撃ボタン判定//
        public void OnButtonAttack()
        {
            if (currentState == State.Walk)
            {
                currentState = State.Attack;
                animator.SetTrigger("Attack");
            }
        }

        public void OnWait()
        {
            currentState = State.Wait;
            animator.SetTrigger("Idle");

            //操作を離れた瞬間に方向キーを押していいると歩きモ－ションが入りっぱなしになる//
            animator.SetFloat("Walk", 0f);
        }

        public override void OnAnimationStateExit(string name)
        {
            if (name == "Attack" && currentState != State.Dead)
            {
                OnWalk();
            }
        }

        public void Update()
        {
            if (IsPaused)
            {
                return;
            }

            switch (currentState)
            {
                case State.Walk:
                    UpdateWalk();
                    break;
                default:
                    break;
            }

            if (IsUsingCard)
            {
                cardEffectiTimer += Time.deltaTime;
                if (cardEffectiTimer > cardEffectTime)
                {
                    cardEffectiTimer = 0f;
                    ClearCardEffect();
                    IsUsingCard = false;
                }
            }
        }

        public void OnHeal()
        {
            healParticle.Play();
            AddLifePoint(30);
            SoundManager.Instance.PlaySE("Heal");
        }

        public void OnCurse()
        {
            damageParticle.Play();
            AddLifePoint(-30);
            SoundManager.Instance.PlaySE("Damage");
        }

        public void SetCardEffect(CardData.Type type, float point)
        {
            IsUsingCard = true;

            switch (type)
            {
                case CardData.Type.Fast:
                    currentMoveSpeed = moveSpeed * point;
                    break;

                case CardData.Type.Shield:
                    currentShieldPoint = 1f / point;
                    break;
            }

            cardEffectParticle.Play();
        }

        public override void OnPause()
        {
            base.OnPause();

            if (healParticle.isPlaying) { healParticle.Pause(); }
            if (damageParticle.isPlaying) { damageParticle.Pause(); }
            if (cardEffectParticle.isPlaying) { cardEffectParticle.Pause(); }
        }

        public override void OnResume()
        {
            if (healParticle.isPaused) { healParticle.Play(); }
            if (damageParticle.isPaused) { damageParticle.Play(); }
            if (cardEffectParticle.isPaused) { cardEffectParticle.Play(); }

            base.OnResume();
        }

        private void ClearCardEffect()
        {
            currentMoveSpeed = moveSpeed;
            currentShieldPoint = 1f;
            cardEffectParticle.Stop();
        }
    }
}