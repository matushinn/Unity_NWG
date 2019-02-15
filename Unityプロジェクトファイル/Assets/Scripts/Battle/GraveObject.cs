using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Battle
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class GraveObject : MonoBehaviour
    {
        private Transform playerTransform;
        public Transform graveMessagePoint;

        public GraveInfo graveInfo;

        private UnityAction<GraveObject> closePlayerCallback;

        public void Set(GraveInfo graveInfo, Transform playerTransform, UnityAction<GraveObject> closePlayerCallback)
        {
            this.graveInfo = graveInfo;
            this.playerTransform = playerTransform;
            this.closePlayerCallback = closePlayerCallback;

            transform.position = graveInfo.position;
        }

        void Update()
        {
            if (Vector3.Distance(playerTransform.position, this.transform.position) < 2f)
            {
                closePlayerCallback(this);
            }
        }
    }
}