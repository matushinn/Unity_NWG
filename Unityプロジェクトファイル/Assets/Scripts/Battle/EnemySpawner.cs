using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public int maxEnemyNum;
        public List<Collider> roamingAreaList = new List<Collider>();

        public Player player;
        public BattleSceneController battleController;

        private List<Enemy> enemyList = new List<Enemy>();

        private void Awake()
        {
            //Enemyのインスタンスを作成する//
            for (int i = 0; i < maxEnemyNum; i++)
            {
                Enemy enemy = Instantiate(enemyPrefab, transform).GetComponent<Enemy>();

                enemy.SetTarget(player.transform);
                enemy.enabled = false;
                enemyList.Add(enemy);
            }
        }

        private void Start()
        {
            for (int i = 0; i < maxEnemyNum; i++)
            {
                SpawnEnemy(roamingAreaList.GetRandom().bounds);
            }
        }

        private void Update()
        {
            //倒されたEnemyが居たら自動的にスポーンする//
            if (enemyList.Any(e => e.enabled == false))
            {
                SpawnEnemyOnFarestAreaWithTarget(player.transform.position);
            }
        }

        //Playerから最も遠いRoamingAreaにリスポーンする//
        public void SpawnEnemyOnFarestAreaWithTarget(Vector3 playerPosition)
        {
            Bounds farestRoamingArea = new Bounds();
            float farestDistance = 0f;
            foreach (Collider roamingArea in roamingAreaList)
            {
                float distance = Vector3.Distance(playerPosition, roamingArea.bounds.center);

                if (distance > farestDistance)
                {
                    farestDistance = distance;
                    farestRoamingArea = roamingArea.bounds;
                }
            }

            SpawnEnemy(farestRoamingArea);
        }

        private void SpawnEnemy(Bounds bounds)
        {
            Enemy enemy = enemyList.FirstOrDefault(e => e.enabled == false);

            if (enemy == null)
                return;

            enemy.SetRoamingArea(bounds);
            enemy.SetRandomStartPointInRoamingArea();
            enemy.enabled = true;
        }
    }
}