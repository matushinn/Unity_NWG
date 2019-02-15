using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Battle
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject coinPrefab;
        public int maxCoinNum = 10;
        private List<Coin> coinList = new List<Coin>();

        private void Awake()
        {
            for (int i = 0; i < maxCoinNum; i++)
            {
                Coin coin = Instantiate(coinPrefab, transform.position,  Quaternion.identity, transform).GetComponent<Coin>();
                coinList.Add(coin);
                coin.gameObject.SetActive(false);
            }
        }

        public void SpawnCoin(Vector3 position, int value = 1)
        {
            Coin coin = coinList.FirstOrDefault(c => c.gameObject.activeSelf == false);

            if (coin == null)
                return;

            coin.Spawn(position, value);
        }
    }
}