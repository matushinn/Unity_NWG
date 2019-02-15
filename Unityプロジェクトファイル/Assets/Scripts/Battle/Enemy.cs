using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Battle
{
    public class Enemy : GameActorBase, IBattleSceneState
    {
        private enum EnemyState { Roaming, FollowPlayer, Attack, Dead }

        [SerializeField]
        private EnemyState currentEnemyState = EnemyState.Roaming;

        public float playerFindDistance = 5f;
        public float playerAttackDistance = 1.3f;
        public float attackInterval = 2f;
        private float attackTimer = 0f;

        private Bounds roamingAreaBounds;//行動範囲//

        private Transform target;
        private float defaultMoveSpeed;

        public int AttackPoint = 20;

        public bool isPlayerLive = true;

        public override void Awake()
        {
            base.Awake();
            defaultMoveSpeed = navMeshAgent.speed;

            BattleSceneController.Instance.AddGameStateChangableList(this);
            BattleSceneController.Instance.AddPausable(this);
        }

        public void SetRoamingArea(Bounds bounds)
        {
            roamingAreaBounds = bounds;
        }

        public void SetTarget(Transform transform)
        {
            target = transform;
        }

        public void SetRandomStartPointInRoamingArea()
        {
            Vector3 point;
            if (CheckRandomPointInRoamingArea(roamingAreaBounds, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                this.transform.position = point;
            }
        }

        public override void OnAttackHit(Collider col)
        {
            if (col.tag == "Player")
            {
                OnDead();
            }
        }

        public void Update()
        {
            if (IsPaused)
            {
                return;
            }

            switch (currentEnemyState)
            {
                case EnemyState.Roaming:
                    UpdateRoaming();
                    UpdateWalkAnimation();
                    break;

                case EnemyState.FollowPlayer:
                    UpdateFollowPlayer();
                    UpdateWalkAnimation();
                    break;

                case EnemyState.Attack:
                    UpdateAttack();

                    break;

                case EnemyState.Dead:
                    break;

                default:
                    break;
            }
        }

        private void OnAttack()
        {
            animator.SetTrigger("Attack");
            currentEnemyState = EnemyState.Attack;
            navMeshAgent.speed = 0f;
        }

        private void UpdateAttack()
        {
            navMeshAgent.SetDestination(target.position);
            attackTimer += Time.deltaTime;
            if (attackTimer > attackInterval)
            {
                attackTimer = 0f;
                OnRoaming();
            }
        }

        private void OnFollowPlayer()
        {
            currentEnemyState = EnemyState.FollowPlayer;
        }

        private void UpdateWalkAnimation()
        {
            animator.SetFloat("Walk", navMeshAgent.velocity.sqrMagnitude);
        }

        private void UpdateFollowPlayer()
        {
            if (navMeshAgent.isOnNavMesh == true)
            {
                //ターゲットを追う//
                navMeshAgent.SetDestination(target.transform.position);

                //プレイヤーとの距離が近かったら、Attackモードになる//

                //navMeshAgent.remainingDistanceはゲーム再開時に一瞬0になることがある//

                if (navMeshAgent.remainingDistance < playerAttackDistance)
                {
                    //Debug.Log("navMeshAgent.remainingDistance " + navMeshAgent.remainingDistance);
                    OnAttack();
                }
            }
        }

        public void OnRoaming()
        {
            currentEnemyState = EnemyState.Roaming;
            navMeshAgent.speed = defaultMoveSpeed;
            SetRandomDestination();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            OnRoaming();
        }

        public override void OnDisable()
        {
            if (animator.isInitialized)
            {
                animator.SetTrigger("Idle");
            }

            base.OnDisable();
        }

        private void UpdateRoaming()
        {
            if (navMeshAgent.enabled == true && //nullが出ることがある？
                navMeshAgent.remainingDistance < 1.0f)
            {
                SetRandomDestination();
            }

            //プレイヤーとの距離が近かったら、Followモードになる
            if (isPlayerLive &&
                Vector3.Distance(target.position, transform.position) < playerFindDistance)
            {
                OnFollowPlayer();
            }
        }

        public override void OnDead()
        {
            currentEnemyState = EnemyState.Dead;
            animator.SetTrigger("Death");

            navMeshAgent.speed = 0f;
            navMeshAgent.enabled = false;

            BattleSceneController.Instance.EnemyDead(transform);

            base.OnDead();

            StartCoroutine(Disappear());
        }

        private IEnumerator Disappear()
        {
            float timeCount = 0f;

            while (timeCount < 2f)
            {
                if (!IsPaused)
                {
                    timeCount += Time.deltaTime;
                }

                yield return null;
            }

            enabled = false;
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        private void SetRandomDestination()
        {
            Vector3 point;
            if (CheckRandomPointInRoamingArea(roamingAreaBounds, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);

                if (navMeshAgent.isOnNavMesh == true)
                    navMeshAgent.SetDestination(point);
            }
        }

        public bool CheckRandomPointInRoamingArea(Bounds area, out Vector3 result)
        {
            //3次元のランダムなポイントをbounds内で取り、ナビメッシュの上なら値を返す//
            //最大30回試行する//
            for (int i = 0; i < 30; i++)
            {
                Vector3 center = area.center;

                float x = UnityEngine.Random.Range(center.x - area.extents.x, center.x + area.extents.x);
                float y = UnityEngine.Random.Range(center.y - area.extents.y, center.y + area.extents.y);
                float z = UnityEngine.Random.Range(center.z - area.extents.z, center.z + area.extents.z);

                Vector3 position = new Vector3(x, y, z);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        public void OnBattleStart()
        {
            isPlayerLive = true;
            OnRoaming();
        }

        public void OnPlayerDead()
        {
            isPlayerLive = false;
            OnRoaming();
        }

        public void OnBattleEnd()
        {

        }
    }
}