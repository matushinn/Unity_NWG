using UnityEngine;
using System;

namespace Battle
{
    public class MainCamera : MonoBehaviour
    {
        public Transform playerObject;
        private float originY;
        private float margineZ;

        private bool takeScreenShotFlag = false;

        private Action<Texture2D> callback;

        private void Awake()
        {
            originY = this.gameObject.transform.position.y;
            margineZ = playerObject.position.z - this.gameObject.transform.position.z;
        }

        private void Start()
        {
            TSM.SoundManager.Instance.SetAudioListenerFollower(this.transform);
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(playerObject.position.x, originY, playerObject.position.z - margineZ);
        }

        public void TakeScreenShotOnNextFlag(Action<Texture2D> callback)
        {
            takeScreenShotFlag = true;
            this.callback = callback;
        }

        public void OnPostRender()
        {
            if (takeScreenShotFlag)
            {
                var tex = new Texture2D(Screen.width, Screen.height);
                tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                tex.Apply();

                callback(tex);

                takeScreenShotFlag = false;
            }
        }
    }
}