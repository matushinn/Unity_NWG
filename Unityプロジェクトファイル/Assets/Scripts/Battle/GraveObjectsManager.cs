using System.Collections.Generic;
using System.Linq;
using TSM;
using UnityEngine;

namespace Battle
{
    public class GraveObjectsManager : MonoBehaviour
    {
        public GameObject gravePrefab;

        private GameObject tempGraveInstance;//Player死亡時に一時的に設置する墓//

        public int graveMaxNum = 20;
        private List<GraveObject> graveObjectsList = new List<GraveObject>();

        public BattleMainUI battleMainUI;
        public Player player;

        private void Awake()
        {
            tempGraveInstance = GenerateDisabledGameObject(gravePrefab, "TempGrave");
            tempGraveInstance.GetComponent<GraveObject>().enabled = false;//Update処理をとめる//

            List<GameObject> graveInstanceList = InstantiateObjectsPool(gravePrefab, graveMaxNum);
            graveObjectsList = graveInstanceList.Select(g => g.GetComponent<GraveObject>()).ToList();
        }

        public void InstallationGraveList(List<GraveInfo> graveInfoList)
        {
            foreach (GraveInfo graveInfo in graveInfoList)
            {
                InstallationGrave(graveInfo);
            }
        }

        public void InstallationGrave(GraveInfo graveInfo)
        {
            GraveObject graveObject = graveObjectsList.FirstOrDefault(obj => obj.gameObject.activeSelf == false);

            if (graveObject == null)
            {
                Debug.Log("Grave Instance Limit Over");
            }
            else
            {
                graveObject.Set(graveInfo, player.transform, battleMainUI.ShowGraveNickNameInField);
                graveObject.gameObject.SetActive(true);
            }
        }

        public void InstallationTempGrave(Vector3 position)
        {
            SoundManager.Instance.PlaySE("GravePutOn");

            tempGraveInstance.transform.position = position;
            tempGraveInstance.SetActive(true);
        }

        public Vector3 GetTempGravePosition()
        {
            return tempGraveInstance.transform.position;
        }

        private List<GameObject> InstantiateObjectsPool(GameObject prefab, int num)
        {
            List<GameObject> gameObjectList = new List<GameObject>();

            for (int i = 0; i < num; i++)
            {
                gameObjectList.Add(GenerateDisabledGameObject(prefab, prefab.name + i));
            }

            return gameObjectList;
        }

        private GameObject GenerateDisabledGameObject(GameObject prefab, string name)
        {
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.name = name;
            obj.transform.parent = this.gameObject.transform;
            obj.SetActive(false);
            return obj;
        }
    }
}